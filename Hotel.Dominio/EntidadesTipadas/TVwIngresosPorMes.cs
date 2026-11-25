namespace Hotel.Dominio.EntidadesTipadas;

public class TVwIngresosPorMes
{
    public int? Anio { get; set; }

    public int? Mes { get; set; }

    public string MetodoPago { get; set; } = string.Empty;

    public string Origen { get; set; } = string.Empty;

    public decimal? Total { get; set; }
}
