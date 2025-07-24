using AIMapper.Core;

namespace AIMapper.Extensions;

/// <summary>
/// Kumpulan extension method untuk proses mapping koleksi menggunakan AIMapper.
/// </summary>
public static class MapperExtensions
{
    /// <summary>
    /// Melakukan mapping koleksi ke tipe tujuan menggunakan IMapper.
    /// </summary>
    /// <typeparam name="TSource">Tipe source data.</typeparam>
    /// <typeparam name="TDestination">Tipe destination hasil mapping.</typeparam>
    /// <param name="source">Koleksi data sumber.</param>
    /// <param name="mapper">Instance IMapper yang digunakan untuk mapping.</param>
    /// <returns>Koleksi hasil mapping.</returns>
    public static IEnumerable<TDestination> ProjectTo<TSource, TDestination>(
        this IEnumerable<TSource> source,
        IMapper mapper)
    {
        foreach (var item in source)
            yield return mapper.Map<TSource, TDestination>(item);
    }

    /// <summary>
    /// Melakukan mapping koleksi hanya jika kondisi terpenuhi.
    /// </summary>
    /// <typeparam name="TSource">Tipe source data.</typeparam>
    /// <typeparam name="TDestination">Tipe destination hasil mapping.</typeparam>
    /// <param name="source">Koleksi data sumber.</param>
    /// <param name="mapper">Instance IMapper yang digunakan untuk mapping.</param>
    /// <param name="condition">Kondisi boolean yang menentukan apakah mapping dijalankan.</param>
    /// <returns>Koleksi hasil mapping jika kondisi terpenuhi, jika tidak akan menghasilkan koleksi kosong.</returns>
    public static IEnumerable<TDestination> MapIf<TSource, TDestination>(
        this IEnumerable<TSource> source,
        IMapper mapper,
        bool condition)
    {
        // Hanya mapping jika condition == true
        return condition
            ? source.ProjectTo<TSource, TDestination>(mapper)
            : Enumerable.Empty<TDestination>();
    }

    /// <summary>
    /// Melakukan mapping koleksi dengan filter berdasarkan predicate.
    /// </summary>
    /// <typeparam name="TSource">Tipe source data.</typeparam>
    /// <typeparam name="TDestination">Tipe destination hasil mapping.</typeparam>
    /// <param name="source">Koleksi data sumber.</param>
    /// <param name="mapper">Instance IMapper yang digunakan untuk mapping.</param>
    /// <param name="predicate">Fungsi filter predicate.</param>
    /// <returns>Koleksi hasil mapping setelah difilter.</returns>
    public static IEnumerable<TDestination> MapWhere<TSource, TDestination>(
        this IEnumerable<TSource> source,
        IMapper mapper,
        Func<TSource, bool> predicate)
    {
        foreach (var item in source.Where(predicate))
            yield return mapper.Map<TSource, TDestination>(item);
    }



}
