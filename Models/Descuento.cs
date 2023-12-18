using SushiPOP_YA1A_2C2023_G2.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SushiPop.Models
{
    [Table("T_DESCUENTO")]
    public class Descuento
    {

        public int Id { get; set; }

        //[Range(1, 7, ErrorMessage = ErrorViewModel.ValorMinMax)]
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public int Dia { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public int Porcentaje { get; set; }


        [Display(Name = "Descuento maximo")]
        public decimal DescuentoMaximo { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public bool Activo { get; set; }



        /*
         * Relaciones 
         */

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [ForeignKey("ProductoId")]
        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }
    }
}
