using Microsoft.EntityFrameworkCore;

namespace AIMapper.Extensions;

/// <summary>
/// Extension method untuk melakukan mapping koleksi ke List hasil mapping secara asynchronous.
/// </summary>
public static class ProjectToListAsyncExtensions
{
    /// <summary>
    /// Melakukan mapping koleksi IQueryable ke tipe tujuan dan mengembalikan hasil sebagai List secara asynchronous.
    /// Cocok untuk penggunaan dengan EF Core (database).
    /// </summary>
    /// <typeparam name="TSource">Tipe data source (entity).</typeparam>
    /// <typeparam name="TDestination">Tipe data tujuan (DTO/model).</typeparam>
    /// <param name="query">Koleksi IQueryable dari database.</param>
    /// <param name="mapper">Instance IMapper yang digunakan untuk mapping.</param>
    /// <returns>List hasil mapping, didapat secara async dari database.</returns>
    public static async Task<List<TDestination>> ProjectToListAsync<TSource, TDestination>(
        this IQueryable<TSource> query,
        IMapper mapper)
    {
        var list = await query.ToListAsync(); // Ambil dari DB secara async
        return list.Select(item => mapper.Map<TSource, TDestination>(item)).ToList();
    }

    /// <summary>
    /// Melakukan mapping koleksi IEnumerable ke tipe tujuan dan mengembalikan hasil sebagai List secara asynchronous.
    /// Untuk source yang sudah in-memory, extension tetap async agar simetris dengan penggunaan IQueryable.
    /// </summary>
    /// <typeparam name="TSource">Tipe data source.</typeparam>
    /// <typeparam name="TDestination">Tipe data tujuan.</typeparam>
    /// <param name="source">Koleksi data sumber (in-memory).</param>
    /// <param name="mapper">Instance IMapper yang digunakan untuk mapping.</param>
    /// <returns>List hasil mapping sebagai Task.</returns>
    public static Task<List<TDestination>> ProjectToListAsync<TSource, TDestination>(
        this IEnumerable<TSource> source,
        IMapper mapper)
    {
        var list = source.Select(item => mapper.Map<TSource, TDestination>(item)).ToList();
        return Task.FromResult(list); // Supaya bisa await, konsisten dengan IQueryable
    }
}