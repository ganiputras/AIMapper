namespace AIMapper.Core;

/// <summary>
/// Konfigurasi mapping untuk setiap pasangan type.
/// </summary>
public class MappingConfiguration<TSource, TDestination>
{
    public Dictionary<string, MappingPropertyOptions> PropertyOptions { get; } = new();

    public Action<TSource, TDestination>? BeforeMapAction { get; private set; }
    public Action<TSource, TDestination>? AfterMapAction { get; private set; }

    public MappingConfiguration<TSource, TDestination> ForMember(
        string destinationProperty,
        Action<MappingPropertyOptions> options)
    {
        if (!PropertyOptions.ContainsKey(destinationProperty))
            PropertyOptions[destinationProperty] = new MappingPropertyOptions();
        options(PropertyOptions[destinationProperty]);
        return this;
    }

    public MappingConfiguration<TSource, TDestination> Ignore(string destinationProperty)
    {
        ForMember(destinationProperty, o => o.Ignore = true);
        return this;
    }

    public MappingConfiguration<TSource, TDestination> BeforeMap(Action<TSource, TDestination> action)
    {
        BeforeMapAction = action;
        return this;
    }

    public MappingConfiguration<TSource, TDestination> AfterMap(Action<TSource, TDestination> action)
    {
        AfterMapAction = action;
        return this;
    }

    /// <summary>
    /// Membalik konfigurasi mapping: otomatis buat mapping dari destination ke source,
    /// dan mendaftarkan ke instance Mapper (reverse mapping langsung aktif).
    /// </summary>
    /// <param name="mapper">Instance Mapper untuk mendaftarkan reverse mapping.</param>
    /// <returns>Instance MappingConfiguration dari destination ke source (reverse).</returns>
    public MappingConfiguration<TDestination, TSource> ReverseMap(Mapper mapper)
    {
        var reverseConfig = new MappingConfiguration<TDestination, TSource>();

        // Copy property options yang dapat direverse
        foreach (var kv in PropertyOptions)
        {
            var propName = kv.Key;
            var opt = kv.Value;
            var reverseOption = new MappingPropertyOptions
            {
                Ignore = opt.Ignore,
                NullSubstitute = opt.NullSubstitute,
                // Catatan: CustomPath/ValueConverter tidak otomatis dibalik
            };
            reverseConfig.PropertyOptions[propName] = reverseOption;
        }

        // Registrasi reverseConfig langsung ke dictionary Mapper
        mapper.RegisterReverseConfig(reverseConfig);

        return reverseConfig;
    }



}
