using System;
using System.Windows;
using rntlOS.Core.Models;
using rntlOS.Core.Services;

namespace rntlOS.BackOffice.Views
{
    public partial class AjouterPaiementWindow : Window
    {
        private readonly PaiementService _paiementService;
        private readonly Booking _booking;

        public AjouterPaiementWindow(PaiementService paiementService, Booking booking)
        {
            InitializeComponent();
            _paiementService = paiementService;
            _booking = booking;

            Loaded += AjouterPaiementWindow_Loaded;
        }

        private void AjouterPaiementWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Afficher les infos de la réservation
            TxtReservation.Text = $"{_booking.Client.Prenom} {_booking.Client.Nom} - " +
                                  $"{_booking.Vehicule.Marque} {_booking.Vehicule.Modele} - " +
                                  $"{_booking.PrixTotal:C}";

            // Montant par défaut = prix total de la réservation
            TxtMontant.Text = _booking.PrixTotal.ToString();

            // Date par défaut = aujourd'hui
            DtpDatePaiement.SelectedDate = DateTime.Now;
        }

        private async void Enregistrer_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(TxtMontant.Text) ||
                !DtpDatePaiement.SelectedDate.HasValue)
            {
                MessageBox.Show("Veuillez remplir tous les champs obligatoires.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtMontant.Text, out decimal montant))
            {
                MessageBox.Show("Le montant doit être un nombre valide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Déterminer la méthode
            MethodePaiement methode = MethodePaiement.CarteBancaire;
            switch (CmbMethode.SelectedIndex)
            {
                case 0: methode = MethodePaiement.CarteBancaire; break;
                case 1: methode = MethodePaiement.Especes; break;
                case 2: methode = MethodePaiement.Virement; break;
            }

            // Déterminer le statut
            StatutPaiement statut = StatutPaiement.EnAttente;
            switch (CmbStatut.SelectedIndex)
            {
                case 0: statut = StatutPaiement.EnAttente; break;
                case 1: statut = StatutPaiement.Reussi; break;
                case 2: statut = StatutPaiement.Echoue; break;
            }

            // Créer le paiement
            var paiement = new Paiement
            {
                BookingId = _booking.Id,
                Montant = montant,
                Methode = methode,
                Statut = statut,
                DatePaiement = DtpDatePaiement.SelectedDate.Value
            };

            try
            {
                await _paiementService.AddAsync(paiement);
                MessageBox.Show("Paiement enregistré avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'enregistrement : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Annuler_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}