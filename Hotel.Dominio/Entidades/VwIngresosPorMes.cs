using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class VwIngresosPorMes
{
    public int? Anio { get; set; }

    public int? Mes { get; set; }

    public string MetodoPago { get; set; } = null!;

    public string Origen { get; set; } = null!;

    public decimal? Total { get; set; }
}
