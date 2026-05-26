using CentralPedidos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CentralPedidos.Infrastructure.Mappings
{
    public class ItemPedidoMapping : IEntityTypeConfiguration<ItemPedido>
    {
        public void Configure(EntityTypeBuilder<ItemPedido> builder)
        {
            _ = builder.ToTable("ItensPedido");

            _ = builder.HasKey(i => i.Id);

            _ = builder.Property(i => i.PedidoId)
                .IsRequired();

            _ = builder.Property(i => i.ProdutoNome)
                .IsRequired()
                .HasMaxLength(30);

            _ = builder.Property(i => i.Quantidade)
                .IsRequired();

            _ = builder.Property(i => i.PrecoUnitario)
                .HasColumnType("decimal(18,2)");
        }
    }
}
