using login.Data;
using login.Models;
using login.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace login.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosControllers : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public UsuariosControllers(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<RespuestasApi> GetUsuarios([FromQuery] int opcion)
        {
            Usuarios usuariosModelo = new Usuarios(_configuration);
            var respuesta = await usuariosModelo.modeloUsuarios(opcion);
            return respuesta;
        }
    }
}
