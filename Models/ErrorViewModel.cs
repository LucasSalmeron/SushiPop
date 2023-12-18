namespace SushiPOP_YA1A_2C2023_G2.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public const string CampoRequerido = "El campo {0} es obligatorio";
        public const string CaracteresMinimos = "{0} debe tener al menos {1} caracteres";
        public const string CaracteresMaximos = "{0} debe tener maximo {1} caracteres";
        public const string ValorMinMax = "{0} debe tener un valor entre {1} y {2}";
    }
}