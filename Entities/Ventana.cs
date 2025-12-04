using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace login.Entities
{
    [Table("tb_ventana")]
    public class Ventana
    {
        [Key]
        [Column("id_ventana")]
        public int IdVentana { get; set; }

        [Column("nombre_ventana")]
        [Required]
        [MaxLength(150)]
        public string? NombreVentana { get; set; }

        [Column("ruta")]
        [Required]
        [MaxLength(250)]
        public string? Ruta { get; set; }

        // Navegación
        public virtual ICollection<PerfilVentana>? PerfilVentanas { get; set; }
    }
}