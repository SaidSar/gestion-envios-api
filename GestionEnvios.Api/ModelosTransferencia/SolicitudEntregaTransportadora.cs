using System.ComponentModel.DataAnnotations;

namespace GestionEnvios.Api.ModelosTransferencia;

public class SolicitudEntregaTransportadora
{
    [Required(ErrorMessage = "El método de entrega es obligatorio.")]
    [MaxLength(50)]
    public string MetodoEntrega { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Notas { get; set; } = string.Empty;

    [Required(ErrorMessage = "La persona de contacto es obligatoria.")]
    [MaxLength(100)]
    public string PersonaContacto { get; set; } = string.Empty;
}