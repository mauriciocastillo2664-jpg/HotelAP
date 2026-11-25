
using Hotel.Dominio.InterfazLN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportesController : ControllerBase
{
	private readonly IReportesLN _reportesLN;

	public ReportesController(IReportesLN reportesLN)
	{
		_reportesLN = reportesLN;
	}

	[HttpGet("estado-cuenta")]
	public async Task<IActionResult> ConsultarEstadoCuenta(
		[FromQuery] int pagina = 1,
		[FromQuery] int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _reportesLN.ConsultarEstadoCuentaAsync(
			usuarioId, pagina, tamanoPagina, cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	[HttpGet("estado-usuario")]
	public async Task<IActionResult> ConsultarEstadoUsuario(
		[FromQuery] int pagina = 1,
		[FromQuery] int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _reportesLN.ConsultarEstadoUsuarioAsync(
			usuarioId, pagina, tamanoPagina, cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	[HttpGet("factura-abono")]
	public async Task<IActionResult> ConsultarFacturaAbono(
		[FromQuery] int pagina = 1,
		[FromQuery] int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _reportesLN.ConsultarFacturaAbonoAsync(
			usuarioId, pagina, tamanoPagina, cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	[HttpGet("factura-concepto")]
	public async Task<IActionResult> ConsultarFacturaConcepto(
		[FromQuery] int pagina = 1,
		[FromQuery] int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _reportesLN.ConsultarFacturaConceptoAsync(
			usuarioId, pagina, tamanoPagina, cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	[HttpGet("historial-correo")]
	public async Task<IActionResult> ConsultarHistorialCorreo(
		[FromQuery] int pagina = 1,
		[FromQuery] int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _reportesLN.ConsultarHistorialCorreoAsync(
			usuarioId, pagina, tamanoPagina, cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	[HttpGet("ingresos-por-mes")]
	public async Task<IActionResult> ConsultarIngresosPorMes(
		[FromQuery] int pagina = 1,
		[FromQuery] int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _reportesLN.ConsultarIngresosPorMesAsync(
			usuarioId, pagina, tamanoPagina, cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	[HttpGet("ocupacion-actual")]
	public async Task<IActionResult> ConsultarOcupacionActual(
		[FromQuery] int pagina = 1,
		[FromQuery] int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _reportesLN.ConsultarOcupacionActualAsync(
			usuarioId, pagina, tamanoPagina, cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	[HttpGet("platillos-mas-vendidos")]
	public async Task<IActionResult> ConsultarPlatillosMasVendidos(
		[FromQuery] int pagina = 1,
		[FromQuery] int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _reportesLN.ConsultarPlatillosMasVendidoAsync(
			usuarioId, pagina, tamanoPagina, cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	[HttpGet("reservaciones-detalle")]
	public async Task<IActionResult> ConsultarReservacionesDetalle(
		[FromQuery] int pagina = 1,
		[FromQuery] int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _reportesLN.ConsultarReservacionesDetalleAsync(
			usuarioId, pagina, tamanoPagina, cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	[HttpGet("tiempos-preparacion")]
	public async Task<IActionResult> ConsultarTiemposPreparacion(
		[FromQuery] int pagina = 1,
		[FromQuery] int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _reportesLN.ConsultarTiemposPreparacionAsync(
			usuarioId, pagina, tamanoPagina, cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	[HttpGet("usuario-permiso")]
	public async Task<IActionResult> ConsultarUsuarioPermiso(
		[FromQuery] int pagina = 1,
		[FromQuery] int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _reportesLN.ConsultarUsuarioPermisoAsync(
			usuarioId, pagina, tamanoPagina, cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	private bool IntentarObtenerUsuarioId(out int usuarioId)
	{
		return int.TryParse(
			User.FindFirst("sub")?.Value,
			out usuarioId);
	}
}
