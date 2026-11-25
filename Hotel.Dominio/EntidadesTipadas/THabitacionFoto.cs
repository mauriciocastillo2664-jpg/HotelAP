namespace Hotel.Dominio.EntidadesTipadas;

public class THabitacionFoto
{
    public int Id { get; set; }

    public int HabitacionId { get; set; }

    public string Url { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool EsPrincipal { get; set; }

    public int Orden { get; set; }
}
