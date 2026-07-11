using Application.Interfaces;
using Application.Services;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped<IMetalProcessingService, MetalProcessingService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
