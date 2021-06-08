using System.Collections.Generic;

namespace Core.Services
{
    public class PagedResult<T>
    {
        public PagedResult() { this.PageSize = 1; } // default to 1 to avoid divide by zero

        public PagedResult(List<T> Data, long Total, int PageSize)
        {
            this.Data = Data;
            this.Total = Total;
            this.PageSize = PageSize;
        }

        public List<T> Data { get; set; }

        /// <summary>
        /// Total data count, in other words Data.Count
        /// </summary>
        public long Total { get; set; }

        public int PageSize { get; set; }

        public long PageCount
        {
            get
            {
                if (this.Total <= this.PageSize) { return 1; }
                return ((this.Total - 1) / this.PageSize) + 1;
            }
        }
    }
}
