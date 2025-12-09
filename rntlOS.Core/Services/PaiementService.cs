using Microsoft.EntityFrameworkCore;
using rntlOS.Core.Data;
using rntlOS.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rntlOS.Core.Services
{
    public class PaiementService
    {
        private readonly AppDbContext _context;

        public PaiementService(AppDbContext context)
        {
            _context = context;
        }

        // Récupérer tous les paiements
        public async Task<List<Paiement>> GetAllAsync()
        {
            return await _context.Paiements
                .Include(p => p.Booking)
                .ThenInclude(b => b.Client)
                .ToListAsync();
        }

        // Récupérer un paiement par ID
        public async Task<Paiement?> GetByIdAsync(int id)
        {
            return await _context.Paiements
                .Include(p => p.Booking)
                .ThenInclude(b => b.Client)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // Récupérer les paiements d’une réservation
        public async Task<List<Paiement>> GetByBookingIdAsync(int bookingId)
        {
            return await _context.Paiements
                .Where(p => p.BookingId == bookingId)
                .ToListAsync();
        }

        // Ajouter un paiement
        public async Task<Paiement> AddAsync(Paiement paiement)
        {
            _context.Paiements.Add(paiement);
            await _context.SaveChangesAsync();
            
            LogService.LogPaiementReceived(paiement.Montant, paiement.Methode.ToString(), paiement.BookingId);
            
            return paiement;
        }

        // Modifier un paiement
        public async Task<Paiement?> UpdateAsync(Paiement paiement)
        {
            var existing = await _context.Paiements.FirstOrDefaultAsync(p => p.Id == paiement.Id);
            if (existing == null) return null;

            existing.Montant = paiement.Montant;
            existing.Methode = paiement.Methode;
            existing.Statut = paiement.Statut;
            existing.DatePaiement = paiement.DatePaiement;

            await _context.SaveChangesAsync();
            return existing;
        }

        // Supprimer un paiement
        public async Task<bool> DeleteAsync(int id)
        {
            var paiement = await _context.Paiements.FirstOrDefaultAsync(p => p.Id == id);
            if (paiement == null) return false;

            _context.Paiements.Remove(paiement);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
