using AutoMapper;
using CentralPedidos.Application.DTOs;
using CentralPedidos.Application.Services;
using CentralPedidos.Domain.Entities;
using CentralPedidos.Domain.Enums;
using CentralPedidos.Domain.Exceptions;
using CentralPedidos.Infrastructure.Interfaces;
using Moq;
using Serilog;

namespace CentralPedidos.UnitTests.Application.Service
{
    public class PedidoServiceTests
    {
        private readonly Mock<IPedidoRepository> _repoMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly PedidoService _service;

        public PedidoServiceTests()
        {
            _repoMock = new Mock<IPedidoRepository>();
            _loggerMock = new Mock<ILogger>();
            _mapperMock = new Mock<IMapper>();

            // Instancia a Service injetando os Mocks (usando o construtor primário do .NET 8+)
            _service = new PedidoService(_repoMock.Object, _loggerMock.Object, _mapperMock.Object);
        }

        #region Cenários do CriarPedidoAsync

        [Fact]
        public async Task CriarPedidoAsync_ComDadosValidos_DeveRetornarPedidoDtoComSucesso()
        {
            // Arrange (Preparação)
            var criarPedidoDto = new CriarPedidoDto
            {
                ClienteNome = "Fulano da Wiz",
                Itens = [new() { ProdutoNome = "Mouse", Quantidade = 1, PrecoUnitario = 100.00m }]
            };

            var pedidoFake = new Pedido(criarPedidoDto.ClienteNome);
            var itemFake = new ItemPedido("Mouse", 1, 100.00m);
            var pedidoDtoEsperado = new PedidoDto { Id = pedidoFake.Id, ClienteNome = "Fulano da Wiz", Status = "Novo" };

            _mapperMock.Setup(m => m.Map<Pedido>(criarPedidoDto)).Returns(pedidoFake);
            _mapperMock.Setup(m => m.Map<ItemPedido>(It.IsAny<CriarItemPedidoDto>())).Returns(itemFake);
            _repoMock.Setup(r => r.AdicionarAsync(It.IsAny<Pedido>())).ReturnsAsync(pedidoFake);
            _mapperMock.Setup(m => m.Map<PedidoDto>(pedidoFake)).Returns(pedidoDtoEsperado);

            // Act (Ação)
            var resultado = await _service.CriarPedidoAsync(criarPedidoDto);

            // Assert (Verificação)
            Assert.NotNull(resultado);
            Assert.Equal(pedidoFake.Id, resultado.Id);
            Assert.Equal(criarPedidoDto.ClienteNome, resultado.ClienteNome);
            _repoMock.Verify(r => r.AdicionarAsync(It.IsAny<Pedido>()), Times.Once);
        }

        #endregion

        #region Cenários do ObterTodosPaginadoAsync

        [Fact]
        public async Task ObterTodosPaginadoAsync_DeveRetornarPagedResultCorreto()
        {
            // Arrange
            var pageParams = new PageParams { PageNumber = 1, PageSize = 10 };
            var pedidosFake = new List<Pedido> { new("Cliente 1"), new("Cliente 2") };
            var dtosFake = new List<PedidoDto> { new() { ClienteNome = "Cliente 1" }, new() { ClienteNome = "Cliente 2" } };

            _repoMock.Setup(r => r.ObterTodosPaginadoAsync(pageParams.PageNumber, pageParams.PageSize))
                .ReturnsAsync((pedidosFake, 2));

            _mapperMock.Setup(m => m.Map<IEnumerable<PedidoDto>>(pedidosFake)).Returns(dtosFake);

            // Act
            var resultado = await _service.ObterTodosPaginadoAsync(pageParams);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.TotalCount);
            Assert.Equal(2, resultado.Pedidos.Count());
            Assert.Equal(1, resultado.CurrentPage);
        }

        #endregion

        #region Cenários do ObterPedidosPorIdAsync

        [Fact]
        public async Task ObterPedidosPorIdAsync_QuandoPedidoExiste_DeveRetornarPedidoDto()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var pedidoFake = new Pedido("Cliente Teste");
            var dtoEsperado = new PedidoDto { Id = pedidoId, ClienteNome = "Cliente Teste" };

            _repoMock.Setup(r => r.ObterPorIdAsync(pedidoId)).ReturnsAsync(pedidoFake);
            _mapperMock.Setup(m => m.Map<PedidoDto>(pedidoFake)).Returns(dtoEsperado);

            // Act
            var resultado = await _service.ObterPedidosPorIdAsync(pedidoId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(pedidoId, resultado.Id);
        }

        [Fact]
        public async Task ObterPedidosPorIdAsync_QuandoPedidoNaoExiste_DeveLancarDomainException()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            _repoMock.Setup(r => r.ObterPorIdAsync(pedidoId)).ReturnsAsync((Pedido)null!);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<DomainException>(() => _service.ObterPedidosPorIdAsync(pedidoId));
            Assert.Contains("não encontrado", excecao.Message);
        }

        #endregion

        #region Cenários do ObterPedidosPorStatusAsync

        [Fact]
        public async Task ObterPedidosPorStatusAsync_ComStatusValido_DeveRetornarListaDePedidos()
        {
            // Arrange
            var statusString = "Pago";
            var pedidosFake = new List<Pedido> { new("Cliente Pago") };
            var dtosFake = new List<PedidoDto> { new() { ClienteNome = "Cliente Pago", Status = "Pago" } };

            _repoMock.Setup(r => r.ObterPorStatusAsync(StatusPedido.Pago)).ReturnsAsync(pedidosFake);
            _mapperMock.Setup(m => m.Map<IEnumerable<PedidoDto>>(pedidosFake)).Returns(dtosFake);

            // Act
            var resultado = await _service.ObterPedidosPorStatusAsync(statusString);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);
            Assert.Equal("Pago", resultado.First().Status);
        }

        [Fact]
        public async Task ObterPedidosPorStatusAsync_ComStatusInvalido_DeveLancarDomainException()
        {
            // Arrange
            var statusInvalido = "StatusQueNaoExiste";

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<DomainException>(() => _service.ObterPedidosPorStatusAsync(statusInvalido));
            Assert.Contains("Valores aceitos: Novo, Pago, Cancelado", excecao.Message);
        }

        #endregion

        #region Cenários do CancelarPedidosPorIdAsync

        [Fact]
        public async Task CancelarPedidosPorIdAsync_QuandoPedidoExiste_DeveMudarStatusParaCanceladoERetornarTrue()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var pedidoFake = new Pedido("Cliente Ativo"); 

            _repoMock.Setup(r => r.ObterPorIdAsync(pedidoId)).ReturnsAsync(pedidoFake);
            _repoMock.Setup(r => r.AtualizarAsync(pedidoFake)).ReturnsAsync(pedidoFake);

            // Act
            var resultado = await _service.CancelarPedidosPorIdAsync(pedidoId);

            // Assert
            Assert.True(resultado);
            _repoMock.Verify(r => r.AtualizarAsync(pedidoFake), Times.Once);
        }

        [Fact]
        public async Task CancelarPedidosPorIdAsync_QuandoPedidoNaoExiste_DeveLancarDomainException()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            _repoMock.Setup(r => r.ObterPorIdAsync(pedidoId)).ReturnsAsync((Pedido)null!);

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() => _service.CancelarPedidosPorIdAsync(pedidoId));
            _repoMock.Verify(r => r.AtualizarAsync(It.IsAny<Pedido>()), Times.Never); // Garante que não salvou nada
        }

        #endregion
    }
}
