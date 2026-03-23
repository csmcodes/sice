using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebAPI.Controllers
{    
    [RoutePrefix("api/comprobante")]
    public class ComprobanteController : ApiController
    {
        [HttpPost]
        [Route("clave")]
        public IHttpActionResult GetClave(Models.ClaveRequest claveRequest)
        {

            try
            {
                string clave = Packages.General.GetClaveComprobante(claveRequest.ruc, claveRequest.establecimiento, claveRequest.puntoemision, claveRequest.secuencia);
                Models.ClaveResponse response = new Models.ClaveResponse();
                response.status = (!string.IsNullOrEmpty(clave) ? "OK" : "ERROR");
                response.clave = (!string.IsNullOrEmpty(clave) ? clave : "CLAVE NOT FOUND");
                return Ok(response);
            }
            catch (Exception ex)
            {
                Models.ClaveResponse response = new Models.ClaveResponse();
                response.status ="ERROR";
                response.clave = ex.Message;
                return Content(HttpStatusCode.ExpectationFailed, response);
            }

        }
    }
}
