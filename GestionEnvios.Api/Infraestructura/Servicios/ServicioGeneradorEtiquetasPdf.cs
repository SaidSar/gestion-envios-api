using GestionEnvios.Api.Dominio.Modelos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;
using ZXing;
using ZXing.Common;
using System.Runtime.InteropServices;

namespace GestionEnvios.Api.Infraestructura.Servicios;

public class ServicioGeneradorEtiquetasPdf
{
    public byte[] GenerarEtiqueta(Envio envio)
    {
        var codigoBarrasBytes = GenerarCodigoBarrasPng(envio.NumeroGuia);

        return Document.Create(contenedor =>
        {
            contenedor.Page(pagina =>
            {
                pagina.Size(PageSizes.A6);
                pagina.Margin(1, Unit.Centimetre);
                pagina.DefaultTextStyle(t => t.FontSize(10));

                pagina.Content().Column(col =>
                {
                    col.Item()
                       .Background("#1a237e")
                       .Padding(10)
                       .AlignCenter()
                       .Text(envio.Transportadora)
                       .FontSize(22).Bold().FontColor(Colors.White);

                    col.Item().PaddingVertical(6);

                    col.Item()
                       .Border(1).BorderColor("#bdbdbd")
                       .Padding(8)
                       .Column(c =>
                       {
                           c.Item().Text("NÚMERO DE GUÍA").FontSize(8).FontColor("#9e9e9e");
                           c.Item().Text(envio.NumeroGuia).FontSize(15).Bold();
                       });

                    col.Item().PaddingVertical(6);

                    col.Item().Row(fila =>
                    {
                        fila.RelativeItem()
                            .Border(1).BorderColor("#e0e0e0")
                            .Padding(6)
                            .Column(c =>
                            {
                                c.Item().Text("DE / REMITENTE").FontSize(7).Bold().FontColor("#9e9e9e");
                                c.Item().Text(envio.NombreRemitente).FontSize(9).Bold();
                                c.Item().Text(envio.CalleRemitente).FontSize(8);
                                c.Item().Text($"{envio.CiudadRemitente}, {envio.EstadoRemitente}").FontSize(8);
                                c.Item().Text($"C.P. {envio.CodigoPostalRemitente}").FontSize(8);
                                c.Item().Text(envio.TelefonoRemitente).FontSize(8).FontColor("#616161");
                            });

                        fila.ConstantItem(6);

                        fila.RelativeItem()
                            .Background("#e8eaf6")
                            .Border(1).BorderColor("#9fa8da")
                            .Padding(6)
                            .Column(c =>
                            {
                                c.Item().Text("PARA / DESTINATARIO").FontSize(7).Bold().FontColor("#3949ab");
                                c.Item().Text(envio.NombreDestinatario).FontSize(9).Bold();
                                c.Item().Text(envio.CalleDestinatario).FontSize(8);
                                c.Item().Text($"{envio.CiudadDestinatario}, {envio.EstadoDestinatario}").FontSize(8);
                                c.Item().Text($"C.P. {envio.CodigoPostalDestinatario}").FontSize(8);
                                c.Item().Text(envio.TelefonoDestinatario).FontSize(8).FontColor("#3949ab");
                            });
                    });

                    col.Item().PaddingVertical(6);

                    col.Item()
                       .Background("#fafafa")
                       .Border(1).BorderColor("#e0e0e0")
                       .Padding(8)
                       .Row(fila =>
                       {
                           fila.RelativeItem().Column(c =>
                           {
                               c.Item().Text("PESO").FontSize(7).FontColor("#9e9e9e");
                               c.Item().Text($"{envio.PesoKilos} kg").FontSize(13).Bold();
                           });
                           fila.ConstantItem(1).Background("#e0e0e0");
                           fila.RelativeItem().PaddingLeft(8).Column(c =>
                           {
                               c.Item().Text("DIMENSIONES").FontSize(7).FontColor("#9e9e9e");
                               c.Item().Text($"{envio.LargoCentimetros}×{envio.AnchoCentimetros}×{envio.AltoCentimetros} cm")
                                       .FontSize(10).Bold();
                           });
                       });

                    col.Item().PaddingVertical(6);

                    // ── Código de barras REAL ──────────────────────────
                    col.Item()
                       .Border(1).BorderColor("#bdbdbd")
                       .Padding(10)
                       .Column(c =>
                       {
                           c.Item().AlignCenter().Image(codigoBarrasBytes).FitWidth();
                           c.Item().AlignCenter()
                                   .Text(envio.NumeroGuia)
                                   .FontSize(8).FontColor("#616161");
                       });

                    col.Item().PaddingVertical(4);

                    col.Item()
                       .AlignCenter()
                       .Text($"Generado el {envio.FechaCreacion:dd/MM/yyyy HH:mm} UTC  •  Nexus Code™")
                       .FontSize(7).FontColor("#9e9e9e");
                });
            });
        }).GeneratePdf();
    }

    // ── Genera Code128 como PNG usando ZXing + SkiaSharp ──────────────
    private static byte[] GenerarCodigoBarrasPng(string contenido)
    {
        var escritor = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.CODE_128,
            Options = new EncodingOptions
            {
                Width = 350,
                Height = 70,
                Margin = 4,
                PureBarcode = true   // sin texto debajo (lo ponemos nosotros)
            }
        };

        var pixelData = escritor.Write(contenido);

        var info = new SKImageInfo(pixelData.Width, pixelData.Height,
                                   SKColorType.Bgra8888, SKAlphaType.Opaque);
        using var bitmap = new SKBitmap(info);
        Marshal.Copy(pixelData.Pixels, 0, bitmap.GetPixels(), pixelData.Pixels.Length);

        using var image = SKImage.FromBitmap(bitmap);
        using var encoded = image.Encode(SKEncodedImageFormat.Png, 100);
        return encoded.ToArray();
    }
}