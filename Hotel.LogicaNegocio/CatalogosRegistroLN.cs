using Hotel.Dominio.InterfazLN;
using Hotel.LogicaNegocio.Implementaciones;
using Microsoft.Extensions.DependencyInjection;

namespace Hotel.LogicaNegocio;

public static class CatalogosRegistroLN
{
    public static IServiceCollection AgregarCatalogosLN(this IServiceCollection services)
    {
        services.AddScoped<IClienteLN, ClienteLN>();
        services.AddScoped<IHabitacionLN, HabitacionLN>();
        services.AddScoped<IAmenidadLN, AmenidadLN>();
        services.AddScoped<IPlatilloLN, PlatilloLN>();
        services.AddScoped<IParametroLN, ParametroLN>();
        services.AddScoped<ITemporadaLN, TemporadaLN>();
        services.AddScoped<IDescuentoLN, DescuentoLN>();
        services.AddScoped<ITipoIdentificacionLN, TipoIdentificacionLN>();
        services.AddScoped<IMetodoPagoLN, MetodoPagoLN>();
        services.AddScoped<IEstadoCorreoLN, EstadoCorreoLN>();
        services.AddScoped<IEstadoHabitacionLN, EstadoHabitacionLN>();
        services.AddScoped<IEstadoPagoLN, EstadoPagoLN>();
        services.AddScoped<IEstadoPedidoLN, EstadoPedidoLN>();
        services.AddScoped<IEstadoReservacionLN, EstadoReservacionLN>();
        services.AddScoped<IEstadoUsuarioLN, EstadoUsuarioLN>();
        services.AddScoped<ITipoCorreoLN, TipoCorreoLN>();
        services.AddScoped<IHabitacionFotoLN, HabitacionFotoLN>();
        return services;
    }
}
