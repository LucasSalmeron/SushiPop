using SushiPOP_YA1A_2C2023_G2.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SushiPop.Models
{
    [Table("T_PEDIDO")]
    public class Pedido
    {
        [Key]
        
        public int Id { get; set; }

        [Display(Name = "Numero de pedido")]
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public int NroPedido { get; set; }

        [Display(Name = "Fecha de Compra")]
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public DateTime FechaCompra { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public decimal Subtotal { get; set; }

        [Display(Name = "Gasto de envio")]
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public decimal GastoEnvio { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public decimal Total { get; set; }

        //[Range(1, 6, ErrorMessage = ErrorViewModel.CampoRequerido)] 
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public int Estado { get; set; }

        /*
         * Relaciones
         */
        
        public virtual Reclamo Reclamo { get; set; }

        [ForeignKey("CarritoId")]
        public int CarritoId { get; set; }
        public Carrito Carrito { get; set; }
    }
}
