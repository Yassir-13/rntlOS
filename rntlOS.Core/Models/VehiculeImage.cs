namespace rntlOS.Core.Models
{
    public class VehiculeImage
    {
        public int Id { get; set; }

        // Chemin de l'image (URL, path local, ou base64)
        public string ImagePath { get; set; }

        // Relation avec le véhicule
        public int VehiculeId { get; set; }
        public Vehicule Vehicule { get; set; }
    }
}
