using System.Reflection;

namespace AIMapper.Profiles;

/// <summary>
/// Utility class untuk mendaftarkan seluruh MapperProfile dalam satu assembly.
/// </summary>
public static class MapperProfileLoader
{
    /// <summary>
    /// Menerapkan semua profile mapping yang ditemukan pada sebuah assembly ke instance IMapper.
    /// </summary>
    /// <param name="mapper">Instance IMapper yang akan dikonfigurasi.</param>
    /// <param name="assembly">Assembly tempat mencari class turunan MapperProfile.</param>
    public static void ApplyProfilesFromAssembly(this IMapper mapper, Assembly assembly)
    {
        var profiles = assembly.GetTypes()
            .Where(t => typeof(MapperProfile).IsAssignableFrom(t) && !t.IsAbstract)
            .Select(t => (MapperProfile)Activator.CreateInstance(t)!);

        foreach (var profile in profiles)
        {
            // Apply each profile configuration
            profile.Configure(mapper);
        }
    }

    /// <summary>
    /// Menerapkan semua MapperProfile yang ada di assembly saat ini ke instance IMapper.
    /// </summary>
    /// <param name="mapper">Instance IMapper yang akan dikonfigurasi.</param>
    public static void ApplyProfilesFromCurrentAssembly(this IMapper mapper)
    {
        mapper.ApplyProfilesFromAssembly(Assembly.GetExecutingAssembly());
    }
}