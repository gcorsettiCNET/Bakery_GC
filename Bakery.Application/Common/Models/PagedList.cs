namespace Bakery.Application.Common.Models;

/// <summary>
/// Paginated list implementation
/// </summary>
/// <typeparam name="T">Type of items in the list</typeparam>
public class PagedList<T>
{
    public IReadOnlyList<T> Items { get; }
    public int TotalCount { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage { get; }
    public bool HasNextPage { get; }

    public PagedList(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items ?? throw new ArgumentNullException(nameof(items));
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        HasPreviousPage = pageNumber > 1;
        HasNextPage = pageNumber < TotalPages;
    }

    public static PagedList<T> Empty(int pageNumber = 1, int pageSize = 10)
    {
        return new PagedList<T>(Array.Empty<T>(), 0, pageNumber, pageSize);
    }
}