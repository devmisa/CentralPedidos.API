using CentralPedidos.Domain.Entities;
using CentralPedidos.Domain.Enums;
using CentralPedidos.Infrastructure.Context;
using CentralPedidos.Infrastructure.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace CentralPedidos.Infrastructure.Repositories
{
    public class PedidoRepository(AppDbContext context) : IPedidoRepository
    {
        public async Task<Pedido> AdicionarAsync(Pedido pedido)
        {
            _ = await context.Pedidos.AddAsync(pedido);
            _ = await context.SaveChangesAsync();

            return pedido;
        }

        public async Task<Pedido> AtualizarAsync(Pedido pedido)
        {
            _ = context.Pedidos.Update(pedido);
            _ = await context.SaveChangesAsync();

            return pedido;
        }

        public async Task<Pedido?> ObterPorIdAsync(Guid id)
        {
            return await context.Pedidos
                .Include(p => p.Itens)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Pedido>> ObterPorStatusAsync(StatusPedido status)
        {
            return await context.Pedidos
             .Where(p => p.Status == status)
             .Include(p => p.Itens)
             .ToListAsync();
        }

        public async Task<(IEnumerable<Pedido> Pedidos, int TotalCount)> ObterTodosPaginadoAsync(int pageNumber, int pageSize)
        {
            DbConnection connection = context.Database.GetDbConnection();

            int totalCount = await connection.ExecuteScalarAsync<int>(countSql);

            if (totalCount == 0)
            {
                return (Enumerable.Empty<Pedido>(), 0);
            }

            int offset = (pageNumber - 1) * pageSize;
            List<Guid> pagedIds = [.. (await connection.QueryAsync<Guid>(pageIdsSql, new { PageSize = pageSize, Offset = offset }))];

            Dictionary<Guid, Pedido> pedidoDictionary = [];

            _ = await connection.QueryAsync<Pedido, ItemPedido, Pedido>(
                querySql,
                (pedido, item) =>
                {
                    if (!pedidoDictionary.TryGetValue(pedido.Id, out Pedido? pedidoExistente))
                    {
                        pedidoExistente = pedido;
                        pedidoDictionary.Add(pedidoExistente.Id, pedidoExistente);
                    }

                    if (item != null)
                    {
                        pedidoExistente.AdicionarItem(item);
                    }

                    return pedidoExistente;
                },
                new { Ids = pagedIds },
                splitOn: "Id"
            );

            return (pedidoDictionary.Values, totalCount);
        }


        #region private queries

        private const string countSql = "SELECT COUNT(*) FROM Pedidos";

        private const string pageIdsSql = @"
                SELECT Id FROM Pedidos 
                ORDER BY DataCriacao DESC 
                LIMIT @PageSize OFFSET @Offset";

        private const string querySql = @"
                SELECT p.*, i.* FROM Pedidos p
                LEFT JOIN ItensPedido i ON p.Id = i.PedidoId
                WHERE p.Id IN @Ids
                ORDER BY p.DataCriacao DESC";

        #endregion
    }
}
