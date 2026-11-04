namespace GestionEnvios.Api.ModelosTransferencia;

public class RespuestaRastreoEnvio
{
    public string NumeroGuia { get; set; } = string.Empty;
    public string Transportadora { get; set; } = string.Empty;
    public string EstadoActual { get; set; } = string.Empty;
    public string Origen { get; set; } = string.Empty;
    public string Destino { get; set; } = string.Empty;
    public List<EventoHistorial> HistorialEventos { get; set; } = new();
}

public class EventoHistorial
{
    public string Estado { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
}