using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Reporting.WebForms;
using System.Data;
using BusinessObjects;
using BusinessLogicLayer;


namespace Packages
{
    public class ReportBuilder
    {
        protected static string reportfolder = "reports/";

        public static void BuildReport(ref ReportViewer reportviewer, string reportcode, string empresa, params object[] parameters)
        {
            Empresa emp = EmpresaBLL.GetByPK(new Empresa { emp_codigo = int.Parse(empresa), emp_codigo_key = int.Parse(empresa) });
            LocalReport rep = reportviewer.LocalReport;
            switch (reportcode)
            {
               
                case "FALTANTES"://Reporte de Comprobantes Faltantes

                    string almacen = parameters[2].ToString();
                    string pventa = parameters[3].ToString();
                    string secuencia = parameters[4].ToString();
                    string cliente = parameters[5].ToString();
                    string ambiente = parameters[6].ToString();

                    int? amb = null;
                    int amb1;
                    if (int.TryParse(ambiente, out amb1))
                        amb = amb1;

                    //string strpve1 = parameters[5].ToString();
                   // string usr1 = parameters[6].ToString();

                   
                   
                   rep.ReportPath = reportfolder + "Faltantes.rdlc";
                   rep.DataSources.Add(new ReportDataSource("DataSet1", Packages.General.GetFaltantes(emp.emp_codigo, almacen, pventa, secuencia, cliente, amb, DateTime.Parse(parameters[0].ToString()), DateTime.Parse(parameters[1].ToString()))));
                   rep.SetParameters(new ReportParameter("desde", DateTime.Parse(parameters[0].ToString()).ToShortDateString()));
                   rep.SetParameters(new ReportParameter("hasta", DateTime.Parse(parameters[1].ToString()).ToShortDateString()));
                   rep.SetParameters(new ReportParameter("empresa", emp.emp_nombre));
                    //rep.SetParameters(new ReportParameter("pventa", strpve));
                    //rep.SetParameters(new ReportParameter("usuario", usr1));

                    break;                

            }
        }



    }
}

