using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using Functions;

namespace BusinessObjects
{
    public class SriReglaMensaje
    {
        #region Properties

        [Data(key = true, auto = true)]
        public Int32 srm_id { get; set; }
        [Data(originalkey = true)]
        public Int32 srm_id_key { get; set; }
        public String srm_patron { get; set; }
        public String srm_accion { get; set; }
        public Int32? srm_estado { get; set; }
        public Int32? srm_orden { get; set; }
        public String srm_descripcion { get; set; }
        public Boolean? srm_activo { get; set; }

        #endregion

        #region Constructors

        public SriReglaMensaje()
        {
        }

        public SriReglaMensaje(Int32 srm_id, String srm_patron, String srm_accion, Int32? srm_estado, Int32? srm_orden, String srm_descripcion, Boolean? srm_activo)
        {
            this.srm_id = srm_id;
            this.srm_patron = srm_patron;
            this.srm_accion = srm_accion;
            this.srm_estado = srm_estado;
            this.srm_orden = srm_orden;
            this.srm_descripcion = srm_descripcion;
            this.srm_activo = srm_activo;
        }

        public SriReglaMensaje(IDataReader reader)
        {
            this.srm_id = (Int32)reader["srm_id"];
            this.srm_patron = reader["srm_patron"].ToString();
            this.srm_accion = reader["srm_accion"].ToString();
            this.srm_estado = (reader["srm_estado"] != DBNull.Value) ? (Int32?)reader["srm_estado"] : null;
            this.srm_orden = (reader["srm_orden"] != DBNull.Value) ? (Int32?)reader["srm_orden"] : null;
            this.srm_descripcion = reader["srm_descripcion"].ToString();
            this.srm_activo = (reader["srm_activo"] != DBNull.Value) ? (Boolean?)reader["srm_activo"] : null;
        }

        public SriReglaMensaje(object objeto)
        {
            if (objeto != null)
            {
                Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
                object srm_id = null;
                object srm_patron = null;
                object srm_accion = null;
                object srm_estado = null;
                object srm_orden = null;
                object srm_descripcion = null;
                object srm_activo = null;

                tmp.TryGetValue("srm_id", out srm_id);
                tmp.TryGetValue("srm_patron", out srm_patron);
                tmp.TryGetValue("srm_accion", out srm_accion);
                tmp.TryGetValue("srm_estado", out srm_estado);
                tmp.TryGetValue("srm_orden", out srm_orden);
                tmp.TryGetValue("srm_descripcion", out srm_descripcion);
                tmp.TryGetValue("srm_activo", out srm_activo);

                this.srm_id = (Int32)Conversiones.GetValueByType(srm_id, typeof(Int32));
                this.srm_patron = (String)Conversiones.GetValueByType(srm_patron, typeof(String));
                this.srm_accion = (String)Conversiones.GetValueByType(srm_accion, typeof(String));
                this.srm_estado = (Int32?)Conversiones.GetValueByType(srm_estado, typeof(Int32?));
                this.srm_orden = (Int32?)Conversiones.GetValueByType(srm_orden, typeof(Int32?));
                this.srm_descripcion = (String)Conversiones.GetValueByType(srm_descripcion, typeof(String));
                this.srm_activo = (Boolean?)Conversiones.GetValueByType(srm_activo, typeof(Boolean?));
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
