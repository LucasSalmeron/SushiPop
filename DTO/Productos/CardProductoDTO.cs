namespace SushiPOP_YA1A_2C2023_G2.DTO.Productos
{
    public class CardProductoDTO
    {
        public int Id { get; set; }


        public string Nombre { get; set; }


        public string Descripcion { get; set; }

        public int Stock { get; set; }

        public decimal Precio { get; set; }

        public decimal Descuento { get; set; }

        public string? Foto { get; set; }

    }
}
