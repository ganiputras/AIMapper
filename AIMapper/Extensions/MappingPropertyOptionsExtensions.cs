using AIMapper.Core;

namespace AIMapper.Extensions;

/// <summary>
///     Extension method agar konfigurasi property mapping lebih fluent (chaining).
/// </summary>
public static class MappingPropertyOptionsExtensions
{
    /// <summary>
    ///     Menandai property untuk diabaikan pada mapping (fluent).
    /// </summary>
    /// <param name="options">Opsi property mapping</param>
    public static void Ignore(this MappingPropertyOptions options)
    {
        options.Ignore = true;
    }

    /// <summary>
    ///     Menentukan syarat/predicate mapping property.
    /// </summary>
    /// <typeparam name="TSource">Tipe source.</typeparam>
    /// <typeparam name="TDestination">Tipe destination.</typeparam>
    /// <param name="options">Opsi property mapping.</param>
    /// <param name="predicate">Predicate menerima (source, destination), return true jika boleh mapping.</param>
    public static void Condition<TSource, TDestination>(
        this MappingPropertyOptions options,
        Func<TSource, TDestination, bool> predicate)
    {
        options.ConditionFunc = predicate;
    }

    /// <summary>
    ///     Set nilai default jika source bernilai null saat mapping.
    /// </summary>
    /// <param name="options">Opsi property mapping.</param>
    /// <param name="value">Nilai pengganti jika source null.</param>
    public static void NullSubstitute(this MappingPropertyOptions options, object? value)
    {
        options.NullSubstitute = value;
    }

    /// <summary>
    ///     Set custom value converter untuk property ini.
    /// </summary>
    /// <typeparam name="TSourceProp">Tipe property source</typeparam>
    /// <typeparam name="TDestProp">Tipe property destination</typeparam>
    /// <param name="options">Opsi property mapping</param>
    /// <param name="converter">Fungsi converter (source → destination)</param>
    public static void ConvertUsing<TSourceProp, TDestProp>(
        this MappingPropertyOptions options,
        Func<TSourceProp, TDestProp> converter)
    {
        options.ValueConverter = converter;
    }
}