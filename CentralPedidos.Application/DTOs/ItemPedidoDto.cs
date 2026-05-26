namespace CentralPedidos.Application.DTOs
{
    public record class ItemPedidoDto
    {
        public int Id { get; set; }
        public string ProdutoNome { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}
