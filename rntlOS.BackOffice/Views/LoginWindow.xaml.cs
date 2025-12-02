using System;
using System.Linq;
using System.Windows;
using rntlOS.Core.Services;
using BCrypt.Net;

namespace rntlOS.BackOffice.Views
{
    public partial class LoginWindow : Window
    {
        private readonly UserService _userService;
        public int UserId { get; private set; }
        public string UserName { get; private set; }
        public string UserRole { get; private set; }

        public LoginWindow(UserService userService)
        {
            InitializeComponent();
            _userService = userService;
        }

        private async void Connexion_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(TxtEmail.Text) ||
                string.IsNullOrWhiteSpace(TxtPassword.Password))
            {
                MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Récupérer tous les users
                var users = await _userService.GetAllAsync();
                var user = users.FirstOrDefault(u => u.Email.ToLower() == TxtEmail.Text.Trim().ToLower());

                if (user == null)
                {
                    MessageBox.Show("Email ou mot de passe incorrect.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Vérifier le mot de passe
                if (!BCrypt.Net.BCrypt.Verify(TxtPassword.Password, user.PasswordHash))
                {
                    MessageBox.Show("Email ou mot de passe incorrect.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Vérifier que le compte est actif
                if (!user.IsActive)
                {
                    MessageBox.Show("Votre compte a été désactivé. Contactez un administrateur.", "Accès refusé", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Connexion réussie
                UserId = user.Id;
                UserName = $"{user.Prenom} {user.Nom}";
                UserRole = user.Role.ToString();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la connexion : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Annuler_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}