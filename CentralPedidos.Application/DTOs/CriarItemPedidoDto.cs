namespace CentralPedidos.Application.DTOs
{
    public record class CriarItemPedidoDto
    {
        public string ProdutoNome { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}
