using SushiPOP_YA1A_2C2023_G2.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SushiPop.Models
{
    [Table("T_CARRITO")]
    public class Carrito
    {

        public int Id { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public bool Procesado { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public bool Cancelado { get; set; }

        /*
         * Relaciones
         */
        [ForeignKey("ClienteId")]
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        public virtual Pedido Pedido { get; set; }
        public ICollection<CarritoItem> CarritosItems { get; set; }
    }
}
