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

        public async Task<List<VehiculeImage>> GetAllAsync()
        {
            return await _context.VehiculeImages
                .Include(vi => vi.Vehicule)
                .ToListAsync();
        }

        public async Task<VehiculeImage> GetByIdAsync(int id)
        {
            return await _context.VehiculeImages
                .Include(vi => vi.Vehicule)
                .FirstOrDefaultAsync(vi => vi.Id == id);
        }

        public async Task AddAsync(VehiculeImage image)
        {
            _context.VehiculeImages.Add(image);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(VehiculeImage image)
        {
            _context.VehiculeImages.Update(image);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var image = await _context.VehiculeImages.FindAsync(id);
            if (image != null)
            {
                _context.VehiculeImages.Remove(image);
                await _context.SaveChangesAsync();
            }
        }
    }
}