using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using Functions;

namespace BusinessObjects
{
	public class Usuario
	{
		#region Properties

		[Data(key = true)]
		public String usr_id { get; set; }
		[Data(originalkey = true)]
		public String usr_id_key { get; set; }
		public String usr_password { get; set; }
		public String usr_nombres { get; set; }
		public String usr_mail { get; set; }
		public String usr_perfil { get; set; }
		public Int32? usr_edit { get; set; }
		public Int32? usr_estado { get; set; }
		[Data(noupdate = true)]
		public String crea_usr { get; set; }
		[Data(noupdate = true)]
		public DateTime? crea_fecha { get; set; }

		public String mod_usr { get; set; }
		public DateTime? mod_fecha { get; set; }


		#endregion

		#region Constructors


		public Usuario()
		{
		}

		public Usuario(String usr_id, String usr_password, String usr_nombres, String usr_mail, String usr_perfil, Int32 usr_estado, String crea_usr, DateTime crea_fecha, String mod_usr, DateTime mod_fecha)
		{
			this.usr_id = usr_id;
			this.usr_password = usr_password;
			this.usr_nombres = usr_nombres;
			this.usr_mail = usr_mail;
			this.usr_perfil = usr_perfil;
			this.usr_estado = usr_estado;
			this.crea_usr = crea_usr;
			this.crea_fecha = crea_fecha;
			this.mod_usr = mod_usr;
			this.mod_fecha = mod_fecha;


		}

		public Usuario(IDataReader reader)
		{
			this.usr_id = reader["usr_id"].ToString();
			this.usr_password = reader["usr_password"].ToString();
			this.usr_nombres = reader["usr_nombres"].ToString();
			this.usr_mail = reader["usr_mail"].ToString();
			this.usr_perfil = reader["usr_perfil"].ToString();
			this.usr_edit = (reader["usr_edit"] != DBNull.Value) ? (Int32?)reader["usr_edit"] : null;
			this.usr_estado = (reader["usr_estado"] != DBNull.Value) ? (Int32?)reader["usr_estado"] : null;
			this.crea_usr = reader["crea_usr"].ToString();
			this.crea_fecha = (reader["crea_fecha"] != DBNull.Value) ? (DateTime?)reader["crea_fecha"] : null;
			this.mod_usr = reader["mod_usr"].ToString();
			this.mod_fecha = (reader["mod_fecha"] != DBNull.Value) ? (DateTime?)reader["mod_fecha"] : null;

		}


		public Usuario(object objeto)
		{
			if (objeto != null)
			{
				Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
				object usr_id = null;
				object usr_password = null;
				object usr_nombres = null;
				object usr_mail = null;
				object usr_perfil = null;
				object usr_edit = null;
				object usr_estado = null;
				object crea_usr = null;
				object crea_fecha = null;
				object mod_usr = null;
				object mod_fecha = null;


				tmp.TryGetValue("usr_id", out usr_id);
				tmp.TryGetValue("usr_password", out usr_password);
				tmp.TryGetValue("usr_nombres", out usr_nombres);
				tmp.TryGetValue("usr_mail", out usr_mail);
				tmp.TryGetValue("usr_perfil", out usr_perfil);
				tmp.TryGetValue("usr_edit", out usr_edit);
				tmp.TryGetValue("usr_estado", out usr_estado);
				tmp.TryGetValue("crea_usr", out crea_usr);
				tmp.TryGetValue("crea_fecha", out crea_fecha);
				tmp.TryGetValue("mod_usr", out mod_usr);
				tmp.TryGetValue("mod_fecha", out mod_fecha);


				this.usr_id = (String)Conversiones.GetValueByType(usr_id, typeof(String));
				this.usr_password = (String)Conversiones.GetValueByType(usr_password, typeof(String));
				this.usr_nombres = (String)Conversiones.GetValueByType(usr_nombres, typeof(String));
				this.usr_mail = (String)Conversiones.GetValueByType(usr_mail, typeof(String));
				this.usr_perfil = (String)Conversiones.GetValueByType(usr_perfil, typeof(String));
				this.usr_edit = (Int32?)Conversiones.GetValueByType(usr_edit, typeof(Int32?));
				this.usr_estado = (Int32?)Conversiones.GetValueByType(usr_estado, typeof(Int32?));
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
