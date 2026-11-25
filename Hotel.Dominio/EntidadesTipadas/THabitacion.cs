namespace Hotel.Dominio.EntidadesTipadas;

public class THabitacion
{
    public int Id { get; set; }

    public string Numero { get; set; } = string.Empty;

    public int Piso { get; set; }

    public int TipoHabitacionId { get; set; }

    public int EstadoHabitacionId { get; set; }

    public bool Activo { get; set; } = true;

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
