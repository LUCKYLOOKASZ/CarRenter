namespace VehicleService.Dtos
{
    public class CreateVehicleDto
    {
        public string Marka { get; set; }
        public string Model { get; set; }
        public int Rok { get; set; }
        public int KwotaZaDzien { get; set; }
        public string Opis { get; set; }
        public string UrlObrazka { get; set; }
    }
}
