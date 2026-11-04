using GestionEnvios.Api.Dominio.Constantes;
using GestionEnvios.Api.Dominio.Modelos;
using GestionEnvios.Api.Infraestructura.Datos;
using GestionEnvios.Api.Infraestructura.Servicios;
using GestionEnvios.Api.ModelosTransferencia;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionEnvios.Api.Controladores;

[ApiController]
[Route("api/v1/envios")]
public class EnviosControlador(
    ContextoBaseDatos contexto,
    IServicioTransportadora servicioTransportadora) : ControllerBase
{
    // ── GET /api/v1/envios ─────────────────────────────────────────────────
    [HttpGet]
    [ProducesResponseType(typeof(List<ResumenEnvio>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarEnvios(
        [FromQuery] int pagina = 1,
        [FromQuery] int porPagina = 20)
    {
        var envios = await contexto.Envios
            .AsNoTracking()
            .OrderByDescending(e => e.FechaCreacion)
            .Skip((pagina - 1) * porPagina)
            .Take(porPagina)
            .Select(e => new ResumenEnvio
            {
                EnvioId = e.Id,
                NumeroGuia = e.NumeroGuia,
                Transportadora = e.Transportadora,
                Estado = e.Estado,
                Origen = $"{e.CiudadRemitente}, {e.EstadoRemitente}",
                Destino = $"{e.CiudadDestinatario}, {e.EstadoDestinatario}",
                FechaCreacion = e.FechaCreacion
            })
            .ToListAsync();

        return Ok(envios);
    }

    // ── POST /api/v1/envios/generar-etiqueta ───────────────────────────
    [HttpPost("generar-etiqueta")]
    [ProducesResponseType(typeof(RespuestaEnvioCreado), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GenerarEtiqueta([FromBody] SolicitudCrearEnvio solicitud)
    {
        var envio = new Envio
        {
            Id = Guid.NewGuid(),
            Transportadora = solicitud.Transportadora.ToUpper(),
            Estado = EstadosEnvio.EtiquetaGenerada,
            FechaCreacion = DateTime.UtcNow,

            NombreRemitente = solicitud.Remitente.NombreCompleto,
            CalleRemitente = solicitud.Remitente.Calle,
            CiudadRemitente = solicitud.Remitente.Ciudad,
            EstadoRemitente = solicitud.Remitente.Estado,
            CodigoPostalRemitente = solicitud.Remitente.CodigoPostal,
            TelefonoRemitente = solicitud.Remitente.Telefono,

            NombreDestinatario = solicitud.Destinatario.NombreCompleto,
            CalleDestinatario = solicitud.Destinatario.Calle,
            CiudadDestinatario = solicitud.Destinatario.Ciudad,
            EstadoDestinatario = solicitud.Destinatario.Estado,
            CodigoPostalDestinatario = solicitud.Destinatario.CodigoPostal,
            TelefonoDestinatario = solicitud.Destinatario.Telefono,

            PesoKilos = solicitud.Paquete.PesoKilos,
            LargoCentimetros = solicitud.Paquete.LargoCentimetros,
            AnchoCentimetros = solicitud.Paquete.AnchoCentimetros,
            AltoCentimetros = solicitud.Paquete.AltoCentimetros
        };

        var (numeroGuia, etiquetaPdf) = await servicioTransportadora.GenerarGuiaYEtiquetaAsync(envio);
        envio.NumeroGuia = numeroGuia;
        envio.EtiquetaPdfBytes = etiquetaPdf;

        envio.Historial.Add(new HistorialEnvio
        {
            EnvioId = envio.Id,
            Estado = EstadosEnvio.EtiquetaGenerada,
            Descripcion = "Etiqueta generada por la API pública.",
            FechaRegistro = DateTime.UtcNow
        });

        contexto.Envios.Add(envio);
        await contexto.SaveChangesAsync();

        var respuesta = new RespuestaEnvioCreado
        {
            EnvioId = envio.Id,
            NumeroGuia = envio.NumeroGuia,
            Transportadora = envio.Transportadora,
            Estado = envio.Estado,
            FechaCreacion = envio.FechaCreacion,
            EnlaceDescargaEtiqueta = $"/api/v1/envios/{envio.Id}/descargar-etiqueta"
        };

        return CreatedAtAction(nameof(DescargarEtiqueta), new { id = envio.Id }, respuesta);
    }

    // ── POST /api/v1/envios/{id}/entregar-transportadora ──────────────
    [HttpPost("{id}/entregar-transportadora")]
    [ProducesResponseType(typeof(RespuestaEntregaTransportadora), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EntregarTransportadora(Guid id, [FromBody] SolicitudEntregaTransportadora solicitud)
    {
        var envio = await contexto.Envios.FindAsync(id);
        if (envio is null)
            return NotFound(new { mensaje = "Envío no encontrado." });

        if (envio.Estado != EstadosEnvio.EtiquetaGenerada)
            return BadRequest(new { mensaje = $"El envío ya se encuentra en estado '{envio.Estado}'." });

        var codigoRecoleccion = await servicioTransportadora
            .AgendarRecoleccionAsync(envio.NumeroGuia, envio.Transportadora, solicitud.Notas);

        envio.Estado = EstadosEnvio.EnTransito;
        envio.CodigoConfirmacionRecoleccion = codigoRecoleccion;
        envio.FechaEntregaTransportadora = DateTime.UtcNow;

        contexto.HistorialEnvios.Add(new HistorialEnvio
        {
            EnvioId = envio.Id,
            Estado = EstadosEnvio.EnTransito,
            Descripcion = $"Paquete entregado a la transportadora. Método: {solicitud.MetodoEntrega}. Contacto: {solicitud.PersonaContacto}.",
            FechaRegistro = DateTime.UtcNow
        });

        await contexto.SaveChangesAsync();

        return Ok(new RespuestaEntregaTransportadora
        {
            EnvioId = envio.Id,
            NumeroGuia = envio.NumeroGuia,
            Estado = envio.Estado,
            CodigoConfirmacionRecoleccion = codigoRecoleccion,
            FechaEntregaTransportadora = envio.FechaEntregaTransportadora!.Value,
            Mensaje = "El paquete ha sido recibido formalmente por la transportadora."
        });
    }

    // ── GET /api/v1/envios/rastrear/{numeroGuia} ───────────────────────
    [HttpGet("rastrear/{numeroGuia}")]
    [ProducesResponseType(typeof(RespuestaRastreoEnvio), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RastrearEnvio(string numeroGuia)
    {
        var envio = await contexto.Envios
            .Include(e => e.Historial)
            .FirstOrDefaultAsync(e => e.NumeroGuia == numeroGuia);

        if (envio is null)
            return NotFound(new { mensaje = $"No se encontró ningún envío con la guía '{numeroGuia}'." });

        return Ok(new RespuestaRastreoEnvio
        {
            NumeroGuia = envio.NumeroGuia,
            Transportadora = envio.Transportadora,
            EstadoActual = envio.Estado,
            Origen = $"{envio.CiudadRemitente}, {envio.EstadoRemitente}",
            Destino = $"{envio.CiudadDestinatario}, {envio.EstadoDestinatario}",
            HistorialEventos = envio.Historial
                .OrderBy(h => h.FechaRegistro)
                .Select(h => new EventoHistorial
                {
                    Estado = h.Estado,
                    Descripcion = h.Descripcion,
                    Fecha = h.FechaRegistro
                }).ToList()
        });
    }

    // ── GET /api/v1/envios/{id}/descargar-etiqueta ─────────────────────
    [HttpGet("{id}/descargar-etiqueta")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DescargarEtiqueta(Guid id)
    {
        var envio = await contexto.Envios.FindAsync(id);

        if (envio is null || envio.EtiquetaPdfBytes.Length == 0)
            return NotFound(new { mensaje = "Etiqueta no encontrada." });

        return File(envio.EtiquetaPdfBytes, "application/pdf", $"etiqueta-{envio.NumeroGuia}.pdf");
    }
}