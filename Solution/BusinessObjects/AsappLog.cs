using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Functions;

namespace BusinessObjects
{
    public class AsappLog
    {
        #region Properties

        [Data(key = true, auto = true)]
        public Int32 asl_id { get; set; }
        [Data(originalkey = true)]
        public Int32 asl_id_key { get; set; }
        public Int32 asl_empresa { get; set; }
        public String asl_claveacceso { get; set; }
        public String asl_endpoint { get; set; }
        public String asl_estado { get; set; }
        public Int32? asl_httpstatus { get; set; }
        public DateTime? asl_fecha { get; set; }
        public String asl_error { get; set; }

        #endregion

        #region Constructors

        public AsappLog()
        {
        }

        public AsappLog(Int32 asl_id, Int32 asl_empresa, String asl_claveacceso, String asl_endpoint, String asl_estado, Int32? asl_httpstatus, DateTime? asl_fecha, String asl_error)
        {
            this.asl_id = asl_id;
            this.asl_empresa = asl_empresa;
            this.asl_claveacceso = asl_claveacceso;
            this.asl_endpoint = asl_endpoint;
            this.asl_estado = asl_estado;
            this.asl_httpstatus = asl_httpstatus;
            this.asl_fecha = asl_fecha;
            this.asl_error = asl_error;
        }

        public AsappLog(IDataReader reader)
        {
            this.asl_id = (Int32)reader["asl_id"];
            this.asl_empresa = (Int32)reader["asl_empresa"];
            this.asl_claveacceso = reader["asl_claveacceso"].ToString();
            this.asl_endpoint = reader["asl_endpoint"].ToString();
            this.asl_estado = reader["asl_estado"].ToString();
            this.asl_httpstatus = (reader["asl_httpstatus"] != DBNull.Value) ? (Int32?)reader["asl_httpstatus"] : null;
            this.asl_fecha = (reader["asl_fecha"] != DBNull.Value) ? (DateTime?)reader["asl_fecha"] : null;
            this.asl_error = reader["asl_error"].ToString();
        }

        public AsappLog(object objeto)
        {
            if (objeto != null)
            {
                Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
                object asl_id = null;
                object asl_empresa = null;
                object asl_claveacceso = null;
                object asl_endpoint = null;
                object asl_estado = null;
                object asl_httpstatus = null;
                object asl_fecha = null;
                object asl_error = null;

                tmp.TryGetValue("asl_id", out asl_id);
                tmp.TryGetValue("asl_empresa", out asl_empresa);
                tmp.TryGetValue("asl_claveacceso", out asl_claveacceso);
                tmp.TryGetValue("asl_endpoint", out asl_endpoint);
                tmp.TryGetValue("asl_estado", out asl_estado);
                tmp.TryGetValue("asl_httpstatus", out asl_httpstatus);
                tmp.TryGetValue("asl_fecha", out asl_fecha);
                tmp.TryGetValue("asl_error", out asl_error);

                this.asl_id = (Int32)Conversiones.GetValueByType(asl_id, typeof(Int32));
                this.asl_empresa = (Int32)Conversiones.GetValueByType(asl_empresa, typeof(Int32));
                this.asl_claveacceso = (String)Conversiones.GetValueByType(asl_claveacceso, typeof(String));
                this.asl_endpoint = (String)Conversiones.GetValueByType(asl_endpoint, typeof(String));
                this.asl_estado = (String)Conversiones.GetValueByType(asl_estado, typeof(String));
                this.asl_httpstatus = (Int32?)Conversiones.GetValueByType(asl_httpstatus, typeof(Int32?));
                this.asl_fecha = (DateTime?)Conversiones.GetValueByType(asl_fecha, typeof(DateTime?));
                this.asl_error = (String)Conversiones.GetValueByType(asl_error, typeof(String));
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
