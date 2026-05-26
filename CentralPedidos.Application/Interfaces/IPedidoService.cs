using CentralPedidos.Application.DTOs;

namespace CentralPedidos.Application.Interfaces
{
    public interface IPedidoService
    {
        Task<PedidoDto> CriarPedidoAsync(CriarPedidoDto dto);
        Task<PedidoDto> ObterPedidosPorIdAsync(Guid id);
        Task<PagedResult<PedidoDto>> ObterTodosPaginadoAsync(PageParams pageParams);
        Task<IEnumerable<PedidoDto>> ObterPedidosPorStatusAsync(string statusString);
        Task<bool> CancelarPedidosPorIdAsync(Guid id);

        Task<PedidoDto> AlterarStatusAsync(Guid id, string novoStatusString);
    }
}
