namespace Hotel.Dominio.EntidadesTipadas;

public class TEstadoPedido
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public int Orden { get; set; }

    public bool PermiteModificar { get; set; }

    public bool PermiteCancelar { get; set; }

    public bool EsFinal { get; set; }
}
