using AIMapper.SampleApp.Models;
using System.Text.Json;

namespace AIMapper.SampleApp;

/// <summary>
/// Demo pemetaan object Guest ke GuestDto menggunakan Source Generator (MapperGenerated).
/// Fitur yang dites:
/// - TargetProperty: [Map("FullName")]
/// - NullSubstitute: [Map(NullSubstitute = "...")]
/// - Converter: [Map(Converter = typeof(...))]
/// - Condition: [Map(Condition = typeof(...))]
/// - Flattening: Address.Street → Street
/// </summary>
public static class GuestGeneratorDemo
{
    /// <summary>
    /// Menjalankan demo pemetaan Guest → GuestDto dan menampilkan hasil serialisasi JSON.
    /// </summary>
    public static void Run()
    {
        Console.WriteLine("⚡ Demo Guest → GuestDto via MapperGenerated\n");

        var guest = new Guest
        {
            Id = 1,
            Name = "John Doe",                       // ✅ [Map("FullName")] → override properti
            Email = null,                            // ✅ [Map(NullSubstitute = "...")]
            City = "london",                         // ✅ [Map(Converter = typeof(UpperCaseConverter))]
            Phone = null,                            // ✅ [Map(Condition = typeof(NonNullCondition))]
            Address = new Address
            {
                Street = "Baker Street",             // ✅ Flattening → Street
                Country = "UK"
            }
        };

        var dto = MapperGenerated.Map(guest);
        Console.WriteLine(JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true }));
    }
}