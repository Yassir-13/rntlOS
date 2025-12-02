using System;
using System.Text.RegularExpressions;
using System.Windows;
using rntlOS.Core.Models;
using rntlOS.Core.Services;
using BCrypt.Net;  // ← AJOUTEZ CETTE LIGNE

namespace rntlOS.BackOffice.Views
{
    public partial class AjouterClientWindow : Window
    {
        private readonly ClientService _clientService;

        public AjouterClientWindow(ClientService clientService)
        {
            InitializeComponent();
            _clientService = clientService;
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
                string.IsNullOrWhiteSpace(TxtTelephone.Text) ||
                string.IsNullOrWhiteSpace(TxtPassword.Password))
            {
                MessageBox.Show("Veuillez remplir tous les champs obligatoires.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validation email avec Regex
            if (!IsValidEmail(TxtEmail.Text.Trim()))
            {
                MessageBox.Show("Veuillez entrer une adresse email valide.\nExemple: utilisateur@domaine.com", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Vérifier que les mots de passe correspondent
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

            // Créer le client avec mot de passe hashé
            var client = new Client
            {
                Nom = TxtNom.Text.Trim(),
                Prenom = TxtPrenom.Text.Trim(),
                Email = TxtEmail.Text.Trim().ToLower(),
                Telephone = TxtTelephone.Text.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(TxtPassword.Password)  // ← MODIFIEZ CETTE LIGNE
            };

            try
            {
                await _clientService.AddAsync(client);
                MessageBox.Show("Client ajouté avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
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