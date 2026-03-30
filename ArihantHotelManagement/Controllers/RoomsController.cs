using ArihantHotelManagement.Data;
using Microsoft.AspNetCore.Mvc;

namespace ArihantHotelManagement.Controllers;

public class RoomsController : Controller
{
    private readonly RoomRepository _roomRepository;

    public RoomsController(RoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task<IActionResult> Index()
    {
        var rooms = await _roomRepository.GetAllActiveRoomsAsync();
        return View(rooms);
    }
}
