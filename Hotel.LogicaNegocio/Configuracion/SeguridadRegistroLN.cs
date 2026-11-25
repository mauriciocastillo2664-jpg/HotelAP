using Hotel.Dominio.InterfazLN;
using Hotel.LogicaNegocio.Implementaciones;
using Microsoft.Extensions.DependencyInjection;

namespace Hotel.LogicaNegocio.Configuracion;

public static class SeguridadRegistroLN
{
    public static IServiceCollection AgregarSeguridadLN(this IServiceCollection servicios)
    {
        servicios.AddScoped<IAutorizacionLN, AutorizacionLN>();
        servicios.AddScoped<IUsuarioLN, UsuarioLN>();
        servicios.AddScoped<IRolLN, RolLN>();
        servicios.AddScoped<IPermisoLN, PermisoLN>();
        servicios.AddScoped<IRefreshTokenLN, RefreshTokenLN>();
        servicios.AddScoped<ITokenUsuarioLN, TokenUsuarioLN>();
        servicios.AddScoped<IAuditoriaLN, AuditoriaLN>();
        servicios.AddScoped<IReportesLN, ReportesLN>();

        return servicios;
    }
}