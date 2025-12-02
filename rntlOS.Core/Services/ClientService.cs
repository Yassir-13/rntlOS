using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using rntlOS.Core.Data;
using rntlOS.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rntlOS.Core.Services
{
    public class ClientService
    {
        private readonly AppDbContext _context;

        public ClientService(AppDbContext context)
        {
            _context = context;
        }

        // Récupérer tous les clients
        public async Task<List<Client>> GetAllAsync()
        {
            return await _context.Clients.ToListAsync();
        }

        // Récupérer un client par Id
        public async Task<Client?> GetByIdAsync(int id)
        {
            return await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);
        }

        // Ajouter un client
        public async Task<Client> AddAsync(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return client;
        }

        // Modifier un client
        public async Task<Client?> UpdateAsync(Client client)
        {
            var existing = await _context.Clients.FirstOrDefaultAsync(c => c.Id == client.Id);
            if (existing == null) return null;

            existing.Nom = client.Nom;
            existing.Prenom = client.Prenom;
            existing.Email = client.Email;
            existing.Telephone = client.Telephone;
            existing.PasswordHash = client.PasswordHash;

            await _context.SaveChangesAsync();
            return existing;
        }

        // Supprimer un client
        public async Task<bool> DeleteAsync(int id)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);
            if (client == null) return false;

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Client?> GetByEmailAsync(string email)
        {
            return await _context.Clients.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Client> RegisterAsync(Client client, string password)
        {
            client.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            client.CreatedAt = DateTime.Now;

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return client;
        }
    }
}
