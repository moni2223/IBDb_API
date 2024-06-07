namespace IBDb.Dto
{
    public class PaginationDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool noPagination { get; set; } = false;   
    }
    public class PaginatedResponse<T>
    {
        public IEnumerable<T> Docs { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
        public bool HasNextPage { get; set; }
    }
}
