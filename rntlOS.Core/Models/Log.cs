using System;

namespace rntlOS.Core.Models
{
    public class Log
    {
        public int Id { get; set; }

        // User qui a effectué l'action (backoffice)
        public int? UserId { get; set; }
        public User User { get; set; }

        // Action (ex: "Ajout véhicule", "Réservation confirmée")
        public string Action { get; set; }

        // Détails optionnels
        public string Details { get; set; }

        // Date de l'action
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
