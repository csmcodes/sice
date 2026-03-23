using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlObjects
{
    public class Auditoria
    {
        public string id { get; set; }
        public string creausr { get; set; }
        public DateTime? creafecha { get; set; }
        public string modusr { get; set; }
        public DateTime? modfecha { get; set; }
        
        public Auditoria()
        {
        }        

        public override string ToString()
        {
            return string.Format("<p><small class='desc' id='lblcrea'>Creado por:{0} {1}</small><small class='desc' id='lblmod'>Modificado por:{2} {3}</small></p>", creausr, creafecha, modusr, modfecha);
        } 
    }
}
