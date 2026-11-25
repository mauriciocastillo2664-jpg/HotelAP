namespace Hotel.Dominio.EntidadesTipadas;

public class TAjusteCuenta
{
    public int Id { get; set; }

    public int ReservacionId { get; set; }

    public decimal Monto { get; set; }

    public string Motivo { get; set; } = string.Empty;

    public int UsuarioId { get; set; }

    public int AutorizadoPorId { get; set; }

    public DateTime Fecha { get; set; }
}
