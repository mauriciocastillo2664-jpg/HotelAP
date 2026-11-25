using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class VwEstadoUsuario
{
    public string TipoRegistro { get; set; } = null!;

    public int Id { get; set; }

    public string Identificador { get; set; } = null!;

    public string NombreCompleto { get; set; } = null!;

    public string TipoIdentificacion { get; set; } = null!;

    public string NumeroIdentificacion { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public bool PermiteAcceso { get; set; }
}
