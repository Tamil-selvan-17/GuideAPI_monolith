using System.Reflection;
using CleanMonolith.Application.Interfaces;
using CleanMonolith.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CleanMonolith.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile<CleanMonolith.Application.Mappings.MappingProfile>());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
