namespace CentralPedidos.Application.DTOs
{
    public record class CriarPedidoDto
    {
        public string ClienteNome { get; set; } = string.Empty;
        public List<CriarItemPedidoDto> Itens { get; set; } = [];
    }
}
