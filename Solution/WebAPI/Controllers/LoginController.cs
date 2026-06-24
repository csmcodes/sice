using System;
using System.Configuration;
using System.Net;
using System.Web.Http;
using BusinessObjects;
using BusinessLogicLayer;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/login")]
    public class LoginController : ApiController
    {
        [HttpPost]
        [Route("")]
        public IHttpActionResult Login(LoginRequest request)
        {
            if (request == null
                || string.IsNullOrEmpty(request.username)
                || string.IsNullOrEmpty(request.password))
                return Content(HttpStatusCode.BadRequest, "username y password requeridos");

            try
            {
                Usuario usuario = new Usuario { usr_id = request.username, usr_id_key = request.username };
                usuario = UsuarioBLL.GetByPK(usuario);

                if (!usuario.usr_estado.HasValue)
                    return Content(HttpStatusCode.Unauthorized, "Usuario no existe");

                if (usuario.usr_password != request.password)
                    return Content(HttpStatusCode.Unauthorized, "Contraseña incorrecta");

                string token = TokenGenerator.GenerateTokenJwt(request.username);
                int expireMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["JWT_EXPIRE_MINUTES"]);

                return Ok(new LoginResponse { token = token, expireMinutes = expireMinutes });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
