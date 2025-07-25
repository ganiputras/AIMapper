namespace AIMapper.Extensions;

/// <summary>
///     Hasil paging untuk data yang di-mapping, berisi item hasil, total count, dan info paging.
/// </summary>
/// <typeparam name="T">Tipe data hasil mapping.</typeparam>
public class PagedListResult<T>
{
    /// <summary>
    ///     Membuat hasil paging baru.
    /// </summary>
    /// <param name="items">Data item hasil mapping pada halaman ini.</param>
    /// <param name="totalCount">Total seluruh item (tanpa paging).</param>
    /// <param name="page">Halaman aktif (mulai dari 1).</param>
    /// <param name="pageSize">Jumlah item per halaman.</param>
    public PagedListResult(List<T> items, int totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }

    /// <summary>
    ///     List item hasil mapping pada halaman ini.
    /// </summary>
    public List<T> Items { get; }

    /// <summary>
    ///     Total item sebelum paging (seluruh data).
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    ///     Halaman aktif (1-based).
    /// </summary>
    public int Page { get; }

    /// <summary>
    ///     Jumlah item per halaman.
    /// </summary>
    public int PageSize { get; }
}