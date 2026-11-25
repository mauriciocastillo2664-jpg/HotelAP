using Hotel.Dominio.InterfazLN;
using Hotel.LogicaNegocio.Implementaciones;
using Microsoft.Extensions.DependencyInjection;

namespace Hotel.LogicaNegocio.Configuracion;

public static class RestauranteFinanzasRegistroLN
{
    public static IServiceCollection AddRestauranteFinanzasLogicaNegocio(this IServiceCollection servicios)
    {
        servicios.AddScoped<IPedidoRestauranteLN, PedidoRestauranteLN>();
        servicios.AddScoped<IDetallePedidoLN, DetallePedidoLN>();
        servicios.AddScoped<IHistorialEstadoPedidoLN, HistorialEstadoPedidoLN>();
        servicios.AddScoped<IPagoLN, PagoLN>();
        servicios.AddScoped<IAbonoLN, AbonoLN>();
        servicios.AddScoped<IFacturaLN, FacturaLN>();
        servicios.AddScoped<IFacturaDetalleLN, FacturaDetalleLN>();
        servicios.AddScoped<ICorreoLN, CorreoLN>();

        return servicios;
    }
}
