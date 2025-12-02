using Microsoft.EntityFrameworkCore;
using rntlOS.Core.Data;
using rntlOS.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rntlOS.Core.Services
{
    public class BookingService
    {
        private readonly AppDbContext _context;

        public BookingService(AppDbContext context)
        {
            _context = context;
        }

        // Récupérer toutes les réservations
        public async Task<List<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.Client)
                .Include(b => b.Vehicule)
                .ThenInclude(v => v.TypeVehicule)
                .ToListAsync();
        }

        // Récupérer une réservation par Id
        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Client)
                .Include(b => b.Vehicule)
                .ThenInclude(v => v.TypeVehicule)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        // Récupérer les réservations d’un client
        public async Task<List<Booking>> GetByClientIdAsync(int clientId)
        {
            return await _context.Bookings
                .Include(b => b.Vehicule)
                .Where(b => b.ClientId == clientId)
                .ToListAsync();
        }

        // Récupérer les réservations d’un véhicule
        public async Task<List<Booking>> GetByVehiculeIdAsync(int vehiculeId)
        {
            return await _context.Bookings
                .Include(b => b.Client)
                .Where(b => b.VehiculeId == vehiculeId)
                .ToListAsync();
        }

        // Ajouter une réservation
        public async Task<Booking> AddAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        // Modifier une réservation
        public async Task<Booking?> UpdateAsync(Booking booking)
        {
            var existing = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == booking.Id);
            if (existing == null) return null;

            existing.DateDebut = booking.DateDebut;
            existing.DateFin = booking.DateFin;
            existing.PrixTotal = booking.PrixTotal;
            existing.Status = booking.Status;

            await _context.SaveChangesAsync();
            return existing;
        }

        // Supprimer une réservation
        public async Task<bool> DeleteAsync(int id)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null) return false;

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
