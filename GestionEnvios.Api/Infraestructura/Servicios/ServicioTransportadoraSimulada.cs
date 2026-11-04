using GestionEnvios.Api.Dominio.Modelos;

namespace GestionEnvios.Api.Infraestructura.Servicios;

public class ServicioTransportadoraSimulada(ServicioGeneradorEtiquetasPdf generadorPdf) : IServicioTransportadora
{
    public Task<(string NumeroGuia, byte[] EtiquetaPdf)> GenerarGuiaYEtiquetaAsync(Envio envio)
    {
        var numero = Random.Shared.Next(10_000_000, 99_999_999);
        var numeroGuia = $"GUIA-{envio.Transportadora.ToUpper()}-{numero}";

        envio.NumeroGuia = numeroGuia;                         // el PDF necesita el número
        var pdfBytes = generadorPdf.GenerarEtiqueta(envio);

        return Task.FromResult((numeroGuia, pdfBytes));
    }

    public Task<string> AgendarRecoleccionAsync(string numeroGuia, string transportadora, string notas)
    {
        var codigo = Random.Shared.Next(10_000, 99_999);
        return Task.FromResult($"REC-{transportadora.ToUpper()}-{codigo}");
    }
}