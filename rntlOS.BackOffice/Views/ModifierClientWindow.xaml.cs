using System;
using System.Text.RegularExpressions;
using System.Windows;
using rntlOS.Core.Models;
using rntlOS.Core.Services;
using BCrypt.Net;  // ← AJOUTEZ CETTE LIGNE

namespace rntlOS.BackOffice.Views
{
    public partial class ModifierClientWindow : Window
    {
        private readonly ClientService _clientService;
        private readonly Client _client;

        public ModifierClientWindow(ClientService clientService, Client client)
        {
            InitializeComponent();
            _clientService = clientService;
            _client = client;

            Loaded += ModifierClientWindow_Loaded;
        }

        private void ModifierClientWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Remplir les champs avec les données existantes
            TxtNom.Text = _client.Nom;
            TxtPrenom.Text = _client.Prenom;
            TxtEmail.Text = _client.Email;
            TxtTelephone.Text = _client.Telephone;
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
                string.IsNullOrWhiteSpace(TxtTelephone.Text))
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

            // Si un nouveau mot de passe est saisi
            if (!string.IsNullOrWhiteSpace(TxtPassword.Password))
            {
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

                // Hasher et mettre à jour le mot de passe
                _client.PasswordHash = BCrypt.Net.BCrypt.HashPassword(TxtPassword.Password); 
            }

            // Mettre à jour le client
            _client.Nom = TxtNom.Text.Trim();
            _client.Prenom = TxtPrenom.Text.Trim();
            _client.Email = TxtEmail.Text.Trim().ToLower();
            _client.Telephone = TxtTelephone.Text.Trim();

            try
            {
                await _clientService.UpdateAsync(_client);
                MessageBox.Show("Client modifié avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
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