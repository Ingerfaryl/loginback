
using login.Models;
using login.Utils;
using Microsoft.AspNetCore.Mvc;

namespace login.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProveedorControllers : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ProveedorControllers(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("GetProveedor")]
        public async Task<RespuestasApi> GetProveedor([FromQuery] int opcion)
        {
            Proveedor proveedor = new Proveedor(_configuration);
            var resultado = await proveedor.modeloProveedor(opcion);
            return resultado;
        }
    }
}
