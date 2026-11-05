using Mapster;
using Microsoft.Extensions.DependencyInjection;
using UmkmJatim.Application.Auth.DTOs;
using UmkmJatim.Application.Users.DTOs;
using UmkmJatim.Domain.Entities;

namespace UmkmJatim.Application.Mappings;

public static class MappingConfig
{
    public static void RegisterMappings(TypeAdapterConfig config)
    {
        config.NewConfig<ApplicationUser, UserDto>();
        config.NewConfig<ApplicationUser, UserProfileDto>();
        config.NewConfig<Address, AddressDto>();
    }

    public static IServiceCollection AddApplicationMappings(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        RegisterMappings(config);
        services.AddSingleton(config);
        return services;
    }
}
