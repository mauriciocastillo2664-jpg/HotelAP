
using Hotel.Dominio.InterfazLN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CargoAdicionalController : ControllerBase
{
	private readonly ICargoAdicionalLN _cargoAdicionalLN;

	public CargoAdicionalController(ICargoAdicionalLN cargoAdicionalLN)
	{
		_cargoAdicionalLN = cargoAdicionalLN;
	}

	// GET: api/CargoAdicional/5
	[HttpGet("{reservacionId:int}")]
	public async Task<IActionResult> Listar(
		int reservacionId,
		[FromQuery] int pagina = 1,
		[FromQuery] int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _cargoAdicionalLN.ListarAsync(
			reservacionId,
			usuarioId,
			pagina,
			tamanoPagina,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// POST: api/CargoAdicional/5
	[HttpPost("{reservacionId:int}")]
	public async Task<IActionResult> Registrar(
		int reservacionId,
		[FromQuery] string concepto,
		[FromQuery] decimal monto,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _cargoAdicionalLN.RegistrarAsync(
			reservacionId,
			concepto,
			monto,
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
