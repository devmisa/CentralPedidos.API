using CentralPedidos.Application.DTOs;
using FluentValidation;

namespace CentralPedidos.Application.Validations
{
    public class CriarPedidoValidator : AbstractValidator<CriarPedidoDto>
    {
        public CriarPedidoValidator()
        {
            _ = RuleFor(x => x.ClienteNome)
                .NotEmpty()
                    .WithMessage("O nome do cliente é obrigatório.")
                .MaximumLength(30)
                    .WithMessage("O nome do cliente não pode exceder 30 caracteres.");

            _ = RuleFor(x => x.Itens)
                .NotEmpty()
                    .WithMessage("O pedido deve conter pelo menos um item.");

            _ = RuleForEach(x => x.Itens)
                .ChildRules(item =>
                {
                    _ = item.RuleFor(i => i.ProdutoNome)
                        .NotEmpty()
                            .WithMessage("O nome do produto é obrigatório.")
                                .MaximumLength(30)
                                    .WithMessage("O nome do produto não pode exceder 30 caracteres.");

                    _ = item.RuleFor(i => i.Quantidade)
                        .GreaterThan(0)
                            .WithMessage("A quantidade deve ser maior que zero.");
                });
        }

    }
}
