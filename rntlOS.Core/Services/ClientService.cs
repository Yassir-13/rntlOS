using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using rntlOS.Core.Data;
using rntlOS.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rntlOS.Core.Services
{
    public class ClientService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public ClientService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        // Récupérer tous les clients
        public async Task<List<Client>> GetAllAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Clients.ToListAsync();
        }

        // Récupérer un client par Id
        public async Task<Client?> GetByIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Clients.FirstOrDefaultAsync(c => c.Id == id);
        }

        // Ajouter un client
        public async Task<Client> AddAsync(Client client)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Clients.Add(client);
            await context.SaveChangesAsync();
            
            LogService.LogInfo("CLIENT CRÉÉ - ID: {ClientId}, Nom: {Nom} {Prenom}, Email: {Email}", 
                client.Id, client.Nom, client.Prenom, client.Email);
            
            return client;
        }

        // Modifier un client
        public async Task<Client?> UpdateAsync(Client client)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var existing = await context.Clients.FirstOrDefaultAsync(c => c.Id == client.Id);
            if (existing == null) return null;

            existing.Nom = client.Nom;
            existing.Prenom = client.Prenom;
            existing.Email = client.Email;
            existing.Telephone = client.Telephone;
            existing.PasswordHash = client.PasswordHash;

            await context.SaveChangesAsync();
            return existing;
        }

        // Supprimer un client
        public async Task<bool> DeleteAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == id);
            if (client == null) return false;

            context.Clients.Remove(client);
            await context.SaveChangesAsync();
            
            LogService.LogWarning("CLIENT SUPPRIMÉ - ID: {ClientId}, Nom: {Nom} {Prenom}", 
                id, client.Nom, client.Prenom);
            
            return true;
        }

        public async Task<Client?> GetByEmailAsync(string email)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Clients.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Client> RegisterAsync(Client client, string password)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            client.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            client.CreatedAt = DateTime.Now;

            context.Clients.Add(client);
            await context.SaveChangesAsync();
            return client;
        }
    }
}
