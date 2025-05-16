using System.ComponentModel.DataAnnotations;

namespace VehicleService.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string Marka { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Rok { get; set; }
        public decimal KwotaZaDzien { get; set; }
        public string Opis { get; set; } = string.Empty;
        public string UrlObrazka { get; set; } = string.Empty;
    }
}
