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
                    // Créer le dossier Images s'il n'existe pas
                    string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                    Directory.CreateDirectory(imagesFolder);

                    // Copier l'image dans le dossier
                    string fileName = $"{_vehicule.Id}_{Guid.NewGuid()}{Path.GetExtension(openDialog.FileName)}";
                    string destinationPath = Path.Combine(imagesFolder, fileName);
                    File.Copy(openDialog.FileName, destinationPath, true);

                    // Enregistrer dans la base de données
                    var image = new VehiculeImage
                    {
                        VehiculeId = _vehicule.Id,
                        ImagePath = destinationPath
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
                    // Supprimer le fichier
                    if (File.Exists(selectedImage.ImagePath))
                    {
                        File.Delete(selectedImage.ImagePath);
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