using System.Threading.Tasks;
using Product.Application.DTOs;

namespace Product.Application.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponse> RegisterAsync(RegisterRequest registerRequest);
        Task<TokenResponse> LoginAsync(LoginRequest loginRequest);
        Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
        Task RevokeTokenAsync(string refreshToken);
    }
}
