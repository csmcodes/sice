using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessLogicLayer;
using BusinessObjects;
using Newtonsoft.Json;

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

        [Authorize]
        [HttpPost]
        [Route("{claveAcceso}/resultado")]
        public IHttpActionResult PostResultado(string claveAcceso, Models.ComprobanteResultadoRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.resultado))
                return Content(HttpStatusCode.BadRequest, new Models.ComprobanteResultadoError { error = "Campo 'resultado' requerido", code = "DatosInvalidos" });

            string payloadCrudo = JsonConvert.SerializeObject(request);

            try
            {
                Packages.Offline.ResultadoAsappOutcome outcome = Packages.Offline.ProcesarResultadoDelegado(
                    claveAcceso, request.resultado, request.numeroAutorizacion, request.fechaAutorizacion,
                    request.xmlAutorizadoBase64, request.mensaje);

                List<Comprobante> comprobantes = ComprobanteBLL.GetAll(new WhereParams("com_numero={0}", claveAcceso), "");
                int empresaLog = comprobantes.Count > 0 ? comprobantes[0].com_empresa : 0;
                Packages.AsappClient.LogResultadoRecibido(empresaLog, claveAcceso, request.resultado, outcome.Detalle, payloadCrudo);

                switch (outcome.Status)
                {
                    case Packages.Offline.ResultadoAsappStatus.Aplicado:
                        return Ok(new Models.ComprobanteResultadoResponse { claveAcceso = claveAcceso, estado = Services.Enums.GetEstadoComprobante(outcome.EstadoFinal), yaProcesado = false });
                    case Packages.Offline.ResultadoAsappStatus.YaProcesado:
                        return Ok(new Models.ComprobanteResultadoResponse { claveAcceso = claveAcceso, estado = Services.Enums.GetEstadoComprobante(outcome.EstadoFinal), yaProcesado = true });
                    case Packages.Offline.ResultadoAsappStatus.NoEncontrado:
                        return Content(HttpStatusCode.NotFound, new Models.ComprobanteResultadoError { error = outcome.Detalle, code = "NotFound" });
                    case Packages.Offline.ResultadoAsappStatus.EmpresaNoDelegada:
                        return Content(HttpStatusCode.Conflict, new Models.ComprobanteResultadoError { error = outcome.Detalle, code = "EmpresaNoDelegada" });
                    case Packages.Offline.ResultadoAsappStatus.ConflictoEstado:
                        return Content(HttpStatusCode.Conflict, new Models.ComprobanteResultadoError { error = outcome.Detalle, code = "ConflictoEstado" });
                    default:
                        return Content(HttpStatusCode.BadRequest, new Models.ComprobanteResultadoError { error = outcome.Detalle, code = "DatosInvalidos" });
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
