using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QRCoder;
using rntlOS.Core.Models;
using System.IO;

namespace rntlOS.Core.PdfService
{
    public class ReservationPdfService
    {
        public byte[] GeneratePdf(Booking booking)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(50);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // HEADER
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text("rntlOS").FontSize(28).Bold().FontColor("#0B1A2E");
                            column.Item().Text("Bon de Réservation").FontSize(16).FontColor("#C9A961");
                        });

                        // Logo - dimensions fixes et simples
                        row.ConstantItem(100).Image(GetLogoBytes()).FitWidth();
                    });

                    // CONTENT
                    page.Content().Column(column =>
                    {
                        column.Spacing(15);

                        // Informations réservation
                        column.Item().Text("Bon de réservation").FontSize(20).Bold();
                        column.Item().Text($"Date : {booking.CreatedAt:dd/MM/yyyy}");

                        column.Item().LineHorizontal(1).LineColor("#C9A961");

                        // Client
                        column.Item().Text("INFORMATIONS CLIENT").FontSize(14).Bold().FontColor("#0B1A2E");
                        column.Item().Text($"Nom : {booking.Client.Nom} {booking.Client.Prenom}");
                        column.Item().Text($"Email : {booking.Client.Email}");
                        column.Item().Text($"Téléphone : {booking.Client.Telephone}");

                        column.Item().PaddingVertical(10);

                        // Véhicule
                        column.Item().Text("VÉHICULE LOUÉ").FontSize(14).Bold().FontColor("#0B1A2E");
                        column.Item().Text($"Marque : {booking.Vehicule.Marque}");
                        column.Item().Text($"Modèle : {booking.Vehicule.Modele}");
                        column.Item().Text($"Matricule : {booking.Vehicule.Matricule}");

                        column.Item().PaddingVertical(10);

                        // Dates et prix
                        column.Item().Text("DÉTAILS DE LA LOCATION").FontSize(14).Bold().FontColor("#0B1A2E");
                        column.Item().Text($"Date début : {booking.DateDebut:dd/MM/yyyy}");
                        column.Item().Text($"Date fin : {booking.DateFin:dd/MM/yyyy}");
                        column.Item().Text($"Nombre de jours : {(booking.DateFin - booking.DateDebut).Days + 1}");
                        column.Item().Text($"Prix total : {booking.PrixTotal:C}").FontSize(16).Bold().FontColor("#C9A961");

                        column.Item().PaddingVertical(10);

                        // QR Code
                        var qrCodeBytes = GenerateQrCode($"Réservation #{booking.Id} - {booking.Client.Nom} {booking.Client.Prenom}");
                        column.Item().AlignCenter().Width(150).Image(qrCodeBytes);
                        column.Item().AlignCenter().Text("Scannez ce QR Code").FontSize(10).Italic();
                    });

                    // FOOTER
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("rntlOS - Location de véhicules | ");
                        text.Span("www.rntlos.com").FontColor("#C9A961");
                    });
                });
            });

            return document.GeneratePdf();
        }

        private byte[] GenerateQrCode(string text)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q))
            using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
            {
                return qrCode.GetGraphic(20);
            }
        }

        private byte[] GetLogoBytes()
        {
            try
            {
                string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "logo.png");

                if (File.Exists(logoPath))
                {
                    return File.ReadAllBytes(logoPath);
                }

                // Si le logo n'existe pas, retourner un pixel transparent
                return new byte[] { };
            }
            catch
            {
                return new byte[] { };
            }
        }
    }
}