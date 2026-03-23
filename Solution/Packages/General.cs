using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObjects;
using BusinessLogicLayer;
using Services;
using System.Web;
using System.Data;
using System.Xml;
using System.IO;
using System.Web.Script.Serialization;

namespace Packages
{
    public class General
    {

        public static List<Comprobante> GetFaltantes(int empresa, string almacen, string pventa, string secuencia, string cliente, int? ambiente, DateTime? desde, DateTime? hasta)
        {

            List<Comprobante> lst = ComprobanteBLL.GetAll(SetWhereClause(empresa, almacen, pventa, secuencia, cliente, ambiente, desde, hasta), "com_almacen, com_pventa, com_secuencia");

            List<Comprobante> lstfalta = new List<Comprobante>();


            int? alm = null;
            int? pve = null;
            int? sec = null;
            int cantfalta = 0;

            for (int i = 0; i < lst.Count; i++)
            {
                Comprobante item = lst[i];
                bool faltasec = false;

                if (!alm.HasValue)
                {
                    alm = int.Parse(item.com_almacen);
                    pve = int.Parse(item.com_pventa);
                    sec = int.Parse(item.com_secuencia);

                }

                if (alm == int.Parse(item.com_almacen) && pve == int.Parse(item.com_pventa))
                {
                    if (sec != int.Parse(item.com_secuencia))
                        faltasec = true;
                }
                else
                {
                    alm = int.Parse(item.com_almacen);
                    pve = int.Parse(item.com_pventa);
                    sec = int.Parse(item.com_secuencia);
                }



                if (faltasec)
                {
                    Comprobante c = new Comprobante();
                    c.com_almacen = item.com_almacen;
                    c.com_pventa = item.com_pventa;
                    c.com_secuencia = Functions.Formatos.FillLeft(sec.ToString(), "0", 9);

                    lstfalta.Add(c);
                    sec++;
                    i--;
                    cantfalta++;
                }
                else
                {
                    sec++;
                }

            }
            return lstfalta;

        }

        public static WhereParams SetWhereClause(int empresa, string almacen, string pventa, string secuencia, string cliente, int? ambiente, DateTime? desde, DateTime? hasta)
        {
            bool vacio = true;
            int contador = 0;
            WhereParams parametros = new WhereParams();
            List<object> valores = new List<object>();

            parametros.where += ((parametros.where != "") ? " and " : "") + " com_empresa = {" + contador + "} ";
            valores.Add(empresa);
            contador++;


            if (desde.HasValue)
            {
                if (desde.Value > DateTime.MinValue)
                {
                    parametros.where += ((parametros.where != "") ? " and " : "") + " com_fecha >= {" + contador + "} ";
                    valores.Add(desde.Value);
                    contador++;
                    vacio = false;

                }
            }
            if (hasta.HasValue)
            {
                if (hasta.Value > DateTime.MinValue)
                {
                    parametros.where += ((parametros.where != "") ? " and " : "") + " com_fecha <= {" + contador + "} ";
                    valores.Add(hasta.Value);
                    contador++;
                    vacio = false;

                }
            }


            if (!string.IsNullOrEmpty(cliente))//CLIENTE 
            {

                parametros.where += ((parametros.where != "") ? " and " : "") + " (com_nombrecliente ILIKE {" + contador + "} or com_ruccliente ILIKE {" + contador + "}) ";
                valores.Add("%" + cliente + "%");
                contador++;
                vacio = false;
            }
            if (!string.IsNullOrEmpty(almacen))//ALMACEN
            {
                parametros.where += ((parametros.where != "") ? " and " : "") + " com_almacen ILIKE  {" + contador + "} ";
                valores.Add("%" + almacen + "%");
                contador++;
                vacio = false;
            }
            if (!string.IsNullOrEmpty(pventa))//PVENTA 
            {
                parametros.where += ((parametros.where != "") ? " and " : "") + " com_pventa ILIKE  {" + contador + "} ";
                valores.Add("%" + pventa + "%");
                contador++;
                vacio = false;
            }
            if (!string.IsNullOrEmpty(secuencia))//SECUENCIA
            {
                parametros.where += ((parametros.where != "") ? " and " : "") + " com_secuencia ILIKE  {" + contador + "} ";
                valores.Add("%" + secuencia + "%");
                contador++;
                vacio = false;
            }

            if (ambiente.HasValue)
            {
                parametros.where += ((parametros.where != "") ? " and " : "") + " com_ambiente = {" + contador + "} ";
                valores.Add(ambiente);
                contador++;
                vacio = false;
            }


            parametros.valores = valores.ToArray();
            return parametros;


        }


        public static string GetString(XmlNode nodo)
        {
            if (nodo != null)
                return nodo.InnerText;
            else
                return null;
        }

        public static Persona GetPersona(string xml)
        {
            Persona p = new Persona();
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(xml);

            var tipo = xmldoc.SelectSingleNode("/").LastChild.Name;
            if (tipo.ToUpper() == "FACTURA")
            {
                p.per_id = GetString(xmldoc.SelectSingleNode("/factura/infoFactura/identificacionComprador"));
                p.per_razon = GetString(xmldoc.SelectSingleNode("/factura/infoFactura/razonSocialComprador"));
                p.per_direccion = GetString(xmldoc.SelectSingleNode("/factura/infoFactura/direccionComprador"));
                p.per_tipoid = GetString(xmldoc.SelectSingleNode("/factura/infoFactura/tipoIdentificacionComprador"));

                XmlNode infoadicional = xmldoc.SelectSingleNode("/factura/infoAdicional");
                if (infoadicional != null)
                {
                    foreach (XmlNode iteminfo in infoadicional.ChildNodes)
                    {
                        if (iteminfo.Attributes["nombre"].Value == "Email")
                            p.per_email = iteminfo.InnerText;
                        if (iteminfo.Attributes["nombre"].Value == "Telefono")
                            p.per_telefono = iteminfo.InnerText;

                    }

                }
                if (Functions.Validaciones.valida_cedularuc(p.per_id))
                    return p;
                else
                    return null;

            }
            return null;
        }

        public static int GetSizeStr(string cadena)
        {
            int largo = 0;
            if (!string.IsNullOrEmpty(cadena))
                largo = cadena.Length;
            return largo;
        }

        public static string GetMail(string mail)
        {
            string newmail = "";
            if (!string.IsNullOrEmpty(mail))
            {
                string[] arraymail = mail.Split(';');
                for (int i = 0; i < arraymail.Length; i++)
                {
                    bool add = true;
                    string m = arraymail[i];
                    if (m.IndexOf("@") >= 0)
                    {
                        if (m.ToUpper().IndexOf("@KARNATAKA.COM") >= 0 || m.ToUpper().IndexOf("@GTEC.COM") >= 0 || m.ToUpper().IndexOf("@INMOT.COM") >= 0 || m.ToUpper().IndexOf("@VYCAST.COM") >= 0 || m.ToUpper().IndexOf("@CUMPLEANOS.COM") >= 0 || m.ToUpper().IndexOf("@NEWTIRE.COM") >= 0)
                            add = false;
                    }
                    else
                        add = false;

                    if (add)
                        newmail += (newmail != "" ? "," : "") + m;
                }

            }

            return newmail;

        }

        public static void GetAllClientes()
        {

            List<Persona> lstpersonas = new List<Persona>();
            List<Usuario> lstusuarios = UsuarioBLL.GetAll("", "");
            DateTime inicio = new DateTime(2014, 1, 1);
            DateTime fin;
            do
            {
                fin = inicio.AddDays(10);
                List<Archivo> lst = ArchivoBLL.GetAll(new WhereParams("crea_fecha between {0} and {1}", inicio, fin), "");

                int c = 0;
                foreach (Archivo item in lst)
                {
                    Persona perxml = GetPersona(item.arc_xml);
                    if (perxml != null)
                    {
                        Persona perlst = lstpersonas.Find(delegate (Persona p) { return p.per_id == perxml.per_id; });
                        if (perlst == null)
                        {
                            Usuario usr = lstusuarios.Find(delegate (Usuario u) { return u.usr_id == perxml.per_id; });
                            if (usr != null)
                                perxml.per_email = GetMail(usr.usr_mail);
                            lstpersonas.Add(perxml);
                        }
                        else
                        {
                            //ACTUALIZA NOMBRES

                            if (GetSizeStr(perxml.per_razon) > GetSizeStr(perlst.per_razon))
                                perlst.per_razon = perxml.per_razon;

                            //ACTUALIZA DIRECCION
                            if (GetSizeStr(perxml.per_direccion) > GetSizeStr(perlst.per_direccion))
                                perlst.per_direccion = perxml.per_direccion;

                            //ACTUALIZA TELEFONO
                            if (GetSizeStr(perxml.per_telefono) > GetSizeStr(perlst.per_telefono))
                                perlst.per_telefono = perxml.per_telefono;

                            //ACTUALIZA EMAIL
                            if (GetSizeStr(perxml.per_email) > GetSizeStr(perlst.per_email))
                                perlst.per_email = perxml.per_email;

                        }
                    }
                    c++;
                }

                lst = null;
                lst = new List<Archivo>();



                inicio = fin;

            }
            while (fin < new DateTime(2017, 12, 31));






            string file = "c:\\TAO\\clientes.csv";

            using (StreamWriter sw = File.CreateText(file))
            {
                foreach (Persona p in lstpersonas)
                {
                    sw.WriteLine(p.ToString());
                }
                sw.Flush();
                sw.Close();
            }

        }



        /************METODOS DEL SERVICIO*****************/

        public static double GetTimerService()
        {
            string serviciotimer = Constantes.GetParameter("serviciotimer");
            double timer = 0;
            double.TryParse(serviciotimer, out timer);

            return timer * 60000; //Convierte en milisegundos

        }


        public static bool RunVerificacion()
        {
            string servicioestado = Constantes.GetParameter("servicioestado");
            string serviciotiempo = Constantes.GetParameter("serviciotiempo");
            string servicioultima = Constantes.GetParameter("servicioultima");


            List<ServicioHorario> horario = new JavaScriptSerializer().Deserialize<List<ServicioHorario>>(Constantes.GetParameter("serviciohorario"));

            bool run = false;

            if (servicioestado == "1")
            {
                DateTime ahora = DateTime.Now;

                ServicioHorario sh = horario.Find(delegate (ServicioHorario s) { return s.dia == (int)ahora.DayOfWeek && s.hora == ahora.Hour; });
                if (sh != null)
                {
                    bool mails = sh.mails == "si";

                    run = true;
                    VerificarCorreos(mails);
                    VerificarComprobantes(mails);
                    ActualizarFechas();
                    CleanTemp();
                }


                //DateTime ultima = new DateTime();
                //DateTime.TryParse(servicioultima, out ultima);

                //Double tiempo = 0;
                //Double.TryParse(serviciotiempo, out tiempo);

                //if (tiempo > 0)
                //{
                //    DateTime ahora = DateTime.Now;

                //    TimeSpan time = ahora.Subtract(ultima);
                //    if (time.TotalMinutes > tiempo)
                //    {
                //        run = true;
                //        VerificarCorreos();
                //        VerificarComprobantes();
                //        ActualizarFechas();
                //        CleanTemp();
                //    }
                //}
            }
            return run;
        }

        public static void ActualizarFechas()
        {
            Parametro servicioultima = Constantes.GetParameterObj("servicioultima");
            servicioultima.par_valor = DateTime.Now.ToString("dd/MM/yyyy");
            servicioultima.par_empresa_key = servicioultima.par_empresa;
            servicioultima.par_id_key = servicioultima.par_id;
            ParametroBLL.Update(servicioultima);

            Parametro fechasrv = Constantes.GetParameterObj("fechasrv");

            DateTime fecha = DateTime.Parse(fechasrv.par_valor).AddDays(1);

            fechasrv.par_valor = fecha.ToString("dd/MM/yyyy");
            fechasrv.par_empresa_key = fechasrv.par_empresa;
            fechasrv.par_id_key = fechasrv.par_id;
            ParametroBLL.Update(fechasrv);


        }

        public static void VerificarCorreos(bool mails)
        {

            //string fecha = Constantes.GetParameter("fechasrv");
            //DateTime dfecha = DateTime.Parse(fecha);

            string correoadmin = Constantes.GetParameter("correoadmin");
            string enviarcorreoadmin = Constantes.GetParameter("enviarcorreoadmin");
            string enviarcorreoemp = Constantes.GetParameter("enviarinfcorreoemp");

            DateTime dfecha = DateTime.Now.Date.AddDays(-1);


            List<Empresa> lstemp = EmpresaBLL.GetAll("", "");

            WhereParams parametros = new WhereParams();
            List<object> valores = new List<object>();
            parametros.where = "cor_estado IN (1,5)";
            //if (!string.IsNullOrEmpty(fecha))
            //{
            parametros.where += " and cor_fecha>={0}";
            valores.Add(dfecha);
            //}
            parametros.valores = valores.ToArray();

            List<Correo> lst = CorreoBLL.GetAll(parametros, "");



            foreach (Empresa empresa in lstemp)
            {

                bool sendmailemp = false;
                string[] arraycorreoemp = enviarcorreoemp.Split(',');
                for (int i = 0; i < arraycorreoemp.Length; i++)
                {
                    if (arraycorreoemp[i] == empresa.emp_codigo.ToString())
                    {
                        sendmailemp = true;
                        break;
                    }

                }



                Correoadmin correoadm = new Correoadmin();

                correoadm.coa_empresa = 1;
                correoadm.coa_fecha = DateTime.Now;
                correoadm.coa_origen = "info@siac.com.ec";
                correoadm.coa_destinatario = correoadmin;
                //correoadm.cor_adjuntos = ;
                correoadm.coa_asunto = string.Format("RESUMEN DE CORREOS NO ENVIADOS Y REENVIADOS  ({0:dd/MM/yyyy}-{1:dd/MM/yyyy HH:mm:ss}) EMPRESA:{2} ", dfecha, DateTime.Now, empresa.emp_nombre);


                Correoadmin correoemp = new Correoadmin();

                correoemp.coa_empresa = empresa.emp_codigo;
                correoemp.coa_fecha = DateTime.Now;
                correoemp.coa_origen = empresa.emp_mail;
                correoemp.coa_destinatario = empresa.emp_mailadm;
                correoemp.coa_asunto = string.Format("NOTIFICACIÓN AUTOMÁTICA DE ERRORES EN ENVIOS DE CORREOS DE COMPROBANTES ({0:dd/MM/yyyy}-{1:dd/MM/yyyy HH:mm:ss}) EMPRESA:{2} ", dfecha, DateTime.Now, empresa.emp_nombre);


                StringBuilder htmlenv = new StringBuilder();
                StringBuilder htmlerr = new StringBuilder();

                List<Correo> lstcor = lst.FindAll(delegate (Correo c) { return c.cor_empresa == empresa.emp_codigo; });

                foreach (Correo item in lstcor)
                {
                    if (item.cor_estado == 1)//1: AUN NO SE ENVIA
                    {
                        htmlenv.AppendFormat("<b>Fecha:</b>{0} <b>Comprobante:</b>{1}-{2}-{3}, <b>Destinatario:</b>{4}, <b>Envios:</b>{5}<br>", item.cor_fecha, item.cor_almacen, item.cor_pventa, item.cor_secuencia, item.cor_destinatario, item.cor_envios);
                        Offline.SendCorreoAsync(item);
                    }
                    if (item.cor_estado == 5)//5: Con error
                    {
                        htmlerr.AppendFormat("<b>Fecha:</b>{0} <b>Comprobante:</b>{1}-{2}-{3}, <b>Destinatario:</b>{4}, <b>Resultado:</b>{5}<br>", item.cor_fecha, item.cor_almacen, item.cor_pventa, item.cor_secuencia, item.cor_destinatario, item.cor_resultado);
                    }
                }

                StringBuilder htmladm = new StringBuilder();
                if (!string.IsNullOrEmpty(htmlenv.ToString()))
                {
                    htmladm.Append("<b>CORREOS COMPROBANTES NO ENVIADOS (Reenviados)</b><br>");
                    htmladm.Append(htmlenv.ToString());
                    htmladm.Append("<br>");
                }

                if (!string.IsNullOrEmpty(htmlerr.ToString()))
                {

                    htmladm.Append("<b>CORREOS COMPROBANTES CON ERROR (Notificar)</b><br>");
                    htmladm.Append(htmlerr.ToString());
                    htmladm.Append("<br>");
                }

                StringBuilder htmlemp = new StringBuilder();
                if (!string.IsNullOrEmpty(htmlerr.ToString()))
                {
                    htmlemp.Append(htmlerr.ToString());
                    htmlemp.Append("<br>");
                }



                if (htmladm.ToString() != "")
                {

                    correoadm.coa_mensaje = htmladm.ToString();
                    correoadm.coa_automatico = 1;
                    correoadm.coa_envios = 1;
                    correoadm.coa_estado = (int)Enums.EstadoCorreo.ACTIVO;
                    correoadm.crea_fecha = DateTime.Now;
                    correoadm.crea_usr = "auto";
                    correoadm.coa_codigo = CorreoadminBLL.InsertIdentity(correoadm);
                    if (enviarcorreoadmin == "1" && mails)
                    {
                        Mail mail = new Mail();
                        mail.SendMail(correoadm);
                    }
                }

                if (htmlemp.ToString() != "")
                {

                    string saludo = string.Format("Sres. <b>{0}</b><br>", empresa.emp_nombre);
                    saludo += string.Format("A continuación se detallan los correos enviados que no han podido ser entregados por errores en el envio.<br><br>");
                    string fin = "<b><br><br>Atentamente<br>SIAC<br></b>";

                    correoemp.coa_mensaje = saludo + htmlemp.ToString() + fin;
                    correoemp.coa_automatico = 1;
                    correoemp.coa_envios = 1;
                    correoemp.coa_estado = (int)Enums.EstadoCorreo.ACTIVO;
                    correoemp.crea_fecha = DateTime.Now;
                    correoemp.crea_usr = "auto";
                    correoemp.coa_codigo = CorreoadminBLL.InsertIdentity(correoemp);
                    if (sendmailemp && mails)
                    {
                        Mail mail = new Mail();
                        mail.SendMail(correoemp);
                    }
                }

            }
        }

        public static void VerificarComprobantes(bool mails)
        {

            //string fecha = Constantes.GetParameter("fechasrv");
            //DateTime dfecha = DateTime.Parse(fecha);


            //DateTime dfecha = DateTime.Now.Date.AddDays(-1);
            DateTime dfecha = DateTime.Now.Date.AddDays(-12);

            string correoadmin = Constantes.GetParameter("correoadmin");

            string enviarcorreoadmin = Constantes.GetParameter("enviarcorreoadmin");
            string enviarcorreoemp = Constantes.GetParameter("enviarcorreoemp");

            List<Empresa> lstemp = EmpresaBLL.GetAll("", "");
            List<ErrorAccion> errores = new JavaScriptSerializer().Deserialize<List<ErrorAccion>>(Constantes.GetParameter("errores"));


            WhereParams parametros = new WhereParams();
            List<object> valores = new List<object>();
            parametros.where = "com_estado IN (4,6)";
            //if (!string.IsNullOrEmpty(fecha))
            //{
            parametros.where += " and com_fecha>={0}";
            valores.Add(dfecha);
            //}
            parametros.valores = valores.ToArray();


            List<Comprobante> lst = ComprobanteBLL.GetAll(parametros, "");


            foreach (Empresa empresa in lstemp)
            {

                bool sendmailemp = false;
                string[] arraycorreoemp = enviarcorreoemp.Split(',');
                for (int i = 0; i < arraycorreoemp.Length; i++)
                {
                    if (arraycorreoemp[i] == empresa.emp_codigo.ToString())
                    {
                        sendmailemp = true;
                        break;
                    }

                }


                Correoadmin correoadm = new Correoadmin();

                correoadm.coa_empresa = empresa.emp_codigo;
                correoadm.coa_fecha = DateTime.Now;
                correoadm.coa_origen = empresa.emp_mail;
                correoadm.coa_destinatario = correoadmin;
                //correoadm.cor_adjuntos = ;
                correoadm.coa_asunto = string.Format("VERIFICACIÓN AUTOMÁTICA DE COMPROBANTES ({0:dd/MM/yyyy}-{1:dd/MM/yyyy HH:mm:ss}) EMPRESA:{2} ", dfecha, DateTime.Now, empresa.emp_nombre);


                Correoadmin correoemp = new Correoadmin();

                correoemp.coa_empresa = empresa.emp_codigo;
                correoemp.coa_fecha = DateTime.Now;
                correoemp.coa_origen = empresa.emp_mail;
                correoemp.coa_destinatario = empresa.emp_mailadm;
                correoemp.coa_asunto = string.Format("NOTIFICACIÓN AUTOMÁTICA DE COMPROBANTES ({0:dd/MM/yyyy}-{1:dd/MM/yyyy HH:mm:ss}) EMPRESA:{2} ", dfecha, DateTime.Now, empresa.emp_nombre);


                StringBuilder noaut = new StringBuilder();
                StringBuilder verif = new StringBuilder();
                StringBuilder notif = new StringBuilder();
                StringBuilder repro = new StringBuilder();
                StringBuilder anula = new StringBuilder();


                List<Comprobante> lstcomp = lst.FindAll(delegate (Comprobante c) { return c.com_empresa == empresa.emp_codigo; });

                foreach (Comprobante item in lstcomp)
                {
                    if (item.com_estado == 4)//DEVUELTO
                    {
                        ErrorAccion err = errores.Find(delegate (ErrorAccion e) { return e.error == item.com_mensaje.Trim(); });
                        if (err.accion == "verificar")
                        {
                            verif.AppendFormat("<b>Fecha:</b>{0} <b>Comprobante:</b>{1}-{2}-{3} <b>Mensaje:</b>{4}<br>", item.com_fecha, item.com_almacen, item.com_pventa, item.com_secuencia, item.com_mensaje);
                            Offline.VerifyComprobanteAsync(item);
                        }
                        if (err.accion == "notificar")
                        {
                            notif.AppendFormat("<b>Fecha:</b>{0} <b>Comprobante:</b>{1}-{2}-{3} <b>Mensaje:</b>{4}<br>", item.com_fecha, item.com_almacen, item.com_pventa, item.com_secuencia, item.com_mensaje);
                        }
                        if (err.accion == "reprocesar")
                        {
                            repro.AppendFormat("<b>Fecha:</b>{0} <b>Comprobante:</b>{1}-{2}-{3} <b>Mensaje:</b>{4}<br>", item.com_fecha, item.com_almacen, item.com_pventa, item.com_secuencia, item.com_mensaje);
                            Offline.ResetComprobanteAsync(item);
                        }
                        if (err.accion == "anular")
                        {
                            anula.AppendFormat("<b>Fecha:</b>{0} <b>Comprobante:</b>{1}-{2}-{3} <b>Mensaje:</b>{4}<br>", item.com_fecha, item.com_almacen, item.com_pventa, item.com_secuencia, item.com_mensaje);
                            Offline.EliminarComprobanteAsync(item);
                        }

                    }
                    if (item.com_estado == 6)
                    {
                        ErrorAccion err = errores.Find(delegate (ErrorAccion e) { return item.com_mensaje.Contains(e.error); });
                        if (err != null)
                        {
                            if (err.accion == "reprocesar")
                            {
                                repro.AppendFormat("<b>Fecha:</b>{0} <b>Comprobante:</b>{1}-{2}-{3} <b>Mensaje:</b>{4}<br>", item.com_fecha, item.com_almacen, item.com_pventa, item.com_secuencia, item.com_mensaje);
                                Offline.ResetComprobanteAsync(item);
                            }
                            else
                                noaut.AppendFormat("<b>Fecha:</b>{0} <b>Comprobante:</b>{1}-{2}-{3} <b>Mensaje:</b>{4}<br>", item.com_fecha, item.com_almacen, item.com_pventa, item.com_secuencia, item.com_mensaje);
                        }
                        else
                            noaut.AppendFormat("<b>Fecha:</b>{0} <b>Comprobante:</b>{1}-{2}-{3} <b>Mensaje:</b>{4}<br>", item.com_fecha, item.com_almacen, item.com_pventa, item.com_secuencia, item.com_mensaje);
                    }

                }

                StringBuilder htmladm = new StringBuilder();
                if (!string.IsNullOrEmpty(noaut.ToString()))
                {
                    htmladm.Append("<b>COMPROBANTES NO AUTORIZADOS (Notificar)</b><br>");
                    htmladm.Append(noaut.ToString());
                    htmladm.Append("<br>");
                }

                if (!string.IsNullOrEmpty(notif.ToString()))
                {
                    htmladm.Append("<b>COMPROBANTES DEVUELTOS POR ERROR XML (Notificar)</b><br>");
                    htmladm.Append(notif.ToString());
                    htmladm.Append("<br>");
                }

                if (!string.IsNullOrEmpty(verif.ToString()))
                {
                    htmladm.Append("<b>COMPROBANTES DEVUELTOS Y ENVIADOS (Verificación)</b><br>");
                    htmladm.Append(verif.ToString());
                    htmladm.Append("<br>");
                }


                if (!string.IsNullOrEmpty(repro.ToString()))
                {
                    htmladm.Append("<b>COMPROBANTES REPROCESADOS Y ENVIADOS (Reset)</b><br>");
                    htmladm.Append(repro.ToString());
                    htmladm.Append("<br>");
                }

                if (!string.IsNullOrEmpty(anula.ToString()))
                {
                    htmladm.Append("<b>COMPROBANTES ANULADOS POR DUPLICACIÓN (Anular)</b><br>");
                    htmladm.Append(anula.ToString());
                    htmladm.Append("<br>");
                }

                if (htmladm.ToString() != "")
                {

                    correoadm.coa_mensaje = htmladm.ToString();
                    correoadm.coa_automatico = 1;
                    correoadm.coa_envios = 1;
                    correoadm.coa_estado = (int)Enums.EstadoCorreo.ACTIVO;
                    correoadm.crea_fecha = DateTime.Now;
                    correoadm.crea_usr = "auto";
                    correoadm.coa_codigo = CorreoadminBLL.InsertIdentity(correoadm);
                    if (enviarcorreoadmin == "1" && mails)
                    {
                        Mail mail = new Mail();
                        mail.SendMail(correoadm);
                    }
                }


                //////////////
                StringBuilder htmlemp = new StringBuilder();
                if (!string.IsNullOrEmpty(noaut.ToString()))
                {
                    htmlemp.Append("<b>COMPROBANTES NO AUTORIZADOS (Notificar)</b><br>");
                    htmlemp.Append(noaut.ToString());
                    htmlemp.Append("<br>");
                }

                if (!string.IsNullOrEmpty(notif.ToString()))
                {
                    htmlemp.Append("<b>COMPROBANTES DEVUELTOS POR ERROR XML (Notificar)</b><br>");
                    htmlemp.Append(notif.ToString());
                    htmlemp.Append("<br>");
                }
                if (!string.IsNullOrEmpty(anula.ToString()))
                {
                    htmlemp.Append("<b>COMPROBANTES ANULADOS POR DUPLICACIÓN (Anular)</b><br>");
                    htmlemp.Append(anula.ToString());
                    htmlemp.Append("<br>");
                }

                if (htmlemp.ToString() != "")
                {

                    string saludo = string.Format("Sres. <b>{0}</b><br>", empresa.emp_nombre);
                    saludo += string.Format("A continuación se detallan comprobantes que deben ser revisados por su parte, para que puedan ser AUTORIZADOS correctamente.<br><br>");
                    string fin = "<b><br><br>Atentamente<br>SIAC<br></b>";
                    correoemp.coa_mensaje = saludo + htmlemp.ToString() + fin;
                    correoemp.coa_automatico = 1;
                    correoemp.coa_envios = 1;
                    correoemp.coa_estado = (int)Enums.EstadoCorreo.ACTIVO;
                    correoemp.crea_fecha = DateTime.Now;
                    correoemp.crea_usr = "auto";
                    correoemp.coa_codigo = CorreoadminBLL.InsertIdentity(correoemp);
                    if (sendmailemp && mails)
                    {
                        Mail mail = new Mail();
                        mail.SendMail(correoemp);
                    }
                }


            }
        }



        public static void CleanTemp()
        {
            string path = Constantes.GetParameter("pathfiles");
            string pathtemp = path + "\\temp";

            System.IO.DirectoryInfo di = new DirectoryInfo(pathtemp);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }


        public static int EnviarComprobantesProceso(int? empresa, DateTime? fecha)
        {

            WhereParams parametros = new WhereParams();
            List<object> valores = new List<object>();
            parametros.where = "com_estado IN (0,1,2)";

            if (empresa.HasValue)
            {

                parametros.where += " and com_empresa ={" + valores.Count + "}";
                valores.Add(empresa);
            }

            if (fecha.HasValue)
            {
                parametros.where += " and com_fecha>={" + valores.Count + "}";
                valores.Add(fecha);
            }
            parametros.valores = valores.ToArray();
            List<Comprobante> lst = ComprobanteBLL.GetAll(parametros, "");
            foreach (Comprobante item in lst)
            {
                Packages.Offline.CrearArchivoXML(item.com_empresa, item.com_numero);
                Offline.SendComprobanteAsync(item);

            }

            return lst.Count;

        }


        public static int VerificarComprobantes(DateTime? fecha)
        {

            WhereParams parametros = new WhereParams();
            List<object> valores = new List<object>();
            parametros.where = "com_estado IN (3)";
            if (fecha.HasValue)
            {
                parametros.where += " and com_fecha>={0}";
                valores.Add(fecha);
            }
            parametros.valores = valores.ToArray();
            List<Comprobante> lst = ComprobanteBLL.GetAll(parametros, "");
            foreach (Comprobante item in lst)
            {
                Offline.VerifyComprobanteAsync(item);

            }

            return lst.Count;

        }

        public static string GetCheckDigit(string number)
        {
            int Sum = 0;
            for (int i = number.Length - 1, Multiplier = 2; i >= 0; i--)
            {
                Sum += (int)char.GetNumericValue(number[i]) * Multiplier;

                if (++Multiplier == 8) Multiplier = 2;
            }
            string Validator = (11 - (Sum % 11)).ToString();

            if (Validator == "11") Validator = "0";
            else if (Validator == "10") Validator = "1";

            return Validator;
        }
        public static string GetClave(DateTime fecha, string ruc, string est, string pem, string num)
        {
            string clave = "";
            clave += fecha.ToString("ddMMyyyy"); //FECHA EMISION
            clave += "01"; //TIPO (01-FACTURA  04-NOTA DE CREDITO 05-NOTA DE DEBITO 06-GUIAREMISION 07-RETENCION)
            clave += ruc; //RUC DE LA EMPRESA EMISORA
            clave += "1"; //TIPO DE AMBIENTE (1 PRUEBAS   2 PRODUCCION)
            clave += est + pem;
            clave += num;
            //clave +=  comp.com_codigo.ToString("00000000");
            clave += "00000001";
            clave += "1";
            clave += GetCheckDigit(clave);

            return clave;



        }
        public static string GetXMLPrueba(int empresa, int numero, string est, string pem, string dir)
        {
            string xml = "<?xml version='1.0' encoding='utf-8'?><factura version='1.1.0' id='comprobante'><infoTributaria><ambiente>1</ambiente><tipoEmision>1</tipoEmision><razonSocial>{empresanombre}</razonSocial><nombreComercial>{empresanombre}</nombreComercial><ruc>{empresaruc}</ruc><claveAcceso>{clave}</claveAcceso><codDoc>01</codDoc><estab>{est}</estab><ptoEmi>{pem}</ptoEmi><secuencial>{sec}</secuencial><dirMatriz>{empresadir}</dirMatriz></infoTributaria><infoFactura><fechaEmision>{fecha}</fechaEmision><dirEstablecimiento>{empresadir}</dirEstablecimiento><obligadoContabilidad>NO</obligadoContabilidad><tipoIdentificacionComprador>04</tipoIdentificacionComprador><razonSocialComprador>{empresanombre}</razonSocialComprador><identificacionComprador>{empresaruc}</identificacionComprador><direccionComprador>S/D</direccionComprador><totalSinImpuestos>1.00</totalSinImpuestos><totalDescuento>0.00</totalDescuento><totalConImpuestos><totalImpuesto><codigo>2</codigo><codigoPorcentaje>0</codigoPorcentaje><baseImponible>1.00</baseImponible><valor>0.00</valor></totalImpuesto></totalConImpuestos><propina>0.00</propina><importeTotal>1.00</importeTotal><moneda>DOLAR</moneda><pagos><pago><formaPago>01</formaPago><total>1.00</total></pago></pagos></infoFactura><detalles><detalle><codigoPrincipal>001</codigoPrincipal><codigoAuxiliar>001</codigoAuxiliar><descripcion>EMISION DE PRUEBA</descripcion><cantidad>1.00</cantidad><precioUnitario>1.00</precioUnitario><descuento>0.00</descuento><precioTotalSinImpuesto>1.00</precioTotalSinImpuesto><impuestos><impuesto><codigo>2</codigo><codigoPorcentaje>0</codigoPorcentaje><tarifa>0.00</tarifa><baseImponible>1.00</baseImponible><valor>0.00</valor></impuesto></impuestos></detalle></detalles><infoAdicional><campoAdicional nombre='Email'>csanmartin@gtec.com.ec</campoAdicional></infoAdicional></factura>";

            Empresa emp = EmpresaBLL.GetByPK(new Empresa { emp_codigo = empresa, emp_codigo_key = empresa });
            DateTime fecha = DateTime.Now;
            xml = xml.Replace("{empresanombre}", emp.emp_nombre);
            xml = xml.Replace("{empresaruc}", emp.emp_ruc);
            xml = xml.Replace("{clave}", GetClave(fecha, emp.emp_ruc, est, pem, string.Format("{0:000000000}", numero)));
            xml = xml.Replace("{fecha}", fecha.ToString("dd/MM/yyyy"));
            xml = xml.Replace("{est}", est);
            xml = xml.Replace("{pem}", pem);
            xml = xml.Replace("{sec}", string.Format("{0:000000000}", numero));
            xml = xml.Replace("{empresadir}", dir);
            return xml;

        }

        public static string FormatXML(string xml)
        {
            string formatXML = Constantes.GetParameter("formatXML");
            if (formatXML == "1")
            {
                string newxml = xml.Replace("&lt;", "<").Replace("&gt;", ">");
                newxml = newxml.Replace("<comprobante><?xml", "<comprobante><![CDATA[<?xml");
                newxml = newxml.Replace("></comprobante>", ">]]></comprobante>");
                newxml = newxml.Replace("<autorizaciones>", "");
                newxml = newxml.Replace("</autorizaciones>", "");
                return newxml;
            }
            return xml;
        }

        public static Empresa GetEmpresaByRuc(string ruc)
        {
            List<Empresa> empresas = EmpresaBLL.GetAll("emp_ruc='" + ruc + "'", "");
            if (empresas.Count > 0)
                return empresas[0];
            return null;
        }


        public static string GetClaveComprobante(string ruc, string est, string pem, string sec)
        {

            int estval = int.Parse(est);
            int pemval = int.Parse(pem);
            int secval = int.Parse(sec);    
           

            Empresa emp = GetEmpresaByRuc(ruc);
            if (emp != null)
            {
                List<Comprobante> lst = ComprobanteBLL.GetAll(SetWhereClause(emp.emp_codigo, estval.ToString("000"), pemval.ToString("000"), secval.ToString("000000000"), null, null, null, null), "");
                if (lst.Count > 0)
                    return lst[0].com_numero;
            }
            return "";
        }

    }
}
