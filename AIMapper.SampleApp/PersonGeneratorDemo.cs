using AIMapper.SampleApp.Models;
using System.Text.Json;

namespace AIMapper.SampleApp;

/// <summary>
/// Demo pemetaan list Person ke PersonDto menggunakan Source Generator (MapperGenerated).
/// Fitur yang dites:
/// - Mapping koleksi otomatis: List<Person> → List<PersonDto>
/// - Auto-matching by property name and type
/// </summary>
public static class PersonGeneratorDemo
{
    /// <summary>
    /// Menjalankan demo pemetaan list Person → PersonDto dan menampilkan hasil serialisasi JSON.
    /// </summary>
    public static void Run()
    {
        Console.WriteLine("👥 Demo Person → PersonDto List\n");

        var people = new List<Person>
        {
            new() { Id = 10, Name = "Alice", Age = 25 }, // 🔁 Auto-match ke PersonDto
            new() { Id = 11, Name = "Bob", Age = 30 }
        };

        // ✅ MapList: fitur koleksi otomatis oleh Source Generator
        var dtos = MapperGenerated.MapList(people);

        Console.WriteLine(JsonSerializer.Serialize(dtos, new JsonSerializerOptions { WriteIndented = true }));
    }
}