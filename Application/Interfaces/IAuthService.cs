using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken);
        Task<(AuthResponseDto user, string token)> LoginAsync(LoginDto dto, CancellationToken cancellationToken);
        Task<AuthResponseDto> GetProfileAsync(int userId, CancellationToken cancellationToken);
        Task ChangePasswordAsync(int userId, ChangePasswordDto dto, CancellationToken cancellationToken);
    }
}
