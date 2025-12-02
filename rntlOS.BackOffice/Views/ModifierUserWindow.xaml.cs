using System;
using System.Text.RegularExpressions;
using System.Windows;
using rntlOS.Core.Models;
using rntlOS.Core.Services;
using BCrypt.Net;

namespace rntlOS.BackOffice.Views
{
    public partial class ModifierUserWindow : Window
    {
        private readonly UserService _userService;
        private readonly User _user;

        public ModifierUserWindow(UserService userService, User user)
        {
            InitializeComponent();
            _userService = userService;
            _user = user;

            Loaded += ModifierUserWindow_Loaded;
        }

        private void ModifierUserWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Remplir les champs
            TxtNom.Text = _user.Nom;
            TxtPrenom.Text = _user.Prenom;
            TxtEmail.Text = _user.Email;
            CmbRole.SelectedIndex = _user.Role == UserRole.Admin ? 0 : 1;
            ChkActif.IsChecked = _user.IsActive;
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
                string.IsNullOrWhiteSpace(TxtEmail.Text))
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

            // Si nouveau mot de passe
            if (!string.IsNullOrWhiteSpace(TxtPassword.Password))
            {
                if (TxtPassword.Password != TxtConfirmPassword.Password)
                {
                    MessageBox.Show("Les mots de passe ne correspondent pas.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (TxtPassword.Password.Length < 6)
                {
                    MessageBox.Show("Le mot de passe doit contenir au moins 6 caractères.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(TxtPassword.Password);
            }

            // Mettre à jour
            _user.Nom = TxtNom.Text.Trim();
            _user.Prenom = TxtPrenom.Text.Trim();
            _user.Email = TxtEmail.Text.Trim().ToLower();
            _user.Role = CmbRole.SelectedIndex == 0 ? UserRole.Admin : UserRole.Employee;
            _user.IsActive = ChkActif.IsChecked ?? true;

            try
            {
                await _userService.UpdateAsync(_user);
                MessageBox.Show("Employé modifié avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la modification : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Annuler_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}