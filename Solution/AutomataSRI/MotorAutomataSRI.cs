using System;
using System.Collections.Generic;
using BusinessLogicLayer;
using BusinessObjects;
using Packages;
using Services;

namespace AutomataSRI
{
    public class MotorAutomataSRI
    {
        private ConfigAutomata _config;
        private ClasificadorMensajeSRI _clasificador;
        private int _procesados;
        private int _autorizados;
        private int _alertas;
        private int _ejecucionId;

        public void Ejecutar()
        {
            _config = ConfigAutomata.Cargar();

            if (_config == null)
            {
                Log("Sin config automatasri (par_empresa=1, par_id='automatasri'). Abortando.");
                return;
            }

            if (!_config.activo)
            {
                Log("Automata inactivo en config. Abortando.");
                return;
            }

            _clasificador = ClasificadorMensajeSRI.CargarDesdeDB();

            List<Empresa> empresas = EmpresaBLL.GetAll(
                new WhereParams("emp_estado = {0}", 1), "");

            Log("Empresas activas: " + empresas.Count + (_config.simulacion ? " [SIMULACION]" : ""));

            foreach (Empresa empresa in empresas)
            {
                try
                {
                    ProcesarEmpresa(empresa);
                }
                catch (Exception ex)
                {
                    AutomataSRILog.Error("Error procesando empresa " + empresa.emp_codigo, ex);
                }
            }
        }

        private void ProcesarEmpresa(Empresa empresa)
        {

            _procesados = 0;
            _autorizados = 0;
            _alertas = 0;

            Log("--- Empresa " + empresa.emp_codigo + " (" + empresa.emp_nombre + ") " + (_config.simulacion ? "[SIMULACION]" : "") + " ---");

            AutomataEjecucion ejecucion = new AutomataEjecucion
            {
                ae_empresa = empresa.emp_codigo,
                ae_fecha_inicio = DateTime.Now,
                ae_simulacion = _config.simulacion
            };
            _ejecucionId = AutomataEjecucionBLL.InsertIdentity(ejecucion);

            LimpiarLogsAntiguos(empresa.emp_codigo);

            ProcesarEnviados(empresa.emp_codigo);
            ProcesarRecibidos(empresa.emp_codigo);
            ProcesarDevueltos(empresa.emp_codigo);
            ProcesarNoAutorizados(empresa.emp_codigo);

            ejecucion.ae_id = _ejecucionId;
            ejecucion.ae_id_key = _ejecucionId;
            ejecucion.ae_fecha_fin = DateTime.Now;
            ejecucion.ae_procesados = _procesados;
            ejecucion.ae_autorizados = _autorizados;
            ejecucion.ae_alertas = _alertas;
            AutomataEjecucionBLL.Update(ejecucion);

            Log("  Resumen: procesados=" + _procesados + " autorizados=" + _autorizados + " alertas=" + _alertas);
        }

        // -------------------------------------------------------
        // ENVIADOS: verificar si el SRI recibio
        // -------------------------------------------------------
        private void ProcesarEnviados(int empresa)
        {
            DateTime desde = DateTime.Now.AddDays(-_config.dias_atras);
            DateTime limiteEnvio = DateTime.Now.AddMinutes(-_config.minutos_espera_enviado);

            List<Comprobante> lista = ComprobanteBLL.GetAll(new WhereParams(
                "com_empresa = {0} AND com_estado = {1} AND com_fecha >= {2} AND com_fechaenvia <= {3} AND (com_reintentos IS NULL OR com_reintentos < {4})",
                empresa,
                (int)Enums.EstadoComprobante.ENVIADO,
                desde,
                limiteEnvio,
                _config.max_reintentos_enviado), "");

            Log("  Enviados pendientes: " + lista.Count);

            foreach (Comprobante com in lista)
            {
                int reintento = (com.com_reintentos ?? 0) + 1;
                _procesados++;
                try
                {
                    if (_config.simulacion)
                    {
                        GrabarLog(com, "REENVIAR", "SIMULACION", "Modo simulacion activo");
                        continue;
                    }

                    Log("    " + com.com_numero + " reintento=" + reintento);
                    Offline.SendComprobanteSync(com);

                    var comActual = RecargarComprobante(com);
                    ActualizarReintentos(comActual, reintento);

                    string resultado = reintento >= _config.max_reintentos_enviado ? "LIMITE" : "OK";
                    if (comActual.com_estado == (int)Enums.EstadoComprobante.AUTORIZADO)
                    {
                        _autorizados++;
                        resultado = "OK";
                    }

                    GrabarLog(com, "REENVIAR", resultado, comActual.com_mensaje, comActual.com_estado);
                }
                catch (Exception ex)
                {
                    AutomataSRILog.Error("ProcesarEnviados " + com.com_numero, ex);
                    GrabarLog(com, "REENVIAR", "ERROR", ex.Message);
                }
            }
        }

        // -------------------------------------------------------
        // RECIBIDOS: consultar autorizacion
        // -------------------------------------------------------
        private void ProcesarRecibidos(int empresa)
        {
            DateTime desde = DateTime.Now.AddDays(-_config.dias_atras);
            DateTime limiteRecibido = DateTime.Now.AddMinutes(-_config.minutos_espera_recibido);

            List<Comprobante> lista = ComprobanteBLL.GetAll(new WhereParams(
                "com_empresa = {0} AND com_estado = {1} AND com_fecha >= {2} AND com_fecharespuesta <= {3}",
                empresa,
                (int)Enums.EstadoComprobante.RECIBIDO,
                desde,
                limiteRecibido), "");

            Log("  Recibidos pendientes: " + lista.Count);

            foreach (Comprobante com in lista)
            {
                _procesados++;
                try
                {
                    if (_config.simulacion)
                    {
                        GrabarLog(com, "VERIFICAR", "SIMULACION", "Modo simulacion activo");
                        continue;
                    }

                    Log("    " + com.com_numero + " verificando...");
                    Offline.VerifyComprobanteSync(com);

                    var comActual = RecargarComprobante(com);
                    ActualizarReintentos(comActual, (comActual.com_reintentos ?? 0) + 1);

                    if (comActual.com_estado == (int)Enums.EstadoComprobante.AUTORIZADO)
                        _autorizados++;

                    GrabarLog(com, "VERIFICAR", "OK", comActual.com_mensaje, comActual.com_estado);
                }
                catch (Exception ex)
                {
                    AutomataSRILog.Error("ProcesarRecibidos " + com.com_numero, ex);
                    GrabarLog(com, "VERIFICAR", "ERROR", ex.Message);
                }
            }
        }

        // -------------------------------------------------------
        // DEVUELTOS: clasificar mensaje y actuar
        // -------------------------------------------------------
        private void ProcesarDevueltos(int empresa)
        {
            DateTime desde = DateTime.Now.AddDays(-_config.dias_atras);

            List<Comprobante> lista = ComprobanteBLL.GetAll(new WhereParams(
                "com_empresa = {0} AND com_estado = {1} AND com_fecha >= {2}",
                empresa,
                (int)Enums.EstadoComprobante.DEVUELTO,
                desde), "");

            Log("  Devueltos pendientes: " + lista.Count);

            foreach (Comprobante com in lista)
            {
                _procesados++;
                ProcesarSegunMensaje(com, (int)Enums.EstadoComprobante.DEVUELTO);
            }
        }

        // -------------------------------------------------------
        // NO AUTORIZADOS: clasificar mensaje y actuar
        // -------------------------------------------------------
        private void ProcesarNoAutorizados(int empresa)
        {
            DateTime desde = DateTime.Now.AddDays(-_config.dias_atras);

            List<Comprobante> lista = ComprobanteBLL.GetAll(new WhereParams(
                "com_empresa = {0} AND com_estado = {1} AND com_fecha >= {2}",
                empresa,
                (int)Enums.EstadoComprobante.NOAUTORIZADO,
                desde), "");

            Log("  NoAutorizados pendientes: " + lista.Count);

            foreach (Comprobante com in lista)
            {
                _procesados++;
                ProcesarSegunMensaje(com, (int)Enums.EstadoComprobante.NOAUTORIZADO);
            }
        }

        // -------------------------------------------------------
        // Lógica compartida Devueltos / NoAutorizados
        // -------------------------------------------------------
        private void ProcesarSegunMensaje(Comprobante com, int estado)
        {
            AccionSRI accion = _clasificador.Clasificar(com.com_mensaje, estado);
            int reintento = (com.com_reintentos ?? 0) + 1;

            try
            {
                switch (accion)
                {
                    case AccionSRI.REENVIAR:
                        if (reintento > _config.max_reintentos_devuelto)
                        {
                            _alertas++;
                            Log("    LIMITE " + com.com_numero + " reintento=" + reintento);
                            GrabarLog(com, "REENVIAR", "LIMITE", "Max reintentos alcanzado");
                            return;
                        }
                        if (_config.simulacion)
                        {
                            GrabarLog(com, "REENVIAR", "SIMULACION", "Modo simulacion activo");
                            return;
                        }
                        Log("    REENVIAR " + com.com_numero + " reintento=" + reintento);
                        Offline.SendComprobanteSync(com);
                        com = RecargarComprobante(com);
                        ActualizarReintentos(com, reintento);
                        if (com.com_estado == (int)Enums.EstadoComprobante.AUTORIZADO)
                            _autorizados++;
                        GrabarLog(com, "REENVIAR", "OK", com.com_mensaje, com.com_estado);
                        break;

                    case AccionSRI.VERIFICAR:
                        if (_config.simulacion)
                        {
                            GrabarLog(com, "VERIFICAR", "SIMULACION", "Modo simulacion activo");
                            return;
                        }
                        Log("    VERIFICAR " + com.com_numero);
                        Offline.VerifyComprobanteSync(com);
                        com = RecargarComprobante(com);
                        ActualizarReintentos(com, reintento);
                        if (com.com_estado == (int)Enums.EstadoComprobante.AUTORIZADO)
                            _autorizados++;
                        GrabarLog(com, "VERIFICAR", "OK", com.com_mensaje, com.com_estado);
                        break;

                    case AccionSRI.IGNORAR:
                        GrabarLog(com, "IGNORAR", "OK", "Ignorado por regla");
                        break;

                    case AccionSRI.ALERTAR:
                    default:
                        _alertas++;
                        Log("    ALERTA " + com.com_numero + " | " + com.com_mensaje);
                        break;
                }
            }
            catch (Exception ex)
            {
                AutomataSRILog.Error("ProcesarSegunMensaje " + com.com_numero, ex);
                GrabarLog(com, accion.ToString(), "ERROR", ex.Message);
            }
        }

        // -------------------------------------------------------
        // Helpers
        // -------------------------------------------------------
        private static Comprobante RecargarComprobante(Comprobante com)
        {
            try
            {
                List<Comprobante> lista = ComprobanteBLL.GetAll(
                    new WhereParams("com_numero = {0} AND com_empresa = {1}", com.com_numero, com.com_empresa), "");
                return lista.Count > 0 ? lista[0] : com;
            }
            catch { return com; }
        }

        private static void ActualizarReintentos(Comprobante com, int reintentos)
        {
            com.com_reintentos = reintentos;
            com.com_fechaultimointento = DateTime.Now;
            com.com_numero_key = com.com_numero;
            com.com_empresa_key = com.com_empresa;
            ComprobanteBLL.Update(com);
        }

        private void GrabarLog(Comprobante com, string accion, string resultado, string mensaje, int? estadoFinal = null)
        {
            try
            {
                AutomataLog log = new AutomataLog
                {
                    al_ejecucion = _ejecucionId,
                    al_empresa = com.com_empresa,
                    al_numero = com.com_numero,
                    al_numero_legible = com.com_almacen + "-" + com.com_pventa + "-" + com.com_secuencia,
                    al_estado_inicial = com.com_estado,
                    al_estado_final = estadoFinal ?? com.com_estado,
                    al_accion = accion,
                    al_resultado = resultado,
                    al_mensaje = mensaje != null && mensaje.Length > 500 ? mensaje.Substring(0, 500) : mensaje,
                    al_fecha = DateTime.Now
                };
                AutomataLogBLL.Insert(log);
            }
            catch (Exception ex)
            {
                AutomataSRILog.Error("GrabarLog", ex);
            }
        }

        private void LimpiarLogsAntiguos(int empresa)
        {
            try
            {
                DateTime limite = DateTime.Now.AddDays(-_config.dias_retencion_log);
                AutomataLogBLL.DeleteAll(new WhereParams(
                    "al_empresa = {0} AND al_fecha < {1}", empresa, limite));
                AutomataEjecucionBLL.GetAll(new WhereParams(
                    "ae_empresa = {0} AND ae_fecha_inicio < {1}", empresa, limite), "");
            }
            catch (Exception ex)
            {
                AutomataSRILog.Error("LimpiarLogsAntiguos", ex);
            }
        }

        private static void Log(string texto)
        {
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " " + texto);
            AutomataSRILog.Linea(texto);
        }
    }
}
