namespace Hotel.Dominio.InterfazLN;

public interface IAutorizacionLN
{
    Task<bool> UsuarioActivoAsync(int usuarioId, CancellationToken cancellationToken = default);

    Task<bool> TienePermisoAsync(
        int usuarioId, string permiso, CancellationToken cancellationToken = default);
}
