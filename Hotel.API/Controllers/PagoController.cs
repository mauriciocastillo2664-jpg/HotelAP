
using Hotel.Dominio.InterfazLN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PagoController : ControllerBase
{
	private readonly IPagoLN _pagoLN;

	public PagoController(IPagoLN pagoLN)
	{
		_pagoLN = pagoLN;
	}

	// GET: api/Pago/cliente/5
	[HttpGet("cliente/{clienteId:int}")]
	public async Task<IActionResult> ListarPorCliente(
		int clienteId,
		[FromQuery] int pagina = 1,
		[FromQuery] int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _pagoLN.ListarPorClienteAsync(
			clienteId,
			usuarioId,
			pagina,
			tamanoPagina,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// GET: api/Pago/5
	[HttpGet("{id:int}")]
	public async Task<IActionResult> Obtener(
		int id,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _pagoLN.ObtenerAsync(
			id,
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
