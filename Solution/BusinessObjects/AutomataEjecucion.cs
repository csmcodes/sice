using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using Functions;

namespace BusinessObjects
{
    public class AutomataEjecucion
    {
        #region Properties

        [Data(key = true, auto = true)]
        public Int32 ae_id { get; set; }
        [Data(originalkey = true)]
        public Int32 ae_id_key { get; set; }
        public Int32 ae_empresa { get; set; }
        public DateTime? ae_fecha_inicio { get; set; }
        public DateTime? ae_fecha_fin { get; set; }
        public Int32? ae_procesados { get; set; }
        public Int32? ae_autorizados { get; set; }
        public Int32? ae_alertas { get; set; }
        public Boolean? ae_simulacion { get; set; }

        #endregion

        #region Constructors

        public AutomataEjecucion()
        {
        }

        public AutomataEjecucion(Int32 ae_id, Int32 ae_empresa, DateTime? ae_fecha_inicio, DateTime? ae_fecha_fin, Int32? ae_procesados, Int32? ae_autorizados, Int32? ae_alertas, Boolean? ae_simulacion)
        {
            this.ae_id = ae_id;
            this.ae_empresa = ae_empresa;
            this.ae_fecha_inicio = ae_fecha_inicio;
            this.ae_fecha_fin = ae_fecha_fin;
            this.ae_procesados = ae_procesados;
            this.ae_autorizados = ae_autorizados;
            this.ae_alertas = ae_alertas;
            this.ae_simulacion = ae_simulacion;
        }

        public AutomataEjecucion(IDataReader reader)
        {
            this.ae_id = (Int32)reader["ae_id"];
            this.ae_empresa = (Int32)reader["ae_empresa"];
            this.ae_fecha_inicio = (reader["ae_fecha_inicio"] != DBNull.Value) ? (DateTime?)reader["ae_fecha_inicio"] : null;
            this.ae_fecha_fin = (reader["ae_fecha_fin"] != DBNull.Value) ? (DateTime?)reader["ae_fecha_fin"] : null;
            this.ae_procesados = (reader["ae_procesados"] != DBNull.Value) ? (Int32?)reader["ae_procesados"] : null;
            this.ae_autorizados = (reader["ae_autorizados"] != DBNull.Value) ? (Int32?)reader["ae_autorizados"] : null;
            this.ae_alertas = (reader["ae_alertas"] != DBNull.Value) ? (Int32?)reader["ae_alertas"] : null;
            this.ae_simulacion = (reader["ae_simulacion"] != DBNull.Value) ? (Boolean?)reader["ae_simulacion"] : null;
        }

        public AutomataEjecucion(object objeto)
        {
            if (objeto != null)
            {
                Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
                object ae_id = null;
                object ae_empresa = null;
                object ae_fecha_inicio = null;
                object ae_fecha_fin = null;
                object ae_procesados = null;
                object ae_autorizados = null;
                object ae_alertas = null;
                object ae_simulacion = null;

                tmp.TryGetValue("ae_id", out ae_id);
                tmp.TryGetValue("ae_empresa", out ae_empresa);
                tmp.TryGetValue("ae_fecha_inicio", out ae_fecha_inicio);
                tmp.TryGetValue("ae_fecha_fin", out ae_fecha_fin);
                tmp.TryGetValue("ae_procesados", out ae_procesados);
                tmp.TryGetValue("ae_autorizados", out ae_autorizados);
                tmp.TryGetValue("ae_alertas", out ae_alertas);
                tmp.TryGetValue("ae_simulacion", out ae_simulacion);

                this.ae_id = (Int32)Conversiones.GetValueByType(ae_id, typeof(Int32));
                this.ae_empresa = (Int32)Conversiones.GetValueByType(ae_empresa, typeof(Int32));
                this.ae_fecha_inicio = (DateTime?)Conversiones.GetValueByType(ae_fecha_inicio, typeof(DateTime?));
                this.ae_fecha_fin = (DateTime?)Conversiones.GetValueByType(ae_fecha_fin, typeof(DateTime?));
                this.ae_procesados = (Int32?)Conversiones.GetValueByType(ae_procesados, typeof(Int32?));
                this.ae_autorizados = (Int32?)Conversiones.GetValueByType(ae_autorizados, typeof(Int32?));
                this.ae_alertas = (Int32?)Conversiones.GetValueByType(ae_alertas, typeof(Int32?));
                this.ae_simulacion = (Boolean?)Conversiones.GetValueByType(ae_simulacion, typeof(Boolean?));
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
