using AIMapper.Core;
using AIMapper.Extensions; // Pastikan namespace extension Anda di-include
using Microsoft.Extensions.DependencyInjection;

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

        // 3. Jalankan semua demo AIMapper
        await AIMapperSampleDemo.RunAllDemo(mapper);

        // Biar console tidak langsung close
        Console.WriteLine("Selesai. Tekan Enter untuk keluar.");
        Console.ReadLine();
    }
}