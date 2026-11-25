namespace Hotel.Dominio.EntidadesTipadas;

public class TAbono
{
    public int Id { get; set; }

    public int PagoId { get; set; }

    public int MetodoPagoId { get; set; }

    public decimal Monto { get; set; }

    public string? Referencia { get; set; }

    public string? UltimosDigitosTarjeta { get; set; }

    public string Estado { get; set; } = string.Empty;

    public string? MotivoReverso { get; set; }

    public DateTime FechaAbono { get; set; }

    public int? UsuarioId { get; set; }

}
