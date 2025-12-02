using System;
using System.Linq;
using System.Windows;
using rntlOS.Core.Models;
using rntlOS.Core.Services;

namespace rntlOS.BackOffice.Views
{
    public partial class AjouterVehiculeWindow : Window
    {
        private readonly VehiculeService _vehiculeService;
        private readonly TypeVehiculeService _typeVehiculeService;

        public AjouterVehiculeWindow(VehiculeService vehiculeService, TypeVehiculeService typeVehiculeService)
        {
            InitializeComponent();
            _vehiculeService = vehiculeService;
            _typeVehiculeService = typeVehiculeService;

            Loaded += AjouterVehiculeWindow_Loaded;
        }

        private async void AjouterVehiculeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Charger les types de véhicules
            var types = await _typeVehiculeService.GetAllAsync();
            CmbTypeVehicule.ItemsSource = types;

            if (types.Any())
                CmbTypeVehicule.SelectedIndex = 0;
        }

        private async void Enregistrer_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(TxtMarque.Text) ||
                string.IsNullOrWhiteSpace(TxtModele.Text) ||
                string.IsNullOrWhiteSpace(TxtMatricule.Text) ||
                string.IsNullOrWhiteSpace(TxtPrixParJour.Text) ||
                string.IsNullOrWhiteSpace(TxtKilometrage.Text) ||
                CmbTypeVehicule.SelectedValue == null ||
                DtpDateMiseEnService.SelectedDate == null)
            {
                MessageBox.Show("Veuillez remplir tous les champs obligatoires.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Créer le véhicule
            var vehicule = new Vehicule
            {
                Marque = TxtMarque.Text.Trim(),
                Modele = TxtModele.Text.Trim(),
                Matricule = TxtMatricule.Text.Trim(),
                PrixParJour = decimal.Parse(TxtPrixParJour.Text),
                TypeVehiculeId = (int)CmbTypeVehicule.SelectedValue,
                Kilometrage = int.Parse(TxtKilometrage.Text),
                DateMiseEnService = DtpDateMiseEnService.SelectedDate.Value,
                Disponible = ChkDisponible.IsChecked ?? true
            };

            try
            {
                await _vehiculeService.AddAsync(vehicule);
                MessageBox.Show("Véhicule ajouté avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
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