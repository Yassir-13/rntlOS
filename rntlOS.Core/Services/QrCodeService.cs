using QRCoder;
using rntlOS.Core.Models;
using System;
using System.Text;

namespace rntlOS.Core.Services
{
    public interface IQrCodeService
    {
        byte[] GenerateBookingQrCode(Booking booking);
    }

    public class QrCodeService : IQrCodeService
    {
        public byte[] GenerateBookingQrCode(Booking booking)
        {
            // Format the content for "Offline Verification"
            // We use a StringBuilder to create a clean, readable list of details.
            var sb = new StringBuilder();
            sb.AppendLine("RNTL-OS RESERVATION");
            sb.AppendLine("-------------------");
            sb.AppendLine($"Ref: #{booking.Id}");
            
            if (booking.Client != null)
            {
                sb.AppendLine($"Client: {booking.Client.Nom} {booking.Client.Prenom}");
            }
            
            if (booking.Vehicule != null)
            {
                sb.AppendLine($"Vehicule: {booking.Vehicule.Marque} {booking.Vehicule.Modele}");
                sb.AppendLine($"Matricule: {booking.Vehicule.Matricule}");
            }

            sb.AppendLine($"Du: {booking.DateDebut:dd/MM/yyyy}");
            sb.AppendLine($"Au: {booking.DateFin:dd/MM/yyyy}");
            sb.AppendLine($"Total: {booking.PrixTotal:C}");

            string qrContent = sb.ToString();

            // Generate the QR Code
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q))
            using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
            {
                // 20 pixels per module ensures a high quality image
                return qrCode.GetGraphic(20);
            }
        }
    }
}
