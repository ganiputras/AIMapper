namespace AIMapper.Extensions;

/// <summary>
/// Extension khusus untuk mapping koleksi dengan filtering atau kondisi global menggunakan AIMapper.
/// </summary>
public static class ProjectToFilteringExtensions
{
    /// <summary>
    /// Melakukan mapping koleksi dengan filter predicate (where).
    /// </summary>
    /// <typeparam name="TSource">Tipe source data.</typeparam>
    /// <typeparam name="TDestination">Tipe destination hasil mapping.</typeparam>
    /// <param name="source">Koleksi data sumber.</param>
    /// <param name="mapper">Instance IMapper yang digunakan untuk mapping.</param>
    /// <param name="predicate">Fungsi predicate untuk filter data.</param>
    /// <returns>Koleksi hasil mapping yang sudah difilter.</returns>
    public static IEnumerable<TDestination> MapWhere<TSource, TDestination>(
        this IEnumerable<TSource> source,
        IMapper mapper,
        Func<TSource, bool> predicate)
    {
        foreach (var item in source.Where(predicate))
            yield return mapper.Map<TSource, TDestination>(item);
    }

    /// <summary>
    /// Melakukan mapping koleksi hanya jika kondisi global terpenuhi.
    /// </summary>
    /// <typeparam name="TSource">Tipe source data.</typeparam>
    /// <typeparam name="TDestination">Tipe destination hasil mapping.</typeparam>
    /// <param name="source">Koleksi data sumber.</param>
    /// <param name="mapper">Instance IMapper yang digunakan untuk mapping.</param>
    /// <param name="condition">Kondisi global untuk eksekusi mapping.</param>
    /// <returns>Koleksi hasil mapping jika kondisi true, jika tidak akan menghasilkan koleksi kosong.</returns>
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

