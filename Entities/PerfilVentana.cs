using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace login.Entities
{
    [Table("tb_perfil_ventana")]
    public class PerfilVentana
    {
        [Column("id_perfil")]
        public int IdPerfil { get; set; }

        [Column("id_ventana")]
        public int IdVentana { get; set; }

        [Column("puede_ver")]
        public bool PuedeVer { get; set; } = true;

        [Column("puede_crear")]
        public bool PuedeCrear { get; set; } = false;

        [Column("puede_editar")]
        public bool PuedeEditar { get; set; } = false;

        [Column("puede_eliminar")]
        public bool PuedeEliminar { get; set; } = false;

        // Navegación
        [ForeignKey("IdPerfil")]
        public virtual Perfil? Perfil { get; set; }

        [ForeignKey("IdVentana")]
        public virtual Ventana? Ventana { get; set; }
    }
}