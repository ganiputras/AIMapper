using Microsoft.Extensions.DependencyInjection;

namespace AIMapper.Extensions;

/// <summary>
/// Ekstensi untuk registrasi AIMapper (IMapper) ke dalam dependency injection (DI) container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Mendaftarkan IMapper sebagai singleton pada IServiceCollection, sehingga dapat digunakan di seluruh aplikasi.
    /// </summary>
    /// <param name="services">Instance IServiceCollection yang akan di-extend.</param>
    /// <returns>IServiceCollection untuk keperluan chaining.</returns>
    public static IServiceCollection AddAIMapper(this IServiceCollection services)
    {
        services.AddSingleton<IMapper, Mapper>(); // Registrasi IMapper dengan implementasi Mapper
        return services;
    }
}