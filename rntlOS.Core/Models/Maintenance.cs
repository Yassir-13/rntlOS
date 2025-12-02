using System;

namespace rntlOS.Core.Models
{
    public class Maintenance
    {
        public int Id { get; set; }

        // Véhicule concerné
        public int VehiculeId { get; set; }
        public Vehicule Vehicule { get; set; }

        // Détails de l'entretien
        public string Description { get; set; }
        public DateTime DateMaintenance { get; set; } = DateTime.UtcNow;
        public int KilometrageAtMaintenance { get; set; }

        // Pour générer des alertes dans BackOffice
        public DateTime? ProchaineMaintenance { get; set; }

        // Pour savoir si la maintenance est réalisée ou future
        public bool IsCompleted { get; set; } = false;
    }
}
