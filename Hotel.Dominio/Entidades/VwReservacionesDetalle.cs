using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class VwReservacionesDetalle
{
    public int Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public int? Noches { get; set; }

    public string Cliente { get; set; } = null!;

    public string Habitacion { get; set; } = null!;

    public string TipoHabitacion { get; set; } = null!;

    public decimal Subtotal { get; set; }

    public decimal Descuento { get; set; }

    public decimal Impuestos { get; set; }

    public decimal Total { get; set; }

    public decimal AnticipoRequerido { get; set; }

    public decimal? Saldo { get; set; }
}
