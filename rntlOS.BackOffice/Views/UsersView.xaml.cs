using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using rntlOS.Core.Models;
using rntlOS.Core.Services;

namespace rntlOS.BackOffice.Views
{
    public partial class UsersView : UserControl
    {
        private readonly UserService _userService;
        private readonly IServiceProvider _serviceProvider;

        public UsersView(UserService userService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _userService = userService;
            _serviceProvider = serviceProvider;
            Loaded += UsersView_Loaded;
        }

        private async void UsersView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadUsers();
        }

        private async Task LoadUsers()
        {
            var list = await _userService.GetAllAsync();
            UsersDataGrid.ItemsSource = list;
        }

        private async void AjouterUser_Click(object sender, RoutedEventArgs e)
        {
            var userService = _serviceProvider.GetRequiredService<UserService>();
            var window = new AjouterUserWindow(userService);

            if (window.ShowDialog() == true)
            {
                await LoadUsers();
            }
        }

        private async void ModifierUser_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var user = button?.DataContext as User;

            if (user == null) return;

            var userService = _serviceProvider.GetRequiredService<UserService>();
            var window = new ModifierUserWindow(userService, user);

            if (window.ShowDialog() == true)
            {
                await LoadUsers();
            }
        }

        private async void SupprimerUser_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var user = button?.DataContext as User;

            if (user == null) return;

            var result = MessageBox.Show(
                $"Êtes-vous sûr de vouloir supprimer l'employé {user.Prenom} {user.Nom} ?",
                "Confirmation de suppression",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _userService.DeleteAsync(user.Id);
                    MessageBox.Show("Employé supprimé avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadUsers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la suppression : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}