using System;

namespace rntlOS.Core.Models
{
    public enum StatutReservation
    {
        EnAttente,
        Confirmee,
        Annulee,
        Terminee
    }

    public class Booking
    {
        public int Id { get; set; }

        // Client concerné
        public int ClientId { get; set; }
        public Client Client { get; set; }

        // Véhicule choisi
        public int VehiculeId { get; set; }
        public Vehicule Vehicule { get; set; }

        // Détails de la réservation
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }

        public decimal PrixTotal { get; set; }

        // État de la réservation
        public StatutReservation Status { get; set; } = StatutReservation.EnAttente;

        // Date de création
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
