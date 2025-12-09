using Microsoft.EntityFrameworkCore;
using rntlOS.Core.Data;
using rntlOS.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rntlOS.Core.Services
{
    public class BookingService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public BookingService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        // Récupérer toutes les réservations
        public async Task<List<Booking>> GetAllAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Bookings
                .Include(b => b.Client)
                .Include(b => b.Vehicule)
                .ThenInclude(v => v.TypeVehicule)
                .ToListAsync();
        }

        // Récupérer une réservation par Id
        public async Task<Booking?> GetByIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Bookings
                .Include(b => b.Client)
                .Include(b => b.Vehicule)
                .ThenInclude(v => v.TypeVehicule)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        // Récupérer les réservations d'un client
        public async Task<List<Booking>> GetByClientIdAsync(int clientId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Bookings
                .Include(b => b.Vehicule)
                .Where(b => b.ClientId == clientId)
                .ToListAsync();
        }

        // Récupérer les réservations d'un véhicule
        public async Task<List<Booking>> GetByVehiculeIdAsync(int vehiculeId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Bookings
                .Include(b => b.Client)
                .Where(b => b.VehiculeId == vehiculeId)
                .ToListAsync();
        }

        // Ajouter une réservation
        public async Task<Booking> AddAsync(Booking booking)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Bookings.Add(booking);
            await context.SaveChangesAsync();
            
            // Charger le client pour le log
            var client = await context.Clients.FindAsync(booking.ClientId);
            if (client != null)
            {
                LogService.LogReservationCreated(booking.Id, client.Email, booking.VehiculeId);
            }
            
            return booking;
        }

        // Modifier une réservation
        public async Task<Booking?> UpdateAsync(Booking booking)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var existing = await context.Bookings.FirstOrDefaultAsync(b => b.Id == booking.Id);
            if (existing == null) return null;

            existing.DateDebut = booking.DateDebut;
            existing.DateFin = booking.DateFin;
            existing.PrixTotal = booking.PrixTotal;
            existing.Status = booking.Status;

            await context.SaveChangesAsync();
            return existing;
        }

        // Supprimer une réservation
        public async Task<bool> DeleteAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var booking = await context.Bookings.FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null) return false;

            context.Bookings.Remove(booking);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> VehiculeEstDisponible(int vehiculeId, DateTime dateDebut, DateTime dateFin, int? bookingIdExclure = null)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var reservationsConflictuelles = await context.Bookings
                .Where(b => b.VehiculeId == vehiculeId
                         && b.Status != StatutReservation.Annulee
                         && (bookingIdExclure == null || b.Id != bookingIdExclure)
                         && ((dateDebut >= b.DateDebut && dateDebut <= b.DateFin)
                          || (dateFin >= b.DateDebut && dateFin <= b.DateFin)
                          || (dateDebut <= b.DateDebut && dateFin >= b.DateFin)))
                .AnyAsync();

            return !reservationsConflictuelles;
        }
    }
}
