using CentralPedidos.Domain.Entities;
using CentralPedidos.Domain.Enums;

namespace CentralPedidos.Infrastructure.Interfaces
{
    public interface IPedidoRepository
    {
        Task<Pedido> AdicionarAsync(Pedido pedido);
        Task<Pedido> AtualizarAsync(Pedido pedido);
        Task<Pedido?> ObterPorIdAsync(Guid id);
        Task<(IEnumerable<Pedido> Pedidos, int TotalCount)> ObterTodosPaginadoAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Pedido>> ObterPorStatusAsync(StatusPedido status);
    }
}
