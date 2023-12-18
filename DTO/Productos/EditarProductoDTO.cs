using SushiPOP_YA1A_2C2023_G2.Models;
using System.ComponentModel.DataAnnotations;

namespace SushiPOP_YA1A_2C2023_G2.DTO.Productos
{
    public class EditarProductoDTO
    {
        public int Id { get; set; }


        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MinLength(20, ErrorMessage = ErrorViewModel.CaracteresMinimos)]
        [MaxLength(250, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public string Descripcion { get; set; }


        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public decimal Precio { get; set; }


        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public int Stock { get; set; }


        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public decimal Costo { get; set; }


    }
}
