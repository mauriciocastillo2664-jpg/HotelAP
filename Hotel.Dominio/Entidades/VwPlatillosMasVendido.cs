using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class VwPlatillosMasVendido
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Categoria { get; set; } = null!;

    public int? CantidadVendida { get; set; }

    public decimal? TotalVendido { get; set; }
}
