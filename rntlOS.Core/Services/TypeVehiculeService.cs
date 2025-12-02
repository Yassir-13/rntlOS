using Microsoft.EntityFrameworkCore;
using rntlOS.Core.Data;
using rntlOS.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rntlOS.Core.Services
{
    public class TypeVehiculeService
    {
        private readonly AppDbContext _context;

        public TypeVehiculeService(AppDbContext context)
        {
            _context = context;
        }

        // Récupérer tous les types
        public async Task<List<TypeVehicule>> GetAllAsync()
        {
            return await _context.TypesVehicules.ToListAsync();
        }

        // Récupérer un type par ID
        public async Task<TypeVehicule?> GetByIdAsync(int id)
        {
            return await _context.TypesVehicules.FirstOrDefaultAsync(tv => tv.Id == id);
        }

        // Ajouter un type
        public async Task<TypeVehicule> AddAsync(TypeVehicule typeVehicule)
        {
            _context.TypesVehicules.Add(typeVehicule);
            await _context.SaveChangesAsync();
            return typeVehicule;
        }

        // Modifier un type
        public async Task<TypeVehicule?> UpdateAsync(TypeVehicule typeVehicule)
        {
            var existing = await _context.TypesVehicules.FirstOrDefaultAsync(tv => tv.Id == typeVehicule.Id);
            if (existing == null) return null;

            existing.Libelle = typeVehicule.Libelle;

            await _context.SaveChangesAsync();
            return existing;
        }

        // Supprimer un type
        public async Task<bool> DeleteAsync(int id)
        {
            var typeVehicule = await _context.TypesVehicules.FirstOrDefaultAsync(tv => tv.Id == id);
            if (typeVehicule == null) return false;

            _context.TypesVehicules.Remove(typeVehicule);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
