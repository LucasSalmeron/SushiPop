using SushiPOP_YA1A_2C2023_G2.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SushiPop.Models
{
    [Table("T_USUARIO")]
    public abstract class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MinLength(5, ErrorMessage = ErrorViewModel.CaracteresMinimos)]
        [MaxLength(30, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public string Nombre { get; set; }


        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MinLength(5, ErrorMessage = ErrorViewModel.CaracteresMinimos)]
        [MaxLength(30, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public string Apellido { get; set; }


        [Display(Name = "Dirección")]
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MaxLength(100, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public string Direccion { get; set; }


        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MinLength(10, ErrorMessage = ErrorViewModel.CaracteresMinimos)]
        [MaxLength(10, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public string Telefono { get; set; }


        [Display(Name = "Fecha de nacimiento")]
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public DateTime FechaNacimiento { get; set; }



        [Display(Name = "Fecha de alta")]
        public DateTime? FechaAlta { get; set; }


        public bool Activo { get; set; }



        [Display(Name = "Correo electrónico")]
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MaxLength(100, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public string Email { get; set; }
    }
}