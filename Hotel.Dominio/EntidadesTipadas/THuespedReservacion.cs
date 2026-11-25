namespace Hotel.Dominio.EntidadesTipadas;

public class THuespedReservacion
{
    public int Id { get; set; }

    public int ReservacionId { get; set; }

    public string NombreCompleto { get; set; } = string.Empty;

    public int? TipoIdentificacionId { get; set; }

    public string? NumeroIdentificacion { get; set; }

    public bool EsMenor { get; set; }

    public bool EsTitular { get; set; }
}
