namespace Hotel.Dominio.EntidadesTipadas;

public class TVwEstadoUsuario
{
    public string TipoRegistro { get; set; } = string.Empty;

    public int Id { get; set; }

    public string Identificador { get; set; } = string.Empty;

    public string NombreCompleto { get; set; } = string.Empty;

    public string TipoIdentificacion { get; set; } = string.Empty;

    public string NumeroIdentificacion { get; set; } = string.Empty;

    public string Estado { get; set; } = string.Empty;

    public bool PermiteAcceso { get; set; }
}
