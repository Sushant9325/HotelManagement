using ArihantHotelManagement.Areas.Admin.ViewModels;
using ArihantHotelManagement.Data;
using Microsoft.AspNetCore.Mvc;

namespace ArihantHotelManagement.Areas.Admin.Controllers;

[Area("Admin")]
public class DashboardController : Controller
{
    private readonly RoomRepository _roomRepository;
    private readonly BookingRepository _bookingRepository;

    public DashboardController(RoomRepository roomRepository, BookingRepository bookingRepository)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<IActionResult> Index()
    {
        var rooms = await _roomRepository.GetAllActiveRoomsAsync();
        var bookings = await _bookingRepository.GetAllAsync();

        var vm = new AdminDashboardViewModel
        {
            TotalRooms = rooms.Count,
            TotalBookings = bookings.Count,
            PendingBookings = bookings.Count(x => x.BookingStatus == "Pending"),
            ConfirmedBookings = bookings.Count(x => x.BookingStatus == "Confirmed"),
            Revenue = bookings.Where(x => x.PaymentStatus == "Paid").Sum(x => x.TotalAmount),
            RecentBookings = bookings.OrderByDescending(x => x.CreatedAt).Take(10).ToList()
        };

        return View(vm);
    }
}
