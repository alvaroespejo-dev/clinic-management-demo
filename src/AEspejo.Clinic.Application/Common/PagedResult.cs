namespace AEspejo.Clinic.Application.Common;

/// <summary>Pagination and search parameters for list endpoints.</summary>
public class PagedRequest
{
    private const int MaxPageSize = 200;
    private int _pageSize = 20;

    public int Page { get; set; } = 1;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value is < 1 or > MaxPageSize ? 20 : value;
    }

    /// <summary>Free-text search term (optional; each service decides which fields it applies to).</summary>
    public string? Search { get; set; }

    /// <summary>Include inactive (soft-deleted) records. Only active ones by default.</summary>
    public bool IncludeInactive { get; set; }
}

/// <summary>A page of results from a list endpoint.</summary>
public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
