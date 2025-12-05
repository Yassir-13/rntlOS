using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using rntlOS.Core.Models;
using rntlOS.Core.Services;

namespace rntlOS.BackOffice.Views
{
    public partial class AjouterBookingWindow : Window
    {
        private readonly BookingService _bookingService;
        private readonly ClientService _clientService;
        private readonly VehiculeService _vehiculeService;
        private decimal _prixParJour = 0;

        public AjouterBookingWindow(BookingService bookingService, ClientService clientService, VehiculeService vehiculeService)
        {
            InitializeComponent();
            _bookingService = bookingService;
            _clientService = clientService;
            _vehiculeService = vehiculeService;

            Loaded += AjouterBookingWindow_Loaded;
        }

        private async void AjouterBookingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Charger les clients
            var clients = await _clientService.GetAllAsync();
            CmbClient.ItemsSource = clients;
            if (clients.Any())
                CmbClient.SelectedIndex = 0;

            // Charger les véhicules disponibles
            var vehicules = await _vehiculeService.GetAllAsync();
            var vehiculesDisponibles = vehicules.Where(v => v.Disponible).ToList();
            CmbVehicule.ItemsSource = vehiculesDisponibles;
            if (vehiculesDisponibles.Any())
                CmbVehicule.SelectedIndex = 0;
        }

        private void CmbVehicule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vehicule = CmbVehicule.SelectedItem as Vehicule;
            if (vehicule != null)
            {
                _prixParJour = vehicule.PrixParJour;
                TxtPrixParJour.Text = $"{_prixParJour:C}";
                CalculerPrixTotal(null, null);
            }
        }

        private void CalculerPrixTotal(object sender, SelectionChangedEventArgs e)
        {
            if (DtpDateDebut.SelectedDate.HasValue && DtpDateFin.SelectedDate.HasValue)
            {
                var dateDebut = DtpDateDebut.SelectedDate.Value;
                var dateFin = DtpDateFin.SelectedDate.Value;

                if (dateFin >= dateDebut)
                {
                    var nbJours = (dateFin - dateDebut).Days + 1;
                    TxtNbJours.Text = nbJours.ToString();

                    var prixTotal = nbJours * _prixParJour;
                    TxtPrixTotal.Text = $"{prixTotal:C}";
                }
                else
                {
                    TxtNbJours.Text = "0";
                    TxtPrixTotal.Text = "0.00 MAD";
                }
            }
        }

        private async void Enregistrer_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (CmbClient.SelectedValue == null ||
                CmbVehicule.SelectedValue == null ||
                !DtpDateDebut.SelectedDate.HasValue ||
                !DtpDateFin.SelectedDate.HasValue)
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

            // Calculer le prix total
            var nbJours = (dateFin - dateDebut).Days + 1;
            var prixTotal = nbJours * _prixParJour;

            // Déterminer le statut
            StatutReservation statut = StatutReservation.EnAttente;
            switch (CmbStatut.SelectedIndex)
            {
                case 0: statut = StatutReservation.EnAttente; break;
                case 1: statut = StatutReservation.Confirmee; break;
                case 2: statut = StatutReservation.Annulee; break;
                case 3: statut = StatutReservation.Terminee; break;
            }

            // Créer la réservation
            var booking = new Booking
            {
                ClientId = (int)CmbClient.SelectedValue,
                VehiculeId = (int)CmbVehicule.SelectedValue,
                DateDebut = dateDebut,
                DateFin = dateFin,
                PrixTotal = prixTotal,
                Status = statut
            };

            await _bookingService.AddAsync(booking);

            // Envoyer email de confirmation
            try
            {
                var emailService = new EmailService();
                await emailService.EnvoyerEmailConfirmationReservation(booking);
                MessageBox.Show("Réservation créée et email de confirmation envoyé !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Réservation créée mais erreur email : {ex.Message}", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            DialogResult = true;
            Close();
        }

        private void Annuler_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}