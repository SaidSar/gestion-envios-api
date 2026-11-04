namespace GestionEnvios.Api.ModelosTransferencia;

public class ResumenEnvio
{
    public Guid EnvioId { get; set; }
    public string NumeroGuia { get; set; } = string.Empty;
    public string Transportadora { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Origen { get; set; } = string.Empty;
    public string Destino { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}