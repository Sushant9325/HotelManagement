# ARIHANT Hotel Management (ASP.NET Core MVC + ADO.NET + Stripe Test Kit)

A fresh **ASP.NET Core MVC** hotel management project with:
- Public booking flow
- Stripe test payment integration
- Admin workflow control panel
- ADO.NET with SQL Server stored procedures
- Solution file for Visual Studio

## Open in Visual Studio
1. Open `ArihantHotelManagement.sln`
2. Restore NuGet packages
3. Execute SQL script: `ArihantHotelManagement/Sql/arihanthotel_full_setup.sql`
4. Update `appsettings.json` connection string and Stripe test keys
5. Run project

## Stripe Test Card
- Card: `4242 4242 4242 4242`
- Exp: any future date
- CVC: any 3 digits
- ZIP: any valid format

## Project Structure
- `ArihantHotelManagement/` -> MVC app
- `ArihantHotelManagement/Sql/` -> Full DB + table + stored procedure setup
- `ArihantHotelManagement.sln` -> Visual Studio solution
