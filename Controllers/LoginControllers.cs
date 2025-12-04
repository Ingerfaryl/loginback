using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using login.Data;
using login.Dto;
namespace login.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginControllers : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LoginControllers(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new LoginResponse
                {
                    Success = false,
                    Message = "Datos inválidos"
                });
            }

            try
            {
                // Buscar usuario con sus relaciones
                var usuario = await _context.Usuarios
                    .Include(u => u.Persona)
                    .Include(u => u.Perfil)
                        .ThenInclude(p => p.PerfilVentanas)
                        .ThenInclude(pv => pv.Ventana)
                    .FirstOrDefaultAsync(u => u.NombreUsuario == request.Usuario);

                if (usuario == null)
                {
                    return Ok(new LoginResponse
                    {
                        Success = false,
                        Message = "Usuario o contraseña incorrectos"
                    });
                }

                // Verificar contraseña (usa BCrypt para mayor seguridad)
                bool contraseñaValida = BCrypt.Net.BCrypt.Verify(request.Contraseña, usuario.Contraseña);

                if (!contraseñaValida)
                {
                    return Ok(new LoginResponse
                    {
                        Success = false,
                        Message = "Usuario o contraseña incorrectos"
                    });
                }

                // Construir respuesta exitosa
                var response = new LoginResponse
                {
                    Success = true,
                    Message = "Login exitoso",
                    Usuario = new UsuarioDto
                    {
                        IdUsuario = usuario.IdUsuario,
                        NombreUsuario = usuario.NombreUsuario,
                        NombreCompleto = $"{usuario.Persona.Nombre} {usuario.Persona.ApellidoP} {usuario.Persona.ApellidoM}",
                        Correo = usuario.Persona.Correo ?? "",
                        Perfil = usuario.Perfil.NombrePerfil
                    },
                    Permisos = usuario.Perfil.PerfilVentanas.Select(pv => new PermisoDto
                    {
                        NombreVentana = pv.Ventana.NombreVentana,
                        Ruta = pv.Ventana.Ruta,
                        PuedeVer = pv.PuedeVer,
                        PuedeCrear = pv.PuedeCrear,
                        PuedeEditar = pv.PuedeEditar,
                        PuedeEliminar = pv.PuedeEliminar
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new LoginResponse
                {
                    Success = false,
                    Message = "Error en el servidor: " + ex.Message
                });
            }
        }
    }
}
