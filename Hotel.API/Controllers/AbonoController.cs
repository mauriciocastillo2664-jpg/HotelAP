
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfazLN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AbonoController : ControllerBase
{
	private readonly IAbonoLN _abonoLN;

	public AbonoController(IAbonoLN abonoLN)
	{
		_abonoLN = abonoLN;
	}

	// GET: api/Abono/pago/5
	[HttpGet("pago/{pagoId:int}")]
	public async Task<IActionResult> ListarPorPago(
		int pagoId,
		[FromQuery] int pagina = 1,
		[FromQuery] int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _abonoLN.ListarPorPagoAsync(
			pagoId,
			usuarioId,
			pagina,
			tamanoPagina,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// GET: api/Abono/5
	[HttpGet("{id:int}")]
	public async Task<IActionResult> Obtener(
		int id,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _abonoLN.ObtenerAsync(
			id,
			usuarioId,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// POST: api/Abono
	[HttpPost]
	public async Task<IActionResult> Registrar(
		[FromBody] TRegistrarAbono datos,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _abonoLN.RegistrarAsync(
			datos,
			usuarioId,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// PATCH: api/Abono/reversar
	[HttpPatch("reversar")]
	public async Task<IActionResult> Reversar(
		[FromBody] TReversarAbono datos,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _abonoLN.ReversarAsync(
			datos,
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
