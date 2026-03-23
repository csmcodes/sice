using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using Functions;

namespace BusinessObjects
{
    public class Correoadmin
    {
        #region Properties

    	[Data(key = true)]
	public Int32 coa_empresa { get; set; }
	[Data(originalkey = true)]
	public Int32 coa_empresa_key { get; set; }
	[Data(key = true, auto =true)]
	public Int32 coa_codigo { get; set; }
	[Data(originalkey = true)]
	public Int32 coa_codigo_key { get; set; }
	public DateTime? coa_fecha { get; set; }
	public String coa_origen { get; set; }
	public String coa_destinatario { get; set; }
	public String coa_adjuntos { get; set; }
	public String coa_asunto { get; set; }
	public String coa_mensaje { get; set; }
	public DateTime? coa_fechaenvio { get; set; }
	public String coa_resultado { get; set; }
	public Int32? coa_automatico { get; set; }
	public Int32? coa_envios { get; set; }
	public Int32? coa_estado { get; set; }
	public String crea_usr { get; set; }
	public DateTime? crea_fecha { get; set; }
	public String mod_usr { get; set; }
	public DateTime? mod_fecha { get; set; }

              
        #endregion

        #region Constructors


        public  Correoadmin()
        {
        }

        public  Correoadmin( Int32 coa_empresa,Int32 coa_codigo,DateTime coa_fecha,String coa_origen,String coa_destinatario,String coa_adjuntos,String coa_asunto,String coa_mensaje,DateTime coa_fechaenvio,String coa_resultado,Int32 coa_automatico,Int32 coa_envios,Int32 coa_estado,String crea_usr,DateTime crea_fecha,String mod_usr,DateTime mod_fecha)
        {                
    	this.coa_empresa = coa_empresa;
	this.coa_codigo = coa_codigo;
	this.coa_fecha = coa_fecha;
	this.coa_origen = coa_origen;
	this.coa_destinatario = coa_destinatario;
	this.coa_adjuntos = coa_adjuntos;
	this.coa_asunto = coa_asunto;
	this.coa_mensaje = coa_mensaje;
	this.coa_fechaenvio = coa_fechaenvio;
	this.coa_resultado = coa_resultado;
	this.coa_automatico = coa_automatico;
	this.coa_envios = coa_envios;
	this.coa_estado = coa_estado;
	this.crea_usr = crea_usr;
	this.crea_fecha = crea_fecha;
	this.mod_usr = mod_usr;
	this.mod_fecha = mod_fecha;

           
       }

        public  Correoadmin(IDataReader reader)
        {
    	this.coa_empresa = (Int32)reader["coa_empresa"];
            this.coa_codigo = (Int32)reader["coa_codigo"];
	this.coa_fecha = (reader["coa_fecha"] != DBNull.Value) ? (DateTime?)reader["coa_fecha"] : null;
	this.coa_origen = reader["coa_origen"].ToString();
	this.coa_destinatario = reader["coa_destinatario"].ToString();
	this.coa_adjuntos = reader["coa_adjuntos"].ToString();
	this.coa_asunto = reader["coa_asunto"].ToString();
	this.coa_mensaje = reader["coa_mensaje"].ToString();
	this.coa_fechaenvio = (reader["coa_fechaenvio"] != DBNull.Value) ? (DateTime?)reader["coa_fechaenvio"] : null;
	this.coa_resultado = reader["coa_resultado"].ToString();
	this.coa_automatico = (reader["coa_automatico"] != DBNull.Value) ? (Int32?)reader["coa_automatico"] : null;
	this.coa_envios = (reader["coa_envios"] != DBNull.Value) ? (Int32?)reader["coa_envios"] : null;
	this.coa_estado = (reader["coa_estado"] != DBNull.Value) ? (Int32?)reader["coa_estado"] : null;
	this.crea_usr = reader["crea_usr"].ToString();
	this.crea_fecha = (reader["crea_fecha"] != DBNull.Value) ? (DateTime?)reader["crea_fecha"] : null;
	this.mod_usr = reader["mod_usr"].ToString();
	this.mod_fecha = (reader["mod_fecha"] != DBNull.Value) ? (DateTime?)reader["mod_fecha"] : null;

        }


        public Correoadmin(object objeto)
        {            
            if (objeto != null)
            {
                Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
                	object coa_empresa = null;
	object coa_codigo = null;
	object coa_fecha = null;
	object coa_origen = null;
	object coa_destinatario = null;
	object coa_adjuntos = null;
	object coa_asunto = null;
	object coa_mensaje = null;
	object coa_fechaenvio = null;
	object coa_resultado = null;
	object coa_automatico = null;
	object coa_envios = null;
	object coa_estado = null;
	object crea_usr = null;
	object crea_fecha = null;
	object mod_usr = null;
	object mod_fecha = null;


                	tmp.TryGetValue("coa_empresa", out coa_empresa);
	tmp.TryGetValue("coa_codigo", out coa_codigo);
	tmp.TryGetValue("coa_fecha", out coa_fecha);
	tmp.TryGetValue("coa_origen", out coa_origen);
	tmp.TryGetValue("coa_destinatario", out coa_destinatario);
	tmp.TryGetValue("coa_adjuntos", out coa_adjuntos);
	tmp.TryGetValue("coa_asunto", out coa_asunto);
	tmp.TryGetValue("coa_mensaje", out coa_mensaje);
	tmp.TryGetValue("coa_fechaenvio", out coa_fechaenvio);
	tmp.TryGetValue("coa_resultado", out coa_resultado);
	tmp.TryGetValue("coa_automatico", out coa_automatico);
	tmp.TryGetValue("coa_envios", out coa_envios);
	tmp.TryGetValue("coa_estado", out coa_estado);
	tmp.TryGetValue("crea_usr", out crea_usr);
	tmp.TryGetValue("crea_fecha", out crea_fecha);
	tmp.TryGetValue("mod_usr", out mod_usr);
	tmp.TryGetValue("mod_fecha", out mod_fecha);


                	this.coa_empresa = (Int32)Conversiones.GetValueByType(coa_empresa, typeof(Int32));
	this.coa_codigo = (Int32)Conversiones.GetValueByType(coa_codigo, typeof(Int32));
	this.coa_fecha = (DateTime?)Conversiones.GetValueByType(coa_fecha, typeof(DateTime?));
	this.coa_origen = (String)Conversiones.GetValueByType(coa_origen, typeof(String));
	this.coa_destinatario = (String)Conversiones.GetValueByType(coa_destinatario, typeof(String));
	this.coa_adjuntos = (String)Conversiones.GetValueByType(coa_adjuntos, typeof(String));
	this.coa_asunto = (String)Conversiones.GetValueByType(coa_asunto, typeof(String));
	this.coa_mensaje = (String)Conversiones.GetValueByType(coa_mensaje, typeof(String));
	this.coa_fechaenvio = (DateTime?)Conversiones.GetValueByType(coa_fechaenvio, typeof(DateTime?));
	this.coa_resultado = (String)Conversiones.GetValueByType(coa_resultado, typeof(String));
	this.coa_automatico = (Int32?)Conversiones.GetValueByType(coa_automatico, typeof(Int32?));
	this.coa_envios = (Int32?)Conversiones.GetValueByType(coa_envios, typeof(Int32?));
	this.coa_estado = (Int32?)Conversiones.GetValueByType(coa_estado, typeof(Int32?));
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
