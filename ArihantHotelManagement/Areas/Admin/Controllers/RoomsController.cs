using ArihantHotelManagement.Data;
using ArihantHotelManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace ArihantHotelManagement.Areas.Admin.Controllers;

[Area("Admin")]
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

    [HttpGet]
    public IActionResult Edit(int id = 0)
    {
        return View(new Room { RoomId = id, IsActive = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Room room)
    {
        if (!ModelState.IsValid)
        {
            return View(room);
        }

        await _roomRepository.UpsertRoomAsync(room);
        return RedirectToAction(nameof(Index));
    }
}
