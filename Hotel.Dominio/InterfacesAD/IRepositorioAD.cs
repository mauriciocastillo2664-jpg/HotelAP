using System.Linq.Expressions;

namespace Hotel.Dominio.InterfacesAD;

public interface IRepositorioAD<TEntity> where TEntity : class
{
    IQueryable<TEntity> Consultar(bool seguimiento = false);

    Task<TEntity?> ObtenerEntidadAsync(
        Expression<Func<TEntity, bool>> predicado,
        CancellationToken cancellationToken = default);

    Task InsertarAsync(
        TEntity entidad,
        CancellationToken cancellationToken = default);

    void Modificar(TEntity entidad);

    void Eliminar(TEntity entidad);
}
