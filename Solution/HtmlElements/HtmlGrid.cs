using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlObjects
{

    public class HtmlGridCol
    {
        public string id { get; set; }
        public string titulo { get; set; }
        public string clase { get; set; }
        public string estilo { get; set; }
        public string control { get; set; }
        public string controlfoot { get; set; }
        
        public HtmlGridCol()
        {
        }


    }

    public class HtmlGridRow
    {
        public string id { get; set; }
        public string clase { get; set; }
        public string estilo { get; set; }
        public List<HtmlGridCell> cells { get; set; }
        //public List<HtmlCols> cols { get; set; }
        public bool header { get; set; }
        
        public bool editable { get; set; }
        public bool selectable { get; set; }
        public string data { get; set; }
        public bool removable { get; set; }
        public bool markable { get; set; }
        public string clickevent { get; set; }
        public string contextevent { get; set; }

        public HtmlGridRow()
        {
            cells = new List<HtmlGridCell>();
        }


        /*public HtmlRow(params object[] valores)
        {
            cells = new List<HtmlCell>();
            foreach (var item in valores)
            {
                cells.Add(new HtmlCell { valor = item.ToString() });
            }
        }*/

        public override string ToString()
        {
            
            StringBuilder html = new StringBuilder();
            html.AppendFormat("<tr id='{0}' class='{1}' {2} {3}>", id, clase, ((clickevent!="")?"onclick='"+clickevent+"'":"") ,((contextevent!="")?"oncontextmenu='"+contextevent+"'":""));
            foreach (HtmlGridCell cell in cells)
            {
                /*cell.header = this.header;
                if (cols.Count > c)
                {
                    if (string.IsNullOrEmpty(cell.alienacion))
                        cell.alienacion = cols[c].alineacion;
                    if (string.IsNullOrEmpty(cell.clase))
                        cell.clase = cols[c].clase;
                }*/
                html.AppendLine(cell.ToString());
            }
            html.AppendLine("</tr>");
            return html.ToString();

        }



    }

    public class HtmlGridCell
    {
        public string id { get; set; }
        public string clase { get; set; }
        public string estilo { get; set; }
        public object valor { get; set; }
        public bool header { get; set; }
        public string data { get; set; }
        //public int? ancho { get; set; }

        public string GetValor()
        {
            return (valor != null) ? valor.ToString() : "";
        }

        public override string ToString()
        {
            StringBuilder html = new StringBuilder();
            html.AppendFormat("<{0} id='{1}' class='{2}' style='{3}' {4}>", ((header) ? "th" : "td"), id, clase, estilo, data);
            html.AppendLine(GetValor());
            html.AppendFormat("</{0}>", ((header) ? "th" : "td"));
            return html.ToString();
        }

    }


    public class HtmlGrid
    {
        public string id { get; set; }
        public string clase { get; set; }
        public List<HtmlGridCol> cols { get; set; }
        public List<HtmlGridRow> rows { get; set; }
        public string titulo { get; set; }
        
        public bool editable { get; set; }
        //public bool selectable { get; set; }
        //public bool personalized { get; set; }
        public bool footer { get; set; }

        public HtmlGrid()
        {
            cols = new List<HtmlGridCol>();
            rows = new List<HtmlGridRow>(); 
 
        }

        public void AddColumn(HtmlGridCol col)
        {
            cols.Add(col);
        }

        public void AddRow(HtmlGridRow row)
        {
            rows.Add(row);
        }
        public override string ToString()
        {
            StringBuilder html = new StringBuilder();

            //clase = "gridtable "+ clase;
            clase = "table table-bordered table-invoice-full " + clase;
            
            if (!string.IsNullOrEmpty(titulo))
                html.AppendFormat("<h4 class=\"widgettitle\">{0}</h4>", titulo);

            html.AppendFormat("<table id='{0}' class='{1}'>", id, clase);
            if (cols.Count > 0)
            {
                html.AppendLine("<colgroup>");
                foreach (HtmlGridCol col in cols)
                {
                    html.AppendFormat("<col id='{0}' class='{1}' style='{2}' />", col.id, col.clase, col.estilo);
                }
                html.AppendLine("</colgroup>");
                html.AppendLine("<thead><tr>");                
                foreach (HtmlGridCol col in cols)
                {
                    html.AppendFormat("<th class='{0}' style='{1}'>{2}</th>", col.clase, col.estilo, col.titulo);
                }
                html.AppendLine("</tr></thead>");

            }
            html.AppendLine("<tbody>");


            foreach (HtmlGridRow row in rows)
            {
                html.AppendLine(row.ToString());
            }

            /*if (cols.Count > 0 && editable)
            {
                html.AppendLine("<tr id='editrow'>");
                foreach (HtmlCols col in cols)
                {
                    html.AppendFormat("<td class='{0} {1}' {3}>{2}</td>", col.clase, col.alineacion, col.control, (col.ancho.HasValue) ? "style='width:" + col.ancho + "px !important;'" : "");
                }
                html.AppendLine("</tr>");
            }*/

            html.AppendLine("</tbody>");

            if (footer)
            {
                html.AppendLine("<tfoot><tr>");
                foreach (HtmlGridCol col in cols)
                {
                    //html.AppendFormat("<th id='{0}' class='head{1} {2} {3}' {4}>{5}</th>", id + "_f" + i, c.ToString(), col.clase, col.alineacion, (col.ancho.HasValue) ? "style='width:" + col.ancho + "px !important;'" : "", col.controlfoot);
                    html.AppendFormat("<th class='{0}' style='{1}'>f</th>", col.clase, col.estilo);
                    
                }
                html.AppendLine("</tr></tfoot>");
            }

            html.AppendLine("</table>");
            return html.ToString();
        }
    }



}
