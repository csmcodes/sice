using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using Functions;

namespace BusinessObjects
{
    public class Subdetalle
    {
        //Para la guia de remision
        public string codigointerno { get; set; }
        public string codigoadicional { get; set; }
        public string descripcion { get; set; }
        public string adicional1{ get; set; }
        public string adicional2 { get; set; }
        public string adicional3 { get; set; }
        public decimal? cantidad { get; set; }

    }
}
