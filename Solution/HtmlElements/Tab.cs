using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlObjects
{
    public class Tab
    {
        public string id { get; set; }
        public string titulo { get; set; }
        public string html { get; set; }

        public Tab()
        {
        }

        public Tab(string id, string titulo, string html)
        {
            this.id = id;
            this.titulo = titulo;
            this.html = html;
        }
    }
}
