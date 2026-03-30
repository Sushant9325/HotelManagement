namespace ArihantHotelManagement.Models;

public class Room
{
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int MaxGuests { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? HeroImageUrl { get; set; }
}
