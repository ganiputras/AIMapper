namespace AIMapper.Core;

/// <summary>
///     Antarmuka untuk AIMapper, menyediakan berbagai method mapping objek, konfigurasi, dan registrasi custom mapping.
/// </summary>
public interface IMapper
{
    /// <summary>
    ///     Melakukan mapping objek dari source ke destination type.
    /// </summary>
    /// <typeparam name="TSource">Tipe sumber</typeparam>
    /// <typeparam name="TDestination">Tipe tujuan</typeparam>
    /// <param name="source">Objek sumber yang akan dimapping</param>
    /// <returns>Objek hasil mapping</returns>
    TDestination Map<TSource, TDestination>(TSource source);

    /// <summary>
    ///     Registrasi custom mapping function untuk tipe tertentu.
    /// </summary>
    void Register<TSource, TDestination>(Func<TSource, TDestination> mapFunc);

    /// <summary>
    ///     Registrasi mapping dua arah antara dua tipe (forward dan reverse).
    /// </summary>
    void RegisterBidirectional<TSource, TDestination>(
        Func<TSource, TDestination> forward,
        Func<TDestination, TSource> reverse);

    /// <summary>
    ///     Registrasi tipe abstract ke concrete, untuk mendukung mapping polymorphic.
    /// </summary>
    void RegisterAbstract<TAbstract, TConcrete>() where TConcrete : TAbstract;

    /// <summary>
    ///     Konfigurasi mapping secara fluent untuk source dan destination tertentu.
    /// </summary>
    void Configure<TSource, TDestination>(Action<MappingConfiguration<TSource, TDestination>> config);

    /// <summary>
    ///     Validasi seluruh konfigurasi mapping yang sudah didaftarkan.
    /// </summary>
    void AssertConfigurationIsValid();
}