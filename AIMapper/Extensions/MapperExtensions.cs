namespace AIMapper.Extensions;

public static class MapperExtensions
{
    /// <summary>
    ///     Mapping koleksi ke tipe tujuan.
    /// </summary>
    public static IEnumerable<TDestination> ProjectTo<TSource, TDestination>(
        this IEnumerable<TSource> source,
        IMapper mapper)
    {
        foreach (var item in source)
            yield return mapper.Map<TSource, TDestination>(item);
    }

    /// <summary>
    ///     Mapping koleksi jika kondisi terpenuhi.
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

    /// <summary>
    ///     Mapping koleksi dengan filter predicate.
    /// </summary>
    public static IEnumerable<TDestination> MapWhere<TSource, TDestination>(
        this IEnumerable<TSource> source,
        IMapper mapper,
        Func<TSource, bool> predicate)
    {
        foreach (var item in source.Where(predicate))
            yield return mapper.Map<TSource, TDestination>(item);
    }
}