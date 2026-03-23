using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace HtmlObjects
{
    public class LogicItem
    {
        public string nombre { get; set; }
        public int? valor { get; set; }

        public LogicItem()
        {
        }

        public LogicItem(string nombre, int? valor)
        {
            this.nombre = nombre;
            this.valor = valor;
        }
    }

    public class ListItem
    {
        public string id { get; set; }        
        public string[] titulo { get; set; }        
        public string[] descripcion { get; set; }
        public LogicItem[] logico { get; set; }        

        public string html { get; set; }

        public ListItem()
        {
        }
        

        public override string ToString()
        {

            StringBuilder html = new StringBuilder();
            html.AppendFormat("<h5>");
            foreach (string item in titulo)
	        {
                html.AppendLine(item); 		 
            }
            html.Append("</h5>");
            html.AppendFormat("<span class='pos'>");
            foreach (string item in descripcion)
            {
                html.AppendLine(item);
            }
            html.Append("</span>");
            html.AppendFormat("<span>");
            foreach (LogicItem item in logico)
            {
                html.AppendFormat("{0}: <input type='checkbox' {1} disabled/>",item.nombre,  ((item.valor.HasValue) ? ((item.valor.Value == 1) ? "checked" : "") : ""));
            }
            html.Append("</span>");
            return html.ToString();
        } 
    }
}
