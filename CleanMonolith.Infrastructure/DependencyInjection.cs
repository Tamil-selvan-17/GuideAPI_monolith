using CleanMonolith.Application.Interfaces;
using CleanMonolith.Application.Services;
using CleanMonolith.Infrastructure.Entity;
using CleanMonolith.Infrastructure.Identity;
using CleanMonolith.Infrastructure.Persistence;
using CleanMonolith.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace CleanMonolith.Infrastructure;

public static class DependencyInjection
{
  
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<nEdit_DEVContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IAIService, AIService>();

        services.AddHttpClient<IAIService, AIService>(client =>
        {
            client.BaseAddress = new Uri("https://api.openai.com/");
            client.Timeout = TimeSpan.FromSeconds(60);
        })
.ConfigurePrimaryHttpMessageHandler(() =>
{
    return new HttpClientHandler
    {
        // 🔥 ONLY FOR LOCAL DEV
        ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };
});


        return services;
    }
}
