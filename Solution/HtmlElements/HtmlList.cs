using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlObjects
{
    public class HtmlList
    {
          public string id { get; set; }
          public string content { get; set; }          
        
        public HtmlList()
        {
        }
        

        public override string ToString()
        {
            StringBuilder html = new StringBuilder();            
            html.AppendLine("<li onmousedown=\"Select(this)\"><div class='uinfo' data-id='" + id + "' >");
            html.AppendLine(content);
            html.AppendLine("</div></li>");
            return html.ToString(); 
        } 
    }
}
