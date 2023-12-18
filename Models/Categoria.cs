using SushiPOP_YA1A_2C2023_G2.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SushiPop.Models
{
    [Table("T_CATEGORIA")]
    public class Categoria

    {

        public int Id { get; set; }


        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MaxLength(100, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public string Nombre { get; set; }


        [MaxLength(1000, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public string? Descripcion { get; set; }
        
        /*
         * Relaciones
         */        
        public ICollection<Producto>? Productos { get; set; }
    }
}
