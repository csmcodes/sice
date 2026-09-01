using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects;
using BusinessLogicLayer;
using Services;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.IO;
using System.Text;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using System.Xml;
using System.Web.Script.Serialization;
using Functions;


namespace Services
{
    public class Pdf
    {
              
        

        public static decimal GetDecimal(string valor)
        {
            decimal valordecimal = 0;
            decimal.TryParse(valor.Replace('.', ','), out valordecimal);
            return valordecimal;
        }



        public static string SavePDF(int empresa, string numero, string path, bool autoprint, string impresora)
        {
            Empresa emp = new Empresa();
            emp.emp_codigo = empresa;
            emp.emp_codigo_key = empresa;
            emp = EmpresaBLL.GetByPK(emp);

            Comprobante comp = new Comprobante();
            comp.com_empresa = empresa;
            comp.com_empresa_key = empresa;
            comp.com_numero = numero;
            comp.com_numero_key = numero;
            comp = ComprobanteBLL.GetByPK(comp);

            if (!comp.com_formato.HasValue)
                comp.com_formato = 1;


            Archivo arc = new Archivo();
            arc.arc_empresa = empresa;
            arc.arc_empresa_key = empresa;
            arc.arc_numero = numero;
            arc.arc_numero_key = numero;
            arc = ArchivoBLL.GetByPK(arc);


            Formato formato = new Formato();
            formato.for_empresa = empresa;
            formato.for_empresa_key = empresa;
            formato.for_codigo = comp.com_formato.Value;
            formato.for_codigo_key = comp.com_formato.Value;
            formato = FormatoBLL.GetByPK(formato);



            if (formato.for_tipo == "FAC")
                return SaveFACPDF(emp, comp, formato, arc, path, autoprint, impresora);
            if (formato.for_tipo == "NC")
                return SaveNCPDF(emp, comp, formato, arc, path, autoprint, impresora);
            if (formato.for_tipo == "RET")
                return SaveRETPDF(emp, comp, formato, arc, path, autoprint, impresora);
            if (formato.for_tipo == "GREM")
                return SaveGREMPDF(emp, comp, formato, arc, path, autoprint, impresora);
            return "";
        }


        public  static string SaveBigFACPDF(Empresa emp, Comprobante comp, Formato formato,Archivo arc, string path, bool autoprint, string impresora)
        {
            string pdfTemplateHeader = path + @"\" + formato.for_pdf.Replace(".pdf","H.pdf");
            string pdfTemplateBody = path + @"\" + formato.for_pdf.Replace(".pdf", "B.pdf");
            string pdfTemplateFooter = path + @"\" + formato.for_pdf.Replace(".pdf", "F.pdf");
            string pdfpathHeader = path + @"\" + comp.com_numero + "H.pdf";
            string pdfpathBody = path + @"\" + comp.com_numero + "B1.pdf";
            string pdfpathFooter = path + @"\" + comp.com_numero + "F.pdf";

            string pdfpath= path + @"\" + comp.com_numero + ".pdf";


            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(arc.arc_xml);

            //CAMPOS CABECERA
            PdfReader pdfReaderHeader = new PdfReader(pdfTemplateHeader);
            PdfStamper pdfStamperHeader = new PdfStamper(pdfReaderHeader, new FileStream(pdfpathHeader, FileMode.Create));
            AcroFields pdfFormFieldsHeader = pdfStamperHeader.AcroFields;

            foreach (KeyValuePair<string, AcroFields.Item> kvp in pdfFormFieldsHeader.Fields)
            {
                pdfFormFieldsHeader.SetFieldProperty(kvp.Key, "textsize", 7f, null);
                pdfFormFieldsHeader.SetFieldProperty(kvp.Key, "setfflags", PdfFormField.MK_CAPTION_LEFT, null);
                pdfFormFieldsHeader.SetFieldProperty(kvp.Key, "setfflags", PdfFormField.FF_READ_ONLY, null);
                if (kvp.Key.IndexOf("DescripciónRow") >= 0)
                {
                    pdfFormFieldsHeader.SetFieldProperty(kvp.Key, "fflags", PdfFormField.FF_MULTILINE, null);
                }
            }

            //CAMPOS FOOTER
            PdfReader pdfReaderFooter = new PdfReader(pdfTemplateFooter);
            PdfStamper pdfStamperFooter = new PdfStamper(pdfReaderFooter, new FileStream(pdfpathFooter, FileMode.Create));
            AcroFields pdfFormFieldsFooter = pdfStamperFooter.AcroFields;

            foreach (KeyValuePair<string, AcroFields.Item> kvp in pdfFormFieldsFooter.Fields)
            {
                pdfFormFieldsFooter.SetFieldProperty(kvp.Key, "textsize", 7f, null);
                pdfFormFieldsFooter.SetFieldProperty(kvp.Key, "setfflags", PdfFormField.MK_CAPTION_LEFT, null);
                pdfFormFieldsFooter.SetFieldProperty(kvp.Key, "setfflags", PdfFormField.FF_READ_ONLY, null);
                if (kvp.Key.IndexOf("DescripciónRow") >= 0)
                {
                    pdfFormFieldsFooter.SetFieldProperty(kvp.Key, "fflags", PdfFormField.FF_MULTILINE, null);
                }

            }


            //CAMPOS BODY
            PdfReader pdfReaderBody = new PdfReader(pdfTemplateBody);
            PdfStamper pdfStamperBody= new PdfStamper(pdfReaderBody, new FileStream(pdfpathBody, FileMode.Create));
            AcroFields pdfFormFieldsBody = pdfStamperBody.AcroFields;

            foreach (KeyValuePair<string, AcroFields.Item> kvp in pdfFormFieldsBody.Fields)
            {
                pdfFormFieldsBody.SetFieldProperty(kvp.Key, "textsize", 7f, null);
                pdfFormFieldsBody.SetFieldProperty(kvp.Key, "setfflags", PdfFormField.MK_CAPTION_LEFT, null);
                pdfFormFieldsBody.SetFieldProperty(kvp.Key, "setfflags", PdfFormField.FF_READ_ONLY, null);
                if (kvp.Key.IndexOf("DescripciónRow") >= 0)
                {
                    pdfFormFieldsBody.SetFieldProperty(kvp.Key, "fflags", PdfFormField.FF_MULTILINE, null);
                }

            }



            pdfFormFieldsHeader.SetField("ruc", emp.emp_ruc);
            pdfFormFieldsHeader.SetField("autorizacion", comp.com_autorizacion);
            pdfFormFieldsHeader.SetField("fechaautorizacion", comp.com_fechaautorizacion);
            //pdfFormFields.SetField("clave", comp.com_numero);
            pdfFormFieldsHeader.SetField("numero", comp.com_almacen + "-" + comp.com_pventa + "-" + comp.com_secuencia);

            pdfFormFieldsHeader.SetField("ruccli", comp.com_ruccliente);
            pdfFormFieldsHeader.SetField("razon", comp.com_nombrecliente);
            pdfFormFieldsHeader.SetField("fechaemision", comp.com_fecha.Value.ToShortDateString());

            XmlNode ambiente = xmldoc.SelectSingleNode("/factura/infoTributaria/ambiente");
            XmlNode emision = xmldoc.SelectSingleNode("/factura/infoTributaria/tipoEmision");
            XmlNode dirmatriz = xmldoc.SelectSingleNode("/factura/infoTributaria/dirMatriz");
            XmlNode dirsucursal = xmldoc.SelectSingleNode("/factura/infoFactura/dirEstablecimiento");
            XmlNode contribuyente = xmldoc.SelectSingleNode("/factura/infoFactura/contribuyenteEspecial");
            XmlNode obligado = xmldoc.SelectSingleNode("/factura/infoFactura/obligadoContabilidad");

            XmlNode infoadicional = xmldoc.SelectSingleNode("/factura/infoAdicional");

            string direccion = "";
            string telefono = "";
            string vendedor = "";
            string ciudad = "";
            string fpago = "";
            string transporte = "";
            string observacion = "";
            string compensacion = "";
            if (infoadicional != null)
            {
                foreach (XmlNode iteminfo in infoadicional.ChildNodes)
                {
                    if (iteminfo.Attributes["nombre"].Value == "Direccion")
                        direccion = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Telefono")
                        telefono = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Vendedor")
                        vendedor = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Ciudad")
                        ciudad = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Fpago")
                        fpago = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Transporte")
                        transporte = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Observacion")
                        observacion = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Compensado")
                        compensacion = iteminfo.InnerText;

                }

            }

            observacion += " " + compensacion;


            string strambiente = ambiente.InnerText == "1" ? "Pruebas" : "Producción";
            pdfFormFieldsHeader.SetField("ambiente", strambiente);

            string stremision = emision.InnerText == "1" ? "Normal" : "Contingencia";
            pdfFormFieldsHeader.SetField("emision", stremision);

            pdfFormFieldsHeader.SetField("dirmatriz", dirmatriz.InnerText);
            pdfFormFieldsHeader.SetField("dirsucursal", dirsucursal != null ? dirsucursal.InnerText : "");

            if (contribuyente != null)
                pdfFormFieldsHeader.SetField("contribuyente", contribuyente.InnerText);
            if (obligado != null)
                pdfFormFieldsHeader.SetField("obligado", obligado.InnerText);

           

            pdfFormFieldsHeader.SetField("direccioncli", direccion);
            pdfFormFieldsHeader.SetField("telefonocli", telefono);
            pdfFormFieldsHeader.SetField("emailcli", comp.com_email);

            pdfFormFieldsFooter.SetField("observacion", observacion);

            int valiva = 12;
            if (comp.com_fecha >= new DateTime(2016, 6, 1))
                valiva = 14;


            decimal subtotaliva = 0;

            decimal valoriva = 0;
            decimal subtotalice = 0;
            decimal valorice = 0;
            decimal subtotal0 = 0;


            XmlNode totalimpuestos = xmldoc.SelectSingleNode("/factura/infoFactura/totalConImpuestos");
            foreach (XmlNode totalimp in totalimpuestos.ChildNodes)
            {
                XmlNode codigo = totalimp.SelectSingleNode("codigo");
                XmlNode codigoporcentaje = totalimp.SelectSingleNode("codigoPorcentaje");
                XmlNode descuento = totalimp.SelectSingleNode("descuentoAdicional");
                XmlNode baseimp = totalimp.SelectSingleNode("baseImponible");
                XmlNode tarifa = totalimp.SelectSingleNode("tarifa");
                XmlNode valor = totalimp.SelectSingleNode("valor");


                if (codigo.InnerText == "2" && (codigoporcentaje.InnerText == "2" || codigoporcentaje.InnerText == "3"))//TARIFA 12 o 14
                {
                    subtotaliva += GetDecimal(baseimp.InnerText);
                    valoriva += GetDecimal(valor.InnerText);
                    if (codigoporcentaje.InnerText == "2")
                        valiva = 12;
                    if (codigoporcentaje.InnerText == "3")
                        valiva = 14;
                }
                if (codigo.InnerText == "2" && codigoporcentaje.InnerText == "0")//TARIFA 0
                {
                    subtotal0 += GetDecimal(baseimp.InnerText);
                }
                if (codigo.InnerText == "3")//TARIFA ICE
                {
                    subtotalice += GetDecimal(baseimp.InnerText);
                    valorice += GetDecimal(valor.InnerText);
                }
            }


            //decimal subtotal0 = 0;

            //NUEVO CAMBIO PARA INCLUIR COMPENSACIÓN EN CASO DE EXISTIR 01/08/2016
            decimal porcdescsolidario = 0;
            decimal descsolidario = 0;
            XmlNode compensaciones = xmldoc.SelectSingleNode("/factura/infoFactura/compensaciones");
            if (compensaciones != null)
            {

                XmlNode tarifacomp = compensaciones.SelectSingleNode("compensacion/tarifa");
                XmlNode valorcomp = compensaciones.SelectSingleNode("compensacion/valor");
                porcdescsolidario = GetDecimal(tarifacomp.InnerText);
                descsolidario = GetDecimal(valorcomp.InnerText);

            }


            decimal totaldesc = 0;
            XmlNode totaldescuento = xmldoc.SelectSingleNode("/factura/infoFactura/totalDescuento");
            totaldesc = GetDecimal(totaldescuento.InnerText);

            decimal totalcomp = 0;
            XmlNode importetotal = xmldoc.SelectSingleNode("/factura/infoFactura/importeTotal");
            totalcomp = GetDecimal(importetotal.InnerText);

            //////////PARA OBTENER el subtotal0 REAL CAMBIO 02/08/2016////////////////////////
            subtotal0 = totalcomp - (subtotaliva + valoriva);
            ///////////////////////////////////////////////////////////////////////////////



            ///AGREGA LAS FORMAS DE PAGO

            XmlNode pagos = xmldoc.SelectSingleNode("/factura/infoFactura/pagos");

            var serializer = new JavaScriptSerializer();
            List<FormaPago> formas = serializer.Deserialize<List<FormaPago>>(Constantes.GetParameter("formaspago"));
            string strformapago = "";
            string strvalorpago = "";
            if (pagos != null)
            {
                foreach (XmlNode pago in pagos.ChildNodes)
                {
                    XmlNode formaPago = pago.SelectSingleNode("formaPago");
                    FormaPago forma = formas.Find(delegate (FormaPago f) { return f.codigo == formaPago.InnerText; });
                    strformapago += forma.forma + Environment.NewLine;
                    XmlNode total = pago.SelectSingleNode("total");
                    strvalorpago += total.InnerText + Environment.NewLine;
                    XmlNode plazo = pago.SelectSingleNode("plazo");
                    XmlNode unidadTiempo = pago.SelectSingleNode("unidadTiempo");

                }
                pdfFormFieldsFooter.SetField("FORMAPAGO", strformapago);
                pdfFormFieldsFooter.SetField("VALORPAGO", strvalorpago);
            }
            ///////////////////////////////



            //CAMBIO SOLICITADO POR SANTI

            XmlNode totalsinimpuestos = xmldoc.SelectSingleNode("/factura/infoFactura/totalSinImpuestos");
            subtotaliva = decimal.Parse(totalsinimpuestos.InnerText.Replace('.', ',')) + totaldesc - subtotal0;

            decimal subtotal = subtotaliva + subtotal0;

            //NUEVO VALOR AGREGADO EL 08 junio 2015 - Solicitado por Santiago
            decimal subtotalivabi = subtotaliva - totaldesc;
            decimal subtotalbi = subtotal - totaldesc;
            ///

            pdfFormFieldsFooter.SetField("Precio totalSUBTOTAL 12", subtotaliva.ToString());
            pdfFormFieldsFooter.SetField("Precio totalSUBTOTAL 12BI", subtotalivabi.ToString());
            pdfFormFieldsFooter.SetField("Precio totalSUBTOTAL 0", subtotal0.ToString());
            pdfFormFieldsFooter.SetField("Precio totalSUBTOTAL", subtotal.ToString());
            pdfFormFieldsFooter.SetField("Precio totalDESCUENTO", totaldesc.ToString());

            //DESCUENTO SOLIDARIO
            pdfFormFieldsFooter.SetField("porcDS", porcdescsolidario.ToString());
            pdfFormFieldsFooter.SetField("Precio totalDS", descsolidario.ToString());

            pdfFormFieldsFooter.SetField("Precio totalSUBTOTALBI", subtotalbi.ToString());

            pdfFormFieldsFooter.SetField("Precio totalICE", valorice.ToString());
            pdfFormFieldsFooter.SetField("Precio totalIVA 12", valoriva.ToString());
            //pdfFormFields.SetField("Precio totalTRANSPORTE", "");
            pdfFormFieldsFooter.SetField("Precio totalVALOR TOTAL", totalcomp.ToString());

            pdfFormFieldsFooter.SetField("vendedor", vendedor);
            pdfFormFieldsFooter.SetField("ciudad", ciudad);
            pdfFormFieldsFooter.SetField("formapago", fpago);
            pdfFormFieldsFooter.SetField("transporte", transporte);

            pdfFormFieldsFooter.SetField("imp", valiva.ToString("0.00") + "%:");

            XmlNode detalles = xmldoc.SelectSingleNode("/factura/detalles");
            int cantlineas = detalles.ChildNodes.Count;
            int linea = 0;
            
            string pos = "H";

            int bodies = 0;//CANTIDAD DE BODYS A GENERAR
            if (cantlineas>80)//NECESITA BODY
            {
                int lineasbody = (cantlineas - 80);
                for (int i = 0; i < lineasbody; i+=53)
                {
                    bodies++;
                }
            }
            int b = 1;
           
            ArrayList files = new ArrayList();
            files.Add(pdfpathHeader); 

            
            
            foreach (XmlNode detalle in detalles.ChildNodes)
            {
                XmlNode codigo = detalle.SelectSingleNode("codigoPrincipal");
                XmlNode codaux = detalle.SelectSingleNode("codigoAuxiliar");
                XmlNode descripcion = detalle.SelectSingleNode("descripcion");
                XmlNode cantidad = detalle.SelectSingleNode("cantidad");
                XmlNode precio = detalle.SelectSingleNode("precioUnitario");
                XmlNode descuento = detalle.SelectSingleNode("descuento");
                XmlNode total = detalle.SelectSingleNode("precioTotalSinImpuesto");
                XmlNode adicionales = detalle.SelectSingleNode("detallesAdicionales");

                string strdescripción = descripcion.InnerText;
                if (comp.com_formato == 2 && adicionales != null)
                {
                    strdescripción = "";
                    foreach (XmlNode det in adicionales.ChildNodes)
                    {
                        strdescripción += det.Attributes["valor"].Value + Environment.NewLine;
                    }
                }


                linea++;

                if (pos=="H")
                {
                    if (linea>35)
                    {
                        if (bodies>0)
                        {
                            linea = 1;
                            bodies--;
                            files.Add(pdfpathBody);
                            pos = "B";
                        }
                        else
                        {
                            linea = 1;
                            pos = "F";
                        }
                    }
                }
                else if (pos=="B")
                {
                    if (linea>53)
                    {
                        if (bodies > 0)
                        {
                            linea = 1;
                            bodies--;
                            b++;
                            files.Add(pdfpathBody);
                            pos = "B";
                            pdfStamperBody.FormFlattening = false;
                            // close the pdf
                            pdfStamperBody.Close();
                            
                            //pdfTemplateBody = path + @"\" + formato.for_pdf.Replace(".pdf", "B"+b.ToString()+".pdf");
                            pdfReaderBody = new PdfReader(pdfTemplateBody);
                            pdfpathBody = path + @"\" + comp.com_numero + "B" + b.ToString() + ".pdf";
                            pdfStamperBody = new PdfStamper(pdfReaderBody, new FileStream(pdfpathBody, FileMode.Create));
                            pdfFormFieldsBody = pdfStamperBody.AcroFields;

                            foreach (KeyValuePair<string, AcroFields.Item> kvp in pdfFormFieldsBody.Fields)
                            {
                                pdfFormFieldsBody.SetFieldProperty(kvp.Key, "textsize", 7f, null);
                                pdfFormFieldsBody.SetFieldProperty(kvp.Key, "setfflags", PdfFormField.MK_CAPTION_LEFT, null);
                                pdfFormFieldsBody.SetFieldProperty(kvp.Key, "setfflags", PdfFormField.FF_READ_ONLY, null);
                                if (kvp.Key.IndexOf("DescripciónRow") >= 0)
                                {
                                    pdfFormFieldsBody.SetFieldProperty(kvp.Key, "fflags", PdfFormField.FF_MULTILINE, null);
                                }

                            }
                        }
                        else
                        {
                            linea = 1;
                            pos = "F";
                        }

                    }
                }


                
             
                if (pos == "H")//HEAEDER
                {
                    pdfFormFieldsHeader.SetField("Código PrincipalRow" + linea.ToString(), codigo.InnerText);
                    pdfFormFieldsHeader.SetField("DescripciónRow" + linea.ToString(), strdescripción);
                    pdfFormFieldsHeader.SetField("CantidadRow" + linea.ToString(), cantidad.InnerText);

                    pdfFormFieldsHeader.SetField("Precio unitarioRow" + linea.ToString(), precio.InnerText);

                    decimal totalrow = decimal.Parse(total.InnerText.Replace('.', ',')) + decimal.Parse(descuento.InnerText.Replace('.', ','));
                    //CAMBIO SOLICITADO POR SANTI DONDE EL PRECIO UNITARIO ES IGUAL AL TOTAL + DECUENTO
                    if (comp.com_formato == 2)
                    {
                        pdfFormFieldsHeader.SetField("Precio totalRow" + linea.ToString(), totalrow.ToString());
                    }
                    else
                    {
                        pdfFormFieldsHeader.SetField("DescuentoRow" + linea.ToString(), descuento.InnerText);
                        pdfFormFieldsHeader.SetField("Precio totalRow" + linea.ToString(), total.InnerText);
                        pdfFormFieldsHeader.SetField("Precio totalRowB" + linea.ToString(), totalrow.ToString());
                    }
                }
                if (pos == "B")//HEAEDER
                {
                    pdfFormFieldsBody.SetField("Código PrincipalRow" + linea.ToString(), codigo.InnerText);
                    pdfFormFieldsBody.SetField("DescripciónRow" + linea.ToString(), strdescripción);
                    pdfFormFieldsBody.SetField("CantidadRow" + linea.ToString(), cantidad.InnerText);

                    pdfFormFieldsBody.SetField("Precio unitarioRow" + linea.ToString(), precio.InnerText);

                    decimal totalrow = decimal.Parse(total.InnerText.Replace('.', ',')) + decimal.Parse(descuento.InnerText.Replace('.', ','));
                    //CAMBIO SOLICITADO POR SANTI DONDE EL PRECIO UNITARIO ES IGUAL AL TOTAL + DECUENTO
                    if (comp.com_formato == 2)
                    {
                        pdfFormFieldsBody.SetField("Precio totalRow" + linea.ToString(), totalrow.ToString());
                    }
                    else
                    {
                        pdfFormFieldsBody.SetField("DescuentoRow" + linea.ToString(), descuento.InnerText);
                        pdfFormFieldsBody.SetField("Precio totalRow" + linea.ToString(), total.InnerText);
                        pdfFormFieldsBody.SetField("Precio totalRowB" + linea.ToString(), totalrow.ToString());
                    }
                }
                if (pos=="F")
                {
                    pdfFormFieldsFooter.SetField("Código PrincipalRow" + linea.ToString(), codigo.InnerText);
                    pdfFormFieldsFooter.SetField("DescripciónRow" + linea.ToString(), strdescripción);
                    pdfFormFieldsFooter.SetField("CantidadRow" + linea.ToString(), cantidad.InnerText);

                    pdfFormFieldsFooter.SetField("Precio unitarioRow" + linea.ToString(), precio.InnerText);

                    decimal totalrow = decimal.Parse(total.InnerText.Replace('.', ',')) + decimal.Parse(descuento.InnerText.Replace('.', ','));
                    //CAMBIO SOLICITADO POR SANTI DONDE EL PRECIO UNITARIO ES IGUAL AL TOTAL + DECUENTO
                    if (comp.com_formato == 2)
                    {
                        pdfFormFieldsFooter.SetField("Precio totalRow" + linea.ToString(), totalrow.ToString());
                    }
                    else
                    {
                        pdfFormFieldsFooter.SetField("DescuentoRow" + linea.ToString(), descuento.InnerText);
                        pdfFormFieldsFooter.SetField("Precio totalRow" + linea.ToString(), total.InnerText);
                        pdfFormFieldsFooter.SetField("Precio totalRowB" + linea.ToString(), totalrow.ToString());
                    }
                }


            }

            // add a barcode image
            PdfContentByte cb = pdfStamperHeader.GetOverContent(1);
            Barcode128 code = new Barcode128();
            code.Code = comp.com_numero;
            code.StartStopText = false;
            code.Extended = true;
            iTextSharp.text.Image barcode = code.CreateImageWithBarcode(cb, null, null);
            barcode.ScalePercent(100, 80);
            barcode.SetAbsolutePosition(327, 617);
            cb.AddImage(barcode);


            // report by reading values from completed PDF
            pdfStamperHeader.FormFlattening = false;
            // close the pdf
            pdfStamperHeader.Close();

            pdfStamperBody.FormFlattening = false;
            // close the pdf
            pdfStamperBody.Close();

            pdfStamperFooter.FormFlattening = false;
            // close the pdf
            pdfStamperFooter.Close();


            files.Add(pdfpathFooter);

            using (var output = new MemoryStream())
            {
                var document = new Document();
                var writer = new PdfCopy(document, output);
                document.Open();
                foreach (string item in files)
                {
                    var reader = new PdfReader(item);
                    int n = reader.NumberOfPages;
                    PdfImportedPage page;
                    for (int p = 1; p <= n; p++)
                    {
                        page = writer.GetImportedPage(reader, p);
                        writer.AddPage(page);
                    }
                }
                
                document.Close();
                File.WriteAllBytes(pdfpath, output.ToArray());

            }


            









           comp.com_empresa_key = comp.com_empresa;
            comp.com_numero_key = comp.com_numero;
            comp.com_pdf = pdfpath;
            ComprobanteBLL.Update(comp);
            return pdfpath;




        }
        
        /// <summary>
        /// GEnera el PDF para factura
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="comp"></param>
        /// <param name="formato"></param>
        /// <param name="arc"></param>
        /// <param name="path"></param>
        /// <param name="autoprint"></param>
        /// <param name="impresora"></param>
        /// <returns></returns>
        public static string SaveFACPDF(Empresa emp, Comprobante comp, Formato formato, Archivo arc, string path, bool autoprint, string impresora)
        {            
            //string pdfTemplate = path + @"\RIDEINMOT.pdf";
            string pdfTemplate = path + @"\\plantillas\\" + formato.for_pdf;
            //string pdfpath = path + @"\\temp\\" + comp.com_numero + ".pdf";
            string pdfpath = path + @"\\temp\\" + string.Format("{0}{1}-{2}-{3}_{4}.pdf", formato.for_tipo, comp.com_almacen, comp.com_pventa, comp.com_secuencia, comp.com_empresa);

            //NUEVO CONTROL PARA RIDES GRANDES 


            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(arc.arc_xml);


            XmlNode detalles = xmldoc.SelectSingleNode("/factura/detalles");
            if (detalles.ChildNodes.Count > 28)// SI ES MAYOR SE DEBE USAR OTRO RIDES
                return SaveBigFACPDF(emp, comp, formato, arc, path, autoprint, impresora);


            PdfReader pdfReader = new PdfReader(pdfTemplate);
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(pdfpath, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;
            //if (autoprint)
            //{
            //    var writer = pdfStamper.Writer;
            //    PdfAction js = PdfAction.JavaScript(GetAutoPrintJs(impresora), writer);
            //    writer.AddJavaScript(js);          //IMPRESION AUTOMATICA
            //}

            // set form pdfFormFields
            // The first worksheet and W-4 form


            //foreach (DictionaryEntry de in pdfFormFields.Fields.Count)            
            foreach (KeyValuePair<string, AcroFields.Item> kvp in pdfFormFields.Fields)
            {
                pdfFormFields.SetFieldProperty(kvp.Key, "textsize", 7f, null);
                pdfFormFields.SetFieldProperty(kvp.Key, "setfflags", PdfFormField.MK_CAPTION_LEFT, null);
                pdfFormFields.SetFieldProperty(kvp.Key, "setfflags", PdfFormField.FF_READ_ONLY, null);
                if (kvp.Key.IndexOf("DescripciónRow") >= 0)
                {
                    //pdfFormFields.SetFieldProperty(kvp.Key, "textsize", f, null);
                    pdfFormFields.SetFieldProperty(kvp.Key, "fflags", PdfFormField.FF_MULTILINE, null);
                }

            }


            pdfFormFields.SetField("ruc", emp.emp_ruc);
            pdfFormFields.SetField("autorizacion", comp.com_autorizacion);
            pdfFormFields.SetField("fechaautorizacion", comp.com_fechaautorizacion);
            //pdfFormFields.SetField("clave", comp.com_numero);
            pdfFormFields.SetField("numero", comp.com_almacen + "-" + comp.com_pventa + "-" + comp.com_secuencia);

            pdfFormFields.SetField("ruccli", comp.com_ruccliente);
            pdfFormFields.SetField("razon", comp.com_nombrecliente);
            pdfFormFields.SetField("fechaemision", comp.com_fecha.Value.ToShortDateString());









            XmlNode ambiente = xmldoc.SelectSingleNode("/factura/infoTributaria/ambiente");
            XmlNode emision = xmldoc.SelectSingleNode("/factura/infoTributaria/tipoEmision");

            XmlNode dirmatriz = xmldoc.SelectSingleNode("/factura/infoTributaria/dirMatriz");

            XmlNode dirsucursal = xmldoc.SelectSingleNode("/factura/infoFactura/dirEstablecimiento");
            XmlNode contribuyente = xmldoc.SelectSingleNode("/factura/infoFactura/contribuyenteEspecial");
            XmlNode obligado = xmldoc.SelectSingleNode("/factura/infoFactura/obligadoContabilidad");

            XmlNode infoadicional = xmldoc.SelectSingleNode("/factura/infoAdicional");

            string direccion = "";
            string telefono = "";
            string vendedor = "";
            string ciudad = "";
            string fpago = "";
            string transporte = "";
            string observacion = "";
            string compensacion = "";
            if (infoadicional != null)
            {
                foreach (XmlNode iteminfo in infoadicional.ChildNodes)
                {
                    if (iteminfo.Attributes["nombre"].Value == "Direccion")
                        direccion = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Telefono")
                        telefono = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Vendedor")
                        vendedor = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Ciudad")
                        ciudad = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Fpago")
                        fpago = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Transporte")
                        transporte = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Observacion")
                        observacion = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Compensado")
                        compensacion = iteminfo.InnerText;

                }

            }

            observacion += " " + compensacion;


            string strambiente = ambiente.InnerText == "1" ? "Pruebas" : "Producción";
            pdfFormFields.SetField("ambiente", strambiente);

            string stremision = emision.InnerText == "1" ? "Normal" : "Contingencia";
            pdfFormFields.SetField("emision", stremision);

            pdfFormFields.SetField("dirmatriz", dirmatriz.InnerText);
            pdfFormFields.SetField("dirsucursal", dirsucursal != null ? dirsucursal.InnerText : "");

            if (contribuyente!=null)
                pdfFormFields.SetField("contribuyente", contribuyente.InnerText);
            if (obligado!=null)
                pdfFormFields.SetField("obligado", obligado.InnerText);

            pdfFormFields.SetField("direccioncli", direccion);
            pdfFormFields.SetField("telefonocli", telefono);
            pdfFormFields.SetField("emailcli", comp.com_email);

            pdfFormFields.SetField("observacion", observacion);


            int valiva = 12;
            if (comp.com_fecha >= new DateTime(2016, 6, 1))
                valiva = 14;


            decimal subtotaliva = 0;

            decimal valoriva = 0;
            decimal subtotalice = 0;
            decimal valorice = 0;
            decimal subtotal0 = 0;


            XmlNode totalimpuestos = xmldoc.SelectSingleNode("/factura/infoFactura/totalConImpuestos");
            foreach (XmlNode totalimp in totalimpuestos.ChildNodes)
            {
                XmlNode codigo = totalimp.SelectSingleNode("codigo");
                XmlNode codigoporcentaje = totalimp.SelectSingleNode("codigoPorcentaje");
                XmlNode descuento = totalimp.SelectSingleNode("descuentoAdicional");
                XmlNode baseimp = totalimp.SelectSingleNode("baseImponible");
                XmlNode tarifa = totalimp.SelectSingleNode("tarifa");
                XmlNode valor = totalimp.SelectSingleNode("valor");


                if (codigo.InnerText == "2" && (codigoporcentaje.InnerText == "2" || codigoporcentaje.InnerText == "3"))//TARIFA 12 o 14
                {
                    subtotaliva += GetDecimal(baseimp.InnerText);
                    valoriva += GetDecimal(valor.InnerText);
                    if (codigoporcentaje.InnerText == "2")
                        valiva = 12;
                    if (codigoporcentaje.InnerText == "3")
                        valiva = 14;
                }
                if (codigo.InnerText == "2" && codigoporcentaje.InnerText == "0")//TARIFA 0
                {
                    subtotal0 += GetDecimal(baseimp.InnerText);
                }
                if (codigo.InnerText == "3")//TARIFA ICE
                {
                    subtotalice += GetDecimal(baseimp.InnerText);
                    valorice += GetDecimal(valor.InnerText);
                }
            }


            //decimal subtotal0 = 0;

            //NUEVO CAMBIO PARA INCLUIR COMPENSACIÓN EN CASO DE EXISTIR 01/08/2016
            decimal porcdescsolidario = 0;
            decimal descsolidario = 0;
            XmlNode compensaciones = xmldoc.SelectSingleNode("/factura/infoFactura/compensaciones");
            if (compensaciones != null)
            {

                XmlNode tarifacomp = compensaciones.SelectSingleNode("compensacion/tarifa");
                XmlNode valorcomp = compensaciones.SelectSingleNode("compensacion/valor");
                porcdescsolidario = GetDecimal(tarifacomp.InnerText);
                descsolidario = GetDecimal(valorcomp.InnerText);

            }


            decimal totaldesc = 0;
            XmlNode totaldescuento = xmldoc.SelectSingleNode("/factura/infoFactura/totalDescuento");
            totaldesc = GetDecimal(totaldescuento.InnerText);

            decimal totalcomp = 0;
            XmlNode importetotal = xmldoc.SelectSingleNode("/factura/infoFactura/importeTotal");
            totalcomp = GetDecimal(importetotal.InnerText);

            //////////PARA OBTENER el subtotal0 REAL CAMBIO 02/08/2016////////////////////////
            subtotal0 = totalcomp - (subtotaliva + valoriva);
            ///////////////////////////////////////////////////////////////////////////////

            ///AGREGA LAS FORMAS DE PAGO

            XmlNode pagos = xmldoc.SelectSingleNode("/factura/infoFactura/pagos");

            var serializer = new JavaScriptSerializer();
            List<FormaPago> formas = serializer.Deserialize<List<FormaPago>>(Constantes.GetParameter("formaspago"));
            string strformapago = "";
            string strvalorpago = "";
            if (pagos != null)
            {
                foreach (XmlNode pago in pagos.ChildNodes)
                {
                    XmlNode formaPago = pago.SelectSingleNode("formaPago");
                    FormaPago forma = formas.Find(delegate (FormaPago f) { return f.codigo == formaPago.InnerText; });
                    strformapago += forma.forma + Environment.NewLine;
                    XmlNode total = pago.SelectSingleNode("total");
                    strvalorpago += total.InnerText + Environment.NewLine;
                    XmlNode plazo = pago.SelectSingleNode("plazo");
                    XmlNode unidadTiempo = pago.SelectSingleNode("unidadTiempo");

                }
                pdfFormFields.SetField("FORMAPAGO", strformapago);
                pdfFormFields.SetField("VALORPAGO", strvalorpago);

            }
            ///////////////////////////////

            //CAMBIO SOLICITADO POR SANTI

            XmlNode totalsinimpuestos = xmldoc.SelectSingleNode("/factura/infoFactura/totalSinImpuestos");
            subtotaliva = decimal.Parse(totalsinimpuestos.InnerText.Replace('.', ',')) + totaldesc - subtotal0;

            decimal subtotal = subtotaliva + subtotal0;

            //NUEVO VALOR AGREGADO EL 08 junio 2015 - Solicitado por Santiago
            decimal subtotalivabi = subtotaliva - totaldesc;
            decimal subtotalbi = subtotal - totaldesc;
            ///
            ///
            ///
            ///
            pdfFormFields.SetField("Precio totalSUBTOTAL 12", subtotaliva.ToString());
            pdfFormFields.SetField("Precio totalSUBTOTAL 12BI", subtotalivabi.ToString());
            pdfFormFields.SetField("Precio totalSUBTOTAL 0", subtotal0.ToString());
            pdfFormFields.SetField("Precio totalSUBTOTAL", subtotal.ToString());
            pdfFormFields.SetField("Precio totalDESCUENTO", totaldesc.ToString());

            //DESCUENTO SOLIDARIO
            pdfFormFields.SetField("porcDS", porcdescsolidario.ToString());
            pdfFormFields.SetField("Precio totalDS", descsolidario.ToString());

            pdfFormFields.SetField("Precio totalSUBTOTALBI", subtotalbi.ToString());

            pdfFormFields.SetField("Precio totalICE", valorice.ToString());
            pdfFormFields.SetField("Precio totalIVA 12", valoriva.ToString());
            //pdfFormFields.SetField("Precio totalTRANSPORTE", "");
            pdfFormFields.SetField("Precio totalVALOR TOTAL", totalcomp.ToString());

            pdfFormFields.SetField("vendedor", vendedor);
            pdfFormFields.SetField("ciudad", ciudad);
            pdfFormFields.SetField("formapago", fpago);
            pdfFormFields.SetField("transporte", transporte);

            pdfFormFields.SetField("imp", valiva.ToString("0.00") + "%:");






            int linea = 0;
            foreach (XmlNode detalle in detalles.ChildNodes)
            {
                XmlNode codigo = detalle.SelectSingleNode("codigoPrincipal");
                XmlNode codaux = detalle.SelectSingleNode("codigoAuxiliar");
                XmlNode descripcion = detalle.SelectSingleNode("descripcion");
                XmlNode cantidad = detalle.SelectSingleNode("cantidad");
                XmlNode precio = detalle.SelectSingleNode("precioUnitario");
                XmlNode descuento = detalle.SelectSingleNode("descuento");
                XmlNode total = detalle.SelectSingleNode("precioTotalSinImpuesto");
                XmlNode adicionales = detalle.SelectSingleNode("detallesAdicionales");

                string strdescripción = descripcion.InnerText;
                if (comp.com_formato == 2 && adicionales != null)
                {
                    strdescripción = "";
                    foreach (XmlNode det in adicionales.ChildNodes)
                    {
                        strdescripción += det.Attributes["valor"].Value + Environment.NewLine;
                    }
                }


                linea++;
                pdfFormFields.SetField("Código PrincipalRow" + linea.ToString(), codigo.InnerText);
                pdfFormFields.SetField("DescripciónRow" + linea.ToString(), strdescripción);
                pdfFormFields.SetField("CantidadRow" + linea.ToString(), cantidad.InnerText);

                pdfFormFields.SetField("Precio unitarioRow" + linea.ToString(), precio.InnerText);

                decimal totalrow = decimal.Parse(total.InnerText.Replace('.', ',')) + decimal.Parse(descuento.InnerText.Replace('.', ','));
                //CAMBIO SOLICITADO POR SANTI DONDE EL PRECIO UNITARIO ES IGUAL AL TOTAL + DECUENTO
                if (comp.com_formato == 2)
                {
                    pdfFormFields.SetField("Precio totalRow" + linea.ToString(), totalrow.ToString());
                }
                else
                {
                    pdfFormFields.SetField("DescuentoRow" + linea.ToString(), descuento.InnerText);
                    pdfFormFields.SetField("Precio totalRow" + linea.ToString(), total.InnerText);
                    pdfFormFields.SetField("Precio totalRowB" + linea.ToString(), totalrow.ToString());
                }



            }

            // add a barcode image
            PdfContentByte cb = pdfStamper.GetOverContent(1);
            Barcode128 code = new Barcode128();
            code.Code = comp.com_numero;
            code.StartStopText = false;
            code.Extended = true;
            iTextSharp.text.Image barcode = code.CreateImageWithBarcode(cb, null, null);
            barcode.ScalePercent(100, 80);
            barcode.SetAbsolutePosition(327, 617);
            cb.AddImage(barcode);


            // report by reading values from completed PDF
            pdfStamper.FormFlattening = false;
            // close the pdf
            pdfStamper.Close();
            comp.com_empresa_key = emp.emp_codigo;
            comp.com_numero_key = comp.com_numero;
            comp.com_pdf = pdfpath;
            ComprobanteBLL.Update(comp);
            return pdfpath;


        }

        /// <summary>
        /// Genera el PDF de la Nota de Credito
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="numero"></param>
        /// <param name="path"></param>
        /// <param name="autoprint"></param>
        /// <param name="impresora"></param>
        /// <returns></returns>
        public static string SaveNCPDF(Empresa emp, Comprobante comp, Formato formato, Archivo arc, string path, bool autoprint, string impresora)
        {

            //string pdfTemplate = path + @"\RIDEINMOT.pdf";
            string pdfTemplate = path + @"\\plantillas\\" + formato.for_pdf;
            //string pdfpath = path + @"\\temp\\" + comp.com_numero + ".pdf";
            string pdfpath = path + @"\\temp\\" + string.Format("{0}{1}-{2}-{3}_{4}.pdf", formato.for_tipo, comp.com_almacen, comp.com_pventa, comp.com_secuencia, comp.com_empresa);


            //NUEVO CONTROL PARA RIDES GRANDES 


            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(arc.arc_xml);

            
            XmlNode detalles = xmldoc.SelectSingleNode("/notaCredito/detalles");
          
            PdfReader pdfReader = new PdfReader(pdfTemplate);
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(pdfpath, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;
            //if (autoprint)
            //{
            //    var writer = pdfStamper.Writer;
            //    PdfAction js = PdfAction.JavaScript(GetAutoPrintJs(impresora), writer);
            //    writer.AddJavaScript(js);          //IMPRESION AUTOMATICA
            //}

            // set form pdfFormFields
            // The first worksheet and W-4 form


            //foreach (DictionaryEntry de in pdfFormFields.Fields.Count)            
            foreach (KeyValuePair<string, AcroFields.Item> kvp in pdfFormFields.Fields)
            {
                pdfFormFields.SetFieldProperty(kvp.Key, "textsize", 7f, null);
                pdfFormFields.SetFieldProperty(kvp.Key, "setfflags", PdfFormField.MK_CAPTION_LEFT, null);
                pdfFormFields.SetFieldProperty(kvp.Key, "setfflags", PdfFormField.FF_READ_ONLY, null);
                if (kvp.Key.IndexOf("DescripciónRow") >= 0)
                {
                    //pdfFormFields.SetFieldProperty(kvp.Key, "textsize", f, null);
                    pdfFormFields.SetFieldProperty(kvp.Key, "fflags", PdfFormField.FF_MULTILINE, null);
                }

            }


            pdfFormFields.SetField("ruc", emp.emp_ruc);
            pdfFormFields.SetField("autorizacion", comp.com_numero);
            pdfFormFields.SetField("fechaautorizacion", comp.com_fechaautorizacion);
            //pdfFormFields.SetField("clave", comp.com_numero);
            pdfFormFields.SetField("numero", comp.com_almacen + "-" + comp.com_pventa + "-" + comp.com_secuencia);

            pdfFormFields.SetField("ruccli", comp.com_ruccliente);
            pdfFormFields.SetField("razon", comp.com_nombrecliente);
            pdfFormFields.SetField("fechaemision", comp.com_fecha.Value.ToString("dd/MM/yyyy"));


            XmlNode ambiente = xmldoc.SelectSingleNode("/notaCredito/infoTributaria/ambiente");
            XmlNode emision = xmldoc.SelectSingleNode("/notaCredito/infoTributaria/tipoEmision");
            XmlNode dirmatriz = xmldoc.SelectSingleNode("/notaCredito/infoTributaria/dirMatriz");

            
            XmlNode dirsucursal = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/dirEstablecimiento");
            XmlNode contribuyente = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/contribuyenteEspecial");
            XmlNode obligado = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/obligadoContabilidad");



            XmlNode infoadicional = xmldoc.SelectSingleNode("/notaCredito/infoAdicional");

            string direccion = "";
            string telefono = "";
            string vendedor = "";
            string ciudad = "";
            string fpago = "";
            string transporte = "";
            string observacion = "";
            string compensacion = "";
            if (infoadicional != null)
            {
                foreach (XmlNode iteminfo in infoadicional.ChildNodes)
                {
                    if (iteminfo.Attributes["nombre"].Value == "Direccion")
                        direccion = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Telefono")
                        telefono = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Vendedor")
                        vendedor = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Ciudad")
                        ciudad = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Fpago")
                        fpago = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Transporte")
                        transporte = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Observacion")
                        observacion = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Compensado")
                        compensacion = iteminfo.InnerText;

                }

            }

            observacion += " " + compensacion;


            string strambiente = ambiente.InnerText == "1" ? "Pruebas" : "Producción";
            pdfFormFields.SetField("ambiente", strambiente);

            string stremision = emision.InnerText == "1" ? "Normal" : "Contingencia";
            pdfFormFields.SetField("emision", stremision);

            pdfFormFields.SetField("dirmatriz", dirmatriz.InnerText);
            pdfFormFields.SetField("dirsucursal", dirsucursal != null ? dirsucursal.InnerText : "");

            if (contribuyente != null)
                pdfFormFields.SetField("contribuyente", contribuyente.InnerText);
            if (obligado != null)
                pdfFormFields.SetField("obligado", obligado.InnerText);

            

            pdfFormFields.SetField("direccioncli", direccion);
            pdfFormFields.SetField("telefonocli", telefono);
            pdfFormFields.SetField("emailcli", comp.com_email);

            pdfFormFields.SetField("observacion", observacion);


            int valiva = 12;
            if (comp.com_fecha >= new DateTime(2016, 6, 1))
                valiva = 14;


            decimal subtotaliva = 0;

            decimal valoriva = 0;
            decimal subtotalice = 0;
            decimal valorice = 0;
            decimal subtotal0 = 0;


            XmlNode totalimpuestos = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/totalConImpuestos");
            foreach (XmlNode totalimp in totalimpuestos.ChildNodes)
            {
                XmlNode codigo = totalimp.SelectSingleNode("codigo");
                XmlNode codigoporcentaje = totalimp.SelectSingleNode("codigoPorcentaje");
                XmlNode baseimp = totalimp.SelectSingleNode("baseImponible");
                XmlNode valor = totalimp.SelectSingleNode("valor");


                if (codigo.InnerText == "2" && (codigoporcentaje.InnerText == "2" || codigoporcentaje.InnerText == "3"))//TARIFA 12 o 14
                {
                    subtotaliva += GetDecimal(baseimp.InnerText);
                    valoriva += GetDecimal(valor.InnerText);
                    if (codigoporcentaje.InnerText == "2")
                        valiva = 12;
                    if (codigoporcentaje.InnerText == "3")
                        valiva = 14;
                }
                if (codigo.InnerText == "2" && codigoporcentaje.InnerText == "0")//TARIFA 0
                {
                    subtotal0 += GetDecimal(baseimp.InnerText);
                }
                if (codigo.InnerText == "3")//TARIFA ICE
                {
                    subtotalice += GetDecimal(baseimp.InnerText);
                    valorice += GetDecimal(valor.InnerText);
                }
            }


            //decimal subtotal0 = 0;

            //NUEVO CAMBIO PARA INCLUIR COMPENSACIÓN EN CASO DE EXISTIR 01/08/2016
            decimal porcdescsolidario = 0;
            decimal descsolidario = 0;
            XmlNode compensaciones = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/compensaciones");
            if (compensaciones != null)
            {

                XmlNode tarifacomp = compensaciones.SelectSingleNode("compensacion/tarifa");
                XmlNode valorcomp = compensaciones.SelectSingleNode("compensacion/valor");
                porcdescsolidario = GetDecimal(tarifacomp.InnerText);
                descsolidario = GetDecimal(valorcomp.InnerText);

            }


            
            decimal valmod = 0;
            XmlNode valormodificacion = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/valorModificacion");
            valmod = GetDecimal(valormodificacion.InnerText);

            decimal totsinimp = 0;
            XmlNode totalsinimpuestos = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/totalSinImpuestos");
            totsinimp = GetDecimal(totalsinimpuestos.InnerText);

                       
            decimal subtotal = subtotaliva + subtotal0;




            XmlNode numDocModificado  = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/numDocModificado");
            XmlNode fechaEmisionDocSustento = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/fechaEmisionDocSustento");
            XmlNode motivo = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/motivo");


            pdfFormFields.SetField("numfac", numDocModificado.InnerText);
            pdfFormFields.SetField("fechafac", fechaEmisionDocSustento.InnerText);
            pdfFormFields.SetField("motivo", motivo.InnerText);


            pdfFormFields.SetField("Precio totalSUBTOTAL IVA", subtotaliva.ToString());            
            pdfFormFields.SetField("Precio totalSUBTOTAL 0", subtotal0.ToString());
            pdfFormFields.SetField("Precio totalSUBTOTAL", subtotal.ToString());

            //DESCUENTO SOLIDARIO
            pdfFormFields.SetField("porcDS", porcdescsolidario.ToString());
            pdfFormFields.SetField("Precio totalDS", descsolidario.ToString());

            
            pdfFormFields.SetField("Precio totalICE", valorice.ToString());
            pdfFormFields.SetField("Precio totalIVA", valoriva.ToString());
            //pdfFormFields.SetField("Precio totalTRANSPORTE", "");
            pdfFormFields.SetField("Precio totalVALOR TOTAL", valmod.ToString());

            pdfFormFields.SetField("vendedor", vendedor);
            pdfFormFields.SetField("ciudad", ciudad);
            pdfFormFields.SetField("formapago", fpago);
            pdfFormFields.SetField("transporte", transporte);

            pdfFormFields.SetField("imp", valiva.ToString("0.00") + "%:");






            int linea = 0;
            foreach (XmlNode detalle in detalles.ChildNodes)
            {
                XmlNode codigo = detalle.SelectSingleNode("codigoInterno");
                XmlNode codaux = detalle.SelectSingleNode("codigoAdicional");
                XmlNode descripcion = detalle.SelectSingleNode("descripcion");
                XmlNode cantidad = detalle.SelectSingleNode("cantidad");
                XmlNode precio = detalle.SelectSingleNode("precioUnitario");
                XmlNode descuento = detalle.SelectSingleNode("descuento");
                XmlNode total = detalle.SelectSingleNode("precioTotalSinImpuesto");
                XmlNode adicionales = detalle.SelectSingleNode("detallesAdicionales");

                string strdescripción = descripcion.InnerText;
                if (comp.com_formato == 2 && adicionales != null)
                {
                    strdescripción = "";
                    foreach (XmlNode det in adicionales.ChildNodes)
                    {
                        strdescripción += det.Attributes["valor"].Value + Environment.NewLine;
                    }
                }


                linea++;
                pdfFormFields.SetField("Código PrincipalRow" + linea.ToString(), codigo.InnerText);
                pdfFormFields.SetField("DescripciónRow" + linea.ToString(), strdescripción);
                pdfFormFields.SetField("CantidadRow" + linea.ToString(), cantidad.InnerText);

                pdfFormFields.SetField("Precio unitarioRow" + linea.ToString(), precio.InnerText);

                decimal totalrow = decimal.Parse(total.InnerText.Replace('.', ',')) + decimal.Parse(descuento.InnerText.Replace('.', ','));
                //CAMBIO SOLICITADO POR SANTI DONDE EL PRECIO UNITARIO ES IGUAL AL TOTAL + DECUENTO
                if (comp.com_formato == 2)
                {
                    pdfFormFields.SetField("Precio totalRow" + linea.ToString(), totalrow.ToString());
                }
                else
                {
                    pdfFormFields.SetField("DescuentoRow" + linea.ToString(), descuento.InnerText);
                    pdfFormFields.SetField("Precio totalRow" + linea.ToString(), total.InnerText);
                    pdfFormFields.SetField("Precio totalRowB" + linea.ToString(), totalrow.ToString());
                }



            }

            // add a barcode image
            PdfContentByte cb = pdfStamper.GetOverContent(1);
            Barcode128 code = new Barcode128();
            code.Code = comp.com_numero;
            code.StartStopText = false;
            code.Extended = true;
            iTextSharp.text.Image barcode = code.CreateImageWithBarcode(cb, null, null);
            barcode.ScalePercent(100, 80);
            barcode.SetAbsolutePosition(327, 617);
            cb.AddImage(barcode);


            // report by reading values from completed PDF
            pdfStamper.FormFlattening = false;
            // close the pdf
            pdfStamper.Close();
            comp.com_empresa_key = emp.emp_codigo;
            comp.com_numero_key = comp.com_numero;
            comp.com_pdf = pdfpath;
            ComprobanteBLL.Update(comp);
            return pdfpath;


        }

        /// <summary>
        /// Genera el PDF de la Nota de Credito
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="numero"></param>
        /// <param name="path"></param>
        /// <param name="autoprint"></param>
        /// <param name="impresora"></param>
        /// <returns></returns>
        public static string SaveRETPDF(Empresa emp, Comprobante comp, Formato formato, Archivo arc, string path, bool autoprint, string impresora)
        {

            //string pdfTemplate = path + @"\RIDEINMOT.pdf";
            string pdfTemplate = path + @"\\plantillas\\" + formato.for_pdf;
            //string pdfpath = path + @"\\temp\\" + comp.com_numero + ".pdf";
            string pdfpath = path + @"\\temp\\" + string.Format("{0}{1}-{2}-{3}_{4}.pdf", formato.for_tipo, comp.com_almacen, comp.com_pventa, comp.com_secuencia, comp.com_empresa);
            //NUEVO CONTROL PARA RIDES GRANDES 


            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(arc.arc_xml);


            XmlNode detalles = xmldoc.SelectSingleNode("/comprobanteRetencion/impuestos");

            PdfReader pdfReader = new PdfReader(pdfTemplate);
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(pdfpath, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;
            //if (autoprint)
            //{
            //    var writer = pdfStamper.Writer;
            //    PdfAction js = PdfAction.JavaScript(GetAutoPrintJs(impresora), writer);
            //    writer.AddJavaScript(js);          //IMPRESION AUTOMATICA
            //}

            // set form pdfFormFields
            // The first worksheet and W-4 form


            //foreach (DictionaryEntry de in pdfFormFields.Fields.Count)            
            foreach (KeyValuePair<string, AcroFields.Item> kvp in pdfFormFields.Fields)
            {
                pdfFormFields.SetFieldProperty(kvp.Key, "textsize", 7f, null);
                pdfFormFields.SetFieldProperty(kvp.Key, "setfflags", PdfFormField.MK_CAPTION_LEFT, null);
                pdfFormFields.SetFieldProperty(kvp.Key, "setfflags", PdfFormField.FF_READ_ONLY, null);
                if (kvp.Key.IndexOf("DescripciónRow") >= 0)
                {
                    //pdfFormFields.SetFieldProperty(kvp.Key, "textsize", f, null);
                    pdfFormFields.SetFieldProperty(kvp.Key, "fflags", PdfFormField.FF_MULTILINE, null);
                }

            }


            pdfFormFields.SetField("ruc", emp.emp_ruc);
            //pdfFormFields.SetField("autorizacion", comp.com_autorizacion);
            pdfFormFields.SetField("autorizacion", comp.com_numero);
            pdfFormFields.SetField("fechaautorizacion", comp.com_fechaautorizacion);
            //pdfFormFields.SetField("clave", comp.com_numero);
            pdfFormFields.SetField("numero", comp.com_almacen + "-" + comp.com_pventa + "-" + comp.com_secuencia);

            pdfFormFields.SetField("ruccli", comp.com_ruccliente);
            pdfFormFields.SetField("razon", comp.com_nombrecliente);
            pdfFormFields.SetField("fechaemision", comp.com_fecha.Value.ToString("dd/MM/yyyy"));


            XmlNode ambiente = xmldoc.SelectSingleNode("/comprobanteRetencion/infoTributaria/ambiente");
            XmlNode emision = xmldoc.SelectSingleNode("/comprobanteRetencion/infoTributaria/tipoEmision");
            XmlNode dirmatriz = xmldoc.SelectSingleNode("/comprobanteRetencion/infoTributaria/dirMatriz");


            XmlNode dirsucursal = xmldoc.SelectSingleNode("/comprobanteRetencion/infoCompRetencion/dirEstablecimiento");
            XmlNode contribuyente = xmldoc.SelectSingleNode("/comprobanteRetencion/infoCompRetencion/contribuyenteEspecial");
            XmlNode obligado = xmldoc.SelectSingleNode("/comprobanteRetencion/infoCompRetencion/obligadoContabilidad");



            XmlNode infoadicional = xmldoc.SelectSingleNode("/comprobanteRetencion/infoAdicional");

            string direccion = "";
            string telefono = "";
            string vendedor = "";
            string ciudad = "";
            string fpago = "";
            string transporte = "";
            string observacion = "";
            string compensacion = "";
            if (infoadicional != null)
            {
                foreach (XmlNode iteminfo in infoadicional.ChildNodes)
                {
                    if (iteminfo.Attributes["nombre"].Value == "Direccion")
                        direccion = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Telefono")
                        telefono = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Vendedor")
                        vendedor = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Ciudad")
                        ciudad = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Fpago")
                        fpago = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Transporte")
                        transporte = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Observacion")
                        observacion = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Compensado")
                        compensacion = iteminfo.InnerText;

                }

            }

            observacion += " " + compensacion;


            string strambiente = ambiente.InnerText == "1" ? "Pruebas" : "Producción";
            pdfFormFields.SetField("ambiente", strambiente);

            string stremision = emision.InnerText == "1" ? "Normal" : "Contingencia";
            pdfFormFields.SetField("emision", stremision);

            pdfFormFields.SetField("dirmatriz", dirmatriz.InnerText);
            pdfFormFields.SetField("dirsucursal", dirsucursal != null ? dirsucursal.InnerText : "");

            if (contribuyente != null)
                pdfFormFields.SetField("contribuyente", contribuyente.InnerText);
            if (obligado != null)
                pdfFormFields.SetField("obligado", obligado.InnerText);



            pdfFormFields.SetField("direccioncli", direccion);
            pdfFormFields.SetField("telefonocli", telefono);
            pdfFormFields.SetField("emailcli", comp.com_email);
            pdfFormFields.SetField("Valor RetenidoTOTAL", comp.com_total.ToString());
            pdfFormFields.SetField("observacion", observacion);

            var serializer = new JavaScriptSerializer();
            List<TipoComprobantes> tipos = serializer.Deserialize<List<TipoComprobantes>>(Constantes.GetParameter("tiposcomprobante"));



            int linea = 0;
            foreach (XmlNode detalle in detalles.ChildNodes)
            {
                XmlNode codigo = detalle.SelectSingleNode("codigo");//1:Renta 2:IVA 6:ISD
                string impuesto = codigo.InnerText == "1" ? "RENTA" : codigo.InnerText == "2" ? "IVA" : "ISD";

                XmlNode codretencion = detalle.SelectSingleNode("codigoRetencion");
                XmlNode baseimp = detalle.SelectSingleNode("baseImponible");
                XmlNode porcret = detalle.SelectSingleNode("porcentajeRetener");
                XmlNode valret = detalle.SelectSingleNode("valorRetenido");
                XmlNode coddocret = detalle.SelectSingleNode("codDocSustento");
                XmlNode numdoc = detalle.SelectSingleNode("numDocSustento");
                XmlNode fechaemi = detalle.SelectSingleNode("fechaEmisionDocSustento");

                TipoComprobantes tc = tipos.Find(delegate (TipoComprobantes t) { return t.codigo == coddocret.InnerText; });


                linea++;
                pdfFormFields.SetField("ComprobanteRow" + linea.ToString(), tc.comprobante);
                pdfFormFields.SetField("NúmeroRow" + linea.ToString(), numdoc.InnerText);
                pdfFormFields.SetField("Fecha EmisiónRow" + linea.ToString(), fechaemi.InnerText);

                DateTime fe = DateTime.Parse(fechaemi.InnerText);
                pdfFormFields.SetField("Ejercicio FiscalRow" + linea.ToString(), fe.ToString("MM/yyyy"));
                pdfFormFields.SetField("Base Imponible para la RetenciónRow" + linea.ToString(), baseimp.InnerText);
                pdfFormFields.SetField("IMPUESTORow" + linea.ToString(), impuesto);
                pdfFormFields.SetField("Porcentaje RetenciónRow" + linea.ToString(), porcret.InnerText);
                pdfFormFields.SetField("Valor RetenidoRow" + linea.ToString(), valret.InnerText);
                

            }

            // add a barcode image
            PdfContentByte cb = pdfStamper.GetOverContent(1);
            Barcode128 code = new Barcode128();
            code.Code = comp.com_numero;
            code.StartStopText = false;
            code.Extended = true;
            iTextSharp.text.Image barcode = code.CreateImageWithBarcode(cb, null, null);
            barcode.ScalePercent(100, 80);
            barcode.SetAbsolutePosition(327, 617);
            cb.AddImage(barcode);


            // report by reading values from completed PDF
            pdfStamper.FormFlattening = false;
            // close the pdf
            pdfStamper.Close();
            comp.com_empresa_key = emp.emp_codigo;
            comp.com_numero_key = comp.com_numero;
            comp.com_pdf = pdfpath;
            ComprobanteBLL.Update(comp);
            return pdfpath;


        }

        /// <summary>
        /// Genera el PDF de la Guia de Remisión
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="numero"></param>
        /// <param name="path"></param>
        /// <param name="autoprint"></param>
        /// <param name="impresora"></param>
        /// <returns></returns>
        public static string SaveGREMPDF(Empresa emp, Comprobante comp, Formato formato, Archivo arc, string path, bool autoprint, string impresora)
        {

            //string pdfTemplate = path + @"\RIDEINMOT.pdf";
            string pdfTemplate = path + @"\\plantillas\\" + formato.for_pdf;
            //string pdfpath = path + @"\\temp\\" + comp.com_numero + ".pdf";
            string pdfpath = path + @"\\temp\\" + string.Format("{0}{1}-{2}-{3}_{4}.pdf", formato.for_tipo, comp.com_almacen, comp.com_pventa, comp.com_secuencia, comp.com_empresa);

            //NUEVO CONTROL PARA RIDES GRANDES 


            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(arc.arc_xml);


            //XmlNode detalles = xmldoc.SelectSingleNode("/comprobanteRetencion/impuestos");

            PdfReader pdfReader = new PdfReader(pdfTemplate);
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(pdfpath, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;
            //if (autoprint)
            //{
            //    var writer = pdfStamper.Writer;
            //    PdfAction js = PdfAction.JavaScript(GetAutoPrintJs(impresora), writer);
            //    writer.AddJavaScript(js);          //IMPRESION AUTOMATICA
            //}

            // set form pdfFormFields
            // The first worksheet and W-4 form


            //foreach (DictionaryEntry de in pdfFormFields.Fields.Count)            
            foreach (KeyValuePair<string, AcroFields.Item> kvp in pdfFormFields.Fields)
            {
                pdfFormFields.SetFieldProperty(kvp.Key, "textsize", 7f, null);
                pdfFormFields.SetFieldProperty(kvp.Key, "setfflags", PdfFormField.MK_CAPTION_LEFT, null);
                pdfFormFields.SetFieldProperty(kvp.Key, "setfflags", PdfFormField.FF_READ_ONLY, null);
                if (kvp.Key.IndexOf("DescripciónRow") >= 0)
                {
                    //pdfFormFields.SetFieldProperty(kvp.Key, "textsize", f, null);
                    pdfFormFields.SetFieldProperty(kvp.Key, "fflags", PdfFormField.FF_MULTILINE, null);
                }

            }


            pdfFormFields.SetField("ruc", emp.emp_ruc);
            pdfFormFields.SetField("autorizacion", comp.com_numero);
            pdfFormFields.SetField("fechaautorizacion", comp.com_fechaautorizacion);
            //pdfFormFields.SetField("clave", comp.com_numero);
            pdfFormFields.SetField("numero", comp.com_almacen + "-" + comp.com_pventa + "-" + comp.com_secuencia);

            pdfFormFields.SetField("ruccli", comp.com_ruccliente);
            pdfFormFields.SetField("razon", comp.com_nombrecliente);




            //pdfFormFields.SetField("fechaemision", comp.com_fecha.Value.ToShortDateString());


            XmlNode ambiente = xmldoc.SelectSingleNode("/guiaRemision/infoTributaria/ambiente");
            XmlNode emision = xmldoc.SelectSingleNode("/guiaRemision/infoTributaria/tipoEmision");
            XmlNode dirmatriz = xmldoc.SelectSingleNode("/guiaRemision/infoTributaria/dirMatriz");


            XmlNode dirsucursal = xmldoc.SelectSingleNode("/guiaRemision/infoGuiaRemision/dirEstablecimiento");
            XmlNode contribuyente = xmldoc.SelectSingleNode("/guiaRemision/infoGuiaRemision/contribuyenteEspecial");
            XmlNode obligado = xmldoc.SelectSingleNode("/guiaRemision/infoGuiaRemision/obligadoContabilidad");


            //DATOS TRANSPORTE
            XmlNode razontrans = xmldoc.SelectSingleNode("/guiaRemision/infoGuiaRemision/razonSocialTransportista");
            XmlNode ructrans = xmldoc.SelectSingleNode("/guiaRemision/infoGuiaRemision/rucTransportista");
            XmlNode fechaini = xmldoc.SelectSingleNode("/guiaRemision/infoGuiaRemision/fechaIniTransporte");
            XmlNode fechafin = xmldoc.SelectSingleNode("/guiaRemision/infoGuiaRemision/fechaFinTransporte");
            XmlNode placa = xmldoc.SelectSingleNode("/guiaRemision/infoGuiaRemision/placa");
            XmlNode partida = xmldoc.SelectSingleNode("/guiaRemision/infoGuiaRemision/dirPartida");


            pdfFormFields.SetField("ructrans", ructrans != null ? ructrans.InnerText : "");
            pdfFormFields.SetField("razontrans", razontrans != null ? razontrans.InnerText : "");
            pdfFormFields.SetField("placa", placa != null ? placa.InnerText : "");
            pdfFormFields.SetField("partida", partida != null ? partida.InnerText : "");
            pdfFormFields.SetField("fechainicio", fechaini != null ? fechaini.InnerText : "");
            pdfFormFields.SetField("fechafin", fechafin != null ? fechafin.InnerText : "");

            //DATOS ADICIONALES
            XmlNode infoadicional = xmldoc.SelectSingleNode("/guiaRemision/infoAdicional");

            string direccion = "";
            string telefono = "";
            string vendedor = "";
            string ciudad = "";
            string fpago = "";
            string transporte = "";
            string observacion = "";
            string compensacion = "";
            if (infoadicional != null)
            {
                foreach (XmlNode iteminfo in infoadicional.ChildNodes)
                {
                    if (iteminfo.Attributes["nombre"].Value == "Direccion")
                        direccion = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Telefono")
                        telefono = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Vendedor")
                        vendedor = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Ciudad")
                        ciudad = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Fpago")
                        fpago = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Transporte")
                        transporte = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Observacion")
                        observacion = iteminfo.InnerText;
                    if (iteminfo.Attributes["nombre"].Value == "Compensado")
                        compensacion = iteminfo.InnerText;

                }

            }

            observacion += " " + compensacion;


            string strambiente = ambiente.InnerText == "1" ? "Pruebas" : "Producción";
            pdfFormFields.SetField("ambiente", strambiente);

            string stremision = emision.InnerText == "1" ? "Normal" : "Contingencia";
            pdfFormFields.SetField("emision", stremision);

            pdfFormFields.SetField("dirmatriz", dirmatriz.InnerText);
            pdfFormFields.SetField("dirsucursal", dirsucursal != null ? dirsucursal.InnerText : "");

            if (contribuyente != null)
                pdfFormFields.SetField("contribuyente", contribuyente.InnerText);
            if (obligado != null)
                pdfFormFields.SetField("obligado", obligado.InnerText);


            
            pdfFormFields.SetField("direccioncli", direccion);
            pdfFormFields.SetField("telefonocli", telefono);
            pdfFormFields.SetField("emailcli", comp.com_email);

            //pdfFormFields.SetField("observacion", observacion);

            

            var serializer = new JavaScriptSerializer();
            List<TipoComprobantes> tipos = serializer.Deserialize<List<TipoComprobantes>>(Constantes.GetParameter("tiposcomprobante"));

            XmlNode destinatarios = xmldoc.SelectSingleNode("guiaRemision/destinatarios");

            foreach (XmlNode destin in destinatarios.ChildNodes)
            {
                XmlNode cirucdes = destin.SelectSingleNode("identificacionDestinatario");
                XmlNode razondes = destin.SelectSingleNode("razonSocialDestinatario");
                XmlNode dirdes = destin.SelectSingleNode("dirDestinatario");
                XmlNode motivo = destin.SelectSingleNode("motivoTraslado");                
                XmlNode docaduana = destin.SelectSingleNode("docAduaneroUnico");            
                XmlNode codalmacen = destin.SelectSingleNode("codEstabDestino");            
                XmlNode ruta = destin.SelectSingleNode("ruta");
                XmlNode coddoc = destin.SelectSingleNode("codDocSustento");
                XmlNode numdoc = destin.SelectSingleNode("numDocSustento");
                XmlNode autdoc = destin.SelectSingleNode("numAutDocSustento");                
                XmlNode fechadoc = destin.SelectSingleNode("fechaEmisionDocSustento");
                if (coddoc != null)
                {
                    TipoComprobantes tc = tipos.Find(delegate (TipoComprobantes t) { return t.codigo == coddoc.InnerText; });
                    pdfFormFields.SetField("tipo", tc.comprobante);
                }
                pdfFormFields.SetField("numerodoc", numdoc != null ? numdoc.InnerText : "");
                pdfFormFields.SetField("fechaemision", fechadoc != null ? fechadoc.InnerText : "");
                pdfFormFields.SetField("autorizaciondoc", autdoc != null ? autdoc.InnerText : "");
                pdfFormFields.SetField("motivo", motivo != null ? motivo.InnerText : "");
                pdfFormFields.SetField("destino", dirdes != null ? dirdes.InnerText : "");
                pdfFormFields.SetField("ruccli", cirucdes != null ? cirucdes.InnerText : "");
                pdfFormFields.SetField("razoncli", razondes != null ? razondes.InnerText : "");
                pdfFormFields.SetField("docaduana", docaduana != null ? docaduana.InnerText : "");
                pdfFormFields.SetField("codalmacen", codalmacen != null ? codalmacen.InnerText : "");
                pdfFormFields.SetField("ruta", ruta != null ? ruta.InnerText : "");

                XmlNode detalles = destin.SelectSingleNode("detalles");


                int linea = 0;
                foreach (XmlNode detalle in detalles.ChildNodes)
                {
                    XmlNode codinterno = detalle.SelectSingleNode("codigoInterno");//1:Renta 2:IVA 6:ISD
                    XmlNode codadicional= detalle.SelectSingleNode("codigoAdicional");
                    XmlNode descripcion = detalle.SelectSingleNode("descripcion");
                    XmlNode cantidad = detalle.SelectSingleNode("cantidad");
                    XmlNode detallesadicionales = detalle.SelectSingleNode("detallesAdicionales");


                    linea++;
                    pdfFormFields.SetField("CantidadRow" + linea.ToString(), cantidad != null ? cantidad.InnerText : "");
                    pdfFormFields.SetField("DescripciónRow" + linea.ToString(), descripcion != null ? descripcion.InnerText : "");

                    pdfFormFields.SetField("Código PrincipalRow" + linea.ToString(), codinterno != null ? codinterno.InnerText : "");
                    pdfFormFields.SetField("Código AuxiliarRow" + linea.ToString(), codadicional!= null ? codadicional.InnerText : "");

                }


           }




      

            // add a barcode image
            PdfContentByte cb = pdfStamper.GetOverContent(1);
            Barcode128 code = new Barcode128();
            code.Code = comp.com_numero;
            code.StartStopText = false;
            code.Extended = true;
            iTextSharp.text.Image barcode = code.CreateImageWithBarcode(cb, null, null);
            barcode.ScalePercent(100, 80);
            barcode.SetAbsolutePosition(327, 617);
            cb.AddImage(barcode);


            // report by reading values from completed PDF
            pdfStamper.FormFlattening = false;
            // close the pdf
            pdfStamper.Close();
            comp.com_empresa_key = emp.emp_codigo;
            comp.com_numero_key = comp.com_numero;
            comp.com_pdf = pdfpath;
            ComprobanteBLL.Update(comp);
            return pdfpath;


        }


        public static string CreatePDF(Comprobante comp)
        {
            string path = Constantes.GetParameter("pathfiles");
            Empresa emp = EmpresaBLL.GetByPK(new Empresa() { emp_codigo = comp.com_empresa, emp_codigo_key = comp.com_empresa });

            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            Formato formato = FormatoBLL.GetByPK(new Formato { for_empresa = comp.com_empresa, for_empresa_key = comp.com_empresa, for_codigo = comp.com_formato.Value, for_codigo_key = comp.com_formato.Value });

            if (formato.for_tipo == "FAC")
            {
                return CreateFAC(emp, comp,formato, path);
            }
            else if (formato.for_tipo == "RET")
            {
                return CreateRET(emp, comp, formato, path);
            }
            else if (formato.for_tipo == "NC")
            {
                return CreateNC(emp, comp, formato, path);
            }
            else if (formato.for_tipo == "ND")
            {
                return CreateND(emp, comp, formato, path);
            }
            else if (formato.for_tipo == "GREM")
            {
                return CreateGREM(emp, comp, formato, path);
            }
            else if (formato.for_tipo == "LC")
            {
                return CreateLC(emp, comp, formato, path);
            }
            else
            {
                return SavePDF(comp.com_empresa, comp.com_numero, path, false, "");
            }



        }

        public static string CreateFAC(Empresa emp, Comprobante comp, Formato formato, string path)
        {
            string file = path + @"\\temp\\" + string.Format("{0}{1}-{2}-{3}_{4}.pdf", formato.for_tipo, comp.com_almacen, comp.com_pventa, comp.com_secuencia, comp.com_empresa);            
            if (File.Exists(file))
                File.Delete(file);

            iTextSharp.text.Font _biggerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _biggerboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            iTextSharp.text.Font _bigFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _bigboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _boldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            iTextSharp.text.Font _smallstandardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _smallboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            float logoheight = 110;
            float size1 = 7;
            float size2 = 5;
            float size3 = 25;
            float lineheight = (float)1.6;
            int minreg = 8;

            Document document = new Document(PageSize.A4, 20, 20, 20, 20);
            if (formato.for_size == "A5")
            {
                document = new Document(PageSize.A5.Rotate(), 20, 20, 20, 20);
                logoheight = 80;
                size1 = 4;
                size2 = 2;
                size3 = 12;
                lineheight = (float)1.5;
                minreg = 2;
                _biggerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                _biggerboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                _bigFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                _bigboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                _boldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.BOLD, BaseColor.BLACK);


            }
            try
            {


                var pdfWriter = PdfWriter.GetInstance(document, new FileStream(file, FileMode.Create));

                pdfWriter.PageEvent = new ITextEvents();

                document.Open();



       

                //TABLA CABECERA

                float[] columnWidths = { 50, 50 };
                PdfPTable tcabecera = new PdfPTable(2);
                tcabecera.WidthPercentage = 100;
                tcabecera.SetWidths(columnWidths);

                PdfPTable tinfo = new PdfPTable(1);
                tinfo.WidthPercentage = 100;


                #region Logo
                //Agrega el logo


                string imageURL = path + "\\logos\\nologo.png";
                if (!string.IsNullOrEmpty(emp.emp_logo))
                    imageURL = path + "\\logos\\" + emp.emp_logo;
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imageURL);




                PdfPCell cinfo_logo = new PdfPCell(logo, true);
                cinfo_logo.BorderWidth = 0;
                cinfo_logo.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                cinfo_logo.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                cinfo_logo.PaddingBottom = size2;
                cinfo_logo.FixedHeight = logoheight;

                #endregion

                #region Información Factura

                PdfPCell cinfo_fac = new PdfPCell();
                cinfo_fac.Padding = 5;
                cinfo_fac.AddElement(new Paragraph(emp.emp_nombre, _boldFont));

                Phrase phrase = new Phrase();
                phrase.Add(new Chunk("Dir. Matriz:", _boldFont));
                phrase.Add(new Chunk(comp.com_direccionmatriz, _standardFont));
                phrase.SetLeading(0, lineheight);
                cinfo_fac.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Dir. Sucursal:", _boldFont));
                phrase.Add(new Chunk(comp.com_direccionsucursal, _standardFont));
                phrase.SetLeading(0, lineheight);
                cinfo_fac.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Contribuyente Especial Nro.:", _boldFont));
                phrase.Add(new Chunk(comp.com_contribuyente, _standardFont));
                phrase.SetLeading(0, lineheight);
                cinfo_fac.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Obligado a llevar contabilidad:", _boldFont));
                phrase.Add(new Chunk(comp.com_contabilidad, _standardFont));
                phrase.SetLeading(0, lineheight);

                cinfo_fac.AddElement(phrase);

                if (!string.IsNullOrEmpty(comp.com_regimenmicro))
                {
                    phrase = new Phrase();                    
                    phrase.Add(new Chunk(comp.com_regimenmicro, _smallboldFont));
                    cinfo_fac.AddElement(phrase);
                }

                //if (!string.IsNullOrEmpty(emp.emp_agenteret))
                if (!string.IsNullOrEmpty(comp.com_agenteret))
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk("Agente de Retención Resolución No.:", _boldFont));
                    phrase.Add(new Chunk(comp.com_agenteret, _standardFont));
                    cinfo_fac.AddElement(phrase);
                }

                if (!string.IsNullOrEmpty(comp.com_regimenrimpe))
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk(comp.com_regimenrimpe, _boldFont));                    
                    phrase.SetLeading(0, lineheight);
                    cinfo_fac.AddElement(phrase);
                }


                //cinfo_fac.AddElement(new Paragraph("Dir. Sucursal:", _boldFont));
                //cinfo_fac.AddElement(new Paragraph("Contribuyente Especial Nro.:", _boldFont));
                //cinfo_fac.AddElement(new Paragraph("Obligado a llevar contabilidad:", _boldFont));


                tinfo.AddCell(cinfo_logo);
                tinfo.AddCell(cinfo_fac);


                PdfPCell cInfoFac = new PdfPCell(tinfo);
                cInfoFac.BorderWidth = 0;
                cInfoFac.PaddingRight = 5;

                #endregion

                #region Información Tributaria

                PdfPCell cInfoTrib = new PdfPCell();
                cInfoTrib.Padding = 5;


                Paragraph paragraph = new Paragraph("RUC: " + emp.emp_ruc, _bigboldFont);
                //paragraph.SpacingAfter = size2;
                paragraph.SetLeading(0, lineheight);
                cInfoTrib.AddElement(paragraph);


                paragraph = new Paragraph("FACTURA", _biggerFont);
                //paragraph.SpacingAfter = size1;
                paragraph.SetLeading(0, lineheight);
                cInfoTrib.AddElement(paragraph);

                paragraph = new Paragraph("No.: " + comp.com_almacen + "-" + comp.com_pventa + "-" + comp.com_secuencia, _bigFont);
                //paragraph.SpacingAfter = size1;
                paragraph.SetLeading(0, lineheight);
                cInfoTrib.AddElement(paragraph);

                paragraph = new Paragraph("NÚMERO AUTORIZACIÓN:", _boldFont);
                paragraph.SetLeading(0, lineheight);
                cInfoTrib.AddElement(paragraph);


                if (!string.IsNullOrEmpty(comp.com_autorizacion))
                {
                    cInfoTrib.AddElement(new Paragraph(comp.com_autorizacion, _standardFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = size1 });
                    //if (comp.com_autorizacion != comp.com_numero)//NO SON OFFLINE
                    //{

                    phrase = new Phrase();
                    phrase.Add(new Chunk("FECHA Y HORA DE AUTORIZACIÓN:", _boldFont));
                    phrase.Add(new Chunk(comp.com_fechaautorizacion, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    cInfoTrib.AddElement(phrase);
                    //}
                }
                else
                    cInfoTrib.AddElement(new Paragraph(comp.com_numero, _standardFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 7 });

                phrase = new Phrase();
                phrase.Add(new Chunk("AMBIENTE:", _boldFont));
                phrase.Add(new Chunk(Enums.GetAmbiente(comp.com_ambiente), _standardFont));
                phrase.SetLeading(0, lineheight);
                cInfoTrib.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("EMISION:", _boldFont));
                phrase.Add(new Chunk(Enums.GetEmision(comp.com_emision), _standardFont));
                phrase.SetLeading(0, lineheight);
                cInfoTrib.AddElement(phrase);


                //cInfoTrib.AddElement(new Paragraph("EMISIÓN:", _boldFont));
                cInfoTrib.AddElement(new Paragraph("CLAVE DE ACCESO:", _boldFont) { SpacingBefore = size1, SpacingAfter = size2 });

                PdfContentByte cb = new PdfContentByte(pdfWriter);
                // add a barcode image
                Barcode128 code = new Barcode128();
                code.Code = comp.com_numero;
                code.StartStopText = false;
                code.Extended = true;                
                iTextSharp.text.Image barcode = code.CreateImageWithBarcode(cb, null, null);

                //barcode.ScaleAbsolute(256f,25f);
                //barcode.SetAbsolutePosition(327, 617);
                cInfoTrib.AddElement(barcode);

                #endregion




                
                //clRuc.BorderWidth = 0;
                tcabecera.AddCell(cInfoFac);
                tcabecera.AddCell(cInfoTrib);
                tcabecera.SpacingAfter = 5;
                document.Add(tcabecera);


                #region Tabla Datos Cliente

                PdfPTable tdatos = new PdfPTable(1);
                tdatos.WidthPercentage = 100;
                tdatos.SpacingAfter = 0;

                PdfPCell cdatos = new PdfPCell();
                cdatos.Padding = 5;

                float[] columnWidthsdatos = { 75, 25 };
                PdfPTable tdatosdet = new PdfPTable(2);
                tdatosdet.WidthPercentage = 100;
                tdatosdet.SetWidths(columnWidthsdatos);


                PdfPCell cdatosdet1 = new PdfPCell();
                cdatosdet1.Border = 0;
                cdatosdet1.Padding = 0;

                phrase = new Phrase();
                phrase.Add(new Chunk("Razón Social/Nombres y Apellidos:", _boldFont));
                phrase.Add(new Chunk(comp.com_nombrecliente, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet1.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Fecha Emisión:", _boldFont));
                phrase.Add(new Chunk(comp.com_fechastr, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet1.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Dirección:", _boldFont));
                phrase.Add(new Chunk(comp.com_direccioncliente, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet1.AddElement(phrase);

                //cdatosdet1.AddElement(new Paragraph("Razón Social/Nombres y Apellidos:", _boldFont));
                //cdatosdet1.AddElement(new Paragraph("Fecha Emisión:", _boldFont));
                //cdatosdet1.AddElement(new Paragraph("Dirección:", _boldFont));


                PdfPCell cdatosdet2 = new PdfPCell();
                cdatosdet2.Border = 0;


                string strid = "RUC / CI:";
                if (comp.com_tipoidcliente == "06")
                    strid = "RUC/CI/TAX PAYER:";


                phrase = new Phrase();
                phrase.Add(new Chunk(strid, _boldFont));
                phrase.Add(new Chunk(comp.com_ruccliente, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet2.AddElement(phrase);

          ;


                phrase = new Phrase();
                phrase.Add(new Chunk("Guía Remisión:", _boldFont));
                phrase.Add(new Chunk("", _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet2.AddElement(phrase);

                //cdatosdet2.AddElement(new Paragraph("RUC/CI:", _boldFont));
                //cdatosdet2.AddElement(new Paragraph("Guía Remisión:", _boldFont));

                tdatosdet.AddCell(cdatosdet1);
                tdatosdet.AddCell(cdatosdet2);


                cdatos.AddElement(tdatosdet);
                tdatos.AddCell(cdatos);
                tdatos.SpacingAfter = 5;
                document.Add(tdatos);
                ///
                #endregion

                #region Tabla Detalle 

                
                float[] columnWidthsdet = { 10, 10, 50, 10, 10, 10 };
                PdfPTable tdetalle = new PdfPTable(6);
                tdetalle.WidthPercentage = 100;
                tdetalle.SetWidths(columnWidthsdet);

                PdfPCell ccodigodet = new PdfPCell();
                ccodigodet.AddElement(new Paragraph("Código", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //ccodigodet.VerticalAlignment = Element.ALIGN_MIDDLE;
                //ccodigodet.FixedHeight = 25;
                //ccodigodet.HorizontalAlignment = Element.ALIGN_CENTER;



                PdfPCell ccantidaddet = new PdfPCell();
                ccantidaddet.AddElement(new Paragraph("Cant.", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //ccantidaddet.VerticalAlignment = Element.ALIGN_MIDDLE;

                PdfPCell cdescripciondet = new PdfPCell();
                cdescripciondet.AddElement(new Paragraph("Descripción", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //cdescripciondet.VerticalAlignment = Element.ALIGN_MIDDLE;

                PdfPCell cpreciodet = new PdfPCell();
                cpreciodet.AddElement(new Paragraph("Precio U.", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //cpreciodet.VerticalAlignment = Element.ALIGN_MIDDLE;

                PdfPCell cdescuentodet = new PdfPCell();
                cdescuentodet.AddElement(new Paragraph("Descuento", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //cdescuentodet.VerticalAlignment = Element.ALIGN_MIDDLE;

                PdfPCell ctotaldet = new PdfPCell();
                ctotaldet.AddElement(new Paragraph("Precio Total", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //ctotaldet.VerticalAlignment = Element.ALIGN_MIDDLE;

                tdetalle.AddCell(ccodigodet);
                tdetalle.AddCell(ccantidaddet);
                tdetalle.AddCell(cdescripciondet);
                tdetalle.AddCell(cpreciodet);
                tdetalle.AddCell(cdescuentodet);
                tdetalle.AddCell(ctotaldet);


                for (int i = 0; i < comp.detalle.Count; i++)
                {
                    Detalle det = comp.detalle[i];

                    //tdetalle.AddCell(new PdfPCell(new Paragraph(det.codigoaux, _standardFont)) { FixedHeight = 30, HorizontalAlignment = Element.ALIGN_CENTER });
                    tdetalle.AddCell(new PdfPCell(new Paragraph(det.codigo, _standardFont)) {  HorizontalAlignment = Element.ALIGN_CENTER });
                    tdetalle.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(det.cantidad), _standardFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                    string desc = det.descripcion;
                    if (!string.IsNullOrEmpty(det.adicional1))
                        desc += " " + det.adicional1;
                    if (!string.IsNullOrEmpty(det.adicional2))
                        desc += " " + det.adicional2;
                    if (!string.IsNullOrEmpty(det.adicional3))
                        desc += " " + det.adicional3;


                    
                    if (comp.com_formato == 2  && (det.adicional1!=null || det.adicional2 != null || det.adicional3 != null))
                    {
                        desc = "";
                        if (!string.IsNullOrEmpty(det.adicional1))
                            desc += " " + det.adicional1;
                        if (!string.IsNullOrEmpty(det.adicional2))
                            desc += " " + det.adicional2;
                        if (!string.IsNullOrEmpty(det.adicional3))
                            desc += " " + det.adicional3;
                    }





                    tdetalle.AddCell(new PdfPCell(new Paragraph(desc, _standardFont)));
                    tdetalle.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(det.precio), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    tdetalle.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(det.descuento), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    tdetalle.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(det.totalsinimp), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                

                    if (comp.detalle.Count < minreg)
                    {
                        for (int i = comp.detalle.Count; i < minreg; i++)
                        {
                            tdetalle.AddCell(new PdfPCell(new Paragraph("")) { FixedHeight = 15 });
                            tdetalle.AddCell(new PdfPCell(new Paragraph("")));
                            tdetalle.AddCell(new PdfPCell(new Paragraph("")));
                            tdetalle.AddCell(new PdfPCell(new Paragraph("")));
                            tdetalle.AddCell(new PdfPCell(new Paragraph("")));
                            tdetalle.AddCell(new PdfPCell(new Paragraph("")));
                        }
                    }
                

                //tdetalle.SpacingAfter = 5;
                document.Add(tdetalle);

                #endregion



                float[] columnWidthspie = { 60, 30, 10 };

                PdfPTable tdpie = new PdfPTable(3);
                tdpie.WidthPercentage = 100;
                tdpie.SetWidths(columnWidthspie);

                PdfPCell cpieinfoadi = new PdfPCell();
                cpieinfoadi.Border = 0;
                cpieinfoadi.PaddingTop = 5;
                cpieinfoadi.PaddingRight = 5;
                cpieinfoadi.Rowspan = 10;

                float[] columnWidthsADI = { 60, 40};
                PdfPTable tdinfoadi = new PdfPTable(2);
                tdinfoadi.WidthPercentage = 100;
                tdinfoadi.SetWidths(columnWidthsADI);
                //tdinfoadi.SpacingAfter = 5;
                tdinfoadi.SpacingAfter = size2;
                tdinfoadi.AddCell(new PdfPCell(new Paragraph("INFORMACIÓN ADICIONAL", _boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Colspan=2 });


              



                if (comp.com_adicional1==null)
                {

                    //phrase = new Phrase();
                    //phrase.Add(new Chunk("Forma de pago:", _boldFont));
                    //phrase.Add(new Chunk(comp.com_adicional3, _standardFont));
                    //tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthRight = 0 });

                    //phrase = new Phrase();
                    //phrase.Add(new Chunk("Ciudad cliente:", _boldFont));
                    //phrase.Add(new Chunk(comp.com_adicional2, _standardFont));
                    //tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthLeft = 0 });

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Teléfono:", _boldFont));
                    phrase.Add(new Chunk(comp.com_telefonocliente, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, Colspan = 2 });

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Email:", _boldFont));
                    phrase.Add(new Chunk(comp.com_email, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, Colspan = 2 });

                    if (!string.IsNullOrEmpty(comp.com_placa))
                    {
                        phrase = new Phrase();
                        phrase.Add(new Chunk("Placa:", _boldFont));
                        phrase.Add(new Chunk(comp.com_placa, _standardFont));
                        phrase.SetLeading(0, lineheight);
                        tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, Colspan = 2 });
                    }

                    foreach (string item in comp.com_adicionales)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            string[] arrayitem = item.Split('|');
                            phrase = new Phrase();
                            phrase.Add(new Chunk(arrayitem[0]+":", _boldFont));
                            phrase.Add(new Chunk(arrayitem[1], _standardFont));
                            phrase.SetLeading(0, lineheight);
                            tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, Colspan = 2 });
                        }


                    }
                    phrase = new Phrase();
                    phrase.Add(new Chunk("Observación:", _boldFont));
                    phrase.Add(new Chunk(comp.com_adicional5, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, PaddingBottom = 5, Colspan = 2 });
                }
                else
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk("Vendedor:", _boldFont));
                    phrase.Add(new Chunk(comp.com_adicional1, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthRight = 0});
                    

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Ciudad cliente:", _boldFont));
                    phrase.Add(new Chunk(comp.com_adicional2, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthLeft = 0 });

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Forma de pago:", _boldFont));
                    phrase.Add(new Chunk(comp.com_adicional3, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthRight = 0});


                    phrase = new Phrase();
                    phrase.Add(new Chunk("Transporte:", _boldFont));
                    phrase.Add(new Chunk(comp.com_adicional4, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthLeft = 0 });




                    phrase = new Phrase();
                    phrase.Add(new Chunk("Teléfono:", _boldFont));
                    phrase.Add(new Chunk(comp.com_telefonocliente, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, Colspan = 2 });



                    phrase = new Phrase();
                    phrase.Add(new Chunk("Email:", _boldFont));
                    phrase.Add(new Chunk(comp.com_email, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, Colspan = 2 });

                    if (!string.IsNullOrEmpty(comp.com_placa))
                    {
                        phrase = new Phrase();
                        phrase.Add(new Chunk("Placa:", _boldFont));
                        phrase.Add(new Chunk(comp.com_placa, _standardFont));
                        phrase.SetLeading(0, lineheight);
                        tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, Colspan = 2 });
                    }

                    foreach (string item in comp.com_adicionales)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            string[] arrayitem = item.Split('|');
                            phrase = new Phrase();
                            phrase.Add(new Chunk(arrayitem[0] + ":", _boldFont));
                            phrase.Add(new Chunk(arrayitem[1], _standardFont));
                            phrase.SetLeading(0, lineheight);
                            tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, Colspan = 2 });
                        }


                    }

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Observación:", _boldFont));
                    phrase.Add(new Chunk(comp.com_adicional5, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, PaddingBottom = 5, Colspan = 2 });
                }

               


                //tdinfoadi.AddCell(new PdfPCell(new Paragraph("Teléfono:", _boldFont)) { BorderWidthTop = 0, BorderWidthBottom = 0 });
                //tdinfoadi.AddCell(new PdfPCell(new Paragraph("Email:", _boldFont)) { BorderWidthTop = 0, BorderWidthBottom = 0 });
                //tdinfoadi.AddCell(new PdfPCell(new Paragraph("Observación:", _boldFont)) { BorderWidthTop = 0, PaddingBottom=5 });
                cpieinfoadi.AddElement(tdinfoadi);


                float[] columnWidthsformas = { 65, 15, 15, 15 };
                PdfPTable tdformas = new PdfPTable(4);
                tdformas.WidthPercentage = 100;
                tdformas.SetWidths(columnWidthsformas);

                tdformas.AddCell(new PdfPCell(new Paragraph("Forma de Pago", _boldFont)));
                tdformas.AddCell(new PdfPCell(new Paragraph("Valor", _boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                tdformas.AddCell(new PdfPCell(new Paragraph("Plazo", _boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                tdformas.AddCell(new PdfPCell(new Paragraph("Tiempo", _boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                for (int i = 0; i < comp.formas.Count; i++)
                {
                    Formapago fp = comp.formas[i];

                    tdformas.AddCell(new PdfPCell(new Paragraph(fp.forma, _standardFont)));
                    tdformas.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(fp.valor), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    tdformas.AddCell(new PdfPCell(new Paragraph(fp.plazo.ToString(), _standardFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    tdformas.AddCell(new PdfPCell(new Paragraph(fp.tiempo, _standardFont)));
                }




                cpieinfoadi.AddElement(tdformas);
                              

                tdpie.AddCell(cpieinfoadi);


                /*
                                PdfPCell cpietotales = new PdfPCell();
                                cpietotales.Border = 0;
                                cpietotales.Padding = 0;

                                float[] columnWidthstot = { 70, 30 };
                                PdfPTable tdtotales = new PdfPTable(2);
                                tdtotales.WidthPercentage = 100;
                                tdtotales.SetWidths(columnWidthstot);*/


                tdpie.AddCell(new PdfPCell(new Paragraph("SUBTOTAL " + comp.com_porciva + "%", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_subtotaliva), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                tdpie.AddCell(new PdfPCell(new Paragraph("SUBTOTAL 0%", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_subtotal0), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });


                tdpie.AddCell(new PdfPCell(new Paragraph("SUBTOTAL NO OBJETO DE IVA", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_subtotalnoiva), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                tdpie.AddCell(new PdfPCell(new Paragraph("SUBTOTAL EXENTO DE IVA", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_subtotalextiva), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                tdpie.AddCell(new PdfPCell(new Paragraph("SUBTOTAL SIN IMPUESTOS", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_subtotalsinimp), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                tdpie.AddCell(new PdfPCell(new Paragraph("TOTAL DESCUENTO", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_descuento), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                if (comp.com_ice.HasValue)
                {
                    if (comp.com_ice.Value > 0)
                    {
                        tdpie.AddCell(new PdfPCell(new Paragraph("ICE", _boldFont)));
                        tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_ice), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    }
                }



                tdpie.AddCell(new PdfPCell(new Paragraph("IVA " + comp.com_porciva + "%", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_iva), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                tdpie.AddCell(new PdfPCell(new Paragraph("PROPINA", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_propina), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                tdpie.AddCell(new PdfPCell(new Paragraph("VALOR TOTAL", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_total), _boldFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                tdpie.AddCell(new PdfPCell() { Border = 0 });
                tdpie.AddCell(new PdfPCell() { Border = 0 });


                document.Add(tdpie);

                //AGREGA HTML PIE  
                Paragraph ppie = new Paragraph();              
                foreach (IElement E in HTMLWorker.ParseToList(new StringReader(formato.for_pie.Replace("%total%", Functions.Conversiones.NumeroALetras(comp.com_total.ToString()))), null))                                        
                    ppie.Add(E);
                //ppie.SetLeading(0.0f, 1.0f);
                ppie.SetLeading(2f, 0f);
                document.Add(ppie);            
                document.Close();

            }
            catch (Exception ex)
            {
                document.Close();
            }

            return file;
        }

        /// <summary>
        /// TODAVIA NO ESTA IMPLEMENTANDO
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="comp"></param>
        /// <param name="formato"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string CreateRET(Empresa emp, Comprobante comp, Formato formato, string path)
        {
            string file = path + @"\\temp\\" + string.Format("{0}{1}-{2}-{3}_{4}.pdf", formato.for_tipo, comp.com_almacen, comp.com_pventa, comp.com_secuencia, comp.com_empresa);
            if (File.Exists(file))
                File.Delete(file);

            //Document document = new Document(PageSize.A5.Rotate(), 10, 10, 10, 10);


            iTextSharp.text.Font _biggerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _biggerboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            iTextSharp.text.Font _bigFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _bigboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _boldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            iTextSharp.text.Font _smallFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _smallboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.BOLD, BaseColor.BLACK);


            float logoheight = 110;
            float size1 = 7;
            float size2 = 5;
            float size3 = 25;
            float lineheight = (float)1.6;
            Document document = new Document(PageSize.A4, 20, 20, 20, 20);
            if (formato.for_size == "A5")
            {
                document = new Document(PageSize.A5.Rotate(), 20, 20, 20, 20);
                logoheight = 80;
                size1 = 4;
                size2 = 2;
                size3 = 12;
                lineheight = (float)1.5;
                _biggerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                _biggerboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                _bigFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                _bigboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                _boldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.BOLD, BaseColor.BLACK);


            }


            try
            {


                var pdfWriter = PdfWriter.GetInstance(document, new FileStream(file, FileMode.Create));

                pdfWriter.PageEvent = new ITextEvents();

                document.Open();



            
                //TABLA CABECERA

                float[] columnWidths = { 50, 50 };
                PdfPTable tcabecera = new PdfPTable(2);
                tcabecera.WidthPercentage = 100;
                tcabecera.SetWidths(columnWidths);

                PdfPTable tinfo = new PdfPTable(1);
                tinfo.WidthPercentage = 100;


                #region Logo
                //Agrega el logo


                string imageURL = path + "\\logos\\nologo.png";
                if (!string.IsNullOrEmpty(emp.emp_logo))
                    imageURL = path + "\\logos\\" + emp.emp_logo;
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imageURL);




                PdfPCell cinfo_logo = new PdfPCell(logo, true);
                cinfo_logo.BorderWidth = 0;
                cinfo_logo.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                cinfo_logo.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                cinfo_logo.PaddingBottom = size2;
                cinfo_logo.FixedHeight = logoheight;

                #endregion

                #region Información Retención

                PdfPCell cinfo_fac = new PdfPCell();
                cinfo_fac.Padding = 5;
                cinfo_fac.AddElement(new Paragraph(emp.emp_nombre, _boldFont));

                Phrase phrase = new Phrase();
                phrase.Add(new Chunk("Dir. Matriz:", _boldFont));
                phrase.Add(new Chunk(comp.com_direccionmatriz, _standardFont));
                phrase.SetLeading(0, lineheight);
                cinfo_fac.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Dir. Sucursal:", _boldFont));
                phrase.Add(new Chunk(comp.com_direccionsucursal, _standardFont));
                phrase.SetLeading(0, lineheight);
                cinfo_fac.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Contribuyente Especial Nro.:", _boldFont));
                phrase.Add(new Chunk(comp.com_contribuyente, _standardFont));
                phrase.SetLeading(0, lineheight);
                cinfo_fac.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Obligado a llevar contabilidad:", _boldFont));
                phrase.Add(new Chunk(comp.com_contabilidad, _standardFont));
                phrase.SetLeading(0, lineheight);
                cinfo_fac.AddElement(phrase);

                if (!string.IsNullOrEmpty(comp.com_regimenmicro))
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk(comp.com_regimenmicro, _smallboldFont));
                    cinfo_fac.AddElement(phrase);
                }
                
                if (!string.IsNullOrEmpty(comp.com_agenteret))
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk("Agente de Retención Resolución No.:", _boldFont));
                    phrase.Add(new Chunk(comp.com_agenteret, _standardFont));
                    cinfo_fac.AddElement(phrase);
                }


                if (!string.IsNullOrEmpty(comp.com_regimenrimpe))
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk(comp.com_regimenrimpe, _boldFont));
                    phrase.SetLeading(0, lineheight);
                    cinfo_fac.AddElement(phrase);
                }

                //cinfo_fac.AddElement(new Paragraph("Dir. Sucursal:", _boldFont));
                //cinfo_fac.AddElement(new Paragraph("Contribuyente Especial Nro.:", _boldFont));
                //cinfo_fac.AddElement(new Paragraph("Obligado a llevar contabilidad:", _boldFont));


                tinfo.AddCell(cinfo_logo);
                tinfo.AddCell(cinfo_fac);


                PdfPCell cInfoFac = new PdfPCell(tinfo);
                cInfoFac.BorderWidth = 0;
                cInfoFac.PaddingRight = 5;

                #endregion

                #region Información Tributaria

                PdfPCell cInfoTrib = new PdfPCell();
                cInfoTrib.Padding = 5;


                Paragraph paragraph = new Paragraph("RUC: " + emp.emp_ruc, _bigboldFont);
                //paragraph.SpacingAfter = size2;
                paragraph.SetLeading(0, lineheight);
                cInfoTrib.AddElement(paragraph);


                paragraph = new Paragraph("COMPROBANTE DE RETENCIÓN", _biggerFont);
                //paragraph.SpacingAfter = size1;
                paragraph.SetLeading(0, lineheight);
                cInfoTrib.AddElement(paragraph);

                paragraph = new Paragraph("No.: " + comp.com_almacen + "-" + comp.com_pventa + "-" + comp.com_secuencia, _bigFont);
                //paragraph.SpacingAfter = size1;
                paragraph.SetLeading(0, lineheight);
                cInfoTrib.AddElement(paragraph);

                paragraph = new Paragraph("NÚMERO AUTORIZACIÓN:", _boldFont);
                paragraph.SetLeading(0, lineheight);
                cInfoTrib.AddElement(paragraph);


                if (!string.IsNullOrEmpty(comp.com_autorizacion))
                {
                    cInfoTrib.AddElement(new Paragraph(comp.com_autorizacion, _standardFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = size1 });
                    //if (comp.com_autorizacion != comp.com_numero)//NO SON OFFLINE
                    //{

                        phrase = new Phrase();
                        phrase.Add(new Chunk("FECHA Y HORA DE AUTORIZACIÓN:", _boldFont));
                        phrase.Add(new Chunk(comp.com_fechaautorizacion, _standardFont));
                        phrase.SetLeading(0, lineheight);
                        cInfoTrib.AddElement(phrase);
                    //}
                }
                else
                     cInfoTrib.AddElement(new Paragraph(comp.com_numero, _standardFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 7 });

                phrase = new Phrase();
                phrase.Add(new Chunk("AMBIENTE:", _boldFont));
                phrase.Add(new Chunk(Enums.GetAmbiente(comp.com_ambiente), _standardFont));
                phrase.SetLeading(0, lineheight);
                cInfoTrib.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("EMISION:", _boldFont));
                phrase.Add(new Chunk(Enums.GetEmision(comp.com_emision), _standardFont));
                phrase.SetLeading(0, lineheight);
                cInfoTrib.AddElement(phrase);


                //cInfoTrib.AddElement(new Paragraph("EMISIÓN:", _boldFont));
                cInfoTrib.AddElement(new Paragraph("CLAVE DE ACCESO:", _boldFont) { SpacingBefore = size1, SpacingAfter = size2 });

                PdfContentByte cb = new PdfContentByte(pdfWriter);
                // add a barcode image
                Barcode128 code = new Barcode128();
                code.Code = comp.com_numero;
                code.StartStopText = false;
                code.Extended = true;
                iTextSharp.text.Image barcode = code.CreateImageWithBarcode(cb, null, null);
                //barcode.ScalePercent(100, 80);
                //barcode.SetAbsolutePosition(327, 617);
                cInfoTrib.AddElement(barcode);

                #endregion
                //clRuc.BorderWidth = 0;
                tcabecera.AddCell(cInfoFac);
                tcabecera.AddCell(cInfoTrib);
                tcabecera.SpacingAfter = size2;
                document.Add(tcabecera);


                #region Tabla Datos Cliente

                PdfPTable tdatos = new PdfPTable(1);
                tdatos.WidthPercentage = 100;                
                tdatos.SpacingAfter = 0;                

                PdfPCell cdatos = new PdfPCell();
                cdatos.Padding = 5;                

                float[] columnWidthsdatos = { 75, 25 };
                PdfPTable tdatosdet = new PdfPTable(2);
                tdatosdet.WidthPercentage = 100;
                tdatosdet.SetWidths(columnWidthsdatos);
               

                PdfPCell cdatosdet1 = new PdfPCell();
                cdatosdet1.Border = 0;
                cdatosdet1.Padding = 0;
                

                phrase = new Phrase();
                phrase.Add(new Chunk("Razón Social/Nombres y Apellidos:", _boldFont));
                phrase.Add(new Chunk(comp.com_nombrecliente, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet1.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Fecha Emisión:", _boldFont));
                phrase.Add(new Chunk(comp.com_fechastr, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet1.AddElement(phrase);
                

                PdfPCell cdatosdet2 = new PdfPCell();
                cdatosdet2.Border = 0;

                phrase = new Phrase();
                phrase.Add(new Chunk("RUC / CI:", _boldFont));
                phrase.Add(new Chunk(comp.com_ruccliente, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet2.AddElement(phrase);
               

                //cdatosdet2.AddElement(new Paragraph("RUC/CI:", _boldFont));
                //cdatosdet2.AddElement(new Paragraph("Guía Remisión:", _boldFont));

                tdatosdet.AddCell(cdatosdet1);
                tdatosdet.AddCell(cdatosdet2);


                cdatos.AddElement(tdatosdet);
                tdatos.AddCell(cdatos);
                tdatos.SpacingAfter = size2;
                document.Add(tdatos);
                ///
                #endregion

                #region Tabla Detalle 

                float[] columnWidthsdet = { 15, 20, 10, 10, 15, 10, 10, 10 };
                PdfPTable tdetalle = new PdfPTable(8);
                tdetalle.WidthPercentage = 100;
                tdetalle.SetWidths(columnWidthsdet);

                PdfPCell ccomprobantedet = new PdfPCell();
                ccomprobantedet.AddElement(new Paragraph("Comprobante", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //ccodigodet.VerticalAlignment = Element.ALIGN_MIDDLE;
                //ccodigodet.HorizontalAlignment = Element.ALIGN_CENTER;



                PdfPCell cnumerodet = new PdfPCell();
                cnumerodet.AddElement(new Paragraph("Número", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //ccantidaddet.VerticalAlignment = Element.ALIGN_MIDDLE;

                PdfPCell cfechaemidet = new PdfPCell();
                cfechaemidet.AddElement(new Paragraph("Fecha Emisión", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //cdescripciondet.VerticalAlignment = Element.ALIGN_MIDDLE;

                PdfPCell cejerciciodet = new PdfPCell();
                cejerciciodet.AddElement(new Paragraph("Ejercicio Fiscal", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //cpreciodet.VerticalAlignment = Element.ALIGN_MIDDLE;

                PdfPCell cbaseimpdet = new PdfPCell();
                cbaseimpdet.AddElement(new Paragraph("Base Imponible para Retención", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //cdescuentodet.VerticalAlignment = Element.ALIGN_MIDDLE;

                PdfPCell cimpuestodet = new PdfPCell();
                cimpuestodet.AddElement(new Paragraph("Impuesto", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //ctotaldet.VerticalAlignment = Element.ALIGN_MIDDLE;


                PdfPCell cporcretdet = new PdfPCell();
                cporcretdet.AddElement(new Paragraph("Porcentaje Retención", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //cporcretdet.VerticalAlignment = Element.ALIGN_MIDDLE;

                PdfPCell cvalretdet = new PdfPCell();
                cvalretdet.AddElement(new Paragraph("Valor Retenido", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //cvalretdet.VerticalAlignment = Element.ALIGN_MIDDLE;



                tdetalle.AddCell(ccomprobantedet);
                tdetalle.AddCell(cnumerodet);
                tdetalle.AddCell(cfechaemidet);
                tdetalle.AddCell(cejerciciodet);
                tdetalle.AddCell(cbaseimpdet);
                tdetalle.AddCell(cimpuestodet);
                tdetalle.AddCell(cporcretdet);
                tdetalle.AddCell(cvalretdet);
                var serializer = new JavaScriptSerializer();
                List<TipoComprobantes> tipos = serializer.Deserialize<List<TipoComprobantes>>(Constantes.GetParameter("tiposcomprobante"));

                for (int i = 0; i < comp.detalle.Count; i++)
                {
                    Detalle det = comp.detalle[i];

                    TipoComprobantes tc = tipos.Find(delegate (TipoComprobantes t) { return t.codigo == det.codigodocsustento; });
                    string impuesto = det.codigo == "1" ? "RENTA" : det.codigo == "2" ? "IVA" : "ISD";


                    //tdetalle.AddCell(new PdfPCell(new Paragraph(det.codigoaux, _standardFont)) { FixedHeight = 30, HorizontalAlignment = Element.ALIGN_CENTER });
                    tdetalle.AddCell(new PdfPCell(new Paragraph(tc.comprobante, _standardFont)) { FixedHeight = 15, HorizontalAlignment = Element.ALIGN_CENTER });
                    tdetalle.AddCell(new PdfPCell(new Paragraph(det.numdocsustento, _standardFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    tdetalle.AddCell(new PdfPCell(new Paragraph(det.fechaemisiondocsustento, _standardFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    tdetalle.AddCell(new PdfPCell(new Paragraph(comp.com_periodofiscal, _standardFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    tdetalle.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(det.baseimponible), _standardFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    tdetalle.AddCell(new PdfPCell(new Paragraph(impuesto, _standardFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    tdetalle.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(det.porcentajeretener), _standardFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    tdetalle.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(det.valorretenido), _standardFont)) { HorizontalAlignment = Element.ALIGN_CENTER });


              
                }

                //if (comp.detalle.Count < 6)
                //{
                //    for (int i = comp.detalle.Count; i < 6; i++)
                //    {
                //        tdetalle.AddCell(new PdfPCell(new Paragraph("")) { FixedHeight = 15 });
                //        tdetalle.AddCell(new PdfPCell(new Paragraph("")));
                //        tdetalle.AddCell(new PdfPCell(new Paragraph("")));
                //        tdetalle.AddCell(new PdfPCell(new Paragraph("")));
                //        tdetalle.AddCell(new PdfPCell(new Paragraph("")));
                //        tdetalle.AddCell(new PdfPCell(new Paragraph("")));
                //        tdetalle.AddCell(new PdfPCell(new Paragraph("")));
                //        tdetalle.AddCell(new PdfPCell(new Paragraph("")));
                //    }
                //}

                //tdetalle.SpacingAfter = 5;
                document.Add(tdetalle);

                #endregion



                float[] columnWidthspie = { 80, 10, 10 };

                PdfPTable tdpie = new PdfPTable(3);
                tdpie.WidthPercentage = 100;
                tdpie.SetWidths(columnWidthspie);                      
                tdpie.AddCell(new PdfPCell(new Paragraph("")) { BorderWidth=0 });
                tdpie.AddCell(new PdfPCell(new Paragraph("TOTAL: ", _boldFont)) { FixedHeight = 15, HorizontalAlignment = Element.ALIGN_RIGHT, BorderWidth=0});
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_total), _boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });


                document.Add(tdpie);

                PdfPTable tdinfoadi = new PdfPTable(1);
                tdinfoadi.WidthPercentage = 80;
                tdinfoadi.SpacingAfter = size2;
                tdinfoadi.AddCell(new PdfPCell(new Paragraph("INFORMACIÓN ADICIONAL", _boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                tdinfoadi.HorizontalAlignment = Element.ALIGN_LEFT;

                phrase = new Phrase();
                phrase.Add(new Chunk("Dirección:", _boldFont));
                phrase.Add(new Chunk(comp.com_adicional1, _standardFont));
                phrase.SetLeading(0, lineheight);
                tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0 });


                phrase = new Phrase();
                phrase.Add(new Chunk("Teléfono:", _boldFont));
                phrase.Add(new Chunk(comp.com_telefonocliente, _standardFont));
                phrase.SetLeading(0, lineheight);
                tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0 });

                phrase = new Phrase();
                phrase.Add(new Chunk("Email:", _boldFont));
                phrase.Add(new Chunk(comp.com_email, _standardFont));
                phrase.SetLeading(0, lineheight);
                tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0 });

                document.Add(tdinfoadi);

                //AGREGA HTML PIE  
                Paragraph ppie = new Paragraph();
                foreach (IElement E in HTMLWorker.ParseToList(new StringReader(formato.for_pie), null))
                    ppie.Add(E);
                ppie.SetLeading(0.0f, 1.0f);
                document.Add(ppie);
                document.Close();

            }
            catch (Exception ex)
            {
                document.Close();
            }

            return file;
        }



        public static string CreateNC(Empresa emp, Comprobante comp, Formato formato, string path)
        {
            string file = path + @"\\temp\\" + string.Format("{0}{1}-{2}-{3}_{4}.pdf", formato.for_tipo, comp.com_almacen, comp.com_pventa, comp.com_secuencia, comp.com_empresa);
            if (File.Exists(file))
                File.Delete(file);

            //Document document = new Document(PageSize.A5.Rotate(), 10, 10, 10, 10);


            iTextSharp.text.Font _biggerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _biggerboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            iTextSharp.text.Font _bigFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _bigboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _boldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            iTextSharp.text.Font _smallFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _smallboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            float logoheight = 110;
            float size1 = 7;
            float size2 = 5;
            float size3 = 25;
            float lineheight = (float)1.6;
            Document document = new Document(PageSize.A4, 20, 20, 20, 20);
            if (formato.for_size == "A5")
            {
                document = new Document(PageSize.A5.Rotate(), 20, 20, 20, 20);
                logoheight = 80;
                size1 = 4;
                size2 = 2;
                size3 = 12;
                lineheight = (float)1.5;
                _biggerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                _biggerboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                _bigFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                _bigboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                _boldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.BOLD, BaseColor.BLACK);


            }


            try
            {


                var pdfWriter = PdfWriter.GetInstance(document, new FileStream(file, FileMode.Create));

                pdfWriter.PageEvent = new ITextEvents();

                document.Open();




                //TABLA CABECERA

                float[] columnWidths = { 50, 50 };
                PdfPTable tcabecera = new PdfPTable(2);
                tcabecera.WidthPercentage = 100;
                tcabecera.SetWidths(columnWidths);

                PdfPTable tinfo = new PdfPTable(1);
                tinfo.WidthPercentage = 100;


                #region Logo
                //Agrega el logo


                string imageURL = path + "\\logos\\nologo.png";
                if (!string.IsNullOrEmpty(emp.emp_logo))
                    imageURL = path + "\\logos\\" + emp.emp_logo;
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imageURL);




                PdfPCell cinfo_logo = new PdfPCell(logo, true);
                cinfo_logo.BorderWidth = 0;
                cinfo_logo.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                cinfo_logo.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                cinfo_logo.PaddingBottom = size2;
                cinfo_logo.FixedHeight = logoheight;

                #endregion

                #region Información NC

                PdfPCell cinfo_fac = new PdfPCell();
                cinfo_fac.Padding = 5;
                cinfo_fac.AddElement(new Paragraph(emp.emp_nombre, _boldFont));

                Phrase phrase = new Phrase();
                phrase.Add(new Chunk("Dir. Matriz:", _boldFont));
                phrase.Add(new Chunk(comp.com_direccionmatriz, _standardFont));
                phrase.SetLeading(0, lineheight);
                cinfo_fac.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Dir. Sucursal:", _boldFont));
                phrase.Add(new Chunk(comp.com_direccionsucursal, _standardFont));
                phrase.SetLeading(0, lineheight);
                cinfo_fac.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Contribuyente Especial Nro.:", _boldFont));
                phrase.Add(new Chunk(comp.com_contribuyente, _standardFont));
                phrase.SetLeading(0, lineheight);
                cinfo_fac.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Obligado a llevar contabilidad:", _boldFont));
                phrase.Add(new Chunk(comp.com_contabilidad, _standardFont));
                phrase.SetLeading(0, lineheight);
                cinfo_fac.AddElement(phrase);



                if (!string.IsNullOrEmpty(comp.com_regimenmicro))
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk(comp.com_regimenmicro, _smallboldFont));
                    cinfo_fac.AddElement(phrase);
                }

                if (!string.IsNullOrEmpty(comp.com_agenteret))
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk("Agente de Retención Resolución No.:", _boldFont));
                    phrase.Add(new Chunk(comp.com_agenteret, _standardFont));
                    cinfo_fac.AddElement(phrase);
                }
                if (!string.IsNullOrEmpty(comp.com_regimenrimpe))
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk(comp.com_regimenrimpe, _boldFont));
                    phrase.SetLeading(0, lineheight);
                    cinfo_fac.AddElement(phrase);
                }

                //cinfo_fac.AddElement(new Paragraph("Dir. Sucursal:", _boldFont));
                //cinfo_fac.AddElement(new Paragraph("Contribuyente Especial Nro.:", _boldFont));
                //cinfo_fac.AddElement(new Paragraph("Obligado a llevar contabilidad:", _boldFont));


                tinfo.AddCell(cinfo_logo);
                tinfo.AddCell(cinfo_fac);


                PdfPCell cInfoFac = new PdfPCell(tinfo);
                cInfoFac.BorderWidth = 0;
                cInfoFac.PaddingRight = 5;

                #endregion

                #region Información Tributaria

                PdfPCell cInfoTrib = new PdfPCell();
                cInfoTrib.Padding = 5;


                Paragraph paragraph = new Paragraph("RUC: " + emp.emp_ruc, _bigboldFont);
                //paragraph.SpacingAfter = size2;
                paragraph.SetLeading(0, lineheight);                                 
                cInfoTrib.AddElement(paragraph);


                paragraph = new Paragraph("NOTA DE CRÉDITO", _biggerFont);
                //paragraph.SpacingAfter = size1;
                paragraph.SetLeading(0, lineheight);
                cInfoTrib.AddElement(paragraph);

                paragraph = new Paragraph("No.: " + comp.com_almacen + "-" + comp.com_pventa + "-" + comp.com_secuencia, _bigFont);
                //paragraph.SpacingAfter = size1;
                paragraph.SetLeading(0, lineheight);
                cInfoTrib.AddElement(paragraph);

                paragraph = new Paragraph("NÚMERO AUTORIZACIÓN:", _boldFont);
                paragraph.SetLeading(0, lineheight);
                cInfoTrib.AddElement(paragraph);
                

                if (!string.IsNullOrEmpty(comp.com_autorizacion))
                {
                    cInfoTrib.AddElement(new Paragraph(comp.com_autorizacion, _standardFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = size1 });
                    //if (comp.com_autorizacion != comp.com_numero)//NO SON OFFLINE
                    //{

                        phrase = new Phrase();
                        phrase.Add(new Chunk("FECHA Y HORA DE AUTORIZACIÓN:", _boldFont));
                        phrase.Add(new Chunk(comp.com_fechaautorizacion, _standardFont));
                        phrase.SetLeading(0, lineheight);
                        cInfoTrib.AddElement(phrase);
                    //}
                }
                else
                     cInfoTrib.AddElement(new Paragraph(comp.com_numero, _standardFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 7 });

                phrase = new Phrase();
                phrase.Add(new Chunk("AMBIENTE:", _boldFont));
                phrase.Add(new Chunk(Enums.GetAmbiente(comp.com_ambiente), _standardFont));
                phrase.SetLeading(0, lineheight);
                cInfoTrib.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("EMISION:", _boldFont));
                phrase.Add(new Chunk(Enums.GetEmision(comp.com_emision), _standardFont));
                phrase.SetLeading(0, lineheight);
                cInfoTrib.AddElement(phrase);


                //cInfoTrib.AddElement(new Paragraph("EMISIÓN:", _boldFont));
                cInfoTrib.AddElement(new Paragraph("CLAVE DE ACCESO:", _boldFont) { SpacingBefore = size1, SpacingAfter = size2 });

                PdfContentByte cb = new PdfContentByte(pdfWriter);
                // add a barcode image
                Barcode128 code = new Barcode128();
                code.Code = comp.com_numero;
                code.StartStopText = false;
                code.Extended = true;
                iTextSharp.text.Image barcode = code.CreateImageWithBarcode(cb, null, null);
                //barcode.ScalePercent(100, 80);
                //barcode.SetAbsolutePosition(327, 617);
                cInfoTrib.AddElement(barcode);

                #endregion
                //clRuc.BorderWidth = 0;
                tcabecera.AddCell(cInfoFac);
                tcabecera.AddCell(cInfoTrib);
                tcabecera.SpacingAfter = size2;
                document.Add(tcabecera);


                #region Tabla Datos Cliente

                PdfPTable tdatos = new PdfPTable(1);
                tdatos.WidthPercentage = 100;
                tdatos.SpacingAfter = 0;

                PdfPCell cdatos = new PdfPCell();
                cdatos.Padding = 5;

                float[] columnWidthsdatos = { 75, 25 };
                PdfPTable tdatosdet = new PdfPTable(2);
                tdatosdet.WidthPercentage = 100;
                tdatosdet.SetWidths(columnWidthsdatos);


                PdfPCell cdatosdet1 = new PdfPCell();
                cdatosdet1.Border = 0;
                cdatosdet1.Padding = 0;


                phrase = new Phrase();
                phrase.Add(new Chunk("Razón Social/Nombres y Apellidos:", _boldFont));
                phrase.Add(new Chunk(comp.com_nombrecliente, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet1.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Fecha Emisión:", _boldFont));
                phrase.Add(new Chunk(comp.com_fechastr, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet1.AddElement(phrase);


                phrase = new Phrase();
                phrase.Add(new Chunk("Dirección:", _boldFont));
                phrase.Add(new Chunk(comp.com_direccioncliente, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet1.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Email:", _boldFont));
                phrase.Add(new Chunk(comp.com_email, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet1.AddElement(phrase);




                PdfPCell cdatosdet2 = new PdfPCell();
                cdatosdet2.Border = 0;

                phrase = new Phrase();
                phrase.Add(new Chunk("RUC / CI:", _boldFont));
                phrase.Add(new Chunk(comp.com_ruccliente, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet2.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Teléfono:", _boldFont));
                phrase.Add(new Chunk(comp.com_telefonocliente, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet2.AddElement(phrase);


                //cdatosdet2.AddElement(new Paragraph("RUC/CI:", _boldFont));
                //cdatosdet2.AddElement(new Paragraph("Guía Remisión:", _boldFont));

                tdatosdet.AddCell(cdatosdet1);
                tdatosdet.AddCell(cdatosdet2);
                
                tdatosdet.AddCell(new PdfPCell(new Paragraph("")) { BorderWidthTop=0,BorderWidthLeft=0,BorderWidthRight=0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment=Element.ALIGN_BOTTOM , Colspan = 2, FixedHeight=size1});


                PdfPCell cdatosdet3 = new PdfPCell();
                cdatosdet3.Border = 0;
                cdatosdet3.Padding = 0;
                
                phrase = new Phrase();
                phrase.Add(new Chunk("Comprobante que se modifica:", _boldFont));
                phrase.Add(new Chunk(comp.com_numdocmodificado, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet3.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Fecha emisión (comprobante a modificar):", _boldFont));
                phrase.Add(new Chunk(comp.com_fechaemisiondocsustento, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet3.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Razon de modificación:", _boldFont));
                phrase.Add(new Chunk(comp.com_motivo, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet3.AddElement(phrase);
                cdatosdet3.Colspan = 2;
                tdatosdet.AddCell(cdatosdet3);



                cdatos.AddElement(tdatosdet);
                tdatos.AddCell(cdatos);
                tdatos.SpacingAfter = size2;
                document.Add(tdatos);
                ///
                #endregion

                #region Tabla Detalle 

                float[] columnWidthsdet = { 10, 10, 50, 10, 10, 10 };
                PdfPTable tdetalle = new PdfPTable(6);
                tdetalle.WidthPercentage = 100;
                tdetalle.SetWidths(columnWidthsdet);

                PdfPCell ccodigodet = new PdfPCell();
                ccodigodet.AddElement(new Paragraph("Código", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //ccodigodet.VerticalAlignment = Element.ALIGN_MIDDLE;
                ccodigodet.FixedHeight = size3;
                //ccodigodet.HorizontalAlignment = Element.ALIGN_CENTER;



                PdfPCell ccantidaddet = new PdfPCell();
                ccantidaddet.AddElement(new Paragraph("Cant.", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //ccantidaddet.VerticalAlignment = Element.ALIGN_MIDDLE;

                PdfPCell cdescripciondet = new PdfPCell();
                cdescripciondet.AddElement(new Paragraph("Descripción", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //cdescripciondet.VerticalAlignment = Element.ALIGN_MIDDLE;

                PdfPCell cpreciodet = new PdfPCell();
                cpreciodet.AddElement(new Paragraph("Precio U.", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //cpreciodet.VerticalAlignment = Element.ALIGN_MIDDLE;

                PdfPCell cdescuentodet = new PdfPCell();
                cdescuentodet.AddElement(new Paragraph("Descuento", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //cdescuentodet.VerticalAlignment = Element.ALIGN_MIDDLE;

                PdfPCell ctotaldet = new PdfPCell();
                ctotaldet.AddElement(new Paragraph("Precio Total", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //ctotaldet.VerticalAlignment = Element.ALIGN_MIDDLE;

                tdetalle.AddCell(ccodigodet);
                tdetalle.AddCell(ccantidaddet);
                tdetalle.AddCell(cdescripciondet);
                tdetalle.AddCell(cpreciodet);
                tdetalle.AddCell(cdescuentodet);
                tdetalle.AddCell(ctotaldet);


                for (int i = 0; i < comp.detalle.Count; i++)
                {
                    Detalle det = comp.detalle[i];

                    //tdetalle.AddCell(new PdfPCell(new Paragraph(det.codigoaux, _standardFont)) { FixedHeight = 30, HorizontalAlignment = Element.ALIGN_CENTER });
                    tdetalle.AddCell(new PdfPCell(new Paragraph(det.codigo, _standardFont)) { FixedHeight = 15, HorizontalAlignment = Element.ALIGN_CENTER });
                    tdetalle.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(det.cantidad), _standardFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                    string desc = det.descripcion;
                    if (!string.IsNullOrEmpty(det.adicional1))
                        desc += " " + det.adicional1;
                    if (!string.IsNullOrEmpty(det.adicional2))
                        desc += " " + det.adicional2;
                    if (!string.IsNullOrEmpty(det.adicional3))
                        desc += " " + det.adicional3;



                    if (comp.com_formato == 2 && (det.adicional1 != null || det.adicional2 != null || det.adicional3 != null))
                    {
                        desc = "";
                        if (!string.IsNullOrEmpty(det.adicional1))
                            desc += " " + det.adicional1;
                        if (!string.IsNullOrEmpty(det.adicional2))
                            desc += " " + det.adicional2;
                        if (!string.IsNullOrEmpty(det.adicional3))
                            desc += " " + det.adicional3;
                    }





                    tdetalle.AddCell(new PdfPCell(new Paragraph(desc, _standardFont)));
                    tdetalle.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(det.precio), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    tdetalle.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(det.descuento), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    tdetalle.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(det.totalsinimp), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                //if (comp.detalle.Count < 4)
                //{
                //    for (int i = comp.detalle.Count; i < 4; i++)
                //    {
                //        tdetalle.AddCell(new PdfPCell(new Paragraph("")) { FixedHeight = 15 });
                //        tdetalle.AddCell(new PdfPCell(new Paragraph("")));
                //        tdetalle.AddCell(new PdfPCell(new Paragraph("")));
                //        tdetalle.AddCell(new PdfPCell(new Paragraph("")));
                //        tdetalle.AddCell(new PdfPCell(new Paragraph("")));
                //        tdetalle.AddCell(new PdfPCell(new Paragraph("")));
                //    }
                //}

                //tdetalle.SpacingAfter = 5;
                document.Add(tdetalle);

                #endregion



                float[] columnWidthspie = { 60, 30, 10 };

                PdfPTable tdpie = new PdfPTable(3);
                tdpie.WidthPercentage = 100;
                tdpie.SetWidths(columnWidthspie);

                PdfPCell cpieinfoadi = new PdfPCell();
                cpieinfoadi.Border = 0;
                cpieinfoadi.PaddingTop = 5;
                cpieinfoadi.PaddingRight = 5;
                cpieinfoadi.Rowspan = 10;

                float[] columnWidthsADI = { 60, 40 };
                PdfPTable tdinfoadi = new PdfPTable(2);
                tdinfoadi.WidthPercentage = 100;
                tdinfoadi.SetWidths(columnWidthsADI);
                tdinfoadi.SpacingAfter = 5;
                tdinfoadi.AddCell(new PdfPCell(new Paragraph("INFORMACIÓN ADICIONAL", _boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Colspan = 2 });

                phrase = new Phrase();
                phrase.Add(new Chunk("Vendedor:", _boldFont));
                phrase.Add(new Chunk(comp.com_adicional1, _standardFont));
                tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthRight = 0 });

                phrase = new Phrase();
                phrase.Add(new Chunk("Ciudad cliente:", _boldFont));
                phrase.Add(new Chunk(comp.com_adicional2, _standardFont));
                tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthLeft = 0 });


                phrase = new Phrase();
                phrase.Add(new Chunk("Forma de pago:", _boldFont));
                phrase.Add(new Chunk(comp.com_adicional3, _standardFont));
                tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthRight = 0 });

                phrase = new Phrase();
                phrase.Add(new Chunk("Transporte:", _boldFont));
                phrase.Add(new Chunk(comp.com_adicional4, _standardFont));
                tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthLeft = 0 });
         

                phrase = new Phrase();
                phrase.Add(new Chunk("Observación:", _boldFont));
                phrase.Add(new Chunk(comp.com_adicional5, _standardFont));
                tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, PaddingBottom = 5, Colspan = 2 });

                cpieinfoadi.AddElement(tdinfoadi);

              

                tdpie.AddCell(cpieinfoadi);


                tdpie.AddCell(new PdfPCell(new Paragraph("SUBTOTAL " + comp.com_porciva + "%", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_subtotaliva), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                tdpie.AddCell(new PdfPCell(new Paragraph("SUBTOTAL 0%", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_subtotal0), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                if (comp.com_subtotalnoiva.HasValue)
                {
                    if (comp.com_subtotalnoiva.Value > 0)
                    {
                        tdpie.AddCell(new PdfPCell(new Paragraph("SUBTOTAL NO OBJETO DE IVA", _boldFont)));
                        tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_subtotalnoiva), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    }
                }


                if (comp.com_subtotalextiva.HasValue)
                {
                    if (comp.com_subtotalextiva.Value > 0)
                    {
                        tdpie.AddCell(new PdfPCell(new Paragraph("SUBTOTAL EXENTO DE IVA", _boldFont)));
                        tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_subtotalextiva), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    }
                }

                tdpie.AddCell(new PdfPCell(new Paragraph("SUBTOTAL SIN IMPUESTOS", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_subtotalsinimp), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                if (comp.com_descuento.HasValue)
                {
                    if (comp.com_descuento.Value > 0)
                    {
                        tdpie.AddCell(new PdfPCell(new Paragraph("TOTAL DESCUENTO", _boldFont)));
                        tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_descuento), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    }
                }

                if (comp.com_ice.HasValue)
                {
                    if (comp.com_ice.Value > 0)
                    {
                        tdpie.AddCell(new PdfPCell(new Paragraph("ICE", _boldFont)));
                        tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_ice), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    }
                }



                tdpie.AddCell(new PdfPCell(new Paragraph("IVA " + comp.com_porciva + "%", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_iva), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                if (comp.com_propina.HasValue)
                {
                    if (comp.com_propina.Value > 0)
                    {
                        tdpie.AddCell(new PdfPCell(new Paragraph("PROPINA", _boldFont)));
                        tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_propina), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    }
                }

                tdpie.AddCell(new PdfPCell(new Paragraph("VALOR TOTAL", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_total), _boldFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });




                document.Add(tdpie);


                //AGREGA HTML PIE  
                Paragraph ppie = new Paragraph();
                foreach (IElement E in HTMLWorker.ParseToList(new StringReader(formato.for_pie), null))
                    ppie.Add(E);
                ppie.SetLeading(0.0f, 1.0f);
                document.Add(ppie);
                document.Close();

            }
            catch (Exception ex)
            {
                document.Close();
            }

            return file;
        }


        public static string CreateND(Empresa emp, Comprobante comp, Formato formato, string path)
        {
            string file = path + @"\\temp\\" + string.Format("{0}{1}-{2}-{3}_{4}.pdf", formato.for_tipo, comp.com_almacen, comp.com_pventa, comp.com_secuencia, comp.com_empresa);
            if (File.Exists(file))
                File.Delete(file);

            //Document document = new Document(PageSize.A5.Rotate(), 10, 10, 10, 10);


            iTextSharp.text.Font _biggerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _biggerboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            iTextSharp.text.Font _bigFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _bigboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _boldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            iTextSharp.text.Font _smallFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _smallboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            float logoheight = 110;
            float size1 = 7;
            float size2 = 5;
            float size3 = 25;
            float lineheight = (float)1.6;
            Document document = new Document(PageSize.A4, 20, 20, 20, 20);
            if (formato.for_size == "A5")
            {
                document = new Document(PageSize.A5.Rotate(), 20, 20, 20, 20);
                logoheight = 80;
                size1 = 4;
                size2 = 2;
                size3 = 12;
                lineheight = (float)1.5;
                _biggerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                _biggerboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                _bigFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                _bigboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                _boldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.BOLD, BaseColor.BLACK);


            }


            try
            {


                var pdfWriter = PdfWriter.GetInstance(document, new FileStream(file, FileMode.Create));

                pdfWriter.PageEvent = new ITextEvents();

                document.Open();




                //TABLA CABECERA

                float[] columnWidths = { 50, 50 };
                PdfPTable tcabecera = new PdfPTable(2);
                tcabecera.WidthPercentage = 100;
                tcabecera.SetWidths(columnWidths);

                PdfPTable tinfo = new PdfPTable(1);
                tinfo.WidthPercentage = 100;


                #region Logo
                //Agrega el logo


                string imageURL = path + "\\logos\\nologo.png";
                if (!string.IsNullOrEmpty(emp.emp_logo))
                    imageURL = path + "\\logos\\" + emp.emp_logo;
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imageURL);




                PdfPCell cinfo_logo = new PdfPCell(logo, true);
                cinfo_logo.BorderWidth = 0;
                cinfo_logo.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                cinfo_logo.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                cinfo_logo.PaddingBottom = size2;
                cinfo_logo.FixedHeight = logoheight;

                #endregion

                #region Información NC

                PdfPCell cinfo_fac = new PdfPCell();
                cinfo_fac.Padding = 5;
                cinfo_fac.AddElement(new Paragraph(emp.emp_nombre, _boldFont));

                Phrase phrase = new Phrase();
                phrase.Add(new Chunk("Dir. Matriz:", _boldFont));
                phrase.Add(new Chunk(comp.com_direccionmatriz, _standardFont));
                phrase.SetLeading(0, lineheight);
                cinfo_fac.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Dir. Sucursal:", _boldFont));
                phrase.Add(new Chunk(comp.com_direccionsucursal, _standardFont));
                phrase.SetLeading(0, lineheight);
                cinfo_fac.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Contribuyente Especial Nro.:", _boldFont));
                phrase.Add(new Chunk(comp.com_contribuyente, _standardFont));
                phrase.SetLeading(0, lineheight);
                cinfo_fac.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Obligado a llevar contabilidad:", _boldFont));
                phrase.Add(new Chunk(comp.com_contabilidad, _standardFont));
                phrase.SetLeading(0, lineheight);
                cinfo_fac.AddElement(phrase);

                if (!string.IsNullOrEmpty(comp.com_regimenmicro))
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk(comp.com_regimenmicro, _smallboldFont));
                    cinfo_fac.AddElement(phrase);
                }

                if (!string.IsNullOrEmpty(comp.com_agenteret))
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk("Agente de Retención Resolución No.:", _boldFont));
                    phrase.Add(new Chunk(comp.com_agenteret, _standardFont));
                    cinfo_fac.AddElement(phrase);
                }
                if (!string.IsNullOrEmpty(comp.com_regimenrimpe))
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk(comp.com_regimenrimpe, _boldFont));
                    phrase.SetLeading(0, lineheight);
                    cinfo_fac.AddElement(phrase);
                }

                //cinfo_fac.AddElement(new Paragraph("Dir. Sucursal:", _boldFont));
                //cinfo_fac.AddElement(new Paragraph("Contribuyente Especial Nro.:", _boldFont));
                //cinfo_fac.AddElement(new Paragraph("Obligado a llevar contabilidad:", _boldFont));


                tinfo.AddCell(cinfo_logo);
                tinfo.AddCell(cinfo_fac);


                PdfPCell cInfoFac = new PdfPCell(tinfo);
                cInfoFac.BorderWidth = 0;
                cInfoFac.PaddingRight = 5;

                #endregion

                #region Información Tributaria

                PdfPCell cInfoTrib = new PdfPCell();
                cInfoTrib.Padding = 5;


                Paragraph paragraph = new Paragraph("RUC: " + emp.emp_ruc, _bigboldFont);
                //paragraph.SpacingAfter = size2;
                paragraph.SetLeading(0, lineheight);
                cInfoTrib.AddElement(paragraph);


                paragraph = new Paragraph("NOTA DE DÉBITO", _biggerFont);
                //paragraph.SpacingAfter = size1;
                paragraph.SetLeading(0, lineheight);
                cInfoTrib.AddElement(paragraph);

                paragraph = new Paragraph("No.: " + comp.com_almacen + "-" + comp.com_pventa + "-" + comp.com_secuencia, _bigFont);
                //paragraph.SpacingAfter = size1;
                paragraph.SetLeading(0, lineheight);
                cInfoTrib.AddElement(paragraph);

                paragraph = new Paragraph("NÚMERO AUTORIZACIÓN:", _boldFont);
                paragraph.SetLeading(0, lineheight);
                cInfoTrib.AddElement(paragraph);


                if (!string.IsNullOrEmpty(comp.com_autorizacion))
                {
                    cInfoTrib.AddElement(new Paragraph(comp.com_autorizacion, _standardFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = size1 });
                    if (comp.com_autorizacion != comp.com_numero)//NO SON OFFLINE
                    {

                        phrase = new Phrase();
                        phrase.Add(new Chunk("FECHA Y HORA DE AUTORIZACIÓN:", _boldFont));
                        phrase.Add(new Chunk(comp.com_fechaautorizacion, _standardFont));
                        phrase.SetLeading(0, lineheight);
                        cInfoTrib.AddElement(phrase);
                    }
                }
                //else
                //     cInfoTrib.AddElement(new Paragraph(comp.com_numero, _standardFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 7 });

                phrase = new Phrase();
                phrase.Add(new Chunk("AMBIENTE:", _boldFont));
                phrase.Add(new Chunk(Enums.GetAmbiente(comp.com_ambiente), _standardFont));
                phrase.SetLeading(0, lineheight);
                cInfoTrib.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("EMISION:", _boldFont));
                phrase.Add(new Chunk(Enums.GetEmision(comp.com_emision), _standardFont));
                phrase.SetLeading(0, lineheight);
                cInfoTrib.AddElement(phrase);


                //cInfoTrib.AddElement(new Paragraph("EMISIÓN:", _boldFont));
                cInfoTrib.AddElement(new Paragraph("CLAVE DE ACCESO:", _boldFont) { SpacingBefore = size1, SpacingAfter = size2 });

                PdfContentByte cb = new PdfContentByte(pdfWriter);
                // add a barcode image
                Barcode128 code = new Barcode128();
                code.Code = comp.com_numero;
                code.StartStopText = false;
                code.Extended = true;
                iTextSharp.text.Image barcode = code.CreateImageWithBarcode(cb, null, null);
                //barcode.ScalePercent(100, 80);
                //barcode.SetAbsolutePosition(327, 617);
                cInfoTrib.AddElement(barcode);

                #endregion
                //clRuc.BorderWidth = 0;
                tcabecera.AddCell(cInfoFac);
                tcabecera.AddCell(cInfoTrib);
                tcabecera.SpacingAfter = size2;
                document.Add(tcabecera);


                #region Tabla Datos Cliente

                PdfPTable tdatos = new PdfPTable(1);
                tdatos.WidthPercentage = 100;
                tdatos.SpacingAfter = 0;

                PdfPCell cdatos = new PdfPCell();
                cdatos.Padding = 5;

                float[] columnWidthsdatos = { 75, 25 };
                PdfPTable tdatosdet = new PdfPTable(2);
                tdatosdet.WidthPercentage = 100;
                tdatosdet.SetWidths(columnWidthsdatos);


                PdfPCell cdatosdet1 = new PdfPCell();
                cdatosdet1.Border = 0;
                cdatosdet1.Padding = 0;


                phrase = new Phrase();
                phrase.Add(new Chunk("Razón Social/Nombres y Apellidos:", _boldFont));
                phrase.Add(new Chunk(comp.com_nombrecliente, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet1.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Fecha Emisión:", _boldFont));
                phrase.Add(new Chunk(comp.com_fechastr, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet1.AddElement(phrase);


                phrase = new Phrase();
                phrase.Add(new Chunk("Dirección:", _boldFont));
                phrase.Add(new Chunk(comp.com_direccioncliente, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet1.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Email:", _boldFont));
                phrase.Add(new Chunk(comp.com_email, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet1.AddElement(phrase);




                PdfPCell cdatosdet2 = new PdfPCell();
                cdatosdet2.Border = 0;

                phrase = new Phrase();
                phrase.Add(new Chunk("RUC / CI:", _boldFont));
                phrase.Add(new Chunk(comp.com_ruccliente, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet2.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Teléfono:", _boldFont));
                phrase.Add(new Chunk(comp.com_telefonocliente, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet2.AddElement(phrase);


                //cdatosdet2.AddElement(new Paragraph("RUC/CI:", _boldFont));
                //cdatosdet2.AddElement(new Paragraph("Guía Remisión:", _boldFont));

                tdatosdet.AddCell(cdatosdet1);
                tdatosdet.AddCell(cdatosdet2);

                tdatosdet.AddCell(new PdfPCell(new Paragraph("")) { BorderWidthTop = 0, BorderWidthLeft = 0, BorderWidthRight = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Colspan = 2, FixedHeight = size1 });


                PdfPCell cdatosdet3 = new PdfPCell();
                cdatosdet3.Border = 0;
                cdatosdet3.Padding = 0;

                phrase = new Phrase();
                phrase.Add(new Chunk("Comprobante que se modifica:", _boldFont));
                phrase.Add(new Chunk(comp.com_numdocmodificado, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet3.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Fecha emisión (comprobante a modificar):", _boldFont));
                phrase.Add(new Chunk(comp.com_fechaemisiondocsustento, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet3.AddElement(phrase);



                //phrase = new Phrase();
                //phrase.Add(new Chunk("Razon de modificación:", _boldFont));
                //phrase.Add(new Chunk(comp.com_motivo, _standardFont));
                //phrase.SetLeading(0, lineheight);
                //cdatosdet3.AddElement(phrase);
                cdatosdet3.Colspan = 2;
                tdatosdet.AddCell(cdatosdet3);



                cdatos.AddElement(tdatosdet);
                tdatos.AddCell(cdatos);
                tdatos.SpacingAfter = size2;
                document.Add(tdatos);


               
                ///
                #endregion

                #region Tabla Detalle 

                float[] columnWidthsdet = { 70, 30 };
                PdfPTable tdetalle = new PdfPTable(2);
                tdetalle.WidthPercentage = 100;
                tdetalle.SetWidths(columnWidthsdet);

                PdfPCell ccodigodet = new PdfPCell();
                ccodigodet.AddElement(new Paragraph("RAZÓN DE LA MODIFICACIÓN", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //ccodigodet.VerticalAlignment = Element.ALIGN_MIDDLE;
                ccodigodet.FixedHeight = size3;
                //ccodigodet.HorizontalAlignment = Element.ALIGN_CENTER;



                PdfPCell ccantidaddet = new PdfPCell();
                ccantidaddet.AddElement(new Paragraph("VALOR DE LA MODIFICACIÓN", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //ccantidaddet.VerticalAlignment = Element.ALIGN_MIDDLE;
               

                tdetalle.AddCell(ccodigodet);
                tdetalle.AddCell(ccantidaddet);


                for (int i = 0; i < comp.detalle.Count; i++)
                {
                    Detalle det = comp.detalle[i];

                    //tdetalle.AddCell(new PdfPCell(new Paragraph(det.codigoaux, _standardFont)) { FixedHeight = 30, HorizontalAlignment = Element.ALIGN_CENTER });
                    tdetalle.AddCell(new PdfPCell(new Paragraph(det.descripcion, _standardFont)) { FixedHeight = 15, HorizontalAlignment = Element.ALIGN_LEFT});
                    tdetalle.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(det.totalsinimp), _standardFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                }
              
                document.Add(tdetalle);

                #endregion



                float[] columnWidthspie = { 60, 30, 10 };

                PdfPTable tdpie = new PdfPTable(3);
                tdpie.WidthPercentage = 100;
                tdpie.SetWidths(columnWidthspie);

                PdfPCell cpieinfoadi = new PdfPCell();
                cpieinfoadi.Border = 0;
                cpieinfoadi.PaddingTop = 5;
                cpieinfoadi.PaddingRight = 5;
                cpieinfoadi.Rowspan = 10;

                float[] columnWidthsADI = { 60, 40 };
                PdfPTable tdinfoadi = new PdfPTable(2);
                tdinfoadi.WidthPercentage = 100;
                tdinfoadi.SetWidths(columnWidthsADI);
                tdinfoadi.SpacingAfter = 5;
                tdinfoadi.AddCell(new PdfPCell(new Paragraph("INFORMACIÓN ADICIONAL", _boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Colspan = 2 });

                phrase = new Phrase();
                phrase.Add(new Chunk("Vendedor:", _boldFont));
                phrase.Add(new Chunk(comp.com_adicional1, _standardFont));
                tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthRight = 0 });

                phrase = new Phrase();
                phrase.Add(new Chunk("Ciudad cliente:", _boldFont));
                phrase.Add(new Chunk(comp.com_adicional2, _standardFont));
                tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthLeft = 0 });


                phrase = new Phrase();
                phrase.Add(new Chunk("Forma de pago:", _boldFont));
                phrase.Add(new Chunk(comp.com_adicional3, _standardFont));
                tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthRight = 0 });

                phrase = new Phrase();
                phrase.Add(new Chunk("Transporte:", _boldFont));
                phrase.Add(new Chunk(comp.com_adicional4, _standardFont));
                tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthLeft = 0 });


                phrase = new Phrase();
                phrase.Add(new Chunk("Observación:", _boldFont));
                phrase.Add(new Chunk(comp.com_adicional5, _standardFont));
                tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, PaddingBottom = 5, Colspan = 2 });

                cpieinfoadi.AddElement(tdinfoadi);



                float[] columnWidthsformas = { 65, 15, 15, 15 };
                PdfPTable tdformas = new PdfPTable(4);
                tdformas.WidthPercentage = 100;
                tdformas.SetWidths(columnWidthsformas);

                tdformas.AddCell(new PdfPCell(new Paragraph("Forma de Pago", _boldFont)));
                tdformas.AddCell(new PdfPCell(new Paragraph("Valor", _boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                tdformas.AddCell(new PdfPCell(new Paragraph("Plazo", _boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                tdformas.AddCell(new PdfPCell(new Paragraph("Tiempo", _boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                for (int i = 0; i < comp.formas.Count; i++)
                {
                    Formapago fp = comp.formas[i];

                    tdformas.AddCell(new PdfPCell(new Paragraph(fp.forma, _standardFont)));
                    tdformas.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(fp.valor), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    tdformas.AddCell(new PdfPCell(new Paragraph(fp.plazo.ToString(), _standardFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    tdformas.AddCell(new PdfPCell(new Paragraph(fp.tiempo, _standardFont)));
                }




                cpieinfoadi.AddElement(tdformas);



                tdpie.AddCell(cpieinfoadi);


                tdpie.AddCell(new PdfPCell(new Paragraph("SUBTOTAL " + comp.com_porciva + "%", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_subtotaliva), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                tdpie.AddCell(new PdfPCell(new Paragraph("SUBTOTAL 0%", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_subtotal0), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                if (comp.com_subtotalnoiva.HasValue)
                {
                    if (comp.com_subtotalnoiva.Value > 0)
                    {
                        tdpie.AddCell(new PdfPCell(new Paragraph("SUBTOTAL NO OBJETO DE IVA", _boldFont)));
                        tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_subtotalnoiva), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    }
                }


                if (comp.com_subtotalextiva.HasValue)
                {
                    if (comp.com_subtotalextiva.Value > 0)
                    {
                        tdpie.AddCell(new PdfPCell(new Paragraph("SUBTOTAL EXENTO DE IVA", _boldFont)));
                        tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_subtotalextiva), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    }
                }

                tdpie.AddCell(new PdfPCell(new Paragraph("SUBTOTAL SIN IMPUESTOS", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_subtotalsinimp), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                if (comp.com_descuento.HasValue)
                {
                    if (comp.com_descuento.Value > 0)
                    {
                        tdpie.AddCell(new PdfPCell(new Paragraph("TOTAL DESCUENTO", _boldFont)));
                        tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_descuento), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    }
                }

                if (comp.com_ice.HasValue)
                {
                    if (comp.com_ice.Value > 0)
                    {
                        tdpie.AddCell(new PdfPCell(new Paragraph("ICE", _boldFont)));
                        tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_ice), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    }
                }



                tdpie.AddCell(new PdfPCell(new Paragraph("IVA " + comp.com_porciva + "%", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_iva), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                if (comp.com_propina.HasValue)
                {
                    if (comp.com_propina.Value > 0)
                    {
                        tdpie.AddCell(new PdfPCell(new Paragraph("PROPINA", _boldFont)));
                        tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_propina), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    }
                }

                tdpie.AddCell(new PdfPCell(new Paragraph("VALOR TOTAL", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_total), _boldFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });




                document.Add(tdpie);


                //AGREGA HTML PIE  
                Paragraph ppie = new Paragraph();
                foreach (IElement E in HTMLWorker.ParseToList(new StringReader(formato.for_pie), null))
                    ppie.Add(E);
                ppie.SetLeading(0.0f, 1.0f);
                document.Add(ppie);
                document.Close();

            }
            catch (Exception ex)
            {
                document.Close();
            }

            return file;
        }

        public static string CreateGREM(Empresa emp, Comprobante comp, Formato formato, string path)
        {
            string file = path + @"\\temp\\" + string.Format("{0}{1}-{2}-{3}_{4}.pdf", formato.for_tipo, comp.com_almacen, comp.com_pventa, comp.com_secuencia, comp.com_empresa);
            if (File.Exists(file))
                File.Delete(file);

            //Document document = new Document(PageSize.A5.Rotate(), 10, 10, 10, 10);


            iTextSharp.text.Font _biggerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _biggerboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            iTextSharp.text.Font _bigFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _bigboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _boldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            iTextSharp.text.Font _smallFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _smallboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            float logoheight = 110;
            float size1 = 7;
            float size2 = 5;
            float size3 = 25;
            float lineheight = (float)1.6;
            Document document = new Document(PageSize.A4, 20, 20, 20, 20);
            if (formato.for_size == "A5")
            {
                document = new Document(PageSize.A5.Rotate(), 20, 20, 20, 20);
                logoheight = 80;
                size1 = 4;
                size2 = 2;
                size3 = 12;
                lineheight = (float)1.5;
                _biggerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                _biggerboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                _bigFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                _bigboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                _boldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.BOLD, BaseColor.BLACK);


            }


            try
            {


                var pdfWriter = PdfWriter.GetInstance(document, new FileStream(file, FileMode.Create));

                pdfWriter.PageEvent = new ITextEvents();

                document.Open();




                //TABLA CABECERA

                float[] columnWidths = { 50, 50 };
                PdfPTable tcabecera = new PdfPTable(2);
                tcabecera.WidthPercentage = 100;
                tcabecera.SetWidths(columnWidths);

                PdfPTable tinfo = new PdfPTable(1);
                tinfo.WidthPercentage = 100;


                #region Logo
                //Agrega el logo


                string imageURL = path + "\\logos\\nologo.png";
                if (!string.IsNullOrEmpty(emp.emp_logo))
                    imageURL = path + "\\logos\\" + emp.emp_logo;
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imageURL);




                PdfPCell cinfo_logo = new PdfPCell(logo, true);
                cinfo_logo.BorderWidth = 0;
                cinfo_logo.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                cinfo_logo.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                cinfo_logo.PaddingBottom = size2;
                cinfo_logo.FixedHeight = logoheight;

                #endregion

                #region Información GREM

                PdfPCell cinfo_fac = new PdfPCell();
                cinfo_fac.Padding = 5;
                cinfo_fac.AddElement(new Paragraph(emp.emp_nombre, _boldFont));

                Phrase phrase = new Phrase();
                phrase.Add(new Chunk("Dir. Matriz:", _boldFont));
                phrase.Add(new Chunk(comp.com_direccionmatriz, _standardFont));
                phrase.SetLeading(0, lineheight);
                cinfo_fac.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Dir. Sucursal:", _boldFont));
                phrase.Add(new Chunk(comp.com_direccionsucursal, _standardFont));
                phrase.SetLeading(0, lineheight);
                cinfo_fac.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Contribuyente Especial Nro.:", _boldFont));
                phrase.Add(new Chunk(comp.com_contribuyente, _standardFont));
                phrase.SetLeading(0, lineheight);
                cinfo_fac.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Obligado a llevar contabilidad:", _boldFont));
                phrase.Add(new Chunk(comp.com_contabilidad, _standardFont));
                phrase.SetLeading(0, lineheight);
                cinfo_fac.AddElement(phrase);



                if (!string.IsNullOrEmpty(comp.com_regimenmicro))
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk(comp.com_regimenmicro, _smallboldFont));
                    cinfo_fac.AddElement(phrase);
                }

                if (!string.IsNullOrEmpty(comp.com_agenteret))
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk("Agente de Retención Resolución No.:", _boldFont));
                    phrase.Add(new Chunk(comp.com_agenteret, _standardFont));
                    cinfo_fac.AddElement(phrase);
                }
                if (!string.IsNullOrEmpty(comp.com_regimenrimpe))
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk(comp.com_regimenrimpe, _boldFont));
                    phrase.SetLeading(0, lineheight);
                    cinfo_fac.AddElement(phrase);
                }

                //cinfo_fac.AddElement(new Paragraph("Dir. Sucursal:", _boldFont));
                //cinfo_fac.AddElement(new Paragraph("Contribuyente Especial Nro.:", _boldFont));
                //cinfo_fac.AddElement(new Paragraph("Obligado a llevar contabilidad:", _boldFont));


                tinfo.AddCell(cinfo_logo);
                tinfo.AddCell(cinfo_fac);


                PdfPCell cInfoFac = new PdfPCell(tinfo);
                cInfoFac.BorderWidth = 0;
                cInfoFac.PaddingRight = 5;

                #endregion

                #region Información Tributaria

                PdfPCell cInfoTrib = new PdfPCell();
                cInfoTrib.Padding = 5;


                Paragraph paragraph = new Paragraph("RUC: " + emp.emp_ruc, _bigboldFont);
                //paragraph.SpacingAfter = size2;
                paragraph.SetLeading(0, lineheight);
                cInfoTrib.AddElement(paragraph);


                paragraph = new Paragraph("GUIA DE REMISIÓN", _biggerFont);
                //paragraph.SpacingAfter = size1;
                paragraph.SetLeading(0, lineheight);
                cInfoTrib.AddElement(paragraph);

                paragraph = new Paragraph("No.: " + comp.com_almacen + "-" + comp.com_pventa + "-" + comp.com_secuencia, _bigFont);
                //paragraph.SpacingAfter = size1;
                paragraph.SetLeading(0, lineheight);
                cInfoTrib.AddElement(paragraph);

                paragraph = new Paragraph("NÚMERO AUTORIZACIÓN:", _boldFont);
                paragraph.SetLeading(0, lineheight);
                cInfoTrib.AddElement(paragraph);


                if (!string.IsNullOrEmpty(comp.com_autorizacion))
                {
                    cInfoTrib.AddElement(new Paragraph(comp.com_autorizacion, _standardFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = size1 });
                    //if (comp.com_autorizacion != comp.com_numero)//NO SON OFFLINE
                    //{

                        phrase = new Phrase();
                        phrase.Add(new Chunk("FECHA Y HORA DE AUTORIZACIÓN:", _boldFont));
                        phrase.Add(new Chunk(comp.com_fechaautorizacion, _standardFont));
                        phrase.SetLeading(0, lineheight);
                        cInfoTrib.AddElement(phrase);
                    //}
                }
                else
                     cInfoTrib.AddElement(new Paragraph(comp.com_numero, _standardFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 7 });

                phrase = new Phrase();
                phrase.Add(new Chunk("AMBIENTE:", _boldFont));
                phrase.Add(new Chunk(Enums.GetAmbiente(comp.com_ambiente), _standardFont));
                phrase.SetLeading(0, lineheight);
                cInfoTrib.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("EMISION:", _boldFont));
                phrase.Add(new Chunk(Enums.GetEmision(comp.com_emision), _standardFont));
                phrase.SetLeading(0, lineheight);
                cInfoTrib.AddElement(phrase);


                //cInfoTrib.AddElement(new Paragraph("EMISIÓN:", _boldFont));
                cInfoTrib.AddElement(new Paragraph("CLAVE DE ACCESO:", _boldFont) { SpacingBefore = size1, SpacingAfter = size2 });

                PdfContentByte cb = new PdfContentByte(pdfWriter);
                // add a barcode image
                Barcode128 code = new Barcode128();
                code.Code = comp.com_numero;
                code.StartStopText = false;
                code.Extended = true;
                iTextSharp.text.Image barcode = code.CreateImageWithBarcode(cb, null, null);
                //barcode.ScalePercent(100, 80);
                //barcode.SetAbsolutePosition(327, 617);
                cInfoTrib.AddElement(barcode);

                #endregion
                //clRuc.BorderWidth = 0;
                tcabecera.AddCell(cInfoFac);
                tcabecera.AddCell(cInfoTrib);
                tcabecera.SpacingAfter = size2;
                document.Add(tcabecera);


                #region Tabla Datos Transporte

                PdfPTable tdatos = new PdfPTable(1);
                tdatos.WidthPercentage = 100;
                tdatos.SpacingAfter = 0;

                PdfPCell cdatos = new PdfPCell();
                cdatos.Padding = 5;

                float[] columnWidthsdatos = { 70, 30 };
                PdfPTable tdatosdet = new PdfPTable(2);
                tdatosdet.WidthPercentage = 100;
                tdatosdet.SetWidths(columnWidthsdatos);


                PdfPCell cdatosdet1 = new PdfPCell();
                cdatosdet1.Border = 0;
                cdatosdet1.Padding = 0;


                phrase = new Phrase();
                phrase.Add(new Chunk("Identificación (Transportista):", _boldFont));
                phrase.Add(new Chunk(comp.com_ruccliente, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet1.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Razón Social / Nombres y Apellidos:", _boldFont));
                phrase.Add(new Chunk(comp.com_nombrecliente, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet1.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Placa:", _boldFont));
                phrase.Add(new Chunk(comp.com_placa, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet1.AddElement(phrase);


                phrase = new Phrase();
                phrase.Add(new Chunk("Punto de Partida:", _boldFont));
                phrase.Add(new Chunk(comp.com_dirpartida, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet1.AddElement(phrase);


          

                PdfPCell cdatosdet2 = new PdfPCell();
                cdatosdet2.Border = 0;

                phrase = new Phrase();
                phrase.Add(new Chunk("Fecha inicio Transporte:", _boldFont));
                phrase.Add(new Chunk(comp.com_fechainitransporte, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet2.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Fecha fin Transporte:", _boldFont));
                phrase.Add(new Chunk(comp.com_fechafintransporte, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet2.AddElement(phrase);
              

                tdatosdet.AddCell(cdatosdet1);
                tdatosdet.AddCell(cdatosdet2);

               

                



                cdatos.AddElement(tdatosdet);
                tdatos.AddCell(cdatos);
                tdatos.SpacingAfter = size2;
                document.Add(tdatos);



                ///
                #endregion


                #region Tabla Detalle 


                var serializer = new JavaScriptSerializer();
                List<TipoComprobantes> tipos = serializer.Deserialize<List<TipoComprobantes>>(Constantes.GetParameter("tiposcomprobante"));

              
                foreach (Detalle det in comp.detalle)
                {

                    PdfPTable tdetalle = new PdfPTable(1);
                    tdetalle.WidthPercentage = 100;
                    tdetalle.SpacingAfter = 0;

                    PdfPCell cdes = new PdfPCell();
                    cdes.Padding = 5;

                    float[] columnWidthsdes = { 70, 30 };
                    PdfPTable tdestinodet = new PdfPTable(2);
                    tdestinodet.WidthPercentage = 100;
                    tdestinodet.SetWidths(columnWidthsdatos);


                    PdfPCell cdestinodet1 = new PdfPCell();
                    cdestinodet1.Border = 0;
                    cdestinodet1.Padding = 0;

                    TipoComprobantes tc = tipos.Find(delegate (TipoComprobantes t) { return t.codigo == det.codigodocsustento; });

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Comprobante de Venta:", _boldFont));
                    phrase.Add(new Chunk(tc!=null? tc.comprobante:"", _standardFont));
                    phrase.Add(new Chunk(" ", _standardFont));
                    phrase.Add(new Chunk(det.numdocsustento, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    cdestinodet1.AddElement(phrase);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Número de Autorización:", _boldFont));
                    phrase.Add(new Chunk(det.numautsustento, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    cdestinodet1.AddElement(phrase);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Motivo Traslado:", _boldFont));
                    phrase.Add(new Chunk(det.motivotraslado, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    cdestinodet1.AddElement(phrase);


                    phrase = new Phrase();
                    phrase.Add(new Chunk("Destino (Punto de llegada):", _boldFont));
                    phrase.Add(new Chunk(det.dirdestinatario, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    cdestinodet1.AddElement(phrase);


                    phrase = new Phrase();
                    phrase.Add(new Chunk("Identificación (Destinatario):", _boldFont));
                    phrase.Add(new Chunk(det.iddestinatario, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    cdestinodet1.AddElement(phrase);


                    phrase = new Phrase();
                    phrase.Add(new Chunk("Razón Social/Nombres Apellidos:", _boldFont));
                    phrase.Add(new Chunk(det.razondestinatario, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    cdestinodet1.AddElement(phrase);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Documento Aduanero:", _boldFont));
                    phrase.Add(new Chunk(det.docaduana, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    cdestinodet1.AddElement(phrase);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Código Establecimiento Destino:", _boldFont));
                    phrase.Add(new Chunk(det.codestabdestino, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    cdestinodet1.AddElement(phrase);

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Ruta:", _boldFont));
                    phrase.Add(new Chunk(det.ruta, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    cdestinodet1.AddElement(phrase);



                    PdfPCell cdestinodet2 = new PdfPCell();
                    cdestinodet2.Border = 0;

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Fecha de Emisión:", _boldFont));
                    phrase.Add(new Chunk(det.fechaemisiondocsustento, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    cdestinodet2.AddElement(phrase);

                   


                    tdestinodet.AddCell(cdestinodet1);
                    tdestinodet.AddCell(cdestinodet2);
                    cdes.AddElement(tdestinodet);

                    #region Tabla SubDetalle 



                    float[] columnWidthsdet = { 20, 40, 20, 20};
                    PdfPTable tsubdetalle = new PdfPTable(4);
                    tsubdetalle.WidthPercentage = 100;
                    tsubdetalle.SetWidths(columnWidthsdet);
                    tsubdetalle.SpacingBefore = size2;

                    PdfPCell ccantidaddet = new PdfPCell();
                    ccantidaddet.AddElement(new Paragraph("Cantidad", _boldFont) { Alignment = Element.ALIGN_CENTER });
                    //ccodigodet.VerticalAlignment = Element.ALIGN_MIDDLE;
                    ccantidaddet.FixedHeight = 25;
                    //ccodigodet.HorizontalAlignment = Element.ALIGN_CENTER;
                    
                    PdfPCell cdescripciondet = new PdfPCell();
                    cdescripciondet.AddElement(new Paragraph("Descripción", _boldFont) { Alignment = Element.ALIGN_CENTER });
                    //cdescripciondet.VerticalAlignment = Element.ALIGN_MIDDLE;

                    PdfPCell cpreciodet = new PdfPCell();
                    cpreciodet.AddElement(new Paragraph("Código Principal", _boldFont) { Alignment = Element.ALIGN_CENTER });
                    //cpreciodet.VerticalAlignment = Element.ALIGN_MIDDLE;

                    PdfPCell cdescuentodet = new PdfPCell();
                    cdescuentodet.AddElement(new Paragraph("Código Auxiliar ", _boldFont) { Alignment = Element.ALIGN_CENTER });
                    //cdescuentodet.VerticalAlignment = Element.ALIGN_MIDDLE;



                    tsubdetalle.AddCell(ccantidaddet);
                    tsubdetalle.AddCell(cdescripciondet);
                    tsubdetalle.AddCell(cpreciodet);
                    tsubdetalle.AddCell(cdescuentodet);


                    foreach (Subdetalle sdet in det.subdetalles)
                    {
                        tsubdetalle.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(sdet.cantidad), _standardFont)) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 15 });
                        string desc = sdet.descripcion;
                        if (!string.IsNullOrEmpty(sdet.adicional1))
                            desc += " " + sdet.adicional1;
                        if (!string.IsNullOrEmpty(sdet.adicional2))
                            desc += " " + sdet.adicional2;
                        if (!string.IsNullOrEmpty(sdet.adicional3))
                            desc += " " + sdet.adicional3;

                        tsubdetalle.AddCell(new PdfPCell(new Paragraph(desc, _standardFont)));
                        tsubdetalle.AddCell(new PdfPCell(new Paragraph(sdet.codigointerno, _standardFont)));
                        tsubdetalle.AddCell(new PdfPCell(new Paragraph(sdet.codigoadicional, _standardFont)));
                    }


                    if (det.subdetalles.Count < 4)
                    {
                        for (int i = det.subdetalles.Count; i < 4; i++)
                        {
                            tsubdetalle.AddCell(new PdfPCell(new Paragraph("")) { FixedHeight = 15 });
                            tsubdetalle.AddCell(new PdfPCell(new Paragraph("")));
                            tsubdetalle.AddCell(new PdfPCell(new Paragraph("")));
                            tsubdetalle.AddCell(new PdfPCell(new Paragraph("")));                            
                        }
                    }



                    cdes.AddElement(tsubdetalle);



                    #endregion










                    tdetalle.AddCell(cdes);
                    tdetalle.SpacingAfter = size2;
                    document.Add(tdetalle);




                }








                #endregion



                #region Tabla Adicionales 

                PdfPTable tdinfoadi = new PdfPTable(1);
                //tdinfoadi.WidthPercentage = 100;                
                tdinfoadi.SpacingAfter = 5;
                tdinfoadi.AddCell(new PdfPCell(new Paragraph("INFORMACIÓN ADICIONAL", _boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER});

                phrase = new Phrase();
                phrase.Add(new Chunk("Dirección:", _boldFont));
                phrase.Add(new Chunk(comp.com_direccioncliente, _standardFont));
                tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0});

                phrase = new Phrase();
                phrase.Add(new Chunk("Teléfono:", _boldFont));
                phrase.Add(new Chunk(comp.com_telefonocliente, _standardFont));
                tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0});


                phrase = new Phrase();
                phrase.Add(new Chunk("Email:", _boldFont));
                phrase.Add(new Chunk(comp.com_email, _standardFont));
                tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0});
                       

                document.Add(tdinfoadi);

                #endregion

                PdfPTable tdleyenda = new PdfPTable(1);
                tdleyenda.WidthPercentage = 100;
                tdleyenda.SpacingAfter = 5;
                

                phrase = new Phrase();
                phrase.Add(new Chunk("DECLARO QUE TODO LO DETALLADO EN LA PARTE SUPERIOR ESTA TOTALMENTE DE ACUERDO, Y ACEPTO SIN NINGUNA OPCIÓN DE RECLAMO POSTERIOR", _standardFont));
                tdleyenda.AddCell(new PdfPCell(phrase) { BorderWidth= 0 });

                phrase = new Phrase();
                phrase.Add(new Chunk("OBSERVACIONES:", _standardFont));
                phrase.Add(new Chunk("___________________________________________________________________________________________________", _standardFont));
                tdleyenda.AddCell(new PdfPCell(phrase) { BorderWidth = 0 });

                phrase = new Phrase();
                phrase.Add(new Chunk("___________________________________________________________________________________________________________________", _standardFont));
                tdleyenda.AddCell(new PdfPCell(phrase) { BorderWidth = 0 });


                phrase = new Phrase();
                phrase.Add(new Chunk("FECHA Y HORA RECEPCIÓN CLIENTE:", _standardFont));
                phrase.Add(new Chunk("_____________________________________________________", _standardFont));
                tdleyenda.AddCell(new PdfPCell(phrase) { BorderWidth = 0 });

                document.Add(tdleyenda);


                float[] columnWidthsfirma = { 4,20,4,20,4,20,4,20,4  };
                
                PdfPTable tdfirmas = new PdfPTable(9);
                tdfirmas.WidthPercentage = 100;
                tdfirmas.SpacingBefore= 40;
                tdfirmas.SetWidths(columnWidthsfirma);

               

                phrase = new Phrase();
                phrase.Add(new Chunk("", _standardFont));
                tdfirmas.AddCell(new PdfPCell(phrase) { BorderWidth = 0});

               
                tdfirmas.AddCell(new PdfPCell(new Paragraph("EMISOR", _standardFont) { Alignment = Element.ALIGN_CENTER }) { BorderWidthBottom = 0, BorderWidthLeft= 0, BorderWidthRight= 0 });

                phrase = new Phrase();
                phrase.Add(new Chunk("", _standardFont));
                tdfirmas.AddCell(new PdfPCell(phrase) { BorderWidth = 0 });

                tdfirmas.AddCell(new PdfPCell(new Paragraph("TRANSPORTISTA", _standardFont) { Alignment = Element.ALIGN_CENTER }) { BorderWidthBottom = 0, BorderWidthLeft = 0, BorderWidthRight = 0 });
                

                phrase = new Phrase();
                phrase.Add(new Chunk("", _standardFont));
                tdfirmas.AddCell(new PdfPCell(phrase) { BorderWidth = 0 });

                tdfirmas.AddCell(new PdfPCell(new Paragraph("ADQUIRIENTE", _standardFont) { Alignment = Element.ALIGN_CENTER }) { BorderWidthBottom = 0, BorderWidthLeft = 0, BorderWidthRight = 0 });

                phrase = new Phrase();
                phrase.Add(new Chunk("", _standardFont));
                tdfirmas.AddCell(new PdfPCell(phrase) { BorderWidth = 0 });

                tdfirmas.AddCell(new PdfPCell(new Paragraph("C.I", _standardFont) { Alignment = Element.ALIGN_CENTER }) { BorderWidthBottom = 0, BorderWidthLeft = 0, BorderWidthRight = 0 });

            
                phrase = new Phrase();
                phrase.Add(new Chunk("", _standardFont));
                tdfirmas.AddCell(new PdfPCell(phrase) { BorderWidth = 0 });

                document.Add(tdfirmas);

                //AGREGA HTML PIE  
                Paragraph ppie = new Paragraph();
                foreach (IElement E in HTMLWorker.ParseToList(new StringReader(formato.for_pie), null))
                    ppie.Add(E);
                ppie.SetLeading(0.0f, 1.0f);
                document.Add(ppie);
                document.Close();

            }
            catch (Exception ex)
            {
                document.Close();
            }

            return file;
        }

        public static string CreateLC(Empresa emp, Comprobante comp, Formato formato, string path)
        {
            string file = path + @"\\temp\\" + string.Format("{0}{1}-{2}-{3}_{4}.pdf", formato.for_tipo, comp.com_almacen, comp.com_pventa, comp.com_secuencia, comp.com_empresa);
            if (File.Exists(file))
                File.Delete(file);

            iTextSharp.text.Font _biggerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _biggerboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            iTextSharp.text.Font _bigFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _bigboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _boldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            iTextSharp.text.Font _smallFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _smallboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            float logoheight = 110;
            float size1 = 7;
            float size2 = 5;
            float size3 = 25;
            float lineheight = (float)1.6;
            int minreg = 8;

            Document document = new Document(PageSize.A4, 20, 20, 20, 20);
            if (formato.for_size == "A5")
            {
                document = new Document(PageSize.A5.Rotate(), 20, 20, 20, 20);
                logoheight = 80;
                size1 = 4;
                size2 = 2;
                size3 = 12;
                lineheight = (float)1.5;
                minreg = 2;
                _biggerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                _biggerboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                _bigFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                _bigboldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                _boldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.BOLD, BaseColor.BLACK);


            }
            try
            {


                var pdfWriter = PdfWriter.GetInstance(document, new FileStream(file, FileMode.Create));

                pdfWriter.PageEvent = new ITextEvents();

                document.Open();





                //TABLA CABECERA

                float[] columnWidths = { 50, 50 };
                PdfPTable tcabecera = new PdfPTable(2);
                tcabecera.WidthPercentage = 100;
                tcabecera.SetWidths(columnWidths);

                PdfPTable tinfo = new PdfPTable(1);
                tinfo.WidthPercentage = 100;


                #region Logo
                //Agrega el logo


                string imageURL = path + "\\logos\\nologo.png";
                if (!string.IsNullOrEmpty(emp.emp_logo))
                    imageURL = path + "\\logos\\" + emp.emp_logo;
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imageURL);




                PdfPCell cinfo_logo = new PdfPCell(logo, true);
                cinfo_logo.BorderWidth = 0;
                cinfo_logo.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                cinfo_logo.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                cinfo_logo.PaddingBottom = size2;
                cinfo_logo.FixedHeight = logoheight;

                #endregion

                #region Información Factura

                PdfPCell cinfo_fac = new PdfPCell();
                cinfo_fac.Padding = 5;
                cinfo_fac.AddElement(new Paragraph(emp.emp_nombre, _boldFont));

                Phrase phrase = new Phrase();
                phrase.Add(new Chunk("Dir. Matriz:", _boldFont));
                phrase.Add(new Chunk(comp.com_direccionmatriz, _standardFont));
                phrase.SetLeading(0, lineheight);
                cinfo_fac.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Dir. Sucursal:", _boldFont));
                phrase.Add(new Chunk(comp.com_direccionsucursal, _standardFont));
                phrase.SetLeading(0, lineheight);
                cinfo_fac.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Contribuyente Especial Nro.:", _boldFont));
                phrase.Add(new Chunk(comp.com_contribuyente, _standardFont));
                phrase.SetLeading(0, lineheight);
                cinfo_fac.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Obligado a llevar contabilidad:", _boldFont));
                phrase.Add(new Chunk(comp.com_contabilidad, _standardFont));
                phrase.SetLeading(0, lineheight);
                cinfo_fac.AddElement(phrase);



                if (!string.IsNullOrEmpty(comp.com_regimenmicro))
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk(comp.com_regimenmicro, _smallboldFont));
                    cinfo_fac.AddElement(phrase);
                }

                if (!string.IsNullOrEmpty(comp.com_agenteret))
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk("Agente de Retención Resolución No.:", _boldFont));
                    phrase.Add(new Chunk(comp.com_agenteret, _standardFont));
                    cinfo_fac.AddElement(phrase);
                }


                tinfo.AddCell(cinfo_logo);
                tinfo.AddCell(cinfo_fac);


                PdfPCell cInfoFac = new PdfPCell(tinfo);
                cInfoFac.BorderWidth = 0;
                cInfoFac.PaddingRight = 5;

                #endregion

                #region Información Tributaria

                PdfPCell cInfoTrib = new PdfPCell();
                cInfoTrib.Padding = 5;


                Paragraph paragraph = new Paragraph("RUC: " + emp.emp_ruc, _bigboldFont);
                //paragraph.SpacingAfter = size2;
                paragraph.SetLeading(0, lineheight);
                cInfoTrib.AddElement(paragraph);


                paragraph = new Paragraph("LIQUIDACIÓN DE COMPRA DE BIENES Y PRESTACIÓN DE SERVICIOS", _biggerFont);
                //paragraph.SpacingAfter = size1;
                paragraph.SetLeading(0, lineheight);
                cInfoTrib.AddElement(paragraph);

                paragraph = new Paragraph("No.: " + comp.com_almacen + "-" + comp.com_pventa + "-" + comp.com_secuencia, _bigFont);
                //paragraph.SpacingAfter = size1;
                paragraph.SetLeading(0, lineheight);
                cInfoTrib.AddElement(paragraph);

                paragraph = new Paragraph("NÚMERO AUTORIZACIÓN:", _boldFont);
                paragraph.SetLeading(0, lineheight);
                cInfoTrib.AddElement(paragraph);


                if (!string.IsNullOrEmpty(comp.com_autorizacion))
                {
                    cInfoTrib.AddElement(new Paragraph(comp.com_autorizacion, _standardFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = size1 });
                    //if (comp.com_autorizacion != comp.com_numero)//NO SON OFFLINE
                    //{

                    phrase = new Phrase();
                    phrase.Add(new Chunk("FECHA Y HORA DE AUTORIZACIÓN:", _boldFont));
                    phrase.Add(new Chunk(comp.com_fechaautorizacion, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    cInfoTrib.AddElement(phrase);
                    //}
                }
                else
                    cInfoTrib.AddElement(new Paragraph(comp.com_numero, _standardFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 7 });

                phrase = new Phrase();
                phrase.Add(new Chunk("AMBIENTE:", _boldFont));
                phrase.Add(new Chunk(Enums.GetAmbiente(comp.com_ambiente), _standardFont));
                phrase.SetLeading(0, lineheight);
                cInfoTrib.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("EMISION:", _boldFont));
                phrase.Add(new Chunk(Enums.GetEmision(comp.com_emision), _standardFont));
                phrase.SetLeading(0, lineheight);
                cInfoTrib.AddElement(phrase);


                //cInfoTrib.AddElement(new Paragraph("EMISIÓN:", _boldFont));
                cInfoTrib.AddElement(new Paragraph("CLAVE DE ACCESO:", _boldFont) { SpacingBefore = size1, SpacingAfter = size2 });

                PdfContentByte cb = new PdfContentByte(pdfWriter);
                // add a barcode image
                Barcode128 code = new Barcode128();
                code.Code = comp.com_numero;
                code.StartStopText = false;
                code.Extended = true;
                iTextSharp.text.Image barcode = code.CreateImageWithBarcode(cb, null, null);

                //barcode.ScaleAbsolute(256f,25f);
                //barcode.SetAbsolutePosition(327, 617);
                cInfoTrib.AddElement(barcode);

                #endregion





                //clRuc.BorderWidth = 0;
                tcabecera.AddCell(cInfoFac);
                tcabecera.AddCell(cInfoTrib);
                tcabecera.SpacingAfter = 5;
                document.Add(tcabecera);


                #region Tabla Datos Cliente

                PdfPTable tdatos = new PdfPTable(1);
                tdatos.WidthPercentage = 100;
                tdatos.SpacingAfter = 0;

                PdfPCell cdatos = new PdfPCell();
                cdatos.Padding = 5;

                float[] columnWidthsdatos = { 75, 25 };
                PdfPTable tdatosdet = new PdfPTable(2);
                tdatosdet.WidthPercentage = 100;
                tdatosdet.SetWidths(columnWidthsdatos);


                PdfPCell cdatosdet1 = new PdfPCell();
                cdatosdet1.Border = 0;
                cdatosdet1.Padding = 0;

                phrase = new Phrase();
                phrase.Add(new Chunk("Nombres y Apellidos:", _boldFont));
                phrase.Add(new Chunk(comp.com_nombrecliente, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet1.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Fecha Emisión:", _boldFont));
                phrase.Add(new Chunk(comp.com_fechastr, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet1.AddElement(phrase);

                phrase = new Phrase();
                phrase.Add(new Chunk("Dirección:", _boldFont));
                phrase.Add(new Chunk(comp.com_direccioncliente, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet1.AddElement(phrase);

                //cdatosdet1.AddElement(new Paragraph("Razón Social/Nombres y Apellidos:", _boldFont));
                //cdatosdet1.AddElement(new Paragraph("Fecha Emisión:", _boldFont));
                //cdatosdet1.AddElement(new Paragraph("Dirección:", _boldFont));


                PdfPCell cdatosdet2 = new PdfPCell();
                cdatosdet2.Border = 0;


                string strid = "Identificación:";
                if (comp.com_tipoidcliente == "06")
                    strid = "RUC/CI/TAX PAYER:";


                phrase = new Phrase();
                phrase.Add(new Chunk(strid, _boldFont));
                phrase.Add(new Chunk(comp.com_ruccliente, _standardFont));
                phrase.SetLeading(0, lineheight);
                cdatosdet2.AddElement(phrase);

               
               

                //cdatosdet2.AddElement(new Paragraph("RUC/CI:", _boldFont));
                //cdatosdet2.AddElement(new Paragraph("Guía Remisión:", _boldFont));

                tdatosdet.AddCell(cdatosdet1);
                tdatosdet.AddCell(cdatosdet2);


                cdatos.AddElement(tdatosdet);
                tdatos.AddCell(cdatos);
                tdatos.SpacingAfter = 5;
                document.Add(tdatos);
                ///
                #endregion

                #region Tabla Detalle 


                float[] columnWidthsdet = { 10, 10, 50, 10, 10, 10 };
                PdfPTable tdetalle = new PdfPTable(6);
                tdetalle.WidthPercentage = 100;
                tdetalle.SetWidths(columnWidthsdet);

                PdfPCell ccodigodet = new PdfPCell();
                ccodigodet.AddElement(new Paragraph("Código", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //ccodigodet.VerticalAlignment = Element.ALIGN_MIDDLE;
                //ccodigodet.FixedHeight = 25;
                //ccodigodet.HorizontalAlignment = Element.ALIGN_CENTER;



                PdfPCell ccantidaddet = new PdfPCell();
                ccantidaddet.AddElement(new Paragraph("Cant.", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //ccantidaddet.VerticalAlignment = Element.ALIGN_MIDDLE;

                PdfPCell cdescripciondet = new PdfPCell();
                cdescripciondet.AddElement(new Paragraph("Descripción", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //cdescripciondet.VerticalAlignment = Element.ALIGN_MIDDLE;

                PdfPCell cpreciodet = new PdfPCell();
                cpreciodet.AddElement(new Paragraph("Precio U.", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //cpreciodet.VerticalAlignment = Element.ALIGN_MIDDLE;

                PdfPCell cdescuentodet = new PdfPCell();
                cdescuentodet.AddElement(new Paragraph("Descuento", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //cdescuentodet.VerticalAlignment = Element.ALIGN_MIDDLE;

                PdfPCell ctotaldet = new PdfPCell();
                ctotaldet.AddElement(new Paragraph("Precio Total", _boldFont) { Alignment = Element.ALIGN_CENTER });
                //ctotaldet.VerticalAlignment = Element.ALIGN_MIDDLE;

                tdetalle.AddCell(ccodigodet);
                tdetalle.AddCell(ccantidaddet);
                tdetalle.AddCell(cdescripciondet);
                tdetalle.AddCell(cpreciodet);
                tdetalle.AddCell(cdescuentodet);
                tdetalle.AddCell(ctotaldet);


                for (int i = 0; i < comp.detalle.Count; i++)
                {
                    Detalle det = comp.detalle[i];

                    //tdetalle.AddCell(new PdfPCell(new Paragraph(det.codigoaux, _standardFont)) { FixedHeight = 30, HorizontalAlignment = Element.ALIGN_CENTER });
                    tdetalle.AddCell(new PdfPCell(new Paragraph(det.codigo, _standardFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    tdetalle.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(det.cantidad), _standardFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                    string desc = det.descripcion;
                    if (!string.IsNullOrEmpty(det.adicional1))
                        desc += " " + det.adicional1;
                    if (!string.IsNullOrEmpty(det.adicional2))
                        desc += " " + det.adicional2;
                    if (!string.IsNullOrEmpty(det.adicional3))
                        desc += " " + det.adicional3;



                    if (comp.com_formato == 2 && (det.adicional1 != null || det.adicional2 != null || det.adicional3 != null))
                    {
                        desc = "";
                        if (!string.IsNullOrEmpty(det.adicional1))
                            desc += " " + det.adicional1;
                        if (!string.IsNullOrEmpty(det.adicional2))
                            desc += " " + det.adicional2;
                        if (!string.IsNullOrEmpty(det.adicional3))
                            desc += " " + det.adicional3;
                    }





                    tdetalle.AddCell(new PdfPCell(new Paragraph(desc, _standardFont)));
                    tdetalle.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(det.precio), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    tdetalle.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(det.descuento), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    tdetalle.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(det.totalsinimp), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                }



                if (comp.detalle.Count < minreg)
                {
                    for (int i = comp.detalle.Count; i < minreg; i++)
                    {
                        tdetalle.AddCell(new PdfPCell(new Paragraph("")) { FixedHeight = 15 });
                        tdetalle.AddCell(new PdfPCell(new Paragraph("")));
                        tdetalle.AddCell(new PdfPCell(new Paragraph("")));
                        tdetalle.AddCell(new PdfPCell(new Paragraph("")));
                        tdetalle.AddCell(new PdfPCell(new Paragraph("")));
                        tdetalle.AddCell(new PdfPCell(new Paragraph("")));
                    }
                }


                //tdetalle.SpacingAfter = 5;
                document.Add(tdetalle);

                #endregion



                float[] columnWidthspie = { 60, 30, 10 };

                PdfPTable tdpie = new PdfPTable(3);
                tdpie.WidthPercentage = 100;
                tdpie.SetWidths(columnWidthspie);

                PdfPCell cpieinfoadi = new PdfPCell();
                cpieinfoadi.Border = 0;
                cpieinfoadi.PaddingTop = 5;
                cpieinfoadi.PaddingRight = 5;
                cpieinfoadi.Rowspan = 10;

                float[] columnWidthsADI = { 60, 40 };
                PdfPTable tdinfoadi = new PdfPTable(2);
                tdinfoadi.WidthPercentage = 100;
                tdinfoadi.SetWidths(columnWidthsADI);
                //tdinfoadi.SpacingAfter = 5;
                tdinfoadi.SpacingAfter = size2;
                tdinfoadi.AddCell(new PdfPCell(new Paragraph("INFORMACIÓN ADICIONAL", _boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Colspan = 2 });






                if (comp.com_adicional1 == null)
                {

                    //phrase = new Phrase();
                    //phrase.Add(new Chunk("Forma de pago:", _boldFont));
                    //phrase.Add(new Chunk(comp.com_adicional3, _standardFont));
                    //tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthRight = 0 });

                    //phrase = new Phrase();
                    //phrase.Add(new Chunk("Ciudad cliente:", _boldFont));
                    //phrase.Add(new Chunk(comp.com_adicional2, _standardFont));
                    //tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthLeft = 0 });

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Teléfono:", _boldFont));
                    phrase.Add(new Chunk(comp.com_telefonocliente, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, Colspan = 2 });

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Email:", _boldFont));
                    phrase.Add(new Chunk(comp.com_email, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, Colspan = 2 });

                    foreach (string item in comp.com_adicionales)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            string[] arrayitem = item.Split('|');
                            phrase = new Phrase();
                            phrase.Add(new Chunk(arrayitem[0] + ":", _boldFont));
                            phrase.Add(new Chunk(arrayitem[1], _standardFont));
                            phrase.SetLeading(0, lineheight);
                            tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, Colspan = 2 });
                        }


                    }
                    phrase = new Phrase();
                    phrase.Add(new Chunk("Observación:", _boldFont));
                    phrase.Add(new Chunk(comp.com_adicional5, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, PaddingBottom = 5, Colspan = 2 });
                }
                else
                {
                    phrase = new Phrase();
                    phrase.Add(new Chunk("Vendedor:", _boldFont));
                    phrase.Add(new Chunk(comp.com_adicional1, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthRight = 0 });


                    phrase = new Phrase();
                    phrase.Add(new Chunk("Ciudad cliente:", _boldFont));
                    phrase.Add(new Chunk(comp.com_adicional2, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthLeft = 0 });

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Forma de pago:", _boldFont));
                    phrase.Add(new Chunk(comp.com_adicional3, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthRight = 0 });


                    phrase = new Phrase();
                    phrase.Add(new Chunk("Transporte:", _boldFont));
                    phrase.Add(new Chunk(comp.com_adicional4, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, BorderWidthLeft = 0 });




                    phrase = new Phrase();
                    phrase.Add(new Chunk("Teléfono:", _boldFont));
                    phrase.Add(new Chunk(comp.com_telefonocliente, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, Colspan = 2 });



                    phrase = new Phrase();
                    phrase.Add(new Chunk("Email:", _boldFont));
                    phrase.Add(new Chunk(comp.com_email, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, BorderWidthBottom = 0, Colspan = 2 });

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Observación:", _boldFont));
                    phrase.Add(new Chunk(comp.com_adicional5, _standardFont));
                    phrase.SetLeading(0, lineheight);
                    tdinfoadi.AddCell(new PdfPCell(phrase) { BorderWidthTop = 0, PaddingBottom = 5, Colspan = 2 });
                }




                //tdinfoadi.AddCell(new PdfPCell(new Paragraph("Teléfono:", _boldFont)) { BorderWidthTop = 0, BorderWidthBottom = 0 });
                //tdinfoadi.AddCell(new PdfPCell(new Paragraph("Email:", _boldFont)) { BorderWidthTop = 0, BorderWidthBottom = 0 });
                //tdinfoadi.AddCell(new PdfPCell(new Paragraph("Observación:", _boldFont)) { BorderWidthTop = 0, PaddingBottom=5 });
                cpieinfoadi.AddElement(tdinfoadi);


                float[] columnWidthsformas = { 65, 15, 15, 15 };
                PdfPTable tdformas = new PdfPTable(4);
                tdformas.WidthPercentage = 100;
                tdformas.SetWidths(columnWidthsformas);

                tdformas.AddCell(new PdfPCell(new Paragraph("Forma de Pago", _boldFont)));
                tdformas.AddCell(new PdfPCell(new Paragraph("Valor", _boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                tdformas.AddCell(new PdfPCell(new Paragraph("Plazo", _boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                tdformas.AddCell(new PdfPCell(new Paragraph("Tiempo", _boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                for (int i = 0; i < comp.formas.Count; i++)
                {
                    Formapago fp = comp.formas[i];

                    tdformas.AddCell(new PdfPCell(new Paragraph(fp.forma, _standardFont)));
                    tdformas.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(fp.valor), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    tdformas.AddCell(new PdfPCell(new Paragraph(fp.plazo.ToString(), _standardFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    tdformas.AddCell(new PdfPCell(new Paragraph(fp.tiempo, _standardFont)));
                }




                cpieinfoadi.AddElement(tdformas);


                tdpie.AddCell(cpieinfoadi);


                /*
                                PdfPCell cpietotales = new PdfPCell();
                                cpietotales.Border = 0;
                                cpietotales.Padding = 0;

                                float[] columnWidthstot = { 70, 30 };
                                PdfPTable tdtotales = new PdfPTable(2);
                                tdtotales.WidthPercentage = 100;
                                tdtotales.SetWidths(columnWidthstot);*/


                tdpie.AddCell(new PdfPCell(new Paragraph("SUBTOTAL " + comp.com_porciva + "%", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_subtotaliva), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                tdpie.AddCell(new PdfPCell(new Paragraph("SUBTOTAL 0%", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_subtotal0), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });


                tdpie.AddCell(new PdfPCell(new Paragraph("SUBTOTAL NO OBJETO DE IVA", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_subtotalnoiva), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                tdpie.AddCell(new PdfPCell(new Paragraph("SUBTOTAL EXENTO DE IVA", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_subtotalextiva), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                tdpie.AddCell(new PdfPCell(new Paragraph("SUBTOTAL SIN IMPUESTOS", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_subtotalsinimp), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                tdpie.AddCell(new PdfPCell(new Paragraph("TOTAL DESCUENTO", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_descuento), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                if (comp.com_ice.HasValue)
                {
                    if (comp.com_ice.Value > 0)
                    {
                        tdpie.AddCell(new PdfPCell(new Paragraph("ICE", _boldFont)));
                        tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_ice), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    }
                }



                tdpie.AddCell(new PdfPCell(new Paragraph("IVA " + comp.com_porciva + "%", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_iva), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                //tdpie.AddCell(new PdfPCell(new Paragraph("PROPINA", _boldFont)));
                //tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_propina), _standardFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                tdpie.AddCell(new PdfPCell(new Paragraph("VALOR TOTAL", _boldFont)));
                tdpie.AddCell(new PdfPCell(new Paragraph(Formatos.CurrencyFormat(comp.com_total), _boldFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                tdpie.AddCell(new PdfPCell() { Border = 0 });
                tdpie.AddCell(new PdfPCell() { Border = 0 });


                document.Add(tdpie);

                //AGREGA HTML PIE  
                Paragraph ppie = new Paragraph();
                foreach (IElement E in HTMLWorker.ParseToList(new StringReader(formato.for_pie.Replace("%total%", Functions.Conversiones.NumeroALetras(comp.com_total.ToString()))), null))
                    ppie.Add(E);
                //ppie.SetLeading(0.0f, 1.0f);
                ppie.SetLeading(2f, 0f);
                document.Add(ppie);
                document.Close();

            }
            catch (Exception ex)
            {
                document.Close();
            }

            return file;
        }
    }
}


#region Itex Events

public class ITextEvents : PdfPageEventHelper
{

    // This is the contentbyte object of the writer
    PdfContentByte cb;

    // we will put the final number of pages in a template
    PdfTemplate headerTemplate, footerTemplate;

    // this is the BaseFont we are going to use for the header / footer
    BaseFont bf = null;

    // This keeps track of the creation time
    DateTime PrintTime = DateTime.Now;


    #region Fields
    private string _header;
    #endregion

    #region Properties
    public string Header
    {
        get { return _header; }
        set { _header = value; }
    }
    #endregion


    public override void OnOpenDocument(PdfWriter writer, Document document)
    {
        try
        {
            PrintTime = DateTime.Now;
            bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            cb = writer.DirectContent;
            headerTemplate = cb.CreateTemplate(100, 100);
            footerTemplate = cb.CreateTemplate(50, 50);
        }
        catch (DocumentException de)
        {
            //handle exception here
        }
        catch (System.IO.IOException ioe)
        {
            //handle exception here
        }
    }

    public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
    {
        base.OnEndPage(writer, document);

        iTextSharp.text.Font baseFontNormal = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);

        iTextSharp.text.Font baseFontBig = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);

        //Phrase p1Header = new Phrase("Sample Header Here", baseFontNormal);

        //Create PdfTable object
        PdfPTable pdfTab = new PdfPTable(2);

        #region Logo TAO
        //Agrega el logo
        string path = Constantes.GetParameter("pathfiles");
        string imageURL = path + "\\logos\\logosiac.png";
        iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imageURL);


        PdfPCell cinfo_logo = new PdfPCell(logo, true);
        cinfo_logo.BorderWidth = 0;
        cinfo_logo.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
        //cinfo_logo.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
        cinfo_logo.PaddingBottom = 5;
        cinfo_logo.FixedHeight = 20;

        #endregion

        Chunk link = new Chunk("Generado por: www.siac.com.ec", baseFontBig);
        link.SetAnchor("http://www.siac.com.ec");

        PdfPCell clink = new PdfPCell(new Phrase(link));
        clink.Border = 0;
        pdfTab.AddCell(clink);

        pdfTab.AddCell(cinfo_logo);



        String text = "Página " + writer.PageNumber + " de ";


        ////Add paging to header
        //{
        //    cb.BeginText();
        //    cb.SetFontAndSize(bf, 12);
        //    cb.SetTextMatrix(document.PageSize.GetRight(200), document.PageSize.GetTop(45));
        //    cb.ShowText(text);
        //    cb.EndText();
        //    float len = bf.GetWidthPoint(text, 12);
        //    //Adds "12" in Page 1 of 12
        //    cb.AddTemplate(headerTemplate, document.PageSize.GetRight(200) + len, document.PageSize.GetTop(45));
        //}
        //Add paging to footer
        {
            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.SetTextMatrix(document.PageSize.GetRight(80), document.PageSize.GetBottom(12));
            cb.ShowText(text);
            cb.EndText();
            float len = bf.GetWidthPoint(text, 8);
            cb.AddTemplate(footerTemplate, document.PageSize.GetRight(80) + len, document.PageSize.GetBottom(12));
        }


        pdfTab.TotalWidth = document.PageSize.Width - 80f;
        pdfTab.WidthPercentage = 70;



        //call WriteSelectedRows of PdfTable. This writes rows from PdfWriter in PdfTable
        //first param is start row. -1 indicates there is no end row and all the rows to be included to write
        //Third and fourth param is x and y position to start writing
        pdfTab.WriteSelectedRows(0, -1, 20, document.Bottom, writer.DirectContent);

        ////Move the pointer and draw line to separate header section from rest of page
        //cb.MoveTo(40, document.PageSize.Height - 100);
        //cb.LineTo(document.PageSize.Width - 40, document.PageSize.Height - 100);
        //cb.Stroke();

        //Move the pointer and draw line to separate footer section from rest of page
        //cb.MoveTo(20, document.PageSize.GetBottom(20));
        //cb.LineTo(document.PageSize.Width - 20, document.PageSize.GetBottom(20));
        //cb.Stroke();
    }

    public override void OnCloseDocument(PdfWriter writer, Document document)
    {
        base.OnCloseDocument(writer, document);

        //headerTemplate.BeginText();
        //headerTemplate.SetFontAndSize(bf, 12);
        //headerTemplate.SetTextMatrix(0, 0);
        //headerTemplate.ShowText((writer.PageNumber - 1).ToString());
        //headerTemplate.EndText();

        footerTemplate.BeginText();
        footerTemplate.SetFontAndSize(bf, 8);
        footerTemplate.SetTextMatrix(0, 0);
        //footerTemplate.ShowText((writer.PageNumber - 1).ToString());
        footerTemplate.ShowText((writer.PageNumber).ToString());
        footerTemplate.EndText();


    }
}

#endregion