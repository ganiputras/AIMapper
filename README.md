![AIMapper Logo](https://raw.githubusercontent.com/ganiputras/AIMapper/master/logo.png)

# AIMapper

**Fleksibel, intuitif, dan performa tinggi untuk object mapping di .NET**

[![.NET](https://img.shields.io/badge/.NET-8%2B-blueviolet?style=flat-square)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/license-MIT-green.svg?style=flat-square)](LICENSE.txt)
[![Status: Production Ready](https://img.shields.io/badge/status-production--ready-brightgreen?style=flat-square)](https://www.nuget.org/packages/AIMapper)

---

## ✨ Tentang AIMapper

**AIMapper** adalah library object-to-object mapping untuk .NET 8 yang mendukung konfigurasi berbasis atribut maupun fluent interface.  
Dirancang sebagai alternatif ringan AutoMapper, AIMapper mendukung:

- Flattening & unflattening properti nested
- Mapping berbasis atribut `[Map]`
- Konfigurasi fluent (`cfg.ForMember(...)`)
- Null substitute, converter, condition, ignore
- Paging async dan collection projection
- ReverseMap dan mapping profile terpusat
- Tanpa konfigurasi runtime

---

## 📦 Instalasi

```bash
dotnet add package AIMapper
```

---

## 🚀 Quick Start

### 1. Definisikan Model & DTO
```csharp
public class User {
    public int Id { get; set; }
    public string? Name { get; set; }
    public UserProfile? Profile { get; set; }
}

public class UserProfile {
    public string? Address { get; set; }
}

public class UserDto {
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
}
```

### 2. Tambahkan ke DI
```csharp
services.AddAIMapper();
```

### 3. Konfigurasi Mapping
```csharp
mapper.Configure<User, UserDto>(cfg => {
    cfg.ForMember("Address", o => o.CustomPath = "Profile.Address");
});
```

### 4. Mapping Object & Koleksi
```csharp
var dto = mapper.Map<User, UserDto>(user);
var dtos = users.ProjectTo<User, UserDto>(mapper).ToList();
```

---

## 🎯 Fitur Fluent Mapping

| Fitur             | Contoh |
|------------------|--------|
| **Ignore**       | `cfg.ForMember("Name", o => o.Ignore());` |
| **CustomPath**   | `cfg.ForMember("Address", o => o.CustomPath = "Profile.Address");` |
| **Condition**    | `cfg.ForMember("IsActive", o => o.Condition<User, UserDto>((src, dest) => src.Status == StatusEnum.Active));` |
| **NullSubstitute** | `cfg.ForMember("Email", o => o.NullSubstitute("default@email.com"));` |
| **ValueConverter** | `cfg.ForMember("Registered", o => o.ConvertUsing<DateTime?, string?>(dt => dt?.ToString("yyyy-MM-dd")));` |
| **BeforeMap / AfterMap** | `cfg.BeforeMap(...); cfg.AfterMap(...);` |
| **ReverseMap**   | `cfg.ReverseMap((Mapper)mapper);` |
| **Paging Async** | `var pageDto = await users.ProjectToPagedListAsync<User, UserDto>(mapper, 1, 10);` |

---

## 🔥 Contoh MappingProfile

```csharp
public class UserProfileMap : MapperProfile
{
    public override void Configure(IMapper mapper)
    {
        mapper.Configure<User, UserDto>(cfg =>
        {
            cfg.ForMember("Address", o => o.CustomPath = "Profile.Address");
            cfg.ForMember("Email", o => o.NullSubstitute("default@email.com"));
            cfg.ForMember("Registered", o => o.ConvertUsing<DateTime?, string?>(dt => dt?.ToString("yyyy-MM-dd")));
            cfg.ForMember("IsActive", o => o.Condition<User, UserDto>((src, dest) => src.Status == StatusEnum.Active));
            cfg.ForMember("Name", o => o.Ignore());
            cfg.BeforeMap((src, dest) => dest.Point = src.Point ?? 0);
            cfg.AfterMap((src, dest) => dest.Name = src.Name?.ToUpper());
            cfg.ReverseMap((Mapper)mapper);
        });

        mapper.Configure<Order, OrderDto>(cfg =>
        {
            cfg.ForMember("CustomerName", o => o.CustomPath = "Customer.Name");
            cfg.ForMember("Total", o => o.NullSubstitute(0));
            cfg.AfterMap((src, dest) => dest.IsPaid = src.Status == OrderStatus.Paid);
            cfg.ReverseMap((Mapper)mapper);
        });
    }
}
```

### Registrasi MappingProfile:
```csharp
mapper.ApplyProfilesFromCurrentAssembly();
```

---

## 📁 Contoh Lengkap

Lihat `AIMapperSampleDemo.cs` di project [AIMapper.SampleApp](https://github.com/ganiputras/AIMapper/tree/master/AIMapper.SampleApp) untuk contoh implementasi penuh.

---



## 📚 Table of Contents

- [🔌 Instalasi & Registrasi](#-instalasi--registrasi)
- [🔧 Basic Mapping](#-basic-mapping)
- [🎛️ Custom Mapping (ForMember)](#-custom-mapping-formember)
- [📦 Flattening Otomatis](#-flattening-otomatis)
- [✳️ Override Flattening](#️-override-flattening)
- [🔁 Reverse Mapping](#-reverse-mapping)
- [📚 Collection Mapping](#-collection-mapping)
- [🔍 LINQ Projection (`ProjectTo`)](#-linq-projection-projectto)
- [🧰 Extension Method `.MapTo<T>()`](#-extension-method-maptot)
- [⚙️ Global Configuration](#️-global-configuration)
- [🛠️ Manual Mapper (tanpa DI)](#️-manual-mapper-tanpa-di)
- [🚫 Ignore Property](#-ignore-property)
- [✅ Validasi Mapping](#-validasi-mapping)
- [🧠 Source Generator + `MapAttribute`](#-source-generator--mapattribute)
- [🛠 Struktur & Debug Source Generator](#-struktur--debug-source-generator)



## 🔌 Instalasi & Registrasi

```bash
dotnet add package AIMapper
```

```csharp
services.AddAIMapper(typeof(MyMapperProfile).Assembly);
```



## 🔧 Basic Mapping

```csharp
var dto = mapper.Map<PersonDto>(person);
```



## 🎛️ Custom Mapping (ForMember)

```csharp
cfg.CreateMap<Person, PersonDto>()
   .ForMember(dest => dest.AddressCity, opt => opt.MapFrom(src => src.Address.City));
```



## 📦 Flattening Otomatis

```csharp
public class PersonDto
{
    public string AddressCity { get; set; }
}
```

Jika `Person.Address.City` ada, maka `AddressCity` akan otomatis terisi.



## ✳️ Override Flattening

Gunakan `.ForMember(...).MapFrom(...)` untuk override flattening otomatis.



## 🔁 Reverse Mapping

```csharp
cfg.CreateMap<Person, PersonDto>().ReverseMap();
```



## 📚 Collection Mapping

```csharp
List<PersonDto> dtos = mapper.Map<List<PersonDto>>(people);
```



## 🔍 LINQ Projection (`ProjectTo`)

```csharp
var dtos = dbContext.People.ProjectTo<PersonDto>(mapper);
```



## 🧰 Extension Method `.MapTo<T>()`

```csharp
var dto = person.MapTo<PersonDto>();
```



## ⚙️ Global Configuration

```csharp
services.AddAIMapper(cfg =>
{
    cfg.NullSubstitution = "-";
    cfg.IgnoreUnmapped = true;
});
```



## 🛠️ Manual Mapper (tanpa DI)

```csharp
var config = new MapperConfigurationExpression();
config.CreateMap<Person, PersonDto>();
var mapper = new Mapper(config);
```



## 🚫 Ignore Property

```csharp
.ForMember(dest => dest.IgnoredField, opt => opt.Ignore());
```



## ✅ Validasi Mapping

```csharp
mapper.Configuration.AssertConfigurationIsValid();
```



## 🧠 Source Generator + `MapAttribute`

```csharp
public class PersonDto
{
    [Map(Source = "Name")]
    [Map(NullSubstitute = "Unknown")]
    [Map(Condition = "x => !string.IsNullOrWhiteSpace(x)")]
    [Map(Converter = "x => x.ToUpperInvariant()")]
    public string DisplayName { get; set; }
}
```


## 📄 Lisensi

Proyek ini menggunakan lisensi [MIT License](LICENSE.txt).
