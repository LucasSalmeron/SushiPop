using SushiPOP_YA1A_2C2023_G2.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SushiPop.Models

{
    [Table("T_CLIENTE")]
    public class Cliente : Usuario

   {
        [Display(Name ="Numero de cliente")]
    
        public int? NumeroCliente { get; set; }

        /*
         * Relaciones
         */
        public ICollection<Reserva>? Reservas { get; set; }
        public ICollection<Carrito>? Carritos { get; set; }
    }
}
