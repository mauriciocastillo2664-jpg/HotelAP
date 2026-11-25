
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfazLN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HabitacionController : ControllerBase
{
	private readonly IHabitacionLN _logicaNegocio;

	public HabitacionController(IHabitacionLN logicaNegocio)
	{
		_logicaNegocio = logicaNegocio;
	}

	// GET: api/Habitacion/Listar
	[HttpGet("Listar")]
	[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
	public async Task<IActionResult> Listar(
		int pagina = 1,
		int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		var resultado = await _logicaNegocio.ListarAsync(
			pagina, tamanoPagina, cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// GET: api/Habitacion/Buscar?texto=101
	[HttpGet("Buscar")]
	[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
	public async Task<IActionResult> Buscar(
		string texto = "",
		int pagina = 1,
		int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		var resultado = await _logicaNegocio.BuscarAsync(
			texto, pagina, tamanoPagina, cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// GET: api/Habitacion/Obtener/1
	[HttpGet("Obtener/{id:int}")]
	[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
	public async Task<IActionResult> Obtener(
		int id,
		CancellationToken cancellationToken = default)
	{
		var resultado = await _logicaNegocio.ObtenerAsync(
			id, cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// POST: api/Habitacion/Insertar
	[HttpPost("Insertar")]
	[Authorize(Policy = "Habitaciones.Editar")]
	public async Task<IActionResult> Insertar(
		[FromBody] THabitacion datos,
		CancellationToken cancellationToken = default)
	{
		var resultado = await _logicaNegocio.InsertarAsync(
			datos, cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// PUT: api/Habitacion/Modificar
	[HttpPut("Modificar")]
	[Authorize(Policy = "Habitaciones.Editar")]
	public async Task<IActionResult> Modificar(
		[FromBody] THabitacion datos,
		CancellationToken cancellationToken = default)
	{
		var resultado = await _logicaNegocio.ModificarAsync(
			datos, cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// DELETE: api/Habitacion/Eliminar/1
	[HttpDelete("Eliminar/{id:int}")]
	[Authorize(Policy = "Habitaciones.Editar")]
	public async Task<IActionResult> Eliminar(
		int id,
		CancellationToken cancellationToken = default)
	{
		var resultado = await _logicaNegocio.EliminarAsync(
			id, cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// GET: api/Habitacion/1/Amenidades
	[HttpGet("{habitacionId:int}/Amenidades")]
	[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
	public async Task<IActionResult> ListarAmenidades(
		int habitacionId,
		CancellationToken cancellationToken = default)
	{
		var resultado = await _logicaNegocio.ListarAmenidadesAsync(
			habitacionId, cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// POST: api/Habitacion/1/Amenidades/2
	[HttpPost("{habitacionId:int}/Amenidades/{amenidadId:int}")]
	[Authorize(Policy = "Habitaciones.Editar")]
	public async Task<IActionResult> AsignarAmenidad(
		int habitacionId,
		int amenidadId,
		CancellationToken cancellationToken = default)
	{
		if (!TryObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _logicaNegocio.AsignarAmenidadAsync(
			habitacionId, amenidadId, usuarioId, cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// DELETE: api/Habitacion/1/Amenidades/2
	[HttpDelete("{habitacionId:int}/Amenidades/{amenidadId:int}")]
	[Authorize(Policy = "Habitaciones.Editar")]
	public async Task<IActionResult> QuitarAmenidad(
		int habitacionId,
		int amenidadId,
		CancellationToken cancellationToken = default)
	{
		if (!TryObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _logicaNegocio.QuitarAmenidadAsync(
			habitacionId, amenidadId, usuarioId, cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// POST: api/Habitacion/1/CambiarEstado?estadoId=2&motivo=Mantenimiento
	[HttpPost("{habitacionId:int}/CambiarEstado")]
	[Authorize(Policy = "Habitaciones.CambiarEstado")]
	public async Task<IActionResult> CambiarEstado(
		int habitacionId,
		[FromQuery] int estadoId,
		[FromQuery] string motivo,
		CancellationToken cancellationToken = default)
	{
		if (!TryObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _logicaNegocio.CambiarEstadoAsync(
			habitacionId, estadoId, motivo, usuarioId, cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// Obtiene el ID del usuario desde el token JWT.
	private bool TryObtenerUsuarioId(out int usuarioId)
	{
		return int.TryParse(
				   User.FindFirst("sub")?.Value,
				   out usuarioId)
			   && usuarioId > 0;
	}
}
