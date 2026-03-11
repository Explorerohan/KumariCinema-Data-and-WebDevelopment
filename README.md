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

---

## 3. Database Assumptions

- **Oracle 11g+** (or XE)
- **User/Schema**: `CINEMA_USER` (or adjust connection string)
- **Sequence/Identity**: `NUMBER GENERATED ALWAYS AS IDENTITY` (Oracle 12c+). For 11g, use sequences instead.
- **SHOWTIME**: Stored as `DATE` (time portion only). App binds `TimeSpan`.
- **Naming**: Oracle uppercase column names; code uses `OracleHelper` to read by column name.
- **Referential integrity**: ON DELETE CASCADE on FKs where appropriate.

---

## 4. Required NuGet Packages

- **Microsoft.AspNetCore.App** (implicit, part of web SDK)
- **Oracle.ManagedDataAccess** (23.x or latest)

---

## 5. Configuration and Connection String Setup

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "OracleConnection": "Data Source=localhost:1521/XE;User Id=CINEMA_USER;Password=your_password;"
  }
}
```

**Common Data Source formats:**
- `localhost:1521/XE` – Oracle XE
- `(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=host)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCL)))`

---

## 6–9. Code Summary

- **Models**: Entity classes with `[Required]`, `[Display]`, `[Range]`, etc.
- **Data layer**: `OracleHelper` + repositories using parameterized queries.
- **Controllers**: CRUD actions + TempData for success/error messages.
- **Views**: Bootstrap tables/forms, dropdowns for FKs, validation partial.

---

## 10. SQL Queries for the 3 Complex Pages

### A. User Ticket (Last 6 Months)

```sql
SELECT c.FULLNAME AS USERNAME, m.TITLE AS MOVIETITLE, t.THEATERNAME, h.HALLNUMBER,
       s.SHOWDATE, s.SHOWTIME, tk.TICKETNUMBER, tk.TICKETPRICE, b.PAYMENTSTATUS
FROM TICKET tk
INNER JOIN BOOKING b ON tk.BOOKINGID = b.BOOKINGID
INNER JOIN CUSTOMER c ON b.CUSTOMERID = c.CUSTOMERID
INNER JOIN SHOWING s ON b.SHOWINGID = s.SHOWINGID
INNER JOIN MOVIE m ON s.MOVIEID = m.MOVIEID
INNER JOIN HALL h ON s.HALLID = h.HALLID
INNER JOIN THEATER t ON h.THEATERID = t.THEATERID
WHERE b.CUSTOMERID = :cid
  AND b.BOOKINGDATE >= ADD_MONTHS(SYSDATE, -6)
ORDER BY s.SHOWDATE DESC, s.SHOWTIME DESC;
```

### B. TheaterCityHall Movie (Filter by Theater and/or Hall)

```sql
SELECT t.THEATERNAME, t.CITY, h.HALLNUMBER, m.TITLE AS MOVIETITLE, m.GENRE, m.LANGUAGE,
       s.SHOWDATE, s.SHOWTIME, s.STATUS
FROM SHOWING s
INNER JOIN HALL h ON s.HALLID = h.HALLID
INNER JOIN THEATER t ON h.THEATERID = t.THEATERID
INNER JOIN MOVIE m ON s.MOVIEID = m.MOVIEID
WHERE (t.THEATERID = :tid OR :tid IS NULL)
  AND (h.HALLID = :hid OR :hid IS NULL)
ORDER BY t.THEATERNAME, h.HALLNUMBER, s.SHOWDATE, s.SHOWTIME;
```

### C. Movie Theater/Hall Occupancy – Top 3 (Paid Tickets Only)

```sql
SELECT * FROM (
  SELECT t.THEATERNAME, t.CITY, h.HALLNUMBER, h.CAPACITY,
         COUNT(tk.TICKETID) AS PAIDTICKETS,
         ROUND((COUNT(tk.TICKETID) * 100.0 / NULLIF(h.CAPACITY, 0)), 2) AS OCCUPANCY
  FROM SHOWING s
  INNER JOIN HALL h ON s.HALLID = h.HALLID
  INNER JOIN THEATER t ON h.THEATERID = t.THEATERID
  INNER JOIN BOOKING b ON s.SHOWINGID = b.SHOWINGID
  INNER JOIN TICKET tk ON b.BOOKINGID = tk.BOOKINGID
  WHERE s.MOVIEID = :mid
    AND UPPER(NVL(b.PAYMENTSTATUS,'')) = 'PAID'
  GROUP BY t.THEATERNAME, t.CITY, h.HALLNUMBER, h.CAPACITY
  ORDER BY OCCUPANCY DESC
) WHERE ROWNUM <= 3;
```

---

## 11. Validation Logic

- **Required fields**: `[Required]` on Username, FullName, Email, Title, etc.
- **Numeric**: `[Range(1, 500)]` for Duration, `[Range(1, 1000)]` for Capacity, `[Range(0, 10000)]` for prices.
- **Dates**: `[DataType(DataType.Date)]` for date fields.
- **Email**: `[EmailAddress]` on CustomerEmail.
- **Duplicate prevention**: Customer `UsernameExists()` in repository.
- **FK dropdowns**: SelectList/Select with “-- Select --” option and server-side checks.

---

## 12. Steps to Run in Visual Studio

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

## 13. Suggested Sample Data

Use `Scripts/SampleData.sql` (2 theaters, 2 customers, 2 movies, 3 halls, showings, bookings, tickets with `PAYMENTSTATUS = 'Paid'`). Add more rows for demo if needed.

---

## 14. Screenshot Checklist for Demo/Testing

- [ ] Home page with dashboard cards
- [ ] Customer CRUD: List, Create, Edit, Delete
- [ ] Hall CRUD with Theater dropdown
- [ ] Movie CRUD
- [ ] Showing CRUD with Hall and Movie dropdowns
- [ ] Ticket CRUD with Booking and Seat dropdowns
- [ ] User Ticket report: select user, see tickets last 6 months
- [ ] Theater/Hall Movies: filter by theater/hall
- [ ] Occupancy report: select movie, see top 3 theater/hall by occupancy
- [ ] Success/error messages after Create/Edit/Delete
- [ ] Validation: submit empty form, see error messages

---

## 15. Example Success and Failure Test Cases

| Scenario              | Expected result                          |
|-----------------------|------------------------------------------|
| Create customer       | Redirect to list, success message        |
| Edit movie            | Changes saved, success message           |
| Delete hall           | Record removed, success message          |
| Create customer with existing username | Validation error, no insert |
| Create ticket with invalid FK | Database/validation error, no insert |
| User Ticket: select user | Show tickets last 6 months           |
| Occupancy: select movie | Top 3 halls by paid-ticket occupancy  |
| Submit empty form     | Validation errors, no submit             |
