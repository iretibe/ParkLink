namespace ParkLink.Gate.Dtos
{
    public sealed record PaginatedResult<T>(
        IReadOnlyCollection<T> Items,
        int Page,
        int PageSize,
        int TotalCount,
        int TotalPages
    );
}
