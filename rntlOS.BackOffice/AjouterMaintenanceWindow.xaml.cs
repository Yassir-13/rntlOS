using System;
using System.Linq;
using System.Windows;
using rntlOS.Core.Models;
using rntlOS.Core.Services;

namespace rntlOS.BackOffice.Views
{
    public partial class AjouterMaintenanceWindow : Window
    {
        private readonly MaintenanceService _maintenanceService;
        private readonly VehiculeService _vehiculeService;

        public AjouterMaintenanceWindow(MaintenanceService maintenanceService, VehiculeService vehiculeService)
        {
            InitializeComponent();
            _maintenanceService = maintenanceService;
            _vehiculeService = vehiculeService;

            Loaded += AjouterMaintenanceWindow_Loaded;
        }

        private async void AjouterMaintenanceWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Charger les véhicules
            var vehicules = await _vehiculeService.GetAllAsync();
            CmbVehicule.ItemsSource = vehicules;
            if (vehicules.Any())
                CmbVehicule.SelectedIndex = 0;

            // Date par défaut = aujourd'hui
            DtpDateMaintenance.SelectedDate = DateTime.Now;
        }

        private async void Enregistrer_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (CmbVehicule.SelectedValue == null ||
                string.IsNullOrWhiteSpace(TxtDescription.Text) ||
                !DtpDateMaintenance.SelectedDate.HasValue ||
                string.IsNullOrWhiteSpace(TxtKilometrage.Text))
            {
                MessageBox.Show("Veuillez remplir tous les champs obligatoires.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtKilometrage.Text, out int kilometrage))
            {
                MessageBox.Show("Le kilométrage doit être un nombre valide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Créer la maintenance
            var maintenance = new Maintenance
            {
                VehiculeId = (int)CmbVehicule.SelectedValue,
                Description = TxtDescription.Text.Trim(),
                DateMaintenance = DtpDateMaintenance.SelectedDate.Value,
                KilometrageAtMaintenance = kilometrage,
                ProchaineMaintenance = DtpProchaineMaintenance.SelectedDate,
                IsCompleted = ChkCompleted.IsChecked ?? false
            };

            try
            {
                await _maintenanceService.AddAsync(maintenance);
                MessageBox.Show("Maintenance planifiée avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la planification : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Annuler_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}