namespace Hotel.Dominio.EntidadesTipadas;

public class TDetallePedido
{
    public int Id { get; set; }

    public int PedidoId { get; set; }

    public int PlatilloId { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal? Subtotal { get; set; }

}
