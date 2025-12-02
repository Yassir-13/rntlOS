using System;

namespace rntlOS.Core.Models
{
    public class Client
    {
        public int Id { get; set; }

        // Informations personnelles
        public string Nom { get; set; }
        public string Prenom { get; set; }

        // Contact
        public string Email { get; set; }
        public string Telephone { get; set; }

        // Authentification FrontOffice
        public string PasswordHash { get; set; }

        // Meta
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
