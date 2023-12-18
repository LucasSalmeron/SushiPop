using SushiPop.Models;
using SushiPOP_YA1A_2C2023_G2.DTO.Productos;
using SushiPOP_YA1A_2C2023_G2.Models;


namespace SushiPOP_YA1A_2C2023_G2.DTO
{
    public class DashboardViewModel
    {

        public List<Producto> topProductos;

        public int gananciaUltimoAño;

        public List<Cliente> topClientesMasPedidosEntregadosUltimos180;

        public List<Cliente> topClientesMayorTicketPromedioUltimos30;

        public String[] nombreApellidoClienteMasCanceladoUltimos10;


    }
}
