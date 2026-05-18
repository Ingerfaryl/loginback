namespace login.Dto
{
    public class OrdenCompraRequest
    {
        public int Opcion { get; set; } 
        public string? Emisor { get; set; } = string.Empty;
        public string? Proveedor { get; set; } = string.Empty;
        public string? Correlativo { get; set; } = string.Empty;
        public int TipoDocumento { get; set; } = 0;
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
