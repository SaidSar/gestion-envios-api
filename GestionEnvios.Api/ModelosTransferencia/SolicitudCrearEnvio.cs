using System.ComponentModel.DataAnnotations;

namespace GestionEnvios.Api.ModelosTransferencia;

public class SolicitudCrearEnvio
{
    [Required(ErrorMessage = "La transportadora es obligatoria.")]
    [MaxLength(20, ErrorMessage = "Máximo 20 caracteres.")]
    public string Transportadora { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los datos del remitente son obligatorios.")]
    public DatosPersona Remitente { get; set; } = new();

    [Required(ErrorMessage = "Los datos del destinatario son obligatorios.")]
    public DatosPersona Destinatario { get; set; } = new();

    [Required(ErrorMessage = "Los datos del paquete son obligatorios.")]
    public DatosPaquete Paquete { get; set; } = new();
}

public class DatosPersona
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(100)]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "La calle es obligatoria.")]
    [MaxLength(200)]
    public string Calle { get; set; } = string.Empty;

    [Required(ErrorMessage = "La ciudad es obligatoria.")]
    [MaxLength(100)]
    public string Ciudad { get; set; } = string.Empty;

    [Required(ErrorMessage = "El estado es obligatorio.")]
    [MaxLength(100)]
    public string Estado { get; set; } = string.Empty;

    [Required(ErrorMessage = "El código postal es obligatorio.")]
    [RegularExpression(@"^\d{5}$", ErrorMessage = "El código postal debe tener 5 dígitos.")]
    public string CodigoPostal { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [MaxLength(20)]
    public string Telefono { get; set; } = string.Empty;
}

public class DatosPaquete
{
    [Range(0.1, 999.9, ErrorMessage = "El peso debe estar entre 0.1 y 999.9 kg.")]
    public decimal PesoKilos { get; set; }

    [Range(1, 999, ErrorMessage = "El largo debe estar entre 1 y 999 cm.")]
    public decimal LargoCentimetros { get; set; }

    [Range(1, 999, ErrorMessage = "El ancho debe estar entre 1 y 999 cm.")]
    public decimal AnchoCentimetros { get; set; }

    [Range(1, 999, ErrorMessage = "El alto debe estar entre 1 y 999 cm.")]
    public decimal AltoCentimetros { get; set; }
}