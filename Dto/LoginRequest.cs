using System.ComponentModel.DataAnnotations;
namespace login.Dto
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "El usuario es requerido")]
        public string? Usuario { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        public string? Contraseña { get; set; }
    }
}
