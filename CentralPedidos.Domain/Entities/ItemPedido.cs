using CentralPedidos.Domain.Exceptions;

namespace CentralPedidos.Domain.Entities
{
    public class ItemPedido
    {
        public int Id { get; private set; }
        public Guid PedidoId { get; private set; }
        public string ProdutoNome { get; private set; }
        public int Quantidade { get; private set; }
        public decimal PrecoUnitario { get; private set; }

        public Pedido Pedido { get; private set; }

        public ItemPedido(string produtoNome, int quantidade, decimal precoUnitario)
        {
            if (string.IsNullOrWhiteSpace(produtoNome))
            {
                throw new DomainException("O nome do produto é obrigatório.");
            }

            if (quantidade <= 0)
            {
                throw new DomainException("A quantidade do item deve ser maior que zero.");
            }

            ProdutoNome = produtoNome;
            Quantidade = quantidade;
            PrecoUnitario = precoUnitario;
        }

        protected ItemPedido() { }

        public void AtualizarQuantidade(int novaQuantidade)
        {
            if (novaQuantidade <= 0)
            {
                throw new DomainException("A quantidade deve ser maior que zero.");
            }

            Quantidade = novaQuantidade;
        }
    }
}
