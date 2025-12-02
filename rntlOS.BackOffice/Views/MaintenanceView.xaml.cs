using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using rntlOS.Core.Models;
using rntlOS.Core.Services;

namespace rntlOS.BackOffice.Views
{
    public partial class MaintenanceView : UserControl
    {
        private readonly MaintenanceService _maintenanceService;
        private readonly IServiceProvider _serviceProvider;

        public MaintenanceView(MaintenanceService maintenanceService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _maintenanceService = maintenanceService;
            _serviceProvider = serviceProvider;
            Loaded += MaintenanceView_Loaded;
        }

        private async void MaintenanceView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadMaintenances();
            await VerifierAlertes();
        }

        private async Task LoadMaintenances()
        {
            var list = await _maintenanceService.GetAllAsync();
            MaintenancesDataGrid.ItemsSource = list.OrderByDescending(m => m.DateMaintenance).ToList();
        }

        private async Task VerifierAlertes()
        {
            var maintenances = await _maintenanceService.GetAllAsync();
            var aujourdhui = DateTime.Now;
            var alertes = maintenances
                .Where(m => !m.IsCompleted &&
                           m.ProchaineMaintenance.HasValue &&
                           m.ProchaineMaintenance.Value <= aujourdhui.AddDays(30))
                .ToList();

            if (alertes.Any())
            {
                AlertesBorder.Visibility = Visibility.Visible;
                var messages = alertes.Select(m =>
                {
                    var jours = (m.ProchaineMaintenance.Value - aujourdhui).Days;
                    return $"• {m.Vehicule.Marque} {m.Vehicule.Modele} ({m.Vehicule.Matricule}) - " +
                           $"Maintenance prévue dans {jours} jour(s) - {m.Description}";
                });
                TxtAlertes.Text = string.Join("\n", messages);
            }
            else
            {
                AlertesBorder.Visibility = Visibility.Collapsed;
            }
        }

        private async void AjouterMaintenance_Click(object sender, RoutedEventArgs e)
        {
            var maintenanceService = _serviceProvider.GetRequiredService<MaintenanceService>();
            var vehiculeService = _serviceProvider.GetRequiredService<VehiculeService>();

            var window = new AjouterMaintenanceWindow(maintenanceService, vehiculeService);

            if (window.ShowDialog() == true)
            {
                await LoadMaintenances();
                await VerifierAlertes();
            }
        }

        private async void ModifierMaintenance_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var maintenance = button?.DataContext as Maintenance;

            if (maintenance == null) return;

            var maintenanceService = _serviceProvider.GetRequiredService<MaintenanceService>();
            var window = new ModifierMaintenanceWindow(maintenanceService, maintenance);

            if (window.ShowDialog() == true)
            {
                await LoadMaintenances();
                await VerifierAlertes();
            }
        }

        private async void TerminerMaintenance_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var maintenance = button?.DataContext as Maintenance;

            if (maintenance == null) return;

            if (maintenance.IsCompleted)
            {
                MessageBox.Show("Cette maintenance est déjà terminée.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Marquer cette maintenance comme terminée ?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    maintenance.IsCompleted = true;
                    await _maintenanceService.UpdateAsync(maintenance);
                    MessageBox.Show("Maintenance marquée comme terminée !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadMaintenances();
                    await VerifierAlertes();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void SupprimerMaintenance_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var maintenance = button?.DataContext as Maintenance;

            if (maintenance == null) return;

            var result = MessageBox.Show(
                $"Êtes-vous sûr de vouloir supprimer cette maintenance ?",
                "Confirmation de suppression",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _maintenanceService.DeleteAsync(maintenance.Id);
                    MessageBox.Show("Maintenance supprimée avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadMaintenances();
                    await VerifierAlertes();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la suppression : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}