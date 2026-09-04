using System;
using System.Globalization;
using System.Threading;

namespace AutomataSRI
{
    class Program
    {
        static int Main(string[] args)
        {
            // CRITICO: la libreria de firma digital MITyC/IKVM requiere locale es-ES
            // es-EC no tiene los bundles i18n necesarios y falla en runtime
            Thread.CurrentThread.CurrentCulture   = new CultureInfo("es-ES");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");

            DateTime inicio = DateTime.Now;
            Console.WriteLine("========================================");
            Console.WriteLine(" AutomataSRI  " + inicio.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.WriteLine("========================================");

            try
            {
                AutomataSRILog.Linea("=== AutomataSRI iniciado ===");

                var motor = new MotorAutomataSRI();
                motor.Ejecutar();

                TimeSpan duracion = DateTime.Now - inicio;
                Console.WriteLine("========================================");
                Console.WriteLine(" Finalizado en " + duracion.TotalSeconds.ToString("0.0") + "s");
                Console.WriteLine("========================================");
                AutomataSRILog.Linea("=== AutomataSRI finalizado en " + duracion.TotalSeconds.ToString("0.0") + "s ===");

                Esperar(0);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR FATAL: " + ex.Message);
                AutomataSRILog.Error("ERROR FATAL", ex);
                Esperar(1);
                return 1;
            }
        }

        static void Esperar(int exitCode)
        {
            for (int i = 5; i > 0; i--)
            {
                Console.Write("\rCerrando en {0}s...  ", i);
                Thread.Sleep(1000);
            }
            Console.WriteLine();
        }
    }
}
