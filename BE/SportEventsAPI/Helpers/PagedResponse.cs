using SportEventsAPI.Models;
using System.Linq;

namespace SportEventsAPI.Helpers
{
    public static class PaginationHelper
    {
        public static PagedResponse<T> CreatePagedResponse<T>(IQueryable<T> query, int pageNumber, int pageSize)
        {
            var totalRecords = query.Count();
            var data = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedResponse<T>(data, pageNumber, pageSize, totalRecords);
        }
    }
}
