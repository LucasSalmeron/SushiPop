using SushiPOP_YA1A_2C2023_G2.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SushiPop.Models

{
    [Table("T_RESERVA")]
    public class Reserva
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public string Local { get; set; }

        [Display (Name = "Fecha y hora")]
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public DateTime FechaHora { get; set; }

        [Display(Name = "Estado")]
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public bool Confirmada { get; set; } = false;

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MinLength(5, ErrorMessage = ErrorViewModel.CaracteresMinimos)]
        [MaxLength(30, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MinLength(5, ErrorMessage = ErrorViewModel.CaracteresMinimos)]
        [MaxLength(30, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public string Apellido { get; set; }

        /*
         * Relaciones
         */
        [ForeignKey("ClienteId")]
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }  //agregue el ? por problema con modelState.isValid en reservas/Create



        
    }
}
