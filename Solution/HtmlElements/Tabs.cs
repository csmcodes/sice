using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlObjects
{
    public class Tabs
    {
        public string id { get; set; }
        public string clase { get; set; }
        public List<Tab> tabs { get; set; }

         public Tabs()
        {
            
        }        

        public override string ToString()
        {

            StringBuilder htmltitulos = new StringBuilder();
            StringBuilder htmlcontent = new StringBuilder();
            foreach (Tab tab in tabs)
            {
                htmltitulos.AppendFormat("<li><a href=\"#{0}\">{1}</a></li>", tab.id, tab.titulo);
                htmlcontent.AppendFormat("<div id=\"{0}\">{1}</div>", tab.id, tab.html);

            }

            return string.Format("<div id={0} class=\"tabbedwidget {1}\"><ul>{2}</ul>{3} </div>", id, clase, htmltitulos.ToString(), htmlcontent.ToString());

        } 

    }
}
