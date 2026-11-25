using Hotel.Dominio.InterfazLN;
using Hotel.LogicaNegocio.Implementaciones;
using Microsoft.Extensions.DependencyInjection;

namespace Hotel.LogicaNegocio;

public static class HospedajeRegistroLN
{
    public static IServiceCollection AddHospedajeLogicaNegocio(this IServiceCollection servicios)
    {
        servicios.AddScoped<IReservacionLN, ReservacionLN>();
        servicios.AddScoped<IReservacionHabitacionLN, ReservacionHabitacionLN>();
        servicios.AddScoped<IHuespedReservacionLN, HuespedReservacionLN>();
        servicios.AddScoped<IEstadiaLN, EstadiaLN>();
        servicios.AddScoped<ICargoAdicionalLN, CargoAdicionalLN>();
        servicios.AddScoped<IAjusteCuentaLN, AjusteCuentaLN>();
        servicios.AddScoped<IHistorialReservacionLN, HistorialReservacionLN>();
        servicios.AddScoped<IHistorialEstadoHabitacionLN, HistorialEstadoHabitacionLN>();
        return servicios;
    }
}
