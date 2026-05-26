using CentralPedidos.Domain.Entities;
using CentralPedidos.Domain.Enums;
using CentralPedidos.Domain.Exceptions;

namespace CentralPedidos.UnitTests.Domain.Entities
{
    public class PedidoDomainTests
    {
        [Fact]
        public void CriarPedido_DeveInicializarComStatusNovoEValorZero()
        {
            // Arrange & Act
            var pedido = new Pedido("Cliente Wiz Co");

            // Assert
            Assert.NotEqual(Guid.Empty, pedido.Id);
            Assert.Equal("Cliente Wiz Co", pedido.ClienteNome);
            Assert.Equal(StatusPedido.Novo, pedido.Status);
            Assert.Equal(0, pedido.ValorTotal);
            Assert.Empty(pedido.Itens);
        }

        [Fact]
        public void AdicionarItem_DeveRecalcularOValorTotalDoPedidoCorretamente()
        {
            // Arrange
            var pedido = new Pedido("Cliente Teste");
            var item1 = new ItemPedido("Teclado", 1, 150.00m);
            var item2 = new ItemPedido("Mouse", 2, 50.00m); 

            // Act
            pedido.AdicionarItem(item1);
            pedido.AdicionarItem(item2);

            // Assert
            Assert.Equal(2, pedido.Itens.Count);
            Assert.Equal(250.00m, pedido.ValorTotal);
        }

        [Fact]
        public void Cancelar_QuandoStatusForNovo_DeveMudarParaCancelado()
        {
            // Arrange
            var pedido = new Pedido("Cliente Teste");

            // Act
            pedido.Cancelar();

            // Assert
            Assert.Equal(StatusPedido.Cancelado, pedido.Status);
        }

        [Fact]
        public void Cancelar_QuandoPedidoJaEstiverPago_DeveLancarDomainException()
        {
            // Arrange
            var pedido = new Pedido("Cliente Teste");

            var propriedadeStatus = typeof(Pedido).GetProperty(nameof(Pedido.Status));
            propriedadeStatus?.SetValue(pedido, StatusPedido.Pago);

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => pedido.Cancelar());
            Assert.Contains("Não é possível cancelar um pedido que já foi pago.", excecao.Message);
        }
    }
}
