using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using rntlOS.Core.Models;
using rntlOS.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Microsoft.Win32;

namespace rntlOS.BackOffice.Views
{
    public partial class PaiementsView : UserControl
    {
        private readonly PaiementService _paiementService;
        private readonly IServiceProvider _serviceProvider;

        public PaiementsView(PaiementService paiementService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _paiementService = paiementService;
            _serviceProvider = serviceProvider;
            Loaded += PaiementsView_Loaded;
        }

        private async void PaiementsView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadPaiements();
            await LoadStatistiques();
        }

        private async Task LoadPaiements()
        {
            var list = await _paiementService.GetAllAsync();
            PaiementsDataGrid.ItemsSource = list.OrderByDescending(p => p.DatePaiement).ToList();
        }

        private async Task LoadStatistiques()
        {
            var paiements = await _paiementService.GetAllAsync();

            var totalPaye = paiements.Where(p => p.Statut == StatutPaiement.Reussi).Sum(p => p.Montant);
            var totalEnAttente = paiements.Where(p => p.Statut == StatutPaiement.EnAttente).Sum(p => p.Montant);
            var totalEchoue = paiements.Where(p => p.Statut == StatutPaiement.Echoue).Sum(p => p.Montant);

            TxtTotalPaye.Text = $"{totalPaye:C}";
            TxtEnAttente.Text = $"{totalEnAttente:C}";
            TxtEchoue.Text = $"{totalEchoue:C}";
        }

        private async void ModifierPaiement_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var paiement = button?.DataContext as Paiement;

            if (paiement == null) return;

            var paiementService = _serviceProvider.GetRequiredService<PaiementService>();
            var window = new ModifierPaiementWindow(paiementService, paiement);

            if (window.ShowDialog() == true)
            {
                await LoadPaiements();
                await LoadStatistiques();
            }
        }

        private async void SupprimerPaiement_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var paiement = button?.DataContext as Paiement;

            if (paiement == null) return;

            var result = MessageBox.Show(
                $"Êtes-vous sûr de vouloir supprimer ce paiement de {paiement.Montant:C} ?",
                "Confirmation de suppression",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _paiementService.DeleteAsync(paiement.Id);
                    MessageBox.Show("Paiement supprimé avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadPaiements();
                    await LoadStatistiques();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la suppression : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void ExporterExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var excelService = _serviceProvider.GetRequiredService<ExcelExportService>();
                var paiements = await _paiementService.GetAllAsync();

                var excelBytes = excelService.ExportPaiements(paiements);

                var saveDialog = new SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    FileName = $"Paiements_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    File.WriteAllBytes(saveDialog.FileName, excelBytes);
                    MessageBox.Show("Export Excel réussi !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

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