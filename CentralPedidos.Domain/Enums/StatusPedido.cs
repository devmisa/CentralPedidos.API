using System.ComponentModel;

namespace CentralPedidos.Domain.Enums
{
    public enum StatusPedido
    {
        [Description("Novo")]
        Novo = 1,
        [Description("Pago")]
        Pago = 2,
        [Description("Cancelado")]
        Cancelado = 3
    }
}
