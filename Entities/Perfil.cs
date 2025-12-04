using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace login.Entities
{
    [Table("tb_perfil")]
    public class Perfil
    {
        [Key]
        [Column("id_perfil")]
        public int IdPerfil { get; set; }

        [Column("perfil")]
        [Required]
        [MaxLength(150)]
        public string? NombrePerfil { get; set; }

        // Navegación
        public virtual ICollection<Usuario>? Usuarios { get; set; }
        public virtual ICollection<PerfilVentana>? PerfilVentanas { get; set; }
    }
}
