using Microsoft.Extensions.DependencyInjection;

namespace AIMapper.Extensions;

/// <summary>
///     Ekstensi untuk registrasi IMapper ke dalam DI.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAIMapper(this IServiceCollection services)
    {
        services.AddSingleton<IMapper, Mapper>();
        return services;
    }
}