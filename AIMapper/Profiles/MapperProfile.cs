namespace AIMapper.Profiles;

/// <summary>
///     Base class untuk grouping konfigurasi mapping.
/// </summary>
public abstract class MapperProfile
{
    public abstract void Configure(IMapper mapper);
}