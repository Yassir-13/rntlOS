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
    }
}