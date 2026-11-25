namespace Hotel.Dominio.EntidadesTipadas;

public class TActualizarEstadoCorreo
{
    public int CorreoId { get; set; }

    public bool Exito { get; set; }

    public string? MensajeError { get; set; }

}
