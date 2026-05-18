using login.Data;
using login.Dto;
using login.Entities;
using login.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace login.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginControllers : ControllerBase
    {
        private readonly IAuthService _authService;

        public LoginControllers(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.Login(request);
            return Ok(result);
        }

        [HttpPost("registro")]
        public async Task<IActionResult> Registro(RegistroRequest request)
        {
            var result = await _authService.Registro(request);
            return Ok(result);
        }

        [HttpPost("hash")]
        public IActionResult Hash(string password)
        {
            var hash = _authService.GenerarHash(password);
            return Ok(new { password, hash });
        }
    }
}
