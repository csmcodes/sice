using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlObjects
{
    public class Check
    {
        public string id { get; set; }
        public string etiqueta { get; set; }        
        public int? valor { get; set; }
        public string clase { get; set; }
        public bool obligatorio { get; set; }
        public bool habilitado { get; set; }
        public int? largo { get; set; }
        public bool password { get; set; }
        public string ayuda { get; set; }
        public bool visible { get; set; }
        public bool info { get; set; }               
        public bool autocomplete { get; set; }


        

        public Check()
        {
            obligatorio = false;
            habilitado = true;
            visible = true;
        }        

        public override string ToString()
        {
            StringBuilder html = new StringBuilder();
            if (!string.IsNullOrEmpty(etiqueta))
            {
                html.Append("<p>");
                html.AppendFormat("<label>{0}</label>", etiqueta);
            }
            
            
            html.AppendFormat("<input id = '{0}' type=\"checkbox\" class='{2}'  {1} {3} />", id, ((valor.HasValue) ? ((valor.Value == 1) ? "checked" : "") : ""), clase, ((!habilitado) ? "disabled='disabled'" : ""));             
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
