# Cinema Ticketing System - ASP.NET MVC with Oracle

A student-friendly, Oracle-based cinema ticketing web application built with ASP.NET MVC, C#, and Bootstrap.

---

## 1. Recommended Project Structure

```
CinemaTicketing/
├── Controllers/        # MVC Controllers (CRUD + Reports)
├── Models/             # Domain models and view models
│   └── ViewModels/     # Report view models
├── Views/              # Razor views
│   ├── Customer/
│   ├── Hall/
│   ├── Movie/
│   ├── Showing/
│   ├── Ticket/
│   ├── UserTicket/
│   ├── TheaterMovie/
│   ├── Occupancy/
│   └── Shared/
├── Data/               # Data access layer
│   ├── OracleHelper.cs
│   ├── *Repository.cs
│   └── ReportRepository.cs
├── wwwroot/
├── Scripts/            # SQL scripts
├── appsettings.json
└── Program.cs
```

---

## 2. Full Folder Structure

```
CinemaTicketing/
├── Controllers/
│   ├── CustomerController.cs
│   ├── HallController.cs
│   ├── MovieController.cs
│   ├── ShowingController.cs
│   ├── TicketController.cs
│   ├── UserTicketController.cs
│   ├── TheaterMovieController.cs
│   ├── OccupancyController.cs
│   └── HomeController.cs
├── Data/
│   ├── OracleHelper.cs
│   ├── CustomerRepository.cs
│   ├── TheaterRepository.cs
│   ├── HallRepository.cs
│   ├── MovieRepository.cs
│   ├── ShowingRepository.cs
│   ├── SeatRepository.cs
│   ├── BookingRepository.cs
│   ├── TicketRepository.cs
│   └── ReportRepository.cs
├── Models/
│   ├── Movie.cs
│   ├── Theater.cs
│   ├── Customer.cs
│   ├── Hall.cs
│   ├── Showing.cs
│   ├── Seat.cs
│   ├── Booking.cs
│   ├── Ticket.cs
│   ├── ErrorViewModel.cs
│   └── ViewModels/
│       ├── UserTicketViewModel.cs
│       ├── TheaterMovieViewModel.cs
│       └── OccupancyViewModel.cs
├── Views/
│   ├── Customer/ (Index, Create, Edit, Delete)
│   ├── Hall/ (Index, Create, Edit, Delete)
│   ├── Movie/ (Index, Create, Edit, Delete)
│   ├── Showing/ (Index, Create, Edit, Delete)
│   ├── Ticket/ (Index, Create, Edit, Delete)
│   ├── UserTicket/ (Index)
│   ├── TheaterMovie/ (Index)
│   ├── Occupancy/ (Index)
│   ├── Home/ (Index)
│   └── Shared/ (_Layout, _ValidationScriptsPartial)
├── wwwroot/
├── Scripts/
│   ├── CreateSchema.sql
│   └── SampleData.sql
├── appsettings.json
├── Program.cs
└── CinemaTicketing.csproj
```

## 11. Steps to Run in Visual Studio

1. **Oracle**
   - Create user/schema and run `Scripts/CreateSchema.sql` and `Scripts/SampleData.sql`.
2. **Connection string**
   - Update `appsettings.json` with your Oracle connection string.
3. **Build**
   - Restore packages: `dotnet restore`
   - Build: `dotnet build`
4. **Run**
   - F5 or `dotnet run` in project folder.
5. **Browser**
   - Open `https://localhost:5001` (or port shown in console).
---
