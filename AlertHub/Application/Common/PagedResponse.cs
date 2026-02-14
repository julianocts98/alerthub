namespace AlertHub.Application.Common;

/// <summary>
/// Base class for cursor-paginated responses.
/// <see cref="NextCursor"/> is null when there are no further pages.
/// </summary>
public abstract class PagedResponse<T>
{
    public IReadOnlyList<T> Items { get; protected init; } = [];

    public string? NextCursor { get; protected init; }

    public bool HasNextPage => NextCursor is not null;
}
