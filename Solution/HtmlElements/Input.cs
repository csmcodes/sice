using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlObjects
{
    public class Input
    { 
        public string id { get; set; }
        public string etiqueta { get; set; }
        public string placeholder { get; set; }
        public object valor { get; set; }
        public string clase { get; set; }
        public bool obligatorio { get; set; }
        public bool habilitado { get; set; }
        public int? largo { get; set; }
        public bool password { get; set; }
        public string ayuda { get; set; }
        public bool visible { get; set; }
        public bool info { get; set; }
        public DateTime? datetimevalor { get; set; }
        public bool datepicker { get; set; }
        public bool timepicker { get; set; }
        public string prepend { get; set; }
        public string append { get; set; }
        public bool numeric { get; set; }               
        public string format { get; set; }
        public string data { get; set; }
                
        

        public string autocomplete { get; set; }
        

        public Input()
        {
            obligatorio = false;
            habilitado = true;
            visible = true;
            numeric = false;
        }

        public string GetValor()
        {
            if (datepicker )
            {
                if (datetimevalor.HasValue)
                    return datetimevalor.Value.ToShortDateString();
                else return "";
            }
            else if (timepicker)
                if (datetimevalor.HasValue)
                    return datetimevalor.Value.ToShortTimeString();
                else return "";
            else
                return (valor != null) ? valor.ToString() : ""; 
        }

        public override string ToString()
        {


            if (datepicker)
                clase += " fecha";
            if (timepicker)
                clase += " hora";
            
            StringBuilder html = new StringBuilder();

            if (!string.IsNullOrEmpty(etiqueta))
            {
                html.Append("<p>");
                html.AppendFormat("<label>{0}</label>", etiqueta);
            }

            if (!string.IsNullOrEmpty(prepend) || !string.IsNullOrEmpty(append) || timepicker)
            {
                html.AppendFormat("<div class='{0} {1} {2}'>", ((!string.IsNullOrEmpty(prepend)) ? "input-prepend" : ""), ((!string.IsNullOrEmpty(append)) ? "input-append" : ""), ((timepicker) ? "bootstrap-timepicker " : ""));
                if (!string.IsNullOrEmpty(prepend))
                    html.AppendFormat("<span class='add-on'>{0}</span>", prepend);
               
            }
            
            //Input
            html.AppendFormat("<input id = '{0}'", id);
            html.AppendFormat(" type='{0}' ", ((password) ? "password" : ((!visible) ? "hidden" : "text")));
            html.AppendFormat(" placeholder='{0}' ", placeholder);
            html.AppendFormat(" value='{0}' ", GetValor());
            html.AppendFormat(" class='{0}' ", clase);
            html.AppendFormat(" data-obligatorio='{0}' ", obligatorio);
            html.AppendFormat(" {0} ", data);
            if (!string.IsNullOrEmpty(autocomplete))
                html.AppendFormat(" data-autocomplete='{0}' ", autocomplete);
            html.AppendFormat(" {0} ", ((!habilitado) ? "disabled='disabled'" : "")); //disabled
            html.AppendFormat(" {0} ", ((largo.HasValue) ? "maxlength='" + largo.Value + "'" : "")); //maxlength
            if (numeric)
            {
                html.Append(" data-numeric='si' ");                
            }
            html.Append(" />");

            if (!string.IsNullOrEmpty(prepend) || !string.IsNullOrEmpty(append) || timepicker )
            {
                if (!string.IsNullOrEmpty(append))
                    html.AppendFormat("<span class='add-on'>{0}</span>", append);
                //if (timepicker)
                //    html.AppendLine("<span class='add-on'><i class=\"iconfa-time\"></i></span>");
                html.AppendLine("</div>"); 
            }

            //
            //html.AppendFormat("<input id = '{0}' type='{7}'  placeholder='{1}' value=\"{2}\" class='{3}' data-obligatorio='{4}' {5} {6} />", id, placeholder, valor, clase, obligatorio, ((!habilitado) ? "disabled='disabled'" : ""), ((largo.HasValue) ? "maxlength='" + largo.Value + "'" : ""), ((password) ? "password" : ((!visible)?"hidden":"text")));
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
