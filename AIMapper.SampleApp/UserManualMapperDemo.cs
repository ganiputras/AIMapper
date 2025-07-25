using AIMapper.Core;
using AIMapper.Extensions;
using AIMapper.SampleApp.Models;

namespace AIMapper.SampleApp;

/// <summary>
/// Demo pemetaan object User ke UserDto menggunakan konfigurasi manual (runtime) via IMapper.Configure.
/// Fitur-fitur yang dites:
/// 1. Auto-matching by name
/// 2. Ignore property
/// 3. CustomPath (flattening override)
/// 4. Condition / When
/// 5. NullSubstitute
/// 6. ValueConverter (ConvertUsing)
/// 7. BeforeMap & AfterMap
/// 8. ReverseMap
/// 9. Mapping koleksi (ProjectTo)
/// 10. MapWhere & MapIf
/// 11. Async: ProjectToListAsync, ToArrayAsync, FirstOrDefaultAsync
/// 12. Paging + ToPagedDto
/// </summary>
public static class UserManualMapperDemo
{
    /// <summary>
    /// Menjalankan semua contoh mapping manual User → UserDto menggunakan konfigurasi lengkap.
    /// </summary>
    public static async Task Run(IMapper mapper)
    {
        Console.WriteLine("🧠 Demo Manual Mapper via IMapper.Configure\n");

        var users = new List<User>
        {
            new()
            {
                Id = 1,
                Name = "Ali",
                Email = null,
                RegisteredAt = DateTime.Now,
                Profile = new UserProfile { Address = "Jalan Mawar", City = "Jakarta" },
                Status = StatusEnum.Active,
                Point = null
            },
            new()
            {
                Id = 2,
                Name = "Budi",
                Email = "budi@email.com",
                RegisteredAt = null,
                Profile = null,
                Status = StatusEnum.Inactive,
                Point = 10
            }
        };

        // ✅ Konfigurasi semua fitur manual mapping di sini
        mapper.Configure<User, UserDto>(cfg =>
        {
            cfg.ForMember("Email", o => o.Ignore()); // 2. Ignore property
            cfg.ForMember("Address", o => o.CustomPath = "Profile.Address"); // 3. Flattening via custom path
            cfg.ForMember("IsActive", o => o.Condition<User, UserDto>((src, dest) => src.Status == StatusEnum.Active)); // 4. Condition/When
            cfg.ForMember("Email", o => o.NullSubstitute("no-email@default.com")); // 5. NullSubstitute
            cfg.ForMember("Point", o => o.NullSubstitute(0)); // 5. NullSubstitute
            cfg.ForMember("Registered", o => o.ConvertUsing<DateTime?, string?>(dt => dt?.ToString("yyyy-MM-dd"))); // 6. ConvertUsing
            cfg.ForMember("Status", o => o.ConvertUsing<StatusEnum, int>(e => (int)e)); // 6. ConvertUsing
            cfg.BeforeMap((src, dest) => dest.Name = src.Name?.ToUpper()); // 7. BeforeMap
            cfg.AfterMap((src, dest) => dest.IsActive = src.Status == StatusEnum.Active); // 7. AfterMap
            cfg.ForMember("Name", o => o.NullSubstitute("Anonymous")); // 5. NullSubstitute (juga valid di reverse)
            cfg.ReverseMap((Mapper)mapper); // 8. ReverseMap
        });

        // 1. Auto-matching
        var dto1 = mapper.Map<User, UserDto>(users[0]);
        Console.WriteLine($"1. Mapping otomatis: {dto1.Name}");

        // 2. Ignore property
        var dto2 = mapper.Map<User, UserDto>(users[0]);
        Console.WriteLine($"2. Email di-ignore: {dto2.Email}");

        // 3. CustomPath (flattening)
        var dto3 = mapper.Map<User, UserDto>(users[0]);
        Console.WriteLine($"3. Address via custom path: {dto3.Address}");

        // 4. Condition/When
        var dto4 = mapper.Map<User, UserDto>(users[0]);
        Console.WriteLine($"4. Condition/When: {dto4.IsActive}");

        // 5. NullSubstitute
        var dto5 = mapper.Map<User, UserDto>(users[0]);
        Console.WriteLine($"5. NullSubstitute: Email={dto5.Email}, Point={dto5.Point}");

        // 6. ValueConverter
        var dto6 = mapper.Map<User, UserDto>(users[0]);
        Console.WriteLine($"6. ValueConverter: Registered={dto6.Registered}, StatusInt={dto6.Status}");

        // 7. BeforeMap & AfterMap
        var dto7 = mapper.Map<User, UserDto>(users[0]);
        Console.WriteLine($"7. BeforeMap: Name={dto7.Name}, AfterMap IsActive={dto7.IsActive}");

        // 8. ReverseMap
        var userBack = mapper.Map<UserDto, User>(new UserDto { Id = 10, Name = null });
        Console.WriteLine($"8. ReverseMap: User.Name={userBack.Name}");

        // 9. ProjectTo List
        var dtoList = users.ProjectTo<User, UserDto>(mapper).ToList();
        Console.WriteLine($"9. ProjectTo List: {dtoList.Count}");

        // 10. MapWhere
        var activeOnly = users.MapWhere<User, UserDto>(mapper, u => u.Status == StatusEnum.Active).ToList();
        Console.WriteLine($"10. MapWhere only active: {activeOnly.Count}");

        // 11. MapIf
        var resultIf = users.MapIf<User, UserDto>(mapper, true).ToList();
        Console.WriteLine($"11. MapIf: {resultIf.Count}");

        // 12. ProjectToListAsync (in-memory)
        var asyncDtos = await users.ProjectToListAsync<User, UserDto>(mapper);
        Console.WriteLine($"12. ProjectToListAsync (in-mem): {asyncDtos.Count}");

        // 13. Paging async
        var paged = await users.ProjectToPagedListAsync<User, UserDto>(mapper, 1, 1);
        Console.WriteLine($"13. Paging: data={paged.Items.Count}, total={paged.TotalCount}");

        // 14. PagedDto conversion
        var pagedDto = paged.ToPagedDto();
        Console.WriteLine($"14. PagedDto: TotalPages={pagedDto.TotalPages}, HasNext={pagedDto.HasNext}");

        // 15. ToArrayAsync & FirstOrDefaultAsync
        var arr = await users.ProjectToArrayAsync<User, UserDto>(mapper);
        var first = await users.ProjectToFirstOrDefaultAsync<User, UserDto>(mapper);
        Console.WriteLine($"15. ToArrayAsync: {arr.Length}, FirstOrDefaultAsync: {first?.Name}");

        // 16. ProjectToWhereAsync
        var activeUsers = await users.ProjectToWhereAsync<User, UserDto>(mapper, u => u.Status == StatusEnum.Active);
        Console.WriteLine($"16. ProjectToWhereAsync: {activeUsers.Count}");
    }
}
