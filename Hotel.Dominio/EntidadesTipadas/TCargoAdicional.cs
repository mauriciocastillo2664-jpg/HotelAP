namespace Hotel.Dominio.EntidadesTipadas;

public class TCargoAdicional
{
    public int Id { get; set; }

    public int ReservacionId { get; set; }

    public string Concepto { get; set; } = string.Empty;

    public decimal Monto { get; set; }

    public decimal Impuesto { get; set; }

    public bool EsLateCheckout { get; set; }

    public int? UsuarioId { get; set; }

    public DateTime Fecha { get; set; }
}
