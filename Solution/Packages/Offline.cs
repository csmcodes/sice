using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObjects;
using BusinessLogicLayer;
using Functions;
using Services;
using System.Web;
using System.Data;
using System.Xml;
using System.Reflection;
using System.Web.Script.Serialization;
using System.IO;
using System.Net;

namespace Packages
{
    public class Offline
    {

        #region Metodos Firmar Comprobante

        public static void SingComprobante(Comprobante comprobante)
        {

           try
            {

                List<Certificado> certificados = CertificadoBLL.GetAll(new WhereParams("cer_empresa={0} and  {1} between cer_desde and cer_hasta and cer_estado = {2}", comprobante.com_empresa, comprobante.com_fecha, (int)Enums.EstadoRegistro.ACTIVO), "");
                if (certificados.Count > 0)
                {
                    string path = Constantes.GetParameter("pathfiles");
                    Firma.signxml(path, comprobante.com_empresa, comprobante.com_numero, certificados[0].cer_nombre, certificados[0].cer_password, certificados[0].cer_path, certificados[0].cer_indice);
                }
                else
                    throw new ArgumentException("No existe certificado para el comprobante en la fecha " + comprobante.com_fecha.ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandling.Log.AddExepcion(ex);
            }





        }

        #endregion

        #region Metodos Enviar Comprobante


        delegate string DelegadoSendComprobante(Comprobante comprobante);

        public static void SendComprobanteAsync(Comprobante comprobante)
        {
            DelegadoSendComprobante delegado = new DelegadoSendComprobante(SendComprobante);
            IAsyncResult result = delegado.BeginInvoke(comprobante, null, null);
        }

        public static string SendComprobante(Comprobante comprobante)
        {
            SingComprobante(comprobante);
            string path = Constantes.GetParameter("pathfiles");
            string pathxml = path + "\\temp\\" + comprobante.com_numero + "_" + comprobante.com_empresa + ".xml";

            try
            {
                byte[] buff = null;
                FileStream fs = new FileStream(pathxml, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                long numBytes = new FileInfo(pathxml).Length;
                buff = br.ReadBytes((int)numBytes);

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                if (comprobante.com_ambiente == (int)Enums.Ambiente.PRUEBAS)
                {
                    Packages.RecepcionComprobantesPruebas.RecepcionComprobantesOfflineService pruebas = new RecepcionComprobantesPruebas.RecepcionComprobantesOfflineService();
                    pruebas.clave = comprobante.com_numero;
                    pruebas.empresa = comprobante.com_empresa;

                    pruebas.validarComprobanteCompleted += new RecepcionComprobantesPruebas.validarComprobanteCompletedEventHandler(Pruebas_validarComprobanteCompleted);
                    pruebas.validarComprobanteAsync(buff);

                    //Array a = (Array)pruebas.validarComprobante(buff);

                }
                if (comprobante.com_ambiente == (int)Enums.Ambiente.PRODUCCIÓN)
                {
                    Packages.RecepcionComprobantesProduccion.RecepcionComprobantesOfflineService produccion = new RecepcionComprobantesProduccion.RecepcionComprobantesOfflineService();
                    produccion.clave = comprobante.com_numero;
                    produccion.empresa = comprobante.com_empresa;

                    produccion.validarComprobanteCompleted += new RecepcionComprobantesProduccion.validarComprobanteCompletedEventHandler(Produccion_validarComprobanteCompleted);
                    produccion.validarComprobanteAsync(buff);

                    //Array a = (Array)pruebas.validarComprobante(buff);

                }



                comprobante.com_estado = (int)Enums.EstadoComprobante.ENVIADO;
                comprobante.com_fechaenvia = DateTime.Now;
                comprobante.com_numero_key = comprobante.com_numero;
                comprobante.com_empresa_key = comprobante.com_empresa;
                ComprobanteBLL.Update(comprobante);



                br.Close();
                br.Dispose();
                fs.Close();
                fs.Dispose();
                return "enviada";
            }
            catch (Exception ex)
            {
                ExceptionHandling.Log.AddExepcion(ex);
                return "no enviada";
            }




        }

        public static void SendComprobanteSync(Comprobante comprobante)
        {
            SingComprobante(comprobante);
            string path = Constantes.GetParameter("pathfiles");
            string pathxml = path + "\\temp\\" + comprobante.com_numero + "_" + comprobante.com_empresa + ".xml";

            byte[] buff = null;
            FileStream fs = new FileStream(pathxml, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(pathxml).Length;
            buff = br.ReadBytes((int)numBytes);
            br.Close();
            br.Dispose();
            fs.Close();
            fs.Dispose();

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            object[] result = null;
            if (comprobante.com_ambiente == (int)Enums.Ambiente.PRUEBAS)
            {
                RecepcionComprobantesPruebas.RecepcionComprobantesOfflineService svc = new RecepcionComprobantesPruebas.RecepcionComprobantesOfflineService();
                result = svc.validarComprobante(buff);
            }
            else if (comprobante.com_ambiente == (int)Enums.Ambiente.PRODUCCIÓN)
            {
                RecepcionComprobantesProduccion.RecepcionComprobantesOfflineService svc = new RecepcionComprobantesProduccion.RecepcionComprobantesOfflineService();
                result = svc.validarComprobante(buff);
            }

            if (result != null && result.Length > 0)
                ValidarComprobante((Array)result[0], comprobante.com_numero, comprobante.com_empresa, sync: true);
        }

        public static void VerifyComprobanteSync(Comprobante comprobante)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            object[] result = null;
            if (comprobante.com_ambiente == (int)Enums.Ambiente.PRUEBAS)
            {
                AutorizacionComprobantesPruebas.AutorizacionComprobantesOfflineService svc = new AutorizacionComprobantesPruebas.AutorizacionComprobantesOfflineService();
                result = svc.autorizacionComprobante(comprobante.com_numero);
            }
            else if (comprobante.com_ambiente == (int)Enums.Ambiente.PRODUCCIÓN)
            {
                AutorizacionComprobantesProduccion.AutorizacionComprobantesOfflineService svc = new AutorizacionComprobantesProduccion.AutorizacionComprobantesOfflineService();
                result = svc.autorizacionComprobante(comprobante.com_numero);
            }

            if (result != null && result.Length > 0)
                AutorizacionComprobante((Array)result[0], comprobante.com_numero, comprobante.com_empresa);
        }

        static void Produccion_validarComprobanteCompleted(object sender, RecepcionComprobantesProduccion.validarComprobanteCompletedEventArgs e)
        {
            try
            {
                RecepcionComprobantesProduccion.RecepcionComprobantesOfflineService clase = (RecepcionComprobantesProduccion.RecepcionComprobantesOfflineService)sender;
                Array a = (Array)e.Result;
                ValidarComprobante(a, clase.clave, clase.empresa);
            }
            catch (Exception ex)
            {
                ExceptionHandling.Log.AddExepcion(ex);
            }
        }

        static void Pruebas_validarComprobanteCompleted(object sender, RecepcionComprobantesPruebas.validarComprobanteCompletedEventArgs e)
        {
            try
            {
                RecepcionComprobantesPruebas.RecepcionComprobantesOfflineService clase = (RecepcionComprobantesPruebas.RecepcionComprobantesOfflineService)sender;
                Array a = (Array)e.Result;
                ValidarComprobante(a, clase.clave, clase.empresa);
            }
            catch (Exception ex)
            {
                ExceptionHandling.Log.AddExepcion(ex);
            }
        }

        public static void ValidarComprobante(Array a, string clave, int empresa, bool sync = false)
        {
            XmlNode[] nodos = (XmlNode[])a.GetValue(0);

            List<Comprobante> lst = ComprobanteBLL.GetAll(new WhereParams("com_numero={0} and com_empresa={1}", clave, empresa), "");
            if (lst.Count > 0)
            {
                Comprobante comprobante = lst[0];
                Archivo arc = ArchivoBLL.GetByPK(new Archivo { arc_empresa = empresa, arc_empresa_key = empresa, arc_numero = comprobante.com_numero, arc_numero_key = comprobante.com_numero });

                XmlNode nodoestado = nodos[0];
                if (nodoestado.InnerText != "RECIBIDA")
                {
                    arc.arc_xmlrespuesta = nodos[1].InnerXml;
                    comprobante.com_estado = (int)Enums.EstadoComprobante.DEVUELTO;
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(arc.arc_xmlrespuesta);
                    comprobante.com_mensaje = GetMensaje(doc);
                }
                else
                {
                    comprobante.com_estado = (int)Enums.EstadoComprobante.RECIBIDO;
                    arc.arc_xmlrespuesta = nodos[0].InnerXml;

                }

                comprobante.com_fecharespuesta = DateTime.Now;
                comprobante.com_numero_key = comprobante.com_numero;
                comprobante.com_empresa_key = comprobante.com_empresa;
                ComprobanteBLL.Update(comprobante);

                arc.arc_empresa_key = arc.arc_empresa;
                arc.arc_numero_key = arc.arc_numero;
                ArchivoBLL.Update(arc);

                if (comprobante.com_estado == (int)Enums.EstadoComprobante.RECIBIDO)
                {
                    CorreoComprobanteAsync(comprobante, comprobante.crea_usr, true);
                    if (sync)
                        VerifyComprobanteSync(comprobante);
                    else
                        VerifyComprobanteAsync(comprobante);
                }

                //Services.Pdf.SavePDF(comprobante.com_empresa, comprobante.com_numero, HttpContext.Current.Server.MapPath("../pdf"), false, "");

                //if (comprobante.com_estado == (int)Enums.EstadoComprobante.Recibido)
                //    VerificarComprobante(comprobante.com_empresa, comprobante.com_numero);
                //else
                //    CorreoErrorComprobante(comprobante.com_empresa, comprobante.com_numero);
            }
        }


        #endregion

        #region Metodos Verificar Comprobante

        delegate string DelegadoVerifyComprobante(Comprobante comprobante);

        public static void VerifyComprobanteAsync(Comprobante comprobante)
        {
            DelegadoVerifyComprobante delegado = new DelegadoVerifyComprobante(VerifyComprobante);
            IAsyncResult result = delegado.BeginInvoke(comprobante, null, null);
        }


        public static string VerifyComprobante(Comprobante comprobante)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            if (comprobante.com_ambiente == (int)Enums.Ambiente.PRUEBAS)
            {
                AutorizacionComprobantesPruebas.AutorizacionComprobantesOfflineService pruebas = new AutorizacionComprobantesPruebas.AutorizacionComprobantesOfflineService();
                pruebas.empresa = comprobante.com_empresa;
                pruebas.clave = comprobante.com_numero;

                pruebas.autorizacionComprobanteCompleted += new AutorizacionComprobantesPruebas.autorizacionComprobanteCompletedEventHandler(Pruebas_autorizacionComprobanteCompleted);
                pruebas.autorizacionComprobanteAsync(comprobante.com_numero);
                return "enviada";
            }
            if (comprobante.com_ambiente == (int)Enums.Ambiente.PRODUCCIÓN)
            {
                AutorizacionComprobantesProduccion.AutorizacionComprobantesOfflineService produccion = new AutorizacionComprobantesProduccion.AutorizacionComprobantesOfflineService();
                produccion.empresa = comprobante.com_empresa;
                produccion.clave = comprobante.com_numero;

                produccion.autorizacionComprobanteCompleted += new AutorizacionComprobantesProduccion.autorizacionComprobanteCompletedEventHandler(Produccion_autorizacionComprobanteCompleted);
                produccion.autorizacionComprobanteAsync(comprobante.com_numero);
                return "enviada";

            }


            return "";

        }

        private static void Produccion_autorizacionComprobanteCompleted(object sender, AutorizacionComprobantesProduccion.autorizacionComprobanteCompletedEventArgs e)
        {
            try
            {
                AutorizacionComprobantesProduccion.AutorizacionComprobantesOfflineService clase = (AutorizacionComprobantesProduccion.AutorizacionComprobantesOfflineService)sender;
                Array a = (Array)e.Result;
                AutorizacionComprobante(a, clase.clave, clase.empresa);
            }
            catch (Exception ex)
            {
                ExceptionHandling.Log.AddExepcion(ex);
            }

        }

        private static void Pruebas_autorizacionComprobanteCompleted(object sender, AutorizacionComprobantesPruebas.autorizacionComprobanteCompletedEventArgs e)
        {
            try
            {
                AutorizacionComprobantesPruebas.AutorizacionComprobantesOfflineService clase = (AutorizacionComprobantesPruebas.AutorizacionComprobantesOfflineService)sender;
                Array a = (Array)e.Result;
                AutorizacionComprobante(a, clase.clave, clase.empresa);
            }
            catch (Exception ex)
            {
                ExceptionHandling.Log.AddExepcion(ex);
            }
        }

        public static void AutorizacionComprobante(Array a, string clave, int empresa)
        {
            try
            {
                XmlNode[] nodos = (XmlNode[])a.GetValue(0);

                XmlNode nodoclave = nodos[0];
                XmlNode nodonumerocomprobantes = nodos[1];

                if (int.Parse(nodonumerocomprobantes.InnerXml) > 0)
                {

                    XmlNode nodoautorizaciones = nodos[2];

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(nodoautorizaciones.OuterXml);

                    List<Comprobante> comprobantes = ComprobanteBLL.GetAll(new WhereParams("com_numero={0}", nodoclave.InnerText), "");
                    foreach (Comprobante comprobante in comprobantes)
                    {
                        Archivo arc = ArchivoBLL.GetByPK(new Archivo { arc_empresa = comprobante.com_empresa, arc_empresa_key = comprobante.com_empresa, arc_numero = comprobante.com_numero, arc_numero_key = comprobante.com_numero});
                        Comprobante comp = FindAutorizacion(doc, comprobante);
                        comprobante.com_autorizacion = comp.com_autorizacion;
                        comprobante.com_fechaautorizacion = comp.com_fechaautorizacion;
                        comprobante.com_fecharespuesta = comp.com_fecharespuesta;
                        comprobante.com_mensaje = comp.com_mensaje;
                        comprobante.com_estado = comp.com_estado;


                        comprobante.com_numero_key = comprobante.com_numero;
                        comprobante.com_empresa_key = comprobante.com_empresa;
                        ComprobanteBLL.Update(comprobante);

                        arc.arc_xmlrespuesta = nodoautorizaciones.OuterXml;
                        arc.arc_empresa_key = arc.arc_empresa;
                        arc.arc_numero_key = arc.arc_numero;
                        ArchivoBLL.Update(arc);


                        //if (comprobante.com_estado == (int)Enums.EstadoComprobante.AUTORIZADO)
                        //{
                        //    string xmlpath = HttpContext.Current.Server.MapPath("../xml/" + comprobante.com_numero + ".xml");
                        //    if (File.Exists(xmlpath))
                        //        File.Delete(xmlpath);
                        //    using (StreamWriter sw = File.CreateText(xmlpath))
                        //    {
                        //        sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
                        //        sw.Write(nodoautorizaciones.InnerXml);
                        //        sw.Flush();
                        //        sw.Close();
                        //    }

                        //    Services.Pdf.SavePDF(comprobante.com_empresa, comprobante.com_numero, HttpContext.Current.Server.MapPath("../pdf"), false, "");
                        //    CorreoComprobante(comprobante.com_empresa, comprobante.com_numero, true);
                        //}
                        //else
                        //{
                        //    CorreoErrorComprobante(comprobante.com_empresa, comprobante.com_numero);
                        //}
                    }





                    //XmlNode nodoestado = nodoautorizaciones.FirstChild.FirstChild;
                    //XmlNode nodonroautorizacion = nodoautorizaciones.FirstChild.SelectSingleNode("numeroAutorizacion");
                    //XmlNode nodofechaautorizacion = nodoautorizaciones.FirstChild.SelectSingleNode("fechaAutorizacion");


                    //List<Comprobante> comprobantes = ComprobanteBLL.GetAll(new WhereParams("com_numero={0}", nodoclave.InnerText), "");
                    //foreach (Comprobante comprobante in comprobantes)
                    //{
                    //    if (comprobante.com_estado != (int)Enums.EstadoComprobante.Autorizado)
                    //    {
                    //        Archivo arc = ArchivoBLL.GetByPK(new Archivo { arc_empresa = comprobante.com_empresa, arc_empresa_key = comprobante.com_empresa, arc_numero = comprobante.com_numero, arc_numero_key = comprobante.com_numero });

                    //        comprobante.com_estado = nodoestado.InnerText == "AUTORIZADO" ? (int)Enums.EstadoComprobante.Autorizado : (int)Enums.EstadoComprobante.NoAutorizado;

                    //        comprobante.com_estado = (nodonroautorizacion != null) ? (int)Enums.EstadoComprobante.Autorizado : (int)Enums.EstadoComprobante.NoAutorizado;


                    //        comprobante.com_autorizacion = (nodonroautorizacion != null) ? nodonroautorizacion.InnerText : "";
                    //        comprobante.com_fechaautorizacion = (nodofechaautorizacion != null) ? nodofechaautorizacion.InnerText : "";
                    //        comprobante.com_fecharespuesta = DateTime.Now;
                    //        comprobante.com_numero_key = comprobante.com_numero;
                    //        comprobante.com_empresa_key = comprobante.com_empresa;
                    //        ComprobanteBLL.Update(comprobante);

                    //        arc.arc_xmlrespuesta = nodoautorizaciones.OuterXml;
                    //        arc.arc_empresa_key = arc.arc_empresa;
                    //        arc.arc_numero_key = arc.arc_numero;
                    //        ArchivoBLL.Update(arc);

                    //        if (comprobante.com_estado == (int)Enums.EstadoComprobante.Autorizado)
                    //        {
                    //            string xmlpath = HttpContext.Current.Server.MapPath("../xml/" + comprobante.com_numero + ".xml");
                    //            if (File.Exists(xmlpath))
                    //                File.Delete(xmlpath);
                    //            using (StreamWriter sw = File.CreateText(xmlpath))
                    //            {
                    //                sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
                    //                sw.Write(nodoautorizaciones.InnerXml);
                    //                sw.Flush();
                    //                sw.Close();
                    //            }

                    //            Services.Pdf.SavePDF(comprobante.com_empresa, comprobante.com_numero, HttpContext.Current.Server.MapPath("../pdf"), false, "");
                    //            CorreoComprobante(comprobante.com_empresa, comprobante.com_numero);
                    //        }
                    //        else
                    //        {
                    //            CorreoErrorComprobante(comprobante.com_empresa, comprobante.com_numero);
                    //        }
                    //    }

                    //}
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.Log.AddLog("ERROR " + clave);
                ExceptionHandling.Log.AddLog(ex);
            }
        }

        #endregion

        public static string GetMensaje(XmlDocument xmldoc)
        {

            string mensaje = "";

            foreach (XmlNode xmlmensaje in xmldoc.SelectNodes("/comprobante/mensajes"))
            {
                XmlNode xmltexto = xmlmensaje.SelectSingleNode("mensaje/mensaje");
                mensaje += xmltexto.InnerText;
            }

            return mensaje;
        }

        public static DateTime? GetDateTime(XmlNode nodo)
        {
            if (nodo != null)
            {
                return GetDateTime(nodo.InnerText);
            }
            return null;
        }

        public static DateTime GetDateTime(string valor)
        {
            DateTime fecha;
            DateTime.TryParse(valor, out fecha);
            return fecha;
        }

        public static Comprobante FindAutorizacion(XmlDocument xmldoc, Comprobante comprobante)
        {

            string mensaje = "";

            foreach (XmlNode xmlautorizacion in xmldoc.SelectNodes("/autorizaciones/autorizacion"))
            {
                XmlNode xmlestado = xmlautorizacion.SelectSingleNode("estado");
                XmlNode xmlnumero = xmlautorizacion.SelectSingleNode("numeroAutorizacion");
                XmlNode xmlfecha = xmlautorizacion.SelectSingleNode("fechaAutorizacion");
                XmlNode xmlmensaje = xmlautorizacion.SelectSingleNode("mensajes");
                if (xmlestado.InnerText == "AUTORIZADO")
                {
                    comprobante.com_estado = (int)Enums.EstadoComprobante.AUTORIZADO;
                    comprobante.com_fechaautorizacion = xmlfecha.InnerText;
                    comprobante.com_autorizacion = xmlnumero.InnerText;
                    comprobante.com_fecharespuesta = DateTime.Now;
                    return comprobante;
                }
                else
                {
                    mensaje += xmlmensaje.InnerText;
                }
            }


            comprobante.com_estado = (int)Enums.EstadoComprobante.NOAUTORIZADO;
            comprobante.com_fecharespuesta = DateTime.Now;
            comprobante.com_mensaje = mensaje;
            return comprobante;
        }


        #region Enviar mail RIDE


        delegate string DelegadoCorreoComprobante(Comprobante comprobante, string usuario, bool automatic);

        public static void CorreoComprobanteAsync(Comprobante comprobante, string usuario, bool automatic)
        {
            DelegadoCorreoComprobante delegado = new DelegadoCorreoComprobante(CorreoComprobante);
            IAsyncResult result = delegado.BeginInvoke(comprobante, usuario, automatic, null, null);
        }


        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email.Trim();
            }
            catch
            {
                return false;
            }
        }

        public static string CorreoComprobante(Comprobante comprobante, string usuario, bool automatic)
        {
            try
            {
                comprobante.com_empresa_key = comprobante.com_empresa;
                comprobante.com_numero_key = comprobante.com_numero;
                comprobante = ComprobanteBLL.GetByPK(comprobante);
                comprobante = XmlReader.CargarFactura(comprobante);
                Empresa empresa = EmpresaBLL.GetByPK(new Empresa { emp_codigo = comprobante.com_empresa, emp_codigo_key = comprobante.com_empresa });
                Archivo archivo = ArchivoBLL.GetByPK(new Archivo { arc_numero = comprobante.com_numero, arc_numero_key = comprobante.com_numero, arc_empresa = comprobante.com_empresa, arc_empresa_key = comprobante.com_empresa_key });
                Formato formato = FormatoBLL.GetByPK(new Formato { for_empresa = comprobante.com_empresa, for_empresa_key = comprobante.com_empresa, for_codigo = comprobante.com_formato.Value, for_codigo_key = comprobante.com_formato.Value });

                string path = Constantes.GetParameter("pathfiles");
                string pathxml = path + "\\temp\\" + comprobante.com_numero + "_" + comprobante.com_empresa + ".xml";
                string pathride = path + "\\temp\\" + string.Format("{0}{1}-{2}-{3}_{4}.pdf",formato.for_tipo, comprobante.com_almacen, comprobante.com_pventa, comprobante.com_secuencia, comprobante.com_empresa);


                if (!File.Exists(pathxml))//RECREA EL XML
                {                    
                    CrearArchivoXML(comprobante.com_empresa, comprobante.com_numero);
                }
                if (!File.Exists(pathride)) //RECREA EL RIDE
                {                    
                    comprobante = XmlReader.CargarFactura(comprobante);
                    pathride = Services.Pdf.CreatePDF(comprobante);                 
                }




                string htmlbody = formato.for_mail;
                string tipocom = "";
                if (formato.for_tipo == "FAC")
                    tipocom = "FACTURA";
                if (formato.for_tipo == "NC")
                    tipocom = "NOTA DE CREDITO";
                if (formato.for_tipo == "RET")
                    tipocom = "RETENCIÓN";
                if (formato.for_tipo == "GREM")
                    tipocom = "GUIA DE REMISIÓN";
                if (formato.for_tipo == "ND")
                    tipocom = "NOTA DE DEBITO";
                if (formato.for_tipo == "LC")
                    tipocom = "LIQUIDACIÓN DE COMPRA";


                List<Correo> lstcorreos = new List<Correo>();
                if (automatic) 
                    lstcorreos = CorreoBLL.GetAll(new WhereParams("cor_empresa={0} and cor_comprobante={1} and cor_automatico={2} and cor_estado={3}", comprobante.com_empresa, comprobante.com_numero, 1, (int)Enums.EstadoCorreo.ENVIADO), "");

;

                string destinatarios = comprobante.com_email;
                destinatarios = destinatarios.Replace(",", ";");
                string[] arraydestinatarios = destinatarios.Split(';');

                foreach (string destinatario in arraydestinatarios)
                {
                    if (IsValidEmail(destinatario))
                    {
                        List<Correo> lst = lstcorreos.FindAll(delegate (Correo c) { return c.cor_destinatario == destinatario; });
                        if (lst.Count == 0)
                        {
                            string strautomatic = !automatic ? "</br>Envío manual " + DateTime.Now.ToString() + "<br>Estado:" + Enums.GetEstadoComprobante(comprobante.com_estado.Value) + "<br>Autorizacion:" + comprobante.com_autorizacion : "";
                            Correo correo = new Correo();
                            correo.cor_empresa = comprobante.com_empresa;
                            correo.cor_comprobante = comprobante.com_numero;
                            correo.cor_fecha = DateTime.Now;
                            correo.cor_origen = empresa.emp_mail;
                            correo.cor_destinatario = destinatario.Trim();
                            correo.cor_adjuntos = pathxml + "|" + pathride;
                            correo.cor_asunto = string.Format("{0} ELECTRÓNICA No:{1}-{2}-{3} de {4}", tipocom, comprobante.com_almacen, comprobante.com_pventa, comprobante.com_secuencia, empresa.emp_nombre);
                            correo.cor_mensaje = htmlbody.Replace("%cliente%", comprobante.com_nombrecliente).Replace("%empresa%", empresa.emp_nombre) + strautomatic;
                            correo.cor_automatico = automatic ? 1 : 0;
                            correo.cor_envios = 0;
                            correo.cor_estado = (int)Enums.EstadoCorreo.ACTIVO;
                            correo.crea_fecha = DateTime.Now;
                            correo.crea_usr = usuario;
                            correo.cor_codigo = CorreoBLL.InsertIdentity(correo);
                            SendCorreo(correo);
                        }
                    }

                }

                return "ok";
            }
            catch (Exception ex)
            {
                ExceptionHandling.Log.AddExepcion(ex);
                return "error";
            }



        }




        delegate string DelegadoSendCorreo(Correo correo);

        public static void SendCorreoAsync(Correo correo)
        {
            DelegadoSendCorreo delegado = new DelegadoSendCorreo(SendCorreo);
            IAsyncResult result = delegado.BeginInvoke(correo, null, null);
        }


        public static string SendCorreo(Correo correo)
        {
            correo.cor_codigo_key = correo.cor_codigo;
            correo.cor_empresa_key = correo.cor_empresa;
            correo.cor_comprobante_key = correo.cor_comprobante;
            correo = CorreoBLL.GetByPK(correo);
            //Actualiza la cantida de envios
            correo.cor_empresa_key = correo.cor_empresa;
            correo.cor_comprobante_key = correo.cor_comprobante;
            correo.cor_codigo_key = correo.cor_codigo;
            correo.cor_envios = correo.cor_envios.HasValue ? (correo.cor_envios + 1) : 1;
            CorreoBLL.Update(correo);




            string pathxml = "";
            string pathride = "";
            string[] adjuntos = correo.cor_adjuntos.Split('|');
            if (adjuntos.Length > 0)
                pathxml = adjuntos[0];
            if (adjuntos.Length > 1)
                pathride = adjuntos[1];

            if (!File.Exists(pathxml))//RECREA EL XML
            {
                Comprobante comprobante = ComprobanteBLL.GetByPK(new Comprobante { com_numero= correo.cor_comprobante, com_numero_key = correo.cor_comprobante, com_empresa = correo.cor_empresa, com_empresa_key = correo.cor_empresa });
                CrearArchivoXML(comprobante.com_empresa,comprobante.com_numero);
            }
            if (!File.Exists(pathride)) //RECREA EL RIDE
            {
                Comprobante comprobante = ComprobanteBLL.GetByPK(new Comprobante { com_numero= correo.cor_comprobante, com_numero_key = correo.cor_comprobante, com_empresa = correo.cor_empresa, com_empresa_key = correo.cor_empresa });
                comprobante = XmlReader.CargarFactura(comprobante);
                pathride = Services.Pdf.CreatePDF(comprobante);
                //pathride = Services.Pdf.SavePDF(comprobante.com_empresa, comprobante.com_numero, HttpContext.Current.Server.MapPath("../pdf"), false, "");
            }


            Mail mail = new Mail();
            if (mail.SendMail(correo))
                return "mail enviado";
            else
                return "mail no enviado";


        }


        delegate string DelegadoResetComprobante(Comprobante comprobante);

        public static void ResetComprobanteAsync(Comprobante comprobante)
        {
            DelegadoResetComprobante delegado = new DelegadoResetComprobante(ResetComprobante);
            IAsyncResult result = delegado.BeginInvoke(comprobante, null, null);
        }


        public static string ResetComprobante(Comprobante comprobante)
        {
            comprobante.com_estado = (int)Enums.EstadoComprobante.PROCESO;
            comprobante.com_numero_key = comprobante.com_numero;
            comprobante.com_empresa_key = comprobante.com_empresa;
            ComprobanteBLL.Update(comprobante);
            SendComprobanteAsync(comprobante);
            return "";

        }


        delegate string DelegadoEliminarComprobante(Comprobante comprobante);

        public static void EliminarComprobanteAsync(Comprobante comprobante)
        {
            DelegadoEliminarComprobante delegado = new DelegadoEliminarComprobante(EliminarComprobante);
            IAsyncResult result = delegado.BeginInvoke(comprobante, null, null);
        }


        public static string EliminarComprobante(Comprobante comprobante)
        {
            comprobante.com_estado = (int)Enums.EstadoComprobante.ANULADO;
            comprobante.com_numero_key = comprobante.com_numero;
            comprobante.com_empresa_key = comprobante.com_empresa;
            ComprobanteBLL.Update(comprobante);
            
            return "";

        }



        #endregion

        #region Generar Archivos


        public static void CrearArchivoXML(int empresa, string numero)
        {
            Archivo archivo = ArchivoBLL.GetByPK(new Archivo { arc_numero = numero, arc_numero_key = numero, arc_empresa = empresa, arc_empresa_key = empresa });
            string path = Constantes.GetParameter("pathfiles");
            string pathxml = path + "\\temp\\" + numero + "_" + empresa + ".xml";
            if (File.Exists(pathxml))
                File.Delete(pathxml);
            // Create a file to write to. 
            using (StreamWriter sw = File.CreateText(pathxml))
            {
                //sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
                sw.Write(archivo.arc_xml);
                sw.Flush();
                sw.Close();
            }
        }

        #endregion


 
    }
}
