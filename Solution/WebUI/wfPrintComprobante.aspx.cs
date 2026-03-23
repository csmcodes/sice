using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusinessObjects;
using BusinessLogicLayer;

namespace WebUI
{
    public partial class wfPrintComprobante : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string empresa = Request.QueryString["empresa"];
                string clave = Request.QueryString["clave"];
                string  generar= Request.QueryString["generar"];

                bool gen = false;
                bool.TryParse(generar, out gen);

                GenReport(empresa, clave,gen);
            }
        }


        private void GenReport(string empresa, string clave, bool generar)
        {
            Comprobante comp = ComprobanteBLL.GetByPK(new Comprobante() { com_empresa = int.Parse(empresa), com_empresa_key = int.Parse(empresa), com_numero= clave , com_numero_key= clave });
            comp = Packages.XmlReader.CargarFactura(comp);

            string filename = Services.Pdf.CreatePDF(comp);

            /*Comprobante comprobante = ComprobanteBLL.GetByPK(new Comprobante { com_numero = clave, com_numero_key = clave, com_empresa = int.Parse(empresa), com_empresa_key = int.Parse(empresa) });
            if (string.IsNullOrEmpty(comprobante.com_pdf))
                generar = true;
            if (generar)
                //comprobante.com_pdf = Services.Pdf.SavePDF(comprobante.com_empresa, comprobante.com_numero, HttpContext.Current.Server.MapPath("htmlride"), false, "");
                comprobante.com_pdf= Services.Pdf.SavePDF(comprobante.com_empresa, comprobante.com_numero, HttpContext.Current.Server.MapPath("pdf"), false, "");
            */
            ShowPdf(filename);

        }






        public void ShowPdf(string filename)
        {
            //Services.PdfPrinter.PrintPDFs(filename);

            //Clears all content output from Buffer Stream
            Response.ClearContent();
            //Clears all headers from Buffer Stream
            Response.ClearHeaders();
            //Adds an HTTP header to the output stream
            Response.AddHeader("Content-Disposition", "inline;filename=" + filename);
            //Gets or Sets the HTTP MIME type of the output stream
            Response.ContentType = "application/pdf";
            //Writes the content of the specified file directory to an HTTP response output stream as a file block
            Response.WriteFile(filename);
            //sends all currently buffered output to the client
            Response.Flush();
            //Clears all content output from Buffer Stream
            Response.Clear();
        }
    }
}