namespace Hotel.Dominio.EntidadesTipadas;

public class THistorialEstadoPedido
{
    public int Id { get; set; }

    public int PedidoId { get; set; }

    public int EstadoPedidoId { get; set; }

    public int? UsuarioId { get; set; }

    public DateTime Fecha { get; set; }

}
