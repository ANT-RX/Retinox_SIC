using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Retinox.Model;
using System;
using System.IO;

namespace Retinox.Services
{
    public class PdfGeneratorService
    {
        public string GenerarReporte(Paciente paciente, string rutaImagen, string resultadoPrediccion)
        {
            try
            {
                // Crear un nuevo documento PDF
                PdfDocument document = new PdfDocument();
                document.Info.Title = $"Reporte_Retinopatía_{paciente.Nombre}";

                // Crear una página en el documento
                PdfPage page = document.AddPage();

                // Obtener el objeto gráfico para dibujar
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Fuentes
                XFont fontTitulo = new XFont("Arial", 20, XFontStyleEx.Bold);
                XFont fontSubtitulo = new XFont("Arial", 14, XFontStyleEx.Bold);
                XFont fontNormal = new XFont("Arial", 12, XFontStyleEx.Regular);

                // Dibujar el título
                gfx.DrawString("Reporte de Análisis de Retinopatía (Retinox)", fontTitulo, XBrushes.DarkBlue,
                  new XRect(0, 40, page.Width, 50),
                  XStringFormats.Center);

                // Dibujar línea separadora
                XPen pen = new XPen(XColors.Black, 2);
                gfx.DrawLine(pen, 40, 90, page.Width - 40, 90);

                // Dibujar Información del Paciente
                gfx.DrawString("Información del Paciente", fontSubtitulo, XBrushes.Black, new XRect(40, 110, page.Width, 20), XStringFormats.TopLeft);
                gfx.DrawString($"Nombre: {paciente.Nombre}", fontNormal, XBrushes.Black, new XRect(40, 140, page.Width, 20), XStringFormats.TopLeft);
                gfx.DrawString($"NSS: {paciente.NSS}", fontNormal, XBrushes.Black, new XRect(40, 160, page.Width, 20), XStringFormats.TopLeft);
                gfx.DrawString($"Edad: {paciente.Edad} años", fontNormal, XBrushes.Black, new XRect(40, 180, page.Width, 20), XStringFormats.TopLeft);
                gfx.DrawString($"Fecha del análisis: {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}", fontNormal, XBrushes.Black, new XRect(40, 200, page.Width, 20), XStringFormats.TopLeft);

                // Dibujar Resultado Multi-línea Dinámico
                int currentY = 240;
                gfx.DrawString("Distribución de Probabilidades (Inteligencia Artificial):", fontSubtitulo, XBrushes.Black, new XRect(40, currentY, page.Width, 20), XStringFormats.TopLeft);
                currentY += 30;

                string[] lineas = resultadoPrediccion.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string linea in lineas)
                {
                    gfx.DrawString($"• {linea}", new XFont("Arial", 14, XFontStyleEx.Bold), XBrushes.DarkRed, new XRect(60, currentY, page.Width, 20), XStringFormats.TopLeft);
                    currentY += 25;
                }

                currentY += 20;

                // Dibujar Imagen (si existe)
                if (!string.IsNullOrEmpty(rutaImagen) && File.Exists(rutaImagen))
                {
                    gfx.DrawString("Imagen Analizada:", fontSubtitulo, XBrushes.Black, new XRect(40, currentY, page.Width, 20), XStringFormats.TopLeft);
                    currentY += 30;
                    
                    XImage image = XImage.FromFile(rutaImagen);
                    
                    // Ajustar tamaño para que quepa bien en el centro inferior
                    double scale = 224.0 / image.PixelWidth; // Asumiendo que queremos que mida unos 224px en el PDF o similar
                    double width = image.PixelWidth * scale;
                    double height = image.PixelHeight * scale;

                    gfx.DrawImage(image, 40, currentY, 300, (300.0 / image.PixelWidth) * image.PixelHeight); // Ajustado a 300px ancho
                }

                // Generar ruta de guardado en el Escritorio
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string fileName = $"Reporte_{paciente.Nombre.Replace(" ", "_")}_{DateTime.Now.ToString("ddMMyyyy_HHmm")}.pdf";
                string fullPath = Path.Combine(desktopPath, fileName);

                // Guardar el documento
                document.Save(fullPath);
                
                return $"El reporte se generó correctamente en el Escritorio:\n{fileName}";
            }
            catch (Exception ex)
            {
                return $"Error al generar el PDF: {ex.Message}";
            }
        }
    }
}
