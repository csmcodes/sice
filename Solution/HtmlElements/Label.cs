using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlObjects
{
    public class Label
    {
        public string id { get; set; }
        public string etiqueta { get; set; }
        public string clase { get; set; }
        public bool ejemplo { get; set; }
        public Label()
        {
        }        

        public override string ToString()
        {
            return string.Format("<label for='{0}' class='control-label {1}'>{2}</label>", id, clase , etiqueta); 
        } 
    }
}
