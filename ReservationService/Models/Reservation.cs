namespace ReservationService.Models
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UserId { get; set; }
        public int VehicleId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Notes { get; set; }
    }
}
