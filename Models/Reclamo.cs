using SushiPOP_YA1A_2C2023_G2.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SushiPop.Models
{
    [Table("T_RECLAMO")]
    public class Reclamo
    {
        public int Id { get; set; }

        [Display(Name = "Nombre Completo")]
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MaxLength(255, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public string NombreCompleto { get; set; }

        [Display(Name = "Correo electrónico")]
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public string Email { get; set; }

        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MinLength(10, ErrorMessage = ErrorViewModel.CaracteresMinimos)]
        [MaxLength(10, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        
        public string Telefono { get; set; }
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MinLength(50, ErrorMessage = ErrorViewModel.CaracteresMinimos)]
        [MaxLength(1000, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public string DetalleReclamo { get; set; }

        /*
         * Relaciones
         */
        [ForeignKey("ClienteId")]
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; }
    }
}
