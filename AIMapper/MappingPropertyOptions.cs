namespace AIMapper;

/// <summary>
/// Opsi konfigurasi untuk property pada mapping. 
/// Mendukung pengabaian (ignore) dan path custom (override flattening).
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
}