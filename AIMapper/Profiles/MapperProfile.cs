using AIMapper.Core;

namespace AIMapper.Profiles;

/// <summary>
///     Kelas dasar untuk grouping konfigurasi mapping.
///     Turunkan class ini untuk mengelompokkan konfigurasi mapping pada satu tempat.
/// </summary>
public abstract class MapperProfile
{
    /// <summary>
    ///     Method untuk mendaftarkan konfigurasi mapping ke IMapper.
    ///     Implementasi harus dipenuhi pada turunan class.
    /// </summary>
    /// <param name="mapper">Instance IMapper yang akan dikonfigurasi.</param>
    public abstract void Configure(IMapper mapper);
}