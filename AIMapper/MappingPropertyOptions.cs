namespace AIMapper;

/// <summary>
/// Opsi konfigurasi untuk property pada mapping, mendukung ignore, custom path, condition, null substitution, dan value converter.
/// </summary>
public class MappingPropertyOptions
{
    /// <summary>
    /// True jika property ini ingin diabaikan pada proses mapping.
    /// </summary>
    public bool Ignore { get; set; }

    /// <summary>
    /// Path custom dari source (misal: "Parent.Child.Name") untuk override flattening property nested.
    /// </summary>
    public string? CustomPath { get; set; }

    /// <summary>
    /// Delegate predicate untuk condition mapping. Jika null, property selalu dimapping.
    /// </summary>
    internal Delegate? ConditionFunc { get; set; }

    /// <summary>
    /// Nilai default jika value source null pada saat mapping.
    /// </summary>
    public object? NullSubstitute { get; set; }

    /// <summary>
    /// Delegate custom converter untuk transformasi value property.
    /// </summary>
    public Delegate? ValueConverter { get; set; }
}