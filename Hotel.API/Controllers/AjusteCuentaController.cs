
using Hotel.Dominio.InterfazLN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AjusteCuentaController : ControllerBase
{
	private readonly IAjusteCuentaLN _ajusteCuentaLN;

	public AjusteCuentaController(IAjusteCuentaLN ajusteCuentaLN)
	{
		_ajusteCuentaLN = ajusteCuentaLN;
	}

	// GET: api/AjusteCuenta/5
	[HttpGet("{reservacionId:int}")]
	public async Task<IActionResult> Listar(
		int reservacionId,
		[FromQuery] int pagina = 1,
		[FromQuery] int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _ajusteCuentaLN.ListarAsync(
			reservacionId,
			usuarioId,
			pagina,
			tamanoPagina,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// POST: api/AjusteCuenta/5
	[HttpPost("{reservacionId:int}")]
	public async Task<IActionResult> Registrar(
		int reservacionId,
		[FromQuery] decimal monto,
		[FromQuery] string motivo,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _ajusteCuentaLN.RegistrarAsync(
			reservacionId,
			monto,
			motivo,
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
