using System.ComponentModel.DataAnnotations;

namespace login.Dto
{
    public class RegistroRequest
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(250, MinimumLength = 4, ErrorMessage = "El usuario debe tener entre 4 y 250 caracteres")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Contraseña { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(120, ErrorMessage = "El nombre no puede exceder 120 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido paterno es requerido")]
        [StringLength(50, ErrorMessage = "El apellido paterno no puede exceder 50 caracteres")]
        public string ApellidoP { get; set; }

        [Required(ErrorMessage = "El apellido materno es requerido")]
        [StringLength(50, ErrorMessage = "El apellido materno no puede exceder 50 caracteres")]
        public string ApellidoM { get; set; }

        [Required(ErrorMessage = "El teléfono es requerido")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        public string Telefono { get; set; }

        [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
        [StringLength(250, ErrorMessage = "El correo no puede exceder 250 caracteres")]
        public string? Correo { get; set; }

        public int? IdPerfil { get; set; } 
    }
}
