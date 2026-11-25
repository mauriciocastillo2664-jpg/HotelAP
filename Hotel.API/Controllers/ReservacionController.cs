
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfazLN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReservacionController : ControllerBase
{
	private readonly IReservacionLN _reservacionLN;

	public ReservacionController(IReservacionLN reservacionLN)
	{
		_reservacionLN = reservacionLN;
	}

	// GET: api/Reservacion
	[HttpGet]
	public async Task<IActionResult> Listar(
		[FromQuery] int pagina = 1,
		[FromQuery] int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _reservacionLN.ListarAsync(
			usuarioId,
			pagina,
			tamanoPagina,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// GET: api/Reservacion/5
	[HttpGet("{id:int}")]
	public async Task<IActionResult> Obtener(
		int id,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _reservacionLN.ObtenerAsync(
			id,
			usuarioId,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// POST: api/Reservacion
	[HttpPost]
	public async Task<IActionResult> Insertar(
		[FromBody] TCrearReservacion datos,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _reservacionLN.InsertarAsync(
			datos,
			usuarioId,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// PUT: api/Reservacion
	[HttpPut]
	public async Task<IActionResult> Modificar(
		[FromBody] TModificarReservacion datos,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _reservacionLN.ModificarAsync(
			datos,
			usuarioId,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// PATCH: api/Reservacion/5/cancelar?motivo=...
	[HttpPatch("{reservacionId:int}/cancelar")]
	public async Task<IActionResult> Cancelar(
		int reservacionId,
		[FromQuery] string motivo,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _reservacionLN.CancelarAsync(
			reservacionId,
			motivo,
			usuarioId,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// GET: api/Reservacion/disponibilidad
	[HttpGet("disponibilidad")]
	public async Task<IActionResult> BuscarDisponibilidad(
		[FromQuery] TConsultaDisponibilidad datos,
		[FromQuery] int pagina = 1,
		[FromQuery] int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		var resultado = await _reservacionLN.BuscarDisponibilidadAsync(
			datos,
			pagina,
			tamanoPagina,
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
