using Microsoft.EntityFrameworkCore;
using rntlOS.Core.Data;
using rntlOS.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rntlOS.Core.Services
{
    public class VehiculeService
    {
        private readonly AppDbContext _context;

        public VehiculeService(AppDbContext context)
        {
            _context = context;
        }

        // Récupérer tous les véhicules
        public async Task<List<Vehicule>> GetAllAsync()
        {
            return await _context.Vehicules
                .Include(v => v.TypeVehicule)
                .ToListAsync();
        }

        // Récupérer un véhicule par ID
        public async Task<Vehicule?> GetByIdAsync(int id)
        {
            return await _context.Vehicules
                .Include(v => v.TypeVehicule)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        // Ajouter un véhicule
        public async Task<Vehicule> AddAsync(Vehicule vehicule)
        {
            _context.Vehicules.Add(vehicule);
            await _context.SaveChangesAsync();
            return vehicule;
        }

        // Modifier un véhicule
        public async Task<Vehicule?> UpdateAsync(Vehicule vehicule)
        {
            var existing = await _context.Vehicules.FirstOrDefaultAsync(v => v.Id == vehicule.Id);
            if (existing == null) return null;

            existing.Marque = vehicule.Marque;
            existing.Modele = vehicule.Modele;
            existing.Matricule = vehicule.Matricule;
            existing.PrixParJour = vehicule.PrixParJour;
            existing.Disponible = vehicule.Disponible;
            existing.TypeVehiculeId = vehicule.TypeVehiculeId;
            existing.Kilometrage = vehicule.Kilometrage;
            existing.DateMiseEnService = vehicule.DateMiseEnService;
            existing.DerniereMaintenance = vehicule.DerniereMaintenance;

            await _context.SaveChangesAsync();
            return existing;
        }

        // Supprimer un véhicule
        public async Task<bool> DeleteAsync(int id)
        {
            var vehicule = await _context.Vehicules.FirstOrDefaultAsync(v => v.Id == id);
            if (vehicule == null) return false;

            _context.Vehicules.Remove(vehicule);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
