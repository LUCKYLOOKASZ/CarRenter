namespace VehicleService.Dtos
{
    public class VehicleDto
    {
        public int Id { get; set; }
        public string Marka { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Rok { get; set; }
        public int KwotaZaDzien { get; set; }
        public string Opis { get; set; } = string.Empty;
        public string UrlObrazka { get; set; } = string.Empty;
    }

}
