using Microsoft.EntityFrameworkCore;
using rntlOS.Core.Data;
using rntlOS.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rntlOS.Core.Services
{
    public class LogService
    {
        private readonly AppDbContext _context;

        public LogService(AppDbContext context)
        {
            _context = context;
        }

        // Récupérer tous les logs
        public async Task<List<Log>> GetAllAsync()
        {
            return await _context.Logs
                .Include(l => l.User)
                .ToListAsync();
        }

        // Récupérer les logs d’un utilisateur
        public async Task<List<Log>> GetByUserIdAsync(int userId)
        {
            return await _context.Logs
                .Where(l => l.UserId == userId)
                .Include(l => l.User)
                .ToListAsync();
        }

        // Ajouter un log
        public async Task<Log> AddAsync(Log log)
        {
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
            return log;
        }

        // Supprimer un log
        public async Task<bool> DeleteAsync(int id)
        {
            var log = await _context.Logs.FirstOrDefaultAsync(l => l.Id == id);
            if (log == null) return false;

            _context.Logs.Remove(log);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
