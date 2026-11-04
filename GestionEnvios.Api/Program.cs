using GestionEnvios.Api.Infraestructura.Datos;
using GestionEnvios.Api.Infraestructura.Servicios;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Licencia gratuita para proyectos comunitarios / académicos
QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new()
    {
        Title = "API de Gestión de Envíos",
        Version = "v1",
        Description = "Microservicio público de logística — Nexus Code™"
    });
});

builder.Services.AddDbContext<ContextoBaseDatos>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("ConexionPrincipal")));

// Inyección de dependencias
builder.Services.AddScoped<IServicioTransportadora, ServicioTransportadoraSimulada>();
builder.Services.AddScoped<ServicioGeneradorEtiquetasPdf>();

// Configuración CORS
builder.Services.AddCors(opciones =>
{
    opciones.AddPolicy("AccesoPublico", politica =>
        politica.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
});

var app = builder.Build();

// Middleware CORS
app.UseCors("AccesoPublico");

// Aplicar migraciones automáticamente al iniciar en la nube o local
using (var alcance = app.Services.CreateScope())
{
    var contexto = alcance.ServiceProvider.GetRequiredService<ContextoBaseDatos>();
    contexto.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Gestión de Envíos v1");
    c.RoutePrefix = string.Empty; // Swagger abre directamente en la raíz de Azure
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();