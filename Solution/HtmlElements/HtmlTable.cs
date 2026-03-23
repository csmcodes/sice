using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlObjects
{
    public class HtmlCell
    {
        public string id { get; set; }
        public string clase { get; set; }
        public string alienacion { get; set; }
        public object valor { get; set; }        
        public bool header { get; set; }
        public string data { get; set; }
        public int? ancho { get; set; }

        public string GetValor()
        {
            return (valor != null) ? valor.ToString() : "";
        }

        public override string ToString()
        {
            StringBuilder html = new StringBuilder();
            html.AppendFormat("<{0} id='{1}' class='{2} {3}' {4} {5}>", ((header) ? "th" : "td"), id, clase, alienacion, data, (ancho.HasValue) ? "style='width:" + ancho + "px !important'" : "");
            html.AppendLine(GetValor());
            html.AppendFormat("</{0}>", ((header) ? "th" : "td"));
            return html.ToString();
        }
        
    }

    public class HtmlCols
    {
        public string titulo { get; set; }
        public string clase { get; set; }
        public string alineacion { get; set; }
        public string control { get; set; }
        public string controlfoot { get; set; }
        public int? ancho { get; set; }

        public HtmlCols()
        {
        }

        public HtmlCols(string titulo, string clase)
        {
            this.titulo = titulo;
            this.clase = clase;
        }

        public HtmlCols(string titulo, string clase, string alineacion, string control, int? ancho)
        {
            this.titulo = titulo;
            this.clase = clase;
            this.alineacion = alineacion;
            this.control = control;
            this.ancho = ancho;
        }
        public HtmlCols(string titulo, string clase, string alineacion, string control, string controlfoot, int? ancho)
        {
            this.titulo = titulo;
            this.clase = clase;
            this.alineacion = alineacion;
            this.control = control;
            this.controlfoot = controlfoot;
            this.ancho = ancho;
        }

       
    }

    public class HtmlRow
    {
        public string id { get; set; }
        public string clase { get; set; }
        public List<HtmlCell> cells { get; set; }
        public List<HtmlCols> cols { get; set; }
        public bool header { get; set; }
        public bool editable { get; set; }
        public bool selectable { get; set; }
        public string data { get; set; }
        public bool removable { get; set; }
        public bool markable { get; set; }
        public string clickevent { get; set; }
        public string selectevent { get; set; }

        public HtmlRow()
        {
            cells = new List<HtmlCell>(); 
        }


        public HtmlRow(params object[] valores)
        {
            cells = new List<HtmlCell>();
            foreach (var item in valores)
            {
                cells.Add(new HtmlCell { valor = item.ToString() });    
            }
        }

        public override string ToString()
        {
            if (clickevent == "")
                clickevent = "Edit(this)";
            if (selectevent == "")
                selectevent = "Select(this)";

            StringBuilder html = new StringBuilder();
            html.AppendFormat("<tr id='{0}' data-id='{0}' class='{1}' {2} {3} {4} {5} {6}>", id, clase, ((editable) ? "onclick='" + clickevent + "'" : ""), ((selectable) ? "onclick='"+selectevent+"'" : ""), ((removable) ? "oncontextmenu=\"Mark(this);return false;\" " : ""), data, ((markable) ? "onclick=\"Mark(this);return false;\" " : ""));
            int c = 0;
            foreach (HtmlCell cell in cells)
            {
                cell.header = this.header;
                if (cols != null)
                {
                    if (cols.Count > c)
                    {
                        if (string.IsNullOrEmpty(cell.alienacion))
                            cell.alienacion = cols[c].alineacion;
                        if (string.IsNullOrEmpty(cell.clase))
                            cell.clase = cols[c].clase;
                    }
                }
                html.AppendLine(cell.ToString());
            }
            html.AppendLine("</tr>");
            return html.ToString(); 

        }



    }

    public class HtmlTable
    {
        public string id { get; set; }
        public string clase { get; set; }
        public List<HtmlCols> cols { get; set; }
        public List<HtmlRow> rows { get; set; }
        public string titulo { get; set; }

        public bool invoice { get; set; }
        public bool editable { get; set; }
        public bool selectable { get; set; }
        public bool personalized { get; set; }

        public bool footer { get; set; }

        public HtmlTable()
        {
            cols = new List<HtmlCols>(); 
            rows = new List<HtmlRow>();
            editable = false;
            selectable = false;
        }

        public void AddColumn(string titulo, string clase, string alineacion)
        {
            cols.Add(new HtmlCols(titulo, clase));   
        }
        public void AddColumn(string titulo, string clase, string alineacion, string control)
        {
            cols.Add(new HtmlCols(titulo, clase, alineacion ,control, null));
        }
        public void AddColumn(string titulo, string clase, string alineacion, string control, int ancho)
        {
            cols.Add(new HtmlCols(titulo, clase, alineacion, control, ancho));
        }
        public void AddColumn(string titulo, string clase, string alineacion, string control, string controlfoot)
        {
            cols.Add(new HtmlCols(titulo, clase, alineacion, control, controlfoot, null));
        }
        public void AddColumn(string titulo, string clase, string alineacion, string control,string controlfoot,  int ancho)
        {
            cols.Add(new HtmlCols(titulo, clase, alineacion, control, controlfoot, ancho));
        }

        public void AddCell(HtmlRow row)
        {
            HtmlCell cell = new HtmlCell();
            cell.id = row.id + "_" + row.cells.Count.ToString();
            cell.valor = cell.id;
            int c = row.cells.Count; 
            if (cols.Count > c)
            {
                if (string.IsNullOrEmpty(cell.alienacion))
                    cell.alienacion = cols[c].alineacion;
                if (string.IsNullOrEmpty(cell.clase))
                    cell.clase = cols[c].clase;
            }
            row.cells.Add(cell);  
        }

        public void AddRow(int cellscount)
        {
            HtmlRow row = new HtmlRow();
            row.id = rows.Count.ToString();
            row.editable = this.editable;
            row.selectable = this.selectable;
            row.cols = this.cols; 
            for (int i = 0; i < cellscount; i++)
            {
                AddCell(row); 
            }
            rows.Add(row); 
        }

        public void AddRow(HtmlRow row)
        {
            row.id = rows.Count.ToString();
            row.editable = this.editable;
            row.selectable = this.selectable;
            row.cols = this.cols; 
            rows.Add(row); 
        }

      
        
        public void CreteEmptyTable(int rows, int cells)
        {            
  
            for (int i = 0; i < rows; i++)
            {
                AddRow(cells); 
            }
        }


        public override string ToString()
        {
            StringBuilder html = new StringBuilder();

            if (invoice)
                clase = "table table-bordered table-invoice-full " + clase;
            else if (personalized)
                clase = clase;
            else
                clase = "table table-bordered table-invoice " + clase;

            if (!string.IsNullOrEmpty(titulo))
                html.AppendFormat("<h4 class=\"widgettitle\">{0}</h4>",titulo);  

            html.AppendFormat("<table id='{0}' class='{1}'>", id, clase);
            if (cols.Count > 0)
            {
                html.AppendLine("<colgroup>");
                int c = 0;
                foreach (HtmlCols col in cols)
                {

                    html.AppendFormat("<col class='con{0} {1} {2}' />", c.ToString(), col.clase, col.alineacion);
                    c = (c == 1) ? 0 : 1;
                }
                html.AppendLine("</colgroup>");
                html.AppendLine("<thead><tr>");
                c = 0;
                foreach (HtmlCols col in cols)
                {
                    html.AppendFormat("<th class='head{0} {1} {2}' {4}>{3}</th>", c.ToString(), col.clase, col.alineacion, col.titulo, (col.ancho.HasValue) ? "style='width:" + col.ancho + "px !important;'" : "");
                    c = (c == 1) ? 0 : 1;
                }
                html.AppendLine("</tr></thead>");

            }
            html.AppendLine("<tbody>");


            foreach (HtmlRow row in rows)
            {
                html.AppendLine(row.ToString());
            }

            if (cols.Count > 0 && editable)
            {
                html.AppendLine("<tr id='editrow'>");
                foreach (HtmlCols col in cols)
                {
                    html.AppendFormat("<td class='{0} {1}' {3}>{2}</td>", col.clase, col.alineacion, col.control, (col.ancho.HasValue) ? "style='width:" + col.ancho + "px !important;'" : "");
                }
                html.AppendLine("</tr>");
            }

            html.AppendLine("</tbody>");

            if (footer)
            {
                html.AppendLine("<tfoot><tr>");
                int c = 0;
                int i =0;
                foreach (HtmlCols col in cols)
                {
                    html.AppendFormat("<th id='{0}' class='head{1} {2} {3}' {4}>{5}</th>", id + "_f" + i, c.ToString(), col.clase, col.alineacion, (col.ancho.HasValue) ? "style='width:" + col.ancho + "px !important;'" : "", col.controlfoot);
                    //html.AppendFormat("<th class='head{0} {1} {2}' {4}></th>", c.ToString(), col.clase, col.alineacion, "", (col.ancho.HasValue) ? "style='width:" + col.ancho + "px;'" : "");
                    c = (c == 1) ? 0 : 1;
                    i++;
                }
                html.AppendLine("</tr></tfoot>");
            }

            html.AppendLine("</table>");
            return html.ToString();
        }

    }
}
