using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace WebUI
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            SICE.Metodos ws = new SICE.Metodos();
            ws.RecibirComprobante(TextBox1.Text, txtmail.Text, 1);
            
            //string path =Server.MapPath("xml/fac4.xml");

            //Firma.sign(path);
            

            //lblmensaje.Text =  ws.RecibirComprobante(TextBox1.Text);
        }


    }
}