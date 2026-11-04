using GestionEnvios.Api.Dominio.Modelos;

namespace GestionEnvios.Api.Infraestructura.Servicios;

public interface IServicioTransportadora
{
    Task<(string NumeroGuia, byte[] EtiquetaPdf)> GenerarGuiaYEtiquetaAsync(Envio envio);
    Task<string> AgendarRecoleccionAsync(string numeroGuia, string transportadora, string notas);
}