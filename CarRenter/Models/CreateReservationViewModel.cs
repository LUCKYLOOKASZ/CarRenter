namespace CarRenter.Models
{
    public class CreateReservationViewModel
    {
        public int VehicleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Notes { get; set; }
        public List<ReservationDto> ExistingReservations { get; set; } = new List<ReservationDto>();
        public int? CurrentUserId { get; set; }
    }
}
