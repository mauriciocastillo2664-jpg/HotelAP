namespace Hotel.Dominio.EntidadesTipadas;

public class TCrearPedido
{
    public int ClienteId { get; set; }

    public string Modalidad { get; set; } = string.Empty;

    public bool CargarAHabitacion { get; set; }

    public DateTime? FechaHoraDeseada { get; set; }

    public string? Instrucciones { get; set; }

    public string? CodigoDescuento { get; set; }

}
