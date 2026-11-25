namespace Hotel.Dominio.EntidadesTipadas;

public class TRegistrarAbono
{
    public int ReservacionId { get; set; }

    public decimal Monto { get; set; }

    public int MetodoPagoId { get; set; }

    public string? Referencia { get; set; }

    public string? UltimosDigitos { get; set; }

}
