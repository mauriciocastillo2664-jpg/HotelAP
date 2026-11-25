using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class HistorialReservacion
{
    public int Id { get; set; }

    public int ReservacionId { get; set; }

    public int? EstadoAnteriorId { get; set; }

    public int EstadoNuevoId { get; set; }

    public int? UsuarioId { get; set; }

    public string? Motivo { get; set; }

    public DateTime Fecha { get; set; }

    public virtual EstadoReservacion? EstadoAnterior { get; set; }

    public virtual EstadoReservacion EstadoNuevo { get; set; } = null!;

    public virtual Reservacion Reservacion { get; set; } = null!;

    public virtual Usuario? Usuario { get; set; }
}
