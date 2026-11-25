
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfazLN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PedidoRestauranteController : ControllerBase
{
	private readonly IPedidoRestauranteLN _pedidoRestauranteLN;

	public PedidoRestauranteController(IPedidoRestauranteLN pedidoRestauranteLN)
	{
		_pedidoRestauranteLN = pedidoRestauranteLN;
	}

	// GET: api/PedidoRestaurante/cliente/5
	[HttpGet("cliente/{clienteId:int}")]
	public async Task<IActionResult> ListarPorCliente(
		int clienteId,
		[FromQuery] int pagina = 1,
		[FromQuery] int tamanoPagina = 20,
		CancellationToken cancellationToken = default)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _pedidoRestauranteLN.ListarPorClienteAsync(
			clienteId,
			usuarioId,
			pagina,
			tamanoPagina,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// GET: api/PedidoRestaurante/5
	[HttpGet("{id:int}")]
	public async Task<IActionResult> Obtener(
		int id,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _pedidoRestauranteLN.ObtenerAsync(
			id,
			usuarioId,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// POST: api/PedidoRestaurante
	[HttpPost]
	public async Task<IActionResult> Crear(
		[FromBody] TCrearPedido datos,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _pedidoRestauranteLN.CrearAsync(
			datos,
			usuarioId,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// POST: api/PedidoRestaurante/platillo
	[HttpPost("platillo")]
	public async Task<IActionResult> AgregarPlatillo(
		[FromBody] TAgregarPlatilloPedido datos,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _pedidoRestauranteLN.AgregarPlatilloAsync(
			datos,
			usuarioId,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// PATCH: api/PedidoRestaurante/estado
	[HttpPatch("estado")]
	public async Task<IActionResult> CambiarEstado(
		[FromBody] TCambiarEstadoPedido datos,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _pedidoRestauranteLN.CambiarEstadoAsync(
			datos,
			usuarioId,
			cancellationToken);

		return StatusCode(resultado.EstadoHttp, resultado);
	}

	// POST: api/PedidoRestaurante/pagar
	[HttpPost("pagar")]
	public async Task<IActionResult> Pagar(
		[FromBody] TPagarPedido datos,
		CancellationToken cancellationToken)
	{
		if (!IntentarObtenerUsuarioId(out int usuarioId))
			return Unauthorized();

		var resultado = await _pedidoRestauranteLN.PagarAsync(
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
