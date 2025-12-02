using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using rntlOS.BackOffice.Views;

namespace rntlOS.BackOffice
{
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _provider;

        public MainWindow(IServiceProvider provider)
        {
            InitializeComponent();
            _provider = provider;

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Afficher les infos de l'utilisateur connecté
            TxtUserName.Text = UserSession.UserName;
            TxtUserRole.Text = UserSession.UserRole;

            OpenDashboard(null, null);
        }

        private void OpenDashboard(object sender, RoutedEventArgs e)
        {
            var view = _provider.GetRequiredService<DashboardView>();
            MainContent.Content = view;
        }

        private void OpenVehicules(object sender, RoutedEventArgs e)
        {
            var view = _provider.GetRequiredService<VehiculesView>();
            MainContent.Content = view;
        }

        private void OpenClients(object sender, RoutedEventArgs e)
        {
            var view = _provider.GetRequiredService<ClientsView>();
            MainContent.Content = view;
        }

        private void OpenReservations(object sender, RoutedEventArgs e)
        {
            var view = _provider.GetRequiredService<BookingsView>();
            MainContent.Content = view;
        }

        private void OpenEmployees(object sender, RoutedEventArgs e)
        {
            var view = _provider.GetRequiredService<UsersView>();
            MainContent.Content = view;
        }

        private void OpenMaintenance(object sender, RoutedEventArgs e)
        {
            var view = _provider.GetRequiredService<MaintenanceView>();
            MainContent.Content = view;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Êtes-vous sûr de vouloir vous déconnecter ?",
                "Déconnexion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                UserSession.Logout();

                // Redémarrer l'application
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

        private void OpenPaiements(object sender, RoutedEventArgs e)
        {
            var view = _provider.GetRequiredService<PaiementsView>();
            MainContent.Content = view;
        }
    }
}