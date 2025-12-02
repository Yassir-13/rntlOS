using System;
using System.Text.RegularExpressions;
using System.Windows;
using rntlOS.Core.Models;
using rntlOS.Core.Services;
using BCrypt.Net;

namespace rntlOS.BackOffice.Views
{
    public partial class AjouterUserWindow : Window
    {
        private readonly UserService _userService;

        public AjouterUserWindow(UserService userService)
        {
            InitializeComponent();
            _userService = userService;
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                return Regex.IsMatch(email, pattern);
            }
            catch
            {
                return false;
            }
        }

        private async void Enregistrer_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(TxtNom.Text) ||
                string.IsNullOrWhiteSpace(TxtPrenom.Text) ||
                string.IsNullOrWhiteSpace(TxtEmail.Text) ||
                string.IsNullOrWhiteSpace(TxtPassword.Password))
            {
                MessageBox.Show("Veuillez remplir tous les champs obligatoires.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validation email
            if (!IsValidEmail(TxtEmail.Text.Trim()))
            {
                MessageBox.Show("Veuillez entrer une adresse email valide.\nExemple: utilisateur@domaine.com", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Vérifier mots de passe
            if (TxtPassword.Password != TxtConfirmPassword.Password)
            {
                MessageBox.Show("Les mots de passe ne correspondent pas.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validation longueur mot de passe
            if (TxtPassword.Password.Length < 6)
            {
                MessageBox.Show("Le mot de passe doit contenir au moins 6 caractères.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Déterminer le rôle
            UserRole role = CmbRole.SelectedIndex == 0 ? UserRole.Admin : UserRole.Employee;

            // Créer l'employé
            var user = new User
            {
                Nom = TxtNom.Text.Trim(),
                Prenom = TxtPrenom.Text.Trim(),
                Email = TxtEmail.Text.Trim().ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(TxtPassword.Password),
                Role = role,
                IsActive = ChkActif.IsChecked ?? true
            };

            try
            {
                await _userService.AddAsync(user);
                MessageBox.Show("Employé ajouté avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ajout : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Annuler_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}