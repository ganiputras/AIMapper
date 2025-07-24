![AIMapper Logo](https://raw.githubusercontent.com/ganiputras/AIMapper/refs/heads/master/logo.png)


# AIMapper

**Fleksibel, intuitif, dan performa tinggi untuk object mapping di .NET**

[![.NET](https://img.shields.io/badge/.NET-8%2B-blueviolet?style=flat-square)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/license-MIT-green.svg?style=flat-square)](https://github.com/ganiputras/AIMapper/blob/main/LICENSE)
[![Status: Production Ready](https://img.shields.io/badge/status-production--ready-brightgreen?style=flat-square)](https://github.com/ganiputras/AIMapper)

---

## ✨ AIMapper

**AIMapper** adalah library object-to-object mapping untuk .NET  
yang menawarkan konfigurasi mapping yang fleksibel, intuitif, dan performa tinggi.  
AIMapper mendukung flattening, custom path, value converter, collection mapping async, paging, dan banyak lagi,  
sehingga dapat menjadi solusi mapping yang scalable dan siap untuk aplikasi modern maupun enterprise.

---

## 📦 Instalasi

```sh
dotnet add package AIMapper
```


## 🚀 Quick Start
1. Definisikan Model & DTO
 ```sh       
public class User { public int Id { get; set; } public string? Name { get; set; } public UserProfile? Profile { get; set; } }
public class UserProfile { public string? Address { get; set; } }
public class UserDto { public int Id { get; set; } public string? Name { get; set; } public string? Address { get; set; } }
```

2. Tambahkan ke DI
```sh   
services.AddAIMapper();
```

3. Konfigurasi Mapping

 ```sh   
mapper.Configure<User, UserDto>(cfg => {
    cfg.ForMember("Address", o => o.CustomPath = "Profile.Address");
});
```

4. Mapping Object & Koleksi
 ```sh   
var dto = mapper.Map<User, UserDto>(user);
var dtos = users.ProjectTo<User, UserDto>(mapper).ToList();
```

## 🎯 Fitur Utama (Contoh)
Ignore Property:

```sh 
cfg.ForMember("Name", o => o.Ignore());
```

CustomPath (Override Flattening):
```sh 
cfg.ForMember("Address", o => o.CustomPath = "Profile.Address");

```

Condition/When:
```sh 
cfg.ForMember("IsActive", o => o.Condition<User, UserDto>((src, dest) => src.Status == StatusEnum.Active));

```

NullSubstitute:
```sh 
cfg.ForMember("Email", o => o.NullSubstitute("default@email.com"));
```

ValueConverter:
```sh 
cfg.ForMember("Registered", o => o.ConvertUsing<DateTime?, string?>(dt => dt?.ToString("yyyy-MM-dd")));
```
Before/AfterMap:
 ```sh 
cfg.BeforeMap((src, dest) => dest.Name = src.Name?.ToUpper());
cfg.AfterMap((src, dest) => dest.IsActive = src.Status == StatusEnum.Active);

```

ReverseMap:
 ```sh 
cfg.ReverseMap((Mapper)mapper);
```

Paging Async:
 ```sh 
var page = await users.ProjectToPagedListAsync<User, UserDto>(mapper, 1, 10);
var pageDto = page.ToPagedDto();
```


## 🔥 Contoh MappingProfile
 ```sh 
using AIMapper;
using AIMapper.Profiles;

// MappingProfile untuk semua mapping User/UserDto
public class UserProfileMap : MapperProfile
{
    public override void Configure(IMapper mapper)
    {
        // Basic mapping + flattening
        mapper.Configure<User, UserDto>(cfg =>
        {
            cfg.ForMember("Address", o => o.CustomPath = "Profile.Address"); // flattening nested
            cfg.ForMember("Email", o => o.NullSubstitute("default@email.com")); // NullSubstitute
            cfg.ForMember("Registered", o => o.ConvertUsing<DateTime?, string?>(dt => dt?.ToString("yyyy-MM-dd"))); // ValueConverter
            cfg.ForMember("IsActive", o => o.Condition<User, UserDto>((src, dest) => src.Status == StatusEnum.Active)); // Condition/When
            cfg.ForMember("Name", o => o.Ignore()); // Ignore property
            cfg.BeforeMap((src, dest) => dest.Point = src.Point ?? 0); // BeforeMap
            cfg.AfterMap((src, dest) => dest.Name = src.Name?.ToUpper()); // AfterMap
            cfg.ReverseMap((Mapper)mapper); // Reverse mapping otomatis
        });

        // Bisa tambahkan mapping lain di profile yang sama!
        mapper.Configure<Order, OrderDto>(cfg =>
        {
            cfg.ForMember("CustomerName", o => o.CustomPath = "Customer.Name");
            cfg.ForMember("Total", o => o.NullSubstitute(0));
            cfg.AfterMap((src, dest) => dest.IsPaid = src.Status == OrderStatus.Paid);
            cfg.ReverseMap((Mapper)mapper);
        });
    }
}

// Contoh model lain
public class Order { public int Id { get; set; } public Customer? Customer { get; set; } public decimal? Total { get; set; } public OrderStatus Status { get; set; } }
public class Customer { public string? Name { get; set; } }
public class OrderDto { public int Id { get; set; } public string? CustomerName { get; set; } public decimal Total { get; set; } public bool IsPaid { get; set; } }
public enum OrderStatus { Pending, Paid }
```

Registrasi profile:
 ```sh 
// Di startup, cukup sekali
mapper.ApplyProfilesFromCurrentAssembly();
```


##  🧑‍💻 Sample Lengkap
Lihat file AIMapperSampleDemo.cs di folder AIMapper.SampleApp untuk contoh pemakaian semua fitur AIMapper.

