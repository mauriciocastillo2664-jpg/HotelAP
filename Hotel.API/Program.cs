using Hotel.AccesoDatos.Contexto;
using Hotel.AccesoDatos.Implementaciones;
using Hotel.API.Middleware;
using Hotel.Dominio.InterfacesAD;
using Hotel.Dominio.DTO;
using Hotel.Dominio.InterfazLN;
using Hotel.LogicaNegocio;
using Hotel.LogicaNegocio.Configuracion;
using Hotel.LogicaNegocio.Implementaciones;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HotelContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Falta configurar ConnectionStrings:DefaultConnection.");

    options.UseSqlServer(connectionString);
});

builder.Services.AddAutoMapper(cfg => { }, typeof(AutoMapperProfile).Assembly);

builder.Services.AddScoped<IUnidadTrabajoEF, UnidadTrabajoEF>();
builder.Services.AddScoped<ITipoHabitacionLN, TipoHabitacionLN>();
builder.Services.AddScoped<ICategoriaPlatilloLN, CategoriaPlatilloLN>();
builder.Services.AgregarCatalogosLN();
builder.Services.AgregarSeguridadLN();
builder.Services.AddHospedajeLogicaNegocio();
builder.Services.AddRestauranteFinanzasLogicaNegocio();
builder.Services.AddScoped<IHospedajeAD, HospedajeAD>();
builder.Services.AddScoped<IEstadoOperativoAD, EstadoOperativoAD>();
builder.Services.AddScoped<IRestauranteFinanzasAD, RestauranteFinanzasAD>();

var issuer = builder.Configuration["Jwt:Issuer"];
var audience = builder.Configuration["Jwt:Audience"];
var signingKey = builder.Configuration["Jwt:Key"];
var jwtConfigured = !string.IsNullOrWhiteSpace(issuer)
    && !string.IsNullOrWhiteSpace(audience)
    && !string.IsNullOrWhiteSpace(signingKey)
    && Encoding.UTF8.GetByteCount(signingKey) >= 32;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ValidateIssuerSigningKey = true,
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
            ClockSkew = TimeSpan.FromSeconds(30)
        };

        if (jwtConfigured)
        {
            options.TokenValidationParameters.IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey!));
        }

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (!jwtConfigured)
                {
                    context.Fail("La autenticación todavía requiere configuración del servidor.");
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Habitaciones.Editar", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("permission", "Habitaciones.Editar");
    });

    options.AddPolicy("Menu.Editar", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("permission", "Menu.Editar");
    });

	options.AddPolicy("Clientes.Leer", policy =>
	{
		policy.RequireAuthenticatedUser();
		policy.RequireClaim("permission", "Clientes.Leer");
	});

	options.AddPolicy("Clientes.Editar", policy =>
	{
		policy.RequireAuthenticatedUser();
		policy.RequireClaim("permission", "Clientes.Editar");
	});


	options.AddPolicy("Habitaciones.CambiarEstado", policy =>
	{
		policy.RequireAuthenticatedUser();
		policy.RequireClaim("permission", "Habitaciones.CambiarEstado");
	});

	options.AddPolicy("Parametros.Editar", policy =>
	{
		policy.RequireAuthenticatedUser();
		policy.RequireClaim("permission", "Parametros.Editar");
	});

	options.AddPolicy("Usuarios.Gestionar", policy =>
	{
		policy.RequireAuthenticatedUser();
		policy.RequireClaim("permission", "Usuarios.Gestionar");
	});

});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMiddleware<ExcepcionesMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
