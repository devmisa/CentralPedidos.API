namespace CentralPedidos.Application.DTOs
{
    public class PagedResult<T>(IEnumerable<T> pedidos, int count, int pageNumber, int pageSize)
    {
        public IEnumerable<T> Pedidos { get; set; } = pedidos;
        public int CurrentPage { get; set; } = pageNumber;
        public int TotalPages { get; set; } = (int)Math.Ceiling(count / (double)pageSize);
        public int PageSize { get; set; } = pageSize;
        public int TotalCount { get; set; } = count;
    }
}
