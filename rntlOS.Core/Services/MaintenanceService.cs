using Microsoft.EntityFrameworkCore;
using rntlOS.Core.Data;
using rntlOS.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rntlOS.Core.Services
{
    public class MaintenanceService
    {
        private readonly AppDbContext _context;

        public MaintenanceService(AppDbContext context)
        {
            _context = context;
        }

        // Récupérer toutes les maintenances
        public async Task<List<Maintenance>> GetAllAsync()
        {
            return await _context.Maintenances
                .Include(m => m.Vehicule)
                .ToListAsync();
        }

        // Récupérer une maintenance par ID
        public async Task<Maintenance?> GetByIdAsync(int id)
        {
            return await _context.Maintenances
                .Include(m => m.Vehicule)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        // Récupérer les maintenances d’un véhicule
        public async Task<List<Maintenance>> GetByVehiculeIdAsync(int vehiculeId)
        {
            return await _context.Maintenances
                .Where(m => m.VehiculeId == vehiculeId)
                .ToListAsync();
        }

        // Ajouter une maintenance
        public async Task<Maintenance> AddAsync(Maintenance maintenance)
        {
            _context.Maintenances.Add(maintenance);
            await _context.SaveChangesAsync();
            return maintenance;
        }

        // Modifier
        public async Task<Maintenance?> UpdateAsync(Maintenance maintenance)
        {
            var existing = await _context.Maintenances.FirstOrDefaultAsync(m => m.Id == maintenance.Id);
            if (existing == null) return null;

            existing.Description = maintenance.Description;
            existing.DateMaintenance = maintenance.DateMaintenance;
            existing.KilometrageAtMaintenance = maintenance.KilometrageAtMaintenance;
            existing.ProchaineMaintenance = maintenance.ProchaineMaintenance;
            existing.IsCompleted = maintenance.IsCompleted;

            await _context.SaveChangesAsync();
            return existing;
        }

        // Supprimer
        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _context.Maintenances.FirstOrDefaultAsync(m => m.Id == id);
            if (item == null) return false;

            _context.Maintenances.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
