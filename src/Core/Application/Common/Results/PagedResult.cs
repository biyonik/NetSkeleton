namespace Application.Common.Results;

/// <summary>
/// Sayfalı sonuçlar için result sınıfı
/// </summary>
public class PagedResult<T> : Result<IReadOnlyList<T>>
{
    public int TotalCount { get; }
    public int PageSize { get; }
    public int CurrentPage { get; }
    public int TotalPages { get; }
    public bool HasNextPage { get; }
    public bool HasPreviousPage { get; }

    private PagedResult(
        IReadOnlyList<T> items,
        int totalCount,
        int pageSize,
        int currentPage,
        bool isSuccess,
        Error error,
        string message)
        : base(items, isSuccess, error, message)
    {
        TotalCount = totalCount;
        PageSize = pageSize;
        CurrentPage = currentPage;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        HasNextPage = CurrentPage < TotalPages;
        HasPreviousPage = CurrentPage > 1;
    }

    public static PagedResult<T> Success(
        IReadOnlyList<T> items,
        int totalCount,
        int pageSize,
        int currentPage,
        string message = "") =>
        new(items, totalCount, pageSize, currentPage, true, Error.None, message);

    public new static PagedResult<T> Failure(Error error) =>
        new(Array.Empty<T>(), 0, 0, 0, false, error, error.Message);

    public new static PagedResult<T> Failure(string message) =>
        new(Array.Empty<T>(), 0, 0, 0, false, Error.Custom(message), message);
}