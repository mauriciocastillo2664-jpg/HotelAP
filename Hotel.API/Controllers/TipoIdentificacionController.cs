
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfazLN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Usuarios.Gestionar")]
public class TipoIdentificacionController : ControllerBase
{
	private readonly ITipoIdentificacionLN _logicaNegocio;

	public TipoIdentificacionController(
		ITipoIdentificacionLN logicaNegocio)
	{
		_logicaNegocio = logicaNegocio;
	}

	// GET: api/TipoIdentificacion/Listar
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

	// GET: api/TipoIdentificacion/Buscar?texto=cedula
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

	// GET: api/TipoIdentificacion/Obtener/1
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

	// POST: api/TipoIdentificacion/Insertar
	[HttpPost("Insertar")]
	public async Task<IActionResult> Insertar(
		[FromBody] TTipoIdentificacion datos,
		CancellationToken cancellationToken = default)
	{
		var resultado = await _logicaNegocio.InsertarAsync(
			datos,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// PUT: api/TipoIdentificacion/Modificar
	[HttpPut("Modificar")]
	public async Task<IActionResult> Modificar(
		[FromBody] TTipoIdentificacion datos,
		CancellationToken cancellationToken = default)
	{
		var resultado = await _logicaNegocio.ModificarAsync(
			datos,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// DELETE: api/TipoIdentificacion/Eliminar/1
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
