using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Web;
using System.IO;
using BusinessObjects;
using BusinessLogicLayer;


namespace ExceptionHandling
{
    public class Log
    {
        /*public static void Add(string error)
        {
            EventLog.WriteEntry("BIOSys", error, EventLogEntryType.Error);
        }*/

        public static void AddLog(Exception ex)
        {
            string filename = string.Format("{0:00}{1:00}{2:0000}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
            string path = "c:\\SICE\\log";

            StreamWriter sw;
             sw = File.AppendText(path+"/"+filename);
             sw.WriteLine(DateTime.Now.ToString() + "," + ex.Source + "," + ex.Message + "," + ex.StackTrace);
             sw.Close();         
        }

        public static void AddLog(string texto)
        {
            string filename = string.Format("{0:00}{1:00}{2:0000}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
            string path = "c:\\SICE\\log";

            StreamWriter sw;
            sw = File.AppendText(path + "/" + filename);
            sw.WriteLine(DateTime.Now.ToString() + "," + texto);
            sw.Close();
        }


        public static void AddExepcion(Exception ex)
        {
            //Excepcion e = new Excepcion();
            //e.exc_fecha = DateTime.Now;
            //e.exc_descripcion = ex.Message;
            //e.exc_origen = ex.Source;
            //e.exc_stack = ex.StackTrace;

            //try
            //{
            //    ExcepcionBLL.Insert(e);  
            //}
            //catch (Exception error)
            //{
               // AddLog(error);
                AddLog(ex);
            //}

        }
    }
}
