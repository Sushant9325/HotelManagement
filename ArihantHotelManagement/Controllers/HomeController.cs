using ArihantHotelManagement.Data;
using Microsoft.AspNetCore.Mvc;

namespace ArihantHotelManagement.Controllers;

public class HomeController : Controller
{
    private readonly RoomRepository _roomRepository;

    public HomeController(RoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task<IActionResult> Index()
    {
        var rooms = await _roomRepository.GetAllActiveRoomsAsync();
        return View(rooms);
    }
}
