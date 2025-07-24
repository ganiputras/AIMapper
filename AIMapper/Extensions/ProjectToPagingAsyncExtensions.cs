using AIMapper.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AIMapper.Extensions;

/// <summary>
/// Extension method untuk mapping koleksi secara async: paging, count, dan filter.
/// </summary>
public static class ProjectToPagingAsyncExtensions
{
    /// <summary>
    /// Paging dan mapping IQueryable ke PagedList (List hasil dan total count) secara async.
    /// </summary>
    public static async Task<PagedListResult<TDestination>> ProjectToPagedListAsync<TSource, TDestination>(
        this IQueryable<TSource> query,
        IMapper mapper,
        int page,
        int pageSize)
    {
        var totalCount = await query.CountAsync();
        var data = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        var mapped = data.Select(item => mapper.Map<TSource, TDestination>(item)).ToList();
        return new PagedListResult<TDestination>(mapped, totalCount, page, pageSize);
    }

    /// <summary>
    /// Paging dan mapping IEnumerable ke PagedList (in-memory, tanpa async DB).
    /// </summary>
    public static Task<PagedListResult<TDestination>> ProjectToPagedListAsync<TSource, TDestination>(
        this IEnumerable<TSource> source,
        IMapper mapper,
        int page,
        int pageSize)
    {
        var totalCount = source.Count();
        var data = source.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(item => mapper.Map<TSource, TDestination>(item)).ToList();
        var result = new PagedListResult<TDestination>(data, totalCount, page, pageSize);
        return Task.FromResult(result);
    }

    /// <summary>
    /// Menghitung jumlah hasil mapping (count) secara async dari IQueryable.
    /// </summary>
    public static Task<int> ProjectToCountAsync<TSource, TDestination>(
        this IQueryable<TSource> query,
        IMapper mapper)
    {
        // Mapper diabaikan karena hanya count, tidak mapping property
        return query.CountAsync();
    }

    /// <summary>
    /// Menghitung jumlah hasil mapping (count) secara async dari IEnumerable.
    /// </summary>
    public static Task<int> ProjectToCountAsync<TSource, TDestination>(
        this IEnumerable<TSource> source,
        IMapper mapper)
    {
        // Mapper diabaikan karena hanya count, tidak mapping property
        return Task.FromResult(source.Count());
    }

    /// <summary>
    /// Mapping IQueryable dengan filter predicate expression (async, tetap IQueryable).
    /// </summary>
    public static async Task<List<TDestination>> ProjectToWhereAsync<TSource, TDestination>(
        this IQueryable<TSource> query,
        IMapper mapper,
        Expression<Func<TSource, bool>> predicate)
    {
        var data = await query.Where(predicate).ToListAsync(); // Tetap IQueryable, bisa async!
        return data.Select(item => mapper.Map<TSource, TDestination>(item)).ToList();
    }

    /// <summary>
    /// Mapping IEnumerable dengan filter predicate (in-memory).
    /// </summary>
    public static Task<List<TDestination>> ProjectToWhereAsync<TSource, TDestination>(
        this IEnumerable<TSource> source,
        IMapper mapper,
        Func<TSource, bool> predicate)
    {
        var data = source.Where(predicate)
            .Select(item => mapper.Map<TSource, TDestination>(item)).ToList();
        return Task.FromResult(data);
    }
}

