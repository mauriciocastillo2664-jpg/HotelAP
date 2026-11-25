
using Hotel.Dominio.InterfazLN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EstadiaController : ControllerBase
{
	private readonly IEstadiaLN _estadiaLN;

	public EstadiaController(IEstadiaLN estadiaLN)
	{
		_estadiaLN = estadiaLN;
	}

	// GET: api/Estadia/5
	[HttpGet("{reservacionId:int}")]
	public async Task<IActionResult> Listar(
		int reservacionId,
		[FromQuery] int pagina = 1,
		[FromQuery] int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _estadiaLN.ListarAsync(
			reservacionId,
			usuarioId,
			pagina,
			tamanoPagina,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// POST: api/Estadia/5/check-in
	[HttpPost("{reservacionId:int}/check-in")]
	public async Task<IActionResult> CheckIn(
		int reservacionId,
		[FromQuery] bool autorizarAnticipado = false,
		CancellationToken cancellationToken = default)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _estadiaLN.CheckInAsync(
			reservacionId,
			usuarioId,
			autorizarAnticipado,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// POST: api/Estadia/5/check-out
	[HttpPost("{reservacionId:int}/check-out")]
	public async Task<IActionResult> CheckOut(
		int reservacionId,
		CancellationToken cancellationToken = default)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _estadiaLN.CheckOutAsync(
			reservacionId,
			usuarioId,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	private bool IntentarObtenerUsuarioId(out int usuarioId)
	{
		return int.TryParse(
			User.FindFirst("sub")?.Value,
			out usuarioId);
	}
}
