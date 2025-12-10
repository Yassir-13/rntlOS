using rntlOS.Core.Models;
using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace rntlOS.Core.Services
{
    public class EmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(
            string smtpServer = "smtp.gmail.com",
            int smtpPort = 587,
            string smtpUser = "nacir3030@gmail.com",
            string smtpPassword = "mnkrqloqnzffnlug",
            string fromEmail = "noreply@rntlos.com",
            string fromName = "rntlOS - Location de Véhicules")
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _smtpUser = smtpUser;
            _smtpPassword = smtpPassword;
            _fromEmail = fromEmail;
            _fromName = fromName;
        }

        public async Task EnvoyerEmailConfirmationReservation(Booking booking)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_fromName, _fromEmail));
                message.To.Add(new MailboxAddress($"{booking.Client.Prenom} {booking.Client.Nom}", booking.Client.Email));
                message.Subject = $"Confirmation de votre réservation #{booking.Id} - rntlOS";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = GenererContenuEmailReservation(booking)
                };

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_smtpUser, _smtpPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                // Log succès
                LogService.LogEmailSent(booking.Client.Email, $"Confirmation réservation #{booking.Id}");
                Console.WriteLine("═══════════════════════════════════════════════");
                Console.WriteLine("✅ EMAIL DE CONFIRMATION RÉSERVATION ENVOYÉ");
                Console.WriteLine($"Destinataire : {booking.Client.Email}");
                Console.WriteLine($"Réservation N° : {booking.Id}");
                Console.WriteLine("═══════════════════════════════════════════════");
            }
            catch (Exception ex)
            {
                // Log erreur
                LogService.LogEmailFailed(booking.Client.Email, ex);
                Console.WriteLine("═══════════════════════════════════════════════");
                Console.WriteLine("❌ ERREUR ENVOI EMAIL RÉSERVATION");
                Console.WriteLine($"Erreur : {ex.Message}");
                Console.WriteLine("═══════════════════════════════════════════════");
                throw;
            }
        }

        public async Task EnvoyerEmailConfirmationPaiement(Paiement paiement)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_fromName, _fromEmail));
                message.To.Add(new MailboxAddress(
                    $"{paiement.Booking.Client.Prenom} {paiement.Booking.Client.Nom}",
                    paiement.Booking.Client.Email));
                message.Subject = $"Confirmation de paiement - Réservation #{paiement.BookingId}";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = GenererContenuEmailPaiement(paiement)
                };

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_smtpUser, _smtpPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                // Log succès
                Console.WriteLine("═══════════════════════════════════════════════");
                Console.WriteLine("✅ EMAIL DE CONFIRMATION PAIEMENT ENVOYÉ");
                Console.WriteLine($"Destinataire : {paiement.Booking.Client.Email}");
                Console.WriteLine($"Montant : {paiement.Montant:C}");
                Console.WriteLine("═══════════════════════════════════════════════");
            }
            catch (Exception ex)
            {
                Console.WriteLine("═══════════════════════════════════════════════");
                Console.WriteLine("❌ ERREUR ENVOI EMAIL PAIEMENT");
                Console.WriteLine($"Erreur : {ex.Message}");
                Console.WriteLine("═══════════════════════════════════════════════");
                throw;
            }
        }

        public string GenererContenuEmailReservation(Booking booking)
        {
            return $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; background-color: #f8f9fa; padding: 20px; }}
                    .container {{ max-width: 600px; margin: 0 auto; background: white; border-radius: 10px; overflow: hidden; box-shadow: 0 4px 20px rgba(0,0,0,0.1); }}
                    .header {{ background: linear-gradient(135deg, #bebebeff 0%, #7e7a7aff 100%); color: white; padding: 30px; text-align: center; border-bottom: 4px solid #C9A961; }}
                    .content {{ padding: 30px; }}
                    .info-box {{ background: #f8f9fa; border-left: 4px solid #C9A961; padding: 15px; margin: 20px 0; }}
                    .total {{ background: #C9A961; color: white; padding: 20px; text-align: center; font-size: 24px; font-weight: bold; margin: 20px 0; border-radius: 8px; }}
                    .footer {{ background: #f8f9fa; padding: 20px; text-align: center; color: #666; font-size: 12px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>rntl<span style='color: #C9A961; font-weight: bold;'>OS</span></h1>
                        <p style='margin: 0; font-size: 18px;'>Confirmation de réservation</p>
                    </div>
                    <div class='content'>
                        <p>Bonjour <strong>{booking.Client.Prenom} {booking.Client.Nom}</strong>,</p>
                        <p>Nous avons le plaisir de confirmer votre réservation :</p>
                        
                        <div class='info-box'>
                            <h3 style='margin-top: 0; color: #0B1A2E;'>Réservation :</h3>
                            <p><strong>Date de début :</strong> {booking.DateDebut:dd/MM/yyyy}</p>
                            <p><strong>Date de fin :</strong> {booking.DateFin:dd/MM/yyyy}</p>
                            <p><strong>Durée :</strong> {(booking.DateFin - booking.DateDebut).Days + 1} jour(s)</p>
                        </div>
                        
                        <div class='info-box'>
                            <h3 style='margin-top: 0; color: #0B1A2E;'>Véhicule loué</h3>
                            <p><strong>Marque :</strong> {booking.Vehicule.Marque}</p>
                            <p><strong>Modèle :</strong> {booking.Vehicule.Modele}</p>
                            <p><strong>Matricule :</strong> {booking.Vehicule.Matricule}</p>
                        </div>
                        
                        <div class='total'>Prix Total : {booking.PrixTotal:C}</div>
                        
                        <p style='color: #666; font-size: 14px;'>
                            Merci de votre confiance. Nous vous attendons pour récupérer votre véhicule.
                        </p>
                    </div>
                    <div class='footer'>
                        <p>rntlOS - Location de Véhicules de Prestige</p>
                        <p>Cet email a été envoyé automatiquement, merci de ne pas y répondre.</p>
                    </div>
                </div>
            </body>
            </html>";
        }

        private string GenererContenuEmailPaiement(Paiement paiement)
        {
            return $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; background-color: #f8f9fa; padding: 20px; }}
                    .container {{ max-width: 600px; margin: 0 auto; background: white; border-radius: 10px; overflow: hidden; box-shadow: 0 4px 20px rgba(0,0,0,0.1); }}
                    .header {{ background: linear-gradient(135deg, #0B1A2E 0%, #000000 100%); color: white; padding: 30px; text-align: center; border-bottom: 4px solid #C9A961; }}
                    .content {{ padding: 30px; }}
                    .success {{ background: #28a745; color: white; padding: 20px; text-align: center; font-size: 20px; border-radius: 8px; margin: 20px 0; }}
                    .info {{ background: #f8f9fa; padding: 15px; margin: 10px 0; border-left: 4px solid #C9A961; }}
                    .footer {{ background: #f8f9fa; padding: 20px; text-align: center; color: #666; font-size: 12px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>rntl<span style='color: #C9A961; font-weight: bold;'>OS</span></h1>
                        <p style='margin: 0; font-size: 18px;'>Confirmation de paiement</p>
                    </div>
                    <div class='content'>
                        <p>Bonjour <strong>{paiement.Booking.Client.Prenom} {paiement.Booking.Client.Nom}</strong>,</p>
                        
                        <div class='success'>
                            ✅ Paiement reçu avec succès
                        </div>
                        
                        <div class='info'>
                            <p><strong>Montant :</strong> {paiement.Montant:C}</p>
                            <p><strong>Méthode :</strong> {paiement.Methode}</p>
                            <p><strong>Date :</strong> {paiement.DatePaiement:dd/MM/yyyy HH:mm}</p>
                            <p><strong>Réservation N° :</strong> {paiement.BookingId}</p>
                        </div>
                        
                        <p style='color: #666; margin-top: 20px;'>
                            Merci pour votre paiement. Votre réservation est maintenant confirmée.
                        </p>
                    </div>
                    <div class='footer'>
                        <p>rntlOS - Location de Véhicules de Prestige</p>
                        <p>Cet email a été envoyé automatiquement, merci de ne pas y répondre.</p>
                    </div>
                </div>
            </body>
            </html>";
        }
    }
}