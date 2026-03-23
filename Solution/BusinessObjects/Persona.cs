using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessObjects
{
    public class Persona:IDisposable
    {
        #region Properties

        public String per_id { get; set; }
        public String per_tipoid { get; set; }
        public String per_nombres { get; set; }
        public String per_apellidos { get; set; }
        public String per_razon { get; set; }
        public String per_provincia { get; set; }
        public String per_canton { get; set; }
        public String per_parroquia { get; set; }
        public String per_direccion { get; set; }
        public String per_telefono { get; set; }
        public String per_email { get; set; }
     

        #endregion

        #region Constructors


        public Persona()
        {
        }



        #endregion

        public override string ToString()
        {
            return   per_id + ";" + per_tipoid + ";;;" + per_razon + ";;;;" + per_direccion + ";" + per_telefono + ";" + per_email + ";1;admin;" + DateTime.Now.ToShortDateString();
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
                per_id = String.Empty;
                per_tipoid = String.Empty;
                per_nombres = String.Empty;
                per_apellidos = String.Empty;
                per_razon = String.Empty;
                per_provincia = String.Empty;
                per_parroquia = String.Empty;
                per_direccion = String.Empty;
                per_telefono = String.Empty;
                per_email = String.Empty;
            }


        }
    }
}
