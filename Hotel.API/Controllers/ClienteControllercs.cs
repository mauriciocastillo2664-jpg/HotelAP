
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfazLN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClienteController : ControllerBase
{
	private readonly IClienteLN _logicaNegocio;

	public ClienteController(IClienteLN logicaNegocio)
	{
		_logicaNegocio = logicaNegocio;
	}

	[HttpGet("Listar")]
	[Authorize(Policy = "Clientes.Leer")]
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

	[HttpGet("Buscar")]
	[Authorize(Policy = "Clientes.Leer")]
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

	[HttpGet("Obtener/{id:int}")]
	[Authorize(Policy = "Clientes.Leer")]
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

	[HttpPost("Insertar")]
	[Authorize(Policy = "Clientes.Editar")]
	public async Task<IActionResult> Insertar(
		[FromBody] TCliente datos,
		CancellationToken cancellationToken = default)
	{
		var resultado = await _logicaNegocio.InsertarAsync(
			datos,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	[HttpPut("Modificar")]
	[Authorize(Policy = "Clientes.Editar")]
	public async Task<IActionResult> Modificar(
		[FromBody] TCliente datos,
		CancellationToken cancellationToken = default)
	{
		var resultado = await _logicaNegocio.ModificarAsync(
			datos,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}
}
