using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class VwFacturaAbono
{
    public int FacturaId { get; set; }

    public string NumeroFactura { get; set; } = null!;

    public int AbonoId { get; set; }

    public DateTime FechaAbono { get; set; }

    public string MetodoPago { get; set; } = null!;

    public string? Referencia { get; set; }

    public string? UltimosDigitosTarjeta { get; set; }

    public decimal Monto { get; set; }

    public string Estado { get; set; } = null!;

    public decimal TotalFactura { get; set; }

    public decimal TotalAbonado { get; set; }

    public decimal? Saldo { get; set; }

    public string EstadoPago { get; set; } = null!;
}
