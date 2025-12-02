using System;
using System.Windows;
using rntlOS.Core.Models;
using rntlOS.Core.Services;

namespace rntlOS.BackOffice.Views
{
    public partial class ModifierBookingWindow : Window
    {
        private readonly BookingService _bookingService;
        private readonly Booking _booking;

        public ModifierBookingWindow(BookingService bookingService, Booking booking)
        {
            InitializeComponent();
            _bookingService = bookingService;
            _booking = booking;

            Loaded += ModifierBookingWindow_Loaded;
        }

        private void ModifierBookingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Remplir les champs
            TxtClient.Text = $"{_booking.Client.Prenom} {_booking.Client.Nom} - {_booking.Client.Email}";
            TxtVehicule.Text = $"{_booking.Vehicule.Marque} {_booking.Vehicule.Modele} - {_booking.Vehicule.PrixParJour:C}/jour";
            DtpDateDebut.SelectedDate = _booking.DateDebut;
            DtpDateFin.SelectedDate = _booking.DateFin;
            TxtPrixTotal.Text = _booking.PrixTotal.ToString();

            // Statut
            switch (_booking.Status)
            {
                case StatutReservation.EnAttente:
                    CmbStatut.SelectedIndex = 0;
                    break;
                case StatutReservation.Confirmee:
                    CmbStatut.SelectedIndex = 1;
                    break;
                case StatutReservation.Annulee:
                    CmbStatut.SelectedIndex = 2;
                    break;
                case StatutReservation.Terminee:
                    CmbStatut.SelectedIndex = 3;
                    break;
            }
        }

        private async void Enregistrer_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (!DtpDateDebut.SelectedDate.HasValue ||
                !DtpDateFin.SelectedDate.HasValue ||
                string.IsNullOrWhiteSpace(TxtPrixTotal.Text))
            {
                MessageBox.Show("Veuillez remplir tous les champs obligatoires.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dateDebut = DtpDateDebut.SelectedDate.Value;
            var dateFin = DtpDateFin.SelectedDate.Value;

            if (dateFin < dateDebut)
            {
                MessageBox.Show("La date de fin doit être supérieure ou égale à la date de début.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Déterminer le statut
            StatutReservation statut = StatutReservation.EnAttente;
            switch (CmbStatut.SelectedIndex)
            {
                case 0: statut = StatutReservation.EnAttente; break;
                case 1: statut = StatutReservation.Confirmee; break;
                case 2: statut = StatutReservation.Annulee; break;
                case 3: statut = StatutReservation.Terminee; break;
            }

            // Mettre à jour
            _booking.DateDebut = dateDebut;
            _booking.DateFin = dateFin;
            _booking.PrixTotal = decimal.Parse(TxtPrixTotal.Text);
            _booking.Status = statut;

            try
            {
                await _bookingService.UpdateAsync(_booking);
                MessageBox.Show("Réservation modifiée avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
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