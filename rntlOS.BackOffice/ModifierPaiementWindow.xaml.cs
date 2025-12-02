using System;
using System.Windows;
using rntlOS.Core.Models;
using rntlOS.Core.Services;

namespace rntlOS.BackOffice.Views
{
    public partial class ModifierPaiementWindow : Window
    {
        private readonly PaiementService _paiementService;
        private readonly Paiement _paiement;

        public ModifierPaiementWindow(PaiementService paiementService, Paiement paiement)
        {
            InitializeComponent();
            _paiementService = paiementService;
            _paiement = paiement;

            Loaded += ModifierPaiementWindow_Loaded;
        }

        private void ModifierPaiementWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Afficher les infos
            TxtReservation.Text = $"{_paiement.Booking.Client.Prenom} {_paiement.Booking.Client.Nom} - " +
                                  $"{_paiement.Booking.Vehicule.Marque} {_paiement.Booking.Vehicule.Modele}";

            TxtMontant.Text = _paiement.Montant.ToString();
            DtpDatePaiement.SelectedDate = _paiement.DatePaiement;

            // Méthode
            switch (_paiement.Methode)
            {
                case MethodePaiement.CarteBancaire: CmbMethode.SelectedIndex = 0; break;
                case MethodePaiement.Especes: CmbMethode.SelectedIndex = 1; break;
                case MethodePaiement.Virement: CmbMethode.SelectedIndex = 2; break;
            }

            // Statut
            switch (_paiement.Statut)
            {
                case StatutPaiement.EnAttente: CmbStatut.SelectedIndex = 0; break;
                case StatutPaiement.Reussi: CmbStatut.SelectedIndex = 1; break;
                case StatutPaiement.Echoue: CmbStatut.SelectedIndex = 2; break;
            }
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

            // Mettre à jour
            _paiement.Montant = montant;
            _paiement.DatePaiement = DtpDatePaiement.SelectedDate.Value;

            // Méthode
            switch (CmbMethode.SelectedIndex)
            {
                case 0: _paiement.Methode = MethodePaiement.CarteBancaire; break;
                case 1: _paiement.Methode = MethodePaiement.Especes; break;
                case 2: _paiement.Methode = MethodePaiement.Virement; break;
            }

            // Statut
            switch (CmbStatut.SelectedIndex)
            {
                case 0: _paiement.Statut = StatutPaiement.EnAttente; break;
                case 1: _paiement.Statut = StatutPaiement.Reussi; break;
                case 2: _paiement.Statut = StatutPaiement.Echoue; break;
            }

            try
            {
                await _paiementService.UpdateAsync(_paiement);
                MessageBox.Show("Paiement modifié avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la modification : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Annuler_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}