using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace SushiPOP_YA1A_2C2023_G2.Models
{
    public class InfoPromocionesViewModel
    {
        public bool TienePromocion { get; set; }
        public String MensajePromocion { get; set; }
        public String RedirectCategorias { get; set; }
    }
}
