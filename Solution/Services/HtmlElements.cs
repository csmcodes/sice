using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Services
{
    

    
    public class HtmlElements
    {
        public static string small = "input-small";
        public static string medium = "input-medium";
        public static string large = "input-large";
        public static string xlarge = "input-xlarge";
        public static string xxlarge = "input-xxlarge";
        public static string blocklevel = "input-block-level";

        public enum htmlcontrol
        {
            input,
            check,
            textarea,
            select

        }   

        public HtmlElements()
        {
            // Constructor
        }

        

        #region Simple Html Elements

        public static string Label(string label, string id)
        {
            //return string.Format("<label>{0}</label>", label);
            return string.Format("<label for='{0}' class='control-label'>{1}</label>",id,label); 
        }

        public static string HelpIn(string help, string id)
        {
            return string.Format("<span id='{0}' class='help-inline'>{1}</span>", "h_"+id, help); 
        }

        public static string ControlGroup(string label, string control, string help)
        {
            //return string.Format("<p><div class='par control-group'>{0}<div class='controls'>{1}{2}</div></div></p>", label, control, help);
            return string.Format("<p>{0}<span class='field'>{1}{2}</span></p>", label, control, help); 
        }


        #region Input

        public static string Input(string label, string id, string value)
        {
            return Input(label, id, value, "");
        }

        public static string Input(string label, string id, string value, string clase)
        {
            return Input(label, id, value, clase, false);
        }

        public static string Input(string label, string id, string value, string clase, bool obligatorio)
        {
            return Input(label, id, value, clase, obligatorio, true);
        }
        public static string Input(string label, string id, string value, string clase, bool obligatorio, bool habilitado)
        {
            return Input(label, id, value, clase, obligatorio, habilitado,null);
        }

        public static string Input(string label, string id, string value, string clase, bool obligatorio, bool habilitado, int? largo)
        {
           return string.Format("<input id = '{1}' type=\"text\" class='{3}' placeholder='{0}' value=\"{2}\" data-obligatorio='{4}' {5} {6} />", label, id, value, clase, obligatorio, ((!habilitado) ? "disabled='disabled'" : ""), ((largo.HasValue) ? "maxlength='" + largo.Value + "'" : ""));
        }

        #endregion

        #region Input Hidden

        public static string InputHidden(string id, string value)
        {
            return string.Format("<input id = '{0}' type=\"hidden\" value=\"{1}\">", id, value);
        }

        #endregion

        #region Check

        public static string Check(string id, int? value)
        {
            return Check(id, value, "");
        }
        public static string Check(string id, int? value, string clase)
        {
            return Check(id,value,clase,true);
        }
        public static string Check(string id, int? value, string clase, bool habilitado)
        {
            return string.Format("<input  id = '{0}'  class='{2}' type=\"checkbox\" {1} {3} />", id, ((value.HasValue) ? ((value.Value == 1) ? "checked" : "") : ""), clase, ((!habilitado) ? "disabled='disabled'" : "")); 

        }

        #endregion

        #region TextArea

        
        public static string TextArea(string id, string value, int cols, int rows)
        {
            return TextArea(id, value, cols, rows, "");
        }

        public static string TextArea(string id, string value, int cols, int rows, string clase)
        {
            return TextArea(id, value, cols, rows, clase, false);
        }

        public static string TextArea(string id, string value, int cols, int rows, string clase, bool obligatorio)
        {
            return TextArea(id, value, cols, rows, clase, obligatorio, true);
        }

        public static string TextArea(string id, string value, int cols, int rows, string clase, bool obligatorio, bool habilitado)
        {
            return string.Format("<textarea id = '{0}' cols='{2}' rows='{3}' class='{4}' data-obligatorio='{5}' {6}>{1}</textarea>", id, value, cols, rows, clase, obligatorio, ((!habilitado) ? "disabled='disabled'" : ""));
        }

        #endregion

        #region Select

        public static string Select(string id, string value, Dictionary<string, string> dictionary)
        {
            return Select(id, value, dictionary, ""); 
        }


        public static string Select(string id, string value, Dictionary<string, string> dictionary, string clase)
        {
            return Select(id, value, dictionary, clase, false); 
        }
        public static string Select(string id, string value, Dictionary<string, string> dictionary, string clase, bool obligatorio)
        {
            return Select(id, value, dictionary, clase, obligatorio, true);
        }
        
        public static string Select(string id, string value, Dictionary<string, string> dictionary, string clase, bool obligatorio, bool habilitado)
        {
            StringBuilder html = new StringBuilder();
            html.AppendFormat("<select id='{0}' name='select' class='uniformselect {1}' data-obligatorio='{2}' {3}>", id, clase, obligatorio, ((!habilitado) ? "disabled='disabled'" : ""));
            foreach (KeyValuePair<string, string> entry in dictionary)
            {
                string selected = "";
                if (value == entry.Key)
                    selected = "selected";
                html.AppendFormat("<option value='{0}' {2}>{1}</option>", entry.Key, entry.Value, selected);
            }
            html.AppendLine("</select>");
            return html.ToString(); 
        }

        #endregion


        #region Multiple Select

        public static string MultipleSelect(string label, string id, string value, Dictionary<string, string> dictionary)
        {
            return MultipleSelect(label, id, value, dictionary, "");
        }


        public static string MultipleSelect(string label, string id, string value, Dictionary<string, string> dictionary, string clase)
        {
            return MultipleSelect(label,id, value, dictionary, clase, false);
        }
        public static string MultipleSelect(string label, string id, string value, Dictionary<string, string> dictionary, string clase, bool obligatorio)
        {
            return MultipleSelect(label, id, value, dictionary, clase, obligatorio, true);
        }

        public static string MultipleSelect(string label, string id, string value, Dictionary<string, string> dictionary, string clase, bool obligatorio, bool habilitado)
        {
            StringBuilder html = new StringBuilder();
            html.AppendFormat("<select id='{0}' data-placeholder='{1}' class='chzn-select {2}' multiple='multiple' style='width:350px;' data-obligatorio='{3}' {4}>", id, label, clase, obligatorio, ((!habilitado) ? "disabled='disabled'" : ""));
            //html.AppendLine("<option value=''></option>");            
            foreach (KeyValuePair<string, string> entry in dictionary)
            {
                string selected = "";
                if (value == entry.Key)
                    selected = "selected";
                html.AppendFormat("<option value='{0}' {2}>{1}</option>", entry.Key, entry.Value, selected);
            }
            html.AppendLine("</select>");
            return html.ToString();
        }

        #endregion

        #region DatePicker

        public static string DatePicker(string label, string id, DateTime? value)
        {
            return DatePicker(label, id, value, false);
        }

        public static string DatePicker(string label, string id, DateTime? value, bool obligatorio)
        {
            return DatePicker(label, id, value, obligatorio, true);
        }

        public static string DatePicker(string label, string id, DateTime? value, bool obligatorio, bool habilitado)
        {
            string valor = "";
            if (value.HasValue)
                valor = value.Value.ToShortDateString(); 
            return Input(label, id, valor, small+" fecha",obligatorio,habilitado); 
        }
        #endregion

        #endregion

        #region Compose Html Elements

        /*
        #region Label Input

        public static string LabelInput(string label, string id, string value)
        {
            return LabelInput(label,id,value,"");
        }

        public static string LabelInput(string label, string id, string value, string clase)
        {
            return LabelInput(label, id, value,clase, "");
        }

        public static string LabelInput(string label, string id, string value, string clase, string help)
        {
            return LabelInput(label, id, value, clase, help, false);
        }

        public static string LabelInput(string label, string id, string value, string clase, string help, bool obligatorio)
        {
            return LabelInput(label, id, value, clase, help, obligatorio, true);
        }

        public static string LabelInput(string label, string id, string value, string clase, string help, bool obligatorio, bool habilitado)
        {
            return LabelInput(label, id, value, clase, help, obligatorio, habilitado,null);
        }

        public static string LabelInput(string label, string id, string value, string clase, string help, bool obligatorio, bool habilitado, int? largo)
        {            
            return ControlGroup(Label(label, id), Input(label, id, value, clase, obligatorio,habilitado,largo), HelpIn(help, id));   
        }

        #endregion

        #region Label Check

        public static string LabelCheck(string label, string id, int? value)
        {
            return LabelCheck(label, id, value, "");
        }

        public static string LabelCheck(string label, string id, int? value, string clase)
        {
            return LabelCheck(label, id, value, clase, "");
        }

        public static string LabelCheck(string label, string id, int? value, string clase, string help)
        {
            return LabelCheck(label, id, value, clase, help, true);
        }

        public static string LabelCheck(string label, string id, int? value, string clase, string help, bool habilitado)
        {
            return ControlGroup(Label(label, id), Check(id, value,clase,habilitado), HelpIn(help, id));               
        }

        #endregion

        #region Label Text Area

        public static string LabelTextArea(string label, string id, string value, int cols, int rows)
        {
            return LabelTextArea(label, id, value, cols, rows, "");
        }

        public static string LabelTextArea(string label, string id, string value, int cols, int rows, string clase)
        {
            return LabelTextArea(label, id, value, cols, rows, clase,"");
        }

        public static string LabelTextArea(string label, string id, string value, int cols, int rows, string clase, string help)
        {
            return LabelTextArea(label, id, value, cols, rows, clase, help, false);
        }

        public static string LabelTextArea(string label, string id, string value, int cols, int rows, string clase, string help, bool obligatorio)
        {
            return LabelTextArea(label, id, value, cols, rows, clase, help, obligatorio, true);
        }

        public static string LabelTextArea(string label, string id, string value, int cols, int rows, string clase, string help, bool obligatorio, bool habilitado)
        {            
            return ControlGroup(Label(label, id), TextArea(id, value, cols, rows, clase, obligatorio, habilitado), HelpIn(help, id));   
        }

        #endregion

        #region Label Select

        public static string LabelSelect(string label, string id, string value, Dictionary<string, string> dictionary)
        {
            return LabelSelect(label, id, value, dictionary, "");
        }

        public static string LabelSelect(string label, string id, string value, Dictionary<string, string> dictionary, string clase)
        {
            return LabelSelect(label, id, value, dictionary, clase,"");
        }

        public static string LabelSelect(string label, string id, string value, Dictionary<string, string> dictionary, string clase, string help)
        {
            return LabelSelect(label, id, value, dictionary, clase, help, false);
        }

        public static string LabelSelect(string label, string id, string value, Dictionary<string, string> dictionary, string clase, string help, bool obligatorio)
        {
            return LabelSelect(label, id, value, dictionary, clase, help, obligatorio, true);
        }

        public static string LabelSelect(string label, string id, string value, Dictionary<string, string> dictionary, string clase, string help, bool obligatorio, bool habilitado)
        {
            return ControlGroup(Label(label, id), Select(id, value, dictionary, clase, obligatorio,habilitado), HelpIn(help, id));
        }




        #endregion

        #region Label MultipleSelect

        public static string LabelMultipleSelect(string label, string id, string value, Dictionary<string, string> dictionary)
        {
            return LabelMultipleSelect(label, id, value, dictionary, "");
        }

        public static string LabelMultipleSelect(string label, string id, string value, Dictionary<string, string> dictionary, string clase)
        {
            return LabelMultipleSelect(label, id, value, dictionary, clase, "");
        }

        public static string LabelMultipleSelect(string label, string id, string value, Dictionary<string, string> dictionary, string clase, string help)
        {
            return LabelMultipleSelect(label, id, value, dictionary, clase, help, false);
        }

        public static string LabelMultipleSelect(string label, string id, string value, Dictionary<string, string> dictionary, string clase, string help, bool obligatorio)
        {
            return LabelMultipleSelect(label, id, value, dictionary, clase, help, obligatorio, true);
        }

        public static string LabelMultipleSelect(string label, string id, string value, Dictionary<string, string> dictionary, string clase, string help, bool obligatorio, bool habilitado)
        {
            return ControlGroup(Label(label, id), MultipleSelect(label, id, value, dictionary, clase, obligatorio, habilitado), HelpIn(help, id));
        }

        #endregion

        #region Label DatePicker

        public static string LabelDatePicker(string label, string id, DateTime? value)
        {
            return LabelDatePicker(label, id, value,"");
        }

        public static string LabelDatePicker(string label, string id, DateTime? value, string help)
        {
            return LabelDatePicker(label, id, value, help,false);
        }

        public static string LabelDatePicker(string label, string id, DateTime? value, string help, bool obligatorio)
        {
            return LabelDatePicker(label, id, value, help, obligatorio,true);
        }

        public static string LabelDatePicker(string label, string id, DateTime? value, string help, bool obligatorio, bool habilitado)
        {
            return ControlGroup(Label(label, id),DatePicker(label, id, value,obligatorio, habilitado), HelpIn(help, id)); 
        }

        #endregion
        
        */

   /*     public static string SubmitButton(string value, string click)
        {
            return string.Format("<p class='stdformbutton'><button class='btn btn-primary' onclick='{1}'>{0}</button></p>", value, click);
        }*/

   /*     public static string Auditoria(string creausr, DateTime? creafecha, string modusr, DateTime? modfecha)
        {
            return string.Format("<p><small class='desc' id='lblcrea'>Creado por:{0} {1}</small><small class='desc' id='lblmod'>Modificado por:{2} {3}</small></p>", creausr, creafecha, modusr, modfecha);
        }

        */

        /*
        #endregion
        
        #region Html Elements Listado

        public static string ListItem(string titulo, string descripcion, int? activo)
        {
            StringBuilder html = new StringBuilder();            
            html.AppendFormat("<h5>{0}</h5>", titulo);
            html.AppendFormat("<span class='pos'>{0}</span>", descripcion);
            html.AppendFormat("<span>Activo: <input type='checkbox' {0} disabled/></span>", ((activo.HasValue) ? ((activo.Value == 1) ? "checked" : "") : ""));            
            return html.ToString();
        }
    
        public static string ListItem(string titulo, ArrayList descripcion)
        {
            StringBuilder html = new StringBuilder();            
            html.AppendFormat("<h5>{0}</h5>", titulo);
            foreach (string item in descripcion)
            {
                html.AppendFormat("<span class='pos'>{0}</span>", item);    
            }            
            return html.ToString();
        }

        public static string ListItem(string titulo, ArrayList descripcion, int? activo)
        {
            StringBuilder html = new StringBuilder();
            html.AppendFormat("<h5>{0}</h5>", titulo);
            foreach (string item in descripcion)
            {
                html.AppendFormat("<span class='pos'>{0}</span>", item);
            }
            html.AppendFormat("<span>Activo: <input type='checkbox' {0} disabled/></span>", ((activo.HasValue) ? ((activo.Value == 1) ? "checked" : "") : ""));            
            return html.ToString();
        }
/*
        public static string HtmlList(string id, string content)
        {
            StringBuilder html = new StringBuilder();    
            //html.AppendLine("<li onmousedown=\"Select(this)\"><div class='uinfo' data-id=\"" + id + "\" >");
            html.AppendLine("<li onmousedown=\"Select(this)\"><div class='uinfo' data-id='" + id + "' >");
            html.AppendLine(content);
            html.AppendLine("</div></li>");
            return html.ToString(); 

        }
        */
        #endregion

        #region Html Elements Busqueda

   /*     public static string SelectBoolean(string label, string field, string value, string clase)
        {
            if (value == "0")
                return string.Format("<select  id = '{0}' ><option value=''>{1}</option><option value='1' selected>Si</option><option value='0'>No</option></select>", field, label, value, clase);
            if (value == "1")
                return string.Format("<select  id = '{0}' ><option value=''>{1}</option><option value='1'>Si</option><option value='0' Selected>No</option></select>", field, label, value, clase);
            return string.Format("<select  id = '{0}' ><option value='' selected>{1}</option><option value='1'>Si</option><option value='0'>No</option></select>", field, label, value, clase);

        }
        */
    /*    public static string RefreshButton()
        {
            return string.Format("<div class='btn' id='refresh' onclick='ReloadData()'><i class='iconsweets-refresh'></i>&nbsp; Refrescar</div> ");            
        }

        public static string CleanButton()
        {
            return string.Format("<div class='btn' id='clean' onclick='CleanSearch()'><i class='iconsweets-pencil'></i>&nbsp; Limpiar</div> ");
        }
        */
        #endregion

        #region Html Elements Menu


        public static string MenuItem(string codigo, string nombre, string clase, bool hijos)
        {
            //return string.Format("<li {3}><a data-id=\"{0}\" href=\"javascript:Select(this)\"><span class=\"{2}\"></span>{1}</a>", codigo, nombre, clase, ((hijos) ? "class=\"dropdown\"" : ""));          
            //return string.Format("<li onmousedown=\"Select(this)\" {3}><a data-id=\"{0}\" ><span class=\"{2}\"></span>{1}</a>", codigo, nombre, clase, ((hijos) ? "class=\"dropdown\"" : ""));          
            return string.Format("<li onmousedown=\"Select(this)\" {3}><a data-id='{0}' ><span class=\"{2}\"></span>{1}</a>", codigo, nombre, clase, ((hijos) ? "class=\"dropdown\"" : ""));          
        }

        public static string HtmlMenuItem(string codigo, string formulario, string nombre, string clase, bool hijos)
        {
            // <li id ="wfArmaMenu"><a href="wfArmaMenu.aspx"><span class="iconfa-option"></span>Gestión Menu</a></li>
            //return string.Format("<li onmousedown=\"Select(this)\" {3}><a data-id=\"{0}\" ><span class=\"{2}\"></span>{1}</a>", codigo, nombre, clase, ((hijos) ? "class=\"dropdown\"" : ""));          
            return string.Format("<li id='{2}' {4}><a href='{1}'><span class=\"{3}\"></span>{2}</a>", codigo, "javascript:SelectOptionMenu(\""+nombre+"\",\"" + formulario + "\");", nombre, clase, ((hijos) ? "class=\"dropdown\"" : ""));
        }

        #endregion

        #region Html Elements Tablas


        public static string TableCell(object dato)
        {
            return string.Format("<td>{0}</td>", dato); 
        }
        public static string TablaRow(ArrayList datos, string id)
        {
            StringBuilder html = new StringBuilder();
            html.AppendFormat("<tr data-id='{0}' onclick=\"Edit(this)\" oncontextmenu=\"Select(this);return false;\"  >", id);
            foreach (object item in datos)
            {
                html.AppendLine(TableCell(item));  
                
            }
            html.AppendLine("</tr>");            
            return html.ToString();
        }

        public static string TablaRow(ArrayList datos, string id, string clase)
        {
            StringBuilder html = new StringBuilder();
            html.AppendFormat("<tr data-id='{0}' class='{1}' onclick=\"Edit(this)\" oncontextmenu=\"Select(this);return false;\"  >", id, clase);
            foreach (object item in datos)
            {
                html.AppendLine(TableCell(item));

            }
            html.AppendLine("</tr>");
            return html.ToString();
        }

        public static string TablaRowBusqueda(ArrayList datos, string id)
        {
            StringBuilder html = new StringBuilder();
            html.AppendFormat("<tr data-id='{0}' onclick=\"Select(this)\">", id);
            foreach (object item in datos)
            {
                html.AppendLine(TableCell(item));

            }
            html.AppendLine("</tr>");
            return html.ToString();
        }


        public static string HeadCell(object dato)
        {
            return string.Format("<th style='width:100px;'>{0}</td>", dato);
        }
        public static string HeadRow(ArrayList datos)
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("<tr><th></th>");
            foreach (object item in datos)
            {
                html.AppendLine(HeadCell(item));
            }
            html.AppendLine("</tr>");
            return html.ToString();
        }

        

        #endregion

      





    }
}
