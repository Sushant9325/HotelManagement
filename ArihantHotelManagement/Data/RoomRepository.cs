using ArihantHotelManagement.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ArihantHotelManagement.Data;

public class RoomRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public RoomRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Room>> GetAllActiveRoomsAsync()
    {
        var rooms = new List<Room>();
        using var conn = _connectionFactory.CreateConnection();
        using var cmd = new SqlCommand("dbo.sp_Room_GetAllActive", conn) { CommandType = CommandType.StoredProcedure };

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            rooms.Add(new Room
            {
                RoomId = reader.GetInt32("RoomId"),
                RoomNumber = reader.GetString("RoomNumber"),
                RoomType = reader.GetString("RoomType"),
                PricePerNight = reader.GetDecimal("PricePerNight"),
                MaxGuests = reader.GetInt32("MaxGuests"),
                Description = reader.GetString("Description"),
                IsActive = reader.GetBoolean("IsActive"),
                HeroImageUrl = reader["HeroImageUrl"] as string
            });
        }

        return rooms;
    }

    public async Task<Room?> GetByIdAsync(int roomId)
    {
        using var conn = _connectionFactory.CreateConnection();
        using var cmd = new SqlCommand("dbo.sp_Room_GetById", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@RoomId", roomId);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new Room
        {
            RoomId = reader.GetInt32("RoomId"),
            RoomNumber = reader.GetString("RoomNumber"),
            RoomType = reader.GetString("RoomType"),
            PricePerNight = reader.GetDecimal("PricePerNight"),
            MaxGuests = reader.GetInt32("MaxGuests"),
            Description = reader.GetString("Description"),
            IsActive = reader.GetBoolean("IsActive"),
            HeroImageUrl = reader["HeroImageUrl"] as string
        };
    }

    public async Task UpsertRoomAsync(Room room)
    {
        using var conn = _connectionFactory.CreateConnection();
        using var cmd = new SqlCommand("dbo.sp_Room_Upsert", conn) { CommandType = CommandType.StoredProcedure };

        cmd.Parameters.AddWithValue("@RoomId", room.RoomId);
        cmd.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);
        cmd.Parameters.AddWithValue("@RoomType", room.RoomType);
        cmd.Parameters.AddWithValue("@PricePerNight", room.PricePerNight);
        cmd.Parameters.AddWithValue("@MaxGuests", room.MaxGuests);
        cmd.Parameters.AddWithValue("@Description", room.Description);
        cmd.Parameters.AddWithValue("@IsActive", room.IsActive);
        cmd.Parameters.AddWithValue("@HeroImageUrl", room.HeroImageUrl ?? (object)DBNull.Value);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }
}
