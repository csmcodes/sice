using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessLogicLayer;
using BusinessObjects;

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

        [Authorize]
        [HttpGet]
        [Route("{claveAcceso}/estado")]
        public IHttpActionResult GetEstado(string claveAcceso)
        {
            try
            {
                List<Comprobante> comprobantes = ComprobanteBLL.GetAll(new WhereParams("com_numero={0}", claveAcceso), "");
                if (comprobantes.Count == 0)
                    return NotFound();

                Comprobante c = comprobantes[0];
                var response = new Models.ComprobanteEstadoResponse
                {
                    claveAcceso       = c.com_numero,
                    numeroComprobante = c.com_almacen + "-" + c.com_pventa + "-" + c.com_secuencia,
                    estado            = Services.Enums.GetEstadoComprobante(c.com_estado),
                    fechaAutorizacion = string.IsNullOrEmpty(c.com_fechaautorizacion) ? null : c.com_fechaautorizacion,
                    numeroAutorizacion = string.IsNullOrEmpty(c.com_autorizacion) ? null : c.com_autorizacion,
                    mensaje           = string.IsNullOrEmpty(c.com_mensaje) ? null : c.com_mensaje
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
