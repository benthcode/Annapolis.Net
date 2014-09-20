using System.Collections.Generic;

namespace Annapolis.Shared.Model
{
    public interface IPagerable
    {
        int PageIndex { get; }
        int PageSize { get; }
        int ActualPageSize { get; }
        int TotalCount { get; }
        int TotalPages { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
    }

    public interface IPagerable<T> : IPagerable, IList<T>
    {
     
    }
}
