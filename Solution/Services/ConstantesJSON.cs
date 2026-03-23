using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services
{

    public class FormaPago
    {
        public string codigo { get; set; }
        public string forma { get; set; }
        
    }

    public class TipoComprobantes
    {
        public string codigo { get; set; }
        public string comprobante { get; set; }
    }

    public class SmtpServer
    {
        public string from { get; set; }
        public string dominio { get; set; }
        public string usuario { get; set; }
        public string password { get; set; }
        public bool? requiereaut { get; set; }
        public string server { get; set; }
        public bool? ssl { get; set; }
        public int? puerto { get; set; }
        public bool? async { get; set; }
        public bool activo { get; set; }


    }
    public class ValorIVA
    {
        public string desde { get; set; }
        public string hasta { get; set; }
        public decimal valor { get; set; }
    }


    public class ErrorAccion
    {
        public string error { get; set; }
        public string accion { get; set; }
    }

    public class ServicioHorario
    {
        public int dia { get; set; }
        public int hora { get; set; }
        public string mails { get; set; }
    }

    public class ConstantesJSON
    {
    }
}
