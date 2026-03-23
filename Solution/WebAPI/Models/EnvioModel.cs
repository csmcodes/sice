using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects;

namespace WebAPI.Models
{
    public class EnvioModel
    {
        public int empresa { get; set; }
        public string compania { get; set; }
        public string oficina { get; set; }
        public DateTime fecha { get; set; }
        public string invoice { get; set; }
        public int secuencia { get; set; }
        public decimal? monto { get; set; }
        public string tipopago { get; set; }

        public string idbeneficiario { get; set; }
        public string nombresbeneficiario { get; set; }
        public string paisbeneficiario { get; set; }
        public string provinciabeneficiario { get; set; }
        public string ciudadbeneficiario { get; set; }
        public string direccionbeneficiario { get; set; }
        public string telefonobeneficiario { get; set; }
        public string emailbeneficiario { get; set; }
        public string bancobeneficiario { get; set; }
        public string nrocuentabeneficiario { get; set; }
        public string tipocuentabeneficiario { get; set; }

        public string idremitente{ get; set; }
        public string nombresremitente { get; set; }
        public string provinciaremitente { get; set; }
        public string ciudadremitente { get; set; }
        public string direccionremitente { get; set; }
        public string telefonoremitente { get; set; }
        public string emailremitente { get; set; }
        public string zipcoderemitente { get; set; }

        public string observacion { get; set; }
        public string auxiliar { get; set; }



    


    }
}
