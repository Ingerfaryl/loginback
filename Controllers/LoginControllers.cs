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
                // LOG 1: Ver qué estamos buscando
                Console.WriteLine($"Buscando usuario: {request.Usuario}");

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
                    Console.WriteLine("Usuario NO encontrado en la base de datos");
                    return Ok(new LoginResponse
                    {
                        Success = false,
                        Message = "Usuario o contraseña incorrectos"
                    });
                }

                Console.WriteLine($"Usuario encontrado: {usuario.NombreUsuario}");
                Console.WriteLine($"Hash en BD: {usuario.Contraseña}");
                Console.WriteLine($"Contraseña ingresada: {request.Contraseña}");

                // Verificar contraseña (usa BCrypt para mayor seguridad)
                bool contraseñaValida = BCrypt.Net.BCrypt.Verify(request.Contraseña, usuario.Contraseña);

                // LOG 3: Ver resultado de la verificación
                Console.WriteLine($"Contraseña válida: {contraseñaValida}");

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

        [HttpPost("generar-hash")]
        public ActionResult GenerarHash([FromBody] string password)
        {
            try
            {
                string hash = BCrypt.Net.BCrypt.HashPassword(password);

                // También probamos la verificación inmediatamente
                bool verifica = BCrypt.Net.BCrypt.Verify(password, hash);

                return Ok(new
                {
                    password,
                    hash,
                    verifica,
                    sqlUpdate = $"UPDATE tb_usuario SET contraseña = '{hash}' WHERE usuario = 'Alex2601';"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
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
                    IdPerfil = request.IdPerfil ?? 2, // Por defecto perfil 2 (usuario normal)
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
    }
}
