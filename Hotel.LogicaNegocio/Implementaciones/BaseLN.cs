using AutoMapper;
using Hotel.Dominio.InterfacesAD;
using Hotel.Utilitarios;
using Microsoft.Extensions.Logging;

namespace Hotel.LogicaNegocio.Implementaciones;

public abstract class BaseLN<T>
{
    protected readonly IUnidadTrabajoEF _unidadDeTrabajo;
    protected readonly ILogger<T> _logger;
    protected readonly IMapper _mapper;

    protected BaseLN(IUnidadTrabajoEF unidadDeTrabajo, ILogger<T> logger, IMapper mapper)
    {
        _unidadDeTrabajo = unidadDeTrabajo;
        _logger = logger;
        _mapper = mapper;
    }

    protected static Respuesta<TDato> Fallar<TDato>(string mensaje, int estadoHttp)
    {
        return new Respuesta<TDato>
        {
            Success = false,
            Error = mensaje,
            EstadoHttp = estadoHttp
        };
    }

    protected static bool PaginacionValida(int pagina, int tamanoPagina)
    {
        return pagina >= 1 && tamanoPagina >= 1 && tamanoPagina <= 100
            && (long)(pagina - 1) * tamanoPagina <= int.MaxValue;
    }
}
