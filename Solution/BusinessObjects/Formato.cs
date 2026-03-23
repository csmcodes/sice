using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using Functions;

namespace BusinessObjects
{
    public class Formato
    {
        #region Properties

        [Data(key = true)]
        public Int32 for_empresa { get; set; }
        [Data(originalkey = true)]
        public Int32 for_empresa_key { get; set; }
        [Data(key = true)]
        public Int32 for_codigo { get; set; }
        [Data(originalkey = true)]
        public Int32 for_codigo_key { get; set; }
        public String for_pdf { get; set; }
        public String for_mail { get; set; }
        public String for_tipo { get; set; }
        public String for_size { get; set; }
        public Int32? for_estado { get; set; }
        public String for_pie { get; set; }
        public String crea_usr { get; set; }
        public DateTime? crea_fecha { get; set; }
        public String mod_usr { get; set; }
        public DateTime? mod_fecha { get; set; }


        #endregion

        #region Constructors


        public Formato()
        {
        }

        public Formato(Int32 for_empresa, Int32 for_codigo, String for_pdf, String for_mail, Int32 for_estado, String crea_usr, DateTime crea_fecha, String mod_usr, DateTime mod_fecha)
        {
            this.for_empresa = for_empresa;
            this.for_codigo = for_codigo;
            this.for_pdf = for_pdf;
            this.for_mail = for_mail;
            this.for_estado = for_estado;
            this.crea_usr = crea_usr;
            this.crea_fecha = crea_fecha;
            this.mod_usr = mod_usr;
            this.mod_fecha = mod_fecha;


        }

        public Formato(IDataReader reader)
        {
            this.for_empresa = (Int32)reader["for_empresa"];
            this.for_codigo = (Int32)reader["for_codigo"];
            this.for_pdf = reader["for_pdf"].ToString();
            this.for_mail = reader["for_mail"].ToString();
            this.for_tipo = reader["for_tipo"].ToString();
            this.for_estado = (reader["for_estado"] != DBNull.Value) ? (Int32?)reader["for_estado"] : null;
            this.for_pie = reader["for_pie"].ToString();
            this.for_size= reader["for_size"].ToString();
            this.crea_usr = reader["crea_usr"].ToString();
            this.crea_fecha = (reader["crea_fecha"] != DBNull.Value) ? (DateTime?)reader["crea_fecha"] : null;
            this.mod_usr = reader["mod_usr"].ToString();
            this.mod_fecha = (reader["mod_fecha"] != DBNull.Value) ? (DateTime?)reader["mod_fecha"] : null;

        }


        public Formato(object objeto)
        {
            if (objeto != null)
            {
                Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
                object for_empresa = null;
                object for_codigo = null;
                object for_pdf = null;
                object for_mail = null;
                object for_tipo = null;
                object for_pie = null;
                object for_size = null;
                object for_estado = null;
                object crea_usr = null;
                object crea_fecha = null;
                object mod_usr = null;
                object mod_fecha = null;


                tmp.TryGetValue("for_empresa", out for_empresa);
                tmp.TryGetValue("for_codigo", out for_codigo);
                tmp.TryGetValue("for_pdf", out for_pdf);
                tmp.TryGetValue("for_mail", out for_mail);
                tmp.TryGetValue("for_tipo", out for_tipo);
                tmp.TryGetValue("for_pie", out for_pie);
                tmp.TryGetValue("for_size", out for_size);
                tmp.TryGetValue("for_estado", out for_estado);
                tmp.TryGetValue("crea_usr", out crea_usr);
                tmp.TryGetValue("crea_fecha", out crea_fecha);
                tmp.TryGetValue("mod_usr", out mod_usr);
                tmp.TryGetValue("mod_fecha", out mod_fecha);


                this.for_empresa = (Int32)Conversiones.GetValueByType(for_empresa, typeof(Int32));
                this.for_codigo = (Int32)Conversiones.GetValueByType(for_codigo, typeof(Int32));
                this.for_pdf = (String)Conversiones.GetValueByType(for_pdf, typeof(String));
                this.for_mail = (String)Conversiones.GetValueByType(for_mail, typeof(String));
                this.for_tipo = (String)Conversiones.GetValueByType(for_tipo, typeof(String));
                this.for_pie = (String)Conversiones.GetValueByType(for_pie, typeof(String));
                this.for_size= (String)Conversiones.GetValueByType(for_size, typeof(String));
                this.for_estado = (Int32?)Conversiones.GetValueByType(for_estado, typeof(Int32?));
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
