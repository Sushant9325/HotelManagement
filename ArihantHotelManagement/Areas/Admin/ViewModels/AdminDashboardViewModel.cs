using ArihantHotelManagement.Models;

namespace ArihantHotelManagement.Areas.Admin.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalRooms { get; set; }
    public int TotalBookings { get; set; }
    public int PendingBookings { get; set; }
    public int ConfirmedBookings { get; set; }
    public decimal Revenue { get; set; }
    public IReadOnlyList<Booking> RecentBookings { get; set; } = Array.Empty<Booking>();
}
