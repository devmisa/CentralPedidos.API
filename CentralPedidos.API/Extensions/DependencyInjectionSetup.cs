using CentralPedidos.Application.Interfaces;
using CentralPedidos.Application.Mappings;
using CentralPedidos.Application.Services;
using CentralPedidos.Application.Validations;
using CentralPedidos.Infrastructure.Context;
using CentralPedidos.Infrastructure.Interfaces;
using CentralPedidos.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CentralPedidos.API.Extensions
{
    public static class DependencyInjectionSetup
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            _ = services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            _ = services.AddScoped<IPedidoRepository, PedidoRepository>();
            _ = services.AddScoped<IPedidoService, PedidoService>();
            _ = services.AddScoped<CriarPedidoValidator>();

            _ = services.AddAutoMapper(config =>
            {
                config.AddProfile<MappingProfile>();
            });

            return services;
        }
    }
}
