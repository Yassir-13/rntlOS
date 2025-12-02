using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using rntlOS.Core.Services;

namespace rntlOS.BackOffice.Views
{
    public partial class DashboardView : UserControl
    {
        private readonly VehiculeService _vehiculeService;
        private readonly ClientService _clientService;
        private readonly BookingService _bookingService;

        public DashboardView(
            VehiculeService vehiculeService,
            ClientService clientService,
            BookingService bookingService)
        {
            InitializeComponent();
            _vehiculeService = vehiculeService;
            _clientService = clientService;
            _bookingService = bookingService;

            Loaded += DashboardView_Loaded;
        }

        private async void DashboardView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadStatistics();
        }

        private async Task LoadStatistics()
        {
            // Charger les véhicules
            var vehicules = await _vehiculeService.GetAllAsync();
            TxtTotalVehicules.Text = vehicules.Count.ToString();
            TxtVehiculesDisponibles.Text = vehicules.Count(v => v.Disponible).ToString();

            // Derniers véhicules (5 derniers)
            DgDerniersVehicules.ItemsSource = vehicules.OrderByDescending(v => v.Id).Take(5).ToList();

            // Charger les clients
            var clients = await _clientService.GetAllAsync();
            TxtTotalClients.Text = clients.Count.ToString();

            // Charger les réservations
            var bookings = await _bookingService.GetAllAsync();
            var reservationsActives = bookings.Count(b =>
                b.Status == Core.Models.StatutReservation.Confirmee ||
                b.Status == Core.Models.StatutReservation.EnAttente);
            TxtReservationsActives.Text = reservationsActives.ToString();

            // Dernières réservations (5 dernières)
            DgDernieresReservations.ItemsSource = bookings.OrderByDescending(b => b.CreatedAt).Take(5).ToList();
        }
    }
}