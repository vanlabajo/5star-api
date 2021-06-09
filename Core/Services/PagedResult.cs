using System.Collections.Generic;

namespace Core.Services
{
    public class PagedResult<T>
    {
        public PagedResult() { }

        public PagedResult(List<T> data, int collectionSize)
        {
            Data = data;
            CollectionSize = collectionSize;
        }

        public List<T> Data { get; set; }

        /// <summary>
        /// Total data count without pagination
        /// </summary>
        public int CollectionSize { get; set; }
    }
}
