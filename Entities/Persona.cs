using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace login.Entities
{
    [Table("tb_persona")]
    public class Persona
    {
        [Key]
        [Column("id_persona")]
        public int IdPersona { get; set; }

        [Column("nombre")]
        [Required]
        [MaxLength(120)]
        public string? Nombre { get; set; }

        [Column("apellido_p")]
        [Required]
        [MaxLength(50)]
        public string? ApellidoP { get; set; }

        [Column("apellido_m")]
        [Required]
        [MaxLength(50)]
        public string? ApellidoM { get; set; }

        [Column("telefono")]
        [Required]
        [MaxLength(20)]
        public string? Telefono { get; set; }

        [Column("correo")]
        [MaxLength(250)]
        public string? Correo { get; set; }

        // Navegación
        public virtual Usuario? Usuario { get; set; }
    }
}
