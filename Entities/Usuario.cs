using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace login.Entities
{
    [Table("tb_usuario")]
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("id_persona")]
        public int IdPersona { get; set; }

        [Column("id_perfil")]
        public int IdPerfil { get; set; }

        [Column("usuario")]
        [Required]
        [MaxLength(250)]
        public string? NombreUsuario { get; set; }

        [Column("contraseña")]
        [Required]
        [MaxLength(250)]
        public string? Contraseña { get; set; }

        // Navegación
        [ForeignKey("IdPersona")]
        public virtual Persona? Persona { get; set; }

        [ForeignKey("IdPerfil")]
        public virtual Perfil? Perfil { get; set; }
    }
}
