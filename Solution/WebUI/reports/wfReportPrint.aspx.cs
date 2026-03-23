using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Reflection;
using System.Data;
using BusinessObjects;
using BusinessLogicLayer;
using Services;


namespace WebUI.reports
{
    public partial class wfReportPrint : System.Web.UI.Page
    {
        protected string empresa;
        protected string report;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                empresa = Request.QueryString["empresa"];
                report = Request.QueryString["report"];

                bool autoprint = false;
                bool.TryParse(Request.QueryString["autoprint"], out autoprint);
                txtparameter1.Text = (Request.QueryString["parameter1"] != null) ? Request.QueryString["parameter1"].ToString() : "";
                txtparameter2.Text = (Request.QueryString["parameter2"] != null) ? Request.QueryString["parameter2"].ToString() : "";
                txtparameter3.Text = (Request.QueryString["parameter3"] != null) ? Request.QueryString["parameter3"].ToString() : "";
                txtparameter4.Text = (Request.QueryString["parameter4"] != null) ? Request.QueryString["parameter4"].ToString() : "";
                txtparameter5.Text = (Request.QueryString["parameter5"] != null) ? Request.QueryString["parameter5"].ToString() : "";
                txtparameter6.Text = (Request.QueryString["parameter6"] != null) ? Request.QueryString["parameter6"].ToString() : "";
                txtparameter7.Text = (Request.QueryString["parameter7"] != null) ? Request.QueryString["parameter7"].ToString() : "";
                txtparameter8.Text = (Request.QueryString["parameter8"] != null) ? Request.QueryString["parameter8"].ToString() : "";
                txtparameter9.Text = (Request.QueryString["parameter9"] != null) ? Request.QueryString["parameter9"].ToString() : "";
                txtparameter10.Text = (Request.QueryString["parameter10"] != null) ? Request.QueryString["parameter10"].ToString() : "";

                GenReport(report,empresa, autoprint);
            }
        }

        private void GenReport(String codigo, string empresa, bool autoprint)
        {
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.Reset();
            ReportViewer1.Visible = true;

            Packages.ReportBuilder.BuildReport(ref ReportViewer1, codigo, empresa, txtparameter1.Text, txtparameter2.Text, txtparameter3.Text, txtparameter4.Text, txtparameter5.Text, txtparameter6.Text, txtparameter7.Text, txtparameter8.Text, txtparameter9.Text, txtparameter10.Text);
            //rp = new ReportPrintDocument(ReportViewer1.LocalReport);


            //if (autoprint)
            //rp.Print();

        }


        protected void print(object sender, EventArgs e)
        {
            //rp.Print();
        }
    }
}