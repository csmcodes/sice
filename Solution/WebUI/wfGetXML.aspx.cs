using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Functions;
using System.Xml;
using BusinessLogicLayer;
using BusinessObjects;

namespace WebUI
{
    public partial class wfGetXML : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetXML();
            }
        }



   


        public void GetXML()
        {
            string clave = Request.QueryString["clave"];


            List<Comprobante> comprobantes = ComprobanteBLL.GetAll(new WhereParams("com_numero={0}", clave), "");
            foreach (Comprobante comprobante in comprobantes)
            {
                Archivo arc = ArchivoBLL.GetByPK(new Archivo { arc_empresa = comprobante.com_empresa, arc_empresa_key = comprobante.com_empresa, arc_numero = comprobante.com_numero, arc_numero_key = comprobante.com_numero });
                XmlDocument xml = new XmlDocument();                
                if (comprobante.com_estado == (int)Enums.EstadoComprobante.Autorizado)                
                    xml.LoadXml(Packages.General.FormatXML(arc.arc_xmlrespuesta));
                else                    
                    xml.LoadXml(arc.arc_xml);


                // Create an XML declaration.
                XmlDeclaration xmldecl;
                xmldecl = xml.CreateXmlDeclaration("1.0", null, null);
                xmldecl.Encoding = "UTF-8";
                xmldecl.Standalone = "yes";

                // Add the new node to the document.
                XmlElement root = xml.DocumentElement;
                xml.InsertBefore(xmldecl, root);



                string filename = comprobante.com_numero + ".xml";

                Response.Clear();
                Response.ContentType = "text/xml";
                Response.AppendHeader("Content-Disposition", "attachment;filename=" + filename);
                XmlTextWriter xWriter = new XmlTextWriter(Response.OutputStream, System.Text.Encoding.UTF8);
                xml.Save(xWriter);
                xWriter.Close();
                Response.End();


            }
        }
    }
}