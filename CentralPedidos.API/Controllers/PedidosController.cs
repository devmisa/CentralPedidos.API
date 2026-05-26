using CentralPedidos.Application.DTOs;
using CentralPedidos.Application.Interfaces;
using CentralPedidos.Application.Validations;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace CentralPedidos.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController(IPedidoService pedidoService, CriarPedidoValidator validator) : ControllerBase
    {
        /// <summary>
        /// POST /pedidos
        /// Cria um pedido com itens
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CriarPedido([FromBody] CriarPedidoDto dto)
        {
            ValidationResult validationResult = validator.Validate(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            PedidoDto pedido = await pedidoService.CriarPedidoAsync(dto);

            return CreatedAtAction(
                nameof(ObterPedidosPorId),
                new { id = pedido.Id },
                pedido);
        }

        /// <summary>
        /// GET /pedidos?status=Pago
        /// Filtro por status
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ObterPedidosPorStatus([FromQuery] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return BadRequest("O parâmetro de status é obrigatório.");
            }

            IEnumerable<PedidoDto> pedidos = await pedidoService.ObterPedidosPorStatusAsync(status);

            return Ok(pedidos);
        }

        /// <summary>
        /// GET /pedidos/{id}
        /// Retorna pedido + itens
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPedidosPorId(Guid id)
        {
            PedidoDto pedido = await pedidoService.ObterPedidosPorIdAsync(id);

            return pedido == null ? NotFound() : Ok(pedido);
        }

        /// <summary>
        /// GET /pedidos
        /// Retorna todos os pedidos com seus respectivos itens.
        /// </summary>
        /// <returns></returns>
        [HttpGet("todos")]
        public async Task<IActionResult> ObterTodos([FromQuery] PageParams pageParams)
        {
            PagedResult<PedidoDto> resultado = await pedidoService.ObterTodosPaginadoAsync(pageParams);

            return Ok(resultado);
        }

        /// <summary>
        /// PUT /pedidos/{id}/cancelar
        /// Cancela pedido (não pode cancelar se já estiver Pago)
        /// </summary>
        [HttpPut("{id}/cancelar")]
        public async Task<IActionResult> CancelarPedidoPorId(Guid id)
        {
            bool result = await pedidoService.CancelarPedidosPorIdAsync(id);

            return !result ? BadRequest("O pedido não pode ser cancelado.") : NoContent();
        }

        /// <summary>
        /// PATCH /api/pedidos/{id}/status?novoStatus=Pago
        /// Altera o status do pedido de forma genérica
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> AlterarStatus(Guid id, [FromQuery] string novoStatus)
        {
            if (string.IsNullOrWhiteSpace(novoStatus))
            {
                return BadRequest("O parâmetro novoStatus é obrigatório na query string.");
            }

            PedidoDto pedidoAtualizado = await pedidoService.AlterarStatusAsync(id, novoStatus);

            return Ok(pedidoAtualizado);
        }
    }
}
