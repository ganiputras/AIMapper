using AIMapper.Core;
using AIMapper.Extensions;
using AIMapper.SampleApp;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("===== AIMapper SampleApp =====\n");

/// <summary>
/// 🔧 Setup dependency injection dan registrasi IMapper dari AIMapper.Core
/// </summary>
var services = new ServiceCollection();
services.AddAIMapper(); // Registrasi AIMapper melalui extension
var provider = services.BuildServiceProvider();
var mapper = provider.GetRequiredService<IMapper>();

/// <summary>
/// ⚡ Demo 1: Mapping Guest → GuestDto menggunakan Source Generator
/// - Fitur yang dites: TargetProperty, NullSubstitute, Converter, Condition, Flattening
/// </summary>
GuestGeneratorDemo.Run();

/// <summary>
/// ⚡ Demo 2: Mapping List<Person> → List<PersonDto> menggunakan Source Generator
/// - Fitur yang dites: Mapping koleksi otomatis (MapList), auto-matching property
/// </summary>
PersonGeneratorDemo.Run();

/// <summary>
/// 🧠 Demo 3: Mapping User → UserDto menggunakan konfigurasi manual (IMapper.Configure)
/// - Fitur yang dites: Ignore, CustomPath, Condition, NullSubstitute, ValueConverter, BeforeMap, AfterMap, ReverseMap, Collection Mapping, Paging
/// </summary>
await UserManualMapperDemo.Run(mapper);
