using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using Functions;

namespace BusinessObjects
{
    public class Archivo: IDisposable
    {
        #region Properties

    	[Data(key = true)]
	public Int32 arc_empresa { get; set; }
	[Data(originalkey = true)]
	public Int32 arc_empresa_key { get; set; }
	[Data(key = true)]
	public String arc_numero { get; set; }
	[Data(originalkey = true)]
	public String arc_numero_key { get; set; }
	public String arc_xml { get; set; }
	public String arc_xmlrespuesta { get; set; }
	public Int32? arc_estado { get; set; }
	public String crea_usr { get; set; }
	public DateTime? crea_fecha { get; set; }
	public String mod_usr { get; set; }
	public DateTime? mod_fecha { get; set; }

              
        #endregion

        #region Constructors


        public  Archivo()
        {
        }

        public  Archivo( Int32 arc_empresa,String arc_numero,String arc_xml,String arc_xmlrespuesta,Int32 arc_estado,String crea_usr,DateTime crea_fecha,String mod_usr,DateTime mod_fecha)
        {                
    	this.arc_empresa = arc_empresa;
	this.arc_numero = arc_numero;
	this.arc_xml = arc_xml;
	this.arc_xmlrespuesta = arc_xmlrespuesta;
	this.arc_estado = arc_estado;
	this.crea_usr = crea_usr;
	this.crea_fecha = crea_fecha;
	this.mod_usr = mod_usr;
	this.mod_fecha = mod_fecha;

           
       }

        public  Archivo(IDataReader reader)
        {
    	this.arc_empresa = (Int32)reader["arc_empresa"];
	this.arc_numero = reader["arc_numero"].ToString();
	this.arc_xml = reader["arc_xml"].ToString();
	this.arc_xmlrespuesta = reader["arc_xmlrespuesta"].ToString();
	this.arc_estado = (reader["arc_estado"] != DBNull.Value) ? (Int32?)reader["arc_estado"] : null;
	this.crea_usr = reader["crea_usr"].ToString();
	this.crea_fecha = (reader["crea_fecha"] != DBNull.Value) ? (DateTime?)reader["crea_fecha"] : null;
	this.mod_usr = reader["mod_usr"].ToString();
	this.mod_fecha = (reader["mod_fecha"] != DBNull.Value) ? (DateTime?)reader["mod_fecha"] : null;

        }


        public Archivo(object objeto)
        {            
            if (objeto != null)
            {
                Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
                	object arc_empresa = null;
	object arc_numero = null;
	object arc_xml = null;
	object arc_xmlrespuesta = null;
	object arc_estado = null;
	object crea_usr = null;
	object crea_fecha = null;
	object mod_usr = null;
	object mod_fecha = null;


                	tmp.TryGetValue("arc_empresa", out arc_empresa);
	tmp.TryGetValue("arc_numero", out arc_numero);
	tmp.TryGetValue("arc_xml", out arc_xml);
	tmp.TryGetValue("arc_xmlrespuesta", out arc_xmlrespuesta);
	tmp.TryGetValue("arc_estado", out arc_estado);
	tmp.TryGetValue("crea_usr", out crea_usr);
	tmp.TryGetValue("crea_fecha", out crea_fecha);
	tmp.TryGetValue("mod_usr", out mod_usr);
	tmp.TryGetValue("mod_fecha", out mod_fecha);


                	this.arc_empresa = (Int32)Conversiones.GetValueByType(arc_empresa, typeof(Int32));
	this.arc_numero = (String)Conversiones.GetValueByType(arc_numero, typeof(String));
	this.arc_xml = (String)Conversiones.GetValueByType(arc_xml, typeof(String));
	this.arc_xmlrespuesta = (String)Conversiones.GetValueByType(arc_xmlrespuesta, typeof(String));
	this.arc_estado = (Int32?)Conversiones.GetValueByType(arc_estado, typeof(Int32?));
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


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }
        protected virtual void Dispose(bool disposing)
        {

            if (disposing)
            {
                // Clear all property values that maybe have been set
                // when the class was instantiated
                arc_numero = String.Empty;
                arc_numero_key = String.Empty;
                arc_xml = String.Empty;
                arc_xmlrespuesta = String.Empty;
                arc_estado = null;
                crea_usr = String.Empty;
                crea_fecha = null;
                mod_usr = String.Empty;
                mod_fecha = null;
               
            }


        }


        #endregion


    }
}
