namespace AIMapper;

/// <summary>
///     Konfigurasi mapping untuk sepasang tipe sumber dan tujuan.
/// </summary>
public class MappingConfiguration<TSource, TDestination>
{
    /// <summary>
    ///     Opsi per properti tujuan.
    /// </summary>
    public Dictionary<string, MappingPropertyOptions> PropertyOptions { get; } = new();
}