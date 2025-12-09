using System;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using rntlOS.Core.Models;
using rntlOS.Core.Services;

namespace rntlOS.BackOffice.Views
{
    public partial class GererImagesWindow : Window
    {
        private readonly VehiculeImageService _imageService;
        private readonly Vehicule _vehicule;

        public GererImagesWindow(VehiculeImageService imageService, Vehicule vehicule)
        {
            InitializeComponent();
            _imageService = imageService;
            _vehicule = vehicule;

            TxtTitre.Text = $"Images de {_vehicule.Marque} {_vehicule.Modele}";

            Loaded += GererImagesWindow_Loaded;
        }

        private async void GererImagesWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadImages();
        }

        private async System.Threading.Tasks.Task LoadImages()
        {
            var images = await _imageService.GetAllAsync();
            var vehiculeImages = images.Where(i => i.VehiculeId == _vehicule.Id).ToList();
            ImagesListBox.ItemsSource = vehiculeImages;
        }

        private async void AjouterImage_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog
            {
                Filter = "Images (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Title = "Sélectionner une image"
            };

            if (openDialog.ShowDialog() == true)
            {
                try
                {
                    // Chemin vers le dossier wwwroot du Front-office
                    string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\"));
                    string frontOfficeWwwroot = Path.Combine(projectRoot, "rntlOS.FrontOffice", "rntlOS.FrontOffice", "wwwroot", "Images");

                    // Créer le dossier s'il n'existe pas
                    Directory.CreateDirectory(frontOfficeWwwroot);

                    // Copier l'image dans wwwroot
                    string fileName = $"{_vehicule.Id}_{Guid.NewGuid()}{Path.GetExtension(openDialog.FileName)}";
                    string destinationPath = Path.Combine(frontOfficeWwwroot, fileName);
                    File.Copy(openDialog.FileName, destinationPath, true);

                    // Enregistrer dans la base de données (sauvegarder juste le nom du fichier)
                    var image = new VehiculeImage
                    {
                        VehiculeId = _vehicule.Id,
                        ImagePath = fileName  // Juste le nom du fichier, pas le chemin complet
                    };

                    await _imageService.AddAsync(image);
                    MessageBox.Show("Image ajoutée avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadImages();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de l'ajout de l'image : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void SupprimerImage_Click(object sender, RoutedEventArgs e)
        {
            var selectedImage = ImagesListBox.SelectedItem as VehiculeImage;

            if (selectedImage == null)
            {
                MessageBox.Show("Veuillez sélectionner une image à supprimer.", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                "Êtes-vous sûr de vouloir supprimer cette image ?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Construire le chemin complet pour supprimer
                    string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\"));
                    string frontOfficeWwwroot = Path.Combine(projectRoot, "rntlOS.FrontOffice", "rntlOS.FrontOffice", "wwwroot", "Images");
                    string fullPath = Path.Combine(frontOfficeWwwroot, selectedImage.ImagePath);

                    // Supprimer le fichier
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }

                    // Supprimer de la base de données
                    await _imageService.DeleteAsync(selectedImage.Id);
                    MessageBox.Show("Image supprimée avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadImages();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la suppression : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}