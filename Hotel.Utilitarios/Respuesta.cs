namespace Hotel.Utilitarios;

public class Respuesta<T>
{
    public bool Success { get; set; } = true;

    public T? Data { get; set; }

    public string Error { get; set; } = string.Empty;

    public int EstadoHttp { get; set; } = 200;
}
