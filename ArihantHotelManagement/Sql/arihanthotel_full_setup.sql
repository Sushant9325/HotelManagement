/* ARIHANT Hotel Management - Full SQL Setup */

IF DB_ID('ArihantHotelDb') IS NULL
BEGIN
    CREATE DATABASE ArihantHotelDb;
END
GO

USE ArihantHotelDb;
GO

IF OBJECT_ID('dbo.Bookings', 'U') IS NOT NULL DROP TABLE dbo.Bookings;
IF OBJECT_ID('dbo.Rooms', 'U') IS NOT NULL DROP TABLE dbo.Rooms;
GO

CREATE TABLE dbo.Rooms
(
    RoomId INT IDENTITY(1,1) PRIMARY KEY,
    RoomNumber NVARCHAR(20) NOT NULL UNIQUE,
    RoomType NVARCHAR(60) NOT NULL,
    PricePerNight DECIMAL(12,2) NOT NULL,
    MaxGuests INT NOT NULL,
    Description NVARCHAR(400) NOT NULL,
    HeroImageUrl NVARCHAR(400) NULL,
    IsActive BIT NOT NULL DEFAULT(1),
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NULL
);
GO

CREATE TABLE dbo.Bookings
(
    BookingId INT IDENTITY(1,1) PRIMARY KEY,
    RoomId INT NOT NULL,
    GuestName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(120) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    CheckInDate DATE NOT NULL,
    CheckOutDate DATE NOT NULL,
    Adults INT NOT NULL,
    Children INT NOT NULL,
    TotalAmount DECIMAL(12,2) NOT NULL,
    BookingStatus NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    PaymentStatus NVARCHAR(20) NOT NULL DEFAULT 'Unpaid',
    StripeSessionId NVARCHAR(120) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT FK_Bookings_Room FOREIGN KEY(RoomId) REFERENCES dbo.Rooms(RoomId)
);
GO

INSERT INTO dbo.Rooms(RoomNumber, RoomType, PricePerNight, MaxGuests, Description, HeroImageUrl, IsActive)
VALUES
('101', 'Deluxe', 180, 2, 'Luxury interior with city view.', 'https://images.unsplash.com/photo-1566073771259-6a8506099945?w=1200', 1),
('201', 'Executive Suite', 320, 4, 'Premium suite with living lounge.', 'https://images.unsplash.com/photo-1590490360182-c33d57733427?w=1200', 1),
('301', 'Presidential Suite', 750, 6, 'Ultra premium experience at ARIHANT.', 'https://images.unsplash.com/photo-1578683010236-d716f9a3f461?w=1200', 1);
GO

CREATE OR ALTER PROCEDURE dbo.sp_Room_GetAllActive
AS
BEGIN
    SET NOCOUNT ON;
    SELECT RoomId, RoomNumber, RoomType, PricePerNight, MaxGuests, Description, HeroImageUrl, IsActive
    FROM dbo.Rooms
    WHERE IsActive = 1
    ORDER BY PricePerNight;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Room_GetById
    @RoomId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT RoomId, RoomNumber, RoomType, PricePerNight, MaxGuests, Description, HeroImageUrl, IsActive
    FROM dbo.Rooms
    WHERE RoomId = @RoomId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Room_Upsert
    @RoomId INT,
    @RoomNumber NVARCHAR(20),
    @RoomType NVARCHAR(60),
    @PricePerNight DECIMAL(12,2),
    @MaxGuests INT,
    @Description NVARCHAR(400),
    @IsActive BIT,
    @HeroImageUrl NVARCHAR(400) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Rooms WHERE RoomId = @RoomId AND @RoomId > 0)
    BEGIN
        UPDATE dbo.Rooms
        SET RoomNumber = @RoomNumber,
            RoomType = @RoomType,
            PricePerNight = @PricePerNight,
            MaxGuests = @MaxGuests,
            Description = @Description,
            IsActive = @IsActive,
            HeroImageUrl = @HeroImageUrl,
            UpdatedAt = SYSUTCDATETIME()
        WHERE RoomId = @RoomId;
    END
    ELSE
    BEGIN
        INSERT INTO dbo.Rooms(RoomNumber, RoomType, PricePerNight, MaxGuests, Description, HeroImageUrl, IsActive)
        VALUES(@RoomNumber, @RoomType, @PricePerNight, @MaxGuests, @Description, @HeroImageUrl, @IsActive);
    END
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Booking_Create
    @RoomId INT,
    @GuestName NVARCHAR(100),
    @Email NVARCHAR(120),
    @Phone NVARCHAR(20),
    @CheckInDate DATE,
    @CheckOutDate DATE,
    @Adults INT,
    @Children INT,
    @TotalAmount DECIMAL(12,2),
    @BookingId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Bookings
    (RoomId, GuestName, Email, Phone, CheckInDate, CheckOutDate, Adults, Children, TotalAmount)
    VALUES
    (@RoomId, @GuestName, @Email, @Phone, @CheckInDate, @CheckOutDate, @Adults, @Children, @TotalAmount);

    SET @BookingId = SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Booking_UpdateStripeSession
    @BookingId INT,
    @StripeSessionId NVARCHAR(120)
AS
BEGIN
    UPDATE dbo.Bookings
    SET StripeSessionId = @StripeSessionId,
        UpdatedAt = SYSUTCDATETIME()
    WHERE BookingId = @BookingId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Booking_MarkPaidBySessionId
    @StripeSessionId NVARCHAR(120)
AS
BEGIN
    UPDATE dbo.Bookings
    SET PaymentStatus = 'Paid',
        BookingStatus = 'Confirmed',
        UpdatedAt = SYSUTCDATETIME()
    WHERE StripeSessionId = @StripeSessionId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Booking_GetAll
AS
BEGIN
    SELECT BookingId, RoomId, GuestName, Email, Phone, CheckInDate, CheckOutDate,
           Adults, Children, TotalAmount, BookingStatus, PaymentStatus, StripeSessionId, CreatedAt
    FROM dbo.Bookings
    ORDER BY BookingId DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Booking_UpdateStatus
    @BookingId INT,
    @BookingStatus NVARCHAR(20)
AS
BEGIN
    UPDATE dbo.Bookings
    SET BookingStatus = @BookingStatus,
        UpdatedAt = SYSUTCDATETIME()
    WHERE BookingId = @BookingId;
END
GO
