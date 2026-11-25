using AutoMapper;
using Hotel.Dominio.Entidades;
using Hotel.Dominio.EntidadesTipadas;

namespace Hotel.Dominio.DTO;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<TipoHabitacion, TTipoHabitacion>();
        CreateMap<TTipoHabitacion, TipoHabitacion>()
            .ForMember(destino => destino.Id, opciones => opciones.Ignore())
            .ForMember(destino => destino.Activo, opciones => opciones.Ignore())
            .ForMember(destino => destino.Habitaciones, opciones => opciones.Ignore());

        CreateMap<CategoriaPlatillo, TCategoriaPlatillo>();
        CreateMap<TCategoriaPlatillo, CategoriaPlatillo>()
            .ForMember(destino => destino.Id, opciones => opciones.Ignore())
            .ForMember(destino => destino.Activo, opciones => opciones.Ignore())
            .ForMember(destino => destino.Platillos, opciones => opciones.Ignore());
    }
}
