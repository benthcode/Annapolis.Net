using System.Web.UI;
using Annapolis.Shared.Model;
using PagedList;

namespace Annapolis.Web.Client
{
    public class PageListClient<T> : ModelListClient<T>
        where T : ClientModel
    {
        static PageListClient()
        {
            RegisterModelTargetClassName(typeof(PageListClient<T>), "PageList");
        }

        public PageListClient()
        { }


        public PageListClient(IPagedList page, int actualSizeOnPage)
        {
         
            PageNumber = page.PageNumber;
            PageSize = page.PageSize;
            TotalCount = page.TotalItemCount;
            TotalPages = page.PageCount;
            HasPreviousPage = page.HasPreviousPage;
            HasNextPage = page.HasNextPage;

            _actualSizeOnPage = actualSizeOnPage;
        }

        private int _actualSizeOnPage;

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get;  set; }
        public bool HasPreviousPage {get; set;}
        public bool HasNextPage { get; set; }
        public int ActualSizeOnPage 
         {
            get { return _actualSizeOnPage; } 
            set { _actualSizeOnPage = value; }
        }
    }

}