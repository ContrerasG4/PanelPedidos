using PanelPedidos.Api.Configuracion;
using PanelPedidos.Api.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Servicios
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configuraciones
builder.Services
    .AgregarBaseDatos(builder.Configuration)
    .AgregarJwt(builder.Configuration)
    .AgregarDependencias(builder.Configuration);

// Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<BaseDatosHealthCheck>("BaseDatos");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapHealthChecks("/health");
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();