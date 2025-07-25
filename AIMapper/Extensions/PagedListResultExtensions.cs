namespace AIMapper.Extensions;

/// <summary>
///     Extension untuk konversi PagedListResult ke DTO response API.
/// </summary>
public static class PagedListResultExtensions
{
    /// <summary>
    ///     Convert hasil paging internal ke DTO standar untuk API response (PagedDto).
    /// </summary>
    public static PagedDto<T> ToPagedDto<T>(this PagedListResult<T> paged)
    {
        return new PagedDto<T>
        {
            Data = paged.Items,
            TotalCount = paged.TotalCount,
            Page = paged.Page,
            PageSize = paged.PageSize
        };
    }
}