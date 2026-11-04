namespace GestionEnvios.Api.ModelosTransferencia;

public class RespuestaEntregaTransportadora
{
    public Guid EnvioId { get; set; }
    public string NumeroGuia { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string CodigoConfirmacionRecoleccion { get; set; } = string.Empty;
    public DateTime FechaEntregaTransportadora { get; set; }
    public string Mensaje { get; set; } = string.Empty;
}