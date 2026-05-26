using AutoMapper;
using CentralPedidos.Application.DTOs;
using CentralPedidos.Domain.Entities;

namespace CentralPedidos.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            _ = CreateMap<CriarItemPedidoDto, ItemPedido>()
                .ConstructUsing(src => new ItemPedido(src.ProdutoNome, src.Quantidade, src.PrecoUnitario));

            CreateMap<CriarPedidoDto, Pedido>()
                .ConstructUsing(src => new Pedido(src.ClienteNome))
                .ForMember(dest => dest.Itens, opt => opt.Ignore())
                .ForSourceMember(src => src.Itens, opt => opt.DoNotValidate());

            _ = CreateMap<Pedido, PedidoDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens));

            _ = CreateMap<ItemPedido, ItemPedidoDto>();

        }
    }
}
