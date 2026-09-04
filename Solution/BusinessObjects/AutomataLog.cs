using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using Functions;

namespace BusinessObjects
{
    public class AutomataLog
    {
        #region Properties

        [Data(key = true, auto = true)]
        public Int32 al_id { get; set; }
        [Data(originalkey = true)]
        public Int32 al_id_key { get; set; }
        public Int32? al_ejecucion { get; set; }
        public Int32 al_empresa { get; set; }
        public String al_numero { get; set; }
        public String al_numero_legible { get; set; }
        public Int32? al_estado_inicial { get; set; }
        public Int32? al_estado_final { get; set; }
        public String al_accion { get; set; }
        public String al_resultado { get; set; }
        public String al_mensaje { get; set; }
        public DateTime? al_fecha { get; set; }

        #endregion

        #region Constructors

        public AutomataLog()
        {
        }

        public AutomataLog(Int32 al_id, Int32? al_ejecucion, Int32 al_empresa, String al_numero, String al_numero_legible, Int32? al_estado_inicial, Int32? al_estado_final, String al_accion, String al_resultado, String al_mensaje, DateTime? al_fecha)
        {
            this.al_id = al_id;
            this.al_ejecucion = al_ejecucion;
            this.al_empresa = al_empresa;
            this.al_numero = al_numero;
            this.al_numero_legible = al_numero_legible;
            this.al_estado_inicial = al_estado_inicial;
            this.al_estado_final = al_estado_final;
            this.al_accion = al_accion;
            this.al_resultado = al_resultado;
            this.al_mensaje = al_mensaje;
            this.al_fecha = al_fecha;
        }

        public AutomataLog(IDataReader reader)
        {
            this.al_id = (Int32)reader["al_id"];
            this.al_ejecucion = (reader["al_ejecucion"] != DBNull.Value) ? (Int32?)reader["al_ejecucion"] : null;
            this.al_empresa = (Int32)reader["al_empresa"];
            this.al_numero = reader["al_numero"].ToString();
            this.al_numero_legible = reader["al_numero_legible"].ToString();
            this.al_estado_inicial = (reader["al_estado_inicial"] != DBNull.Value) ? (Int32?)reader["al_estado_inicial"] : null;
            this.al_estado_final = (reader["al_estado_final"] != DBNull.Value) ? (Int32?)reader["al_estado_final"] : null;
            this.al_accion = reader["al_accion"].ToString();
            this.al_resultado = reader["al_resultado"].ToString();
            this.al_mensaje = reader["al_mensaje"].ToString();
            this.al_fecha = (reader["al_fecha"] != DBNull.Value) ? (DateTime?)reader["al_fecha"] : null;
        }

        public AutomataLog(object objeto)
        {
            if (objeto != null)
            {
                Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
                object al_id = null;
                object al_ejecucion = null;
                object al_empresa = null;
                object al_numero = null;
                object al_numero_legible = null;
                object al_estado_inicial = null;
                object al_estado_final = null;
                object al_accion = null;
                object al_resultado = null;
                object al_mensaje = null;
                object al_fecha = null;

                tmp.TryGetValue("al_id", out al_id);
                tmp.TryGetValue("al_ejecucion", out al_ejecucion);
                tmp.TryGetValue("al_empresa", out al_empresa);
                tmp.TryGetValue("al_numero", out al_numero);
                tmp.TryGetValue("al_numero_legible", out al_numero_legible);
                tmp.TryGetValue("al_estado_inicial", out al_estado_inicial);
                tmp.TryGetValue("al_estado_final", out al_estado_final);
                tmp.TryGetValue("al_accion", out al_accion);
                tmp.TryGetValue("al_resultado", out al_resultado);
                tmp.TryGetValue("al_mensaje", out al_mensaje);
                tmp.TryGetValue("al_fecha", out al_fecha);

                this.al_id = (Int32)Conversiones.GetValueByType(al_id, typeof(Int32));
                this.al_ejecucion = (Int32?)Conversiones.GetValueByType(al_ejecucion, typeof(Int32?));
                this.al_empresa = (Int32)Conversiones.GetValueByType(al_empresa, typeof(Int32));
                this.al_numero = (String)Conversiones.GetValueByType(al_numero, typeof(String));
                this.al_numero_legible = (String)Conversiones.GetValueByType(al_numero_legible, typeof(String));
                this.al_estado_inicial = (Int32?)Conversiones.GetValueByType(al_estado_inicial, typeof(Int32?));
                this.al_estado_final = (Int32?)Conversiones.GetValueByType(al_estado_final, typeof(Int32?));
                this.al_accion = (String)Conversiones.GetValueByType(al_accion, typeof(String));
                this.al_resultado = (String)Conversiones.GetValueByType(al_resultado, typeof(String));
                this.al_mensaje = (String)Conversiones.GetValueByType(al_mensaje, typeof(String));
                this.al_fecha = (DateTime?)Conversiones.GetValueByType(al_fecha, typeof(DateTime?));
            }
        }

        #endregion

        #region Methods

        public PropertyInfo[] GetProperties()
        {
            return this.GetType().GetProperties();
        }

        #endregion
    }
}
