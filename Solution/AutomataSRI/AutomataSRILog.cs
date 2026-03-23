using System;
using System.IO;

namespace AutomataSRI
{
    public static class AutomataSRILog
    {
        private static readonly string _logPath = AppDomain.CurrentDomain.BaseDirectory + "automata_sri.log";

        public static void Linea(string texto)
        {
            try
            {
                string linea = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + texto;
                File.AppendAllText(_logPath, linea + Environment.NewLine);
            }
            catch { }
        }

        public static void Error(string texto, Exception ex)
        {
            Linea("ERROR " + texto + " | " + ex.Message);
            if (ex.InnerException != null)
                Linea("  INNER: " + ex.InnerException.Message);
        }
    }
}
