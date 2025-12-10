using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace rntlOS.BackOffice.Views
{
    public partial class LogsView : UserControl
    {
        private readonly string _logsPath;

        public LogsView()
        {
            InitializeComponent();
            _logsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            Loaded += LogsView_Loaded;
        }

        private void LogsView_Loaded(object sender, RoutedEventArgs e)
        {
            ChargerLogs();
        }

        private void ChargerLogs()
        {
            try
            {
                if (!Directory.Exists(_logsPath))
                {
                    TxtLogs.Text = "Aucun fichier de logs trouvé.\n\nLe dossier Logs sera créé automatiquement au prochain démarrage.";
                    return;
                }

                // Trouver le fichier log du jour
                var today = DateTime.Now.ToString("yyyyMMdd");
                var logFile = Path.Combine(_logsPath, $"BackOffice_{today}.log");

                if (File.Exists(logFile))
                {
                    // Lire le fichier même s'il est verrouillé par Serilog
                    using (var fileStream = new FileStream(logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var reader = new StreamReader(fileStream))
                    {
                        var content = reader.ReadToEnd();
                        TxtLogs.Text = content;
                    }
                    
                    // Scroller vers le bas (dernières lignes)
                    TxtLogs.CaretIndex = TxtLogs.Text.Length;
                    TxtLogs.ScrollToEnd();
                }
                else
                {
                    // Afficher le dernier fichier log disponible
                    var logFiles = Directory.GetFiles(_logsPath, "BackOffice_*.log")
                                            .OrderByDescending(f => f)
                                            .ToList();

                    if (logFiles.Any())
                    {
                        var latestLog = logFiles.First();
                        var fileName = Path.GetFileName(latestLog);
                        
                        using (var fileStream = new FileStream(latestLog, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        using (var reader = new StreamReader(fileStream))
                        {
                            var content = reader.ReadToEnd();
                            TxtLogs.Text = $"=== Fichier: {fileName} ===\n\n{content}";
                        }
                        TxtLogs.ScrollToEnd();
                    }
                    else
                    {
                        TxtLogs.Text = "Aucun fichier de logs disponible.";
                    }
                }
            }
            catch (Exception ex)
            {
                TxtLogs.Text = $"Erreur lors du chargement des logs:\n{ex.Message}";
            }
        }

        private void Actualiser_Click(object sender, RoutedEventArgs e)
        {
            ChargerLogs();
            MessageBox.Show("Logs actualisés!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OuvrirDossier_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Directory.Exists(_logsPath))
                {
                    Process.Start("explorer.exe", _logsPath);
                }
                else
                {
                    MessageBox.Show("Le dossier Logs n'existe pas encore.\nIl sera créé au prochain démarrage.", 
                        "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EffacerLogs_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Voulez-vous vraiment supprimer tous les anciens fichiers de logs (>7 jours)?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (!Directory.Exists(_logsPath))
                    {
                        MessageBox.Show("Aucun fichier à supprimer.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    var sevenDaysAgo = DateTime.Now.AddDays(-7);
                    var logFiles = Directory.GetFiles(_logsPath, "BackOffice_*.log");
                    int deletedCount = 0;

                    foreach (var file in logFiles)
                    {
                        var fileInfo = new FileInfo(file);
                        if (fileInfo.LastWriteTime < sevenDaysAgo)
                        {
                            File.Delete(file);
                            deletedCount++;
                        }
                    }

                    MessageBox.Show($"{deletedCount} fichier(s) supprimé(s)!", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    ChargerLogs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
