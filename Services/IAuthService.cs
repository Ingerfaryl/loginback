using login.Dto;

namespace login.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(LoginRequest request);
        Task<RegistroResponse> Registro(RegistroRequest request);
        string GenerarHash(string password);
    }
}
