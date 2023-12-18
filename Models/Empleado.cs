using SushiPOP_YA1A_2C2023_G2.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SushiPop.Models
{
    [Table("T_EMPLEADO")]
    public class Empleado : Usuario
    {

        public int? Legajo { get; set; }
    }
}
