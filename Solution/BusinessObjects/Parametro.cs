using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using Functions;

namespace BusinessObjects
{
    public class Parametro
    {
        #region Properties

    	[Data(key = true)]
	public Int32 par_empresa { get; set; }
	[Data(originalkey = true)]
	public Int32 par_empresa_key { get; set; }
	[Data(key = true)]
	public String par_id { get; set; }
	[Data(originalkey = true)]
	public String par_id_key { get; set; }
	public String par_descripcion { get; set; }
	public String par_tipo { get; set; }
	public String par_valor { get; set; }
	public Int32? par_estado { get; set; }
	public String crea_usr { get; set; }
	public DateTime? crea_fecha { get; set; }
	public String mod_usr { get; set; }
	public DateTime? mod_fecha { get; set; }

              
        #endregion

        #region Constructors


        public  Parametro()
        {
        }

        public  Parametro( Int32 par_empresa,String par_id,String par_descripcion,String par_tipo,String par_valor,Int32 par_estado,String crea_usr,DateTime crea_fecha,String mod_usr,DateTime mod_fecha)
        {                
    	this.par_empresa = par_empresa;
	this.par_id = par_id;
	this.par_descripcion = par_descripcion;
	this.par_tipo = par_tipo;
	this.par_valor = par_valor;
	this.par_estado = par_estado;
	this.crea_usr = crea_usr;
	this.crea_fecha = crea_fecha;
	this.mod_usr = mod_usr;
	this.mod_fecha = mod_fecha;

           
       }

        public  Parametro(IDataReader reader)
        {
    	this.par_empresa = (Int32)reader["par_empresa"];
	this.par_id = reader["par_id"].ToString();
	this.par_descripcion = reader["par_descripcion"].ToString();
	this.par_tipo = reader["par_tipo"].ToString();
	this.par_valor = reader["par_valor"].ToString();
	this.par_estado = (reader["par_estado"] != DBNull.Value) ? (Int32?)reader["par_estado"] : null;
	this.crea_usr = reader["crea_usr"].ToString();
	this.crea_fecha = (reader["crea_fecha"] != DBNull.Value) ? (DateTime?)reader["crea_fecha"] : null;
	this.mod_usr = reader["mod_usr"].ToString();
	this.mod_fecha = (reader["mod_fecha"] != DBNull.Value) ? (DateTime?)reader["mod_fecha"] : null;

        }


        public Parametro(object objeto)
        {            
            if (objeto != null)
            {
                Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
                	object par_empresa = null;
	object par_id = null;
	object par_descripcion = null;
	object par_tipo = null;
	object par_valor = null;
	object par_estado = null;
	object crea_usr = null;
	object crea_fecha = null;
	object mod_usr = null;
	object mod_fecha = null;


                	tmp.TryGetValue("par_empresa", out par_empresa);
	tmp.TryGetValue("par_id", out par_id);
	tmp.TryGetValue("par_descripcion", out par_descripcion);
	tmp.TryGetValue("par_tipo", out par_tipo);
	tmp.TryGetValue("par_valor", out par_valor);
	tmp.TryGetValue("par_estado", out par_estado);
	tmp.TryGetValue("crea_usr", out crea_usr);
	tmp.TryGetValue("crea_fecha", out crea_fecha);
	tmp.TryGetValue("mod_usr", out mod_usr);
	tmp.TryGetValue("mod_fecha", out mod_fecha);


                	this.par_empresa = (Int32)Conversiones.GetValueByType(par_empresa, typeof(Int32));
	this.par_id = (String)Conversiones.GetValueByType(par_id, typeof(String));
	this.par_descripcion = (String)Conversiones.GetValueByType(par_descripcion, typeof(String));
	this.par_tipo = (String)Conversiones.GetValueByType(par_tipo, typeof(String));
	this.par_valor = (String)Conversiones.GetValueByType(par_valor, typeof(String));
	this.par_estado = (Int32?)Conversiones.GetValueByType(par_estado, typeof(Int32?));
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
