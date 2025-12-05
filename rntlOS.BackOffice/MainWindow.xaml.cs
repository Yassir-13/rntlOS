using Microsoft.Extensions.DependencyInjection;
using rntlOS.BackOffice.Views;
using System;
using System.Windows;
using System.Windows.Controls;

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
            TxtUserRole.Text = UserSession.UserRole == "Admin" ? "Administrateur" : "Employé";

            // Configurer le menu selon le rôle
            ConfigurerMenuSelonRole();

            // Ouvrir le dashboard par défaut
            OpenDashboard(null, null);
        }

        private void ConfigurerMenuSelonRole()
        {
            // Récupérer les boutons du menu
            var employesButton = FindName("BtnEmployes") as Button;
            var maintenanceButton = FindName("BtnMaintenance") as Button;

            // Si l'utilisateur n'est pas Admin, masquer les sections administration
            if (UserSession.UserRole != "Admin")
            {
                if (employesButton != null)
                    employesButton.Visibility = Visibility.Collapsed;

                if (maintenanceButton != null)
                    maintenanceButton.Visibility = Visibility.Collapsed;
            }
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

        private void OpenPaiements(object sender, RoutedEventArgs e)
        {
            var view = _provider.GetRequiredService<PaiementsView>();
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
    }
}