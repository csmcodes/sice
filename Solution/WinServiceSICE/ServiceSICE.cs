using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.IO;

namespace WinServiceSICE
{
    public partial class ServiceSICE : ServiceBase
    {
        Timer timer = null;

        public ServiceSICE()
        {
            InitializeComponent();
          
        }


        private void RunService()
        {
            try
            {

                string path = @"C:\SICE\log\logsrv.txt";
                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine(DateTime.Now.ToString() + " -  Inicia ejecución servicio");                
                if (Packages.General.RunVerificacion())
                    tw.WriteLine(DateTime.Now.ToString() + " -  EJECUTADO");
                tw.WriteLine(DateTime.Now.ToString() + " -  Fin ejecución servicio");
                tw.Close();

            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("SICE", "Ocurrió el siguiente error: " + ex.Message);
            }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RunService();
        }

        protected override void OnStart(string[] args)
        {
            timer = new Timer(Packages.General.GetTimerService());
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = true;
            timer.Start();
            RunService();
            
        }

        protected override void OnStop()
        {
            timer.Stop();
        }
    }
}
