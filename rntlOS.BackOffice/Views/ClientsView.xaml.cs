using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using rntlOS.Core.Models;
using rntlOS.Core.Services;

namespace rntlOS.BackOffice.Views
{
    public partial class ClientsView : UserControl
    {
        private readonly ClientService _clientService;
        private readonly IServiceProvider _serviceProvider;

        public ClientsView(ClientService clientService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _clientService = clientService;
            _serviceProvider = serviceProvider;
            Loaded += ClientsView_Loaded;
        }

        private async void ClientsView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadClients();
        }

        private async Task LoadClients()
        {
            var list = await _clientService.GetAllAsync();
            ClientsDataGrid.ItemsSource = list;
        }

        private async void AjouterClient_Click(object sender, RoutedEventArgs e)
        {
            var clientService = _serviceProvider.GetRequiredService<ClientService>();
            var window = new AjouterClientWindow(clientService);

            if (window.ShowDialog() == true)
            {
                await LoadClients();
            }
        }

        private async void ModifierClient_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var client = button?.DataContext as Client;

            if (client == null) return;

            var clientService = _serviceProvider.GetRequiredService<ClientService>();
            var window = new ModifierClientWindow(clientService, client);

            if (window.ShowDialog() == true)
            {
                await LoadClients();
            }
        }

        private async void SupprimerClient_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var client = button?.DataContext as Client;

            if (client == null) return;

            var result = MessageBox.Show(
                $"Êtes-vous sûr de vouloir supprimer le client {client.Prenom} {client.Nom} ?",
                "Confirmation de suppression",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _clientService.DeleteAsync(client.Id);
                    MessageBox.Show("Client supprimé avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadClients();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la suppression : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}