using SushiPOP_YA1A_2C2023_G2.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SushiPop.Models
{
    [Table("T_CARRITOITEM")]
    public class CarritoItem
    {
        

        public int Id { get; set; }

        [Display(Name = "Precio unitario con descuento")]
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public decimal PrecioUnitarioConDescuento { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public int Cantidad { get; set; }

        /*
         * Relaciones
         */

        [ForeignKey("CarritoId")]
        public int CarritoId { get; set; }
        public Carrito Carrito { get; set; }
        
        [ForeignKey("ProductoId")]
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
    }
}
