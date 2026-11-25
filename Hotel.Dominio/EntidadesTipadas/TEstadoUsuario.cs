namespace Hotel.Dominio.EntidadesTipadas;

public class TEstadoUsuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool PermiteAcceso { get; set; }
}
