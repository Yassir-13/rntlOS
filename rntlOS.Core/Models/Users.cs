using System;

namespace rntlOS.Core.Models
{
    public enum UserRole
    {
        Admin,
        Employee
    }

    public class User
    {
        public int Id { get; set; }

        // Informations personnelles / affichage
        public string Nom { get; set; }
        public string Prenom { get; set; }

        // Identifiants / sécurité
        public string Email { get; set; }
        public string PasswordHash { get; set; }  // stocke le hash du mot de passe, pas le mot de passe en clair

        // Rôle et statut
        public UserRole Role { get; set; } = UserRole.Employee;
        public bool IsActive { get; set; } = true;

        // Meta
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
