namespace Hotel.Dominio.EntidadesTipadas;

public class THistorialReservacion
{
    public int Id { get; set; }

    public int ReservacionId { get; set; }

    public int? EstadoAnteriorId { get; set; }

    public int EstadoNuevoId { get; set; }

    public int? UsuarioId { get; set; }

    public string? Motivo { get; set; }

    public DateTime Fecha { get; set; }
}
