using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using Functions;

namespace BusinessObjects
{
    public class Perfil
    {
        #region Properties

    	[Data(key = true)]
	public String per_id { get; set; }
	[Data(originalkey = true)]
	public String per_id_key { get; set; }
	public String per_descripcion { get; set; }
	public Int32? per_estado { get; set; }
	public String crea_usr { get; set; }
	public DateTime? crea_fecha { get; set; }
	public String mod_usr { get; set; }
	public DateTime? mod_fecha { get; set; }

              
        #endregion

        #region Constructors


        public  Perfil()
        {
        }

        public  Perfil( String per_id,String per_descripcion,Int32 per_estado,String crea_usr,DateTime crea_fecha,String mod_usr,DateTime mod_fecha)
        {                
    	this.per_id = per_id;
	this.per_descripcion = per_descripcion;
	this.per_estado = per_estado;
	this.crea_usr = crea_usr;
	this.crea_fecha = crea_fecha;
	this.mod_usr = mod_usr;
	this.mod_fecha = mod_fecha;

           
       }

        public  Perfil(IDataReader reader)
        {
    	this.per_id = reader["per_id"].ToString();
	this.per_descripcion = reader["per_descripcion"].ToString();
	this.per_estado = (reader["per_estado"] != DBNull.Value) ? (Int32?)reader["per_estado"] : null;
	this.crea_usr = reader["crea_usr"].ToString();
	this.crea_fecha = (reader["crea_fecha"] != DBNull.Value) ? (DateTime?)reader["crea_fecha"] : null;
	this.mod_usr = reader["mod_usr"].ToString();
	this.mod_fecha = (reader["mod_fecha"] != DBNull.Value) ? (DateTime?)reader["mod_fecha"] : null;

        }


        public Perfil(object objeto)
        {            
            if (objeto != null)
            {
                Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
                	object per_id = null;
	object per_descripcion = null;
	object per_estado = null;
	object crea_usr = null;
	object crea_fecha = null;
	object mod_usr = null;
	object mod_fecha = null;


                	tmp.TryGetValue("per_id", out per_id);
	tmp.TryGetValue("per_descripcion", out per_descripcion);
	tmp.TryGetValue("per_estado", out per_estado);
	tmp.TryGetValue("crea_usr", out crea_usr);
	tmp.TryGetValue("crea_fecha", out crea_fecha);
	tmp.TryGetValue("mod_usr", out mod_usr);
	tmp.TryGetValue("mod_fecha", out mod_fecha);


                	this.per_id = (String)Conversiones.GetValueByType(per_id, typeof(String));
	this.per_descripcion = (String)Conversiones.GetValueByType(per_descripcion, typeof(String));
	this.per_estado = (Int32?)Conversiones.GetValueByType(per_estado, typeof(Int32?));
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
