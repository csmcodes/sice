using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlObjects
{
    public class Textarea
    {
        public string id { get; set; }
        public string etiqueta { get; set; }
        public string placeholder { get; set; }
        public string valor { get; set; }
        public string clase { get; set; }
        public bool obligatorio { get; set; }
        public bool habilitado { get; set; }

        public int cols { get; set; }
        public int rows { get; set; }

        public int? largo { get; set; }
        public string ayuda { get; set; }
        public bool info { get; set; }
        

        public Textarea()
        {
            obligatorio = false;
            habilitado = true;            
        }

        public string GetValor()
        {

            return (valor != null) ? valor.ToString() : "";
        }

        public override string ToString()
        {

            StringBuilder html = new StringBuilder();

            if (!string.IsNullOrEmpty(etiqueta))
            {
                html.Append("<p>");
                html.AppendFormat("<label>{0}</label>", etiqueta);
            }
            html.AppendFormat("<textarea id = '{0}'", id);
            html.AppendFormat(" cols='{0}'", cols);
            html.AppendFormat(" rows='{0}'", rows);
             html.AppendFormat(" class='{0}' ", clase);
             html.AppendFormat(" placeholder='{0}' ", placeholder);             
            html.AppendFormat(" data-obligatorio='{0}' ", obligatorio);             
            html.AppendFormat(" {0} ", ((!habilitado) ? "disabled='disabled'" : "")); //disabled
            html.AppendFormat(" {0} ", ((largo.HasValue) ? "maxlength='" + largo.Value + "'" : "")); //maxlength            
            html.AppendFormat(">{0}</textarea>", GetValor());
            if (!string.IsNullOrEmpty(ayuda))
                html.AppendFormat("<span id='{0}' class='help-inline'>{1}</span>", id+"_help", ayuda);
            if(info)
                html.AppendFormat("<span id='{0}' class='help-inline'>{1}</span>", id+"_info","");
            if (!string.IsNullOrEmpty(etiqueta))
                html.Append("</p>");
            return html.ToString(); 
                      
        } 
    }
}
