using Hotel.Dominio.Entidades;

namespace Hotel.Dominio.InterfacesAD;

public interface IUnidadTrabajoEF : IAsyncDisposable
{
    IRepositorioAD<TipoHabitacion> TTipoHabitacion { get; }

    IRepositorioAD<CategoriaPlatillo> TCategoriaPlatillo { get; }

    IRepositorioAD<TEntity> Repositorio<TEntity>() where TEntity : class;

    Task<int> GuardarCambiosAsync(
        CancellationToken cancellationToken = default);

    Task EjecutarEnTransaccionAsync(
        Func<CancellationToken, Task> operacion,
        CancellationToken cancellationToken = default);
}
