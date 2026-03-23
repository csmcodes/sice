using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using Functions;

namespace BusinessObjects
{
    public class Certificado
    {
        #region Properties

        [Data(key = true)]
        public Int32 cer_empresa { get; set; }
        [Data(originalkey = true)]
        public Int32 cer_empresa_key { get; set; }
        [Data(key = true)]
        public DateTime cer_desde { get; set; }
        [Data(originalkey = true)]
        public DateTime cer_desde_key { get; set; }
        [Data(key = true)]
        public DateTime cer_hasta { get; set; }
        [Data(originalkey = true)]
        public DateTime cer_hasta_key { get; set; }
        public String cer_path { get; set; }
        public String cer_nombre { get; set; }
        public String cer_password { get; set; }
        public Int32? cer_indice { get; set; }
        public Int32? cer_estado { get; set; }
        public String crea_usr { get; set; }
        public DateTime? crea_fecha { get; set; }
        public String mod_usr { get; set; }
        public DateTime? mod_fecha { get; set; }


        #endregion

        #region Constructors


        public Certificado()
        {
        }

        public Certificado(Int32 cer_empresa, DateTime cer_desde, DateTime cer_hasta, String cer_nombre, String cer_password, Int32 cer_estado, String crea_usr, DateTime crea_fecha, String mod_usr, DateTime mod_fecha)
        {
            this.cer_empresa = cer_empresa;
            this.cer_desde = cer_desde;
            this.cer_hasta = cer_hasta;
            this.cer_nombre = cer_nombre;
            this.cer_password = cer_password;
            this.cer_estado = cer_estado;
            this.crea_usr = crea_usr;
            this.crea_fecha = crea_fecha;
            this.mod_usr = mod_usr;
            this.mod_fecha = mod_fecha;


        }

        public Certificado(IDataReader reader)
        {
            this.cer_empresa = (Int32)reader["cer_empresa"];
            this.cer_desde = (DateTime)reader["cer_desde"];
            this.cer_hasta = (DateTime)reader["cer_hasta"];
            this.cer_path = reader["cer_path"].ToString();
            this.cer_nombre = reader["cer_nombre"].ToString();
            this.cer_password = reader["cer_password"].ToString();
            this.cer_indice = (reader["cer_indice"] != DBNull.Value) ? (Int32?)reader["cer_indice"] : null;
            this.cer_estado = (reader["cer_estado"] != DBNull.Value) ? (Int32?)reader["cer_estado"] : null;
            this.crea_usr = reader["crea_usr"].ToString();
            this.crea_fecha = (reader["crea_fecha"] != DBNull.Value) ? (DateTime?)reader["crea_fecha"] : null;
            this.mod_usr = reader["mod_usr"].ToString();
            this.mod_fecha = (reader["mod_fecha"] != DBNull.Value) ? (DateTime?)reader["mod_fecha"] : null;

        }


        public Certificado(object objeto)
        {
            if (objeto != null)
            {
                Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
                object cer_empresa = null;
                object cer_desde = null;
                object cer_hasta = null;
                object cer_path = null;
                object cer_nombre = null;
                object cer_password = null;
                object cer_indice = null;
                object cer_estado = null;
                object crea_usr = null;
                object crea_fecha = null;
                object mod_usr = null;
                object mod_fecha = null;


                tmp.TryGetValue("cer_empresa", out cer_empresa);
                tmp.TryGetValue("cer_desde", out cer_desde);
                tmp.TryGetValue("cer_hasta", out cer_hasta);
                tmp.TryGetValue("cer_path", out cer_path);
                tmp.TryGetValue("cer_nombre", out cer_nombre);
                tmp.TryGetValue("cer_password", out cer_password);
                tmp.TryGetValue("cer_indice", out cer_estado);
                tmp.TryGetValue("cer_estado", out cer_estado);
                tmp.TryGetValue("crea_usr", out crea_usr);
                tmp.TryGetValue("crea_fecha", out crea_fecha);
                tmp.TryGetValue("mod_usr", out mod_usr);
                tmp.TryGetValue("mod_fecha", out mod_fecha);


                this.cer_empresa = (Int32)Conversiones.GetValueByType(cer_empresa, typeof(Int32));
                this.cer_desde = (DateTime)Conversiones.GetValueByType(cer_desde, typeof(DateTime));
                this.cer_hasta = (DateTime)Conversiones.GetValueByType(cer_hasta, typeof(DateTime));
                this.cer_path = (String)Conversiones.GetValueByType(cer_path, typeof(String));
                this.cer_nombre = (String)Conversiones.GetValueByType(cer_nombre, typeof(String));
                this.cer_password = (String)Conversiones.GetValueByType(cer_password, typeof(String));
                this.cer_indice = (Int32?)Conversiones.GetValueByType(cer_indice, typeof(Int32?));
                this.cer_estado = (Int32?)Conversiones.GetValueByType(cer_estado, typeof(Int32?));
                this.crea_usr = (String)Conversiones.GetValueByType(crea_usr, typeof(String));
                this.crea_fecha = (DateTime?)Conversiones.GetValueByType(crea_fecha, typeof(DateTime?));
                this.mod_usr = (String)Conversiones.GetValueByType(mod_usr, typeof(String));
                this.mod_fecha = (DateTime?)Conversiones.GetValueByType(mod_fecha, typeof(DateTime?));

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
