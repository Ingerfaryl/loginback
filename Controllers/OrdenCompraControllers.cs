using login.Data;
using login.Dto;
using login.Models;
using login.Utils;
using Microsoft.AspNetCore.Mvc;

namespace login.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdenCompraControllers : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public OrdenCompraControllers(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("GetOrdenCompra")]
        public async Task<RespuestasApi> GetOrdenCompra([FromQuery] OrdenCompraRequest parametros)
        {
            OrdenCompra oc = new OrdenCompra(_configuration);
            var resultado = await oc.modeloOrdenCompra(parametros);
            return resultado;
        }
    }
}
