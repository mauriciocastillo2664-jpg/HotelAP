using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class Temporada
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public decimal PorcentajeRecargo { get; set; }

    public bool Activo { get; set; }
}
