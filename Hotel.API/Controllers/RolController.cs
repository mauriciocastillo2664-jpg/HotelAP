
using Hotel.Dominio.InterfazLN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Usuarios.Gestionar")]
public class RolController : ControllerBase
{
	private readonly IRolLN _rolLN;

	public RolController(IRolLN rolLN)
	{
		_rolLN = rolLN;
	}

	// GET: api/Rol
	[HttpGet]
	public async Task<IActionResult> Listar(
		[FromQuery] int pagina = 1,
		[FromQuery] int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _rolLN.ListarAsync(
			usuarioId,
			pagina,
			tamanoPagina,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// GET: api/Rol/5
	[HttpGet("{id:int}")]
	public async Task<IActionResult> Obtener(
		int id,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _rolLN.ObtenerAsync(
			usuarioId,
			id,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// POST: api/Rol/5/permisos/2
	[HttpPost("{rolId:int}/permisos/{permisoId:int}")]
	public async Task<IActionResult> AsignarPermiso(
		int rolId,
		int permisoId,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _rolLN.AsignarPermisoAsync(
			usuarioId,
			rolId,
			permisoId,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// DELETE: api/Rol/5/permisos/2
	[HttpDelete("{rolId:int}/permisos/{permisoId:int}")]
	public async Task<IActionResult> QuitarPermiso(
		int rolId,
		int permisoId,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _rolLN.QuitarPermisoAsync(
			usuarioId,
			rolId,
			permisoId,
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
