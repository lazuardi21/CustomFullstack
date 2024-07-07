namespace SportEventsAPI.Models
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Data { get; set; }
        public Pagination Pagination { get; set; }
        public object Items { get; set; }

        public PagedResponse(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords)
        {
            Data = data;
            Pagination = new Pagination
            {
                TotalRecords = totalRecords,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
            };
        }
    }

    public class Pagination
    {
        public int TotalRecords { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
