using CentralPedidos.API.Controllers;
using CentralPedidos.Application.DTOs;
using CentralPedidos.Application.Interfaces;
using CentralPedidos.Application.Validations;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CentralPedidos.UnitTests.API.Controllers
{
    public class PedidosControllerTests
    {
        private readonly Mock<IPedidoService> _serviceMock;
        private readonly CriarPedidoValidator _realValidator;
        private readonly PedidosController _controller;

        public PedidosControllerTests()
        {
            _serviceMock = new Mock<IPedidoService>();
            _realValidator = new CriarPedidoValidator();
            _controller = new PedidosController(_serviceMock.Object, _realValidator);
        }

        #region Cenários de CriarPedido (POST)

        [Fact]
        public async Task CriarPedido_QuandoDadosForemValidos_DeveRetornarCreatedAtAction()
        {
            // Arrange
            var dto = new CriarPedidoDto
            {
                ClienteNome = "Cliente Valido",
                Itens = [new CriarItemPedidoDto { ProdutoNome = "Teste", Quantidade = 1, PrecoUnitario = 100.00m }]
            };

            var pedidoDtoResultado = new PedidoDto { Id = Guid.NewGuid(), ClienteNome = "Cliente Valido" };

            _serviceMock.Setup(s => s.CriarPedidoAsync(dto)).ReturnsAsync(pedidoDtoResultado);

            // Act
            var resultado = await _controller.CriarPedido(dto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(resultado);
            Assert.Equal(nameof(_controller.ObterPedidosPorId), createdAtActionResult.ActionName);
            Assert.Equal(pedidoDtoResultado.Id, ((PedidoDto)createdAtActionResult.Value!).Id);
        }

        [Fact]
        public async Task CriarPedido_QuandoDadosForemInvalidos_DeveRetornarBadRequestComErros()
        {
            // Arrange - Forçamos os dados incorretos para o validador real pegar
            var dto = new CriarPedidoDto
            {
                ClienteNome = "", 
                Itens = []       
            };

            // Act
            var resultado = await _controller.CriarPedido(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);
            var erros = Assert.IsType<IEnumerable<string>>(badRequestResult.Value, exactMatch: false).ToList();

            Assert.Equal(2, erros.Count);
            Assert.Contains("O nome do cliente é obrigatório.", erros);
            Assert.Contains("O pedido deve conter pelo menos um item.", erros);

            _serviceMock.Verify(s => s.CriarPedidoAsync(It.IsAny<CriarPedidoDto>()), Times.Never);
        }

        #endregion

        #region Cenários de ObterPedidosPorStatus (GET com Query)

        [Fact]
        public async Task ObterPedidosPorStatus_ComStatusValido_DeveRetornarOkComLista()
        {
            // Arrange
            var status = "Pago";
            var listaFake = new List<PedidoDto> { new() { ClienteNome = "Teste", Status = "Pago" } };
            _serviceMock.Setup(s => s.ObterPedidosPorStatusAsync(status)).ReturnsAsync(listaFake);

            // Act
            var resultado = await _controller.ObterPedidosPorStatus(status);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var retorno = Assert.IsAssignableFrom<IEnumerable<PedidoDto>>(okResult.Value);
            Assert.Single(retorno);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task ObterPedidosPorStatus_ComStatusEmBranco_DeveRetornarBadRequest(string statusInvalido)
        {
            // Act
            var resultado = await _controller.ObterPedidosPorStatus(statusInvalido);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);
            Assert.Equal("O parâmetro de status é obrigatório.", badRequestResult.Value);
        }

        #endregion

        #region Cenários de ObterPedidosPorId (GET por ID)

        [Fact]
        public async Task ObterPedidosPorId_QuandoPedidoExiste_DeveRetornarOkComPedido()
        {
            // Arrange
            var id = Guid.NewGuid();
            var pedidoFake = new PedidoDto { Id = id, ClienteNome = "Teste" };
            _serviceMock.Setup(s => s.ObterPedidosPorIdAsync(id)).ReturnsAsync(pedidoFake);

            // Act
            var resultado = await _controller.ObterPedidosPorId(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(id, ((PedidoDto)okResult.Value!).Id);
        }

        [Fact]
        public async Task ObterPedidosPorId_QuandoPedidoNaoExiste_DeveRetornarNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _serviceMock.Setup(s => s.ObterPedidosPorIdAsync(id)).ReturnsAsync((PedidoDto)null!);

            // Act
            var resultado = await _controller.ObterPedidosPorId(id);

            // Assert
            Assert.IsType<NotFoundResult>(resultado);
        }

        #endregion

        #region Cenários de ObterTodos (GET Paginado)

        [Fact]
        public async Task ObterTodos_DeveRetornarOkComPagedResult()
        {
            // Arrange
            var pageParams = new PageParams { PageNumber = 1, PageSize = 10 };
            var pagedResultFake = new PagedResult<PedidoDto>(new List<PedidoDto>(), 0, 1, 10);

            _serviceMock.Setup(s => s.ObterTodosPaginadoAsync(pageParams)).ReturnsAsync(pagedResultFake);

            // Act
            var resultado = await _controller.ObterTodos(pageParams);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.IsType<PagedResult<PedidoDto>>(okResult.Value);
        }

        #endregion

        #region Cenários de CancelarPedidoPorId (PUT)

        [Fact]
        public async Task CancelarPedidoPorId_QuandoSucesso_DeveRetornarNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            _serviceMock.Setup(s => s.CancelarPedidosPorIdAsync(id)).ReturnsAsync(true);

            // Act
            var resultado = await _controller.CancelarPedidoPorId(id);

            // Assert
            Assert.IsType<NoContentResult>(resultado);
        }

        [Fact]
        public async Task CancelarPedidoPorId_QuandoFalha_DeveRetornarBadRequest()
        {
            // Arrange
            var id = Guid.NewGuid();
            _serviceMock.Setup(s => s.CancelarPedidosPorIdAsync(id)).ReturnsAsync(false);

            // Act
            var resultado = await _controller.CancelarPedidoPorId(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);
            Assert.Equal("O pedido não pode ser cancelado.", badRequestResult.Value);
        }

        #endregion
    }
}
