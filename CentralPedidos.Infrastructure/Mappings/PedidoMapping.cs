using CentralPedidos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CentralPedidos.Infrastructure.Mappings
{
    public class PedidoMapping : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            _ = builder.ToTable("Pedidos");

            _ = builder.HasKey(p => p.Id);

            _ = builder.Property(p => p.ClienteNome)
                .IsRequired()
                .HasMaxLength(30);

            _ = builder.Property(p => p.DataCriacao);

            _ = builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<string>();

            _ = builder.Property(p => p.ValorTotal)
                .HasColumnType("decimal(18,2)");

            _ = builder.HasMany(p => p.Itens)
                .WithOne(i => i.Pedido)
                .HasForeignKey(i => i.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
