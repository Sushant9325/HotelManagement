using ArihantHotelManagement.Models;

namespace ArihantHotelManagement.Services;

public interface IStripeCheckoutService
{
    Task<string> CreateCheckoutSessionAsync(Booking booking, Room room, string successUrl, string cancelUrl);
}
