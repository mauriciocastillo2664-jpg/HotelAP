using Hotel.AccesoDatos.Contexto;
using Hotel.Dominio.Entidades;
using Hotel.Dominio.InterfacesAD;

namespace Hotel.AccesoDatos.Implementaciones;

public class UnidadTrabajoEF : IUnidadTrabajoEF
{
    private readonly HotelContext _contexto;

    public UnidadTrabajoEF(HotelContext contexto)
    {
        _contexto = contexto;
        TTipoHabitacion = new RepositorioAD<TipoHabitacion>(contexto);
        TCategoriaPlatillo = new RepositorioAD<CategoriaPlatillo>(contexto);
    }

    public IRepositorioAD<TipoHabitacion> TTipoHabitacion { get; }

    public IRepositorioAD<CategoriaPlatillo> TCategoriaPlatillo { get; }

    public IRepositorioAD<TEntity> Repositorio<TEntity>() where TEntity : class
    {
        return new RepositorioAD<TEntity>(_contexto);
    }

    public async Task<int> GuardarCambiosAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _contexto.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Un guardado fallido no debe dejar cambios pendientes para la siguiente operación.
            _contexto.ChangeTracker.Clear();
            throw;
        }
    }

    public async Task EjecutarEnTransaccionAsync(
        Func<CancellationToken, Task> operacion,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operacion);

        if (_contexto.Database.CurrentTransaction != null)
        {
            throw new InvalidOperationException("Ya existe una transacción en curso.");
        }

        if (_contexto.ChangeTracker.HasChanges())
        {
            throw new InvalidOperationException(
                "La transacción debe comenzar sin cambios pendientes de otra operación.");
        }

        await using var transaccion = await _contexto.Database
            .BeginTransactionAsync(cancellationToken);

        try
        {
            await operacion(cancellationToken);
            await GuardarCambiosAsync(cancellationToken);
            await transaccion.CommitAsync(cancellationToken);
        }
        catch
        {
            try
            {
                // El rollback también debe intentarse si se canceló la solicitud.
                await transaccion.RollbackAsync(CancellationToken.None);
            }
            catch
            {
                // Se conserva la excepción de la operación. DisposeAsync cierra la transacción.
            }

            _contexto.ChangeTracker.Clear();
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _contexto.DisposeAsync();
    }
}
