using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Text;
using BusinessObjects;
using BusinessLogicLayer;
using Functions;
using WebUI.ec.gob.sri.celcer;
using WebUI.ec.gob.sri.celcer1;
using System.Xml;
using System.IO;
using Services;

namespace WebUI.ws
{
    /// <summary>
    /// Descripción breve de Metodos
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    // Para permitir que se llame a este servicio Web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class Metodos : System.Web.Services.WebService
    {


        #region Menu

        public string ActiveMenuOption(string path, string opcion, bool padre)
        {
            if (path.IndexOf(opcion) >= 0)
            {
                return (padre) ? "active current" : "active current";
            }
            return "";
        }

        [WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetMenu(object objeto)
        {

            Usuarioxempresa uxe = new Usuarioxempresa(objeto);

            Usuario usr = UsuarioBLL.GetByPK(new Usuario { usr_id = uxe.uxe_usuario, usr_id_key = uxe.uxe_usuario });
 

            Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
            object path = null;
            tmp.TryGetValue("path", out path);
            if (path == null)
                path = "";

            StringBuilder html = new StringBuilder();

            html.AppendLine("<ul class=\"sidebar-nav\">");
            html.AppendFormat("<li class='{0}'><a href=\"listado.html\"><i class=\"fa fa-copy\"></i><span class=\"sidebar-text\">Comprobantes</span></a></li>", ActiveMenuOption(path.ToString(), "Comprobantes", false));
            if (usr.usr_perfil == Constantes.cPerfilAdministrador || usr.usr_perfil == Constantes.cPerfilEmpresa)
            {
                html.AppendFormat("<li class='{0}'><a href=\"subir.html\"><i class=\"fa fa-cog\"></i><span class=\"sidebar-text\">Envio Manual XML</span></a></li>", ActiveMenuOption(path.ToString(), "Envio", false));
                //html.AppendFormat("<li class='{0}'><a href=\"ventas.html\"><i class=\"fa fa-money\"></i><span class=\"sidebar-text\">Ventas</span></a></li>", ActiveMenuOption(path.ToString(), "Ventas", false));                
            }
            html.AppendLine("</ul>");
            return html.ToString();
        }

        #endregion


        #region Login 


        public string ValidaIngreso(Usuario usr)
        {
            string retorono = "OK";
           return retorono;
        }

        [WebMethod]
        public string Login(object objeto)
        {

            Usuario usuario = new Usuario(objeto);
            usuario.usr_id_key = usuario.usr_id;
            string pass = usuario.usr_password;
            usuario = UsuarioBLL.GetByPK(usuario);
            if (usuario.usr_estado.HasValue)
            {
                if (usuario.usr_password == pass)
                    return ValidaIngreso(usuario);
                else
                    return "Contraseña incorrecta";
            }

            else
                return "No existe el usuario";

        }

        [WebMethod]
        public string SignUp(object objeto)
        {

            Usuario usuario = new Usuario(objeto);
            usuario.usr_id_key= usuario.usr_id;

            usuario = UsuarioBLL.GetByPK(usuario);
            if (usuario.usr_estado.HasValue)
            {
                return "El usuario ya se encuentra registrado con el correo "+usuario.usr_mail;
            }
            else
            {
                usuario.usr_estado = 1;
                usuario.usr_perfil= "user";
                usuario.usr_password = System.Web.Security.Membership.GeneratePassword(6, 0);
                if (UsuarioBLL.Insert(usuario) > 0)
                {
                    if (!EnviarConfirmación(usuario))
                        return "Su registro ha sido completado pero no ha podido enviar  su confirmación a la direccion de correo indicada, comuniquese con nosotros para la confirmación";
                    else
                        return "Su registro se ha completado exitosamente, verifique su correo para obtener las credenciales de accesso";
                }
                else
                    return "No se puede completar registro";
            }

        }

        [WebMethod]
        public string Olvido(object objeto)
        {
            
            

            Usuario usuario = new Usuario(objeto);
            usuario.usr_id_key = usuario.usr_id;

            string mail = usuario.usr_mail;
            usuario = UsuarioBLL.GetByPK(usuario);
            if (usuario.usr_estado.HasValue)
            {
                if (usuario.usr_mail == mail)
                {
                    if (EnviarPassword(usuario))
                        return "La contraseña ha sido enviada al correo electrónico registrado";
                    else
                        return "No se ha podido enviar la contraseña al correo electrónico registrado...";

                }
                else
                    return "El correo electrónico ingresado no coincide con el registrado para el usuario " + usuario.usr_id;
            }
            else
            {

                return "No existe el usuario " + usuario.usr_id;
            }

        }


        //[WebMethod]
        //public string GetMenu(object objeto)
        //{
        //    Usuario usuario = new Usuario(objeto);
        //    usuario.email_key = usuario.email;
        //    usuario = UsuarioBLL.GetByPK(usuario);

        //    Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
        //    object menu = null;
        //    tmp.TryGetValue("menu", out menu);


        //    StringBuilder html = new StringBuilder();


        //    string activo = "class=\"current\"";
        //    html.AppendLine("<ul class=\"sidebar-nav\">");
        //    //html.AppendFormat("<li {0}><a href=\"index.html\"><i class=\"fa fa-dashboard\"></i><span class=\"sidebar-text\">Inicio</span></a></li>", (menu.ToString() == "index") ? activo : "");
        //    html.AppendFormat("<li {0}><a href=\"listado.html\"><i class=\"fa fa-trophy\"></i><span class=\"sidebar-text\">Jugar</span></a></li>", (menu.ToString() == "listado") ? activo : "");
        //    html.AppendFormat("<li {0}><a href=\"resultados.html\"><i class=\"glyph-icon flaticon-charts2\"></i><span class=\"sidebar-text\">Resultados <span class=\"label label-info pull-right\">Nuevo</span></span></a></li>", (menu.ToString() == "resultados") ? activo : "");
        //    html.AppendFormat("<li {0}><a href=\"info.html\"><i class=\"glyphicon glyphicon-info-sign\"></i><span class=\"sidebar-text\">Información de Pago </a></li>", (menu.ToString() == "info") ? activo : "");
        //    html.AppendFormat("<li {0}><a href=\"reglas.html\"><i class=\"glyphicon glyphicon-certificate\"></i><span class=\"sidebar-text\">Reglas de Juego</span></a></li>", (menu.ToString() == "reglas") ? activo : "");
        //    html.AppendFormat("<li {0}><a href=\"config.html\"><i class=\"glyphicon glyphicon-cog\"></i><span class=\"sidebar-text\">Configuración</a></li>", (menu.ToString() == "config") ? activo : "");
        //    if (usuario.rol == "admin")
        //    {
        //        html.AppendFormat("<li {0}><a href=\"mails.html\"><i class=\"glyphicon glyphicon-mail\"></i><span class=\"sidebar-text\">Envio Mail</a></li>", (menu.ToString() == "mails") ? activo : "");
        //        html.AppendFormat("<li {0}><a href=\"setresultados.html\"><i class=\"glyphicon glyphicon-mail\"></i><span class=\"sidebar-text\">Set Resultados</a></li>", (menu.ToString() == "set") ? activo : "");
        //    }
        //    html.AppendLine("</ul>");

        //    return html.ToString();

        //}




        #endregion


        [WebMethod]
        public string CreaUsuarios(object objeto)
        {
            
            return Proceso.CreaUsuarios();

        }



        [WebMethod]
        public string SetClave(object objeto)
        {
            Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
            object clave = null;
            tmp.TryGetValue("clave", out clave);

            return  Functions.Formatos.getMod11(clave.ToString());
                     
        }


        [WebMethod]
        public string VerificarComprobantes(object  objeto)
        {

            Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
            object fecha = null;
            object empresa = null;

            tmp.TryGetValue("empresa", out empresa);
            tmp.TryGetValue("fecha", out fecha);

            DateTime? dfecha = Conversiones.ObjectToDateTimeNull(fecha);


            int cant = Packages.General.VerificarComprobantes(dfecha);
            return cant.ToString() + " comprobantes verificados";
            
        }

        #region Recepcion Comprobantes

        //[WebMethod]
        //public string RecibirComprobante(string xml)
        //{
        //    return Proceso.RecibirComprobante(xml,"",1);                
        //}

        [WebMethod]
        public string GetComprobanteData(string clave)
        {
            List<Comprobante> comprobantes = ComprobanteBLL.GetAll(new WhereParams("com_numero={0}", clave), "");
            if (comprobantes.Count > 0)
            {

                string retorno = string.Format("<data>" +
                                 "   <clave>{0}</clave>" +
                                 "   <fecharecibe>{1}</fecharecibe>" +
                                 "   <fechaenvia>{2}</fechaenvia>" +
                                 "   <fecharespuesta>{3}</fecharespuesta>" +
                                 "   <fechaautoriza>{4}</fechaautoriza>" +
                                 "   <estado>{5}</estado>" +
                                 "   <mensaje>{6}</mensaje>" +
                                 "</data>", comprobantes[0].com_numero, comprobantes[0].com_fecharecibe, comprobantes[0].com_fechaenvia, comprobantes[0].com_fecharespuesta, comprobantes[0].com_fechaautorizacion, Enums.GetEstadoComprobante(comprobantes[0].com_estado.Value), comprobantes[0].com_mensaje);



                return retorno;
            }
            return "";
        }



        [WebMethod]
        public string RecibirComprobanteObj(object objeto)
        {
            try
            {




                Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
                object mail = null;
                object xml = null;
                object tipo = null;
                tmp.TryGetValue("xml", out xml);
                tmp.TryGetValue("mail", out mail);
                tmp.TryGetValue("tipo", out tipo);

                ExceptionHandling.Log.AddLog("**MANUAL**," + mail + " " + tipo + " " + xml);

                return Proceso.RecibirComprobante(xml.ToString(), mail.ToString(), int.Parse(tipo.ToString()));
            }
            catch (Exception ex)
            {
                ExceptionHandling.Log.AddLog("**MANUAL**" + ex.Message + " " + ex.InnerException);
                return "Error " + ex.Message;

            }
        }

        [WebMethod]
        public string RecibirComprobante(string xml, string mail, int? formato)
        {

            ExceptionHandling.Log.AddLog(mail + " " + formato + " " + xml);
            if (!formato.HasValue)
                formato = 1;
            //if (string.IsNullOrEmpty(formato))
            //    formato = "1";
            return Proceso.RecibirComprobante(xml,mail,formato.Value);
        }

        [WebMethod]
        public string ObtieneAutorizacion(string clave)
        {

            List<Comprobante> comprobantes = ComprobanteBLL.GetAll(new WhereParams("com_numero={0}", clave), "");
            if (comprobantes.Count > 0)
            {

                return (string.IsNullOrEmpty(comprobantes[0].com_autorizacion) ? "SIN AUTORIZACION" : comprobantes[0].com_autorizacion);
            }
            return "NO COMPROBANTE";
        }

        [WebMethod]
        public string EnviarComprobante(object objeto)
        {

            Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
            object empresa = null;
            object clave = null;
            tmp.TryGetValue("empresa", out empresa);
            tmp.TryGetValue("clave", out clave);
            return Proceso.ReenviarComprobante(int.Parse(empresa.ToString()), clave.ToString());
        }

        [WebMethod]
        public string ResetComprobante(object objeto)
        {

            Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
            object empresa = null;
            object clave = null;
            tmp.TryGetValue("empresa", out empresa);
            tmp.TryGetValue("clave", out clave);

            return Proceso.ResetComprobante(int.Parse(empresa.ToString()), clave.ToString());
        }


        [WebMethod]
        public string MailComprobante(object objeto)
        {

            Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
            object empresa = null;
            object clave = null;
            tmp.TryGetValue("empresa", out empresa);
            tmp.TryGetValue("clave", out clave);

            Comprobante comprobante = ComprobanteBLL.GetByPK(new Comprobante { com_numero = clave.ToString(), com_numero_key = clave.ToString(), com_empresa = int.Parse(empresa.ToString()), com_empresa_key = int.Parse(empresa.ToString()) });

            return Packages.Offline.CorreoComprobante(comprobante, "admin", false);

            //return Proceso.CorreoComprobante(int.Parse(empresa.ToString()), clave.ToString(),false);
        }


        [WebMethod]
        public string BuscarAutorizacion(object objeto)
        {

            Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
            object empresa = null;
            object clave = null;
            tmp.TryGetValue("empresa", out empresa);
            tmp.TryGetValue("clave", out clave);
            return Proceso.BuscarAutorizacion(int.Parse(empresa.ToString()), clave.ToString());
        }


        [WebMethod]
        public string GetMensaje(object objeto)
        {

            Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
            object empresa = null;
            object clave = null;
            tmp.TryGetValue("empresa", out empresa);
            tmp.TryGetValue("clave", out clave);
            return Proceso.GetMensaje(int.Parse(empresa.ToString()), clave.ToString());
        }


        [WebMethod]
        public string Eliminar(object objeto)
        {

            Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
            object empresa = null;
            object clave = null;
            tmp.TryGetValue("empresa", out empresa);
            tmp.TryGetValue("clave", out clave);
            return Proceso.Eliminar(int.Parse(empresa.ToString()), clave.ToString());
        }




        [WebMethod]
        public string VerificarComprobante(string clave)
        {
            List<Comprobante> comprobantes = ComprobanteBLL.GetAll(new WhereParams("com_numero={0}", clave),"");
            if (comprobantes.Count > 0)
            {
                Packages.Offline.VerifyComprobanteAsync(comprobantes[0]);
                return "Verificando...";
                //return Proceso.VerificarComprobante(comprobantes[0].com_empresa, clave);
            }
            return "NO COMPROBANTE";
        }

        [WebMethod]
        public string GetRespuesta(string clave)
        {
            string html = "";
             List<Comprobante> comprobantes = ComprobanteBLL.GetAll(new WhereParams("com_numero={0}", clave), "");
             foreach (Comprobante comprobante in comprobantes)
             {
                 Archivo arc = ArchivoBLL.GetByPK(new Archivo { arc_empresa = comprobante.com_empresa, arc_empresa_key = comprobante.com_empresa, arc_numero = comprobante.com_numero, arc_numero_key = comprobante.com_numero });
                 html = arc.arc_xmlrespuesta;
             }

             return html;
        }

        [WebMethod]
        public string GetXML(string clave)
        {
            string html = "";
            List<Comprobante> comprobantes = ComprobanteBLL.GetAll(new WhereParams("com_numero={0}", clave), "");
            foreach (Comprobante comprobante in comprobantes)
            {
                
                Archivo arc = ArchivoBLL.GetByPK(new Archivo { arc_empresa = comprobante.com_empresa, arc_empresa_key = comprobante.com_empresa, arc_numero = comprobante.com_numero, arc_numero_key = comprobante.com_numero });
                if (comprobante.com_estado == (int)Enums.EstadoComprobante.Autorizado)
                    html = Packages.General.FormatXML(arc.arc_xmlrespuesta);
                else
                    html = "<xmp>" + arc.arc_xml + "</xmp>";
            }
            return html;
        }


        [WebMethod]
        public string UploadComprobante(object objeto)
        {
            Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
            object mail = null;
            object xml = null;
            object tipo = null;
            tmp.TryGetValue("xml", out xml);
            tmp.TryGetValue("mail", out mail);
            tmp.TryGetValue("tipo", out tipo);


            SICE.Metodos ws = new SICE.Metodos();

            return ws.RecibirComprobante(xml.ToString(),mail.ToString(),int.Parse(tipo.ToString()));      




        }
        #endregion

        [WebMethod]
        public string GetCabeceraListado(object objeto)
        {
            Usuarioxempresa uxe = new Usuarioxempresa(objeto);
            Usuario usuario = UsuarioBLL.GetByPK(new Usuario { usr_id = uxe.uxe_usuario, usr_id_key = uxe.uxe_usuario });

            List<Formato> lstformatos = FormatoBLL.GetAll(new WhereParams("for_empresa={0} and for_estado={1}", uxe.uxe_empresa, 1), "for_tipo");
            List<Empresa> empresas = EmpresaBLL.GetAll("emp_estado=1", "");
            StringBuilder html = new StringBuilder();

            html.AppendLine("<form id=\"form1\" class=\"form-horizontal\">");
            
            html.AppendLine("<div class=\"col-md-4 m-b-20\">");
            html.AppendLine("<div class=\"form-group\">");
            html.AppendLine("<label class=\"col-sm-3 control-label\">Almacen:</label>");
            html.AppendLine("<div class=\"col-sm-6\"><input type='text' id='txtalmacen' class=\"filter form-control\" /></div>");
            html.AppendLine("</div>");
            html.AppendLine("<div class=\"form-group\">");
            html.AppendLine("<label class=\"col-sm-3 control-label\">P.Venta:</label>");
            html.AppendLine("<div class=\"col-sm-6\"><input type='text' id='txtpventa' class=\"filter form-control\" /></div>");
            html.AppendLine("</div>");
            html.AppendLine("<div class=\"form-group\">");
            html.AppendLine("<label class=\"col-sm-3 control-label\">Número:</label>");
            html.AppendLine("<div class=\"col-sm-6\"><input type='text' id='txtnumero' class=\"filter form-control\" /></div>");
            html.AppendLine("</div>");

         


            html.AppendLine("</div>");


            html.AppendLine("<div class=\"col-md-4 m-b-20\">");
            html.AppendLine("<div class=\"form-group\">");
            html.AppendLine("<label class=\"col-sm-3 control-label\">Desde:</label>");
            html.AppendFormat("<div class=\"col-sm-6\"><input id='txtdesde' class='filter pickadate form-control' type='text' placeholder='Fecha desde' value='{0}' /></div>", DateTime.Now.ToShortDateString());            
            html.AppendLine("</div>");

            html.AppendLine("<div class=\"form-group\">");
            html.AppendLine("<label class=\"col-sm-3 control-label\">Hasta:</label>");
            html.AppendFormat("<div class=\"col-sm-6\"><input id='txthasta' class='filter pickadate form-control' type='text' placeholder='Fecha hasta' value='{0}' /></div>", DateTime.Now.ToShortDateString());
            html.AppendLine("</div>");
            html.AppendLine("<div class=\"form-group\">");
            html.AppendLine("<label class=\"col-sm-3 control-label\">Cliente:</label>");
            html.AppendLine("<div class=\"col-sm-6\"><input type='text' id='txtcliente' class=\"filter form-control\" /></div>");
            html.AppendLine("</div>");

            if (usuario.usr_perfil == Constantes.cPerfilAdministrador || usuario.usr_perfil == Constantes.cPerfilEmpresa)
            {
                html.AppendLine("<div class=\"form-group\">");
                html.AppendLine("<label class=\"col-sm-3 control-label\">Ver faltantes:</label>");
                html.AppendLine("<div class=\"col-sm-6\"><input type='checkbox' id='chkfalta' class=\"filter\" /></div>");
                html.AppendLine("</div>");
            }

            html.AppendLine("</div>");



            html.AppendLine("</div>");

            html.AppendLine("<div class=\"col-md-4 m-b-20\">");
            html.AppendLine("<div class=\"form-group\">");
            html.AppendLine("<label class=\"col-sm-3 control-label\">Estado:</label>");
            html.AppendFormat("<div class=\"col-sm-6\"><select id='cmbestado' class=\"filter form-control\"  {0} >", (usuario.usr_perfil == "user") ? "disabled" : "");
            html.AppendFormat("<option value=''></option>");
            string[] enumNames = Enum.GetNames(typeof(Enums.EstadoComprobante));
            foreach (string item in enumNames)
            {
                int value = (int)Enum.Parse(typeof(Enums.EstadoComprobante), item);
                html.AppendFormat("<option value='{0}'>{1}</option>", value,item);
           }

            html.AppendLine("</select></div>");
            html.AppendLine("</div>");
            html.AppendLine("<div class=\"form-group\">");
            html.AppendLine("<label class=\"col-sm-3  control-label\">Ambiente:</label>");
            html.AppendFormat("<div class=\"col-sm-6\"><select id='cmbambiente' class=\"filter form-control\"  {0} >", (usuario.usr_perfil == "user") ? "disabled" : "");
            html.AppendFormat("<option value=''></option>");
            html.AppendFormat("<option value='1'>Pruebas</option>");
            html.AppendFormat("<option value='2'>Producción</option>");
            html.AppendLine("</select></div>");
            html.AppendLine("</div>");

            if (usuario.usr_perfil == Constantes.cPerfilAdministrador || usuario.usr_perfil == Constantes.cPerfilEmpresa)
            {                
                html.AppendLine("<div class=\"form-group\">");
                html.AppendLine("<label class=\"col-sm-3  control-label\">Tipo Comprobante:</label>");
                html.AppendFormat("<div class=\"col-sm-6\"><select id='cmbformato' class=\"filter form-control\"  {0} >", "");
                html.AppendFormat("<option value=''></option>");

                foreach (Formato item in lstformatos)
                {
                    html.AppendFormat("<option value='{0}'>{1}</option>", item.for_codigo, item.for_tipo);
                }
                html.AppendLine("</select></div>");
                html.AppendLine("</div>");
            }


            if (usuario.usr_perfil == Constantes.cPerfilAdministrador)
            {
                html.AppendLine("<div class=\"form-group\">");
                html.AppendLine("<label class=\"col-sm-3  control-label\">Empresa:</label>");
                html.AppendFormat("<div class=\"col-sm-6\"><select id='cmbempresa' class=\"selectpicker filter form-control\"  {0} >", (usuario.usr_perfil != "admin") ? "disabled" : "");
                html.AppendFormat("<option value=''></option>");

                foreach (Empresa item in empresas)
                {
                    html.AppendFormat("<option value='{0}' {2}>{1}</option>", item.emp_codigo, item.emp_nombre, ((item.emp_codigo == uxe.uxe_empresa) ? "selected" : ""));
                }
                html.AppendLine("</select></div>");
                html.AppendLine("</div>");
            }

   
            html.AppendLine("</div>");

            if (usuario.usr_perfil == Constantes.cPerfilAdministrador || usuario.usr_perfil == Constantes.cPerfilEmpresa)
            {
                html.AppendLine("<div class=\"row\">");
                html.AppendLine("<div class=\"col-sm-6\">");
                //html.AppendLine(" <button type=\"button\" class=\"btn btn-sm btn-default\" onclick='LoadComprobantes();'><i class=\"fa fa-copy\"></i> Comprobantes</button>");
                html.AppendLine(" <button type=\"button\" class=\"btn btn-sm btn-default\" onclick='Faltantes();'><i class=\"fa fa-list-ol\"></i> Faltantes</button>");
                html.AppendLine(" <button type=\"button\" class=\"btn btn-sm btn-primary\" onclick=\"GestionComprobantesAll('reenviar');\"><i class=\"fa fa-sign-out\"></i> Reenviar Todos</button>");
                html.AppendLine(" <button type=\"button\" class=\"btn btn-sm btn-success\" onclick=\"GestionComprobantesAll('verificar');\"><i class=\"fa fa-certificate\"></i> Verificar Todos</button>");
                html.AppendLine(" <button type=\"button\" class=\"btn btn-sm btn-info\"onclick=\"GestionComprobantesAll('mail');\"><i class=\"fa fa-envelope \"></i> Mail Todos</button>");
                html.AppendLine(" <button type=\"button\" class=\"btn btn-sm btn-dark\" onclick=\"GestionComprobantesAll('resetear');\"><i class=\"fa fa-cog\"></i> Resetear Todos</button>");
                

                //html.AppendLine(" <button type=\"button\" class=\"btn btn-sm btn-default\"><i class=\"fa fa-money\"></i> Ventas</button>");
                //html.AppendLine(" <button type=\"button\" class=\"btn btn-sm btn-primary\"><i class=\"fa fa-download\"></i> Descargar PDF</button>");
                html.AppendLine("</div>");
                html.AppendLine("</div>");
            }

            html.AppendLine("</form>");
            return html.ToString();

        }

    

        [WebMethod]
        public string LoadComprobantes(object objeto)
        {

            try
            {
                Comprobante comprobante = new Comprobante(objeto);


                Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
                object strfalta = null;
                tmp.TryGetValue("falta", out strfalta);

                bool falta = false;
                bool.TryParse(strfalta.ToString(), out falta);




                Usuario usr = UsuarioBLL.GetByPK(new Usuario { usr_id = comprobante.crea_usr, usr_id_key = comprobante.crea_usr });


                List<Comprobante> lst = ComprobanteBLL.GetAll(SetWhereClause(comprobante, usr), "com_almacen, com_pventa, com_secuencia");
                StringBuilder html = new StringBuilder();
                html.AppendLine("<div class=\"row\">");
                html.AppendLine("<div class=\"col-md-12 m-b-20\">");
                html.Append("<table>");
                html.AppendLine("<table id='tdcomprobantes' class=\"table table-hover listado\"><thead><tr>");
                //html.AppendLine("<th>CLAVE</th>");
                html.AppendLine("<th >FECHA</th>");
                html.AppendLine("<th >NUMERO</th>");
                html.AppendLine("<th >RUC</th>");
                html.AppendLine("<th >CLIENTE</th>");
                html.AppendLine("<th >ESTADO</th>");
                html.AppendLine("<th >FECHA ENVÍO</th>");
                html.AppendLine("<th >TOTAL</th>");
                html.AppendLine("<th >OPCIONES</th>");
                html.AppendLine("</tr></thead><tbody>");


                int? almacen = null;
                int? pventa = null;
                int? secuencia = null;
                decimal totalventas = 0;
                int cantfalta = 0;

                for (int i = 0; i < lst.Count; i++)
                {
                    Comprobante item = lst[i];
                    bool faltasec = false;
                    if (falta)
                    {
                        if (!almacen.HasValue)
                        {
                            almacen = int.Parse(item.com_almacen);
                            pventa = int.Parse(item.com_pventa);
                            secuencia = int.Parse(item.com_secuencia);

                        }

                        if (almacen == int.Parse(item.com_almacen) && pventa == int.Parse(item.com_pventa))
                        {
                            if (secuencia != int.Parse(item.com_secuencia))
                                faltasec = true;
                        }
                        else
                        {
                            almacen = int.Parse(item.com_almacen);
                            pventa = int.Parse(item.com_pventa);
                            secuencia = int.Parse(item.com_secuencia);
                        }
                    }


                    if (faltasec)
                    {
                        html.AppendLine("<tr class='danger'>");
                        html.AppendLine("<td></td>");
                        html.AppendFormat("<td>{0}</td>", item.com_almacen + "-" + item.com_pventa + "-" + Functions.Formatos.FillLeft(secuencia.ToString(), "0", 9));
                        html.AppendLine("<td></td>");
                        html.AppendLine("<td></td>");
                        html.AppendLine("<td>Faltante</td>");
                        html.AppendLine("<td></td>");
                        html.AppendLine("<td></td>");
                        html.AppendLine("<td></td>");
                        html.AppendLine("</tr>");
                        secuencia++;
                        i--;
                        cantfalta++;
                    }
                    else
                    {

                        html.AppendLine("<tr>");
                        //html.AppendFormat("<td>{0}</td>", item.com_numero);
                        html.AppendFormat("<td>{0}</td>", item.com_fecha.Value.ToShortDateString());
                        //if (usr.usr_perfil == Constantes.cPerfilEmpresa || usr.usr_perfil == Constantes.cPerfilAdministrador)
                        //if (usr.usr_perfil == Constantes.cPerfilAdministrador)
                        if ((usr.usr_edit ?? 0) == 1)
                            html.AppendFormat("<td>{1} {2} {4} {0} {3}</td>", item.com_almacen + "-" + item.com_pventa + "-" + item.com_secuencia, "<i class=\"fa fa-edit\" style ='cursor:pointer;' title = 'Editar Comprobante' onclick='EditComprobante(\"" + item.com_empresa + "\",\"" + item.com_numero + "\")' ></i>", "<i class=\"fa fa-code\" style ='cursor:pointer;' title = 'Editar XML' onclick='EditComprobanteXML(\"" + item.com_empresa + "\",\"" + item.com_numero + "\")'></i>", "<i class=\"fa fa-envelope-o\" style ='cursor:pointer;' title = 'Ver Mails Enviados' onclick='GetMailsComprobante(\"" + item.com_empresa + "\",\"" + item.com_numero + "\")'></i>", item.com_formatonom);
                        //html.AppendFormat("<td>{1} {2} {0}</td>", item.com_almacen + "-" + item.com_pventa + "-" + item.com_secuencia, "<i class=\"fa fa-edit\" style ='cursor:pointer;' title = 'Editar Comprobante' onclick='EditComprobante(\"" + item.com_empresa+ "\",\"" + item.com_numero + "\")' ></i>", "<i class=\"fa fa-edit\" style ='cursor:pointer;' title = 'Editar Comprobante desde XML' onclick='Edit(\"" + item.com_empresa+ "\",\"" + item.com_numero + "\")' ></i>");
                        else
                            html.AppendFormat("<td>{0}</td>", item.com_almacen + "-" + item.com_pventa + "-" + item.com_secuencia);
                        html.AppendFormat("<td>{0}</td>", item.com_ruccliente);
                        html.AppendFormat("<td>{0}</td>", item.com_nombrecliente);
                        html.AppendFormat("<td>{0}</td>", Enums.GetEstadoComprobante(item.com_estado.Value));
                        html.AppendFormat("<td>{0}</td>", (item.com_fechaenvia.HasValue) ? item.com_fechaenvia.Value.ToString() : "");
                        html.AppendFormat("<td>{0}</td>", item.com_total);
                        if (usr.usr_perfil == "user")
                            html.AppendFormat("<td>{0} {1} {2}</td>", "<img src='images/pagexml.png' class='respuesta' title = 'Ver XML' onclick='GetXML(\"" + item.com_numero + "\",\"xml\")'>", "<img src='images/page.png' class='respuesta' title = 'Ver XML' onclick='Respuesta(\"" + item.com_numero + "\",\"res\")'>", "<img src='images/pagepdf.png' class='respuesta' title = 'Ver RIDE' onclick='RIDE(\"" + item.com_empresa + "\", \"" + item.com_numero + "\")'>");
                        else if (usr.usr_perfil == Constantes.cPerfilEmpresa)
                            html.AppendFormat("<td>{0} {1} {2} {3} {4} {5} {6} {7}</td>", "<img src='images/pagexml.png' class='respuesta' title = 'Ver XML' onclick='GetXML(\"" + item.com_numero + "\",\"xml\")'>", "<img src='images/page.png' class='respuesta' title = 'Ver Respuesta' onclick='Respuesta(\"" + item.com_numero + "\",\"res\")'>", (item.com_estado <= (int)Enums.EstadoComprobante.Enviado) ? "<img src='images/pagesend.png' class='respuesta' title = 'Reenviar' onclick='Reenviar(\"" + item.com_empresa + "\", \"" + item.com_numero + "\")'>" : "", (item.com_estado == (int)Enums.EstadoComprobante.Recibido || item.com_estado == (int)Enums.EstadoComprobante.Devuelto) ? "<img src='images/pageverify.png' class='respuesta' title = 'Verificar autorizacion' onclick='Verificar(\"" + item.com_numero + "\")'>" : "", (item.com_estado != (int)Enums.EstadoComprobante.Autorizado) ? "<img src='images/pagegear.png' class='respuesta' title = 'Resetear comprobante' onclick='Resetear(\"" + item.com_empresa + "\", \"" + item.com_numero + "\")'>" : "", "<img src='images/pagepdf.png' class='respuesta' title = 'Ver RIDE' onclick='RIDE(\"" + item.com_empresa + "\", \"" + item.com_numero + "\")'>", "<img src='images/email.png' class='respuesta' title = 'Enviar Mail' onclick='Mail(\"" + item.com_empresa + "\", \"" + item.com_numero + "\")'>", (item.com_estado != (int)Enums.EstadoComprobante.Autorizado) ? "<img src='images/delete.png' class='respuesta' title = 'Eliminar comprobante' onclick='Eliminar(\"" + item.com_empresa + "\", \"" + item.com_numero + "\")'>" : "");
                        else if (usr.usr_perfil == Constantes.cPerfilAdministrador)
                            html.AppendFormat("<td>{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}</td>", "<img src='images/pagexml.png' class='respuesta' title = 'Ver XML' onclick='GetXML(\"" + item.com_numero + "\",\"xml\")'>", "<img src='images/page.png' class='respuesta' title = 'Ver Respuesta' onclick='Respuesta(\"" + item.com_numero + "\",\"res\")'>", (item.com_estado <= (int)Enums.EstadoComprobante.Enviado) ? "<img src='images/pagesend.png' class='respuesta' title = 'Reenviar' onclick='Reenviar(\"" + item.com_empresa + "\", \"" + item.com_numero + "\")'>" : "", (item.com_estado == (int)Enums.EstadoComprobante.Recibido || item.com_estado == (int)Enums.EstadoComprobante.Devuelto) ? "<img src='images/pageverify.png' class='respuesta' title = 'Verificar autorizacion' onclick='Verificar(\"" + item.com_numero + "\")'>" : "", (item.com_estado != (int)Enums.EstadoComprobante.Autorizado) ? "<img src='images/pagegear.png' class='respuesta' title = 'Resetear comprobante' onclick='Resetear(\"" + item.com_empresa + "\", \"" + item.com_numero + "\")'>" : "", "<img src='images/pagepdf.png' class='respuesta' title = 'Ver RIDE' onclick='RIDE(\"" + item.com_empresa + "\", \"" + item.com_numero + "\")'>", "<img src='images/email.png' class='respuesta' title = 'Enviar Mail' onclick='Mail(\"" + item.com_empresa + "\", \"" + item.com_numero + "\")'>", (item.com_estado == (int)Enums.EstadoComprobante.NoAutorizado || item.com_estado == (int)Enums.EstadoComprobante.Recibido) ? "<img src='images/asterisk.png' class='respuesta' title = 'Buscar Autorizacion' onclick='BuscarAutorizacion(\"" + item.com_empresa + "\", \"" + item.com_numero + "\")'>" : "", (item.com_estado != (int)Enums.EstadoComprobante.Autorizado) ? "<img src='images/delete.png' class='respuesta' title = 'Eliminar comprobante' onclick='Eliminar(\"" + item.com_empresa + "\", \"" + item.com_numero + "\")'>" : "", (item.com_estado == (int)Enums.EstadoComprobante.Devuelto) ? "<i class=\"fa fa-edit\" style ='cursor:pointer;' title = 'Get Mensaje Devolucion' onclick='GetMensaje(\"" + item.com_empresa + "\",\"" + item.com_numero + "\")' ></i>" : "");

                        html.AppendLine("</tr>");
                        totalventas += (item.com_total.HasValue ? item.com_total.Value : 0);
                        secuencia++;
                    }

                }







                //foreach (Comprobante item in lst)
                //{
                //    html.AppendLine("<tr>");
                //    //html.AppendFormat("<td>{0}</td>", item.com_numero);
                //    html.AppendFormat("<td>{0}</td>", item.com_fecha.Value.ToShortDateString());
                //    html.AppendFormat("<td>{0}</td>", item.com_almacen+"-"+item.com_pventa+"-"+item.com_secuencia);
                //    html.AppendFormat("<td>{0}</td>", item.com_ruccliente);
                //    html.AppendFormat("<td>{0}</td>", item.com_nombrecliente);
                //    html.AppendFormat("<td>{0}</td>", Enums.GetEstadoComprobante(item.com_estado.Value));
                //    html.AppendFormat("<td>{0}</td>", (item.com_fechaenvia.HasValue)?item.com_fechaenvia.Value.ToString():"");
                //    html.AppendFormat("<td>{0}</td>", item.com_total);
                //    if (usr.usr_perfil== "user")
                //        html.AppendFormat("<td>{0} {1}</td>", "<img src='images/page.png' class='respuesta' title = 'Ver XML' onclick='Respuesta(\"" + item.com_numero + "\",\"res\")'>",  "<img src='images/pagepdf.png' class='respuesta' title = 'Ver RIDE' onclick='RIDE(\"" + item.com_empresa + "\", \"" + item.com_numero + "\")'>");
                //    else if (usr.usr_perfil == Constantes.cPerfilEmpresa)
                //        html.AppendFormat("<td>{0} {1} {2} {3} {4} {5} {6}</td>", "<img src='images/pagexml.png' class='respuesta' title = 'Ver XML' onclick='Respuesta(\"" + item.com_numero + "\",\"xml\")'>", "<img src='images/page.png' class='respuesta' title = 'Ver Respuesta' onclick='Respuesta(\"" + item.com_numero + "\",\"res\")'>", (item.com_estado <= (int)Enums.EstadoComprobante.Enviado) ? "<img src='images/pagesend.png' class='respuesta' title = 'Reenviar' onclick='Reenviar(\"" + item.com_empresa + "\", \"" + item.com_numero + "\")'>" : "", (item.com_estado == (int)Enums.EstadoComprobante.Recibido) ? "<img src='images/pageverify.png' class='respuesta' title = 'Verificar autorizacion' onclick='Verificar(\"" + item.com_numero + "\")'>" : "", (item.com_estado != (int)Enums.EstadoComprobante.Autorizado) ? "<img src='images/pagegear.png' class='respuesta' title = 'Resetear comprobante' onclick='Resetear(\"" + item.com_empresa + "\", \"" + item.com_numero + "\")'>" : "", "<img src='images/pagepdf.png' class='respuesta' title = 'Ver RIDE' onclick='RIDE(\"" + item.com_empresa + "\", \"" + item.com_numero + "\")'>", "<img src='images/email.png' class='respuesta' title = 'Enviar Mail' onclick='Mail(\"" + item.com_empresa + "\", \"" + item.com_numero + "\")'>");
                //    else if (usr.usr_perfil == Constantes.cPerfilAdministrador)
                //        html.AppendFormat("<td>{0} {1} {2} {3} {4} {5} {6} {7}</td>", "<img src='images/pagexml.png' class='respuesta' title = 'Ver XML' onclick='Respuesta(\"" + item.com_numero + "\",\"xml\")'>", "<img src='images/page.png' class='respuesta' title = 'Ver Respuesta' onclick='Respuesta(\"" + item.com_numero + "\",\"res\")'>", (item.com_estado <= (int)Enums.EstadoComprobante.Enviado) ? "<img src='images/pagesend.png' class='respuesta' title = 'Reenviar' onclick='Reenviar(\"" + item.com_empresa + "\", \"" + item.com_numero + "\")'>" : "", (item.com_estado == (int)Enums.EstadoComprobante.Recibido) ? "<img src='images/pageverify.png' class='respuesta' title = 'Verificar autorizacion' onclick='Verificar(\"" + item.com_numero + "\")'>" : "", (item.com_estado != (int)Enums.EstadoComprobante.Autorizado) ? "<img src='images/pagegear.png' class='respuesta' title = 'Resetear comprobante' onclick='Resetear(\"" + item.com_empresa + "\", \"" + item.com_numero + "\")'>" : "", "<img src='images/pagepdf.png' class='respuesta' title = 'Ver RIDE' onclick='RIDE(\"" + item.com_empresa + "\", \"" + item.com_numero + "\")'>", "<img src='images/email.png' class='respuesta' title = 'Enviar Mail' onclick='Mail(\"" + item.com_empresa + "\", \"" + item.com_numero + "\")'>", (item.com_estado == (int)Enums.EstadoComprobante.NoAutorizado) ? "<img src='images/asterisk.png' class='respuesta' title = 'Buscar Autorizacion' onclick='BuscarAutorizacion(\"" + item.com_empresa + "\", \"" + item.com_numero + "\")'>":"");

                //    html.AppendLine("</tr>");
                //}
                html.AppendLine("</tbody></table>");
                html.AppendLine("</div>");
                html.AppendLine("</div>");
                html.AppendLine("<div class=\"row\">");
                html.AppendLine("<div class=\"col-md-6 m-b-20\">");
                html.Append("<table>");
                html.AppendLine("<table id='tdtotales' class=\"table table-hover listado\"><thead><tr>");
                html.AppendLine("<th colspan='2'>TOTALES</th>");
                html.AppendLine("</tr></thead><tbody>");
                html.AppendLine("<tr>");
                html.AppendFormat("<td># Comprobantes</td><td>{0}</td>", lst.Count);
                html.AppendLine("</tr>");
                if (falta)
                {
                    html.AppendLine("<tr>");
                    html.AppendFormat("<td># Comprobantes Faltantes</td><td>{0}</td>", cantfalta);
                    html.AppendLine("</tr>");
                }

                html.AppendLine("<tr>");
                html.AppendFormat("<td>Total Ventas</td><td>{0}</td>", totalventas.ToString());
                html.AppendLine("</tr>");
                html.AppendLine("</thead><tbody>");
                html.AppendLine("</div>");
                html.AppendLine("</div>");

                return html.ToString();
            }
            catch (Exception ex)
            {
                ExceptionHandling.Log.AddLog("LOADCOMPROBANTES ERROR:" + ex.Message + " " + ex.InnerException);
                return "Error " + ex.Message;

            }

        }

        public WhereParams SetWhereClause(Comprobante obj, Usuario usr)
        {
            bool vacio = true;
            int contador = 0;
            WhereParams parametros = new WhereParams();
            List<object> valores = new List<object>();

            //if (obj.com_empresa > 0)
            //{
                parametros.where += ((parametros.where != "") ? " and " : "") + " com_empresa = {" + contador + "} and emp_estado=1 "; //solo empresas activas
                valores.Add(obj.com_empresa);
                contador++;
                vacio = false;
            //}


            //if (!string.IsNullOrEmpty(obj.crea_usr))
            //{
                //Usuario usr = UsuarioBLL.GetByPK(new Usuario { usr_id = obj.crea_usr, usr_id_key = obj.crea_usr });
                if (usr.usr_perfil == "user")
                {
                    parametros.where += ((parametros.where != "") ? " and " : "") + " com_ruccliente = {" + contador + "} ";
                    valores.Add(obj.crea_usr);
                    contador++;
                }
            //}


            //if (obj.com_fecha.HasValue)
            //{
            //    if (obj.com_fecha.Value > DateTime.MinValue)
            //    {
            //        parametros.where += ((parametros.where != "") ? " and " : "") + " com_fecha between {" + contador + "} ";
            //        valores.Add(obj.com_fecha);
            //        contador++;
            //        parametros.where += ((parametros.where != "") ? " and " : "") + "   {" + contador + "} ";
            //        valores.Add(obj.com_fecha.Value.AddDays(1));
            //        contador++;
            //        vacio = false;

            //    }
                 
            //}


                if (obj.crea_fecha.HasValue)
                {
                    if (obj.crea_fecha.Value > DateTime.MinValue)
                    {
                        parametros.where += ((parametros.where != "") ? " and " : "") + " com_fecha >= {" + contador + "} ";
                        valores.Add(obj.crea_fecha);
                        contador++;
                        vacio = false;

                    }
                }
                if (obj.mod_fecha.HasValue)
                {
                    if (obj.mod_fecha.Value > DateTime.MinValue)
                    {
                        parametros.where += ((parametros.where != "") ? " and " : "") + " com_fecha <= {" + contador + "} ";
                        valores.Add(obj.mod_fecha);
                        contador++;
                        vacio = false;

                    }
                }
                //if (obj.com_fecha.HasValue)
                //{
                //    if (obj.com_fecha.Value > DateTime.MinValue)
                //    {
                //        parametros.where += ((parametros.where != "") ? " and " : "") + " com_fecha = {" + contador + "} ";
                //        valores.Add(obj.com_fecha);
                //        contador++;                        
                //        vacio = false;

                //    }

                //}

            if (!string.IsNullOrEmpty(obj.com_nombrecliente))//CLIENTE 
            {
                
                parametros.where += ((parametros.where != "") ? " and " : "") + " (com_nombrecliente ILIKE {" + contador + "} or com_ruccliente ILIKE {" + contador + "}) ";
                valores.Add("%" + obj.com_nombrecliente + "%");
                contador++;
                vacio = false;
            }
            if (!string.IsNullOrEmpty(obj.com_almacen))//ALMACEN
            {
                parametros.where += ((parametros.where != "") ? " and " : "") + " com_almacen ILIKE  {" + contador + "} ";
                valores.Add("%" + obj.com_almacen+ "%");
                contador++;
                vacio = false;
            }
            if (!string.IsNullOrEmpty(obj.com_pventa))//PVENTA 
            {
                parametros.where += ((parametros.where != "") ? " and " : "") + " com_pventa ILIKE  {" + contador + "} ";
                valores.Add("%" + obj.com_pventa+ "%");
                contador++;
                vacio = false;
            }
            if (!string.IsNullOrEmpty(obj.com_secuencia))//CLIENTE 
            {
                parametros.where += ((parametros.where != "") ? " and " : "") + " com_secuencia ILIKE  {" + contador + "} ";
                valores.Add("%" + obj.com_secuencia+ "%");
                contador++;
                vacio = false;
            }
            if (obj.com_estado.HasValue)
            {
                parametros.where += ((parametros.where != "") ? " and " : "") + " com_estado = {" + contador + "} ";
                valores.Add(obj.com_estado);
                contador++;
                vacio = false;
            }
            else
            {
                parametros.where += ((parametros.where != "") ? " and " : "") + " com_estado <> {" + contador + "} ";
                valores.Add((int)Enums.EstadoComprobante.Eliminado);
                contador++;
                vacio = false;
            }
            if (obj.com_ambiente.HasValue)
            {
                parametros.where += ((parametros.where != "") ? " and " : "") + " com_ambiente = {" + contador + "} ";
                valores.Add(obj.com_ambiente);
                contador++;
                vacio = false;
            }
            if (obj.com_formato.HasValue)
            {
                parametros.where += ((parametros.where != "") ? " and " : "") + " com_formato = {" + contador + "} ";
                valores.Add(obj.com_formato);
                contador++;
                vacio = false;
            }
            

            //if (!string.IsNullOrEmpty(obj.crea_usr))
            //{
            //    parametros.where += ((parametros.where != "") ? " and " : "") + " crea_usr = {" + contador + "} ";
            //    valores.Add(obj.crea_usr);
            //    contador++;
            //}

            parametros.valores = valores.ToArray();
            return parametros;
           

        }


        [WebMethod]
        public string SendMail(object objeto)
        {

            Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
            object empresa = null;
            object asunto = null;
            
            tmp.TryGetValue("empresa", out empresa);
            tmp.TryGetValue("asunto", out asunto);
            

            return Proceso.CorreoMasivo(int.Parse(empresa.ToString()), asunto.ToString());

           
        }


        [WebMethod]
        public string GetMailsComprobante(object objeto)
        {

            Comprobante comprobante = new Comprobante(objeto);
            comprobante.com_empresa_key = comprobante.com_empresa;
            comprobante.com_numero_key = comprobante.com_numero;

            Usuario usr = UsuarioBLL.GetByPK(new Usuario { usr_id = comprobante.crea_usr, usr_id_key = comprobante.crea_usr });

            comprobante = ComprobanteBLL.GetByPK(comprobante);
            Formato formato = FormatoBLL.GetByPK(new Formato { for_empresa = comprobante.com_empresa, for_empresa_key = comprobante.com_empresa, for_codigo = comprobante.com_formato.Value, for_codigo_key = comprobante.com_formato.Value });
            List<Correo> lst = CorreoBLL.GetAll(new WhereParams("cor_empresa={0} and cor_comprobante={1}", comprobante.com_empresa, comprobante.com_numero), "cor_fecha");

            StringBuilder html = new StringBuilder();
            html.AppendLine("<div class=\"row\">");
            html.AppendLine("<div class=\"col-md-12\">");
            html.Append("<table>");
            html.AppendLine("<table class=\"table table-hover listado\"><thead><tr>");
            //html.AppendLine("<th>CLAVE</th>");
            html.AppendLine("<th >FECHA</th>");
            html.AppendLine("<th >DESTINATARIO</th>");
            html.AppendLine("<th >AUTOMATICO</th>");
            html.AppendLine("<th >RESULTADO</th>");
            html.AppendLine("<th >FECHA RESULTADO</th>");
            html.AppendLine("<th >ESTADO</th>");
            html.AppendLine("</tr></thead><tbody>");


            foreach (Correo item in lst)
            {
                html.Append("<tr>");
                html.AppendFormat("<td>{0}</td>",item.cor_fecha);
                html.AppendFormat("<td>{0}</td>",item.cor_destinatario);
                html.AppendFormat("<td>{0}</td>",item.cor_automatico.Value==1?"SI":"NO");
                html.AppendFormat("<td>{0}</td>",item.cor_resultado);
                html.AppendFormat("<td>{0}</td>",item.cor_fechaenvio);
                html.AppendFormat("<td>{0}</td>", Services.Enums.GetEstadoCorreo(item.cor_estado.Value));
                html.Append("<tr>");
            }
            html.Append("</tbody>");
            html.Append("</table>");
            

            string tipocom = "";
            if (formato.for_tipo == "FAC")
                tipocom = "FACTURA";
            if (formato.for_tipo == "NC")
                tipocom = "NOTA DE CREDITO";
            if (formato.for_tipo == "RET")
                tipocom = "RETENCIÓN";
            if (formato.for_tipo == "GREM")
                tipocom = "GUIA DE REMISIÓN";

            object[] retorno = new object[4];
            retorno[0] = string.Format("Mails {0} No:{1}-{2}-{3}", tipocom, comprobante.com_almacen, comprobante.com_pventa, comprobante.com_secuencia); 
            retorno[1] = html.ToString();
            return new JavaScriptSerializer().Serialize(retorno);


        }




        #region Comprobantes

        [WebMethod]
        public string EditComprobante(object objeto)
        {

            Comprobante comprobante = new Comprobante(objeto);
            comprobante.com_empresa_key = comprobante.com_empresa;
            comprobante.com_numero_key = comprobante.com_numero;

            Usuario usr = UsuarioBLL.GetByPK(new Usuario { usr_id = comprobante.crea_usr, usr_id_key = comprobante.crea_usr });

            comprobante = ComprobanteBLL.GetByPK(comprobante);

            StringBuilder html = new StringBuilder();
            html.AppendFormat("<div id='editcomprobante' data-empresa='{0}' class=\"row\">", comprobante.com_empresa);
            html.AppendLine("<div class=\"col-md-12\">");

            //html.AppendLine("<div class=\"panel panel-default\">");
            //html.AppendLine("<div class=\"panel-heading\">");
            //html.AppendLine("<h3 class=\"panel-title\"><strong>Comprobante</strong> Datos</h3>");
            //html.AppendLine("</div>");
            //html.AppendLine("<div class=\"panel-body\">");
            html.AppendLine("<div class=\"row\">");
            html.AppendLine("<div class=\"col-md-12 col-sm-12 col-xs-12\">");


            html.AppendLine("<div class=\"form-group\">");
            html.AppendLine("<label class=\"form-label\"><strong>Clave</strong></label>");
            html.AppendLine("<span class=\"tips\"></span>");
            html.AppendLine("<div class=\"controls\">");
            html.AppendFormat("<input id='txtclave_e' type=\"text\" class=\"form-control\" value='{0}' disabled='disabled'>", comprobante.com_numero);
            html.AppendLine("</div>");
            html.AppendLine("</div>");


            html.AppendLine("<div class=\"form-group\">");
            html.AppendLine("<label class=\"form-label\"><strong>Número</strong></label>");
            html.AppendLine("<span class=\"tips\"> Almacen, Punto Venta, Secuencia</span>");
            html.AppendLine("<div class=\"controls\">");
            html.AppendFormat("<input id='txtnumero_e' type=\"text\" class=\"form-control\" value='{0}' disabled='disabled'>", (comprobante.com_numero != null) ? comprobante.com_almacen + "-" + comprobante.com_pventa + "-" + comprobante.com_secuencia : "");
            html.AppendLine("</div>");
            html.AppendLine("</div>");


            html.AppendLine("<div class=\"form-group\">");
            html.AppendLine("<label class=\"form-label\"><strong>Ruc</strong></label>");
            html.AppendLine("<span class=\"tips\"></span>");
            html.AppendLine("<div class=\"controls\">");
            html.AppendFormat("<input id='txtruc_e' type=\"text\" class=\"form-control\" value='{0}' disabled='disabled'>", comprobante.com_ruccliente);
            html.AppendLine("</div>");
            html.AppendLine("</div>");



            html.AppendLine("<div class=\"form-group\">");
            html.AppendLine("<label class=\"form-label\"><strong>Cliente</strong></label>");
            html.AppendLine("<span class=\"tips\"> Nombres / Razón social</span>");
            html.AppendLine("<div class=\"controls\">");
            html.AppendFormat("<input id='txtcliente_e' type=\"text\" class=\"form-control\" value='{0}' disabled='disabled'>", comprobante.com_nombrecliente);
            html.AppendLine("</div>");
            html.AppendLine("</div>");


            html.AppendLine("<div class=\"form-group\">");
            html.AppendLine("<label class=\"form-label\"><strong>Fecha</strong></label>");
            html.AppendLine("<span class=\"tips\"> Emisión</span>");
            html.AppendLine("<div class=\"controls\">");
            html.AppendFormat("<input id='txtfecha_e' type=\"text\" class=\"form-control\" value='{0}' disabled='disabled'>", comprobante.com_fecha.ToString());
            html.AppendLine("</div>");
            html.AppendLine("</div>");

            html.AppendLine("<div class=\"form-group\">");
            html.AppendLine("<label class=\"form-label\"><strong>E-mail</strong></label>");
            html.AppendLine("<span class=\"tips\"></span>");
            html.AppendLine("<div class=\"controls\">");
            html.AppendFormat("<input id='txtmail_e' type=\"text\" class=\"form-control\" value='{0}'>", comprobante.com_email.ToString());
            html.AppendLine("</div>");
            html.AppendLine("</div>");


            html.AppendLine("<div class=\"form-group\">");
            html.AppendLine("<label class=\"form-label\"><strong>Estado</strong></label>");
            html.AppendLine("<span class=\"tips\"> </span>");
            html.AppendLine("<div class=\"controls\">");
            html.AppendFormat("<select id='cmbestado_e' class=\"form-control\" {0}>", (usr.usr_perfil == Constantes.cPerfilAdministrador) ? (comprobante.com_estado == (int)Enums.EstadoComprobante.Autorizado ? " disabled " : "") : "disabled");
            html.AppendFormat("<option value=''></option>");
            string[] enumNames = Enum.GetNames(typeof(Enums.EstadoComprobante));
            foreach (string item in enumNames)
            {
                int value = (int)Enum.Parse(typeof(Enums.EstadoComprobante), item);

                html.AppendFormat("<option value='{0}' {2}>{1}</option>", value, item, (value == comprobante.com_estado) ? " selected " : "");
            }
            html.AppendLine("</select>");
            html.AppendLine("</div>");
            html.AppendLine("</div>");



            html.AppendLine("</div>");
            html.AppendLine("</div>");
            html.AppendLine("</div>");
            html.AppendLine("</div>");

            //html.AppendLine("</div>");
            //html.AppendLine("</div>");


            return html.ToString();
        }

        [WebMethod]
        public string SaveComprobante(object objeto)
        {
            Comprobante comprobante = new Comprobante(objeto);

            Comprobante comprobanteUP = new Comprobante(objeto); 
            comprobanteUP.com_empresa_key = comprobante.com_empresa;
            comprobanteUP.com_numero_key = comprobante.com_numero;

            comprobanteUP = ComprobanteBLL.GetByPK(comprobanteUP);
            comprobanteUP.com_empresa_key = comprobante.com_empresa;
            comprobanteUP.com_numero_key = comprobante.com_numero;
            comprobanteUP.com_estado = comprobante.com_estado;
            comprobanteUP.com_email = comprobante.com_email;
            comprobanteUP.mod_usr = comprobante.mod_usr;
            comprobanteUP.mod_fecha = comprobante.mod_fecha;
            ComprobanteBLL.Update(comprobanteUP);
            return new JavaScriptSerializer().Serialize(comprobanteUP);
            
        }



        public string GetComprobanteFromXML(XmlDocument xmldoc)
        {
            XmlNode xmlcla = xmldoc.SelectSingleNode("/factura/infoTributaria/claveAcceso");
            XmlNode xmlruc = xmldoc.SelectSingleNode("/factura/infoTributaria/ruc");
            XmlNode xmlalm = xmldoc.SelectSingleNode("/factura/infoTributaria/estab");
            XmlNode xmlpve = xmldoc.SelectSingleNode("/factura/infoTributaria/ptoEmi");
            XmlNode xmlsec = xmldoc.SelectSingleNode("/factura/infoTributaria/secuencial");
            XmlNode xmlruccli = xmldoc.SelectSingleNode("/factura/infoFactura/identificacionComprador");
            XmlNode xmlcli = xmldoc.SelectSingleNode("/factura/infoFactura/razonSocialComprador");
            XmlNode xmlfec = xmldoc.SelectSingleNode("/factura/infoFactura/fechaEmision");

            XmlNode xmlamb = xmldoc.SelectSingleNode("/factura/infoTributaria/ambiente");
            XmlNode xmlemi = xmldoc.SelectSingleNode("/factura/infoTributaria/tipoEmision");




            StringBuilder html = new StringBuilder();
 
            html.Append("<div class=\"row\">");
            
            html.Append("<div class=\"col-md-6\">");            
            html.Append("<div class=\"panel panel-default\">");
            html.Append("<div class=\"panel-heading\">");
            html.Append("<h3 class=\"panel-title\"><strong>Factura</strong> Datos</h3>");
            html.Append("</div>");
            html.Append("<div class=\"panel-body\">");

                html.Append("<div class=\"row\">");
                    html.Append("<div class=\"col-md-12 col-sm-12 col-xs-12\">");
                        html.Append("<div class=\"form-group\">");
                            html.Append("<label class=\"control-label\">Numero</label");
                            html.Append("<div class=\"controls\">");
                            html.AppendFormat("<input type=\"text\" class=\"form-control\" value='{0}-{1}-{2}' >",xmlalm.InnerText, xmlpve.InnerText, xmlsec.InnerText);
                            html.Append("</div>");
                        html.Append("</div>");                        
                    html.Append("</div>");
            html.Append("</div>");


            html.Append("</div>");
            html.Append("</div>");
            html.Append("</div>");

            html.Append("<div class=\"col-md-6\">");
            html.Append("<div class=\"panel panel-default\">");            
            html.Append("<div class=\"panel-heading\">");
            html.Append("<h3 class=\"panel-title\"><strong>Cliente</strong> Datos</h3>");
            html.Append("</div>");
            html.Append("<div class=\"panel-body\">");



            html.Append("</div>");
            html.Append("</div>");
            html.Append("</div>");



            html.Append("</div>");
            return html.ToString();
        }



        [WebMethod]
        public string GetComprobanteFromXML(object objeto)
        {

            Comprobante comprobante = new Comprobante(objeto);
            comprobante.com_empresa_key = comprobante.com_empresa;
            comprobante.com_numero_key = comprobante.com_numero;



            Usuario usr = UsuarioBLL.GetByPK(new Usuario { usr_id = comprobante.crea_usr, usr_id_key = comprobante.crea_usr });

            comprobante = ComprobanteBLL.GetByPK(comprobante);

            Archivo arc = ArchivoBLL.GetByPK(new Archivo { arc_empresa = comprobante.com_empresa, arc_empresa_key = comprobante.com_empresa, arc_numero = comprobante.com_numero, arc_numero_key = comprobante.com_numero });


            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(arc.arc_xml);

            return GetComprobanteFromXML(xmldoc);

      
        }


        [WebMethod]
        public string EditComprobanteXML(object objeto)
        {

            Comprobante comprobante = new Comprobante(objeto);
            comprobante.com_empresa_key = comprobante.com_empresa;
            comprobante.com_numero_key = comprobante.com_numero;

            Usuario usr = UsuarioBLL.GetByPK(new Usuario { usr_id = comprobante.crea_usr, usr_id_key = comprobante.crea_usr });

            comprobante = ComprobanteBLL.GetByPK(comprobante);

            Archivo arc = ArchivoBLL.GetByPK(new Archivo { arc_empresa = comprobante.com_empresa, arc_empresa_key = comprobante.com_empresa, arc_numero = comprobante.com_numero, arc_numero_key = comprobante.com_numero });
            StringBuilder html = new StringBuilder();
            html.AppendFormat("<div id='editcomprobante' data-empresa='{0}' data-numero='{1}' class=\"row\">", comprobante.com_empresa, comprobante.com_numero);
            html.AppendLine("<div class=\"col-md-12\">");
            html.AppendFormat("<textarea id='txtxml' class='area' rows='30' cols='100'>{0}</textarea>", arc.arc_xml);
            html.AppendLine("</div>");
            html.AppendLine("</div>");

            return html.ToString();


        }

        [WebMethod]
        public string SaveComprobanteXML(object objeto)
        {
            Archivo archivo = new Archivo(objeto);

            Archivo archivoUP = new Archivo(objeto);
            archivoUP.arc_empresa_key = archivo.arc_empresa;
            archivoUP.arc_numero_key = archivo.arc_numero;
            archivoUP = ArchivoBLL.GetByPK(archivoUP);

            try
            {
                archivoUP.arc_empresa_key = archivo.arc_empresa;
                archivoUP.arc_numero_key = archivo.arc_numero;
                archivoUP.arc_xml = archivo.arc_xml;
                archivoUP.mod_usr = archivo.mod_usr;
                archivoUP.mod_fecha = archivo.mod_fecha;
                ArchivoBLL.Update(archivoUP);
                return "OK";
            }
            catch (Exception ex)
            {
                return "ERROR:" + ex.Message;
            }

        }



        #endregion

        #region Empresa


        [WebMethod(EnableSession = true)]
        public string SaveEmpresa(object objeto)
        {
            Empresa emp = new Empresa(objeto);

            if (emp.emp_codigo > 0)
            {
                EmpresaBLL.Update(emp);
            }
            else
                EmpresaBLL.Insert(emp);


            return "OK";



        }


        [WebMethod]
        public string GetEmpresas(object objeto)
        {
            StringBuilder html = new StringBuilder();             
            List<Empresa> empresas = EmpresaBLL.GetAll("", "");
            foreach (Empresa emp in empresas)
            {
                html.AppendFormat("<option value = {0}>{1}</option>", emp.emp_codigo,emp.emp_nombre);                
            }

            return html.ToString();
        }

        #endregion


        #region Envio Correos

        public bool EnviarPassword(Usuario usr)
        {
            Mail mail = new Mail();
            mail.to = usr.usr_mail;
            mail.subject = "Envío de Contraseña";
            mail.body = "Estimado <b>" + usr.usr_nombres + "</b><br><br>" +
                "Hemos generado automaticamente su contraseña de acuerdo a su solicitud <br>" +
                "Para acceder a su informacion utilice los siguientes datos <br><br>" +
                "<b>usuario:</b>" + usr.usr_id+ "<br>" +
                "<b>password:</b>" + usr.usr_password + "<br><br><br>" +
                "Atentamente<br>" +
                "SIAC (Sistemas Inteligentes)" +
                "";
            return mail.SendMail(false);

        }

        public bool EnviarConfirmación(Usuario usr)
        {
            Mail mail = new Mail();
            mail.to = usr.usr_mail;
            mail.subject = "Confirmación de Registro";
            mail.body = "Estimado <b>" + usr.usr_nombres + "</b><br><br>" +
                "Su información ha sido registrada correctamente, <br>" +
                "a partir de este momento Ud. puede verificar y descargar sus comprobantes electrónicos.<br><br><br>" +                
                "Para acceder al sitio utilice los siguientes datos <br><br>" +
                "<b>usuario:</b>" + usr.usr_id + "<br>" +
                "<b>password:</b>" + usr.usr_password+ "<br><br><br>" +
                "Atentamente<br>" +
                "SIAC (Sistemas Inteligentes)" +
                "";
            return mail.SendMail(false);

        }

        #endregion

        #region Robot
        [WebMethod]
        public string CheckComprobantes(object objeto)
        {
            Comprobante comprobante = new Comprobante(objeto); 
            
            int contador = 0;
            WhereParams parametros = new WhereParams();
            List<object> valores = new List<object>();

            if (comprobante.com_empresa > 0)
            {
                parametros.where += ((parametros.where != "") ? " and " : "") + " com_empresa = {" + contador + "} ";
                valores.Add(comprobante.com_empresa);
                contador++;
            }
            if (comprobante.com_fecha.HasValue)
            {
                if (comprobante.com_fecha.Value > DateTime.MinValue)
                {
                    parametros.where += ((parametros.where != "") ? " and " : "") + " com_fecha >= {" + contador + "} ";
                    valores.Add(comprobante.com_fecha);
                    contador++;
                }
            }
            if (comprobante.crea_fecha.HasValue)
            {
                if (comprobante.com_fecha.Value > DateTime.MinValue)
                {
                    parametros.where += ((parametros.where != "") ? " and " : "") + " com_fecha <= {" + contador + "} ";
                    valores.Add(comprobante.crea_fecha);
                    contador++;
                }
            }
            if (!string.IsNullOrEmpty(comprobante.com_numero))
            {
                parametros.where += ((parametros.where != "") ? " and " : "") + " com_numero = {" + contador + "} ";
                valores.Add(comprobante.com_numero);
                contador++;
            }
            if (comprobante.com_ambiente.HasValue)
            {
                parametros.where += ((parametros.where != "") ? " and " : "") + " com_ambiente = {" + contador + "} ";
                valores.Add(comprobante.com_ambiente);
                contador++;
            }
            if (comprobante.com_estado.HasValue)
            {
                parametros.where += ((parametros.where != "") ? " and " : "") + " com_estado = {" + contador + "} ";
                valores.Add(comprobante.com_estado);
                contador++;
            }
            else
            {
                parametros.where += ((parametros.where != "") ? " and " : "") + " com_estado <> {" + contador + "} and com_estado <> {" + (contador+1) + "}";
                valores.Add((int)Enums.EstadoComprobante.Autorizado);
                contador++;
                valores.Add((int)Enums.EstadoComprobante.Eliminado);
                contador++;
            }

            //NUEVO CODIGO PARA DEJAR AFUERA LOS COMPRO
            parametros.where += ((parametros.where != "") ? " and " : "") + " emp_estado = {" + contador + "} ";
            valores.Add((int)Enums.EstadoEmpresa.Activa);
            contador++;

            parametros.valores = valores.ToArray();

            int top = 0;
            Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
            object tope = null;
            tmp.TryGetValue("top", out tope);

            int.TryParse(tope.ToString(), out top);
            

            List<Comprobante> lst = new List<Comprobante>();
            if (top > 0)
                lst = ComprobanteBLL.GetAllTop(parametros, "", top);
            else
                lst = ComprobanteBLL.GetAll(parametros, "");

            DateTime inicio = DateTime.Now;

            

            int enviados = 0;
            string strenviados = "";
            int verificados = 0;
            string strverificados= "";
            int reseteados = 0;
            string strreseteados = "";
            int devueltos = 0;
            string strdevueltos = "";

            foreach (Comprobante item in lst)
            {                
                if (item.com_estado < (int)Enums.EstadoComprobante.Recibido)
                {
                    Proceso.EnviarComprobanteAsync(item.com_empresa, item.com_numero);
                    enviados++;
                    strenviados += item.com_almacen + "-" + item.com_pventa + "-" + item.com_secuencia + "<br>";
                }
                if (item.com_estado == (int)Enums.EstadoComprobante.Recibido)
                {
                    Proceso.VerificarComprobanteAsync(item.com_empresa, item.com_numero);
                    verificados++;
                    strverificados += item.com_almacen + "-" + item.com_pventa + "-" + item.com_secuencia + "<br>";
                }
                if (item.com_estado == (int)Enums.EstadoComprobante.Devuelto)
                {
                    if (item.com_mensaje.IndexOf("REGISTRAD") >= 0)
                    {
                        Proceso.VerificarComprobanteAsync(item.com_empresa, item.com_numero);
                        devueltos++;
                        strdevueltos += item.com_almacen + "-" + item.com_pventa + "-" + item.com_secuencia + "<br>";
                    }
                }
                if (item.com_estado == (int)Enums.EstadoComprobante.NoAutorizado)
                {
                    if (item.com_mensaje.IndexOf("FIRMA INVALIDA") >= 0)
                    {
                        Proceso.ResetComprobante(item.com_empresa, item.com_numero);
                        Proceso.EnviarComprobanteAsync(item.com_empresa, item.com_numero);
                        reseteados++;
                        strreseteados += item.com_almacen + "-" + item.com_pventa + "-" + item.com_secuencia + "<br>";
                    }
                    //if (item.com_mensaje.IndexOf("REGISTRADO") >= 0)
                    //{
                    //    Proceso.VerificarComprobanteAsync(item.com_empresa, item.com_numero);
                    //    devueltos++;
                    //    strdevueltos += item.com_almacen + "-" + item.com_pventa + "-" + item.com_secuencia + "<br>";
                    //}
                }
            }

            DateTime fin = DateTime.Now;
            TimeSpan duracion = fin.Subtract(inicio);

            StringBuilder html = new StringBuilder();
            html.AppendLine("<br><br><b> Ejecución Robot SICE</b><br>");
            html.AppendFormat("Inicio   : {0}<br>", inicio.ToString());
            html.AppendFormat("Fin      : {0}<br>", fin.ToString());
            html.AppendFormat("Duración (hh:mm:ss): {0}:{1}:{2}<br>", duracion.Hours,duracion.Minutes,duracion.Seconds);

            html.AppendLine("<table class='robotinfo'><thead><tr>");
            html.AppendFormat("<th>Enviados: {0}</th>", enviados);
            html.AppendFormat("<th>Verificados: {0}</th>", verificados);
            html.AppendFormat("<th>Devueltos Registrados: {0}</th>", devueltos);
            html.AppendFormat("<th>Reseteados: {0}</th>", reseteados);
            html.AppendLine("</tr></thead>");
            html.AppendLine("<tbody><tr>");
            html.AppendFormat("<td>{0}</td>", strenviados);
            html.AppendFormat("<td>{0}</td>", strverificados);
            html.AppendFormat("<td>{0}</td>", strdevueltos);
            html.AppendFormat("<td>{0}</td>", strreseteados);
            html.AppendLine("</tr></tbody></table>");
            return html.ToString();

        }

        [WebMethod]
        public string DevueltoComprobantes(object objeto)
        {
            Comprobante comprobante = new Comprobante(objeto);

            int contador = 0;
            WhereParams parametros = new WhereParams();
            List<object> valores = new List<object>();

            if (comprobante.com_empresa > 0)
            {
                parametros.where += ((parametros.where != "") ? " and " : "") + " com_empresa = {" + contador + "} ";
                valores.Add(comprobante.com_empresa);
                contador++;
            }
            if (comprobante.com_fecha.HasValue)
            {
                if (comprobante.com_fecha.Value > DateTime.MinValue)
                {
                    parametros.where += ((parametros.where != "") ? " and " : "") + " com_fecha >= {" + contador + "} ";
                    valores.Add(comprobante.com_fecha);
                    contador++;
                }
            }
            if (comprobante.crea_fecha.HasValue)
            {
                if (comprobante.com_fecha.Value > DateTime.MinValue)
                {
                    parametros.where += ((parametros.where != "") ? " and " : "") + " com_fecha <= {" + contador + "} ";
                    valores.Add(comprobante.crea_fecha);
                    contador++;
                }
            }
            if (!string.IsNullOrEmpty(comprobante.com_numero))
            {
                parametros.where += ((parametros.where != "") ? " and " : "") + " com_numero = {" + contador + "} ";
                valores.Add(comprobante.com_numero);
                contador++;
            }
            if (comprobante.com_ambiente.HasValue)
            {
                parametros.where += ((parametros.where != "") ? " and " : "") + " com_ambiente = {" + contador + "} ";
                valores.Add(comprobante.com_ambiente);
                contador++;
            }
           
                parametros.where += ((parametros.where != "") ? " and " : "") + " (com_estado = {" + contador + "} or com_estado = {" + (contador + 1) + "})";
                valores.Add((int)Enums.EstadoComprobante.Devuelto);
                contador++;
                valores.Add((int)Enums.EstadoComprobante.Recibido);
                contador++;
           

            //NUEVO CODIGO PARA DEJAR AFUERA LOS COMPRO
            parametros.where += ((parametros.where != "") ? " and " : "") + " emp_estado = {" + contador + "} ";
            valores.Add((int)Enums.EstadoEmpresa.Activa);
            contador++;

            parametros.valores = valores.ToArray();

            int top = 0;
            Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
            object tope = null;
            object sleep = null;
            tmp.TryGetValue("top", out tope);
            tmp.TryGetValue("sleep", out sleep);
            if (sleep == null)
                sleep = "30000";

            int.TryParse(tope.ToString(), out top);


            List<Comprobante> lst = new List<Comprobante>();
            if (top > 0)
                lst = ComprobanteBLL.GetAllTop(parametros, "", top);
            else
                lst = ComprobanteBLL.GetAll(parametros, "");

            DateTime inicio = DateTime.Now;



            int enviados = 0;
            string strenviados = "";
            int verificados = 0;
            string strverificados = "";
            int reseteados = 0;
            string strreseteados = "";
            int devueltos = 0;
            string strdevueltos = "";

            foreach (Comprobante item in lst)
            {                
                if (item.com_estado == (int)Enums.EstadoComprobante.Recibido)
                {
                    Proceso.VerificarComprobanteAsync(item.com_empresa, item.com_numero);
                    verificados++;
                    strverificados += item.com_almacen + "-" + item.com_pventa + "-" + item.com_secuencia + "<br>";
                    System.Threading.Thread.Sleep(int.Parse(sleep.ToString()));
                }
                if (item.com_estado == (int)Enums.EstadoComprobante.Devuelto)
                {
                    if (item.com_mensaje.IndexOf("REGISTRAD") >= 0)
                    {
                        Proceso.VerificarComprobanteAsync(item.com_empresa, item.com_numero);
                        devueltos++;
                        strdevueltos += item.com_almacen + "-" + item.com_pventa + "-" + item.com_secuencia + "<br>";
                        System.Threading.Thread.Sleep(int.Parse(sleep.ToString()));
                    }
                }
            }

            DateTime fin = DateTime.Now;
            TimeSpan duracion = fin.Subtract(inicio);

            StringBuilder html = new StringBuilder();
            html.AppendLine("<br><br><b> Ejecución Robot SICE</b><br>");
            html.AppendFormat("Inicio   : {0}<br>", inicio.ToString());
            html.AppendFormat("Fin      : {0}<br>", fin.ToString());
            html.AppendFormat("Duración (hh:mm:ss): {0}:{1}:{2}<br>", duracion.Hours, duracion.Minutes, duracion.Seconds);

            html.AppendLine("<table class='robotinfo'><thead><tr>");
            html.AppendFormat("<th>Verificados: {0}</th>", verificados);
            html.AppendFormat("<th>Devueltos Registrados: {0}</th>", devueltos);
            html.AppendLine("</tr></thead>");
            html.AppendLine("<tbody><tr>");
            html.AppendFormat("<td>{0}</td>", strverificados);
            html.AppendFormat("<td>{0}</td>", strdevueltos);
            html.AppendLine("</tr></tbody></table>");
            return html.ToString();

        }



        [WebMethod]
        public string RecalcularComprobantes(object objeto)
        {
            Comprobante comprobante = new Comprobante(objeto);

            int contador = 0;
            WhereParams parametros = new WhereParams();
            List<object> valores = new List<object>();

            if (comprobante.com_empresa > 0)
            {
                parametros.where += ((parametros.where != "") ? " and " : "") + " com_empresa = {" + contador + "} ";
                valores.Add(comprobante.com_empresa);
                contador++;
            }
            if (comprobante.com_fecha.HasValue)
            {
                if (comprobante.com_fecha.Value > DateTime.MinValue)
                {
                    parametros.where += ((parametros.where != "") ? " and " : "") + " com_fecha = {" + contador + "} ";
                    valores.Add(comprobante.com_fecha);
                    contador++;
                }
            }
            if (!string.IsNullOrEmpty(comprobante.com_numero))
            {
                parametros.where += ((parametros.where != "") ? " and " : "") + " com_numero = {" + contador + "} ";
                valores.Add(comprobante.com_numero);
                contador++;
            }
            if (comprobante.com_ambiente.HasValue)
            {
                parametros.where += ((parametros.where != "") ? " and " : "") + " com_ambiente = {" + contador + "} ";
                valores.Add(comprobante.com_ambiente);
                contador++;
            }
            if (comprobante.com_estado.HasValue)
            {
                parametros.where += ((parametros.where != "") ? " and " : "") + " com_estado = {" + contador + "} ";
                valores.Add(comprobante.com_estado);
                contador++;
            }
            else
            {
                parametros.where += ((parametros.where != "") ? " and " : "") + " com_estado <> {" + contador + "} ";
                valores.Add((int)Enums.EstadoComprobante.Eliminado);
                contador++;
            }

            parametros.valores = valores.ToArray();
            List<Comprobante> lst = ComprobanteBLL.GetAll(parametros, "");

            foreach (Comprobante item in lst)
            {

                Proceso.ResetValores(item.com_empresa,item.com_numero);           
            }


            return "OK";

        }


        [WebMethod]
        public string RunService(object objeto)
        {
            Packages.General.RunVerificacion();
            return "OK";

        }


        [WebMethod]
        public string Resend(object objeto)
        {

            Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
            object fecha = null;
            object empresa = null;

            tmp.TryGetValue("empresa", out empresa);
            tmp.TryGetValue("fecha", out fecha);

            DateTime? dfecha = Conversiones.ObjectToDateTimeNull(fecha);
            Int32? dempresa = Conversiones.ObjectToIntNull(empresa);
            
            int cant = Packages.General.EnviarComprobantesProceso(dempresa, dfecha);

            return cant.ToString()+" comprobantes reenviados";

        }



       

        #endregion


        [WebMethod]
        public string GetAllCli(object objeto)
        {


            Packages.General.GetAllClientes();

            return "OK";

        }


        #region Gestion masiva de comprobantes

        [WebMethod]
        public string GestionComprobantesAll(object objeto)
        {

            Comprobante comprobante = new Comprobante(objeto);

            Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
            object objopcion = null;
            tmp.TryGetValue("opcion", out objopcion);

            Usuario usr = UsuarioBLL.GetByPK(new Usuario { usr_id = comprobante.crea_usr, usr_id_key = comprobante.crea_usr });


            List<Comprobante> lst = ComprobanteBLL.GetAll(SetWhereClause(comprobante, usr), "com_almacen, com_pventa, com_secuencia");


            


       
            DateTime inicio = DateTime.Now;



            int enviados = 0;
            string strenviados = "";
            int verificados = 0;
            string strverificados = "";
            int reseteados = 0;
            string strreseteados = "";
            int devueltos = 0;
            string strdevueltos = "";

            foreach (Comprobante item in lst)
            {

                if (objopcion.ToString() == "reenviar")
                {
                    if (item.com_estado < (int)Enums.EstadoComprobante.Recibido)
                    {
                        Packages.Offline.CrearArchivoXML(item.com_empresa, item.com_numero);
                        Packages.Offline.SendComprobanteAsync(item);
                        //Proceso.EnviarComprobanteAsync(item.com_empresa, item.com_numero);
                        enviados++;
                        strenviados += item.com_almacen + "-" + item.com_pventa + "-" + item.com_secuencia + "<br>";
                    }                    

                }
                if (objopcion.ToString() == "verificar")
                {
                    if (item.com_estado == (int)Enums.EstadoComprobante.Recibido)
                    {
                        Packages.Offline.VerifyComprobanteAsync(item);
                        //Proceso.VerificarComprobanteAsync(item.com_empresa, item.com_numero);
                        verificados++;
                        strverificados += item.com_almacen + "-" + item.com_pventa + "-" + item.com_secuencia + "<br>";
                    }

                    if (item.com_estado == (int)Enums.EstadoComprobante.Devuelto)
                    {
                        if (item.com_mensaje.IndexOf("REGISTRAD") >= 0)
                        {
                            Packages.Offline.VerifyComprobanteAsync(item);
                            //Proceso.VerificarComprobanteAsync(item.com_empresa, item.com_numero);
                            devueltos++;
                            strdevueltos += item.com_almacen + "-" + item.com_pventa + "-" + item.com_secuencia + "<br>";
                        }
                    }
                }

                if (objopcion.ToString() == "resetar")
                {
                    if (item.com_estado != (int)Enums.EstadoComprobante.Autorizado && item.com_estado != (int)Enums.EstadoComprobante.Proceso)
                    {
                        //Packages.Offline.ResetComprobanteAsync(item);
                        Proceso.ResetComprobante(item.com_empresa, item.com_numero);
                        //Proceso.EnviarComprobanteAsync(item.com_empresa, item.com_numero);
                        reseteados++;
                        strreseteados += item.com_almacen + "-" + item.com_pventa + "-" + item.com_secuencia + "<br>";
                    }
                }
                //if (objopcion.ToString() == "mail")
                //{
                //    if (item.com_estado != (int)Enums.EstadoComprobante.Autorizado && item.com_estado != (int)Enums.EstadoComprobante.Proceso)
                //    {
                //        Proceso.ResetComprobante(item.com_empresa, item.com_numero);
                //        Proceso.EnviarComprobanteAsync(item.com_empresa, item.com_numero);
                //        reseteados++;
                //        strreseteados += item.com_almacen + "-" + item.com_pventa + "-" + item.com_secuencia + "<br>";
                //    }
                //}

            }

            DateTime fin = DateTime.Now;
            TimeSpan duracion = fin.Subtract(inicio);

            if (enviados > 0)
                return "Enviados: " + enviados;
            if (verificados > 0)
                return "Verificados: " + verificados;
            if (reseteados > 0)
                return "Reseteados: " + reseteados;
            return "Ok";


        }

        [WebMethod]
        public string ImpresionMasiva(object objeto)
        {
                       

            int? empresa = (int?)Dictionaries.GetObject(objeto, "empresa", typeof(int?));
            DateTime? desde = (DateTime?)Dictionaries.GetObject(objeto, "desde", typeof(DateTime?));
            DateTime? hasta = (DateTime?)Dictionaries.GetObject(objeto, "hasta", typeof(DateTime?));

            WhereParams whereParams = new WhereParams();
            //whereParams.where = "com_empresa=" + empresa + " and com_estado<> " + (int)Enums.EstadoComprobante.Eliminado;
            whereParams.where = "com_empresa=" + empresa + " and com_estado= " + (int)Enums.EstadoComprobante.Autorizado;
            whereParams.where += " and com_fecha between {0} and {1}";
            List<object> valores = new List<object>();
            valores.Add(desde);
            valores.Add(hasta);

            whereParams.valores = valores.ToArray();    


            List<Comprobante> lst = ComprobanteBLL.GetAll(whereParams, "com_fecha");
            int i = 0;
            foreach (var item in lst)
            {
                Comprobante comprobante = Packages.XmlReader.CargarFactura(item);
                Services.Pdf.CreatePDF(comprobante);
                i++;
            }

            return lst.Count() + " comprobantes impresos";


        }



        #endregion

        #region Automata SRI

        [WebMethod]
        public string GetAutomataData(object objeto)
        {
            try
            {
                Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
                object empObj = null;
                tmp.TryGetValue("empresa", out empObj);
                int empresa = 0;
                int.TryParse(empObj != null ? empObj.ToString() : "0", out empresa);

                WhereParams where = empresa > 0
                    ? new WhereParams("ae_empresa = {0}", empresa)
                    : new WhereParams("1=1");

                List<AutomataEjecucion> lista = AutomataEjecucionBLL.GetAllTop(where, "ae_fecha_inicio DESC", 20);

                StringBuilder html = new StringBuilder();
                html.AppendLine("<table class='table table-bordered table-sm' id='tblAutomata'>");
                html.AppendLine("<thead><tr>");
                html.AppendLine("<th>Empresa</th><th>Inicio</th><th>Fin</th><th>Procesados</th><th>Autorizados</th><th>Alertas</th><th>Modo</th><th>Logs</th>");
                html.AppendLine("</tr></thead><tbody>");

                foreach (AutomataEjecucion ej in lista)
                {
                    string modo = (ej.ae_simulacion ?? false) ? "<span style='color:orange'>SIM</span>" : "<span style='color:green'>REAL</span>";
                    string fin = ej.ae_fecha_fin.HasValue ? ej.ae_fecha_fin.Value.ToString("dd/MM HH:mm:ss") : "...";
                    html.AppendFormat("<tr>");
                    html.AppendFormat("<td>{0}</td>", ej.ae_empresa);
                    html.AppendFormat("<td>{0}</td>", ej.ae_fecha_inicio.HasValue ? ej.ae_fecha_inicio.Value.ToString("dd/MM HH:mm:ss") : "");
                    html.AppendFormat("<td>{0}</td>", fin);
                    html.AppendFormat("<td>{0}</td>", ej.ae_procesados ?? 0);
                    html.AppendFormat("<td style='color:green'>{0}</td>", ej.ae_autorizados ?? 0);
                    html.AppendFormat("<td style='color:{1}'>{0}</td>", ej.ae_alertas ?? 0, (ej.ae_alertas ?? 0) > 0 ? "red" : "inherit");
                    html.AppendFormat("<td>{0}</td>", modo);
                    html.AppendFormat("<td><a href='#' onclick='GetAutomataLogs({0});return false;'>Ver</a></td>", ej.ae_id);
                    html.AppendLine("</tr>");
                }

                html.AppendLine("</tbody></table>");
                return html.ToString();
            }
            catch (Exception ex)
            {
                return "<p style='color:red'>Error: " + ex.Message + "</p>";
            }
        }

        [WebMethod]
        public string GetAutomataLogs(object objeto)
        {
            try
            {
                Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
                object ejObj = null;
                tmp.TryGetValue("ejecucion", out ejObj);
                int ejecucion = int.Parse(ejObj.ToString());

                List<AutomataLog> logs = AutomataLogBLL.GetAll(
                    new WhereParams("al_ejecucion = {0}", ejecucion), "al_fecha ASC");

                StringBuilder html = new StringBuilder();
                html.AppendFormat("<h5>Logs ejecución #{0}</h5>", ejecucion);
                html.AppendLine("<table class='table table-bordered table-sm'>");
                html.AppendLine("<thead><tr><th>Hora</th><th>Número</th><th>Acción</th><th>Resultado</th><th>Mensaje</th></tr></thead><tbody>");

                foreach (AutomataLog log in logs)
                {
                    string color = log.al_resultado == "OK" ? "" : log.al_resultado == "ERROR" ? "danger" : "warning";
                    html.AppendFormat("<tr class='{0}'>", color);
                    html.AppendFormat("<td>{0}</td>", log.al_fecha.HasValue ? log.al_fecha.Value.ToString("HH:mm:ss") : "");
                    html.AppendFormat("<td>{0}</td>", log.al_numero_legible ?? log.al_numero);
                    html.AppendFormat("<td>{0}</td>", log.al_accion);
                    html.AppendFormat("<td>{0}</td>", log.al_resultado);
                    html.AppendFormat("<td style='max-width:300px;word-wrap:break-word'>{0}</td>", HttpUtility.HtmlEncode(log.al_mensaje ?? ""));
                    html.AppendLine("</tr>");
                }

                html.AppendLine("</tbody></table>");
                return html.ToString();
            }
            catch (Exception ex)
            {
                return "<p style='color:red'>Error: " + ex.Message + "</p>";
            }
        }

        #endregion


        [WebMethod]
        public string SendPrueba(object objeto)
        {

            Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
            object empresa = null;
            object numero = null;
            object est = null;
            object pem= null;
            object dir = null;

            tmp.TryGetValue("empresa", out empresa);
            tmp.TryGetValue("numero", out numero);
            tmp.TryGetValue("est", out est);
            tmp.TryGetValue("pem", out pem);
            tmp.TryGetValue("dir", out dir);
            

            string xml = Packages.General.GetXMLPrueba(int.Parse(empresa.ToString()), int.Parse(numero.ToString()),est.ToString(),pem.ToString(),dir.ToString());

            return Proceso.RecibirComprobante(xml, "csanmartin@gtec.com.ec", 1);
            


        }


    }
}
