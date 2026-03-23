using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using Functions;

namespace BusinessObjects
{
    public class Detalle
    {
        public int secuencia { get; set; }
        public string codigo { get; set; }
        public string codigoaux { get; set; }
        public string descripcion { get; set; }
        public string adicional1 { get; set; }
        public string adicional2 { get; set; }
        public string adicional3 { get; set; }
        public decimal? cantidad { get; set; }
        public decimal? precio { get; set; }
        public decimal? descuento { get; set; }
        public decimal? totalsinimp { get; set; }

        public int? grabaiva { get; set; }
        public int? grabaice { get; set; }

        public string codiva { get; set; }
        public decimal? porciva { get; set; }
        public decimal? valiva { get; set; }

        public string codice { get; set; }
        public decimal? porcice { get; set; }
        public decimal? valice { get; set; }


        //Para retencion
        public string codigoretencion { get; set; }
        public decimal? baseimponible{ get; set; }
        public decimal? porcentajeretener { get; set; }
        public decimal? valorretenido { get; set; }
        public string codigodocsustento{ get; set; }
        public string numdocsustento { get; set; }
        public string fechaemisiondocsustento { get; set; }


        //Para la guia de remision
        public string iddestinatario { get; set; }
        public string razondestinatario { get; set; }
        public string dirdestinatario { get; set; }
        public string motivotraslado { get; set; }
        public string docaduana { get; set; }
        public string codestabdestino { get; set; }
        public string numautsustento { get; set; }
        public string ruta { get; set; }
        public List<Subdetalle> subdetalles{ get; set; }





        public Detalle()
        {

        }

        public Detalle(object objeto)
        {
            if (objeto != null)
            {
                Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
                object secuencia = null;
                object codigo = null;
                object codigoaux = null;
                object descripcion = null;
                object adicional1 = null;
                object adicional2 = null;
                object adicional3 = null;
                object cantidad = null;
                object precio = null;
                object descuento = null;
                object totalsinimp = null;

                object grabaiva = null;
                object grabaice = null;

                object codiva = null;
                object porciva = null;
                object valiva = null;

                object codice = null;
                object porcice = null;
                object valice = null;


                tmp.TryGetValue("secuencia", out secuencia);
                tmp.TryGetValue("codigo", out codigo);
                tmp.TryGetValue("codigoaux", out codigoaux);
                tmp.TryGetValue("descripcion", out descripcion);
                tmp.TryGetValue("adicional1", out adicional1);
                tmp.TryGetValue("adicional2", out adicional2);
                tmp.TryGetValue("adicional3", out adicional3);
                tmp.TryGetValue("cantidad", out cantidad);
                tmp.TryGetValue("precio", out precio);
                tmp.TryGetValue("descuento", out descuento);
                tmp.TryGetValue("totalsinimp", out totalsinimp);
                tmp.TryGetValue("grabaiva", out grabaiva);
                tmp.TryGetValue("grabaice", out grabaice);
                tmp.TryGetValue("codiva", out codiva);
                tmp.TryGetValue("porciva", out porciva);
                tmp.TryGetValue("valiva", out valiva);

                tmp.TryGetValue("codice", out codice);
                tmp.TryGetValue("porcice", out porcice);
                tmp.TryGetValue("valice", out valice);


                this.secuencia = (Int32)Conversiones.GetValueByType(secuencia, typeof(Int32));
                this.codigo = (String)Conversiones.GetValueByType(codigo, typeof(String));
                this.codigoaux = (String)Conversiones.GetValueByType(codigoaux, typeof(String));
                this.descripcion = (String)Conversiones.GetValueByType(descripcion, typeof(String));
                this.adicional1 = (String)Conversiones.GetValueByType(adicional1, typeof(String));
                this.adicional2 = (String)Conversiones.GetValueByType(adicional2, typeof(String));
                this.adicional3 = (String)Conversiones.GetValueByType(adicional3, typeof(String));
                this.cantidad = (Decimal?)Conversiones.GetValueByType(cantidad, typeof(Decimal?));
                this.precio = (Decimal?)Conversiones.GetValueByType(precio, typeof(Decimal?));
                this.descuento = (Decimal?)Conversiones.GetValueByType(descuento, typeof(Decimal?));
                this.totalsinimp = (Decimal?)Conversiones.GetValueByType(totalsinimp, typeof(Decimal?));
                this.grabaiva = (Int32?)Conversiones.GetValueByType(grabaiva, typeof(Int32?));
                this.grabaice = (Int32?)Conversiones.GetValueByType(grabaice, typeof(Int32?));

                this.codiva = (String)Conversiones.GetValueByType(codiva, typeof(String));
                this.porciva = (Decimal?)Conversiones.GetValueByType(porciva, typeof(Decimal?));
                this.valiva = (Decimal?)Conversiones.GetValueByType(valiva, typeof(Decimal?));
                this.codice = (String)Conversiones.GetValueByType(codice, typeof(String));
                this.porcice = (Decimal?)Conversiones.GetValueByType(porcice, typeof(Decimal?));
                this.valice = (Decimal?)Conversiones.GetValueByType(valice, typeof(Decimal?));
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
