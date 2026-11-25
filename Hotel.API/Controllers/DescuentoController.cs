
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfazLN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Parametros.Editar")]
public class DescuentoController : ControllerBase
{
	private readonly IDescuentoLN _logicaNegocio;

	public DescuentoController(IDescuentoLN logicaNegocio)
	{
		_logicaNegocio = logicaNegocio;
	}

	// GET: api/Descuento/Listar
	[HttpGet("Listar")]
	[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
	public async Task<IActionResult> Listar(
		int pagina = 1,
		int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		var resultado = await _logicaNegocio.ListarAsync(
			pagina,
			tamanoPagina,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// GET: api/Descuento/Buscar?texto=...
	[HttpGet("Buscar")]
	[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
	public async Task<IActionResult> Buscar(
		string texto = "",
		int pagina = 1,
		int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		var resultado = await _logicaNegocio.BuscarAsync(
			texto,
			pagina,
			tamanoPagina,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// GET: api/Descuento/Obtener/1
	[HttpGet("Obtener/{id:int}")]
	[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
	public async Task<IActionResult> Obtener(
		int id,
		CancellationToken cancellationToken = default)
	{
		var resultado = await _logicaNegocio.ObtenerAsync(
			id,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// POST: api/Descuento/Insertar
	[HttpPost("Insertar")]
	public async Task<IActionResult> Insertar(
		[FromBody] TDescuento datos,
		CancellationToken cancellationToken = default)
	{
		var resultado = await _logicaNegocio.InsertarAsync(
			datos,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// PUT: api/Descuento/Modificar
	[HttpPut("Modificar")]
	public async Task<IActionResult> Modificar(
		[FromBody] TDescuento datos,
		CancellationToken cancellationToken = default)
	{
		var resultado = await _logicaNegocio.ModificarAsync(
			datos,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// DELETE: api/Descuento/Eliminar/1
	[HttpDelete("Eliminar/{id:int}")]
	public async Task<IActionResult> Eliminar(
		int id,
		CancellationToken cancellationToken = default)
	{
		var resultado = await _logicaNegocio.EliminarAsync(
			id,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}
}
