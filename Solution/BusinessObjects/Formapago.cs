using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using Functions;

namespace BusinessObjects
{
    public class Formapago
    {
        public int secuencia { get; set; }
        public string codigo { get; set; }
        public string forma { get; set; }
        public decimal? valor { get; set; }
        public int? plazo { get; set; }
        public string tiempo { get; set; }


        public Formapago()
        {

        }

        public Formapago(object objeto)
        {
            if (objeto != null)
            {
                Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
                object secuencia = null;
                object codigo = null;
                object forma = null;
                object valor = null;
                object plazo = null;
                object tiempo = null;



                tmp.TryGetValue("secuencia", out secuencia);
                tmp.TryGetValue("codigo", out codigo);
                tmp.TryGetValue("forma", out forma);
                tmp.TryGetValue("valor", out valor);
                tmp.TryGetValue("plazo", out plazo);
                tmp.TryGetValue("tiempo", out tiempo);


                this.secuencia = (Int32)Conversiones.GetValueByType(secuencia, typeof(Int32));
                this.codigo = (String)Conversiones.GetValueByType(codigo, typeof(String));
                this.forma = (String)Conversiones.GetValueByType(forma, typeof(String));
                this.valor = (decimal?)Conversiones.GetValueByType(valor, typeof(decimal?));
                this.plazo = (int?)Conversiones.GetValueByType(plazo, typeof(int?));
                this.tiempo = (String)Conversiones.GetValueByType(tiempo, typeof(String));

            }
        }
        #region Methods
        public PropertyInfo[] GetProperties()
        {
            return this.GetType().GetProperties();
        }
        #endregion
    }


}
