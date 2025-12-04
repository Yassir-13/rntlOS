namespace rntlOS.Core.Models
{
    public class VehiculeImage
    {
        public int Id { get; set; }
        public int VehiculeId { get; set; }
        public Vehicule Vehicule { get; set; }
        public string ImagePath { get; set; }
    }
}