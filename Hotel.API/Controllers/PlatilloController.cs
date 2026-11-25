
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfazLN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Menu.Editar")]
public class PlatilloController : ControllerBase
{
	private readonly IPlatilloLN _logicaNegocio;

	public PlatilloController(IPlatilloLN logicaNegocio)
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

	[HttpGet("Obtener/{id:int}")]
	[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
	public async Task<IActionResult> Obtener(
		int id,
		CancellationToken cancellationToken = default)
	{
		var resultado = await _logicaNegocio.ObtenerAsync(
			id, cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	[HttpPost("Insertar")]
	public async Task<IActionResult> Insertar(
		[FromBody] TPlatillo datos,
		CancellationToken cancellationToken = default)
	{
		var resultado = await _logicaNegocio.InsertarAsync(
			datos, cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	[HttpPut("Modificar")]
	public async Task<IActionResult> Modificar(
		[FromBody] TPlatillo datos,
		CancellationToken cancellationToken = default)
	{
		var resultado = await _logicaNegocio.ModificarAsync(
			datos, cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	[HttpDelete("Eliminar/{id:int}")]
	public async Task<IActionResult> Eliminar(
		int id,
		CancellationToken cancellationToken = default)
	{
		var resultado = await _logicaNegocio.EliminarAsync(
			id, cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}
}
