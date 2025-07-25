![AIMapper Logo](https://raw.githubusercontent.com/ganiputras/AIMapper/refs/heads/master/logo.png)

# AIMapper

**Fleksibel, intuitif, dan performa tinggi untuk object mapping di .NET**

[![.NET](https://img.shields.io/badge/.NET-8%2B-blueviolet?style=flat-square)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/license-MIT-green.svg?style=flat-square)](https://github.com/ganiputras/AIMapper/blob/main/LICENSE)
[![Status: Production Ready](https://img.shields.io/badge/status-production--ready-brightgreen?style=flat-square)](https://github.com/ganiputras/AIMapper)

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

---

## 🔌 Instalasi & Registrasi

```bash
dotnet add package AIMapper
```

```csharp
services.AddAIMapper(typeof(MyMapperProfile).Assembly);
```

---

## 🔧 Basic Mapping

```csharp
var dto = mapper.Map<PersonDto>(person);
```

---

## 🎛️ Custom Mapping (ForMember)

```csharp
cfg.CreateMap<Person, PersonDto>()
   .ForMember(dest => dest.AddressCity, opt => opt.MapFrom(src => src.Address.City));
```

---

## 📦 Flattening Otomatis

```csharp
public class PersonDto
{
    public string AddressCity { get; set; }
}
```

Jika `Person.Address.City` ada, maka `AddressCity` akan otomatis terisi.

---

## ✳️ Override Flattening

Gunakan `.ForMember(...).MapFrom(...)` untuk override flattening otomatis.

---

## 🔁 Reverse Mapping

```csharp
cfg.CreateMap<Person, PersonDto>().ReverseMap();
```

---

## 📚 Collection Mapping

```csharp
List<PersonDto> dtos = mapper.Map<List<PersonDto>>(people);
```

---

## 🔍 LINQ Projection (`ProjectTo`)

```csharp
var dtos = dbContext.People.ProjectTo<PersonDto>(mapper);
```

---

## 🧰 Extension Method `.MapTo<T>()`

```csharp
var dto = person.MapTo<PersonDto>();
```

---

## ⚙️ Global Configuration

```csharp
services.AddAIMapper(cfg =>
{
    cfg.NullSubstitution = "-";
    cfg.IgnoreUnmapped = true;
});
```

---

## 🛠️ Manual Mapper (tanpa DI)

```csharp
var config = new MapperConfigurationExpression();
config.CreateMap<Person, PersonDto>();
var mapper = new Mapper(config);
```

---

## 🚫 Ignore Property

```csharp
.ForMember(dest => dest.IgnoredField, opt => opt.Ignore());
```

---

## ✅ Validasi Mapping

```csharp
mapper.Configuration.AssertConfigurationIsValid();
```

---

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

---

## 🛠 Struktur & Debug Source Generator

- `MapAttribute.cs`: atribut yang dipindai
- `SyntaxReceiver`: mendeteksi properti yang relevan
- `MappingClassBuilder`: membangun kode `partial` saat compile
- Output berada di folder `obj/Debug/net8.0/AIMapper.Generators/*.g.cs`
- Untuk debug, gunakan `context.ReportDiagnostic(...)` atau log ke file

---

