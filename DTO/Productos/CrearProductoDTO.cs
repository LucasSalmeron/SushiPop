using SushiPOP_YA1A_2C2023_G2.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SushiPOP_YA1A_2C2023_G2.DTO.Productos
{
    public class CrearProductoDTO
    {
        public int Id { get; set; }


        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MaxLength(100, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        [NombreUnico(ErrorMessage = "El producto ingresado ya existe en stock.")]
        public string Nombre { get; set; }


        [Display(Name = "Descripción")]
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MinLength(20, ErrorMessage = ErrorViewModel.CaracteresMinimos)]
        [MaxLength(250, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public string Descripcion { get; set; }


        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public decimal Precio { get; set; }


        public string? Foto { get; set; }


        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public int Stock { get; set; }


        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public decimal Costo { get; set; }


        [Display(Name = "Categoría")]
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public int CategoriaId { get; set; }

        // Método de validación personalizado
    }
    public class NombreUnicoAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dbContext = (DBContext)validationContext.GetService(typeof(DBContext));
            var nombre = (string)value;

            if (dbContext.Producto.Any(p => p.Nombre == nombre))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
