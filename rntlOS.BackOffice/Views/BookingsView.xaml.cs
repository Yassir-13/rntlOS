using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using rntlOS.Core.Models;
using rntlOS.Core.Services;
using rntlOS.Core.PdfService;

namespace rntlOS.BackOffice.Views
{
    public partial class BookingsView : UserControl
    {
        private readonly BookingService _bookingService;
        private readonly IServiceProvider _serviceProvider;

        public BookingsView(BookingService bookingService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _bookingService = bookingService;
            _serviceProvider = serviceProvider;
            Loaded += BookingsView_Loaded;
        }

        private async void BookingsView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadBookings();
        }

        private async Task LoadBookings()
        {
            var list = await _bookingService.GetAllAsync();
            BookingsDataGrid.ItemsSource = list;
        }

        private async void AjouterReservation_Click(object sender, RoutedEventArgs e)
        {
            var bookingService = _serviceProvider.GetRequiredService<BookingService>();
            var clientService = _serviceProvider.GetRequiredService<ClientService>();
            var vehiculeService = _serviceProvider.GetRequiredService<VehiculeService>();

            var window = new AjouterBookingWindow(bookingService, clientService, vehiculeService);

            if (window.ShowDialog() == true)
            {
                await LoadBookings();
            }
        }

        private async void ModifierReservation_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var booking = button?.DataContext as Booking;

            if (booking == null) return;

            var bookingService = _serviceProvider.GetRequiredService<BookingService>();
            var window = new ModifierBookingWindow(bookingService, booking);

            if (window.ShowDialog() == true)
            {
                await LoadBookings();
            }
        }

        private async void SupprimerReservation_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var booking = button?.DataContext as Booking;

            if (booking == null) return;

            var result = MessageBox.Show(
                $"Êtes-vous sûr de vouloir supprimer cette réservation ?",
                "Confirmation de suppression",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _bookingService.DeleteAsync(booking.Id);
                    MessageBox.Show("Réservation supprimée avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadBookings();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la suppression : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void GererPaiement_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var booking = button?.DataContext as Booking;

            if (booking == null) return;

            var paiementService = _serviceProvider.GetRequiredService<PaiementService>();
            var window = new AjouterPaiementWindow(paiementService, booking);

            if (window.ShowDialog() == true)
            {
                MessageBox.Show("Paiement enregistré !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // ← AJOUTEZ CETTE MÉTHODE
        private async void GenererPdf_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var booking = button?.DataContext as Booking;

            if (booking == null) return;

            try
            {
                // Recharger la réservation complète avec les relations
                var bookingComplet = await _bookingService.GetByIdAsync(booking.Id);

                if (bookingComplet == null)
                {
                    MessageBox.Show("Impossible de charger les détails de la réservation.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Générer le PDF
                var pdfService = new ReservationPdfService();
                var pdfBytes = pdfService.GeneratePdf(bookingComplet);

                // Demander où enregistrer le fichier
                var saveDialog = new SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    FileName = $"Reservation_{bookingComplet.Id}_{DateTime.Now:yyyyMMdd}.pdf"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    File.WriteAllBytes(saveDialog.FileName, pdfBytes);
                    MessageBox.Show("PDF généré avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Ouvrir le PDF
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = saveDialog.FileName,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la génération du PDF : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}