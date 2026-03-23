using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using BusinessLogicLayer;
using BusinessObjects;

namespace AutomataSRI
{
    public class ConfigAutomata
    {
        public bool activo { get; set; }
        public bool simulacion { get; set; }
        public int dias_atras { get; set; }
        public int dias_retencion_log { get; set; }
        public int max_reintentos_enviado { get; set; }
        public int max_reintentos_devuelto { get; set; }
        public int minutos_espera_enviado { get; set; }
        public int minutos_espera_recibido { get; set; }
        public int horas_alerta_recibido { get; set; }

        public static ConfigAutomata Cargar(int empresa)
        {
            try
            {
                Parametro par = ParametroBLL.GetByPK(new Parametro
                {
                    par_empresa = empresa,
                    par_empresa_key = empresa,
                    par_id = "automatasri",
                    par_id_key = "automatasri"
                });

                if (string.IsNullOrEmpty(par.par_valor))
                    return null;

                var js = new JavaScriptSerializer();
                return js.Deserialize<ConfigAutomata>(par.par_valor);
            }
            catch (Exception ex)
            {
                AutomataSRILog.Error("ConfigAutomata.Cargar empresa=" + empresa, ex);
                return null;
            }
        }
    }
}
