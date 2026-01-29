using login.Dto;
using login.Utils;
using login.Models;
using Microsoft.AspNetCore.Mvc;
namespace login.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PerfilController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PerfilController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost]
        public async Task<ActionResult<RespuestasApi>> GetPerfiles([FromBody] PerfilRequest parametros)
        {
            Perfiles perfilesModel = new Perfiles(_configuration);
            var respuesta = await perfilesModel.modeloPerfiles(parametros);
            return Ok(respuesta);
        }
    }
}
