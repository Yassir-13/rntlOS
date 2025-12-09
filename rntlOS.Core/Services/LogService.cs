using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace rntlOS.Core.Services
{
    public static class LogService
    {
        private static ILogger? _logger;

        public static void InitializeLogger(string applicationName)
        {
            var logsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(
                    path: Path.Combine(logsPath, $"{applicationName}_.log"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    retainedFileCountLimit: 30)
                .CreateLogger();

            Log.Logger = _logger;

            _logger.Information("═══════════════════════════════════════════════");
            _logger.Information("{ApplicationName} démarré", applicationName);
            _logger.Information("═══════════════════════════════════════════════");
        }

        public static void LogInfo(string message, params object[] args)
        {
            _logger?.Information(message, args);
        }

        public static void LogWarning(string message, params object[] args)
        {
            _logger?.Warning(message, args);
        }

        public static void LogError(string message, Exception? ex = null, params object[] args)
        {
            if (ex != null)
                _logger?.Error(ex, message, args);
            else
                _logger?.Error(message, args);
        }

        public static void LogDebug(string message, params object[] args)
        {
            _logger?.Debug(message, args);
        }

        // ACTIONS MÉTIER
        public static void LogReservationCreated(int bookingId, string clientEmail, int vehiculeId)
        {
            _logger?.Information("RÉSERVATION CRÉÉE - ID: {BookingId}, Client: {ClientEmail}, Véhicule: {VehiculeId}", 
                bookingId, clientEmail, vehiculeId);
        }

        public static void LogReservationModified(int bookingId, string modifiedBy)
        {
            _logger?.Information("RÉSERVATION MODIFIÉE - ID: {BookingId}, Par: {ModifiedBy}", 
                bookingId, modifiedBy);
        }

        public static void LogReservationDeleted(int bookingId, string deletedBy)
        {
            _logger?.Warning("RÉSERVATION SUPPRIMÉE - ID: {BookingId}, Par: {DeletedBy}", 
                bookingId, deletedBy);
        }

        public static void LogEmailSent(string recipientEmail, string subject)
        {
            _logger?.Information("EMAIL ENVOYÉ - Destinataire: {RecipientEmail}, Sujet: {Subject}", 
                recipientEmail, subject);
        }

        public static void LogEmailFailed(string recipientEmail, Exception ex)
        {
            _logger?.Error(ex, "ÉCHEC ENVOI EMAIL - Destinataire: {RecipientEmail}", recipientEmail);
        }

        public static void LogLogin(string userEmail, bool success)
        {
            if (success)
                _logger?.Information("CONNEXION RÉUSSIE - Utilisateur: {UserEmail}", userEmail);
            else
                _logger?.Warning("TENTATIVE CONNEXION ÉCHOUÉE - Utilisateur: {UserEmail}", userEmail);
        }

        public static void LogVehiculeAdded(int vehiculeId, string marque, string modele)
        {
            _logger?.Information("VÉHICULE AJOUTÉ - ID: {VehiculeId}, {Marque} {Modele}", 
                vehiculeId, marque, modele);
        }

        public static void LogPaiementReceived(decimal montant, string methode, int bookingId)
        {
            _logger?.Information("PAIEMENT REÇU - Montant: {Montant:C}, Méthode: {Methode}, Réservation: {BookingId}", 
                montant, methode, bookingId);
        }

        public static void Shutdown()
        {
            _logger?.Information("Application arrêtée");
            Log.CloseAndFlush();
        }
    }
}
