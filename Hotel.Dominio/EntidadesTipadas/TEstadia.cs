namespace Hotel.Dominio.EntidadesTipadas;

public class TEstadia
{
    public int Id { get; set; }

    public int ReservacionId { get; set; }

    public DateTime CheckInReal { get; set; }

    public DateTime? CheckOutReal { get; set; }

    public int UsuarioCheckInId { get; set; }

    public int? UsuarioCheckOutId { get; set; }

    public string? CodigoAcceso { get; set; }
}
