using System.Linq.Expressions;
using Hotel.AccesoDatos.Contexto;
using Hotel.Dominio.InterfacesAD;
using Microsoft.EntityFrameworkCore;

namespace Hotel.AccesoDatos.Implementaciones;

public class RepositorioAD<TEntity> : IRepositorioAD<TEntity>
    where TEntity : class
{
    private readonly HotelContext _contexto;

    public RepositorioAD(HotelContext contexto)
    {
        _contexto = contexto;
    }

    public IQueryable<TEntity> Consultar(bool seguimiento = false)
    {
        IQueryable<TEntity> consulta = _contexto.Set<TEntity>();

        if (!seguimiento)
        {
            consulta = consulta.AsNoTracking();
        }

        return consulta;
    }

    public async Task<TEntity?> ObtenerEntidadAsync(
        Expression<Func<TEntity, bool>> predicado,
        CancellationToken cancellationToken = default)
    {
        return await _contexto.Set<TEntity>()
            .FirstOrDefaultAsync(predicado, cancellationToken);
    }

    public async Task InsertarAsync(
        TEntity entidad,
        CancellationToken cancellationToken = default)
    {
        await _contexto.Set<TEntity>().AddAsync(entidad, cancellationToken);
    }

    public void Modificar(TEntity entidad)
    {
        _contexto.Set<TEntity>().Update(entidad);
    }

    public void Eliminar(TEntity entidad)
    {
        _contexto.Set<TEntity>().Remove(entidad);
    }
}
