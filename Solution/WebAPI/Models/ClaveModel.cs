using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    public class ClaveRequest
    {
        public string ruc { get; set; }
        public string establecimiento { get; set; }
        public string puntoemision { get; set; }
        public string secuencia { get; set; }
    }


    public class ClaveResponse
    {
        public string status { get; set; }
        public string clave { get; set; }
    }



}