namespace login.Dto
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public UsuarioDto? Usuario { get; set; }
        public List<PermisoDto>? Permisos { get; set; }
    }

    public class UsuarioDto
    {
        public int IdUsuario { get; set; }
        public string? NombreUsuario { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Correo { get; set; }
        public string? Perfil { get; set; }
    }

    public class PermisoDto
    {
        public string? NombreVentana { get; set; }
        public string? Ruta { get; set; }
        public bool PuedeVer { get; set; }
        public bool PuedeCrear { get; set; }
        public bool PuedeEditar { get; set; }
        public bool PuedeEliminar { get; set; }
    }
}
