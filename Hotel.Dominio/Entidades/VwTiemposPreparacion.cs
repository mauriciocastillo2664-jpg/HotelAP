using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class VwTiemposPreparacion
{
    public int PedidoId { get; set; }

    public DateTime InicioPreparacion { get; set; }

    public DateTime Listo { get; set; }

    public int? MinutosReales { get; set; }

    public int? MinutosEstimados { get; set; }
}
