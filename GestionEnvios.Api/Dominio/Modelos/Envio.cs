namespace GestionEnvios.Api.Dominio.Modelos;

public class Envio
{
    public Guid Id { get; set; }
    public string NumeroGuia { get; set; } = string.Empty;
    public string Transportadora { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;

    // Remitente
    public string NombreRemitente { get; set; } = string.Empty;
    public string CalleRemitente { get; set; } = string.Empty;
    public string CiudadRemitente { get; set; } = string.Empty;
    public string EstadoRemitente { get; set; } = string.Empty;
    public string CodigoPostalRemitente { get; set; } = string.Empty;
    public string TelefonoRemitente { get; set; } = string.Empty;

    // Destinatario
    public string NombreDestinatario { get; set; } = string.Empty;
    public string CalleDestinatario { get; set; } = string.Empty;
    public string CiudadDestinatario { get; set; } = string.Empty;
    public string EstadoDestinatario { get; set; } = string.Empty;
    public string CodigoPostalDestinatario { get; set; } = string.Empty;
    public string TelefonoDestinatario { get; set; } = string.Empty;

    // Dimensiones
    public decimal PesoKilos { get; set; }
    public decimal LargoCentimetros { get; set; }
    public decimal AnchoCentimetros { get; set; }
    public decimal AltoCentimetros { get; set; }

    // Archivos y recolección
    public byte[] EtiquetaPdfBytes { get; set; } = Array.Empty<byte>();
    public string? CodigoConfirmacionRecoleccion { get; set; }

    // Fechas
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaEntregaTransportadora { get; set; }

    // Navegación
    public ICollection<HistorialEnvio> Historial { get; set; } = new List<HistorialEnvio>();
}