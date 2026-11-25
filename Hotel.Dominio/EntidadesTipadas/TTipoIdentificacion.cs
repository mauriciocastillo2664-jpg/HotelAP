namespace Hotel.Dominio.EntidadesTipadas;

public class TTipoIdentificacion
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Formato { get; set; }

    public bool Activo { get; set; } = true;
}
