using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class Parametro
{
    public int Id { get; set; }

    public string Clave { get; set; } = null!;

    public string Valor { get; set; } = null!;

    public string TipoDato { get; set; } = null!;

    public string? Descripcion { get; set; }

    public DateTime FechaModificacion { get; set; }
}
