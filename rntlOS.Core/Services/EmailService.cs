using rntlOS.Core.Models;
using System;
using System.Threading.Tasks;

namespace rntlOS.Core.Services
{
    public class EmailService
    {
        public async Task EnvoyerEmailConfirmationReservation(Booking booking)
        {
            // Simulation d'envoi d'email
            await Task.Delay(500); // Simule le temps d'envoi

            // Log dans la console (visible dans la sortie de débogage)
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("📧 EMAIL DE CONFIRMATION RÉSERVATION ENVOYÉ");
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine($"Destinataire : {booking.Client.Prenom} {booking.Client.Nom}");
            Console.WriteLine($"Email : {booking.Client.Email}");
            Console.WriteLine($"Réservation N° : {booking.Id}");
            Console.WriteLine($"Véhicule : {booking.Vehicule.Marque} {booking.Vehicule.Modele}");
            Console.WriteLine($"Date début : {booking.DateDebut:dd/MM/yyyy}");
            Console.WriteLine($"Date fin : {booking.DateFin:dd/MM/yyyy}");
            Console.WriteLine($"Prix total : {booking.PrixTotal:C}");
            Console.WriteLine("═══════════════════════════════════════════════");
        }

        public async Task EnvoyerEmailConfirmationPaiement(Paiement paiement)
        {
            // Simulation d'envoi d'email
            await Task.Delay(500); // Simule le temps d'envoi

            // Log dans la console
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("📧 EMAIL DE CONFIRMATION PAIEMENT ENVOYÉ");
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine($"Destinataire : {paiement.Booking.Client.Prenom} {paiement.Booking.Client.Nom}");
            Console.WriteLine($"Email : {paiement.Booking.Client.Email}");
            Console.WriteLine($"Montant : {paiement.Montant:C}");
            Console.WriteLine($"Méthode : {paiement.Methode}");
            Console.WriteLine($"Date : {paiement.DatePaiement:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Réservation N° : {paiement.BookingId}");
            Console.WriteLine("═══════════════════════════════════════════════");
        }

        public string GenererContenuEmailReservation(Booking booking)
        {
            // Pour la démo : retourne le HTML de l'email
            return $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; background-color: #f8f9fa; padding: 20px; }}
                    .container {{ max-width: 600px; margin: 0 auto; background: white; border-radius: 10px; overflow: hidden; box-shadow: 0 4px 20px rgba(0,0,0,0.1); }}
                    .header {{ background: linear-gradient(135deg, #0B1A2E 0%, #000000 100%); color: white; padding: 30px; text-align: center; border-bottom: 4px solid #C9A961; }}
                    .content {{ padding: 30px; }}
                    .info-box {{ background: #f8f9fa; border-left: 4px solid #C9A961; padding: 15px; margin: 20px 0; }}
                    .total {{ background: #C9A961; color: white; padding: 20px; text-align: center; font-size: 24px; font-weight: bold; margin: 20px 0; border-radius: 8px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>rntl<span style='color: #C9A961; font-weight: bold;'>OS</span></h1>
                        <p>Confirmation de réservation</p>
                    </div>
                    <div class='content'>
                        <p>Bonjour <strong>{booking.Client.Prenom} {booking.Client.Nom}</strong>,</p>
                        <div class='info-box'>
                            <h3>📋 Réservation N° {booking.Id}</h3>
                            <p><strong>Date de début :</strong> {booking.DateDebut:dd/MM/yyyy}</p>
                            <p><strong>Date de fin :</strong> {booking.DateFin:dd/MM/yyyy}</p>
                        </div>
                        <div class='info-box'>
                            <h3>🚗 Véhicule loué</h3>
                            <p><strong>Marque :</strong> {booking.Vehicule.Marque}</p>
                            <p><strong>Modèle :</strong> {booking.Vehicule.Modele}</p>
                        </div>
                        <div class='total'>Prix Total : {booking.PrixTotal:C}</div>
                    </div>
                </div>
            </body>
            </html>";
        }
    }
}