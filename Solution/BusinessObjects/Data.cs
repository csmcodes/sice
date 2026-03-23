using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessObjects
{
    public class Data : System.Attribute
    {

        public bool key { get; set; }
        public bool auto { get; set; }
        public bool originalkey { get; set; }
        public bool noupdate { get; set; }

        public bool nosql { get; set; }
        public string tablaref { get; set; }
        public string camporef { get; set; }        
        public string foreign { get; set; }
        public string keyref { get; set; }
        public string join { get; set; }

        public bool noprop { get; set; }



    }
}
