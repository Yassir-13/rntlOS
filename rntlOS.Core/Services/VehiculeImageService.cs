using Microsoft.EntityFrameworkCore;
using rntlOS.Core.Data;
using rntlOS.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rntlOS.Core.Services
{
    public class VehiculeImageService
    {
        private readonly AppDbContext _context;

        public VehiculeImageService(AppDbContext context)
        {
            _context = context;
        }

        // Récupérer toutes les images
        public async Task<List<VehiculeImage>> GetAllAsync()
        {
            return await _context.VehiculeImages
                .Include(i => i.Vehicule)
                .ToListAsync();
        }

        // Récupérer les images d’un seul véhicule
        public async Task<List<VehiculeImage>> GetByVehiculeIdAsync(int vehiculeId)
        {
            return await _context.VehiculeImages
                .Where(i => i.VehiculeId == vehiculeId)
                .ToListAsync();
        }

        // Ajouter une image
        public async Task<VehiculeImage> AddAsync(VehiculeImage image)
        {
            _context.VehiculeImages.Add(image);
            await _context.SaveChangesAsync();
            return image;
        }

        // Supprimer une image
        public async Task<bool> DeleteAsync(int id)
        {
            var img = await _context.VehiculeImages.FirstOrDefaultAsync(i => i.Id == id);
            if (img == null) return false;

            _context.VehiculeImages.Remove(img);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
