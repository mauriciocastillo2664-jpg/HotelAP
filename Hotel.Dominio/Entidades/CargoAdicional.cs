using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class CargoAdicional
{
    public int Id { get; set; }

    public int ReservacionId { get; set; }

    public string Concepto { get; set; } = null!;

    public decimal Monto { get; set; }

    public decimal Impuesto { get; set; }

    public bool EsLateCheckout { get; set; }

    public int? UsuarioId { get; set; }

    public DateTime Fecha { get; set; }

    public virtual Reservacion Reservacion { get; set; } = null!;

    public virtual Usuario? Usuario { get; set; }
}
