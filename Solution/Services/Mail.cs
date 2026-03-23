using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using RestSharp;
using BusinessLogicLayer;
using BusinessObjects;
using System.Web.Script.Serialization;
using System.Net.Mail;

namespace Services
{
    public class Mail
    {
        public string from { get; set; }
        public string to { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public ArrayList attachments { get; set; }

        public MailMessage myMail { get; set; }

        public Mail()
        {
            attachments = new ArrayList(); 
        }

        public bool SendMail(bool async)
        {
            string smtp = System.Configuration.ConfigurationSettings.AppSettings["smtp"].ToString();

            string mailfrom = "";
            string maildominio = "";
            string mailusuario = "";
            string mailpassword = "";
            string mailrequiereaut = "";
            string mailserver = "";
            string mailusassl = "";
            string mailport = "";




            if (smtp == "0")
            {
                mailfrom = System.Configuration.ConfigurationSettings.AppSettings["mailfrom"].ToString();
                maildominio = System.Configuration.ConfigurationSettings.AppSettings["maildominio"].ToString();
                mailusuario = System.Configuration.ConfigurationSettings.AppSettings["mailusuario"].ToString();
                mailpassword = System.Configuration.ConfigurationSettings.AppSettings["mailpassword"].ToString();
                mailrequiereaut = System.Configuration.ConfigurationSettings.AppSettings["mailrequiereaut"].ToString();
                mailserver = System.Configuration.ConfigurationSettings.AppSettings["mailserver"].ToString();
                mailusassl = System.Configuration.ConfigurationSettings.AppSettings["mailusassl"].ToString();
                mailport = System.Configuration.ConfigurationSettings.AppSettings["mailport"].ToString();
            }
            if (smtp == "1")
            {
                mailfrom = System.Configuration.ConfigurationSettings.AppSettings["mailfrom1"].ToString();
                maildominio = System.Configuration.ConfigurationSettings.AppSettings["maildominio1"].ToString();
                mailusuario = System.Configuration.ConfigurationSettings.AppSettings["mailusuario1"].ToString();
                mailpassword = System.Configuration.ConfigurationSettings.AppSettings["mailpassword1"].ToString();
                mailrequiereaut = System.Configuration.ConfigurationSettings.AppSettings["mailrequiereaut1"].ToString();
                mailserver = System.Configuration.ConfigurationSettings.AppSettings["mailserver1"].ToString();
                mailusassl = System.Configuration.ConfigurationSettings.AppSettings["mailusassl1"].ToString();
                mailport = System.Configuration.ConfigurationSettings.AppSettings["mailport1"].ToString();
            }



            bool usarssl = (mailusassl == "1") ? true : false;
            bool requiereautentificacion = (mailrequiereaut == "1") ? true : false;
            string nombreusuario = mailusuario;
            string password = mailpassword;
            string dominio = maildominio;
            
            //this.from = (string.IsNullOrEmpty(this.from)) ? mailfrom : this.from;
            this.from = mailfrom;


            //bool usarssl = (Constantes.cMailUsarSSL == "1") ? true : false;
            //bool requiereautentificacion = (Constantes.cMailRequiereAut == "1") ? true : false;
            //string nombreusuario = Constantes.cMailUsuario;
            //string password = Constantes.cMailPassword;
            //string dominio = Constantes.cMailDominio;
            //this.from = (string.IsNullOrEmpty(this.from)) ? Constantes.cMailFrom : this.from;


            //if (this.usarssl)
            //{
            //    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(RemoteServerCertificateValidationCallback);
            //}

            System.Net.Mail.MailMessage myMail = new System.Net.Mail.MailMessage();
            try
            {
                myMail.From = new System.Net.Mail.MailAddress(this.from);
                string[] splitTo = to.Split(';');
                for (int i = 0; i < splitTo.Length; i++)
                {
                    myMail.To.Add(splitTo[i]);
                }
                myMail.Subject = subject.Trim();
                myMail.Priority = System.Net.Mail.MailPriority.Normal;
                myMail.IsBodyHtml = true;
                myMail.Body = body;

                if (attachments != null)
                {
                    for (int i = 0; i < attachments.Count; i++)
                    {
                        if (attachments[i] != null)
                            myMail.Attachments.Add(new System.Net.Mail.Attachment(attachments[i].ToString()));
                    }
                }
                System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient();
                smtpClient.Host = mailserver;
                smtpClient.Port = int.Parse(mailport);

                smtpClient.EnableSsl = usarssl;

                if (requiereautentificacion && !string.IsNullOrEmpty(nombreusuario))
                {
                    if (!string.IsNullOrEmpty(dominio))
                        smtpClient.Credentials = new System.Net.NetworkCredential(nombreusuario, password, dominio);
                    else
                        smtpClient.Credentials = new System.Net.NetworkCredential(nombreusuario, password);
                }

                if (async)
                {
                    smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtpClient.Timeout = 20000;
                    smtpClient.SendCompleted += new System.Net.Mail.SendCompletedEventHandler(smtpClient_SendCompleted);
                    smtpClient.SendAsync(myMail, "");
                }
                else
                {


                    smtpClient.Send(myMail);
                }
            }
            catch (Exception ex)
            {
                for (int i = myMail.Attachments.Count - 1; i >= 0; i--)
                {
                    myMail.Attachments.RemoveAt(i);
                }
                //Servicios.Excepciones.ManejoExcepciones.IngresarExcepcion(ex);
                //error = ex.Message;
                return false;
            }
            //finally
            //{
            //    if (this.usarssl)
            //    {
            //        ServicePointManager.ServerCertificateValidationCallback = null;
            //    }
            //}
            return true;

        }



        public bool SendMail(Correo correo)
        {


            List<SmtpServer> lst = new JavaScriptSerializer().Deserialize<List<SmtpServer>>(Constantes.GetParameter("smtpserver"));

            SmtpServer smtp = lst.Find(delegate (SmtpServer s) { return s.activo == true; });
            if (smtp != null)
            {
                //System.Net.Mail.MailMessage myMail = new System.Net.Mail.MailMessage();
                myMail = new System.Net.Mail.MailMessage();
                try
                {
                    myMail.From = new System.Net.Mail.MailAddress(correo.cor_origen);
                    string[] splitTo = correo.cor_destinatario.Split(';');
                    for (int i = 0; i < splitTo.Length; i++)
                    {
                        myMail.To.Add(splitTo[i]);
                    }
                    myMail.Subject = correo.cor_asunto.Trim();
                    myMail.Priority = System.Net.Mail.MailPriority.Normal;
                    myMail.IsBodyHtml = true;
                    myMail.Body = correo.cor_mensaje;
                    string[] adjuntos = correo.cor_adjuntos.Split('|');
                    if (adjuntos != null)
                    {
                        for (int i = 0; i < adjuntos.Length; i++)
                        {
                            if (adjuntos[i] != null)
                                myMail.Attachments.Add(new System.Net.Mail.Attachment(adjuntos[i].ToString()));
                        }
                    }

                    System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient();
                    smtpClient.Host = smtp.server;
                    smtpClient.Port = smtp.puerto.Value;
                    smtpClient.EnableSsl = smtp.ssl.Value;

                    if (smtp.requiereaut.Value && !string.IsNullOrEmpty(smtp.usuario))
                    {
                        if (!string.IsNullOrEmpty(smtp.dominio))
                            smtpClient.Credentials = new System.Net.NetworkCredential(smtp.usuario, smtp.password, smtp.dominio);
                        else
                            smtpClient.Credentials = new System.Net.NetworkCredential(smtp.usuario, smtp.password);
                    }

                    if (smtp.async.Value)
                    {
                        smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                        smtpClient.Timeout = 20000;
                        smtpClient.SendCompleted += new System.Net.Mail.SendCompletedEventHandler(smtpClient_SendCompleted1);
                        



                        smtpClient.SendAsync(myMail, correo.cor_empresa + "," + correo.cor_comprobante + "," + correo.cor_codigo);
                    }
                    else
                    {


                        smtpClient.Send(myMail);
                    }

                }
                catch (Exception ex)
                {
                    for (int i = myMail.Attachments.Count - 1; i >= 0; i--)
                    {
                        myMail.Attachments.RemoveAt(i);
                    }
                    myMail.Dispose();
                    correo.cor_resultado = "ERROR:" + ex.Message;
                    correo.cor_estado = (int)Enums.EstadoCorreo.ERROR;
                    //correo.cor_fechaenvio = DateTime.Now;

                    correo.cor_empresa_key = correo.cor_empresa;
                    correo.cor_comprobante_key = correo.cor_comprobante;
                    correo.cor_codigo_key = correo.cor_codigo;                    
                    CorreoBLL.Update(correo);

                    //Servicios.Excepciones.ManejoExcepciones.IngresarExcepcion(ex);
                    //error = ex.Message;
                    return false;
                }
            }





            return true;

        }


        void smtpClient_SendCompleted1(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            myMail.Dispose();

            int estado = (int)Enums.EstadoCorreo.ENVIADO;
            string resultado = "";
            if (e.Cancelled)
            {
                resultado = "Envio correo cancelado";
                estado = (int)Enums.EstadoCorreo.CANCELADO;
                //Console.WriteLine("[{0}] Send canceled.", token);
            }
            if (e.Error != null)
            {
                estado = (int)Enums.EstadoCorreo.ERROR;
                resultado = "Envio correo con error:" + e.Error.ToString();
                //Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            }
            else
            {
                estado = (int)Enums.EstadoCorreo.ENVIADO;
                resultado = "Envio correo OK ";

            }
            String token = (string)e.UserState;

            string[] datos = token.Split(',');
            if (datos.Length > 0)
            {
                int empresa = int.Parse(datos[0]);
                string numero = datos[1].ToString();
                int codigo = int.Parse(datos[2]);
                Correo correo = CorreoBLL.GetByPK(new Correo { cor_empresa = empresa, cor_empresa_key = empresa, cor_comprobante = numero, cor_comprobante_key = numero, cor_codigo = codigo, cor_codigo_key = codigo });
                if (correo != null)
                {
                    correo.cor_resultado = resultado;
                    correo.cor_estado = estado;
                    correo.cor_fechaenvio = DateTime.Now;
                    correo.cor_empresa_key = correo.cor_empresa;
                    correo.cor_comprobante_key = correo.cor_comprobante;
                    correo.cor_codigo_key = correo.cor_codigo;
                    CorreoBLL.Update(correo);
                }


            }

            //mailSent = true;
        }




        /*
        public bool SendMailAsync()
        {
             //string externalsmtp = System.Configuration.ConfigurationSettings.AppSettings["externalsmtp"].ToString();
             //if (externalsmtp == "yes")
             //{
             //    return SendMailEx();
             //}
             //else
             //{

                 string mailfrom = System.Configuration.ConfigurationSettings.AppSettings["mailfrom"].ToString();
                 string maildominio = System.Configuration.ConfigurationSettings.AppSettings["maildominio"].ToString();
                 string mailusuario = System.Configuration.ConfigurationSettings.AppSettings["mailusuario"].ToString();
                 string mailpassword = System.Configuration.ConfigurationSettings.AppSettings["mailpassword"].ToString();
                 string mailrequiereaut = System.Configuration.ConfigurationSettings.AppSettings["mailrequiereaut"].ToString();
                 string mailserver = System.Configuration.ConfigurationSettings.AppSettings["mailserver"].ToString();
                 string mailusassl = System.Configuration.ConfigurationSettings.AppSettings["mailusassl"].ToString();
                 string mailport = System.Configuration.ConfigurationSettings.AppSettings["mailport"].ToString();



                 bool usarssl = (mailusassl == "1") ? true : false;
                 bool requiereautentificacion = (mailrequiereaut == "1") ? true : false;
                 string nombreusuario = mailusuario;
                 string password = mailpassword;
                 string dominio = maildominio;
                 this.from = (string.IsNullOrEmpty(this.from)) ? mailfrom : this.from;

                 //bool usarssl = (Constantes.cMailUsarSSL == "1") ? true : false;
                 //bool requiereautentificacion = (Constantes.cMailRequiereAut == "1") ? true : false;
                 //string nombreusuario = Constantes.cMailUsuario;
                 //string password = Constantes.cMailPassword;
                 //string dominio = Constantes.cMailDominio;
                 //this.from = (string.IsNullOrEmpty(this.from)) ? Constantes.cMailFrom : this.from;


                 //if (this.usarssl)
                 //{
                 //    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(RemoteServerCertificateValidationCallback);
                 //}

                 System.Net.Mail.MailMessage myMail = new System.Net.Mail.MailMessage();
                 try
                 {
                     myMail.From = new System.Net.Mail.MailAddress(this.from);
                     string[] splitTo = to.Split(';');
                     for (int i = 0; i < splitTo.Length; i++)
                     {
                         myMail.To.Add(splitTo[i]);
                     }
                     myMail.Subject = subject.Trim();
                     myMail.Priority = System.Net.Mail.MailPriority.Normal;
                     myMail.IsBodyHtml = true;
                     myMail.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");
                     myMail.Body = body;

                     if (attachments != null)
                     {
                         for (int i = 0; i < attachments.Count; i++)
                         {
                             if (attachments[i] != null)
                             {
                                 if (File.Exists(attachments[i].ToString()))
                                     myMail.Attachments.Add(new System.Net.Mail.Attachment(attachments[i].ToString()));
                             }
                         }
                     }
                     System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient();
                     smtpClient.Host = mailserver;//Constantes.cMailServer;
                     smtpClient.Port = int.Parse(mailport);

                     smtpClient.EnableSsl = usarssl;
                     smtpClient.UseDefaultCredentials = true;
                     if (requiereautentificacion && !string.IsNullOrEmpty(nombreusuario))
                     {
                         if (!string.IsNullOrEmpty(dominio))
                             smtpClient.Credentials = new System.Net.NetworkCredential(nombreusuario, password, dominio);
                         else
                             smtpClient.Credentials = new System.Net.NetworkCredential(nombreusuario, password);

                     }
                     smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                     smtpClient.Timeout = 20000;
                     smtpClient.SendCompleted += new System.Net.Mail.SendCompletedEventHandler(smtpClient_SendCompleted);
                     smtpClient.SendAsync(myMail, "");
                 }
                 catch (Exception ex)
                 {
                     for (int i = myMail.Attachments.Count - 1; i >= 0; i--)
                     {
                         myMail.Attachments.RemoveAt(i);
                     }
                     //Servicios.Excepciones.ManejoExcepciones.IngresarExcepcion(ex);
                     //error = ex.Message;
                     return false;
                 }
                 //finally
                 //{
                 //    if (this.usarssl)
                 //    {
                 //        ServicePointManager.ServerCertificateValidationCallback = null;
                 //    }
                 //}
                 return true;
             //}
        }
        */
        void smtpClient_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            String token = (string)e.UserState;

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Send canceled.", token);
            }
            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            }
            else
            {
                Console.WriteLine("Message sent.");
            }
            //mailSent = true;
        }


        /*ENVIO DE CORREOS ADMINISTRATIVO*/



        public bool SendMail(Correoadmin correo)
        {


            List<SmtpServer> lst = new JavaScriptSerializer().Deserialize<List<SmtpServer>>(Constantes.GetParameter("smtpserver"));

            SmtpServer smtp = lst.Find(delegate (SmtpServer s) { return s.activo == true; });
            if (smtp != null)
            {
                //System.Net.Mail.MailMessage myMail = new System.Net.Mail.MailMessage();
                myMail = new System.Net.Mail.MailMessage();
                try
                {
                    myMail.From = new System.Net.Mail.MailAddress(correo.coa_origen);
                    string[] splitTo = correo.coa_destinatario.Split(';');
                    for (int i = 0; i < splitTo.Length; i++)
                    {
                        myMail.To.Add(splitTo[i]);
                    }
                    myMail.Subject = correo.coa_asunto.Trim();
                    myMail.Priority = System.Net.Mail.MailPriority.Normal;
                    myMail.IsBodyHtml = true;
                    myMail.Body = correo.coa_mensaje;
                    if (!string.IsNullOrEmpty(correo.coa_adjuntos))
                    {
                        string[] adjuntos = correo.coa_adjuntos.Split('|');
                        if (adjuntos != null)
                        {
                            for (int i = 0; i < adjuntos.Length; i++)
                            {
                                if (adjuntos[i] != null)
                                    myMail.Attachments.Add(new System.Net.Mail.Attachment(adjuntos[i].ToString()));
                            }
                        }
                    }
                    System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient();
                    smtpClient.Host = smtp.server;
                    smtpClient.Port = smtp.puerto.Value;
                    smtpClient.EnableSsl = smtp.ssl.Value;

                    if (smtp.requiereaut.Value && !string.IsNullOrEmpty(smtp.usuario))
                    {
                        if (!string.IsNullOrEmpty(smtp.dominio))
                            smtpClient.Credentials = new System.Net.NetworkCredential(smtp.usuario, smtp.password, smtp.dominio);
                        else
                            smtpClient.Credentials = new System.Net.NetworkCredential(smtp.usuario, smtp.password);
                    }

                    if (smtp.async.Value)
                    {
                        smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                        smtpClient.Timeout = 20000;
                        smtpClient.SendCompleted += new System.Net.Mail.SendCompletedEventHandler(smtpClient_SendCompleted2);




                        smtpClient.SendAsync(myMail, correo.coa_empresa + "," + correo.coa_codigo);
                    }
                    else
                    {


                        smtpClient.Send(myMail);
                    }

                }
                catch (Exception ex)
                {
                    for (int i = myMail.Attachments.Count - 1; i >= 0; i--)
                    {
                        myMail.Attachments.RemoveAt(i);
                    }
                    myMail.Dispose();
                    correo.coa_resultado = "ERROR:" + ex.Message;
                    correo.coa_estado = (int)Enums.EstadoCorreo.ERROR;
                    //correo.cor_fechaenvio = DateTime.Now;

                    correo.coa_empresa_key = correo.coa_empresa;
                    correo.coa_codigo_key = correo.coa_codigo;
                    CorreoadminBLL.Update(correo);

                    //Servicios.Excepciones.ManejoExcepciones.IngresarExcepcion(ex);
                    //error = ex.Message;
                    return false;
                }
            }





            return true;

        }


        void smtpClient_SendCompleted2(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            myMail.Dispose();

            int estado = (int)Enums.EstadoCorreo.ENVIADO;
            string resultado = "";
            if (e.Cancelled)
            {
                resultado = "Envio correo cancelado";
                estado = (int)Enums.EstadoCorreo.CANCELADO;
                //Console.WriteLine("[{0}] Send canceled.", token);
            }
            if (e.Error != null)
            {
                estado = (int)Enums.EstadoCorreo.ERROR;
                resultado = "Envio correo con error:" + e.Error.ToString();
                //Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            }
            else
            {
                estado = (int)Enums.EstadoCorreo.ENVIADO;
                resultado = "Envio correo OK ";

            }
            String token = (string)e.UserState;

            string[] datos = token.Split(',');
            if (datos.Length > 0)
            {
                int empresa = int.Parse(datos[0]);
                int codigo = int.Parse(datos[1]);
                Correoadmin correo = CorreoadminBLL.GetByPK(new Correoadmin { coa_empresa = empresa, coa_empresa_key = empresa, coa_codigo = codigo, coa_codigo_key = codigo });
                if (correo != null)
                {
                    correo.coa_resultado = resultado;
                    correo.coa_estado = estado;
                    correo.coa_fechaenvio = DateTime.Now;
                    correo.coa_empresa_key = correo.coa_empresa;
                    correo.coa_codigo_key = correo.coa_codigo;
                    CorreoadminBLL.Update(correo);
                }


            }

            //mailSent = true;
        }



    }
}
