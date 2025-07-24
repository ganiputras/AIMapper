namespace AIMapper;

/// <summary>
///     Opsi konfigurasi untuk properti tujuan tertentu.
/// </summary>
public class MappingPropertyOptions
{
    public bool Ignore { get; set; } = false;
    public Func<object?, object?>? CustomConverter { get; set; }
    public Func<object?, bool>? Condition { get; set; }

    /// <summary>
    ///     Path eksplisit dari properti sumber (contoh: "BillingAddress.City").
    /// </summary>
    public string? CustomPath { get; set; }
}