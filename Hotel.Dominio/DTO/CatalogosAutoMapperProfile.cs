using AutoMapper;
using Hotel.Dominio.Entidades;
using Hotel.Dominio.EntidadesTipadas;

namespace Hotel.Dominio.DTO;

public class CatalogosAutoMapperProfile : Profile
{
    public CatalogosAutoMapperProfile()
    {
        CreateMap<Cliente, TCliente>();

        CreateMap<TCliente, Cliente>()
            .ForMember(destino => destino.Id, opciones => opciones.Ignore())
            .ForMember(destino => destino.EstadoUsuarioId, opciones => opciones.Ignore())
            .ForMember(destino => destino.FechaRegistro, opciones => opciones.Ignore())
            .ForMember(destino => destino.RowVersion, opciones => opciones.Ignore())
            .ForMember(destino => destino.Correos, opciones => opciones.Ignore())
            .ForMember(destino => destino.Descuento, opciones => opciones.Ignore())
            .ForMember(destino => destino.EstadoUsuario, opciones => opciones.Ignore())
            .ForMember(destino => destino.Facturas, opciones => opciones.Ignore())
            .ForMember(destino => destino.Pagos, opciones => opciones.Ignore())
            .ForMember(destino => destino.PedidosRestaurante, opciones => opciones.Ignore())
            .ForMember(destino => destino.Reservaciones, opciones => opciones.Ignore())
            .ForMember(destino => destino.TipoIdentificacion, opciones => opciones.Ignore())
            .ForMember(destino => destino.Usuario, opciones => opciones.Ignore());

        CreateMap<Habitacion, THabitacion>();

        CreateMap<THabitacion, Habitacion>()
            .ForMember(destino => destino.Id, opciones => opciones.Ignore())
            .ForMember(destino => destino.EstadoHabitacionId, opciones => opciones.Ignore())
            .ForMember(destino => destino.Activo, opciones => opciones.Ignore())
            .ForMember(destino => destino.RowVersion, opciones => opciones.Ignore())
            .ForMember(destino => destino.EstadoHabitacion, opciones => opciones.Ignore())
            .ForMember(destino => destino.HabitacionFotos, opciones => opciones.Ignore())
            .ForMember(destino => destino.HistorialEstadosHabitacion, opciones => opciones.Ignore())
            .ForMember(destino => destino.PedidosRestaurante, opciones => opciones.Ignore())
            .ForMember(destino => destino.ReservacionHabitaciones, opciones => opciones.Ignore())
            .ForMember(destino => destino.TipoHabitacion, opciones => opciones.Ignore())
            .ForMember(destino => destino.Amenidads, opciones => opciones.Ignore());

        CreateMap<Amenidad, TAmenidad>();

        CreateMap<TAmenidad, Amenidad>()
            .ForMember(destino => destino.Id, opciones => opciones.Ignore())
            .ForMember(destino => destino.Habitacions, opciones => opciones.Ignore());

        CreateMap<Platillo, TPlatillo>();

        CreateMap<TPlatillo, Platillo>()
            .ForMember(destino => destino.Id, opciones => opciones.Ignore())
            .ForMember(destino => destino.Activo, opciones => opciones.Ignore())
            .ForMember(destino => destino.Categoria, opciones => opciones.Ignore())
            .ForMember(destino => destino.DetallePedidos, opciones => opciones.Ignore());

        CreateMap<Parametro, TParametro>();

        CreateMap<TParametro, Parametro>()
            .ForMember(destino => destino.Id, opciones => opciones.Ignore())
            .ForMember(destino => destino.FechaModificacion, opciones => opciones.Ignore());

        CreateMap<Temporada, TTemporada>();

        CreateMap<TTemporada, Temporada>()
            .ForMember(destino => destino.Id, opciones => opciones.Ignore())
            .ForMember(destino => destino.Activo, opciones => opciones.Ignore());

        CreateMap<Descuento, TDescuento>();

        CreateMap<TDescuento, Descuento>()
            .ForMember(destino => destino.Id, opciones => opciones.Ignore())
            .ForMember(destino => destino.Activo, opciones => opciones.Ignore())
            .ForMember(destino => destino.Clientes, opciones => opciones.Ignore())
            .ForMember(destino => destino.PedidosRestaurante, opciones => opciones.Ignore())
            .ForMember(destino => destino.Reservaciones, opciones => opciones.Ignore());

        CreateMap<TipoIdentificacion, TTipoIdentificacion>();

        CreateMap<TTipoIdentificacion, TipoIdentificacion>()
            .ForMember(destino => destino.Id, opciones => opciones.Ignore())
            .ForMember(destino => destino.Activo, opciones => opciones.Ignore())
            .ForMember(destino => destino.Clientes, opciones => opciones.Ignore())
            .ForMember(destino => destino.HuespedesReservacion, opciones => opciones.Ignore())
            .ForMember(destino => destino.Usuarios, opciones => opciones.Ignore());

        CreateMap<MetodoPago, TMetodoPago>();

        CreateMap<TMetodoPago, MetodoPago>()
            .ForMember(destino => destino.Id, opciones => opciones.Ignore())
            .ForMember(destino => destino.Activo, opciones => opciones.Ignore())
            .ForMember(destino => destino.Abonos, opciones => opciones.Ignore());

        CreateMap<EstadoCorreo, TEstadoCorreo>();

        CreateMap<EstadoHabitacion, TEstadoHabitacion>();

        CreateMap<EstadoPago, TEstadoPago>();

        CreateMap<EstadoPedido, TEstadoPedido>();

        CreateMap<EstadoReservacion, TEstadoReservacion>();

        CreateMap<EstadoUsuario, TEstadoUsuario>();

        CreateMap<TipoCorreo, TTipoCorreo>();

        CreateMap<HabitacionFoto, THabitacionFoto>();

        CreateMap<THabitacionFoto, HabitacionFoto>()
            .ForMember(destino => destino.Id, opciones => opciones.Ignore())
            .ForMember(destino => destino.Habitacion, opciones => opciones.Ignore());
    }
}
