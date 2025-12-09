using System;
using System.Collections.Generic;

namespace rntlOS.Core.Models
{
    public class Vehicule
    {
        public int Id { get; set; }

        // Informations de base
        public string? Marque { get; set; }
        public string? Modele { get; set; }
        public string? Matricule { get; set; }

        // Location
        public decimal PrixParJour { get; set; }
        public bool Disponible { get; set; } = true;

        // Type de véhicule (clé étrangère)
        public int TypeVehiculeId { get; set; }
        public TypeVehicule? TypeVehicule { get; set; }

        public ICollection<VehiculeImage>? Images { get; set; }

        // Maintenance
        public int Kilometrage { get; set; }
        public DateTime DateMiseEnService { get; set; }
        public DateTime? DerniereMaintenance { get; set; }
    }
}
