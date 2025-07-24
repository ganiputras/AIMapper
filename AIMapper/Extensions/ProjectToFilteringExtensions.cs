namespace AIMapper.Extensions;

public static class ProjectToFilteringExtensions
{
    /// <summary>
    ///     Mapping koleksi dengan kondisi filter (where).
    /// </summary>
    public static IEnumerable<TDestination> MapWhere<TSource, TDestination>(
        this IEnumerable<TSource> source,
        IMapper mapper,
        Func<TSource, bool> predicate)
    {
        foreach (var item in source.Where(predicate)) yield return mapper.Map<TSource, TDestination>(item);
    }

    /// <summary>
    ///     Mapping koleksi jika kondisi global terpenuhi.
    /// </summary>
    public static IEnumerable<TDestination> MapIf<TSource, TDestination>(
        this IEnumerable<TSource> source,
        IMapper mapper,
        bool condition)
    {
        return condition
            ? source.ProjectTo<TSource, TDestination>(mapper)
            : Enumerable.Empty<TDestination>();
    }
}