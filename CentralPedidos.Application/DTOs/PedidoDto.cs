namespace CentralPedidos.Application.DTOs
{
    public record class PedidoDto
    {
        public Guid Id { get; set; }
        public string ClienteNome { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal ValorTotal { get; set; }
        public List<ItemPedidoDto> Itens { get; set; } = [];
    }
}
