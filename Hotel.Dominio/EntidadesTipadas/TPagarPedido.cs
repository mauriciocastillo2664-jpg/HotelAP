namespace Hotel.Dominio.EntidadesTipadas;

public class TPagarPedido
{
    public int PedidoId { get; set; }

    public int MetodoPagoId { get; set; }

    public string? Referencia { get; set; }

    public string? UltimosDigitos { get; set; }

}
