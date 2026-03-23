using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using Functions;

namespace BusinessObjects
{
    public class Sysdiagrams
    {
        #region Properties

    	public String name { get; set; }
	public Int32 principal_id { get; set; }
	[Data(key = true)]
	public Int32 diagram_id { get; set; }
	[Data(originalkey = true)]
	public Int32 diagram_id_key { get; set; }
	public Int32? version { get; set; }
	public String definition { get; set; }

              
        #endregion

        #region Constructors


        public  Sysdiagrams()
        {
        }

        public  Sysdiagrams( String name,Int32 principal_id,Int32 diagram_id,Int32 version,String definition)
        {                
    	this.name = name;
	this.principal_id = principal_id;
	this.diagram_id = diagram_id;
	this.version = version;
	this.definition = definition;

           
       }

        public  Sysdiagrams(IDataReader reader)
        {
    	this.name = reader["name"].ToString();
	this.principal_id = (Int32)reader["principal_id"];
	this.diagram_id = (Int32)reader["diagram_id"];
	this.version = (reader["version"] != DBNull.Value) ? (Int32?)reader["version"] : null;
	this.definition = reader["definition"].ToString();

        }


        public Sysdiagrams(object objeto)
        {            
            if (objeto != null)
            {
                Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
                	object name = null;
	object principal_id = null;
	object diagram_id = null;
	object version = null;
	object definition = null;


                	tmp.TryGetValue("name", out name);
	tmp.TryGetValue("principal_id", out principal_id);
	tmp.TryGetValue("diagram_id", out diagram_id);
	tmp.TryGetValue("version", out version);
	tmp.TryGetValue("definition", out definition);


                	this.name = (String)Conversiones.GetValueByType(name, typeof(String));
	this.principal_id = (Int32)Conversiones.GetValueByType(principal_id, typeof(Int32));
	this.diagram_id = (Int32)Conversiones.GetValueByType(diagram_id, typeof(Int32));
	this.version = (Int32?)Conversiones.GetValueByType(version, typeof(Int32?));
	this.definition = (String)Conversiones.GetValueByType(definition, typeof(String));

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
