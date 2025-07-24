using AIMapper;

/// <summary>
/// Konfigurasi mapping untuk setiap pasangan type, support ForMember & CustomPath.
/// </summary>
public class MappingConfiguration<TSource, TDestination>
{
    /// <summary>
    /// Koleksi opsi property mapping per nama property destination.
    /// </summary>
    public Dictionary<string, MappingPropertyOptions> PropertyOptions { get; } = new();

    /// <summary>
    /// Atur custom mapping property, misal: override flattening, ignore property, dsb.
    /// </summary>
    /// <param name="destinationProperty">Nama property tujuan (destination).</param>
    /// <param name="options">Konfigurasi property.</param>
    /// <returns>Instance konfigurasi untuk chaining.</returns>
    public MappingConfiguration<TSource, TDestination> ForMember(
        string destinationProperty,
        Action<MappingPropertyOptions> options)
    {
        if (!PropertyOptions.ContainsKey(destinationProperty))
            PropertyOptions[destinationProperty] = new MappingPropertyOptions();

        options(PropertyOptions[destinationProperty]); // Set konfigurasi property
        return this;
    }
}