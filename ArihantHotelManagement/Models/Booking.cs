namespace ArihantHotelManagement.Models;

public class Booking
{
    public int BookingId { get; set; }
    public int RoomId { get; set; }
    public string GuestName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
    public decimal TotalAmount { get; set; }
    public string BookingStatus { get; set; } = "Pending";
    public string PaymentStatus { get; set; } = "Unpaid";
    public string? StripeSessionId { get; set; }
    public DateTime CreatedAt { get; set; }
}
