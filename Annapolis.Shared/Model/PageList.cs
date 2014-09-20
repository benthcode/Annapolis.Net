using System.Collections.Generic;

namespace Annapolis.Shared.Model
{
    public class PageList<T> : List<T>, IPagerable<T>
    {

        public PageList(IList<T> source, int pageIndex, int pageSize, int total)
        {
            TotalCount = total;
            TotalPages = total / pageSize;
            if (total % pageSize > 0)
                TotalPages++;
            PageSize = pageSize;
            ActualPageSize = source.Count;
            PageIndex = pageIndex;
            AddRange(source);
        }

        protected PageList(){}
        
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int ActualPageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }

        public bool HasPreviousPage
        {
            get { return (PageIndex > 0); }
        }
        public bool HasNextPage
        {
            get { return (PageIndex + 1 < TotalPages); }
        }

    }

}
