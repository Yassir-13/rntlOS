using System;

namespace rntlOS.Core.Models
{
    public enum StatutPaiement
    {
        EnAttente,
        Reussi,
        Echoue
    }

    public enum MethodePaiement
    {
        CarteBancaire,
        Especes,
        Virement
    }

    public class Paiement
    {
        public int Id { get; set; }

        // Réservation associée
        public int BookingId { get; set; }
        public Booking Booking { get; set; }

        // Informations du paiement
        public decimal Montant { get; set; }
        public MethodePaiement Methode { get; set; }
        public StatutPaiement Statut { get; set; } = StatutPaiement.EnAttente;

        // Audit
        public DateTime DatePaiement { get; set; } = DateTime.UtcNow;
    }
}
