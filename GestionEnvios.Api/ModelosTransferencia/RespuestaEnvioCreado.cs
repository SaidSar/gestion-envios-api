namespace GestionEnvios.Api.ModelosTransferencia;

public class RespuestaEnvioCreado
{
    public Guid EnvioId { get; set; }
    public string NumeroGuia { get; set; } = string.Empty;
    public string Transportadora { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public string EnlaceDescargaEtiqueta { get; set; } = string.Empty;
}