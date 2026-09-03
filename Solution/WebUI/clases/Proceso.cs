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
//SERVICIOS ONLINE PRUEBAS
using WebUI.ec.gob.sri.celcer;
using WebUI.ec.gob.sri.celcer1;
//SERVICIOS ONLINE PRODUCCION
using WebUI.ec.gob.sri.cel;
using WebUI.ec.gob.sri.cel1;
//SERVICIOS OFFLINE PRUBEAS
using WebUI.ec.gob.sri.celcerOff;
//usingWebUI.ec.gob.sri.celcerOff1;
//SERVICIOS OFFLINE PRODUCCION
using WebUI.ec.gob.sri.celOff;
using WebUI.ec.gob.sri.cel1Off;

using System.Xml;
using System.IO;
using Services;
using Functions;

namespace WebUI
{
    public class Proceso
    {

        public static string CreaUsuarios()
        {
            List<Comprobante> comprobantes = ComprobanteBLL.GetAll("", "");
            List<Archivo> archivos = ArchivoBLL.GetAll("", "");
            foreach (Comprobante item in comprobantes)
            {
                Archivo archivo = archivos.Find(delegate(Archivo  a) { return a.arc_numero == item.com_numero; });
                SaveUsuario(archivo.arc_xml, item.com_email);
                
            }
            return "ok";
        }

        public static string SaveUsuario(string xml, string mail)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(xml);

            XmlNode xmlruccli = null;
            XmlNode xmlcli = null;
            XmlNode infoadicional = null;
            XmlNode destinatarios = null;

            var tipo = xmldoc.SelectSingleNode("/").LastChild.Name;
            if (tipo.ToUpper() == "FACTURA")
            {

                xmlruccli = xmldoc.SelectSingleNode("/factura/infoFactura/identificacionComprador");
                xmlcli = xmldoc.SelectSingleNode("/factura/infoFactura/razonSocialComprador");
                infoadicional = xmldoc.SelectSingleNode("/factura/infoAdicional");
            }
            if (tipo.ToUpper() == "NOTACREDITO")
            {
                xmlruccli = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/identificacionComprador");
                xmlcli = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/razonSocialComprador");
                infoadicional = xmldoc.SelectSingleNode("/notaCredito/infoAdicional");
            }
            if (tipo.ToUpper() == "NOTADEBITO")
            {
                xmlruccli = xmldoc.SelectSingleNode("/notaDebito/infoNotaDebito/identificacionComprador");
                xmlcli = xmldoc.SelectSingleNode("/notaDebito/infoNotaDebito/razonSocialComprador");
                infoadicional = xmldoc.SelectSingleNode("/notaDebito/infoAdicional");
            }
            if (tipo.ToUpper() == "COMPROBANTERETENCION")
            {
                xmlruccli = xmldoc.SelectSingleNode("/comprobanteRetencion/infoCompRetencion/identificacionSujetoRetenido");
                xmlcli = xmldoc.SelectSingleNode("/comprobanteRetencion/infoCompRetencion/razonSocialSujetoRetenido");
                infoadicional = xmldoc.SelectSingleNode("/comprobanteRetencion/infoAdicional");
            }
            if (tipo.ToUpper() == "GUIAREMISION")
            {
                destinatarios = xmldoc.SelectSingleNode("guiaRemision/destinatarios");
                foreach (XmlNode destin in destinatarios.ChildNodes)
                {
                    xmlruccli = destin.SelectSingleNode("identificacionDestinatario");
                    xmlcli = destin.SelectSingleNode("razonSocialDestinatario");
                    break;
                }
                infoadicional = xmldoc.SelectSingleNode("/guiaRemision/infoAdicional");
            }

            if (tipo.ToUpper() == "LIQUIDACIONCOMPRA")
            {
                xmlruccli = xmldoc.SelectSingleNode("/liquidacionCompra/infoLiquidacionCompra/identificacionProveedor");
                xmlcli = xmldoc.SelectSingleNode("/liquidacionCompra/infoLiquidacionCompra/razonSocialProveedor");
                infoadicional = xmldoc.SelectSingleNode("/liquidacionCompra/infoAdicional");
            }

            string password = "";
            if (infoadicional != null)
            {
                foreach (XmlNode iteminfo in infoadicional.ChildNodes)
                {
                    if (iteminfo.Attributes["nombre"].Value == "Clavecliente")
                        password = iteminfo.InnerText;
                }
            }

            Usuario usuario = UsuarioBLL.GetByPK(new Usuario { usr_id = xmlruccli.InnerText.Trim(), usr_id_key = xmlruccli.InnerText.Trim() });
            if (usuario.crea_fecha.HasValue)//USUARIO EXISTE
            {
                usuario.usr_id = xmlruccli.InnerText.Trim();
                usuario.usr_id_key = xmlruccli.InnerText.Trim();
                usuario.usr_mail = mail;
                usuario.mod_fecha = DateTime.Now;
                usuario.mod_usr = "admin";
                UsuarioBLL.Update(usuario);
            }
            else//USUARIO NUEVO
            {
                if (password != "")
                {
                    usuario.usr_id = xmlruccli.InnerText.Trim();
                    usuario.usr_nombres = xmlcli.InnerText;
                    usuario.usr_password = password;
                    usuario.usr_perfil = "user";
                    usuario.usr_mail = mail;
                    usuario.usr_estado = 1;
                    usuario.crea_fecha = DateTime.Now;
                    usuario.crea_usr = "admin";
                    UsuarioBLL.Insert(usuario);
                }
            }
            return "saved";




       }

        public static Comprobante SetValores(XmlDocument xmldoc, Comprobante comprobante, string version)
        {
            decimal subtotal12 = 0;
            decimal valor12 = 0;
            decimal subtotalice = 0;
            decimal valorice = 0;
            decimal subtotal0 = 0;

            decimal totalsinimp = 0;
            decimal totaldesc = 0;
            decimal totalcomp = 0;
            decimal propina = 0;

            decimal valorretenido = 0;


            XmlNode totalimpuestos = null;
            XmlNode totaldescuento = null;
            XmlNode totalsinimpuestos = null;
            XmlNode importetotal = null;
            XmlNode xmlpropina = null;
            XmlNode retencion = null;

            var tipo = xmldoc.SelectSingleNode("/").LastChild.Name;
            if (tipo.ToUpper() == "FACTURA")
            {
                totalimpuestos = xmldoc.SelectSingleNode("/factura/infoFactura/totalConImpuestos");
                totaldescuento = xmldoc.SelectSingleNode("/factura/infoFactura/totalDescuento");
                totalsinimpuestos = xmldoc.SelectSingleNode("/factura/infoFactura/totalSinImpuestos");
                importetotal = xmldoc.SelectSingleNode("/factura/infoFactura/importeTotal");
                xmlpropina = xmldoc.SelectSingleNode("/factura/infoFactura/propina");
            }
            if (tipo.ToUpper() == "NOTACREDITO")
            {
                totalimpuestos = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/totalConImpuestos");
                //totaldescuento = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/totalDescuento");
                totalsinimpuestos = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/totalSinImpuestos");
                importetotal = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/valorModificacion");
                //xmlpropina = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/propina");
            }
            if (tipo.ToUpper() == "NOTADEBITO")
            {
                //totalimpuestos = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/totalConImpuestos");
                //totaldescuento = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/totalDescuento");
                totalsinimpuestos = xmldoc.SelectSingleNode("/notaDebito/infoNotaDebito/totalSinImpuestos");
                importetotal = xmldoc.SelectSingleNode("/notaDebito/infoNotaDebito/valorTotal");
                //xmlpropina = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/propina");
            }
            if (tipo.ToUpper() == "COMPROBANTERETENCION")
            {
                if (version == "2.0.0")
                {
                    XmlNode docsSustento = xmldoc.SelectSingleNode("/comprobanteRetencion/docsSustento");
                    //CALCULO DE TOTAL DE LA RETENCION
                    foreach (XmlNode docsus in docsSustento.ChildNodes)
                    {
                        retencion = docsus.SelectSingleNode("retenciones");
                        foreach (XmlNode imp in retencion.ChildNodes)
                        {
                            XmlNode valorret = imp.SelectSingleNode("valorRetenido");
                            valorretenido += Conversiones.GetDecimal(valorret.InnerText);
                        }                        
                    }
                }
                else
                {
                    retencion = xmldoc.SelectSingleNode("/comprobanteRetencion/impuestos");
                    //CALCULO DE TOTAL DE LA RETENCION

                    foreach (XmlNode imp in retencion.ChildNodes)
                    {
                        XmlNode valorret = imp.SelectSingleNode("valorRetenido");
                        valorretenido += Conversiones.GetDecimal(valorret.InnerText);
                    }
                }
            }

            if (tipo.ToUpper() == "LIQUIDACIONCOMPRA")
            {
                totalimpuestos = xmldoc.SelectSingleNode("/liquidacionCompra/infoLiquidacionCompra/totalConImpuestos");
                totaldescuento = xmldoc.SelectSingleNode("/liquidacionCompra/infoLiquidacionCompra/totalDescuento");
                totalsinimpuestos = xmldoc.SelectSingleNode("/liquidacionCompra/infoLiquidacionCompra/totalSinImpuestos");
                importetotal = xmldoc.SelectSingleNode("/liquidacionCompra/infoLiquidacionCompra/importeTotal");                
            }


            if (totalimpuestos != null)
            {

                foreach (XmlNode totalimp in totalimpuestos.ChildNodes)
                {
                    XmlNode codigo = totalimp.SelectSingleNode("codigo");
                    XmlNode codigoporcentaje = totalimp.SelectSingleNode("codigoPorcentaje");
                    XmlNode descuento = totalimp.SelectSingleNode("descuentoAdicional");
                    XmlNode baseimp = totalimp.SelectSingleNode("baseImponible");
                    XmlNode tarifa = totalimp.SelectSingleNode("tarifa");
                    XmlNode valor = totalimp.SelectSingleNode("valor");

                    if (codigo.InnerText == "2" && codigoporcentaje.InnerText == "2")//TARIFA 12
                    {
                        subtotal12 += Conversiones.GetDecimal(baseimp.InnerText);
                        valor12 += Conversiones.GetDecimal(valor.InnerText);
                    }
                    if (codigo.InnerText == "2" && codigoporcentaje.InnerText == "0")//TARIFA 0
                    {
                        subtotal0 += Conversiones.GetDecimal(baseimp.InnerText);
                    }
                    if (codigo.InnerText == "3")//TARIFA ICE
                    {
                        subtotalice += Conversiones.GetDecimal(baseimp.InnerText);
                        valorice += Conversiones.GetDecimal(valor.InnerText);
                    }
                }
            }

            if (totaldescuento!=null)                  
                totaldesc = Conversiones.GetDecimal(totaldescuento.InnerText);

            if (totalsinimpuestos != null)
                totalsinimp = Conversiones.GetDecimal(totalsinimpuestos.InnerText);

            //CAMBIO SOLICITADO POR SANTI
            subtotal12 = totalsinimp + totaldesc - subtotal0;

            decimal subtotal = subtotal12 + subtotal0;

            if (importetotal!=null)            
                totalcomp = Conversiones.GetDecimal(importetotal.InnerText);

            if (xmlpropina!=null)   
                propina = Conversiones.GetDecimal(xmlpropina.InnerText);

            //TOTAL DEL VALORE RETENIDO
            if (valorretenido > 0)
                totalcomp = valorretenido;



            comprobante.com_subtotalnoimp = subtotal0;
            comprobante.com_subtotalimp = subtotal12;
            comprobante.com_total = totalcomp;
            comprobante.com_propina = propina;
            comprobante.com_descuento = totaldesc;
            comprobante.com_iva = valor12;
            comprobante.com_ice = valorice;
            return comprobante;
        }

        public static string ResetValores(int empresa, string clave)
        {
            Comprobante comprobante = ComprobanteBLL.GetByPK(new Comprobante { com_numero = clave, com_numero_key = clave, com_empresa = empresa, com_empresa_key = empresa });
            Archivo archivo = ArchivoBLL.GetByPK(new Archivo { arc_numero = clave, arc_numero_key = clave, arc_empresa = empresa, arc_empresa_key = empresa }); 
              XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(archivo.arc_xml);
            var version = xmldoc.SelectSingleNode("/").LastChild.Attributes["version"].Value;
            comprobante = SetValores(xmldoc, comprobante,version);
            comprobante.com_numero_key = comprobante.com_numero;
            comprobante.com_empresa_key = comprobante.com_empresa;
            ComprobanteBLL.Update(comprobante);
            return "recalculado";
        }
        
        public static string RecibirComprobante(string xml, string mail, int formato)
        {
            string retorno = "";
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(xml);

            Packages.General.AsegurarCampoAdicionalRucProveedor(xmldoc);
            xml = Packages.General.SerializarXml(xmldoc);

            XmlNode xmlcla = null;
            XmlNode xmlruc=null;
            XmlNode xmlalm = null;
            XmlNode xmlpve = null;
            XmlNode xmlsec = null;
            XmlNode xmlruccli = null;
            XmlNode xmlcli = null;
            XmlNode xmlfec = null;

            XmlNode xmlamb = null;
            XmlNode xmlemi = null;



            var tipo = xmldoc.SelectSingleNode("/").LastChild.Name;
            var version = xmldoc.SelectSingleNode("/").LastChild.Attributes["version"].Value;

            if (tipo.ToUpper() == "FACTURA")
            {


                xmlcla = xmldoc.SelectSingleNode("/factura/infoTributaria/claveAcceso");
                xmlruc = xmldoc.SelectSingleNode("/factura/infoTributaria/ruc");
                xmlalm = xmldoc.SelectSingleNode("/factura/infoTributaria/estab");
                xmlpve = xmldoc.SelectSingleNode("/factura/infoTributaria/ptoEmi");
                xmlsec = xmldoc.SelectSingleNode("/factura/infoTributaria/secuencial");
                xmlruccli = xmldoc.SelectSingleNode("/factura/infoFactura/identificacionComprador");
                xmlcli = xmldoc.SelectSingleNode("/factura/infoFactura/razonSocialComprador");
                xmlfec = xmldoc.SelectSingleNode("/factura/infoFactura/fechaEmision");

                xmlamb = xmldoc.SelectSingleNode("/factura/infoTributaria/ambiente");
                xmlemi = xmldoc.SelectSingleNode("/factura/infoTributaria/tipoEmision");

            }
            if (tipo.ToUpper()=="NOTACREDITO")
            {
                xmlcla = xmldoc.SelectSingleNode("/notaCredito/infoTributaria/claveAcceso");
                xmlruc = xmldoc.SelectSingleNode("/notaCredito/infoTributaria/ruc");
                xmlalm = xmldoc.SelectSingleNode("/notaCredito/infoTributaria/estab");
                xmlpve = xmldoc.SelectSingleNode("/notaCredito/infoTributaria/ptoEmi");
                xmlsec = xmldoc.SelectSingleNode("/notaCredito/infoTributaria/secuencial");
                xmlruccli = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/identificacionComprador");
                xmlcli = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/razonSocialComprador");
                xmlfec = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/fechaEmision");

                xmlamb = xmldoc.SelectSingleNode("/notaCredito/infoTributaria/ambiente");
                xmlemi = xmldoc.SelectSingleNode("/notaCredito/infoTributaria/tipoEmision");
            }
            if (tipo.ToUpper() == "NOTADEBITO")
            {
                xmlcla = xmldoc.SelectSingleNode("/notaDebito/infoTributaria/claveAcceso");
                xmlruc = xmldoc.SelectSingleNode("/notaDebito/infoTributaria/ruc");
                xmlalm = xmldoc.SelectSingleNode("/notaDebito/infoTributaria/estab");
                xmlpve = xmldoc.SelectSingleNode("/notaDebito/infoTributaria/ptoEmi");
                xmlsec = xmldoc.SelectSingleNode("/notaDebito/infoTributaria/secuencial");
                xmlruccli = xmldoc.SelectSingleNode("/notaDebito/infoNotaDebito/identificacionComprador");
                xmlcli = xmldoc.SelectSingleNode("/notaDebito/infoNotaDebito/razonSocialComprador");
                xmlfec = xmldoc.SelectSingleNode("/notaDebito/infoNotaDebito/fechaEmision");

                xmlamb = xmldoc.SelectSingleNode("/notaDebito/infoTributaria/ambiente");
                xmlemi = xmldoc.SelectSingleNode("/notaDebito/infoTributaria/tipoEmision");
            }
            if (tipo.ToUpper()=="COMPROBANTERETENCION")
            {
                xmlcla = xmldoc.SelectSingleNode("/comprobanteRetencion/infoTributaria/claveAcceso");
                xmlruc = xmldoc.SelectSingleNode("/comprobanteRetencion/infoTributaria/ruc");
                xmlalm = xmldoc.SelectSingleNode("/comprobanteRetencion/infoTributaria/estab");
                xmlpve = xmldoc.SelectSingleNode("/comprobanteRetencion/infoTributaria/ptoEmi");
                xmlsec = xmldoc.SelectSingleNode("/comprobanteRetencion/infoTributaria/secuencial");

                xmlruccli = xmldoc.SelectSingleNode("/comprobanteRetencion/infoCompRetencion/identificacionSujetoRetenido");
                xmlcli = xmldoc.SelectSingleNode("/comprobanteRetencion/infoCompRetencion/razonSocialSujetoRetenido");
                xmlfec = xmldoc.SelectSingleNode("/comprobanteRetencion/infoCompRetencion/fechaEmision");

                xmlamb = xmldoc.SelectSingleNode("/comprobanteRetencion/infoTributaria/ambiente");
                xmlemi = xmldoc.SelectSingleNode("/comprobanteRetencion/infoTributaria/tipoEmision");
            }
            if (tipo.ToUpper() == "GUIAREMISION")
            {
                xmlcla = xmldoc.SelectSingleNode("/guiaRemision/infoTributaria/claveAcceso");
                xmlruc = xmldoc.SelectSingleNode("/guiaRemision/infoTributaria/ruc");
                xmlalm = xmldoc.SelectSingleNode("/guiaRemision/infoTributaria/estab");
                xmlpve = xmldoc.SelectSingleNode("/guiaRemision/infoTributaria/ptoEmi");
                xmlsec = xmldoc.SelectSingleNode("/guiaRemision/infoTributaria/secuencial");


                XmlNode destinatarios = xmldoc.SelectSingleNode("guiaRemision/destinatarios");
                foreach (XmlNode destin in destinatarios.ChildNodes)
                {
                    xmlruccli = destin.SelectSingleNode("identificacionDestinatario");
                    xmlcli = destin.SelectSingleNode("razonSocialDestinatario");
                    break;
                }                
                xmlfec = xmldoc.SelectSingleNode("/guiaRemision/infoGuiaRemision/fechaIniTransporte");

                xmlamb = xmldoc.SelectSingleNode("/guiaRemision/infoTributaria/ambiente");
                xmlemi = xmldoc.SelectSingleNode("/guiaRemision/infoTributaria/tipoEmision");
            }

            if (tipo.ToUpper() == "LIQUIDACIONCOMPRA")
            {
                xmlcla = xmldoc.SelectSingleNode("/liquidacionCompra/infoTributaria/claveAcceso");
                xmlruc = xmldoc.SelectSingleNode("/liquidacionCompra/infoTributaria/ruc");
                xmlalm = xmldoc.SelectSingleNode("/liquidacionCompra/infoTributaria/estab");
                xmlpve = xmldoc.SelectSingleNode("/liquidacionCompra/infoTributaria/ptoEmi");
                xmlsec = xmldoc.SelectSingleNode("/liquidacionCompra/infoTributaria/secuencial");

                xmlruccli = xmldoc.SelectSingleNode("/liquidacionCompra/infoLiquidacionCompra/identificacionProveedor");
                xmlcli = xmldoc.SelectSingleNode("/liquidacionCompra/infoLiquidacionCompra/razonSocialProveedor");
                xmlfec = xmldoc.SelectSingleNode("/liquidacionCompra/infoLiquidacionCompra/fechaEmision");

                xmlamb = xmldoc.SelectSingleNode("/liquidacionCompra/infoTributaria/ambiente");
                xmlemi = xmldoc.SelectSingleNode("/liquidacionCompra/infoTributaria/tipoEmision");



            }





            if (xmlruc != null)
            {
                string ruc = xmlruc.InnerText;
                List<Empresa> lst = EmpresaBLL.GetAll(new WhereParams("emp_ruc = {0} and emp_estado=1", ruc), "");
                if (lst.Count > 0)
                {
                    SaveUsuario(xml, mail);
                    bool insert = false;
                    bool update = false;
                    Archivo archivo = new Archivo();
                    Comprobante comprobante = ComprobanteBLL.GetByPK(new Comprobante { com_numero = xmlcla.InnerText, com_numero_key = xmlcla.InnerText, com_empresa = lst[0].emp_codigo, com_empresa_key = lst[0].emp_codigo });
                    if (comprobante.com_almacen == null)
                    {
                        insert = true;
                        comprobante = new Comprobante();
                        comprobante.com_empresa = lst[0].emp_codigo;
                        comprobante.com_numero = xmlcla.InnerText;
                        comprobante.com_almacen = xmlalm.InnerText;
                        comprobante.com_pventa = xmlpve.InnerText;
                        comprobante.com_secuencia = xmlsec.InnerText;
                        comprobante.com_ruccliente = xmlruccli.InnerText;
                        comprobante.com_nombrecliente = xmlcli.InnerText;
                        comprobante.com_fecha = DateTime.Parse(xmlfec.InnerText);
                        comprobante.com_fecharecibe = DateTime.Now;
                        comprobante.com_email = mail;
                        comprobante.com_formato = formato;
                        comprobante.com_ambiente = int.Parse(xmlamb.InnerText);
                        comprobante.com_emision = int.Parse(xmlemi.InnerText);
                        comprobante.com_estado = (int)Enums.EstadoComprobante.Proceso;
                        comprobante.crea_usr = "admin";
                        comprobante.crea_fecha = DateTime.Now;

                        archivo = new Archivo();
                        archivo.arc_empresa = comprobante.com_empresa;
                        archivo.arc_numero = comprobante.com_numero;
                        archivo.arc_xml = xml;
                        archivo.arc_estado = (int)Enums.EstadoComprobante.Proceso;
                        archivo.crea_usr = "admin";
                        archivo.crea_fecha = DateTime.Now;
                    }
                    else
                    {

                        if (comprobante.com_estado != (int)Enums.EstadoComprobante.Autorizado)// || comprobante.com_estado == (int)Enums.EstadoComprobante.Devuelto || comprobante.com_estado == (int)Enums.EstadoComprobante.NoAutorizado)
                        {
                            update = true;
                            comprobante.com_numero_key = comprobante.com_numero;
                            comprobante.com_empresa_key = comprobante.com_empresa;
                            comprobante.com_empresa = lst[0].emp_codigo;
                            comprobante.com_numero = xmlcla.InnerText;
                            comprobante.com_almacen = xmlalm.InnerText;
                            comprobante.com_pventa = xmlpve.InnerText;
                            comprobante.com_secuencia = xmlsec.InnerText;
                            comprobante.com_ruccliente = xmlruccli.InnerText;
                            comprobante.com_nombrecliente = xmlcli.InnerText;
                            comprobante.com_fecha = DateTime.Parse(xmlfec.InnerText);
                            comprobante.com_fecharecibe = DateTime.Now;
                            comprobante.com_ambiente = int.Parse(xmlamb.InnerText);
                            comprobante.com_emision = int.Parse(xmlemi.InnerText);
                            comprobante.com_estado = (int)Enums.EstadoComprobante.Proceso;
                            comprobante.com_email = mail;
                            comprobante.com_formato = formato;
                            comprobante.mod_usr = "admin";
                            comprobante.mod_fecha = DateTime.Now;

                            archivo = ArchivoBLL.GetByPK(new Archivo { arc_empresa = comprobante.com_empresa, arc_empresa_key = comprobante.com_empresa, arc_numero = comprobante.com_numero, arc_numero_key = comprobante.com_numero });
                            archivo.arc_empresa_key = archivo.arc_empresa;
                            archivo.arc_numero_key = archivo.arc_numero;
                            archivo.arc_xml = xml;
                            archivo.arc_estado = (int)Enums.EstadoComprobante.Proceso;
                            archivo.mod_usr = "admin";
                            archivo.mod_fecha = DateTime.Now;
                        }
                        else
                        {
                            retorno = "El comprobante ya esta en proceso";
                        }
                    }
                    comprobante = SetValores(xmldoc, comprobante, version);

                    if (insert || update)
                    {
                        
                        /*
                        string xmlpath = HttpContext.Current.Server.MapPath("../xml/" + comprobante.com_numero + ".xml");
                        if (File.Exists(xmlpath))
                            File.Delete(xmlpath);
                        // Create a file to write to. 
                        using (StreamWriter sw = File.CreateText(xmlpath))
                        {
                            sw.Write(xml);
                            sw.Flush();
                            sw.Close();
                        }
                        comprobante.com_xml = xmlpath;
                        */


                        BLL transaction = new BLL();
                        transaction.CreateTransaction();

                        try
                        {
                            transaction.BeginTransaction();
                            if (insert)
                            {
                                ComprobanteBLL.Insert(transaction, comprobante);
                                ArchivoBLL.Insert(transaction, archivo);
                            }
                            if (update)
                            {
                                ComprobanteBLL.Update(transaction, comprobante);
                                ArchivoBLL.Update(transaction, archivo);
                            }

                            transaction.Commit();
                            //EnviarComprobanteAsync(comprobante.com_empresa, comprobante.com_numero);
                            //EnviarComprobante(comprobante.com_empresa, comprobante.com_numero);
                            //NUEVO PROCESO OFFLINE
                            Packages.Offline.CrearArchivoXML(comprobante.com_empresa, comprobante.com_numero);
                            Packages.Offline.SendComprobanteAsync(comprobante);

                            retorno = "OK";
                            //return codigo.ToString();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            retorno = "ERROR " + ex.Message;
                        }
                    }                    

                }
                else
                    retorno = "No existe empresa";
            }
            else
                retorno = "Xml error sin ruc";
            return retorno;

        }

        public static void FirmarComprobante(int empresa, string clave)
        {
            Comprobante comprobante = ComprobanteBLL.GetByPK(new Comprobante { com_numero = clave, com_numero_key = clave, com_empresa = empresa, com_empresa_key = empresa });
            if (comprobante.com_estado < (int)Enums.EstadoComprobante.Firmado)
            {
                Empresa emp = EmpresaBLL.GetByPK(new Empresa { emp_codigo = empresa, emp_codigo_key = empresa });
                //Firma.signxml(comprobante.com_xml, clave, emp.emp_certificado, emp.emp_password);
                List<Certificado> certificados = CertificadoBLL.GetAll(new WhereParams("cer_empresa={0} and  {1} between cer_desde and cer_hasta and cer_estado = {2}", emp.emp_codigo, comprobante.com_fecha, 1), "");
                if (certificados.Count > 0)
                {

                    Firma.signxml(comprobante.com_xml, clave, certificados[0].cer_nombre, certificados[0].cer_password, certificados[0].cer_path);
                    comprobante.com_estado = (int)Enums.EstadoComprobante.Firmado; //enviada
                    comprobante.com_numero_key = comprobante.com_numero;
                    comprobante.com_empresa_key = comprobante.com_empresa;
                    ComprobanteBLL.Update(comprobante);
                }
                else
                    throw new ArgumentException("No existe certificado para el comprobante en la fecha " + comprobante.com_fecha.ToString());
            }
            
        }

        public static string ReenviarComprobante(int empresa, string clave)
        {
            Comprobante comprobante = ComprobanteBLL.GetByPK(new Comprobante { com_numero = clave, com_numero_key = clave, com_empresa = empresa, com_empresa_key = empresa });

            Packages.Offline.CrearArchivoXML(comprobante.com_empresa, comprobante.com_numero);
            Packages.Offline.SendComprobanteAsync(comprobante);

            /*Archivo arc = ArchivoBLL.GetByPK(new Archivo { arc_empresa = comprobante.com_empresa, arc_empresa_key = comprobante.com_empresa, arc_numero = comprobante.com_numero, arc_numero_key = comprobante.com_numero });

            string xmlpath = comprobante.com_xml;
            if (File.Exists(xmlpath))
                File.Delete(xmlpath);
            // Create a file to write to. 
            using (StreamWriter sw = File.CreateText(xmlpath))
            {
                sw.Write(arc.arc_xml);
                sw.Flush();
                sw.Close();
            }*/
            return "Reenviando...";
            //return EnviarComprobante(empresa, clave);

        }

        public static string ResetComprobante(int empresa, string clave)
        {
            Comprobante comprobante = ComprobanteBLL.GetByPK(new Comprobante { com_numero = clave, com_numero_key = clave, com_empresa = empresa, com_empresa_key = empresa });
            comprobante.com_estado = (int)Enums.EstadoComprobante.Proceso;
            comprobante.com_numero_key = comprobante.com_numero;
            comprobante.com_empresa_key = comprobante.com_empresa;
            

            //Archivo archivo = ArchivoBLL.GetByPK(new Archivo{ arc_numero = clave, arc_numero_key = clave, arc_empresa = empresa, arc_empresa_key     = empresa });

            //string xmlpath = HttpContext.Current.Server.MapPath("../xml/" + comprobante.com_numero + ".xml");
            //if (File.Exists(xmlpath))
            //    File.Delete(xmlpath);
            //// Create a file to write to. 
            //using (StreamWriter sw = File.CreateText(xmlpath))
            //{
            //    sw.Write(archivo.arc_xml);
            //    sw.Flush();
            //    sw.Close();
            //}
            //comprobante.com_xml = xmlpath;
            ComprobanteBLL.Update(comprobante);
            return "reseteado";

        }

        #region Metodos Enviar Comprobante

        delegate string DelegadoEnviarComprobante(int empresa, string clave);

        public static void EnviarComprobanteAsync(int empresa, string clave)
        {
            DelegadoEnviarComprobante delegado = new DelegadoEnviarComprobante(EnviarComprobante);
            IAsyncResult result = delegado.BeginInvoke(empresa, clave, null, null);
        }

        public static string EnviarComprobante(int empresa, string clave)
        {
            Empresa emp = EmpresaBLL.GetByPK(new Empresa { emp_codigo = empresa, emp_codigo_key = empresa });
            FirmarComprobante(empresa, clave);
            Comprobante comprobante = ComprobanteBLL.GetByPK(new Comprobante { com_numero = clave, com_numero_key = clave, com_empresa = empresa, com_empresa_key = empresa });
            if (comprobante.com_estado >= (int)Enums.EstadoComprobante.Firmado)
            {
                byte[] buff = null;
                FileStream fs = new FileStream(comprobante.com_xml, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                long numBytes = new FileInfo(comprobante.com_xml).Length;
                buff = br.ReadBytes((int)numBytes);


                if (emp.emp_funcion.Value == (int)Enums.FuncionEmpresa.Online)//ONLINE
                {

                    if (comprobante.com_ambiente == 1)//PRUEBAS
                    {
                        WebUI.ec.gob.sri.celcer.RecepcionComprobantesService a = new ec.gob.sri.celcer.RecepcionComprobantesService();
                        a.clave = clave;
                        a.empresa = empresa;
                        a.validarComprobanteCompleted += new WebUI.ec.gob.sri.celcer.validarComprobanteCompletedEventHandler(prueba_validarComprobanteCompleted);
                        a.validarComprobanteAsync(buff);
                    }
                    if (comprobante.com_ambiente == 2)//PRODUCCION
                    {
                        WebUI.ec.gob.sri.cel.RecepcionComprobantesService a = new ec.gob.sri.cel.RecepcionComprobantesService();
                        a.clave = clave;
                        a.empresa = empresa;
                        a.validarComprobanteCompleted += new WebUI.ec.gob.sri.cel.validarComprobanteCompletedEventHandler(produccion_validarComprobante1Completed);

                        a.validarComprobanteAsync(buff);
                    }
                }
                if (emp.emp_funcion.Value == (int)Enums.FuncionEmpresa.Offline)//OFFLINE
                {

                    if (comprobante.com_ambiente == 1)//PRUEBAS
                    {
                        WebUI.ec.gob.sri.celcerOff.RecepcionComprobantesOfflineService a = new ec.gob.sri.celcerOff.RecepcionComprobantesOfflineService();
                        a.clave = clave;
                        a.empresa = empresa;
                        a.validarComprobanteCompleted += new WebUI.ec.gob.sri.celcerOff.validarComprobanteCompletedEventHandler(prueba_validarComprobanteOffCompleted);
                        a.validarComprobanteAsync(buff);
                    }
                    if (comprobante.com_ambiente == 2)//PRODUCCION
                    {
                        WebUI.ec.gob.sri.cel.RecepcionComprobantesService a = new ec.gob.sri.cel.RecepcionComprobantesService();
                        a.clave = clave;
                        a.empresa = empresa;
                        a.validarComprobanteCompleted += new WebUI.ec.gob.sri.cel.validarComprobanteCompletedEventHandler(produccion_validarComprobante1Completed);

                        a.validarComprobanteAsync(buff);
                    }
                }


                comprobante.com_estado = (int)Enums.EstadoComprobante.Enviado;
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
            return "no enviada";
        }

        //Validar comprobante Online Pruebas
        static void prueba_validarComprobanteCompleted(object sender, WebUI.ec.gob.sri.celcer.validarComprobanteCompletedEventArgs e)
        {
            WebUI.ec.gob.sri.celcer.RecepcionComprobantesService clase = (WebUI.ec.gob.sri.celcer.RecepcionComprobantesService)sender;
            Array a = (Array)e.Result;

            ValidarComprobante(a, clase.clave, clase.empresa);
                        
        }

        //Validar comprobante Online Produccion
        static void produccion_validarComprobante1Completed(object sender, WebUI.ec.gob.sri.cel.validarComprobanteCompletedEventArgs e)
        {
            WebUI.ec.gob.sri.cel.RecepcionComprobantesService clase = (WebUI.ec.gob.sri.cel.RecepcionComprobantesService)sender;

            Array a = (Array)e.Result;
            ValidarComprobante(a, clase.clave, clase.empresa);
        }

        //Validar comprobante Offline Pruebas
        static void prueba_validarComprobanteOffCompleted(object sender, WebUI.ec.gob.sri.celcerOff.validarComprobanteCompletedEventArgs e)
        {
            WebUI.ec.gob.sri.celcerOff.RecepcionComprobantesOfflineService clase = (WebUI.ec.gob.sri.celcerOff.RecepcionComprobantesOfflineService)sender;
            Array a = (Array)e.Result;
            ValidarComprobante(a, clase.clave, clase.empresa);
        }

        //Validar comprobante Offline Produccion
        static void produccion_validarComprobante1OffCompleted(object sender, WebUI.ec.gob.sri.celOff.validarComprobanteCompletedEventArgs e)
        {
            WebUI.ec.gob.sri.celOff.RecepcionComprobantesOfflineService clase = (WebUI.ec.gob.sri.celOff.RecepcionComprobantesOfflineService)sender;
            Array a = (Array)e.Result;
            ValidarComprobante(a, clase.clave, clase.empresa);
        }


        public static void ValidarComprobante(Array a, string clave, int empresa)
        {
            XmlNode[] nodos = (XmlNode[])a.GetValue(0);

            Comprobante comprobante = ComprobanteBLL.GetByPK(new Comprobante { com_numero = clave, com_numero_key = clave, com_empresa = empresa, com_empresa_key = empresa });
            Archivo arc = ArchivoBLL.GetByPK(new Archivo { arc_empresa = empresa, arc_empresa_key = empresa, arc_numero = clave, arc_numero_key = clave });
            XmlNode nodoestado = nodos[0];            
            if (nodoestado.InnerText != "RECIBIDA")
            {
                arc.arc_xmlrespuesta = nodos[1].InnerXml;
                comprobante.com_estado = (int)Enums.EstadoComprobante.Devuelto;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(arc.arc_xmlrespuesta);
                comprobante.com_mensaje = GetMensaje(doc);
            }
            else
            {
                comprobante.com_estado = (int)Enums.EstadoComprobante.Recibido;
                arc.arc_xmlrespuesta = nodos[0].InnerXml;
                
            }
            
            comprobante.com_fecharespuesta = DateTime.Now;
            comprobante.com_numero_key = comprobante.com_numero;
            comprobante.com_empresa_key = comprobante.com_empresa;
            ComprobanteBLL.Update(comprobante);

            arc.arc_empresa_key = arc.arc_empresa;
            arc.arc_numero_key = arc.arc_numero;
            ArchivoBLL.Update(arc);
            Services.Pdf.SavePDF(comprobante.com_empresa, comprobante.com_numero, HttpContext.Current.Server.MapPath("../pdf"), false, "");

            if (comprobante.com_estado == (int)Enums.EstadoComprobante.Recibido)
                VerificarComprobante(comprobante.com_empresa, comprobante.com_numero);
            else
                CorreoErrorComprobante(comprobante.com_empresa, comprobante.com_numero);
        }


        #endregion

        #region Metodos Verificar Comprobante

        delegate string DelegadoVerificarComprobante(int empresa, string clave);

        public static void VerificarComprobanteAsync(int empresa, string clave)
        {
            DelegadoVerificarComprobante delegado = new DelegadoVerificarComprobante(VerificarComprobante);
            IAsyncResult result = delegado.BeginInvoke(empresa, clave, null, null);
        }

        public static string VerificarComprobante(int empresa, string clave)
        {
            Empresa emp = EmpresaBLL.GetByPK(new Empresa { emp_codigo = empresa, emp_codigo_key = empresa });
            Comprobante comprobante = ComprobanteBLL.GetByPK(new Comprobante { com_numero = clave, com_numero_key = clave, com_empresa = empresa, com_empresa_key = empresa });
            if (emp.emp_funcion.Value == (int)Enums.FuncionEmpresa.Online)//ONLINE
            {
                if (comprobante.com_ambiente == 1)//PRUEBAS
                {
                    WebUI.ec.gob.sri.celcer1.AutorizacionComprobantesService a = new ec.gob.sri.celcer1.AutorizacionComprobantesService();                    
                    a.autorizacionComprobanteCompleted += new WebUI.ec.gob.sri.celcer1.autorizacionComprobanteCompletedEventHandler(prueba_autorizacionComprobanteCompleted);
                    a.empresa = empresa;
                    a.clave = clave;
                    a.autorizacionComprobanteAsync(clave.ToString());
                    return "enviada";
                }
                if (comprobante.com_ambiente == 2)//PRODUCCION
                {
                    WebUI.ec.gob.sri.cel1.AutorizacionComprobantesService a = new ec.gob.sri.cel1.AutorizacionComprobantesService();
                    a.autorizacionComprobanteCompleted += new WebUI.ec.gob.sri.cel1.autorizacionComprobanteCompletedEventHandler(produccion_autorizacionComprobanteCompleted);

                    a.empresa = empresa;
                    a.clave = clave;
                    a.autorizacionComprobanteAsync(clave.ToString());
                    return "enviada";
                }
            }
            if (emp.emp_funcion.Value == (int)Enums.FuncionEmpresa.Offline)//OFFLINE
            {
                if (comprobante.com_ambiente == 1)//PRUEBAS
                {
                    //WebUI.ec.gob.sri.celcer.AutorizacionComprobantesService a = new ec.gob.sri.celcer1.AutorizacionComprobantesService();
                    //a.autorizacionComprobanteCompleted += new WebUI.ec.gob.sri.celcer1.autorizacionComprobanteCompletedEventHandler(prueba_autorizacionComprobanteCompleted);
                    //a.empresa = empresa;
                    //a.clave = clave;
                    //a.autorizacionComprobanteAsync(clave.ToString());
                    //return "enviada";
                }
                if (comprobante.com_ambiente == 2)//PRODUCCION
                {
                    WebUI.ec.gob.sri.cel1Off.AutorizacionComprobantesOfflineService a = new ec.gob.sri.cel1Off.AutorizacionComprobantesOfflineService();
                    a.autorizacionComprobanteCompleted += new WebUI.ec.gob.sri.cel1Off.autorizacionComprobanteCompletedEventHandler(produccion_autorizacionComprobanteOffCompleted);
                    a.empresa = empresa;
                    a.clave = clave;
                    a.autorizacionComprobanteAsync(clave.ToString());
                    return "enviada";
                }
            }

            return "";
           
        }

        static void produccion_autorizacionComprobanteLoteCompleted(object sender, ec.gob.sri.cel1.autorizacionComprobanteLoteCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        //Autorizacion comprobante ONLINE pruebas
        static void prueba_autorizacionComprobanteCompleted(object sender, WebUI.ec.gob.sri.celcer1.autorizacionComprobanteCompletedEventArgs e)
        {
            WebUI.ec.gob.sri.celcer1.AutorizacionComprobantesService clase = (WebUI.ec.gob.sri.celcer1.AutorizacionComprobantesService)sender;
            Array a = (Array)e.Result;
            AutorizacionComprobante(a, clase.clave, clase.empresa);

        }

        //Autorizacion comprobante ONLINE producccion
        static void produccion_autorizacionComprobanteCompleted(object sender, WebUI.ec.gob.sri.cel1.autorizacionComprobanteCompletedEventArgs e)
        {
            WebUI.ec.gob.sri.cel1.AutorizacionComprobantesService clase = (WebUI.ec.gob.sri.cel1.AutorizacionComprobantesService)sender;
            Array a = (Array)e.Result;
            AutorizacionComprobante(a, clase.clave, clase.empresa);

        }

        //Autorizacion comprobante OFFLINE pruebas
        //static void prueba_autorizacionComprobanteCompleted(object sender, WebUI.ec.gob.sri.celcer1.autorizacionComprobanteCompletedEventArgs e)
        //{
        //    WebUI.ec.gob.sri.celcer1.AutorizacionComprobantesService clase = (WebUI.ec.gob.sri.celcer1.AutorizacionComprobantesService)sender;
        //    Array a = (Array)e.Result;
        //    AutorizacionComprobante(a, clase.clave, clase.empresa);

        //}

        //Autorizacion comprobante OFFLINE producccion

        static void produccion_autorizacionComprobanteOffCompleted(object sender, WebUI.ec.gob.sri.cel1Off.autorizacionComprobanteCompletedEventArgs e)
        {
            WebUI.ec.gob.sri.cel1Off.AutorizacionComprobantesOfflineService clase = (WebUI.ec.gob.sri.cel1Off.AutorizacionComprobantesOfflineService)sender;
            Array a = (Array)e.Result;
            AutorizacionComprobante(a, clase.clave, clase.empresa);

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
                        Archivo arc = ArchivoBLL.GetByPK(new Archivo { arc_empresa = comprobante.com_empresa, arc_empresa_key = comprobante.com_empresa, arc_numero = comprobante.com_numero, arc_numero_key = comprobante.com_numero });
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
                        

                        if (comprobante.com_estado == (int)Enums.EstadoComprobante.Autorizado)
                        {
                            string xmlpath = HttpContext.Current.Server.MapPath("../xml/" + comprobante.com_numero + ".xml");
                            if (File.Exists(xmlpath))
                                File.Delete(xmlpath);
                            using (StreamWriter sw = File.CreateText(xmlpath))
                            {
                                sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
                                sw.Write(nodoautorizaciones.InnerXml);
                                sw.Flush();
                                sw.Close();
                            }

                            Services.Pdf.SavePDF(comprobante.com_empresa, comprobante.com_numero, HttpContext.Current.Server.MapPath("../pdf"), false, "");
                            CorreoComprobante(comprobante.com_empresa, comprobante.com_numero,true);
                        }
                        else
                        {
                            CorreoErrorComprobante(comprobante.com_empresa, comprobante.com_numero);
                        }
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

        public static string CorreoComprobante(int empresa, string clave, bool automatic)
        {
            try
            {
                Comprobante comprobante = ComprobanteBLL.GetByPK(new Comprobante { com_numero = clave, com_numero_key = clave, com_empresa = empresa, com_empresa_key = empresa });
                Empresa emp = EmpresaBLL.GetByPK(new Empresa { emp_codigo = empresa, emp_codigo_key = empresa });
                Formato formato = FormatoBLL.GetByPK(new Formato { for_empresa = empresa, for_empresa_key = empresa, for_codigo = comprobante.com_formato.Value, for_codigo_key = comprobante.com_formato.Value });

                string strautomatic = !automatic ? "</br>Envío manual " + DateTime.Now.ToString() + "<br>Estado:" + Enums.GetEstadoComprobante(comprobante.com_estado.Value) + "<br>Autorizacion:" + comprobante.com_autorizacion : "";
                
                Mail mail = new Mail();
                if (comprobante.com_estado == (int)Enums.EstadoComprobante.Autorizado)
                {
                    mail.from = emp.emp_mail;
                    mail.to = comprobante.com_email;
                    mail.subject = "Su factura electrónica de " + emp.emp_nombre;
                    if (!File.Exists(comprobante.com_xml))
                    {
                        Archivo archivo = ArchivoBLL.GetByPK(new Archivo { arc_numero = clave, arc_numero_key = clave, arc_empresa = empresa, arc_empresa_key = empresa });
                        using (StreamWriter sw = File.CreateText(comprobante.com_xml))
                        {
                            sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
                            sw.Write(Packages.General.FormatXML(archivo.arc_xmlrespuesta));
                            sw.Flush();
                            sw.Close();
                        }
                    }
                    mail.attachments.Add(comprobante.com_xml);
                    if (string.IsNullOrEmpty(comprobante.com_pdf))
                        comprobante.com_pdf = Services.Pdf.SavePDF(comprobante.com_empresa, comprobante.com_numero, HttpContext.Current.Server.MapPath("../pdf"), false, "");
                    mail.attachments.Add(comprobante.com_pdf);
                    mail.body = formato.for_mail.Replace("%cliente%", comprobante.com_nombrecliente).Replace("%empresa%", emp.emp_nombre) +strautomatic;
                }
                else
                {
                    mail.to = emp.emp_mailadm;

                    mail.subject = "Notificación Comprobante Electrónico " + emp.emp_nombre;
                    mail.body = "El comprobante # " + comprobante.com_almacen + "-" + comprobante.com_pventa + "-" + comprobante.com_secuencia + "  se encuentra en estado <b>" + Enums.GetEstadoComprobante(comprobante.com_estado.Value) +"</b>"+
                    "<br><br>Motivo: " + comprobante.com_mensaje + "<br><br>Por favor revise más información en el portal.<br><br><p>Atentamente,<br/>" + emp.emp_nombre + "</p>"+strautomatic;
                    
                }
                
                //if (mail.SendMailAsync())
                //    return "mail enviado";
                //else
                //    return "mail no enviado";
                if (mail.SendMail(false))
                    return "mail enviado";
                else
                    return "mail no enviado";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }





          
        }

        public static bool ValidaMail(string mail)
        {
            if (mail.IndexOf("@") >= 0)
                return true;
            else
                return false;
        }

        public static string CorreoMasivo(int empresa, string asunto)
        {
            StringBuilder resultado = new StringBuilder();
            Empresa emp = EmpresaBLL.GetByPK(new Empresa { emp_codigo = empresa, emp_codigo_key = empresa });
            Formato formato = FormatoBLL.GetByPK(new Formato { for_empresa = empresa, for_empresa_key = empresa, for_codigo = 3, for_codigo_key = 3});
            List<Usuario> users = UsuarioBLL.GetAll(new WhereParams("usr_perfil={0} and usr_estado={1}", Constantes.cPerfilUsuario, 1), "");
            List<Comprobante> comprobantes = ComprobanteBLL.GetAll(new WhereParams("com_empresa={0} and com_estado={1} and com_ambiente={2}", empresa, 5, 2), "");


            foreach (Usuario usr in users)
            {
                try
                {

                    if (ValidaMail(usr.usr_mail))
                    {
                        List<Comprobante> lst = comprobantes.FindAll(delegate (Comprobante c) { return c.com_email  == usr.usr_mail; });
                        if (lst.Count > 0)
                        {
                            Mail mail = new Mail();

                            mail.from = emp.emp_mail;
                            mail.to = usr.usr_mail;
                            mail.subject = asunto;
                            mail.body = formato.for_mail.Replace("%usuario%", usr.usr_nombres).Replace("%mail%", usr.usr_mail).Replace("%password%", usr.usr_password).Replace("%codempresa%", emp.emp_codigo.ToString()).Replace("%empresa%", emp.emp_nombre);
                            mail.SendMail(true);
                            resultado.AppendFormat("OK {0} {1}<br>", usr.usr_id, usr.usr_mail);
                        }
                    }
                }
                catch (Exception ex)
                {
                    resultado.AppendFormat("ERROR {0} {1} {2}<br>", usr.usr_id, usr.usr_mail, ex.Message);
                }

            }
            return resultado.ToString();








        }

        //public static string FindAutorizacion(XmlDocument xmldoc)
        //{

        //    foreach (XmlNode xmlautorizacion in xmldoc.SelectNodes("/autorizaciones/autorizacion"))
        //    {
        //        XmlNode xmlestado = xmlautorizacion.SelectSingleNode("estado");
        //        XmlNode xmlnumero = xmlautorizacion.SelectSingleNode("numeroAutorizacion");
        //        if (xmlestado.InnerText == "AUTORIZADO")
        //            return xmlnumero.InnerText;
        //    }
        //    return "";
        //}

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

        public static string GetMensaje(int empresa, string clave)
        {
            try
            {
                Comprobante comprobante = ComprobanteBLL.GetByPK(new Comprobante { com_numero = clave, com_numero_key = clave, com_empresa = empresa, com_empresa_key = empresa });
                Archivo arc = ArchivoBLL.GetByPK(new Archivo { arc_empresa = comprobante.com_empresa, arc_empresa_key = comprobante.com_empresa, arc_numero = comprobante.com_numero, arc_numero_key = comprobante.com_numero });

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(arc.arc_xmlrespuesta);
                comprobante.com_mensaje = GetMensaje(doc);
                comprobante.com_numero_key = comprobante.com_numero;
                comprobante.com_empresa_key = comprobante.com_empresa;
                ComprobanteBLL.Update(comprobante);

                return  comprobante.com_mensaje;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
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
                    comprobante.com_estado = (int)Enums.EstadoComprobante.Autorizado;
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


            comprobante.com_estado = (int)Enums.EstadoComprobante.NoAutorizado;                        
            comprobante.com_fecharespuesta = DateTime.Now;
            comprobante.com_mensaje = mensaje;
            return comprobante;
        }

        public static string BuscarAutorizacion(int empresa, string clave)
        {
            try
            {
                Comprobante comprobante = ComprobanteBLL.GetByPK(new Comprobante { com_numero = clave, com_numero_key = clave, com_empresa = empresa, com_empresa_key = empresa });
                Archivo arc = ArchivoBLL.GetByPK(new Archivo { arc_empresa = comprobante.com_empresa, arc_empresa_key = comprobante.com_empresa, arc_numero = comprobante.com_numero, arc_numero_key = comprobante.com_numero });

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(arc.arc_xmlrespuesta);
                comprobante = FindAutorizacion(doc, comprobante);
                comprobante.com_numero_key = comprobante.com_numero;
                comprobante.com_empresa_key = comprobante.com_empresa;
                ComprobanteBLL.Update(comprobante);

                return comprobante.com_autorizacion + " " + comprobante.com_mensaje;


                //string numero = FindAutorizacion(doc);
                //if (numero != "")
                //{
                //    comprobante.com_estado = (int)Enums.EstadoComprobante.Autorizado;
                //    comprobante.com_autorizacion = numero;
                //    //comprobante.com_fechaautorizacion = (nodofechaautorizacion != null) ? nodofechaautorizacion.InnerText : "";
                //    comprobante.com_numero_key = comprobante.com_numero;
                //    comprobante.com_empresa_key = comprobante.com_empresa;
                //    ComprobanteBLL.Update(comprobante);
                //}
                //return numero;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string VerificarComprobantes()
        {

            List<Comprobante> comprobantes = ComprobanteBLL.GetAll(new WhereParams("com_estado={0}", (int)Enums.EstadoComprobante.Recibido),"");
            foreach (Comprobante comprobante in comprobantes)
            {
                ExceptionHandling.Log.AddLog("VERIFICAR " + comprobante.com_numero);
                VerificarComprobante(comprobante.com_empresa, comprobante.com_numero);
                
            }            
            return "ok";

        }

        public static string CorreoErrorComprobante(int empresa, string clave)
        {
            try
            {
                Comprobante comprobante = ComprobanteBLL.GetByPK(new Comprobante { com_numero = clave, com_numero_key = clave, com_empresa = empresa, com_empresa_key = empresa });
                Empresa emp = EmpresaBLL.GetByPK(new Empresa { emp_codigo = empresa, emp_codigo_key = empresa });
                
                


                Mail mail = new Mail();
                mail.from = emp.emp_mail;
                mail.to = emp.emp_mailadm;
                mail.subject = "Notificación Comprobante Electrónico " + emp.emp_nombre;

                 mail.body = "El comprobante # " + comprobante.com_almacen + "-" + comprobante.com_pventa + "-" + comprobante.com_secuencia + "  se encuentra en estado <b>" + Enums.GetEstadoComprobante(comprobante.com_estado.Value) +"</b>"+
                    "<br><br>Motivo: " + comprobante.com_mensaje + "<br><br>Por favor revise más información en el portal.<br><br><p>Atentamente,<br/>" + emp.emp_nombre + "</p>";
                if (mail.SendMail(true))
                    return "mail enviado";
                else
                    return "mail no enviado";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }


        }

        public static string Eliminar(int empresa, string clave)
        {

            Comprobante comprobante = ComprobanteBLL.GetByPK(new Comprobante { com_numero = clave, com_numero_key = clave, com_empresa = empresa, com_empresa_key = empresa });
            comprobante.com_estado = (int)Enums.EstadoComprobante.Eliminado;
            comprobante.com_numero_key = comprobante.com_numero;
            comprobante.com_empresa_key = comprobante.com_empresa;
            ComprobanteBLL.Update(comprobante);

            return "ok";


        }

        #region Recalculos




        #endregion

    }
}