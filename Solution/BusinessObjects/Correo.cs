using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using Functions;

namespace BusinessObjects
{
    public class Correo
    {
        #region Properties

        [Data(key = true)]
        public Int32 cor_empresa { get; set; }
        [Data(originalkey = true)]
        public Int32 cor_empresa_key { get; set; }
        [Data(key = true)]
        public String cor_comprobante { get; set; }
        [Data(originalkey = true)]
        public String cor_comprobante_key { get; set; }
        [Data(key = true, auto = true)]
        public Int32 cor_codigo { get; set; }
        [Data(originalkey = true)]
        public Int32 cor_codigo_key { get; set; }
        public DateTime? cor_fecha { get; set; }
        public String cor_origen { get; set; }
        public String cor_destinatario { get; set; }
        public String cor_adjuntos { get; set; }
        public String cor_asunto { get; set; }
        public String cor_mensaje { get; set; }
        public DateTime? cor_fechaenvio { get; set; }
        public String cor_resultado { get; set; }
        public Int32? cor_automatico { get; set; }
        public Int32? cor_estado { get; set; }
        public Int32? cor_envios { get; set; }
        public String crea_usr { get; set; }
        public DateTime? crea_fecha { get; set; }
        public String mod_usr { get; set; }
        public DateTime? mod_fecha { get; set; }


        [Data(nosql = true, tablaref = "comprobante", camporef = "com_almacen", foreign = "cor_empresa,cor_comprobante", keyref = "com_empresa, com_numero", join = "left")]
        public String cor_almacen{ get; set; }
        [Data(nosql = true, tablaref = "comprobante", camporef = "com_pventa", foreign = "cor_empresa,cor_comprobante", keyref = "com_empresa, com_numero", join = "left")]
        public String cor_pventa { get; set; }
        [Data(nosql = true, tablaref = "comprobante", camporef = "com_secuencia", foreign = "cor_empresa,cor_comprobante", keyref = "com_empresa, com_numero", join = "left")]
        public String cor_secuencia { get; set; }




        #endregion

        #region Constructors


        public Correo()
        {
        }

        public Correo(Int32 cor_empresa, String cor_comprobante, Int32 cor_codigo, DateTime cor_fecha, String cor_destinatario, String cor_adjuntos, String cor_asunto, String cor_mensaje, DateTime cor_fechaenvio, String cor_resultado, Int32 cor_automatico, Int32 cor_estado, String crea_usr, DateTime crea_fecha, String mod_usr, DateTime mod_fecha)
        {
            this.cor_empresa = cor_empresa;
            this.cor_comprobante = cor_comprobante;
            this.cor_codigo = cor_codigo;
            this.cor_fecha = cor_fecha;
            this.cor_destinatario = cor_destinatario;
            this.cor_adjuntos = cor_adjuntos;
            this.cor_asunto = cor_asunto;
            this.cor_mensaje = cor_mensaje;
            this.cor_fechaenvio = cor_fechaenvio;
            this.cor_resultado = cor_resultado;
            this.cor_automatico = cor_automatico;
            this.cor_estado = cor_estado;
            this.crea_usr = crea_usr;
            this.crea_fecha = crea_fecha;
            this.mod_usr = mod_usr;
            this.mod_fecha = mod_fecha;


        }

        public Correo(IDataReader reader)
        {
            this.cor_empresa = (Int32)reader["cor_empresa"];
            this.cor_comprobante = reader["cor_comprobante"].ToString();
            this.cor_codigo = (Int32)reader["cor_codigo"];
            this.cor_fecha = (reader["cor_fecha"] != DBNull.Value) ? (DateTime?)reader["cor_fecha"] : null;
            this.cor_origen = reader["cor_origen"].ToString();
            this.cor_destinatario = reader["cor_destinatario"].ToString();
            this.cor_adjuntos = reader["cor_adjuntos"].ToString();
            this.cor_asunto = reader["cor_asunto"].ToString();
            this.cor_mensaje = reader["cor_mensaje"].ToString();
            this.cor_fechaenvio = (reader["cor_fechaenvio"] != DBNull.Value) ? (DateTime?)reader["cor_fechaenvio"] : null;
            this.cor_resultado = reader["cor_resultado"].ToString();
            this.cor_automatico = (reader["cor_automatico"] != DBNull.Value) ? (Int32?)reader["cor_automatico"] : null;
            this.cor_envios= (reader["cor_envios"] != DBNull.Value) ? (Int32?)reader["cor_envios"] : null;
            this.cor_estado = (reader["cor_estado"] != DBNull.Value) ? (Int32?)reader["cor_estado"] : null;
            this.crea_usr = reader["crea_usr"].ToString();
            this.crea_fecha = (reader["crea_fecha"] != DBNull.Value) ? (DateTime?)reader["crea_fecha"] : null;
            this.mod_usr = reader["mod_usr"].ToString();
            this.mod_fecha = (reader["mod_fecha"] != DBNull.Value) ? (DateTime?)reader["mod_fecha"] : null;

            this.cor_almacen = reader["cor_almacen"].ToString();
            this.cor_pventa = reader["cor_pventa"].ToString();
            this.cor_secuencia = reader["cor_secuencia"].ToString();

        }


        public Correo(object objeto)
        {
            if (objeto != null)
            {
                Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
                object cor_empresa = null;
                object cor_comprobante = null;
                object cor_codigo = null;
                object cor_fecha = null;
                object cor_origen = null;
                object cor_destinatario = null;
                object cor_adjuntos = null;
                object cor_asunto = null;
                object cor_mensaje = null;
                object cor_fechaenvio = null;
                object cor_resultado = null;
                object cor_automatico = null;
                object cor_envios= null;
                object cor_estado = null;
                object crea_usr = null;
                object crea_fecha = null;
                object mod_usr = null;
                object mod_fecha = null;


                tmp.TryGetValue("cor_empresa", out cor_empresa);
                tmp.TryGetValue("cor_comprobante", out cor_comprobante);
                tmp.TryGetValue("cor_codigo", out cor_codigo);
                tmp.TryGetValue("cor_fecha", out cor_fecha);
                tmp.TryGetValue("cor_origen", out cor_origen);
                tmp.TryGetValue("cor_destinatario", out cor_destinatario);
                tmp.TryGetValue("cor_adjuntos", out cor_adjuntos);
                tmp.TryGetValue("cor_asunto", out cor_asunto);
                tmp.TryGetValue("cor_mensaje", out cor_mensaje);
                tmp.TryGetValue("cor_fechaenvio", out cor_fechaenvio);
                tmp.TryGetValue("cor_resultado", out cor_resultado);
                tmp.TryGetValue("cor_automatico", out cor_automatico);
                tmp.TryGetValue("cor_envios", out cor_envios);
                tmp.TryGetValue("cor_estado", out cor_estado);
                tmp.TryGetValue("crea_usr", out crea_usr);
                tmp.TryGetValue("crea_fecha", out crea_fecha);
                tmp.TryGetValue("mod_usr", out mod_usr);
                tmp.TryGetValue("mod_fecha", out mod_fecha);


                this.cor_empresa = (Int32)Conversiones.GetValueByType(cor_empresa, typeof(Int32));
                this.cor_comprobante = (String)Conversiones.GetValueByType(cor_comprobante, typeof(String));
                this.cor_codigo = (Int32)Conversiones.GetValueByType(cor_codigo, typeof(Int32));
                this.cor_fecha = (DateTime?)Conversiones.GetValueByType(cor_fecha, typeof(DateTime?));
                this.cor_origen = (String)Conversiones.GetValueByType(cor_origen, typeof(String));
                this.cor_destinatario = (String)Conversiones.GetValueByType(cor_destinatario, typeof(String));
                this.cor_adjuntos = (String)Conversiones.GetValueByType(cor_adjuntos, typeof(String));
                this.cor_asunto = (String)Conversiones.GetValueByType(cor_asunto, typeof(String));
                this.cor_mensaje = (String)Conversiones.GetValueByType(cor_mensaje, typeof(String));
                this.cor_fechaenvio = (DateTime?)Conversiones.GetValueByType(cor_fechaenvio, typeof(DateTime?));
                this.cor_resultado = (String)Conversiones.GetValueByType(cor_resultado, typeof(String));
                this.cor_automatico = (Int32?)Conversiones.GetValueByType(cor_automatico, typeof(Int32?));
                this.cor_envios= (Int32?)Conversiones.GetValueByType(cor_envios, typeof(Int32?));
                this.cor_estado = (Int32?)Conversiones.GetValueByType(cor_estado, typeof(Int32?));
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
