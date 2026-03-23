using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using Functions;

namespace BusinessObjects
{
    public class Usuarioxempresa
    {
        #region Properties

    	[Data(key = true)]
	public String uxe_usuario { get; set; }
	[Data(originalkey = true)]
	public String uxe_usuario_key { get; set; }
	[Data(key = true)]
	public Int32 uxe_empresa { get; set; }
	[Data(originalkey = true)]
	public Int32 uxe_empresa_key { get; set; }
	public Int32? uxe_estado { get; set; }
	public String crea_usr { get; set; }
	public DateTime? crea_fecha { get; set; }
	public String mod_usr { get; set; }
	public DateTime? mod_fecha { get; set; }

              
        #endregion

        #region Constructors


        public  Usuarioxempresa()
        {
        }

        public  Usuarioxempresa( String uxe_usuario,Int32 uxe_empresa,Int32 uxe_estado,String crea_usr,DateTime crea_fecha,String mod_usr,DateTime mod_fecha)
        {                
    	this.uxe_usuario = uxe_usuario;
	this.uxe_empresa = uxe_empresa;
	this.uxe_estado = uxe_estado;
	this.crea_usr = crea_usr;
	this.crea_fecha = crea_fecha;
	this.mod_usr = mod_usr;
	this.mod_fecha = mod_fecha;

           
       }

        public  Usuarioxempresa(IDataReader reader)
        {
    	this.uxe_usuario = reader["uxe_usuario"].ToString();
	this.uxe_empresa = (Int32)reader["uxe_empresa"];
	this.uxe_estado = (reader["uxe_estado"] != DBNull.Value) ? (Int32?)reader["uxe_estado"] : null;
	this.crea_usr = reader["crea_usr"].ToString();
	this.crea_fecha = (reader["crea_fecha"] != DBNull.Value) ? (DateTime?)reader["crea_fecha"] : null;
	this.mod_usr = reader["mod_usr"].ToString();
	this.mod_fecha = (reader["mod_fecha"] != DBNull.Value) ? (DateTime?)reader["mod_fecha"] : null;

        }


        public Usuarioxempresa(object objeto)
        {            
            if (objeto != null)
            {
                Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
                	object uxe_usuario = null;
	object uxe_empresa = null;
	object uxe_estado = null;
	object crea_usr = null;
	object crea_fecha = null;
	object mod_usr = null;
	object mod_fecha = null;


                	tmp.TryGetValue("uxe_usuario", out uxe_usuario);
	tmp.TryGetValue("uxe_empresa", out uxe_empresa);
	tmp.TryGetValue("uxe_estado", out uxe_estado);
	tmp.TryGetValue("crea_usr", out crea_usr);
	tmp.TryGetValue("crea_fecha", out crea_fecha);
	tmp.TryGetValue("mod_usr", out mod_usr);
	tmp.TryGetValue("mod_fecha", out mod_fecha);


                	this.uxe_usuario = (String)Conversiones.GetValueByType(uxe_usuario, typeof(String));
	this.uxe_empresa = (Int32)Conversiones.GetValueByType(uxe_empresa, typeof(Int32));
	this.uxe_estado = (Int32?)Conversiones.GetValueByType(uxe_estado, typeof(Int32?));
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
