using System.ComponentModel.DataAnnotations;

namespace SushiPOP_YA1A_2C2023_G2.DTO
{
    public enum EstadoPedidoEnum
    {
        [Display(Name = "Sin Confirmar")]
        SinConfirmar = 1,

        Confirmado = 2,

        [Display(Name = "En Preparación")]
        EnPreparacion = 3,

        [Display(Name = "En Reparto")]
        EnReparto = 4,

        Entregado = 5,
        Cancelado = 6,

    }
}
