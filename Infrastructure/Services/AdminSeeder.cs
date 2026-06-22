using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class AdminSeeder
    {
        public static async Task SeedAsync(AppDbContext context, IConfiguration configuration)
        {
            if (await context.Users.AnyAsync(u => u.Role == UserRole.Admin))
                return;

            var adminSection = configuration.GetSection("AdminSeed");
            var email = adminSection["Email"];
            var password = adminSection["Password"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return;

            var admin = new User
            {
                Email = email.Trim().ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = UserRole.Admin,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}
