using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using Functions;

namespace BusinessObjects
{
    public class Empresa
    {
        #region Properties

        [Data(key = true)]
        public Int32 emp_codigo { get; set; }
        [Data(originalkey = true)]
        public Int32 emp_codigo_key { get; set; }
        public String emp_ruc { get; set; }
        public String emp_nombre { get; set; }
        public Int32? emp_estado { get; set; }
        public String emp_certificado { get; set; }
        public String emp_password { get; set; }
        public String emp_mail { get; set; }
        public String emp_mailadm { get; set; }
        public String emp_logo { get; set; }
        public String emp_agenteret { get; set; }
        public Int32? emp_funcion { get; set; }
        public Int32? emp_asapp_activo { get; set; }
        public String emp_asapp_apikey { get; set; }
        public Int32? emp_asapp_modo { get; set; }
        public String crea_usr { get; set; }
        public DateTime? crea_fecha { get; set; }
        public String mod_usr { get; set; }
        public DateTime? mod_fecha { get; set; }


        #endregion

        #region Constructors


        public Empresa()
        {
        }

        public Empresa(Int32 emp_codigo, String emp_ruc, String emp_nombre, Int32 emp_estado, String emp_certificado, String emp_password, String emp_mail, String emp_mailadm, String crea_usr, DateTime crea_fecha, String mod_usr, DateTime mod_fecha)
        {
            this.emp_codigo = emp_codigo;
            this.emp_ruc = emp_ruc;
            this.emp_nombre = emp_nombre;
            this.emp_estado = emp_estado;
            this.emp_certificado = emp_certificado;
            this.emp_password = emp_password;
            this.emp_mail = emp_mail;
            this.emp_mailadm = emp_mailadm;
            this.crea_usr = crea_usr;
            this.crea_fecha = crea_fecha;
            this.mod_usr = mod_usr;
            this.mod_fecha = mod_fecha;


        }

        public Empresa(IDataReader reader)
        {
            this.emp_codigo = (Int32)reader["emp_codigo"];
            this.emp_ruc = reader["emp_ruc"].ToString();
            this.emp_nombre = reader["emp_nombre"].ToString();
            this.emp_estado = (reader["emp_estado"] != DBNull.Value) ? (Int32?)reader["emp_estado"] : null;
            this.emp_certificado = reader["emp_certificado"].ToString();
            this.emp_password = reader["emp_password"].ToString();
            this.emp_mail = reader["emp_mail"].ToString();
            this.emp_mailadm = reader["emp_mailadm"].ToString();
            this.emp_logo= reader["emp_logo"].ToString();
            this.emp_agenteret= reader["emp_agenteret"].ToString();
            this.emp_funcion = (reader["emp_funcion"] != DBNull.Value) ? (Int32?)reader["emp_funcion"] : null;
            this.emp_asapp_activo = HasColumn(reader, "emp_asapp_activo") && reader["emp_asapp_activo"] != DBNull.Value ? (Int32?)reader["emp_asapp_activo"] : null;
            this.emp_asapp_apikey = HasColumn(reader, "emp_asapp_apikey") ? reader["emp_asapp_apikey"].ToString() : null;
            this.emp_asapp_modo = HasColumn(reader, "emp_asapp_modo") && reader["emp_asapp_modo"] != DBNull.Value ? (Int32?)reader["emp_asapp_modo"] : null;
            this.crea_usr = reader["crea_usr"].ToString();
            this.crea_fecha = (reader["crea_fecha"] != DBNull.Value) ? (DateTime?)reader["crea_fecha"] : null;
            this.mod_usr = reader["mod_usr"].ToString();
            this.mod_fecha = (reader["mod_fecha"] != DBNull.Value) ? (DateTime?)reader["mod_fecha"] : null;

        }


        public Empresa(object objeto)
        {
            if (objeto != null)
            {
                Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
                object emp_codigo = null;
                object emp_ruc = null;
                object emp_nombre = null;
                object emp_estado = null;
                object emp_certificado = null;
                object emp_password = null;
                object emp_mail = null;
                object emp_mailadm = null;
                object emp_logo = null;
                object emp_funcion = null;
                object emp_agenteret = null;
                object emp_asapp_activo = null;
                object emp_asapp_apikey = null;
                object emp_asapp_modo = null;
                object crea_usr = null;
                object crea_fecha = null;
                object mod_usr = null;
                object mod_fecha = null;


                tmp.TryGetValue("emp_codigo", out emp_codigo);
                tmp.TryGetValue("emp_ruc", out emp_ruc);
                tmp.TryGetValue("emp_nombre", out emp_nombre);
                tmp.TryGetValue("emp_estado", out emp_estado);
                tmp.TryGetValue("emp_certificado", out emp_certificado);
                tmp.TryGetValue("emp_password", out emp_password);
                tmp.TryGetValue("emp_mail", out emp_mail);
                tmp.TryGetValue("emp_mailadm", out emp_mailadm);
                tmp.TryGetValue("emp_logo", out emp_logo);
                tmp.TryGetValue("emp_agenteret", out emp_agenteret);
                tmp.TryGetValue("emp_funcion", out emp_funcion);
                tmp.TryGetValue("emp_asapp_activo", out emp_asapp_activo);
                tmp.TryGetValue("emp_asapp_apikey", out emp_asapp_apikey);
                tmp.TryGetValue("emp_asapp_modo", out emp_asapp_modo);
                tmp.TryGetValue("crea_usr", out crea_usr);
                tmp.TryGetValue("crea_fecha", out crea_fecha);
                tmp.TryGetValue("mod_usr", out mod_usr);
                tmp.TryGetValue("mod_fecha", out mod_fecha);


                this.emp_codigo = (Int32)Conversiones.GetValueByType(emp_codigo, typeof(Int32));
                this.emp_ruc = (String)Conversiones.GetValueByType(emp_ruc, typeof(String));
                this.emp_nombre = (String)Conversiones.GetValueByType(emp_nombre, typeof(String));
                this.emp_estado = (Int32?)Conversiones.GetValueByType(emp_estado, typeof(Int32?));
                this.emp_certificado = (String)Conversiones.GetValueByType(emp_certificado, typeof(String));
                this.emp_password = (String)Conversiones.GetValueByType(emp_password, typeof(String));
                this.emp_mail = (String)Conversiones.GetValueByType(emp_mail, typeof(String));
                this.emp_mailadm = (String)Conversiones.GetValueByType(emp_mailadm, typeof(String));
                this.emp_agenteret = (String)Conversiones.GetValueByType(emp_agenteret, typeof(String));
                this.emp_logo = (String)Conversiones.GetValueByType(emp_logo, typeof(String));
                this.emp_funcion = (Int32?)Conversiones.GetValueByType(emp_funcion, typeof(Int32?));
                this.emp_asapp_activo = (Int32?)Conversiones.GetValueByType(emp_asapp_activo, typeof(Int32?));
                this.emp_asapp_apikey = (String)Conversiones.GetValueByType(emp_asapp_apikey, typeof(String));
                this.emp_asapp_modo = (Int32?)Conversiones.GetValueByType(emp_asapp_modo, typeof(Int32?));
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

        private static bool HasColumn(IDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }
        #endregion


    }
}
