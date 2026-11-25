using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class VwEstadoCuenta
{
    public int PagoId { get; set; }

    public string Cliente { get; set; } = null!;

    public string? Concepto { get; set; }

    public decimal MontoTotal { get; set; }

    public decimal TotalAbonado { get; set; }

    public decimal? Saldo { get; set; }

    public string EstadoPago { get; set; } = null!;

    public int? CantidadAbonos { get; set; }

    public DateTime? FechaActualizacion { get; set; }
}
