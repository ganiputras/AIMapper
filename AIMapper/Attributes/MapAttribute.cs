namespace AIMapper.Attributes;

/// <summary>
///     Menandakan bahwa class atau properti akan otomatis dibuatkan mapping ke tipe target.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
public sealed class MapAttribute : Attribute
{
    /// <summary>
    ///     Inisialisasi MapAttribute untuk class (menentukan target type).
    /// </summary>
    /// <param name="targetType">Tipe tujuan mapping.</param>
    public MapAttribute(Type targetType)
    {
        TargetType = targetType;
    }

    /// <summary>
    ///     Inisialisasi MapAttribute untuk properti (override target property).
    /// </summary>
    /// <param name="targetProperty">Nama properti tujuan.</param>
    public MapAttribute(string targetProperty)
    {
        TargetProperty = targetProperty;
    }

    /// <summary>
    ///     Inisialisasi tanpa parameter (untuk properti dengan konfigurasi lanjutan).
    /// </summary>
    public MapAttribute()
    {
    }

    /// <summary>
    ///     Tipe target mapping (hanya untuk class).
    /// </summary>
    public Type? TargetType { get; }

    /// <summary>
    ///     Nama properti tujuan jika berbeda (override mapping properti).
    /// </summary>
    public string? TargetProperty { get; set; }

    /// <summary>
    ///     Nilai default jika sumber null.
    /// </summary>
    public object? NullSubstitute { get; set; }

    /// <summary>
    ///     Tipe converter statis, harus memiliki method static: object Convert(object)
    /// </summary>
    public Type? Converter { get; set; }

    /// <summary>
    ///     Tipe kondisi, harus memiliki method static: bool ShouldMap(source, destination)
    /// </summary>
    public Type? Condition { get; set; }
}