
using Hotel.Dominio.InterfazLN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Usuarios.Gestionar")]
public class PermisoController : ControllerBase
{
	private readonly IPermisoLN _permisoLN;

	public PermisoController(IPermisoLN permisoLN)
	{
		_permisoLN = permisoLN;
	}

	[HttpGet]
	public async Task<IActionResult> Listar(
		[FromQuery] int pagina = 1,
		[FromQuery] int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _permisoLN.ListarAsync(
			usuarioId,
			pagina,
			tamanoPagina,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	[HttpGet("{id:int}")]
	public async Task<IActionResult> Obtener(
		int id,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _permisoLN.ObtenerAsync(
			usuarioId,
			id,
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
