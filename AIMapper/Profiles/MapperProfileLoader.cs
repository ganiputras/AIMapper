using System.Reflection;

namespace AIMapper.Profiles;

public static class MapperProfileLoader
{
    public static void ApplyProfilesFromAssembly(this IMapper mapper, Assembly assembly)
    {
        var profiles = assembly.GetTypes()
            .Where(t => typeof(MapperProfile).IsAssignableFrom(t) && !t.IsAbstract)
            .Select(t => (MapperProfile)Activator.CreateInstance(t)!);

        foreach (var profile in profiles)
            profile.Configure(mapper);
    }

    public static void ApplyProfilesFromCurrentAssembly(this IMapper mapper)
    {
        mapper.ApplyProfilesFromAssembly(Assembly.GetExecutingAssembly());
    }
}