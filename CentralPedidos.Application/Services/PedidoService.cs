using AutoMapper;
using CentralPedidos.Application.DTOs;
using CentralPedidos.Application.Interfaces;
using CentralPedidos.Domain.Entities;
using CentralPedidos.Domain.Enums;
using CentralPedidos.Domain.Exceptions;
using CentralPedidos.Infrastructure.Interfaces;
using Serilog;

namespace CentralPedidos.Application.Services
{
    public class PedidoService(
        IPedidoRepository repo,
        ILogger logger,
        IMapper mapper) : IPedidoService
    {
        public async Task<PedidoDto> CriarPedidoAsync(CriarPedidoDto dto)
        {
            logger.Information("Processando a criação do pedido para o cliente: {ClienteNome}", dto.ClienteNome);

            Pedido pedido = mapper.Map<Pedido>(dto);

            foreach (CriarItemPedidoDto itemDto in dto.Itens)
            {
                ItemPedido itemPedido = mapper.Map<ItemPedido>(itemDto);
                pedido.AdicionarItem(itemPedido);
            }

            Pedido pedidoSalvo = await repo.AdicionarAsync(pedido);

            logger.Information("Pedido criado com sucesso para o cliente: {ClienteNome}, Id: {Id}",
                pedidoSalvo.ClienteNome,
                pedidoSalvo.Id);

            return mapper.Map<PedidoDto>(pedidoSalvo);
        }

        public async Task<PagedResult<PedidoDto>> ObterTodosPaginadoAsync(PageParams pageParams)
        {
            (IEnumerable<Pedido>? pedidos, int totalCount) = await repo.ObterTodosPaginadoAsync(pageParams.PageNumber, pageParams.PageSize);

            IEnumerable<PedidoDto> dtos = mapper.Map<IEnumerable<PedidoDto>>(pedidos);

            return new PagedResult<PedidoDto>(dtos, totalCount, pageParams.PageNumber, pageParams.PageSize);
        }

        public async Task<PedidoDto> ObterPedidosPorIdAsync(Guid id)
        {
            Pedido? pedido = await repo.ObterPorIdAsync(id);

            if (pedido == null)
            {
                logger.Warning("Pedido com Id {Id} não encontrado.", id);
                throw new DomainException($"Pedido com Id {id} não encontrado.");
            }

            return mapper.Map<PedidoDto>(pedido);
        }

        public async Task<IEnumerable<PedidoDto>> ObterPedidosPorStatusAsync(string statusString)
        {
            if (!Enum.TryParse<StatusPedido>(statusString, true, out StatusPedido statusEnum))
            {
                throw new DomainException($"O status '{statusString}' é inválido. Valores aceitos: Novo, Pago, Cancelado.");
            }

            IEnumerable<Pedido> pedidos = await repo.ObterPorStatusAsync(statusEnum);

            return mapper.Map<IEnumerable<PedidoDto>>(pedidos);
        }

        public async Task<bool> CancelarPedidosPorIdAsync(Guid id)
        {
            Pedido? pedido = await repo.ObterPorIdAsync(id);

            if (pedido == null)
            {
                logger.Warning("Pedido com Id {Id} não encontrado.", id);
                throw new DomainException($"Pedido com Id {id} não encontrado.");
            }

            pedido.Cancelar();
            _ = await repo.AtualizarAsync(pedido);

            logger.Information("Pedido com Id {Id} cancelado com sucesso.", id);

            return true;
        }

        public async Task<PedidoDto> AlterarStatusAsync(Guid id, string novoStatusString)
        {
            if (!Enum.TryParse<StatusPedido>(novoStatusString, true, out StatusPedido novoStatusEnum))
                throw new DomainException($"O status '{novoStatusString}' é inválido. Valores aceitos: Novo, Pago, Cancelado.");

            Pedido? pedido = await repo.ObterPorIdAsync(id);

            if (pedido == null)
            {
                logger.Warning("Pedido com Id {Id} não encontrado para alteração de status.", id);
                throw new DomainException($"Pedido com Id {id} não encontrado.");
            }

            pedido.AlterarStatus(novoStatusEnum);

            _ = await repo.AtualizarAsync(pedido);

            logger.Information("Status do pedido {Id} alterado com sucesso para {NovoStatus}.", id, novoStatusEnum);

            return mapper.Map<PedidoDto>(pedido);
        }
    }
}
