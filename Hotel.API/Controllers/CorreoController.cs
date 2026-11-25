
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfazLN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CorreoController : ControllerBase
{
	private readonly ICorreoLN _correoLN;

	public CorreoController(ICorreoLN correoLN)
	{
		_correoLN = correoLN;
	}

	// GET: api/Correo/cliente/5
	[HttpGet("cliente/{clienteId:int}")]
	public async Task<IActionResult> ListarPorCliente(
		int clienteId,
		[FromQuery] int pagina = 1,
		[FromQuery] int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _correoLN.ListarPorClienteAsync(
			clienteId,
			usuarioId,
			pagina,
			tamanoPagina,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// GET: api/Correo/5
	[HttpGet("{id:int}")]
	public async Task<IActionResult> Obtener(
		int id,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _correoLN.ObtenerAsync(
			id,
			usuarioId,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// POST: api/Correo
	[HttpPost]
	public async Task<IActionResult> Registrar(
		[FromBody] TRegistrarCorreo datos,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _correoLN.RegistrarAsync(
			datos,
			usuarioId,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// PATCH: api/Correo/estado
	[HttpPatch("estado")]
	public async Task<IActionResult> ActualizarEstado(
		[FromBody] TActualizarEstadoCorreo datos,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _correoLN.ActualizarEstadoAsync(
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
