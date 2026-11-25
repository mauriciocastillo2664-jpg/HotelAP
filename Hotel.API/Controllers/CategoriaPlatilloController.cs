using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfazLN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriaPlatilloController : ControllerBase
{
    private readonly ICategoriaPlatilloLN _logicaNegocio;

    public CategoriaPlatilloController(ICategoriaPlatilloLN logicaNegocio)
    {
        _logicaNegocio = logicaNegocio;
    }

    [HttpGet("Listar")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Listar(
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _logicaNegocio.ListarAsync(
            pagina, tamanoPagina, cancellationToken);

        return StatusCode(resultado.EstadoHttp, resultado);
    }

    [HttpGet("Obtener/{id:int}")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Obtener(
        int id,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _logicaNegocio.ObtenerAsync(id, cancellationToken);

        return StatusCode(resultado.EstadoHttp, resultado);
    }

    [HttpGet("Buscar")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Buscar(
        string texto = "",
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _logicaNegocio.BuscarAsync(
            texto, pagina, tamanoPagina, cancellationToken);

        return StatusCode(resultado.EstadoHttp, resultado);
    }

    [HttpPost("Insertar")]
    [Authorize(Policy = "Menu.Editar")]
    public async Task<IActionResult> Insertar(
        [FromBody] TCategoriaPlatillo datos,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _logicaNegocio.InsertarAsync(datos, cancellationToken);

        return StatusCode(resultado.EstadoHttp, resultado);
    }

    [HttpPut("Modificar")]
    [Authorize(Policy = "Menu.Editar")]
    public async Task<IActionResult> Modificar(
        [FromBody] TCategoriaPlatillo datos,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _logicaNegocio.ModificarAsync(datos, cancellationToken);

        return StatusCode(resultado.EstadoHttp, resultado);
    }

    [HttpDelete("Eliminar/{id:int}")]
    [Authorize(Policy = "Menu.Editar")]
    public async Task<IActionResult> Eliminar(
        int id,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _logicaNegocio.EliminarAsync(id, cancellationToken);

        return StatusCode(resultado.EstadoHttp, resultado);
    }
}