using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessObjects;
using BusinessLogicLayer;
using WebAPI.Models;
using Newtonsoft.Json.Linq;

namespace WebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/envio")]
    public class EnvioController : ApiController
    {
        [HttpPost]
        [Route("save")]
        public IHttpActionResult Save(EnvioModel envio)
        {
            ResponseModel response = new ResponseModel();
            /*try
            {
                response.estado = Services.Enums.ResponseEstados.OK.ToString();

                Envio env = Packages.General.SaveEnvio(envio.GetEnvio());
                JObject jobj = new JObject
                {
                    {"id", env.env_id},
                    {"cod", env.env_codigo}

                };
                response.data = jobj;
                return Ok(response);
             
            }
            catch (Exception ex)
            {
                response.estado = Services.Enums.ResponseEstados.ERROR.ToString();
                response.data = ex.Message;
                return Content(HttpStatusCode.ExpectationFailed, response);
            }*/
            return Ok("");

        }
    }
}
