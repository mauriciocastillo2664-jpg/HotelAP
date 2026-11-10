using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class Auditoria
{
    public int Id { get; set; }

    public int? UsuarioId { get; set; }

    public string Accion { get; set; } = null!;

    public string Entidad { get; set; } = null!;

    public string? EntidadId { get; set; }

    public DateTime FechaUtc { get; set; }

    public string? Datos { get; set; }

    public virtual Usuario? Usuario { get; set; }
}
