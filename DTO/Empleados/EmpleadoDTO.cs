using System.ComponentModel.DataAnnotations;

namespace SushiPOP_YA1A_2C2023_G2.DTO.Empleados;

public class EmpleadoDTO
{
    public int Id { get; set; }


    public int? Legajo { get; set; }


    public string Nombre { get; set; }


    public string Apellido { get; set; }

    [Display(Name = "Dirección")]
    public string Direccion { get; set; }


    [Display(Name = "Teléfono")]
    public string Telefono { get; set; }


    [Display(Name = "Fecha de nacimiento")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
    public DateTime FechaNacimiento { get; set; }


    [Display(Name = "Fecha de alta")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
    public DateTime? FechaAlta { get; set; }


    public bool? Activo { get; set; }


    [Display(Name = "Correo electrónico")]
    public string Email { get; set; }
}
