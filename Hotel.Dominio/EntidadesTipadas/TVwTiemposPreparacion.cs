namespace Hotel.Dominio.EntidadesTipadas;

public class TVwTiemposPreparacion
{
    public int PedidoId { get; set; }

    public DateTime InicioPreparacion { get; set; }

    public DateTime Listo { get; set; }

    public int? MinutosReales { get; set; }

    public int? MinutosEstimados { get; set; }
}
