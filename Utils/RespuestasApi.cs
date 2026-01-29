using System.Net;

namespace login.Utils
{
    public class RespuestasApi
    {
        public RespuestasApi()
        {
            ErrorMessages = new List<string>();
        }

        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public List<string> ErrorMessages { get; set; }
        public string? Message { get; set; }
        public object? Result { get; set; }
        public IEnumerable<object>? Results { get; set; }
        public byte[]? ResultByte { get; set; }
    }
}
