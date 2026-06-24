using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Services
{
    public class AuthService(IUnitOfWork unitOfWork, IJwtService jwtService) : IAuthService
    {
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                throw new AppValidationException("Email и пароль обязательны.");

            if (dto.Password.Length < 6)
                throw new AppValidationException("Пароль должен содержать не менее 6 символов.");

            if (await unitOfWork.Users.ExistsWithEmailAsync(dto.Email, cancellationToken))
                throw new AppValidationException("Пользователь с таким Email уже существует.");

            var user = new User
            {
                Email = dto.Email.Trim().ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRole.User
            };

            await unitOfWork.Users.AddAsync(user, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new AuthResponseDto { Id = user.Id, Email = user.Email, Role = user.Role.ToString() };
        }

        public async Task<(AuthResponseDto user, string token)> LoginAsync(LoginDto dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                throw new AppValidationException("Email и пароль обязательны.");

            var user = await unitOfWork.Users.GetByEmailAsync(dto.Email.Trim().ToLower(), cancellationToken);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new AppValidationException("Неверный Email или пароль.");

            var token = jwtService.GenerateToken(user);
            var response = new AuthResponseDto { Id = user.Id, Email = user.Email, Role = user.Role.ToString() };

            return (response, token);
        }

        public async Task<AuthResponseDto> GetProfileAsync(int userId, CancellationToken cancellationToken)
        {
            var user = await unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user == null) throw new NotFoundException("Пользователь", userId);

            return new AuthResponseDto { Id = user.Id, Email = user.Email, Role = user.Role.ToString() };
        }

        public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(dto.CurrentPassword) || string.IsNullOrWhiteSpace(dto.NewPassword))
                throw new AppValidationException("Оба поля обязательны.");

            if (dto.NewPassword.Length < 6)
                throw new AppValidationException("Новый пароль должен содержать не менее 6 символов.");

            var user = await unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user == null) throw new NotFoundException("Пользователь", userId);

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                throw new AppValidationException("Текущий пароль неверен.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            unitOfWork.Users.Update(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
