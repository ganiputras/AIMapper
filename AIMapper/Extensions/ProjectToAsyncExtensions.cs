using AIMapper.Core;
using Microsoft.EntityFrameworkCore;

namespace AIMapper.Extensions;

/// <summary>
///     Extension method untuk mapping koleksi dengan hasil asynchronous (array, first, dsb).
/// </summary>
public static class ProjectToAsyncExtensions
{
    /// <summary>
    ///     Mapping IQueryable ke array hasil mapping secara async.
    /// </summary>
    public static async Task<TDestination[]> ProjectToArrayAsync<TSource, TDestination>(
        this IQueryable<TSource> query,
        IMapper mapper)
    {
        var list = await query.ToListAsync();
        return list.Select(item => mapper.Map<TSource, TDestination>(item)).ToArray();
    }

    /// <summary>
    ///     Mapping IEnumerable ke array hasil mapping secara async.
    /// </summary>
    public static Task<TDestination[]> ProjectToArrayAsync<TSource, TDestination>(
        this IEnumerable<TSource> source,
        IMapper mapper)
    {
        var arr = source.Select(item => mapper.Map<TSource, TDestination>(item)).ToArray();
        return Task.FromResult(arr);
    }

    /// <summary>
    ///     Mapping IQueryable, ambil satu hasil pertama (atau null) secara async.
    /// </summary>
    public static async Task<TDestination?> ProjectToFirstOrDefaultAsync<TSource, TDestination>(
        this IQueryable<TSource> query,
        IMapper mapper)
    {
        var src = await query.FirstOrDefaultAsync();
        return src == null ? default : mapper.Map<TSource, TDestination>(src);
    }

    /// <summary>
    ///     Mapping IEnumerable, ambil satu hasil pertama (atau null) secara async.
    /// </summary>
    public static Task<TDestination?> ProjectToFirstOrDefaultAsync<TSource, TDestination>(
        this IEnumerable<TSource> source,
        IMapper mapper)
    {
        var src = source.FirstOrDefault();
        var result = src == null ? default : mapper.Map<TSource, TDestination>(src);
        return Task.FromResult(result);
    }
}