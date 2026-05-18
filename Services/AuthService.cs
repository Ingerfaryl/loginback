using login.Data;
using login.Dto;
using login.Entities;
using Microsoft.EntityFrameworkCore;

namespace login.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Persona)
                .Include(u => u.Perfil)
                    .ThenInclude(p => p.PerfilVentanas)
                    .ThenInclude(pv => pv.Ventana)
                .FirstOrDefaultAsync(u => u.NombreUsuario == request.Usuario);

            if (usuario == null)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            bool contraseñaValida = BCrypt.Net.BCrypt.Verify(request.Contraseña, usuario.Contraseña);

            if (!contraseñaValida)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Usuario o contraseña incorrectos"
                };
            }

            return new LoginResponse
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
        }

        public async Task<RegistroResponse> Registro(RegistroRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var usuarioExiste = await _context.Usuarios
                .AnyAsync(u => u.NombreUsuario == request.Usuario);

            if (usuarioExiste)
            {
                return new RegistroResponse
                {
                    Success = false,
                    Message = "El nombre de usuario ya está registrado"
                };
            }

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

            string hash = BCrypt.Net.BCrypt.HashPassword(request.Contraseña);

            var usuario = new Usuario
            {
                IdPersona = persona.IdPersona,
                IdPerfil = request.IdPerfil ?? 1,
                NombreUsuario = request.Usuario,
                Contraseña = hash
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new RegistroResponse
            {
                Success = true,
                Message = "Usuario registrado exitosamente",
                IdUsuario = usuario.IdUsuario
            };
        }

        public string GenerarHash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
