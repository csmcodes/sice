using System;

namespace WebAPI.Models
{
    public class ComprobanteEstadoResponse
    {
        public string claveAcceso { get; set; }
        public string numeroComprobante { get; set; }
        public string estado { get; set; }
        public string fechaAutorizacion { get; set; }
        public string numeroAutorizacion { get; set; }
        public string mensaje { get; set; }
    }
}
