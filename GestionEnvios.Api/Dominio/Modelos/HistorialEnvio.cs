namespace GestionEnvios.Api.Dominio.Modelos;

public class HistorialEnvio
{
    public int Id { get; set; }
    public Guid EnvioId { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }

    // Navegación
    public Envio Envio { get; set; } = null!;
}