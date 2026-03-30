using ArihantHotelManagement.Models;
using ArihantHotelManagement.ViewModels;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ArihantHotelManagement.Data;

public class BookingRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public BookingRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> CreateBookingAsync(CreateBookingViewModel model, decimal totalAmount)
    {
        using var conn = _connectionFactory.CreateConnection();
        using var cmd = new SqlCommand("dbo.sp_Booking_Create", conn) { CommandType = CommandType.StoredProcedure };

        cmd.Parameters.AddWithValue("@RoomId", model.RoomId);
        cmd.Parameters.AddWithValue("@GuestName", model.GuestName);
        cmd.Parameters.AddWithValue("@Email", model.Email);
        cmd.Parameters.AddWithValue("@Phone", model.Phone);
        cmd.Parameters.AddWithValue("@CheckInDate", model.CheckInDate);
        cmd.Parameters.AddWithValue("@CheckOutDate", model.CheckOutDate);
        cmd.Parameters.AddWithValue("@Adults", model.Adults);
        cmd.Parameters.AddWithValue("@Children", model.Children);
        cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);

        var bookingIdParam = new SqlParameter("@BookingId", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        cmd.Parameters.Add(bookingIdParam);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();

        return (int)(bookingIdParam.Value ?? 0);
    }

    public async Task UpdateStripeSessionAsync(int bookingId, string stripeSessionId)
    {
        using var conn = _connectionFactory.CreateConnection();
        using var cmd = new SqlCommand("dbo.sp_Booking_UpdateStripeSession", conn) { CommandType = CommandType.StoredProcedure };

        cmd.Parameters.AddWithValue("@BookingId", bookingId);
        cmd.Parameters.AddWithValue("@StripeSessionId", stripeSessionId);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task MarkPaidBySessionIdAsync(string sessionId)
    {
        using var conn = _connectionFactory.CreateConnection();
        using var cmd = new SqlCommand("dbo.sp_Booking_MarkPaidBySessionId", conn) { CommandType = CommandType.StoredProcedure };

        cmd.Parameters.AddWithValue("@StripeSessionId", sessionId);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<IReadOnlyList<Booking>> GetAllAsync()
    {
        var bookings = new List<Booking>();
        using var conn = _connectionFactory.CreateConnection();
        using var cmd = new SqlCommand("dbo.sp_Booking_GetAll", conn) { CommandType = CommandType.StoredProcedure };

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            bookings.Add(new Booking
            {
                BookingId = reader.GetInt32("BookingId"),
                RoomId = reader.GetInt32("RoomId"),
                GuestName = reader.GetString("GuestName"),
                Email = reader.GetString("Email"),
                Phone = reader.GetString("Phone"),
                CheckInDate = reader.GetDateTime("CheckInDate"),
                CheckOutDate = reader.GetDateTime("CheckOutDate"),
                Adults = reader.GetInt32("Adults"),
                Children = reader.GetInt32("Children"),
                TotalAmount = reader.GetDecimal("TotalAmount"),
                BookingStatus = reader.GetString("BookingStatus"),
                PaymentStatus = reader.GetString("PaymentStatus"),
                StripeSessionId = reader["StripeSessionId"] as string,
                CreatedAt = reader.GetDateTime("CreatedAt")
            });
        }

        return bookings;
    }

    public async Task UpdateBookingStatusAsync(int bookingId, string status)
    {
        using var conn = _connectionFactory.CreateConnection();
        using var cmd = new SqlCommand("dbo.sp_Booking_UpdateStatus", conn) { CommandType = CommandType.StoredProcedure };

        cmd.Parameters.AddWithValue("@BookingId", bookingId);
        cmd.Parameters.AddWithValue("@BookingStatus", status);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }
}
