
using Hotel.Dominio.InterfazLN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Usuarios.Gestionar")]
public class UsuarioController : ControllerBase
{
	private readonly IUsuarioLN _usuarioLN;

	public UsuarioController(IUsuarioLN usuarioLN)
	{
		_usuarioLN = usuarioLN;
	}

	// GET: api/Usuario
	[HttpGet]
	public async Task<IActionResult> Listar(
		[FromQuery] int pagina = 1,
		[FromQuery] int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _usuarioLN.ListarAsync(
			usuarioId,
			pagina,
			tamanoPagina,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// GET: api/Usuario/5
	[HttpGet("{id:int}")]
	public async Task<IActionResult> Obtener(
		int id,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _usuarioLN.ObtenerAsync(
			usuarioId,
			id,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// PATCH: api/Usuario/5/estado/2
	[HttpPatch("{usuarioDestinoId:int}/estado/{estadoId:int}")]
	public async Task<IActionResult> CambiarEstado(
		int usuarioDestinoId,
		int estadoId,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _usuarioLN.CambiarEstadoAsync(
			usuarioId,
			usuarioDestinoId,
			estadoId,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// POST: api/Usuario/5/roles/2
	[HttpPost("{usuarioDestinoId:int}/roles/{rolId:int}")]
	public async Task<IActionResult> AsignarRol(
		int usuarioDestinoId,
		int rolId,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _usuarioLN.AsignarRolAsync(
			usuarioId,
			usuarioDestinoId,
			rolId,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// DELETE: api/Usuario/5/roles/2
	[HttpDelete("{usuarioDestinoId:int}/roles/{rolId:int}")]
	public async Task<IActionResult> QuitarRol(
		int usuarioDestinoId,
		int rolId,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _usuarioLN.QuitarRolAsync(
			usuarioId,
			usuarioDestinoId,
			rolId,
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
