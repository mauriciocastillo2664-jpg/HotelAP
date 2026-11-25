namespace Hotel.Dominio.EntidadesTipadas;

public class TMetodoPago
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public bool RequiereReferencia { get; set; }

    public bool Activo { get; set; } = true;
}
