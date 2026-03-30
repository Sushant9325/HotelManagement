using System.ComponentModel.DataAnnotations;

namespace ArihantHotelManagement.ViewModels;

public class CreateBookingViewModel
{
    [Required]
    public int RoomId { get; set; }

    [Required, StringLength(100)]
    public string GuestName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required, DataType(DataType.Date)]
    public DateTime CheckInDate { get; set; }

    [Required, DataType(DataType.Date)]
    public DateTime CheckOutDate { get; set; }

    [Range(1, 10)]
    public int Adults { get; set; } = 1;

    [Range(0, 10)]
    public int Children { get; set; }
}
