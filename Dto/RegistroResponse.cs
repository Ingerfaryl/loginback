namespace login.Dto
{
    public class RegistroResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int? IdUsuario { get; set; }
        public string? NombreUsuario { get; set; }
    }
}