using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using rntlOS.Core.Services;
using rntlOS.Core.Models;
using System;
using System.IO;
using Microsoft.Win32;

namespace rntlOS.BackOffice.Views
{
    public partial class VehiculesView : UserControl
    {
        private readonly VehiculeService _vehiculeService;
        private readonly IServiceProvider _serviceProvider;

        public VehiculesView(VehiculeService vehiculeService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _vehiculeService = vehiculeService;
            _serviceProvider = serviceProvider;
            Loaded += VehiculesView_Loaded;
        }

        private async void VehiculesView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadVehicles();
        }

        private async Task LoadVehicles()
        {
            var list = await _vehiculeService.GetAllAsync();
            VehiculesDataGrid.ItemsSource = list;
        }

        private async void AjouterVehicule_Click(object sender, RoutedEventArgs e)
        {
            var vehiculeService = _serviceProvider.GetRequiredService<VehiculeService>();
            var typeVehiculeService = _serviceProvider.GetRequiredService<TypeVehiculeService>();

            var window = new AjouterVehiculeWindow(vehiculeService, typeVehiculeService);

            if (window.ShowDialog() == true)
            {
                await LoadVehicles();
            }
        }

        private async void ModifierVehicule_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var vehicule = button?.DataContext as Vehicule;

            if (vehicule == null) return;

            var vehiculeService = _serviceProvider.GetRequiredService<VehiculeService>();
            var typeVehiculeService = _serviceProvider.GetRequiredService<TypeVehiculeService>();

            var window = new ModifierVehiculeWindow(vehiculeService, typeVehiculeService, vehicule);

            if (window.ShowDialog() == true)
            {
                await LoadVehicles();
            }
        }

        private async void SupprimerVehicule_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var vehicule = button?.DataContext as Vehicule;

            if (vehicule == null) return;

            var result = MessageBox.Show(
                $"Êtes-vous sûr de vouloir supprimer le véhicule {vehicule.Marque} {vehicule.Modele} ({vehicule.Matricule}) ?",
                "Confirmation de suppression",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _vehiculeService.DeleteAsync(vehicule.Id);
                    MessageBox.Show("Véhicule supprimé avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadVehicles();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la suppression : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void GererImages_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var vehicule = button?.DataContext as Vehicule;

            if (vehicule == null) return;

            var imageService = _serviceProvider.GetRequiredService<VehiculeImageService>();
            var window = new GererImagesWindow(imageService, vehicule);
            window.ShowDialog();
        }

        private async void ExporterExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var excelService = _serviceProvider.GetRequiredService<ExcelExportService>();
                var vehicules = await _vehiculeService.GetAllAsync();

                var excelBytes = excelService.ExportVehicules(vehicules);

                var saveDialog = new SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    FileName = $"Vehicules_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    File.WriteAllBytes(saveDialog.FileName, excelBytes);
                    MessageBox.Show("Export Excel réussi !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Ouvrir le fichier
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = saveDialog.FileName,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'export : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}