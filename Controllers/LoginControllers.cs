using login.Data;
using login.Dto;
using login.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpPost("login")]
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

                // LOG 2: Ver si encontramos el usuario
                if (usuario == null)
                {
                    return Ok(new LoginResponse
                    {
                        Success = false,
                        Message = "Usuario no encontrado"
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
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new LoginResponse
                {
                    Success = false,
                    Message = "Error en el servidor: " + ex.Message
                });
            }
        }

        [HttpPost("registro")]
        public async Task<ActionResult<RegistroResponse>> Registro([FromBody] RegistroRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new RegistroResponse
                {
                    Success = false,
                    Message = "Datos inválidos"
                });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Validar que el usuario no exista
                var usuarioExiste = await _context.Usuarios
                    .AnyAsync(u => u.NombreUsuario == request.Usuario);

                if (usuarioExiste)
                {
                    return Ok(new RegistroResponse
                    {
                        Success = false,
                        Message = "El nombre de usuario ya está registrado"
                    });
                }

                // Validar que el correo no esté registrado (opcional pero recomendado)
                if (!string.IsNullOrEmpty(request.Correo))
                {
                    var correoExiste = await _context.Personas
                        .AnyAsync(p => p.Correo == request.Correo);

                    if (correoExiste)
                    {
                        return Ok(new RegistroResponse
                        {
                            Success = false,
                            Message = "El correo ya está registrado"
                        });
                    }
                }

                // 1. Crear la Persona
                var persona = new Persona
                {
                    Nombre = request.Nombre,
                    ApellidoP = request.ApellidoP,
                    ApellidoM = request.ApellidoM,
                    Telefono = request.Telefono,
                    Correo = request.Correo
                };

                _context.Personas.Add(persona);
                await _context.SaveChangesAsync();

                // 2. Encriptar la contraseña
                string contraseñaHash = BCrypt.Net.BCrypt.HashPassword(request.Contraseña);

                // 3. Crear el Usuario vinculado a la Persona
                var usuario = new Usuario
                {
                    IdPersona = persona.IdPersona,
                    IdPerfil = request.IdPerfil??1, // Por defecto perfil 2 (usuario normal)
                    NombreUsuario = request.Usuario,
                    Contraseña = contraseñaHash
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // Confirmar la transacción
                await transaction.CommitAsync();

                return Ok(new RegistroResponse
                {
                    Success = true,
                    Message = "Usuario registrado exitosamente",
                    IdUsuario = usuario.IdUsuario,
                    NombreUsuario = usuario.NombreUsuario
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new RegistroResponse
                {
                    Success = false,
                    Message = "Error al registrar usuario: " + ex.Message
                });
            }
        }

        [HttpPost("hash")]
        public ActionResult<string> GenerarHash([FromBody] string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return BadRequest("La contraseña no puede estar vacía.");

            try
            {
                string hash = BCrypt.Net.BCrypt.HashPassword(password);
                return Ok(new
                {
                    password,
                    hash
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al generar hash",
                    error = ex.Message
                });
            }
        }

    }
}
