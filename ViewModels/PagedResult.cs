using System.Collections.Generic;

namespace LixoZero.ViewModels
{
    public class PagedResult<T>
    {
        public required IReadOnlyList<T> Items { get; init; }
        public int TotalItems { get; init; }
        public int Page { get; init; }
        public int PageSize { get; init; }
    }
}
