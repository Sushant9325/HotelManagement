using ArihantHotelManagement.Models;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace ArihantHotelManagement.Services;

public class StripeCheckoutService : IStripeCheckoutService
{
    private readonly StripeSettings _stripeSettings;

    public StripeCheckoutService(IOptions<StripeSettings> stripeSettings)
    {
        _stripeSettings = stripeSettings.Value;
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
    }

    public async Task<string> CreateCheckoutSessionAsync(Booking booking, Room room, string successUrl, string cancelUrl)
    {
        var totalNights = Math.Max(1, (booking.CheckOutDate - booking.CheckInDate).Days);
        var totalAmount = booking.TotalAmount;
        var options = new SessionCreateOptions
        {
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
            Mode = "payment",
            CustomerEmail = booking.Email,
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmountDecimal = totalAmount * 100,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"ARIHANT - {room.RoomType} Room Booking",
                            Description = $"{totalNights} nights stay in room {room.RoomNumber}."
                        }
                    }
                }
            },
            Metadata = new Dictionary<string, string>
            {
                ["BookingId"] = booking.BookingId.ToString()
            }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);
        return session.Id;
    }
}
