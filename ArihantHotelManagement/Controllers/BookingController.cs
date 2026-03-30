using ArihantHotelManagement.Data;
using ArihantHotelManagement.Models;
using ArihantHotelManagement.Services;
using ArihantHotelManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ArihantHotelManagement.Controllers;

public class BookingController : Controller
{
    private readonly RoomRepository _roomRepository;
    private readonly BookingRepository _bookingRepository;
    private readonly IStripeCheckoutService _stripeCheckoutService;

    public BookingController(
        RoomRepository roomRepository,
        BookingRepository bookingRepository,
        IStripeCheckoutService stripeCheckoutService)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
        _stripeCheckoutService = stripeCheckoutService;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int roomId)
    {
        var room = await _roomRepository.GetByIdAsync(roomId);
        if (room is null)
        {
            return NotFound();
        }

        ViewBag.Room = room;
        var model = new CreateBookingViewModel
        {
            RoomId = room.RoomId,
            CheckInDate = DateTime.Today.AddDays(1),
            CheckOutDate = DateTime.Today.AddDays(2)
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBookingViewModel model)
    {
        var room = await _roomRepository.GetByIdAsync(model.RoomId);
        if (room is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Room = room;
            return View(model);
        }

        var nights = Math.Max(1, (model.CheckOutDate - model.CheckInDate).Days);
        var totalAmount = nights * room.PricePerNight;

        var bookingId = await _bookingRepository.CreateBookingAsync(model, totalAmount);
        var booking = new Booking
        {
            BookingId = bookingId,
            RoomId = room.RoomId,
            GuestName = model.GuestName,
            Email = model.Email,
            Phone = model.Phone,
            CheckInDate = model.CheckInDate,
            CheckOutDate = model.CheckOutDate,
            Adults = model.Adults,
            Children = model.Children,
            TotalAmount = totalAmount
        };

        var successUrl = Url.Action("PaymentSuccess", "Booking", null, Request.Scheme) + "?session_id={CHECKOUT_SESSION_ID}";
        var cancelUrl = Url.Action("PaymentCancel", "Booking", null, Request.Scheme)!;

        var sessionId = await _stripeCheckoutService.CreateCheckoutSessionAsync(booking, room, successUrl!, cancelUrl);
        await _bookingRepository.UpdateStripeSessionAsync(bookingId, sessionId);

        return Redirect($"https://checkout.stripe.com/c/pay/{sessionId}");
    }

    [HttpGet]
    public async Task<IActionResult> PaymentSuccess(string session_id)
    {
        if (!string.IsNullOrWhiteSpace(session_id))
        {
            await _bookingRepository.MarkPaidBySessionIdAsync(session_id);
        }

        ViewBag.SessionId = session_id;
        return View();
    }

    [HttpGet]
    public IActionResult PaymentCancel()
    {
        return View();
    }
}
