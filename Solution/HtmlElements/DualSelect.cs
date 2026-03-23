using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlObjects
{
    public class DualSelect
    {

        public string id { get; set; }
        public string etiqueta { get; set; }
        public string clase { get; set; }
        public string origen { get; set; }
        public string destino { get; set; }

         public DualSelect()
        {
            
        }        

        public override string ToString()
        {
            StringBuilder html = new StringBuilder();
            if (!string.IsNullOrEmpty(etiqueta))
            {
                html.Append("<p>");
                html.AppendFormat("<label>{0}</label>", etiqueta);
            }

            html.AppendFormat("<span id='{0}' class='dualselect {1}'>", id, clase);
            html.AppendLine(origen);
            html.AppendLine("<span class='ds_arrow'><button class='btn ds_prev'><i class='iconfa-chevron-left'></i></button><br />");
            html.AppendLine("<button class='btn ds_next'><i class='iconfa-chevron-right'></i></button></span>");
            html.AppendLine(destino);
            html.AppendLine("</span>");
            if (!string.IsNullOrEmpty(etiqueta))
                html.Append("</p>");
            return html.ToString(); 
  
            

        } 
    }
}
