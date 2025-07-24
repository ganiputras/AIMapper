namespace AIMapper.Extensions;

/// <summary>
/// DTO standar untuk response hasil paging di API atau aplikasi frontend.
/// </summary>
/// <typeparam name="T">Tipe data item hasil paging.</typeparam>
public class PagedDto<T>
{
    /// <summary>
    /// Data hasil paging (list item di halaman aktif).
    /// </summary>
    public List<T> Data { get; set; } = new();

    /// <summary>
    /// Total seluruh item sebelum paging/filter.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Halaman aktif (mulai dari 1).
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Jumlah item per halaman.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Jumlah total halaman.
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

    /// <summary>
    /// True jika ada halaman berikutnya.
    /// </summary>
    public bool HasNext => Page < TotalPages;

    /// <summary>
    /// True jika ada halaman sebelumnya.
    /// </summary>
    public bool HasPrevious => Page > 1;
}