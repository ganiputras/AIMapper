using AIMapper.Attributes;

namespace AIMapper.SampleApp.Models;

/// <summary>
///     Contoh entitas sumber dengan konfigurasi MapAttribute.
/// </summary>
[Map(typeof(GuestDto))]
public class Guest
{
    public int Id { get; set; }

    [Map("FullName")] public string Name { get; set; } = string.Empty;

    [Map(NullSubstitute = "unknown@email.com")]
    public string? Email { get; set; }

    [Map(Converter = typeof(UpperCaseConverter))]
    public string? City { get; set; }

    [Map(Condition = typeof(NonNullCondition))]
    public string? Phone { get; set; }

    public Address? Address { get; set; }
}

/// <summary>
///     Contoh nested object yang akan di-flatten.
/// </summary>
public class Address
{
    public string? Street { get; set; }
    public string? Country { get; set; }
}

/// <summary>
///     DTO hasil mapping dari Guest.
/// </summary>
public class GuestDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    // Hasil flattening dari Guest.Address.Street
    public string? Street { get; set; }
}

/// <summary>
///     Converter untuk mengubah string menjadi huruf kapital.
/// </summary>
public static class UpperCaseConverter
{
    public static object? Convert(object? input)
    {
        return input?.ToString()?.ToUpperInvariant();
    }
}

/// <summary>
///     Condition: hanya map jika Phone tidak null.
/// </summary>
public static class NonNullCondition
{
    public static bool ShouldMap(object? source, object? target)
    {
        var phone = source?.GetType().GetProperty("Phone")?.GetValue(source) as string;
        return !string.IsNullOrEmpty(phone);
    }
}