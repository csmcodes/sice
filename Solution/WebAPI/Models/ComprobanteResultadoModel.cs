using System;

namespace WebAPI.Models
{
    public class ComprobanteResultadoRequest
    {
        public string resultado { get; set; }              // AUTORIZADO | NOAUTORIZADO | DEVUELTO
        public string numeroAutorizacion { get; set; }      // requerido si AUTORIZADO
        public string fechaAutorizacion { get; set; }        // requerido si AUTORIZADO
        public string xmlAutorizadoBase64 { get; set; }      // requerido si AUTORIZADO
        public string mensaje { get; set; }                   // requerido si NOAUTORIZADO/DEVUELTO
        public string referenciaAsapp { get; set; }           // opcional, solo trazabilidad
    }


    public class ComprobanteResultadoResponse
    {
        public string claveAcceso { get; set; }
        public string estado { get; set; }
        public bool yaProcesado { get; set; }
    }


    public class ComprobanteResultadoError
    {
        public string error { get; set; }
        public string code { get; set; }
    }
}
