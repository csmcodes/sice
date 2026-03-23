using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using BusinessObjects;
using BusinessLogicLayer;
using Services;
using System.Web.Script.Serialization;
using System.Collections;

namespace Packages
{
    public class XmlReader
    {



        public XmlReader()
        {

        }

        public static string GetString(XmlNode nodo)
        {
            if (nodo != null)
                return nodo.InnerText;
            else
                return null;
        }

        public static decimal? GetDecimal(XmlNode nodo)
        {
            if (nodo != null)
            {
                return GetDecimal(nodo.InnerText);
            }
            return null;
        }

        public static decimal GetDecimal(string valor)
        {
            decimal valordecimal = 0;
            decimal.TryParse(valor.Replace('.', ','), out valordecimal);
            return valordecimal;
        }

        public static int? GetEntero(XmlNode nodo)
        {
            if (nodo != null)
            {
                return GetEntero(nodo.InnerText);
            }
            return null;
        }

        public static int GetEntero(string valor)
        {
            int valorentero = 0;
            int.TryParse(valor.Replace('.', ','), out valorentero);
            return valorentero;
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
            DateTime date;
            DateTime.TryParse(valor, out date);
            return date;
        }

        public static Comprobante CargarFactura(Comprobante comprobante)
        {


            Archivo arc = new Archivo();
            arc.arc_empresa = comprobante.com_empresa;
            arc.arc_empresa_key = comprobante.com_empresa;
            arc.arc_numero = comprobante.com_numero;
            arc.arc_numero_key = comprobante.com_numero;
            arc = ArchivoBLL.GetByPK(arc);
            comprobante = CargarFactura(comprobante, arc.arc_xml);
            return comprobante;

        }


        public static Comprobante CargarFactura(Comprobante comprobante, string xml)
        {

            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ",";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            var serializer = new JavaScriptSerializer();

            if (!string.IsNullOrEmpty(xml))
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(xml);

                var tipo = xmldoc.SelectSingleNode("/").LastChild.Name;
                var version = xmldoc.SelectSingleNode("/").LastChild.Attributes["version"].Value;
                if (tipo.ToUpper() == "FACTURA")
                {

                    comprobante.com_numero = xmldoc.SelectSingleNode("/factura/infoTributaria/claveAcceso").InnerText;
                    comprobante.com_almacen = xmldoc.SelectSingleNode("/factura/infoTributaria/estab").InnerText;
                    comprobante.com_pventa = xmldoc.SelectSingleNode("/factura/infoTributaria/ptoEmi").InnerText;
                    comprobante.com_secuencia = xmldoc.SelectSingleNode("/factura/infoTributaria/secuencial").InnerText;
                    comprobante.com_fecha = GetDateTime(xmldoc.SelectSingleNode("/factura/infoFactura/fechaEmision"));
                    comprobante.com_fechastr = GetString(xmldoc.SelectSingleNode("/factura/infoFactura/fechaEmision"));
                    //comprobante.com_autorizacion = 
                    comprobante.com_ambiente = int.Parse(xmldoc.SelectSingleNode("/factura/infoTributaria/ambiente").InnerText);
                    comprobante.com_emision = int.Parse(xmldoc.SelectSingleNode("/factura/infoTributaria/tipoEmision").InnerText);
                    comprobante.com_contribuyente = GetString(xmldoc.SelectSingleNode("/factura/infoFactura/contribuyenteEspecial"));
                    comprobante.com_contabilidad = xmldoc.SelectSingleNode("/factura/infoFactura/obligadoContabilidad").InnerText;
                    XmlNode rimpe = xmldoc.SelectSingleNode("/factura/infoTributaria/contribuyenteRimpe");
                    if (rimpe!=null)
                        comprobante.com_regimenrimpe = rimpe.InnerText;

                    comprobante.com_direccionmatriz = GetString(xmldoc.SelectSingleNode("/factura/infoTributaria/dirMatriz"));
                    comprobante.com_direccionsucursal = GetString(xmldoc.SelectSingleNode("/factura/infoFactura/dirEstablecimiento"));
                    comprobante.com_agenteret = GetString(xmldoc.SelectSingleNode("/factura/infoTributaria/agenteRetencion"));
                    comprobante.com_regimenmicro= GetString(xmldoc.SelectSingleNode("/factura/infoTributaria/regimenMicroempresas"));

                    comprobante.com_ruccliente = xmldoc.SelectSingleNode("/factura/infoFactura/identificacionComprador").InnerText;
                    comprobante.com_nombrecliente = xmldoc.SelectSingleNode("/factura/infoFactura/razonSocialComprador").InnerText;

                    comprobante.com_tipoidcliente = xmldoc.SelectSingleNode("/factura/infoFactura/tipoIdentificacionComprador").InnerText;
                    //comprobante.com_guiaremision falta implementar
                    comprobante.com_direccioncliente = GetString(xmldoc.SelectSingleNode("/factura/infoFactura/direccionComprador"));





                    comprobante.com_subtotalsinimp = GetDecimal(xmldoc.SelectSingleNode("/factura/infoFactura/totalSinImpuestos").InnerText);
                    comprobante.com_descuento = GetDecimal(xmldoc.SelectSingleNode("/factura/infoFactura/totalDescuento").InnerText);
                    comprobante.com_total = GetDecimal(xmldoc.SelectSingleNode("/factura/infoFactura/importeTotal").InnerText);

                    XmlNode totalimpuestos = xmldoc.SelectSingleNode("/factura/infoFactura/totalConImpuestos");

                    decimal porciva = 0;
                    
                    if (comprobante.com_fecha.HasValue)
                        porciva = Constantes.GetValorIVA(comprobante.com_fecha);
                    decimal subtotaliva = 0;
                    decimal valoriva = 0;
                    decimal subtotalice = 0;
                    decimal valorice = 0;
                    decimal subtotal0 = 0;
                    decimal subtotalnoiva = 0;
                    decimal subtotalextiva = 0;

                    foreach (XmlNode totalimp in totalimpuestos.ChildNodes)
                    {
                        XmlNode codigo = totalimp.SelectSingleNode("codigo");
                        XmlNode codigoporcentaje = totalimp.SelectSingleNode("codigoPorcentaje");
                        XmlNode descuento = totalimp.SelectSingleNode("descuentoAdicional");
                        XmlNode baseimp = totalimp.SelectSingleNode("baseImponible");
                        XmlNode tarifa = totalimp.SelectSingleNode("tarifa");
                        XmlNode valor = totalimp.SelectSingleNode("valor");


                        //if (codigo.InnerText == "2" && (codigoporcentaje.InnerText == "2" || codigoporcentaje.InnerText == "3"))//TARIFA 12 o 14
                        if (codigo.InnerText == "2" && (codigoporcentaje.InnerText == "2" || codigoporcentaje.InnerText == "3" || codigoporcentaje.InnerText == "4" || codigoporcentaje.InnerText == "10"))//TARIFA 12 13 14 15

                        {
                            subtotaliva += GetDecimal(baseimp.InnerText);
                            valoriva += GetDecimal(valor.InnerText);
                            if (codigoporcentaje.InnerText == "2")
                                porciva = 12;
                            if (codigoporcentaje.InnerText == "3")
                                porciva = 14;
                            if (codigoporcentaje.InnerText == "4")
                                porciva = 15;
                            if (codigoporcentaje.InnerText == "10")
                                porciva = 13;
                        }
                        if (codigo.InnerText == "2" && codigoporcentaje.InnerText == "0")//TARIFA 0
                        {
                            subtotal0 += GetDecimal(baseimp.InnerText);
                        }
                        if (codigo.InnerText == "2" && codigoporcentaje.InnerText == "6")//NO OBJETO DE IVA
                        {
                            subtotalnoiva += GetDecimal(baseimp.InnerText);
                        }
                        if (codigo.InnerText == "2" && codigoporcentaje.InnerText == "7")//EXENTO DE IVA
                        {
                            subtotalextiva += GetDecimal(baseimp.InnerText);
                        }
                        if (codigo.InnerText == "3")//TARIFA ICE
                        {
                            subtotalice += GetDecimal(baseimp.InnerText);
                            valorice += GetDecimal(valor.InnerText);
                        }
                    }
                    comprobante.com_subtotal0 = subtotal0;
                    comprobante.com_subtotaliva = subtotaliva;
                    comprobante.com_subtotalnoiva = subtotalnoiva;
                    comprobante.com_subtotalextiva = subtotalextiva;
                    comprobante.com_iva = valoriva;
                    comprobante.com_porciva = porciva;
                    comprobante.com_ice = valorice;
                    comprobante.com_iva0 = 0;

                    //Validacion SUBTOTALES por ERRORES DE ENVIO 
                    //ESTO SOLO PARA COMPROBANES DE FORMATO 2 y EMPRESA=1 INMOT

                    if (comprobante.com_formato == 2 && comprobante.com_empresa == 1)
                    {
                        decimal subtotal0_1 = comprobante.com_total.Value - (subtotaliva + valoriva);
                        if (subtotal0_1 > subtotal0)
                        {
                            comprobante.com_subtotal0 = subtotal0_1;
                        }
                        decimal subtotaliva_1 = comprobante.com_subtotalsinimp.Value + comprobante.com_descuento.Value - comprobante.com_subtotal0.Value;
                        //if (subtotaliva_1 >subtotaliva)
                        //{
                        comprobante.com_subtotaliva = subtotaliva_1;
                        //}

                        decimal totalsinimp_1 = comprobante.com_subtotal0.Value + comprobante.com_subtotaliva.Value;
                        if (totalsinimp_1 > comprobante.com_subtotalsinimp.Value)
                            comprobante.com_subtotalsinimp = totalsinimp_1;
                    }

                    XmlNode infoadicional = xmldoc.SelectSingleNode("/factura/infoAdicional");

                    string observacion = "";
                    string direccion = "";
                    string telefono = "";
                    string vendedor = "";
                    string ciudad = "";
                    string fpago = "";
                    string transporte = "";
                    string compensacion = "";

                    List<String> adicionales = new List<string>(); 
                    if (infoadicional != null)
                    {
                        foreach (XmlNode iteminfo in infoadicional.ChildNodes)
                        {

                            if (iteminfo.Attributes["nombre"].Value == "Direccion")
                                comprobante.com_direccioncliente = iteminfo.InnerText;
                            else if (iteminfo.Attributes["nombre"].Value == "Telefono")
                                comprobante.com_telefonocliente = iteminfo.InnerText;
                            else if (iteminfo.Attributes["nombre"].Value == "Email")
                            {
                                if (string.IsNullOrEmpty(comprobante.com_email))
                                    comprobante.com_email = iteminfo.InnerText;
                            }
                            else if (iteminfo.Attributes["nombre"].Value == "Vendedor")
                                comprobante.com_adicional1 = iteminfo.InnerText;
                            else if (iteminfo.Attributes["nombre"].Value == "Ciudad")
                                comprobante.com_adicional2 = iteminfo.InnerText;
                            else if (iteminfo.Attributes["nombre"].Value == "Fpago")
                                comprobante.com_adicional3 = iteminfo.InnerText;
                            else if (iteminfo.Attributes["nombre"].Value == "Transporte")
                                comprobante.com_adicional4 = iteminfo.InnerText;
                            else if (iteminfo.Attributes["nombre"].Value == "Observacion")
                                comprobante.com_adicional5 = iteminfo.InnerText;
                            else if (iteminfo.Attributes["nombre"].Value == "Compensado")
                                comprobante.com_adicional6 = iteminfo.InnerText;
                            else
                                adicionales.Add(iteminfo.Attributes["nombre"].Value + "|" + iteminfo.InnerText);

                            


                        }

                    }
                    comprobante.com_adicionales = adicionales;


                    /*PAGOS*/
                    XmlNode pagos = xmldoc.SelectSingleNode("/factura/infoFactura/pagos");


                    List<FormaPago> formas = serializer.Deserialize<List<FormaPago>>(Constantes.GetParameter("formaspago"));

                    comprobante.formas = new List<Formapago>();

                    if (pagos != null)
                    {
                        foreach (XmlNode pago in pagos.ChildNodes)
                        {
                            Formapago fp = new Formapago();
                            fp.secuencia = comprobante.formas.Count() + 1;
                            fp.codigo = pago.SelectSingleNode("formaPago").InnerText;
                            FormaPago forma = formas.Find(delegate (FormaPago f) { return f.codigo == fp.codigo; });
                            fp.forma = forma.forma;
                            fp.valor = GetDecimal(pago.SelectSingleNode("total"));
                            fp.plazo = GetEntero(pago.SelectSingleNode("plazo"));
                            fp.tiempo = GetString(pago.SelectSingleNode("unidadTiempo"));
                            comprobante.formas.Add(fp);

                        }
                    }

                    /*DETALLES*/
                    XmlNode detalles = xmldoc.SelectSingleNode("/factura/detalles");
                    comprobante.detalle = new List<Detalle>();

                    //List<CodigoImpuesto> lstiva = new JavaScriptSerializer().Deserialize<List<CodigoImpuesto>>(Constantes.GetParameterValue("codigosiva"));
                    //List<CodigoImpuesto> lstice = new JavaScriptSerializer().Deserialize<List<CodigoImpuesto>>(Constantes.GetParameterValue("codigosice"));

                    foreach (XmlNode detalle in detalles.ChildNodes)
                    {
                        Detalle det = new Detalle();
                        det.secuencia = comprobante.detalle.Count() + 1;
                        det.codigo = GetString(detalle.SelectSingleNode("codigoPrincipal"));
                        det.codigoaux = GetString(detalle.SelectSingleNode("codigoAuxiliar"));
                        det.descripcion = GetString(detalle.SelectSingleNode("descripcion"));
                        det.cantidad = GetDecimal(detalle.SelectSingleNode("cantidad").InnerText);
                        det.precio = GetDecimal(detalle.SelectSingleNode("precioUnitario").InnerText);
                        det.descuento = GetDecimal(detalle.SelectSingleNode("descuento").InnerText);
                        det.totalsinimp = GetDecimal(detalle.SelectSingleNode("precioTotalSinImpuesto").InnerText);

                        XmlNode impuestosdet = detalle.SelectSingleNode("impuestos");
                        foreach (XmlNode impuesto in impuestosdet.ChildNodes)
                        {
                            if (impuesto.SelectSingleNode("codigo").InnerText == "2")//IVA
                            {
                                det.codiva = GetString(impuesto.SelectSingleNode("codigoPorcentaje"));
                                det.porciva = GetDecimal(impuesto.SelectSingleNode("tarifa").InnerText);
                                det.valiva = GetDecimal(impuesto.SelectSingleNode("valor").InnerText);
                            }
                            if (impuesto.SelectSingleNode("codigo").InnerText == "3")//ICE
                            {
                                det.codice = GetString(impuesto.SelectSingleNode("codigoPorcentaje"));
                                det.porcice = GetDecimal(impuesto.SelectSingleNode("tarifa").InnerText);
                                det.valice = GetDecimal(impuesto.SelectSingleNode("valor").InnerText);
                            }

                        }

                        XmlNode adicionalesdet = detalle.SelectSingleNode("detallesAdicionales");
                        if (adicionalesdet != null)
                        {
                            foreach (XmlNode detadi in adicionalesdet.ChildNodes)
                            {
                                //if (detadi.Attributes["nombre"].Value == "detadicional1")
                                //    det.adicional1 = detadi.Attributes["valor"].Value;
                                //if (detadi.Attributes["nombre"].Value == "detadicional2")
                                //    det.adicional2 = detadi.Attributes["valor"].Value;
                                //if (detadi.Attributes["nombre"].Value == "detadicional3")
                                //    det.adicional3 = detadi.Attributes["valor"].Value;

                                if (detadi.Attributes["nombre"].Value == "LINEA1")
                                    det.adicional1 = detadi.Attributes["valor"].Value;
                                if (detadi.Attributes["nombre"].Value == "LINEA2")
                                    det.adicional2 = detadi.Attributes["valor"].Value;
                                if (detadi.Attributes["nombre"].Value == "LINEA3")
                                    det.adicional3 = detadi.Attributes["valor"].Value;

                            }
                        }


                        comprobante.detalle.Add(det);

                    }


                }
                if (tipo.ToUpper() == "COMPROBANTERETENCION")
                {

                    comprobante.com_numero = xmldoc.SelectSingleNode("/comprobanteRetencion/infoTributaria/claveAcceso").InnerText;
                    comprobante.com_almacen = xmldoc.SelectSingleNode("/comprobanteRetencion/infoTributaria/estab").InnerText;
                    comprobante.com_pventa = xmldoc.SelectSingleNode("/comprobanteRetencion/infoTributaria/ptoEmi").InnerText;
                    comprobante.com_secuencia = xmldoc.SelectSingleNode("/comprobanteRetencion/infoTributaria/secuencial").InnerText;
                    comprobante.com_fecha = GetDateTime(xmldoc.SelectSingleNode("/comprobanteRetencion/infoCompRetencion/fechaEmision"));
                    comprobante.com_fechastr = GetString(xmldoc.SelectSingleNode("/comprobanteRetencion/infoCompRetencion/fechaEmision"));
                    //comprobante.com_autorizacion = 
                    comprobante.com_ambiente = int.Parse(xmldoc.SelectSingleNode("/comprobanteRetencion/infoTributaria/ambiente").InnerText);
                    comprobante.com_emision = int.Parse(xmldoc.SelectSingleNode("/comprobanteRetencion/infoTributaria/tipoEmision").InnerText);
                    comprobante.com_contribuyente = GetString(xmldoc.SelectSingleNode("/comprobanteRetencion/infoCompRetencion/contribuyenteEspecial"));
                    comprobante.com_contabilidad = xmldoc.SelectSingleNode("/comprobanteRetencion/infoCompRetencion/obligadoContabilidad").InnerText;

                    XmlNode rimpe = xmldoc.SelectSingleNode("/comprobanteRetencion/infoTributaria/contribuyenteRimpe");
                    if (rimpe != null)
                        comprobante.com_regimenrimpe = rimpe.InnerText;

                    comprobante.com_direccionmatriz = GetString(xmldoc.SelectSingleNode("/comprobanteRetencion/infoTributaria/dirMatriz"));
                    comprobante.com_direccionsucursal = GetString(xmldoc.SelectSingleNode("/comprobanteRetencion/infoCompRetencion/dirEstablecimiento"));
                    comprobante.com_agenteret = GetString(xmldoc.SelectSingleNode("/comprobanteRetencion/infoTributaria/agenteRetencion"));
                    comprobante.com_regimenmicro = GetString(xmldoc.SelectSingleNode("/comprobanteRetencion/infoTributaria/regimenMicroempresas"));


                    comprobante.com_ruccliente = xmldoc.SelectSingleNode("/comprobanteRetencion/infoCompRetencion/identificacionSujetoRetenido").InnerText;
                    comprobante.com_nombrecliente = xmldoc.SelectSingleNode("/comprobanteRetencion/infoCompRetencion/razonSocialSujetoRetenido").InnerText;
                    comprobante.com_tipoidcliente = xmldoc.SelectSingleNode("/comprobanteRetencion/infoCompRetencion/tipoIdentificacionSujetoRetenido").InnerText;

                    comprobante.com_periodofiscal = xmldoc.SelectSingleNode("/comprobanteRetencion/infoCompRetencion/periodoFiscal").InnerText;

                    if (version == "2.0.0")
                    {
                        XmlNode docsSustento = xmldoc.SelectSingleNode("/comprobanteRetencion/docsSustento");
                        comprobante.detalle = new List<Detalle>();

                        //CALCULO DE TOTAL DE LA RETENCION
                        foreach (XmlNode docsus in docsSustento.ChildNodes)
                        {
                            XmlNode retenciones = docsus.SelectSingleNode("retenciones");
                            foreach (XmlNode detalle in retenciones.ChildNodes)
                            {
                                Detalle det = new Detalle();
                                det.secuencia = comprobante.detalle.Count() + 1;
                                det.codigo = GetString(detalle.SelectSingleNode("codigo"));
                                det.codigoretencion = GetString(detalle.SelectSingleNode("codigoRetencion"));

                                det.baseimponible = GetDecimal(detalle.SelectSingleNode("baseImponible"));
                                det.porcentajeretener = GetDecimal(detalle.SelectSingleNode("porcentajeRetener"));
                                det.valorretenido = GetDecimal(detalle.SelectSingleNode("valorRetenido"));

                                det.codigodocsustento = GetString(docsus.SelectSingleNode("codDocSustento"));
                                det.numdocsustento = GetString(docsus.SelectSingleNode("numDocSustento"));
                                det.fechaemisiondocsustento = GetString(docsus.SelectSingleNode("fechaEmisionDocSustento"));
                                comprobante.detalle.Add(det);

                            }
                        }


                        comprobante.com_total = comprobante.detalle.Sum(s => s.valorretenido.Value);
                    }
                    else
                    {
                        XmlNode detalles = xmldoc.SelectSingleNode("/comprobanteRetencion/impuestos");
                        comprobante.detalle = new List<Detalle>();

                        foreach (XmlNode detalle in detalles.ChildNodes)
                        {
                            Detalle det = new Detalle();
                            det.secuencia = comprobante.detalle.Count() + 1;
                            det.codigo = GetString(detalle.SelectSingleNode("codigo"));
                            det.codigoretencion = GetString(detalle.SelectSingleNode("codigoRetencion"));

                            det.baseimponible = GetDecimal(detalle.SelectSingleNode("baseImponible"));
                            det.porcentajeretener = GetDecimal(detalle.SelectSingleNode("porcentajeRetener"));
                            det.valorretenido = GetDecimal(detalle.SelectSingleNode("valorRetenido"));
                            det.codigodocsustento = GetString(detalle.SelectSingleNode("codDocSustento"));
                            det.numdocsustento = GetString(detalle.SelectSingleNode("numDocSustento"));
                            det.fechaemisiondocsustento = GetString(detalle.SelectSingleNode("fechaEmisionDocSustento"));
                            comprobante.detalle.Add(det);

                        }

                        comprobante.com_total = comprobante.detalle.Sum(s => s.valorretenido.Value);
                    }
                    XmlNode infoadicional = xmldoc.SelectSingleNode("/comprobanteRetencion/infoAdicional");

                    string observacion = "";
                    string direccion = "";
                    string telefono = "";
                    string vendedor = "";
                    string ciudad = "";
                    string fpago = "";
                    string transporte = "";
                    string compensacion = "";


                    if (infoadicional != null)
                    {
                        foreach (XmlNode iteminfo in infoadicional.ChildNodes)
                        {

                            if (iteminfo.Attributes["nombre"].Value == "Direccion")
                                comprobante.com_direccioncliente = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Telefono")
                                comprobante.com_telefonocliente = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Email")
                                comprobante.com_email = iteminfo.InnerText;

                            if (iteminfo.Attributes["nombre"].Value == "Vendedor")
                                comprobante.com_adicional1 = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Ciudad")
                                comprobante.com_adicional2 = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Fpago")
                                comprobante.com_adicional3 = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Transporte")
                                comprobante.com_adicional4 = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Observacion")
                                comprobante.com_adicional5 = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Compensado")
                                comprobante.com_adicional6 = iteminfo.InnerText;

                        }

                    }
                }


                if (tipo.ToUpper() == "NOTACREDITO")
                {

                    comprobante.com_numero = xmldoc.SelectSingleNode("/notaCredito/infoTributaria/claveAcceso").InnerText;
                    comprobante.com_almacen = xmldoc.SelectSingleNode("/notaCredito/infoTributaria/estab").InnerText;
                    comprobante.com_pventa = xmldoc.SelectSingleNode("/notaCredito/infoTributaria/ptoEmi").InnerText;
                    comprobante.com_secuencia = xmldoc.SelectSingleNode("/notaCredito/infoTributaria/secuencial").InnerText;
                    comprobante.com_fecha = GetDateTime(xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/fechaEmision"));
                    comprobante.com_fechastr = GetString(xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/fechaEmision"));
                    //comprobante.com_autorizacion = 
                    comprobante.com_ambiente = int.Parse(xmldoc.SelectSingleNode("/notaCredito/infoTributaria/ambiente").InnerText);
                    comprobante.com_emision = int.Parse(xmldoc.SelectSingleNode("/notaCredito/infoTributaria/tipoEmision").InnerText);
                    comprobante.com_contribuyente = GetString(xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/contribuyenteEspecial"));
                    comprobante.com_contabilidad = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/obligadoContabilidad").InnerText;

                    XmlNode rimpe = xmldoc.SelectSingleNode("/notaCredito/infoTributaria/contribuyenteRimpe");
                    if (rimpe != null)
                        comprobante.com_regimenrimpe = rimpe.InnerText;

                    comprobante.com_direccionmatriz = GetString(xmldoc.SelectSingleNode("/notaCredito/infoTributaria/dirMatriz"));
                    comprobante.com_direccionsucursal = GetString(xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/dirEstablecimiento"));
                    comprobante.com_agenteret = GetString(xmldoc.SelectSingleNode("/notaCredito/infoTributaria/agenteRetencion"));
                    comprobante.com_regimenmicro = GetString(xmldoc.SelectSingleNode("/notaCredito/infoTributaria/regimenMicroempresas"));

                    comprobante.com_ruccliente = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/identificacionComprador").InnerText;
                    comprobante.com_nombrecliente = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/razonSocialComprador").InnerText;
                    comprobante.com_tipoidcliente = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/tipoIdentificacionComprador").InnerText;
                    comprobante.com_motivo= xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/motivo").InnerText;

                    comprobante.com_rise = GetString(xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/rise"));
                    comprobante.com_coddocmodificado = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/codDocModificado").InnerText;
                    comprobante.com_numdocmodificado= xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/numDocModificado").InnerText;
                    comprobante.com_fechaemisiondocsustento= xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/fechaEmisionDocSustento").InnerText;


                    comprobante.com_subtotalsinimp = GetDecimal(xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/totalSinImpuestos").InnerText);
                    comprobante.com_valormodificacion= GetDecimal(xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/valorModificacion").InnerText);
                    

                    XmlNode totalimpuestos = xmldoc.SelectSingleNode("/notaCredito/infoNotaCredito/totalConImpuestos");

                    decimal porciva = 0;
                    decimal subtotaliva = 0;
                    decimal valoriva = 0;
                    decimal subtotalice = 0;
                    decimal valorice = 0;
                    decimal subtotal0 = 0;
                    decimal subtotalnoiva = 0;
                    decimal subtotalextiva = 0;

                    foreach (XmlNode totalimp in totalimpuestos.ChildNodes)
                    {
                        XmlNode codigo = totalimp.SelectSingleNode("codigo");
                        XmlNode codigoporcentaje = totalimp.SelectSingleNode("codigoPorcentaje");
                        XmlNode baseimp = totalimp.SelectSingleNode("baseImponible");
                        XmlNode valor = totalimp.SelectSingleNode("valor");


                        if (codigo.InnerText == "2" && (codigoporcentaje.InnerText == "2" || codigoporcentaje.InnerText == "3" || codigoporcentaje.InnerText == "4" || codigoporcentaje.InnerText == "10"))//TARIFA 12 13 14 15
                        {
                            subtotaliva += GetDecimal(baseimp.InnerText);
                            valoriva += GetDecimal(valor.InnerText);
                            if (codigoporcentaje.InnerText == "2")
                                porciva = 12;
                            if (codigoporcentaje.InnerText == "3")
                                porciva = 14;
                            if (codigoporcentaje.InnerText == "4")
                                porciva = 15;
                            if (codigoporcentaje.InnerText == "10")
                                porciva = 13;
                        }
                        if (codigo.InnerText == "2" && codigoporcentaje.InnerText == "0")//TARIFA 0
                        {
                            subtotal0 += GetDecimal(baseimp.InnerText);
                        }
                        if (codigo.InnerText == "2" && codigoporcentaje.InnerText == "6")//NO OBJETO DE IVA
                        {
                            subtotalnoiva += GetDecimal(baseimp.InnerText);
                        }
                        if (codigo.InnerText == "2" && codigoporcentaje.InnerText == "7")//EXENTO DE IVA
                        {
                            subtotalextiva += GetDecimal(baseimp.InnerText);
                        }
                        if (codigo.InnerText == "3")//TARIFA ICE
                        {
                            subtotalice += GetDecimal(baseimp.InnerText);
                            valorice += GetDecimal(valor.InnerText);
                        }
                    }
                    comprobante.com_subtotal0 = subtotal0;
                    comprobante.com_subtotaliva = subtotaliva;
                    comprobante.com_subtotalnoiva = subtotalnoiva;
                    comprobante.com_subtotalextiva = subtotalextiva;
                    comprobante.com_iva = valoriva;
                    comprobante.com_porciva = porciva;
                    comprobante.com_ice = valorice;
                    comprobante.com_iva0 = 0;


                    /*DETALLES*/
                    XmlNode detalles = xmldoc.SelectSingleNode("/notaCredito/detalles");
                    comprobante.detalle = new List<Detalle>();

                    //List<CodigoImpuesto> lstiva = new JavaScriptSerializer().Deserialize<List<CodigoImpuesto>>(Constantes.GetParameterValue("codigosiva"));
                    //List<CodigoImpuesto> lstice = new JavaScriptSerializer().Deserialize<List<CodigoImpuesto>>(Constantes.GetParameterValue("codigosice"));

                    foreach (XmlNode detalle in detalles.ChildNodes)
                    {
                        Detalle det = new Detalle();
                        det.secuencia = comprobante.detalle.Count() + 1;
                        det.codigo = GetString(detalle.SelectSingleNode("codigoInterno"));
                        det.codigoaux = GetString(detalle.SelectSingleNode("codigoAdicional"));
                        det.descripcion = GetString(detalle.SelectSingleNode("descripcion"));
                        det.cantidad = GetDecimal(detalle.SelectSingleNode("cantidad").InnerText);
                        det.precio = GetDecimal(detalle.SelectSingleNode("precioUnitario").InnerText);
                        det.descuento = GetDecimal(detalle.SelectSingleNode("descuento").InnerText);
                        det.totalsinimp = GetDecimal(detalle.SelectSingleNode("precioTotalSinImpuesto").InnerText);

                        XmlNode impuestosdet = detalle.SelectSingleNode("impuestos");
                        foreach (XmlNode impuesto in impuestosdet.ChildNodes)
                        {
                            if (impuesto.SelectSingleNode("codigo").InnerText == "2")//IVA
                            {
                                det.codiva = GetString(impuesto.SelectSingleNode("codigoPorcentaje"));
                                det.porciva = GetDecimal(impuesto.SelectSingleNode("tarifa").InnerText);
                                det.valiva = GetDecimal(impuesto.SelectSingleNode("valor").InnerText);
                            }
                            if (impuesto.SelectSingleNode("codigo").InnerText == "3")//ICE
                            {
                                det.codice = GetString(impuesto.SelectSingleNode("codigoPorcentaje"));
                                det.porcice = GetDecimal(impuesto.SelectSingleNode("tarifa").InnerText);
                                det.valice = GetDecimal(impuesto.SelectSingleNode("valor").InnerText);
                            }

                        }

                        XmlNode adicionalesdet = detalle.SelectSingleNode("detallesAdicionales");
                        if (adicionalesdet != null)
                        {
                            foreach (XmlNode detadi in adicionalesdet.ChildNodes)
                            {
                                //if (detadi.Attributes["nombre"].Value == "detadicional1")
                                //    det.adicional1 = detadi.Attributes["valor"].Value;
                                //if (detadi.Attributes["nombre"].Value == "detadicional2")
                                //    det.adicional2 = detadi.Attributes["valor"].Value;
                                //if (detadi.Attributes["nombre"].Value == "detadicional3")
                                //    det.adicional3 = detadi.Attributes["valor"].Value;

                                if (detadi.Attributes["nombre"].Value == "LINEA1")
                                    det.adicional1 = detadi.Attributes["valor"].Value;
                                if (detadi.Attributes["nombre"].Value == "LINEA2")
                                    det.adicional2 = detadi.Attributes["valor"].Value;
                                if (detadi.Attributes["nombre"].Value == "LINEA3")
                                    det.adicional3 = detadi.Attributes["valor"].Value;

                            }
                        }


                        comprobante.detalle.Add(det);

                    }




                    XmlNode infoadicional = xmldoc.SelectSingleNode("/notaCredito/infoAdicional");

                    string observacion = "";
                    string direccion = "";
                    string telefono = "";
                    string vendedor = "";
                    string ciudad = "";
                    string fpago = "";
                    string transporte = "";
                    string compensacion = "";


                    if (infoadicional != null)
                    {
                        foreach (XmlNode iteminfo in infoadicional.ChildNodes)
                        {

                            if (iteminfo.Attributes["nombre"].Value == "Direccion")
                                comprobante.com_direccioncliente = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Telefono")
                                comprobante.com_telefonocliente = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Email")
                                comprobante.com_email = iteminfo.InnerText;

                            if (iteminfo.Attributes["nombre"].Value == "Vendedor")
                                comprobante.com_adicional1 = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Ciudad")
                                comprobante.com_adicional2 = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Fpago")
                                comprobante.com_adicional3 = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Transporte")
                                comprobante.com_adicional4 = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Observacion")
                                comprobante.com_adicional5 = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Compensado")
                                comprobante.com_adicional6 = iteminfo.InnerText;

                        }

                    }
                }
                if (tipo.ToUpper() == "NOTADEBITO")
                {

                    comprobante.com_numero = xmldoc.SelectSingleNode("/notaDebito/infoTributaria/claveAcceso").InnerText;
                    comprobante.com_almacen = xmldoc.SelectSingleNode("/notaDebito/infoTributaria/estab").InnerText;
                    comprobante.com_pventa = xmldoc.SelectSingleNode("/notaDebito/infoTributaria/ptoEmi").InnerText;
                    comprobante.com_secuencia = xmldoc.SelectSingleNode("/notaDebito/infoTributaria/secuencial").InnerText;
                    comprobante.com_fecha = GetDateTime(xmldoc.SelectSingleNode("/notaDebito/infoNotaDebito/fechaEmision"));
                    comprobante.com_fechastr = GetString(xmldoc.SelectSingleNode("/notaDebito/infoNotaDebito/fechaEmision"));
                    //comprobante.com_autorizacion = 
                    comprobante.com_ambiente = int.Parse(xmldoc.SelectSingleNode("/notaDebito/infoTributaria/ambiente").InnerText);
                    comprobante.com_emision = int.Parse(xmldoc.SelectSingleNode("/notaDebito/infoTributaria/tipoEmision").InnerText);
                    comprobante.com_contribuyente = GetString(xmldoc.SelectSingleNode("/notaDebito/infoNotaDebito/contribuyenteEspecial"));
                    comprobante.com_contabilidad = xmldoc.SelectSingleNode("/notaDebito/infoNotaDebito/obligadoContabilidad").InnerText;

                    XmlNode rimpe = xmldoc.SelectSingleNode("/notaDebito/infoTributaria/contribuyenteRimpe");
                    if (rimpe != null)
                        comprobante.com_regimenrimpe = rimpe.InnerText;

                    comprobante.com_direccionmatriz = GetString(xmldoc.SelectSingleNode("/notaDebito/infoTributaria/dirMatriz"));
                    comprobante.com_direccionsucursal = GetString(xmldoc.SelectSingleNode("/notaDebito/infoNotaDebito/dirEstablecimiento"));
                    comprobante.com_agenteret = GetString(xmldoc.SelectSingleNode("/notaDebito/infoTributaria/agenteRetencion"));
                    comprobante.com_regimenmicro = GetString(xmldoc.SelectSingleNode("/notaDebito/infoTributaria/regimenMicroempresas"));

                    comprobante.com_ruccliente = xmldoc.SelectSingleNode("/notaDebito/infoNotaDebito/identificacionComprador").InnerText;
                    comprobante.com_nombrecliente = xmldoc.SelectSingleNode("/notaDebito/infoNotaDebito/razonSocialComprador").InnerText;
                    comprobante.com_tipoidcliente = xmldoc.SelectSingleNode("/notaDebito/infoNotaDebito/tipoIdentificacionComprador").InnerText;

                    //comprobante.com_motivo = xmldoc.SelectSingleNode("/notaDebito/infoNotaCredito/motivo").InnerText;

                    comprobante.com_rise = GetString(xmldoc.SelectSingleNode("/notaDebito/infoNotaDebito/rise"));
                    comprobante.com_coddocmodificado = xmldoc.SelectSingleNode("/notaDebito/infoNotaDebito/codDocModificado").InnerText;
                    comprobante.com_numdocmodificado = xmldoc.SelectSingleNode("/notaDebito/infoNotaDebito/numDocModificado").InnerText;
                    comprobante.com_fechaemisiondocsustento = xmldoc.SelectSingleNode("/notaDebito/infoNotaDebito/fechaEmisionDocSustento").InnerText;


                    comprobante.com_subtotalsinimp = GetDecimal(xmldoc.SelectSingleNode("/notaDebito/infoNotaDebito/totalSinImpuestos").InnerText);
                    comprobante.com_total= GetDecimal(xmldoc.SelectSingleNode("/notaDebito/infoNotaDebito/valorTotal").InnerText);





                    XmlNode totalimpuestos = xmldoc.SelectSingleNode("/notaDebito/infoNotaDebito/impuestos");

                    decimal porciva = 0;
                    decimal subtotaliva = 0;
                    decimal valoriva = 0;
                    decimal subtotalice = 0;
                    decimal valorice = 0;
                    decimal subtotal0 = 0;
                    decimal subtotalnoiva = 0;
                    decimal subtotalextiva = 0;

                    foreach (XmlNode totalimp in totalimpuestos.ChildNodes)
                    {
                        XmlNode codigo = totalimp.SelectSingleNode("codigo");
                        XmlNode codigoporcentaje = totalimp.SelectSingleNode("codigoPorcentaje");
                        XmlNode baseimp = totalimp.SelectSingleNode("baseImponible");
                        XmlNode valor = totalimp.SelectSingleNode("valor");
                        XmlNode tarifa = totalimp.SelectSingleNode("tarifa");

                        if (codigo.InnerText == "2" && (codigoporcentaje.InnerText == "2" || codigoporcentaje.InnerText == "3" || codigoporcentaje.InnerText == "4" || codigoporcentaje.InnerText == "10"))//TARIFA 12 13 14 15
                        {
                            subtotaliva += GetDecimal(baseimp.InnerText);
                            valoriva += GetDecimal(valor.InnerText);
                            if (codigoporcentaje.InnerText == "2")
                                porciva = 12;
                            if (codigoporcentaje.InnerText == "3")
                                porciva = 14;
                            if (codigoporcentaje.InnerText == "4")
                                porciva = 15;
                            if (codigoporcentaje.InnerText == "10")
                                porciva = 13;
                        }
                        if (codigo.InnerText == "2" && codigoporcentaje.InnerText == "0")//TARIFA 0
                        {
                            subtotal0 += GetDecimal(baseimp.InnerText);
                        }
                        if (codigo.InnerText == "2" && codigoporcentaje.InnerText == "6")//NO OBJETO DE IVA
                        {
                            subtotalnoiva += GetDecimal(baseimp.InnerText);
                        }
                        if (codigo.InnerText == "2" && codigoporcentaje.InnerText == "7")//EXENTO DE IVA
                        {
                            subtotalextiva += GetDecimal(baseimp.InnerText);
                        }
                        if (codigo.InnerText == "3")//TARIFA ICE
                        {
                            subtotalice += GetDecimal(baseimp.InnerText);
                            valorice += GetDecimal(valor.InnerText);
                        }
                    }
                    comprobante.com_subtotal0 = subtotal0;
                    comprobante.com_subtotaliva = subtotaliva;
                    comprobante.com_subtotalnoiva = subtotalnoiva;
                    comprobante.com_subtotalextiva = subtotalextiva;
                    comprobante.com_iva = valoriva;
                    comprobante.com_porciva = porciva;
                    comprobante.com_ice = valorice;
                    comprobante.com_iva0 = 0;


                    /*DETALLES*/
                    XmlNode detalles = xmldoc.SelectSingleNode("/notaDebito/motivos");
                    comprobante.detalle = new List<Detalle>();


                    foreach (XmlNode detalle in detalles.ChildNodes)
                    {
                        Detalle det = new Detalle();
                        det.descripcion = GetString(detalle.SelectSingleNode("razon"));
                        det.totalsinimp = GetDecimal(detalle.SelectSingleNode("valor").InnerText);
                        comprobante.detalle.Add(det);

                    }


                    /*PAGOS*/
                    XmlNode pagos = xmldoc.SelectSingleNode("/notaDebito/infoNotaDebito/pagos");


                    List<FormaPago> formas = serializer.Deserialize<List<FormaPago>>(Constantes.GetParameter("formaspago"));

                    comprobante.formas = new List<Formapago>();

                    if (pagos != null)
                    {
                        foreach (XmlNode pago in pagos.ChildNodes)
                        {
                            Formapago fp = new Formapago();
                            fp.secuencia = comprobante.formas.Count() + 1;
                            fp.codigo = pago.SelectSingleNode("formaPago").InnerText;
                            FormaPago forma = formas.Find(delegate (FormaPago f) { return f.codigo == fp.codigo; });
                            fp.forma = forma.forma;
                            fp.valor = GetDecimal(pago.SelectSingleNode("total"));
                            fp.plazo = GetEntero(pago.SelectSingleNode("plazo"));
                            fp.tiempo = GetString(pago.SelectSingleNode("unidadTiempo"));
                            comprobante.formas.Add(fp);

                        }
                    }





                    XmlNode infoadicional = xmldoc.SelectSingleNode("/notaDebito/infoAdicional");

                    string observacion = "";
                    string direccion = "";
                    string telefono = "";
                    string vendedor = "";
                    string ciudad = "";
                    string fpago = "";
                    string transporte = "";
                    string compensacion = "";


                    if (infoadicional != null)
                    {
                        foreach (XmlNode iteminfo in infoadicional.ChildNodes)
                        {

                            if (iteminfo.Attributes["nombre"].Value == "Direccion")
                                comprobante.com_direccioncliente = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Telefono")
                                comprobante.com_telefonocliente = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Email")
                                comprobante.com_email = iteminfo.InnerText;

                            if (iteminfo.Attributes["nombre"].Value == "Vendedor")
                                comprobante.com_adicional1 = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Ciudad")
                                comprobante.com_adicional2 = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Fpago")
                                comprobante.com_adicional3 = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Transporte")
                                comprobante.com_adicional4 = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Observacion")
                                comprobante.com_adicional5 = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Compensado")
                                comprobante.com_adicional6 = iteminfo.InnerText;

                        }

                    }
                }

                if (tipo.ToUpper() == "GUIAREMISION")
                {

                    comprobante.com_numero = xmldoc.SelectSingleNode("/guiaRemision/infoTributaria/claveAcceso").InnerText;
                    comprobante.com_almacen = xmldoc.SelectSingleNode("/guiaRemision/infoTributaria/estab").InnerText;
                    comprobante.com_pventa = xmldoc.SelectSingleNode("/guiaRemision/infoTributaria/ptoEmi").InnerText;
                    comprobante.com_secuencia = xmldoc.SelectSingleNode("/guiaRemision/infoTributaria/secuencial").InnerText;
                    comprobante.com_ambiente = int.Parse(xmldoc.SelectSingleNode("/guiaRemision/infoTributaria/ambiente").InnerText);
                    comprobante.com_emision = int.Parse(xmldoc.SelectSingleNode("/guiaRemision/infoTributaria/tipoEmision").InnerText);
                    comprobante.com_direccionmatriz = GetString(xmldoc.SelectSingleNode("/guiaRemision/infoTributaria/dirMatriz"));
       				comprobante.com_agenteret = GetString(xmldoc.SelectSingleNode("/guiaRemision/infoTributaria/agenteRetencion"));
                    comprobante.com_regimenmicro = GetString(xmldoc.SelectSingleNode("/guiaRemision/infoTributaria/regimenMicroempresas"));

                    XmlNode rimpe = xmldoc.SelectSingleNode("/guiaRemision/infoTributaria/contribuyenteRimpe");
                    if (rimpe != null)
                        comprobante.com_regimenrimpe = rimpe.InnerText;


                    comprobante.com_direccionsucursal = GetString(xmldoc.SelectSingleNode("/guiaRemision/infoGuiaRemision/dirEstablecimiento"));
                    comprobante.com_dirpartida = GetString(xmldoc.SelectSingleNode("/guiaRemision/infoGuiaRemision/dirPartida"));

                    comprobante.com_nombrecliente = GetString(xmldoc.SelectSingleNode("/guiaRemision/infoGuiaRemision/razonSocialTransportista"));
                    comprobante.com_tipoidcliente = GetString(xmldoc.SelectSingleNode("/guiaRemision/infoGuiaRemision/tipoIdentificacionTransportista"));
                    comprobante.com_ruccliente = GetString(xmldoc.SelectSingleNode("/guiaRemision/infoGuiaRemision/rucTransportista"));
                    comprobante.com_rise = GetString(xmldoc.SelectSingleNode("/guiaRemision/infoGuiaRemision/rise"));
                    comprobante.com_contribuyente = GetString(xmldoc.SelectSingleNode("/guiaRemision/infoGuiaRemision/contribuyenteEspecial"));
                    comprobante.com_contabilidad = xmldoc.SelectSingleNode("/guiaRemision/infoGuiaRemision/obligadoContabilidad").InnerText;
                    comprobante.com_fechainitransporte = GetString(xmldoc.SelectSingleNode("/guiaRemision/infoGuiaRemision/fechaIniTransporte"));
                    comprobante.com_fechafintransporte = GetString(xmldoc.SelectSingleNode("/guiaRemision/infoGuiaRemision/fechaFinTransporte"));
                    comprobante.com_placa= GetString(xmldoc.SelectSingleNode("/guiaRemision/infoGuiaRemision/placa"));




                    /*DETALLES*/
                    XmlNode detalles = xmldoc.SelectSingleNode("/guiaRemision/destinatarios");
                    comprobante.detalle = new List<Detalle>();


                    foreach (XmlNode detalle in detalles.ChildNodes)
                    {
                        Detalle det = new Detalle();
                        det.iddestinatario = GetString(detalle.SelectSingleNode("identificacionDestinatario"));
                        det.razondestinatario = GetString(detalle.SelectSingleNode("razonSocialDestinatario"));
                        det.dirdestinatario = GetString(detalle.SelectSingleNode("dirDestinatario"));
                        det.motivotraslado = GetString(detalle.SelectSingleNode("motivoTraslado"));
                        det.docaduana = GetString(detalle.SelectSingleNode("docAduaneroUnico"));
                        det.codestabdestino = GetString(detalle.SelectSingleNode("codEstabDestino"));
                        det.ruta = GetString(detalle.SelectSingleNode("ruta"));

                        det.codigodocsustento = GetString(detalle.SelectSingleNode("codDocSustento"));
                        det.numdocsustento = GetString(detalle.SelectSingleNode("numDocSustento"));
                        det.numautsustento = GetString(detalle.SelectSingleNode("numAutDocSustento"));
                        det.fechaemisiondocsustento = GetString(detalle.SelectSingleNode("fechaEmisionDocSustento"));


                        XmlNode subdetalles = detalle.SelectSingleNode("detalles");
                        det.subdetalles = new List<Subdetalle>();

                        foreach (XmlNode subdetalle in subdetalles.ChildNodes)
                        {
                            Subdetalle sdet = new Subdetalle();

                            sdet.codigointerno = GetString(subdetalle.SelectSingleNode("codigoInterno"));
                            sdet.codigoadicional = GetString(subdetalle.SelectSingleNode("codigoAdicional"));
                            sdet.descripcion = GetString(subdetalle.SelectSingleNode("descripcion"));
                            sdet.cantidad = GetDecimal(subdetalle.SelectSingleNode("cantidad"));

                            XmlNode adicionalesdet = subdetalle.SelectSingleNode("detallesAdicionales");
                            if (adicionalesdet != null)
                            {
                                int i = 0;
                                foreach (XmlNode detadi in adicionalesdet.ChildNodes)
                                {
                                    if (i == 0)
                                        sdet.adicional1 = detadi.Attributes["valor"].Value;
                                    if (i == 1)
                                        sdet.adicional1 = detadi.Attributes["valor"].Value;
                                    if (i == 2)
                                        sdet.adicional1 = detadi.Attributes["valor"].Value;
                                    i++;
                                }
                            }
                            det.subdetalles.Add(sdet);
                        }




                        comprobante.detalle.Add(det);

                    }


                   
                    XmlNode infoadicional = xmldoc.SelectSingleNode("/guiaRemision/infoAdicional");


                    if (infoadicional != null)
                    {
                        foreach (XmlNode iteminfo in infoadicional.ChildNodes)
                        {

                            if (iteminfo.Attributes["nombre"].Value == "Direccion")
                                comprobante.com_direccioncliente = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Telefono")
                                comprobante.com_telefonocliente = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Email")
                                comprobante.com_email = iteminfo.InnerText;

                            if (iteminfo.Attributes["nombre"].Value == "Vendedor")
                                comprobante.com_adicional1 = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Ciudad")
                                comprobante.com_adicional2 = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Fpago")
                                comprobante.com_adicional3 = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Transporte")
                                comprobante.com_adicional4 = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Observacion")
                                comprobante.com_adicional5 = iteminfo.InnerText;
                            if (iteminfo.Attributes["nombre"].Value == "Compensado")
                                comprobante.com_adicional6 = iteminfo.InnerText;

                        }

                    }
                }




                if (tipo.ToUpper() == "LIQUIDACIONCOMPRA")
                {

                    comprobante.com_numero = xmldoc.SelectSingleNode("/liquidacionCompra/infoTributaria/claveAcceso").InnerText;
                    comprobante.com_almacen = xmldoc.SelectSingleNode("/liquidacionCompra/infoTributaria/estab").InnerText;
                    comprobante.com_pventa = xmldoc.SelectSingleNode("/liquidacionCompra/infoTributaria/ptoEmi").InnerText;
                    comprobante.com_secuencia = xmldoc.SelectSingleNode("/liquidacionCompra/infoTributaria/secuencial").InnerText;
                    comprobante.com_fecha = GetDateTime(xmldoc.SelectSingleNode("/liquidacionCompra/infoLiquidacionCompra/fechaEmision"));
                    comprobante.com_fechastr = GetString(xmldoc.SelectSingleNode("/liquidacionCompra/infoLiquidacionCompra/fechaEmision"));
                    //comprobante.com_autorizacion = 
                    comprobante.com_ambiente = int.Parse(xmldoc.SelectSingleNode("/liquidacionCompra/infoTributaria/ambiente").InnerText);
                    comprobante.com_emision = int.Parse(xmldoc.SelectSingleNode("/liquidacionCompra/infoTributaria/tipoEmision").InnerText);
                    comprobante.com_contribuyente = GetString(xmldoc.SelectSingleNode("/liquidacionCompra/infoLiquidacionCompra/contribuyenteEspecial"));
                    comprobante.com_contabilidad = xmldoc.SelectSingleNode("/liquidacionCompra/infoLiquidacionCompra/obligadoContabilidad").InnerText;

                    comprobante.com_direccionmatriz = GetString(xmldoc.SelectSingleNode("/liquidacionCompra/infoLiquidacionCompra/dirMatriz"));
                    comprobante.com_direccionsucursal = GetString(xmldoc.SelectSingleNode("/liquidacionCompra/infoLiquidacionCompra/dirEstablecimiento"));
                    comprobante.com_agenteret = GetString(xmldoc.SelectSingleNode("/liquidacionCompra/infoTributaria/agenteRetencion"));
                    comprobante.com_regimenmicro = GetString(xmldoc.SelectSingleNode("/liquidacionCompra/infoTributaria/regimenMicroempresas"));

                    XmlNode rimpe = xmldoc.SelectSingleNode("/liquidacionCompra/infoTributaria/contribuyenteRimpe");
                    if (rimpe != null)
                        comprobante.com_regimenrimpe = rimpe.InnerText;
                    comprobante.com_ruccliente = xmldoc.SelectSingleNode("/liquidacionCompra/infoLiquidacionCompra/identificacionProveedor").InnerText;
                    comprobante.com_nombrecliente = xmldoc.SelectSingleNode("/liquidacionCompra/infoLiquidacionCompra/razonSocialProveedor").InnerText;
                    comprobante.com_tipoidcliente = xmldoc.SelectSingleNode("/liquidacionCompra/infoLiquidacionCompra/tipoIdentificacionProveedor").InnerText;                    
                    //comprobante.com_guiaremision falta implementar
                     comprobante.com_direccioncliente = GetString(xmldoc.SelectSingleNode("/liquidacionCompra/infoLiquidacionCompra/direccionProveedor"));


                                        

                    comprobante.com_subtotalsinimp = GetDecimal(xmldoc.SelectSingleNode("/liquidacionCompra/infoLiquidacionCompra/totalSinImpuestos").InnerText);
                    comprobante.com_descuento = GetDecimal(xmldoc.SelectSingleNode("/liquidacionCompra/infoLiquidacionCompra/totalDescuento").InnerText);
                    comprobante.com_total = GetDecimal(xmldoc.SelectSingleNode("/liquidacionCompra/infoLiquidacionCompra/importeTotal").InnerText);

                    XmlNode totalimpuestos = xmldoc.SelectSingleNode("/liquidacionCompra/infoLiquidacionCompra/totalConImpuestos");

                    decimal porciva = 0;

                    if (comprobante.com_fecha.HasValue)
                        porciva = Constantes.GetValorIVA(comprobante.com_fecha);
                    decimal subtotaliva = 0;
                    decimal valoriva = 0;
                    decimal subtotalice = 0;
                    decimal valorice = 0;
                    decimal subtotal0 = 0;
                    decimal subtotalnoiva = 0;
                    decimal subtotalextiva = 0;

                    foreach (XmlNode totalimp in totalimpuestos.ChildNodes)
                    {
                        XmlNode codigo = totalimp.SelectSingleNode("codigo");
                        XmlNode codigoporcentaje = totalimp.SelectSingleNode("codigoPorcentaje");
                        XmlNode descuento = totalimp.SelectSingleNode("descuentoAdicional");
                        XmlNode baseimp = totalimp.SelectSingleNode("baseImponible");
                        XmlNode tarifa = totalimp.SelectSingleNode("tarifa");
                        XmlNode valor = totalimp.SelectSingleNode("valor");


                        if (codigo.InnerText == "2" && (codigoporcentaje.InnerText == "2" || codigoporcentaje.InnerText == "3" || codigoporcentaje.InnerText == "4" || codigoporcentaje.InnerText == "10"))//TARIFA 12 13 14 15
                        {
                            subtotaliva += GetDecimal(baseimp.InnerText);
                            valoriva += GetDecimal(valor.InnerText);
                            if (codigoporcentaje.InnerText == "2")
                                porciva = 12;
                            if (codigoporcentaje.InnerText == "3")
                                porciva = 14;
                            if (codigoporcentaje.InnerText == "4")
                                porciva = 15;
                            if (codigoporcentaje.InnerText == "10")
                                porciva = 13;
                        }
                        if (codigo.InnerText == "2" && codigoporcentaje.InnerText == "0")//TARIFA 0
                        {
                            subtotal0 += GetDecimal(baseimp.InnerText);
                        }
                        if (codigo.InnerText == "2" && codigoporcentaje.InnerText == "6")//NO OBJETO DE IVA
                        {
                            subtotalnoiva += GetDecimal(baseimp.InnerText);
                        }
                        if (codigo.InnerText == "2" && codigoporcentaje.InnerText == "7")//EXENTO DE IVA
                        {
                            subtotalextiva += GetDecimal(baseimp.InnerText);
                        }
                        if (codigo.InnerText == "3")//TARIFA ICE
                        {
                            subtotalice += GetDecimal(baseimp.InnerText);
                            valorice += GetDecimal(valor.InnerText);
                        }
                    }
                    comprobante.com_subtotal0 = subtotal0;
                    comprobante.com_subtotaliva = subtotaliva;
                    comprobante.com_subtotalnoiva = subtotalnoiva;
                    comprobante.com_subtotalextiva = subtotalextiva;
                    comprobante.com_iva = valoriva;
                    comprobante.com_porciva = porciva;
                    comprobante.com_ice = valorice;
                    comprobante.com_iva0 = 0;

                    //Validacion SUBTOTALES por ERRORES DE ENVIO 
                    //ESTO SOLO PARA COMPROBANES DE FORMATO 2 y EMPRESA=1 INMOT

                    if (comprobante.com_formato == 2 && comprobante.com_empresa == 1)
                    {
                        decimal subtotal0_1 = comprobante.com_total.Value - (subtotaliva + valoriva);
                        if (subtotal0_1 > subtotal0)
                        {
                            comprobante.com_subtotal0 = subtotal0_1;
                        }
                        decimal subtotaliva_1 = comprobante.com_subtotalsinimp.Value + comprobante.com_descuento.Value - comprobante.com_subtotal0.Value;
                        //if (subtotaliva_1 >subtotaliva)
                        //{
                        comprobante.com_subtotaliva = subtotaliva_1;
                        //}

                        decimal totalsinimp_1 = comprobante.com_subtotal0.Value + comprobante.com_subtotaliva.Value;
                        if (totalsinimp_1 > comprobante.com_subtotalsinimp.Value)
                            comprobante.com_subtotalsinimp = totalsinimp_1;
                    }

                    XmlNode infoadicional = xmldoc.SelectSingleNode("/liquidacionCompra/infoAdicional");

                    string observacion = "";
                    string direccion = "";
                    string telefono = "";
                    string vendedor = "";
                    string ciudad = "";
                    string fpago = "";
                    string transporte = "";
                    string compensacion = "";

                    List<String> adicionales = new List<string>();
                    if (infoadicional != null)
                    {
                        foreach (XmlNode iteminfo in infoadicional.ChildNodes)
                        {

                            if (iteminfo.Attributes["nombre"].Value == "Direccion")
                                comprobante.com_direccioncliente = iteminfo.InnerText;
                            else if (iteminfo.Attributes["nombre"].Value == "Telefono")
                                comprobante.com_telefonocliente = iteminfo.InnerText;
                            else if (iteminfo.Attributes["nombre"].Value == "Email")
                            {
                                if (string.IsNullOrEmpty(comprobante.com_email))
                                    comprobante.com_email = iteminfo.InnerText;
                            }
                            else if (iteminfo.Attributes["nombre"].Value == "Vendedor")
                                comprobante.com_adicional1 = iteminfo.InnerText;
                            else if (iteminfo.Attributes["nombre"].Value == "Ciudad")
                                comprobante.com_adicional2 = iteminfo.InnerText;
                            else if (iteminfo.Attributes["nombre"].Value == "Fpago")
                                comprobante.com_adicional3 = iteminfo.InnerText;
                            else if (iteminfo.Attributes["nombre"].Value == "Transporte")
                                comprobante.com_adicional4 = iteminfo.InnerText;
                            else if (iteminfo.Attributes["nombre"].Value == "Observacion")
                                comprobante.com_adicional5 = iteminfo.InnerText;
                            else if (iteminfo.Attributes["nombre"].Value == "Compensado")
                                comprobante.com_adicional6 = iteminfo.InnerText;
                            else
                                adicionales.Add(iteminfo.Attributes["nombre"].Value + "|" + iteminfo.InnerText);




                        }

                    }
                    comprobante.com_adicionales = adicionales;


                    /*PAGOS*/
                    XmlNode pagos = xmldoc.SelectSingleNode("/liquidacionCompra/infoLiquidacionCompra/pagos");


                    List<FormaPago> formas = serializer.Deserialize<List<FormaPago>>(Constantes.GetParameter("formaspago"));

                    comprobante.formas = new List<Formapago>();

                    if (pagos != null)
                    {
                        foreach (XmlNode pago in pagos.ChildNodes)
                        {
                            Formapago fp = new Formapago();
                            fp.secuencia = comprobante.formas.Count() + 1;
                            fp.codigo = pago.SelectSingleNode("formaPago").InnerText;
                            FormaPago forma = formas.Find(delegate (FormaPago f) { return f.codigo == fp.codigo; });
                            fp.forma = forma.forma;
                            fp.valor = GetDecimal(pago.SelectSingleNode("total"));
                            fp.plazo = GetEntero(pago.SelectSingleNode("plazo"));
                            fp.tiempo = GetString(pago.SelectSingleNode("unidadTiempo"));
                            comprobante.formas.Add(fp);

                        }
                    }

                    /*DETALLES*/
                    XmlNode detalles = xmldoc.SelectSingleNode("/liquidacionCompra/detalles");
                    comprobante.detalle = new List<Detalle>();

                    //List<CodigoImpuesto> lstiva = new JavaScriptSerializer().Deserialize<List<CodigoImpuesto>>(Constantes.GetParameterValue("codigosiva"));
                    //List<CodigoImpuesto> lstice = new JavaScriptSerializer().Deserialize<List<CodigoImpuesto>>(Constantes.GetParameterValue("codigosice"));

                    foreach (XmlNode detalle in detalles.ChildNodes)
                    {
                        Detalle det = new Detalle();
                        det.secuencia = comprobante.detalle.Count() + 1;
                        det.codigo = GetString(detalle.SelectSingleNode("codigoPrincipal"));
                        det.codigoaux = GetString(detalle.SelectSingleNode("codigoAuxiliar"));
                        det.descripcion = GetString(detalle.SelectSingleNode("descripcion"));
                        det.cantidad = GetDecimal(detalle.SelectSingleNode("cantidad").InnerText);
                        det.precio = GetDecimal(detalle.SelectSingleNode("precioUnitario").InnerText);
                        det.descuento = GetDecimal(detalle.SelectSingleNode("descuento").InnerText);
                        det.totalsinimp = GetDecimal(detalle.SelectSingleNode("precioTotalSinImpuesto").InnerText);

                        XmlNode impuestosdet = detalle.SelectSingleNode("impuestos");
                        foreach (XmlNode impuesto in impuestosdet.ChildNodes)
                        {
                            if (impuesto.SelectSingleNode("codigo").InnerText == "2")//IVA
                            {
                                det.codiva = GetString(impuesto.SelectSingleNode("codigoPorcentaje"));
                                det.porciva = GetDecimal(impuesto.SelectSingleNode("tarifa").InnerText);
                                det.valiva = GetDecimal(impuesto.SelectSingleNode("valor").InnerText);
                            }
                            if (impuesto.SelectSingleNode("codigo").InnerText == "3")//ICE
                            {
                                det.codice = GetString(impuesto.SelectSingleNode("codigoPorcentaje"));
                                det.porcice = GetDecimal(impuesto.SelectSingleNode("tarifa").InnerText);
                                det.valice = GetDecimal(impuesto.SelectSingleNode("valor").InnerText);
                            }

                        }

                        XmlNode adicionalesdet = detalle.SelectSingleNode("detallesAdicionales");
                        if (adicionalesdet != null)
                        {
                            foreach (XmlNode detadi in adicionalesdet.ChildNodes)
                            {
                                //if (detadi.Attributes["nombre"].Value == "detadicional1")
                                //    det.adicional1 = detadi.Attributes["valor"].Value;
                                //if (detadi.Attributes["nombre"].Value == "detadicional2")
                                //    det.adicional2 = detadi.Attributes["valor"].Value;
                                //if (detadi.Attributes["nombre"].Value == "detadicional3")
                                //    det.adicional3 = detadi.Attributes["valor"].Value;

                                if (detadi.Attributes["nombre"].Value == "LINEA1")
                                    det.adicional1 = detadi.Attributes["valor"].Value;
                                if (detadi.Attributes["nombre"].Value == "LINEA2")
                                    det.adicional2 = detadi.Attributes["valor"].Value;
                                if (detadi.Attributes["nombre"].Value == "LINEA3")
                                    det.adicional3 = detadi.Attributes["valor"].Value;

                            }
                        }


                        comprobante.detalle.Add(det);

                    }


                }



            }
            return comprobante;

        }

    }
}
