namespace Core.Services
{
    public class PagedQuery
    {
        public string SearchTerm { get; set; }
        public PageInfo PageInfo { get; set; }

        public PagedQuery()
        {
            PageInfo = new PageInfo(1, 30);
        }

    }

    public class PageInfo
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Skip
        {
            get
            {
                return PageSize * (Page - 1);
            }
        }

        public PageInfo(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }
    }
}
