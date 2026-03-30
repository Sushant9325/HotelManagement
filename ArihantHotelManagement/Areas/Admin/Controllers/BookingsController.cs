using ArihantHotelManagement.Data;
using Microsoft.AspNetCore.Mvc;

namespace ArihantHotelManagement.Areas.Admin.Controllers;

[Area("Admin")]
public class BookingsController : Controller
{
    private readonly BookingRepository _bookingRepository;

    public BookingsController(BookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<IActionResult> Index()
    {
        var bookings = await _bookingRepository.GetAllAsync();
        return View(bookings.OrderByDescending(x => x.CreatedAt));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int bookingId, string status)
    {
        await _bookingRepository.UpdateBookingStatusAsync(bookingId, status);
        return RedirectToAction(nameof(Index));
    }
}
