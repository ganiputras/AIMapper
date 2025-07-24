using AIMapper.Core;
using AIMapper.Extensions;
using AIMapper.SampleApp.Models;

namespace AIMapper.SampleApp;

public static class AIMapperSampleDemo
{
    public static async Task RunAllDemo(IMapper mapper)
    {
        // Dummy data
        var users = new List<User>
        {
            new User { Id = 1, Name = "Ali", Email = null, RegisteredAt = DateTime.Now, Profile = new UserProfile { Address = "Jalan Mawar", City = "Jakarta" }, Status = StatusEnum.Active, Point = null },
            new User { Id = 2, Name = "Budi", Email = "budi@email.com", RegisteredAt = null, Profile = null, Status = StatusEnum.Inactive, Point = 10 }
        };

        // HANYA SEKALI konfig: Semua fitur langsung diaktifkan di sini!
        mapper.Configure<User, UserDto>(cfg =>
        {
            cfg.ForMember("Email", o => o.Ignore()); // 2. Ignore property
            cfg.ForMember("Address", o => o.CustomPath = "Profile.Address"); // 3. CustomPath (flattening)
            cfg.ForMember("IsActive", o => o.Condition<User, UserDto>((src, dest) => src.Status == StatusEnum.Active)); // 4. Condition/When
            cfg.ForMember("Email", o => o.NullSubstitute("no-email@default.com")); // 5. NullSubstitute
            cfg.ForMember("Point", o => o.NullSubstitute(0)); // 5. NullSubstitute
            cfg.ForMember("Registered", o => o.ConvertUsing<DateTime?, string?>(dt => dt?.ToString("yyyy-MM-dd"))); // 6. ValueConverter
            cfg.ForMember("Status", o => o.ConvertUsing<StatusEnum, int>(e => (int)e)); // 6. ValueConverter
            cfg.BeforeMap((src, dest) => dest.Name = src.Name?.ToUpper()); // 7. BeforeMap
            cfg.AfterMap((src, dest) => dest.IsActive = src.Status == StatusEnum.Active); // 7. AfterMap
            cfg.ForMember("Name", o => o.NullSubstitute("Anonymous")); // 8. ReverseMap juga support NullSubstitute
            cfg.ReverseMap((Mapper)mapper); // 8. ReverseMap
        });

        // 1. Mapping otomatis by name (dan hasil mapping lain juga)
        var dto1 = mapper.Map<User, UserDto>(users[0]);
        Console.WriteLine($"1. Mapping otomatis: {dto1.Name}");

        // 2. Email di-ignore (lihat di config, Email=ignore + NullSubstitute)
        var dto2 = mapper.Map<User, UserDto>(users[0]);
        Console.WriteLine($"2. Email di-ignore: {dto2.Email}"); // Output: null

        // 3. CustomPath (override flattening/nested)
        var dto3 = mapper.Map<User, UserDto>(users[0]);
        Console.WriteLine($"3. Address via custom path: {dto3.Address}");

        // 4. Condition/When
        var dto4 = mapper.Map<User, UserDto>(users[0]);
        Console.WriteLine($"4. Condition/When: {dto4.IsActive}"); // true kalau Active

        // 5. NullSubstitute (lihat Email, Point)
        var dto5 = mapper.Map<User, UserDto>(users[0]);
        Console.WriteLine($"5. NullSubstitute: Email={dto5.Email}, Point={dto5.Point}");

        // 6. ValueConverter / ConvertUsing (Registered ke string, Status ke int)
        var dto6 = mapper.Map<User, UserDto>(users[0]);
        Console.WriteLine($"6. ValueConverter: Registered={dto6.Registered}, StatusInt={dto6.Status}");

        // 7. BeforeMap & AfterMap
        var dto7 = mapper.Map<User, UserDto>(users[0]);
        Console.WriteLine($"7. BeforeMap: Name={dto7.Name}, AfterMap IsActive={dto7.IsActive}");

        // 8. ReverseMap (mapping dua arah otomatis)
        var userBack = mapper.Map<UserDto, User>(new UserDto { Id = 10, Name = null });
        Console.WriteLine($"8. ReverseMap: User.Name={userBack.Name}");

        // 9. Mapping koleksi (ProjectTo)
        var dtoList = users.ProjectTo<User, UserDto>(mapper).ToList();
        Console.WriteLine($"9. ProjectTo List: {dtoList.Count}");

        // 10. MapIf & MapWhere
        var activeOnly = users.MapWhere<User, UserDto>(mapper, u => u.Status == StatusEnum.Active).ToList();
        Console.WriteLine($"10. MapWhere only active: {activeOnly.Count}");

        var resultIf = users.MapIf<User, UserDto>(mapper, condition: true).ToList();
        Console.WriteLine($"11. MapIf: {resultIf.Count}");

        // 11. Async ProjectToListAsync (IEnumerable/in-memory)
        var asyncDtos = await users.ProjectToListAsync<User, UserDto>(mapper);
        Console.WriteLine($"12. ProjectToListAsync (in-mem): {asyncDtos.Count}");

        // 12. Async ProjectToListAsync (IQueryable/EF Core)
        // Jika pakai EF Core: await db.Users.ProjectToListAsync<User, UserDto>(mapper);

        // 13. Paging async (in-memory)
        var paged = await users.ProjectToPagedListAsync<User, UserDto>(mapper, page: 1, pageSize: 1);
        Console.WriteLine($"13. Paging: data={paged.Items.Count}, total={paged.TotalCount}");

        // 14. Paging to DTO standar
        var pagedDto = paged.ToPagedDto();
        Console.WriteLine($"14. PagedDto: TotalPages={pagedDto.TotalPages}, HasNext={pagedDto.HasNext}");

        // 15. ProjectToArrayAsync, ProjectToFirstOrDefaultAsync
        var arr = await users.ProjectToArrayAsync<User, UserDto>(mapper);
        var first = await users.ProjectToFirstOrDefaultAsync<User, UserDto>(mapper);
        Console.WriteLine($"15. ToArrayAsync: {arr.Length}, FirstOrDefaultAsync: {first?.Name}");

        // 16. Filter async (ProjectToWhereAsync)
        var activeUsers = await users.ProjectToWhereAsync<User, UserDto>(mapper, u => u.Status == StatusEnum.Active);
        Console.WriteLine($"16. ProjectToWhereAsync: {activeUsers.Count}");
    }
}