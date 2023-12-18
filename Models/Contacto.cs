using SushiPOP_YA1A_2C2023_G2.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SushiPop.Models
{
    [Table("T_CONTACTO")]
    public class Contacto
    {
        public int Id { get; set; }

        [Display(Name = "Nombre Completo")]
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MaxLength(255, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public string NombreCompleto { get; set; }

        [Display(Name = "Correo electrónico")]
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MaxLength(100, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public string Email { get; set; }

        [Display(Name = "Teléfono")]
        [MinLength(10, ErrorMessage = ErrorViewModel.CaracteresMinimos)]
        [MaxLength(10, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public string Telefono { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MaxLength(1000, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public string Mensaje { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public bool Leido { get; set; }
    }
}
