using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using BusinessLogicLayer;
using BusinessObjects;
using Services;

namespace Packages
{
    public static class AsappClient
    {
        private static readonly HttpClient _http = new HttpClient();
        private static readonly JavaScriptSerializer _json = new JavaScriptSerializer();

        // ----------------------------------------------------------------
        // Notificaciones públicas — no bloquean, no lanzan excepciones
        // ----------------------------------------------------------------

        public static void NotificarEnviado(Comprobante comprobante)
        {
            int status = 0;
            string error = null;
            try
            {
                Empresa empresa = GetEmpresa(comprobante.com_empresa);
                if (!IsActive(empresa)) return;

                string baseUrl = GetBaseUrl();
                if (baseUrl == null) return;

                string pathxml = Constantes.GetParameter("pathfiles")
                    + "\\temp\\" + comprobante.com_numero + "_" + comprobante.com_empresa + ".xml";

                string xmlBase64 = File.Exists(pathxml)
                    ? Convert.ToBase64String(File.ReadAllBytes(pathxml))
                    : "";

                var body = new
                {
                    xmlBase64 = xmlBase64,
                    tipoDocumento = comprobante.com_numero != null && comprobante.com_numero.Length >= 10
                        ? comprobante.com_numero.Substring(8, 2)
                        : "",
                    ambiente = comprobante.com_ambiente == (int)Enums.Ambiente.PRODUCCIÓN
                        ? "Produccion"
                        : "Pruebas",
                    claveAcceso = comprobante.com_numero,
                    numeroComprobante = comprobante.com_almacen + "-" + comprobante.com_pventa + "-" + comprobante.com_secuencia,
                    fechaEmision = comprobante.com_fecha.HasValue
                        ? comprobante.com_fecha.Value.ToString("yyyy-MM-ddTHH:mm:ss") + "-05:00"
                        : ""
                };

                status = Post(baseUrl + "/v1/sice/comprobantes", empresa.emp_asapp_apikey, body, comprobante.com_numero);
                if (status == 0) error = "Sin respuesta (red/timeout)";
                else if (status >= 400) error = "HTTP " + status;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                ExceptionHandling.Log.AddLog("ASAPP NotificarEnviado ERROR " + comprobante.com_numero + ": " + ex.Message);
            }
            finally
            {
                WriteLog(comprobante.com_empresa, comprobante.com_numero, "Ingest", "Enviado", status > 0 ? status : (int?)null, error);
            }
        }

        public static void NotificarRecibido(Comprobante comprobante)
        {
            int status = 0;
            string error = null;
            try
            {
                Empresa empresa = GetEmpresa(comprobante.com_empresa);
                if (!IsActive(empresa)) return;

                string baseUrl = GetBaseUrl();
                if (baseUrl == null) return;

                status = Patch(baseUrl + "/v1/sice/comprobantes/" + comprobante.com_numero,
                    empresa.emp_asapp_apikey,
                    new { estado = "Recibido" },
                    comprobante.com_numero);
                if (status == 0) error = "Sin respuesta (red/timeout)";
                else if (status >= 400) error = "HTTP " + status;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                ExceptionHandling.Log.AddLog("ASAPP NotificarRecibido ERROR " + comprobante.com_numero + ": " + ex.Message);
            }
            finally
            {
                WriteLog(comprobante.com_empresa, comprobante.com_numero, "UpdateEstado", "Recibido", status > 0 ? status : (int?)null, error);
            }
        }

        public static void NotificarDevuelto(Comprobante comprobante)
        {
            int status = 0;
            string error = null;
            try
            {
                Empresa empresa = GetEmpresa(comprobante.com_empresa);
                if (!IsActive(empresa)) return;

                string baseUrl = GetBaseUrl();
                if (baseUrl == null) return;

                status = Patch(baseUrl + "/v1/sice/comprobantes/" + comprobante.com_numero,
                    empresa.emp_asapp_apikey,
                    new { estado = "Devuelto", mensaje = comprobante.com_mensaje ?? "" },
                    comprobante.com_numero);
                if (status == 0) error = "Sin respuesta (red/timeout)";
                else if (status >= 400) error = "HTTP " + status;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                ExceptionHandling.Log.AddLog("ASAPP NotificarDevuelto ERROR " + comprobante.com_numero + ": " + ex.Message);
            }
            finally
            {
                WriteLog(comprobante.com_empresa, comprobante.com_numero, "UpdateEstado", "Devuelto", status > 0 ? status : (int?)null, error);
            }
        }

        // xmlAutorizadoStr = arc.arc_xmlrespuesta en el momento de la autorización
        public static void NotificarAutorizado(Comprobante comprobante, string xmlAutorizadoStr)
        {
            int status = 0;
            string error = null;
            try
            {
                Empresa empresa = GetEmpresa(comprobante.com_empresa);
                if (!IsActive(empresa)) return;

                string baseUrl = GetBaseUrl();
                if (baseUrl == null) return;

                string xmlBase64 = !string.IsNullOrEmpty(xmlAutorizadoStr)
                    ? Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlAutorizadoStr))
                    : "";

                string fechaAut = comprobante.com_fechaautorizacion ?? "";
                DateTime dtAut;
                if (DateTime.TryParse(fechaAut, out dtAut))
                    fechaAut = dtAut.ToString("yyyy-MM-ddTHH:mm:ss") + "-05:00";

                status = Patch(baseUrl + "/v1/sice/comprobantes/" + comprobante.com_numero,
                    empresa.emp_asapp_apikey,
                    new
                    {
                        estado = "Autorizado",
                        numeroAutorizacion = !string.IsNullOrEmpty(comprobante.com_autorizacion)
                            ? comprobante.com_autorizacion
                            : comprobante.com_numero,
                        fechaAutorizacion = fechaAut,
                        xmlAutorizadoBase64 = xmlBase64
                    },
                    comprobante.com_numero);
                if (status == 0) error = "Sin respuesta (red/timeout)";
                else if (status >= 400) error = "HTTP " + status;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                ExceptionHandling.Log.AddLog("ASAPP NotificarAutorizado ERROR " + comprobante.com_numero + ": " + ex.Message);
            }
            finally
            {
                WriteLog(comprobante.com_empresa, comprobante.com_numero, "UpdateEstado", "Autorizado", status > 0 ? status : (int?)null, error);
            }
        }

        public static void NotificarNoAutorizado(Comprobante comprobante)
        {
            int status = 0;
            string error = null;
            try
            {
                Empresa empresa = GetEmpresa(comprobante.com_empresa);
                if (!IsActive(empresa)) return;

                string baseUrl = GetBaseUrl();
                if (baseUrl == null) return;

                status = Patch(baseUrl + "/v1/sice/comprobantes/" + comprobante.com_numero,
                    empresa.emp_asapp_apikey,
                    new { estado = "NoAutorizado", mensaje = comprobante.com_mensaje ?? "" },
                    comprobante.com_numero);
                if (status == 0) error = "Sin respuesta (red/timeout)";
                else if (status >= 400) error = "HTTP " + status;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                ExceptionHandling.Log.AddLog("ASAPP NotificarNoAutorizado ERROR " + comprobante.com_numero + ": " + ex.Message);
            }
            finally
            {
                WriteLog(comprobante.com_empresa, comprobante.com_numero, "UpdateEstado", "NoAutorizado", status > 0 ? status : (int?)null, error);
            }
        }

        // ----------------------------------------------------------------
        // HTTP helpers — retornan el código HTTP, 0 si falla la red
        // ----------------------------------------------------------------

        private static int Post(string url, string apiKey, object body, string clave)
        {
            int status = 0;
            CallWithRetry(() =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("X-Api-Key", apiKey);
                request.Content = new StringContent(_json.Serialize(body), Encoding.UTF8, "application/json");
                var response = _http.SendAsync(request).GetAwaiter().GetResult();
                status = (int)response.StatusCode;
                LogHttpError("POST", clave, response);
            }, "POST", clave);
            return status;
        }

        private static int Patch(string url, string apiKey, object body, string clave)
        {
            int status = 0;
            CallWithRetry(() =>
            {
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), url);
                request.Headers.Add("X-Api-Key", apiKey);
                request.Content = new StringContent(_json.Serialize(body), Encoding.UTF8, "application/json");
                var response = _http.SendAsync(request).GetAwaiter().GetResult();
                status = (int)response.StatusCode;
                LogHttpError("PATCH", clave, response);
            }, "PATCH", clave);
            return status;
        }

        private static void CallWithRetry(Action call, string metodo, string clave)
        {
            const int maxIntentos = 3;
            int intento = 0;
            while (intento < maxIntentos)
            {
                try
                {
                    call();
                    return;
                }
                catch (HttpRequestException)
                {
                    intento++;
                    if (intento >= maxIntentos)
                    {
                        ExceptionHandling.Log.AddLog(string.Format(
                            "ASAPP {0} sin respuesta tras {1} intentos: {2}", metodo, maxIntentos, clave));
                        return;
                    }
                    Thread.Sleep((int)Math.Pow(3, intento) * 1000);
                }
            }
        }

        private static void LogHttpError(string metodo, string clave, HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                ExceptionHandling.Log.AddLog(string.Format(
                    "ASAPP {0} {1} [{2}]: {3}", metodo, clave, (int)response.StatusCode, body));
            }
        }

        // ----------------------------------------------------------------
        // Auditoría — escribe en asapp_log, nunca lanza
        // ----------------------------------------------------------------

        private static void WriteLog(int empresa, string claveAcceso, string endpoint, string estado, int? httpStatus, string error)
        {
            try
            {
                AsappLogBLL.Insert(new AsappLog
                {
                    asl_empresa     = empresa,
                    asl_claveacceso = claveAcceso,
                    asl_endpoint    = endpoint,
                    asl_estado      = estado,
                    asl_httpstatus  = httpStatus,
                    asl_fecha       = DateTime.Now,
                    asl_error       = error
                });
            }
            catch
            {
                // El log no debe romper el flujo bajo ninguna circunstancia
            }
        }

        // ----------------------------------------------------------------
        // Config helpers
        // ----------------------------------------------------------------

        private static Empresa GetEmpresa(int codigo)
        {
            return EmpresaBLL.GetByPK(new Empresa { emp_codigo = codigo, emp_codigo_key = codigo });
        }

        private static bool IsActive(Empresa empresa)
        {
            return empresa != null
                && empresa.emp_asapp_activo == 1
                && !string.IsNullOrEmpty(empresa.emp_asapp_apikey);
        }

        private static string GetBaseUrl()
        {
            try
            {
                string raw = Constantes.GetParameter("asapp");
                var cfg = _json.Deserialize<Dictionary<string, object>>(raw);
                return cfg["baseUrl"].ToString().TrimEnd('/');
            }
            catch
            {
                return null;
            }
        }
    }
}
