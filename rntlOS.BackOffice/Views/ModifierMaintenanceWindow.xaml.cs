using System;
using System.Windows;
using rntlOS.Core.Models;
using rntlOS.Core.Services;

namespace rntlOS.BackOffice.Views
{
    public partial class ModifierMaintenanceWindow : Window
    {
        private readonly MaintenanceService _maintenanceService;
        private readonly Maintenance _maintenance;

        public ModifierMaintenanceWindow(MaintenanceService maintenanceService, Maintenance maintenance)
        {
            InitializeComponent();
            _maintenanceService = maintenanceService;
            _maintenance = maintenance;

            Loaded += ModifierMaintenanceWindow_Loaded;
        }

        private void ModifierMaintenanceWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Remplir les champs
            TxtVehicule.Text = $"{_maintenance.Vehicule.Marque} {_maintenance.Vehicule.Modele} - {_maintenance.Vehicule.Matricule}";
            TxtDescription.Text = _maintenance.Description;
            DtpDateMaintenance.SelectedDate = _maintenance.DateMaintenance;
            TxtKilometrage.Text = _maintenance.KilometrageAtMaintenance.ToString();
            DtpProchaineMaintenance.SelectedDate = _maintenance.ProchaineMaintenance;
            ChkCompleted.IsChecked = _maintenance.IsCompleted;
        }

        private async void Enregistrer_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(TxtDescription.Text) ||
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

            // Mettre à jour
            _maintenance.Description = TxtDescription.Text.Trim();
            _maintenance.DateMaintenance = DtpDateMaintenance.SelectedDate.Value;
            _maintenance.KilometrageAtMaintenance = kilometrage;
            _maintenance.ProchaineMaintenance = DtpProchaineMaintenance.SelectedDate;
            _maintenance.IsCompleted = ChkCompleted.IsChecked ?? false;

            try
            {
                await _maintenanceService.UpdateAsync(_maintenance);
                MessageBox.Show("Maintenance modifiée avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
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