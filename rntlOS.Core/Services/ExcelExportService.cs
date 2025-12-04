using OfficeOpenXml;
using rntlOS.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace rntlOS.Core.Services
{
    public class ExcelExportService
    {
        public ExcelExportService()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public byte[] ExportVehicules(List<Vehicule> vehicules)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Véhicules");

                // En-têtes
                worksheet.Cells[1, 1].Value = "Marque";
                worksheet.Cells[1, 2].Value = "Modèle";
                worksheet.Cells[1, 3].Value = "Matricule";
                worksheet.Cells[1, 4].Value = "Prix/Jour";
                worksheet.Cells[1, 5].Value = "Kilométrage";
                worksheet.Cells[1, 6].Value = "Disponible";
                worksheet.Cells[1, 7].Value = "Type";

                // Style en-têtes
                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(11, 26, 46));
                    range.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(201, 169, 97));
                }

                // Données
                int row = 2;
                foreach (var vehicule in vehicules)
                {
                    worksheet.Cells[row, 1].Value = vehicule.Marque;
                    worksheet.Cells[row, 2].Value = vehicule.Modele;
                    worksheet.Cells[row, 3].Value = vehicule.Matricule;
                    worksheet.Cells[row, 4].Value = vehicule.PrixParJour;
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "#,##0.00 €";
                    worksheet.Cells[row, 5].Value = vehicule.Kilometrage;
                    worksheet.Cells[row, 6].Value = vehicule.Disponible ? "Oui" : "Non";
                    worksheet.Cells[row, 7].Value = vehicule.TypeVehicule?.Libelle;
                    row++;
                }

                worksheet.Cells.AutoFitColumns();
                return package.GetAsByteArray();
            }
        }

        public byte[] ExportClients(List<Client> clients)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Clients");

                // En-têtes
                worksheet.Cells[1, 1].Value = "Nom";
                worksheet.Cells[1, 2].Value = "Prénom";
                worksheet.Cells[1, 3].Value = "Email";
                worksheet.Cells[1, 4].Value = "Téléphone";

                // Style en-têtes
                using (var range = worksheet.Cells[1, 1, 1, 4])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(11, 26, 46));
                    range.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(201, 169, 97));
                }

                // Données
                int row = 2;
                foreach (var client in clients)
                {
                    worksheet.Cells[row, 1].Value = client.Nom;
                    worksheet.Cells[row, 2].Value = client.Prenom;
                    worksheet.Cells[row, 3].Value = client.Email;
                    worksheet.Cells[row, 4].Value = client.Telephone;
                    row++;
                }

                worksheet.Cells.AutoFitColumns();
                return package.GetAsByteArray();
            }
        }

        public byte[] ExportReservations(List<Booking> bookings)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Réservations");

                // En-têtes
                worksheet.Cells[1, 1].Value = "N° Réservation";
                worksheet.Cells[1, 2].Value = "Client";
                worksheet.Cells[1, 3].Value = "Véhicule";
                worksheet.Cells[1, 4].Value = "Date Début";
                worksheet.Cells[1, 5].Value = "Date Fin";
                worksheet.Cells[1, 6].Value = "Prix Total";
                worksheet.Cells[1, 7].Value = "Statut";

                // Style en-têtes
                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(11, 26, 46));
                    range.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(201, 169, 97));
                }

                // Données
                int row = 2;
                foreach (var booking in bookings)
                {
                    worksheet.Cells[row, 1].Value = booking.Id;
                    worksheet.Cells[row, 2].Value = $"{booking.Client?.Prenom} {booking.Client?.Nom}";
                    worksheet.Cells[row, 3].Value = $"{booking.Vehicule?.Marque} {booking.Vehicule?.Modele}";
                    worksheet.Cells[row, 4].Value = booking.DateDebut.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 5].Value = booking.DateFin.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 6].Value = booking.PrixTotal;
                    worksheet.Cells[row, 6].Style.Numberformat.Format = "#,##0.00 €";
                    worksheet.Cells[row, 7].Value = booking.Status.ToString();
                    row++;
                }

                worksheet.Cells.AutoFitColumns();
                return package.GetAsByteArray();
            }
        }

        public byte[] ExportPaiements(List<Paiement> paiements)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Paiements");

                // En-têtes
                worksheet.Cells[1, 1].Value = "N° Paiement";
                worksheet.Cells[1, 2].Value = "Réservation";
                worksheet.Cells[1, 3].Value = "Client";
                worksheet.Cells[1, 4].Value = "Montant";
                worksheet.Cells[1, 5].Value = "Méthode";
                worksheet.Cells[1, 6].Value = "Statut";
                worksheet.Cells[1, 7].Value = "Date";

                // Style en-têtes
                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(11, 26, 46));
                    range.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(201, 169, 97));
                }

                // Données
                int row = 2;
                foreach (var paiement in paiements)
                {
                    worksheet.Cells[row, 1].Value = paiement.Id;
                    worksheet.Cells[row, 2].Value = paiement.BookingId;
                    worksheet.Cells[row, 3].Value = $"{paiement.Booking?.Client?.Prenom} {paiement.Booking?.Client?.Nom}";
                    worksheet.Cells[row, 4].Value = paiement.Montant;
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "#,##0.00 €";
                    worksheet.Cells[row, 5].Value = paiement.Methode.ToString();
                    worksheet.Cells[row, 6].Value = paiement.Statut.ToString();
                    worksheet.Cells[row, 7].Value = paiement.DatePaiement.ToString("dd/MM/yyyy HH:mm");
                    row++;
                }

                worksheet.Cells.AutoFitColumns();
                return package.GetAsByteArray();
            }
        }
    }
}