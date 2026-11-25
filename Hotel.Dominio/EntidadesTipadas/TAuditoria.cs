namespace Hotel.Dominio.EntidadesTipadas;

public class TAuditoria
{
    public int Id { get; set; }

    public int? UsuarioId { get; set; }

    public string Accion { get; set; } = string.Empty;

    public string Entidad { get; set; } = string.Empty;

    public int? EntidadId { get; set; }

    public string Resultado { get; set; } = string.Empty;

    public DateTime FechaUtc { get; set; }
}
