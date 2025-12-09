using OfficeOpenXml;
using rntlOS.Core.Data;
using rntlOS.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace rntlOS.Core.Services
{
    public class ExcelImportService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public ExcelImportService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<(int successCount, List<string> errors)> ImportVehicules(string filePath)
        {
            var errors = new List<string>();
            int successCount = 0;

            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                using var package = new ExcelPackage(new FileInfo(filePath));
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;

                // Charger les types de véhicules existants
                var typesVehicules = await context.TypesVehicules.ToListAsync();

                for (int row = 2; row <= rowCount; row++) // Ligne 1 = headers
                {
                    try
                    {
                        var marque = worksheet.Cells[row, 1].Text.Trim();
                        var modele = worksheet.Cells[row, 2].Text.Trim();
                        var matricule = worksheet.Cells[row, 3].Text.Trim();
                        var anneeText = worksheet.Cells[row, 4].Text.Trim();
                        var kilometrageText = worksheet.Cells[row, 5].Text.Trim();
                        var prixText = worksheet.Cells[row, 6].Text.Trim();
                        var typeVehiculeNom = worksheet.Cells[row, 7].Text.Trim();
                        var disponibleText = worksheet.Cells[row, 8].Text.Trim();

                        // Validation
                        if (string.IsNullOrEmpty(marque) || string.IsNullOrEmpty(modele) || string.IsNullOrEmpty(matricule))
                        {
                            errors.Add($"Ligne {row}: Marque, Modèle et Matricule sont obligatoires");
                            continue;
                        }

                        // Vérifier matricule unique
                        if (await context.Vehicules.AnyAsync(v => v.Matricule == matricule))
                        {
                            errors.Add($"Ligne {row}: Matricule {matricule} existe déjà");
                            continue;
                        }

                        // Parser les valeurs
                        if (!int.TryParse(anneeText, out int annee) || annee < 1900)
                        {
                            errors.Add($"Ligne {row}: Année invalide");
                            continue;
                        }

                        if (!int.TryParse(kilometrageText, out int kilometrage) || kilometrage < 0)
                        {
                            errors.Add($"Ligne {row}: Kilométrage invalide");
                            continue;
                        }

                        if (!decimal.TryParse(prixText, out decimal prix) || prix <= 0)
                        {
                            errors.Add($"Ligne {row}: Prix invalide");
                            continue;
                        }

                        // Trouver le type de véhicule
                        var typeVehicule = typesVehicules.FirstOrDefault(t => 
                            t.Libelle.Equals(typeVehiculeNom, StringComparison.OrdinalIgnoreCase));

                        if (typeVehicule == null)
                        {
                            errors.Add($"Ligne {row}: Type de véhicule '{typeVehiculeNom}' introuvable");
                            continue;
                        }

                        bool disponible = disponibleText.Equals("Oui", StringComparison.OrdinalIgnoreCase) ||
                                         disponibleText.Equals("True", StringComparison.OrdinalIgnoreCase) ||
                                         disponibleText == "1";

                        // Créer le véhicule
                        var vehicule = new Vehicule
                        {
                            Marque = marque,
                            Modele = modele,
                            Matricule = matricule,
                            Kilometrage = kilometrage,
                            PrixParJour = prix,
                            TypeVehiculeId = typeVehicule.Id,
                            Disponible = disponible,
                            DateMiseEnService = DateTime.Now
                        };

                        context.Vehicules.Add(vehicule);
                        await context.SaveChangesAsync();
                        
                        LogService.LogVehiculeAdded(vehicule.Id, vehicule.Marque, vehicule.Modele);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Ligne {row}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Erreur générale: {ex.Message}");
            }

            return (successCount, errors);
        }

        public async Task<(int successCount, List<string> errors)> ImportClients(string filePath)
        {
            var errors = new List<string>();
            int successCount = 0;

            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                using var package = new ExcelPackage(new FileInfo(filePath));
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var nom = worksheet.Cells[row, 1].Text.Trim();
                        var prenom = worksheet.Cells[row, 2].Text.Trim();
                        var email = worksheet.Cells[row, 3].Text.Trim();
                        var telephone = worksheet.Cells[row, 4].Text.Trim();

                        // Validation
                        if (string.IsNullOrEmpty(nom) || string.IsNullOrEmpty(prenom) || string.IsNullOrEmpty(email))
                        {
                            errors.Add($"Ligne {row}: Nom, Prénom et Email sont obligatoires");
                            continue;
                        }

                        // Vérifier email unique
                        if (await context.Clients.AnyAsync(c => c.Email == email))
                        {
                            errors.Add($"Ligne {row}: Email {email} existe déjà");
                            continue;
                        }

                        // Valider format email
                        if (!email.Contains("@") || !email.Contains("."))
                        {
                            errors.Add($"Ligne {row}: Format email invalide");
                            continue;
                        }

                        // Créer le client avec mot de passe par défaut
                        var client = new Client
                        {
                            Nom = nom,
                            Prenom = prenom,
                            Email = email,
                            Telephone = telephone ?? "",
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!") // Mot de passe par défaut
                        };

                        context.Clients.Add(client);
                        await context.SaveChangesAsync();
                        
                        LogService.LogInfo("CLIENT CRÉÉ (Import Excel) - ID: {ClientId}, Nom: {Nom} {Prenom}, Email: {Email}", 
                            client.Id, client.Nom, client.Prenom, client.Email);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Ligne {row}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Erreur générale: {ex.Message}");
            }

            return (successCount, errors);
        }
    }
}
