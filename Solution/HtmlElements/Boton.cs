using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlObjects
{
    public class Boton
    {
        public string id { get; set; }
        public string clase { get; set; }
        public string valor { get; set; }
        public string click { get; set; }
        public string tooltip { get; set; }
        public string texto { get; set; }

        public bool refresh { get; set; }
        public bool clean { get; set; }
        public bool removerow { get; set; }
        //public bool cleanrow { get; set; }

        public bool persona { get; set; }
        public bool small { get; set; }
        public bool personalized { get; set; }

        public Boton()
        {
        }        

        public override string ToString()
        {
            if (refresh)
            {
                return string.Format("<div class='btn' id='refresh' onclick='ReloadData()'><i class='iconsweets-refresh'></i>&nbsp; Refrescar</div> ");            
            }
            else if (clean)
            {
                return string.Format("<div class='btn' id='clean' onclick='CleanSearch()'><i class='iconsweets-pencil'></i>&nbsp; Limpiar</div> ");
            }
            else if (removerow)
            {
                //return string.Format("<a href='' class=\"deleterow\"><span class=\"icon-trash\" ></span></a>");
                return string.Format("<div class='smallbtn' onclick='RemoveRow(this)'  title='{0}'><span class=\"iconsweets-trashcan\" ></span></div>", tooltip);
            }
            else if (persona)
            {
                //return string.Format("<a href='' class=\"deleterow\"><span class=\"icon-trash\" ></span></a>");
                return string.Format("<div class='smallbtn' onclick='CallPersona()' title='{0}'  ><span class=\"iconsweets-user\" ></span></div>", tooltip);
            }
            else if (small)
            {
                return string.Format("<div id='{0}'  class='smallbtn' onclick='{1}' title='{2}'  ><span class=\"{3}\" ></span></div>", id, click, tooltip, clase);
            }
            else if (personalized)
            {
                return string.Format("<div class='btn' id='{0}' onclick='{1}' title='{2}'  ><i class='{3}'></i>&nbsp; {4}</div> ", id, click, tooltip, clase, texto);
            }
            //else if (cleanrow)
            //{
            //    return string.Format("<a href='' class=\"cleanrow\"><span class=\"icon-trash\" ></span></a>");
            //}    
            else
                return string.Format("<p class='stdformbutton'><button id='{0}' class='btn btn-primary {1}' onclick='{2}'>{3}</button></p>", id, clase, click, valor);

        } 
    }
}
