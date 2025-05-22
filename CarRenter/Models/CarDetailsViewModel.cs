namespace CarRenter.Models
{
    public class CarDetailsViewModel
    {
        public VehicleViewModel Vehicle { get; set; }
        public List<ReservationDto> Reservations { get; set; }
    }

}
