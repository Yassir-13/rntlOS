using System;
using System.Linq;
using System.Windows;
using rntlOS.Core.Models;
using rntlOS.Core.Services;

namespace rntlOS.BackOffice.Views
{
    public partial class ModifierVehiculeWindow : Window
    {
        private readonly VehiculeService _vehiculeService;
        private readonly TypeVehiculeService _typeVehiculeService;
        private readonly Vehicule _vehicule;

        public ModifierVehiculeWindow(VehiculeService vehiculeService, TypeVehiculeService typeVehiculeService, Vehicule vehicule)
        {
            InitializeComponent();
            _vehiculeService = vehiculeService;
            _typeVehiculeService = typeVehiculeService;
            _vehicule = vehicule;

            Loaded += ModifierVehiculeWindow_Loaded;
        }

        private async void ModifierVehiculeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Charger les types de véhicules
            var types = await _typeVehiculeService.GetAllAsync();
            CmbTypeVehicule.ItemsSource = types;

            // Remplir les champs avec les données existantes
            TxtMarque.Text = _vehicule.Marque;
            TxtModele.Text = _vehicule.Modele;
            TxtMatricule.Text = _vehicule.Matricule;
            TxtPrixParJour.Text = _vehicule.PrixParJour.ToString();
            TxtKilometrage.Text = _vehicule.Kilometrage.ToString();
            DtpDateMiseEnService.SelectedDate = _vehicule.DateMiseEnService;
            ChkDisponible.IsChecked = _vehicule.Disponible;

            CmbTypeVehicule.SelectedValue = _vehicule.TypeVehiculeId;
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

            // Mettre à jour le véhicule
            _vehicule.Marque = TxtMarque.Text.Trim();
            _vehicule.Modele = TxtModele.Text.Trim();
            _vehicule.Matricule = TxtMatricule.Text.Trim();
            _vehicule.PrixParJour = decimal.Parse(TxtPrixParJour.Text);
            _vehicule.TypeVehiculeId = (int)CmbTypeVehicule.SelectedValue;
            _vehicule.Kilometrage = int.Parse(TxtKilometrage.Text);
            _vehicule.DateMiseEnService = DtpDateMiseEnService.SelectedDate.Value;
            _vehicule.Disponible = ChkDisponible.IsChecked ?? true;

            try
            {
                await _vehiculeService.UpdateAsync(_vehicule);
                MessageBox.Show("Véhicule modifié avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
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