using AIMapper.Core;
using AIMapper.Extensions;
using AIMapper.SampleApp.Models;
using AIMapper.SampleApp.Models.Generated;
using Microsoft.Extensions.DependencyInjection;

// Pastikan namespace extension Anda di-include

namespace AIMapper.SampleApp;

class Program
{
    static async Task Main(string[] args)
    {
        // 1. Setup DI/ServiceCollection (optional, best practice)
        var services = new ServiceCollection();
        services.AddAIMapper(); // Extension Anda

        var provider = services.BuildServiceProvider();

        // 2. Ambil IMapper dari DI
        var mapper = provider.GetRequiredService<IMapper>();

        var person = new Person
        {
            Id = 1,
            Name = "Gani",
            Age = 30
        };

        // Coba mapping otomatis
        var dto = person.ToPersonDto(); // <-- HARUS muncul, tidak merah!
        Console.WriteLine($"{dto.Id} - {dto.Name} - {dto.Age}");



        //// 3. Jalankan semua demo AIMapper
        await AIMapperSampleDemo.RunAllDemo(mapper);

        // Biar console tidak langsung close
        Console.WriteLine("Selesai. Tekan Enter untuk keluar.");
        Console.ReadLine();
    }
}