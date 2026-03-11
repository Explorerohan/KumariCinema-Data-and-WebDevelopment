using CinemaTicketing.Models.ViewModels;
using Oracle.ManagedDataAccess.Client;

namespace CinemaTicketing.Data;

/// <summary>
/// Data access for complex report pages - parameterized queries
/// </summary>
public class ReportRepository
{
    private readonly IConfiguration _config;

    public ReportRepository(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// User Ticket page: tickets bought by user in last 6 months
    /// </summary>
    public List<UserTicketViewModel> GetUserTickets(decimal customerId)
    {
        var list = new List<UserTicketViewModel>();
        using var conn = OracleHelper.CreateConnection(_config);
        var sql = @"
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
            ORDER BY s.SHOWDATE DESC, s.SHOWTIME DESC";
        using var cmd = new OracleCommand(sql, conn);
        cmd.Parameters.Add(":cid", OracleDbType.Decimal, customerId, System.Data.ParameterDirection.Input);
        using var rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            list.Add(new UserTicketViewModel
            {
                CustomerId = customerId,
                UserName = OracleHelper.GetString(rdr, "USERNAME"),
                MovieTitle = OracleHelper.GetString(rdr, "MOVIETITLE"),
                TheaterName = OracleHelper.GetString(rdr, "THEATERNAME"),
                HallNumber = OracleHelper.GetString(rdr, "HALLNUMBER"),
                ShowDate = OracleHelper.GetDateTime(rdr, "SHOWDATE"),
                ShowTime = OracleHelper.GetTimeSpan(rdr, "SHOWTIME"),
                TicketNumber = OracleHelper.GetString(rdr, "TICKETNUMBER"),
                TicketPrice = OracleHelper.GetDecimal(rdr, "TICKETPRICE"),
                PaymentStatus = OracleHelper.GetString(rdr, "PAYMENTSTATUS")
            });
        }
        return list;
    }

    /// <summary>
    /// TheaterCityHall Movie page: movies and showtimes for theater and/or hall
    /// </summary>
    public List<TheaterMovieViewModel> GetTheaterMovies(decimal? theaterId, decimal? hallId)
    {
        var list = new List<TheaterMovieViewModel>();
        using var conn = OracleHelper.CreateConnection(_config);
        var sql = @"
            SELECT t.THEATERNAME, t.CITY, h.HALLNUMBER, m.TITLE AS MOVIETITLE, m.GENRE, m.LANGUAGE,
                   s.SHOWDATE, s.SHOWTIME, s.STATUS
            FROM SHOWING s
            INNER JOIN HALL h ON s.HALLID = h.HALLID
            INNER JOIN THEATER t ON h.THEATERID = t.THEATERID
            INNER JOIN MOVIE m ON s.MOVIEID = m.MOVIEID
            WHERE 1=1";
        var pars = new List<OracleParameter>();
        if (theaterId.HasValue)
        {
            sql += " AND t.THEATERID = :tid";
            pars.Add(new OracleParameter(":tid", theaterId.Value));
        }
        if (hallId.HasValue)
        {
            sql += " AND h.HALLID = :hid";
            pars.Add(new OracleParameter(":hid", hallId.Value));
        }
        sql += " ORDER BY t.THEATERNAME, h.HALLNUMBER, s.SHOWDATE, s.SHOWTIME";
        using var cmd = new OracleCommand(sql, conn);
        foreach (var p in pars) cmd.Parameters.Add(p);
        using var rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            list.Add(new TheaterMovieViewModel
            {
                TheaterName = OracleHelper.GetString(rdr, "THEATERNAME"),
                City = OracleHelper.GetString(rdr, "CITY"),
                HallNumber = OracleHelper.GetString(rdr, "HALLNUMBER"),
                MovieTitle = OracleHelper.GetString(rdr, "MOVIETITLE"),
                Genre = OracleHelper.GetString(rdr, "GENRE"),
                Language = OracleHelper.GetString(rdr, "LANGUAGE"),
                ShowDate = OracleHelper.GetDateTime(rdr, "SHOWDATE"),
                ShowTime = OracleHelper.GetTimeSpan(rdr, "SHOWTIME"),
                Status = OracleHelper.GetString(rdr, "STATUS")
            });
        }
        return list;
    }

    /// <summary>
    /// MovieTheatherCityHallOccupancy: top 3 theater/hall by occupancy (only paid tickets)
    /// Occupancy = (paid tickets / hall capacity) * 100
    /// </summary>
    public List<OccupancyViewModel> GetTopOccupancy(decimal movieId)
    {
        var list = new List<OccupancyViewModel>();
        using var conn = OracleHelper.CreateConnection(_config);
        var sql = @"
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
            ) WHERE ROWNUM <= 3";
        using var cmd = new OracleCommand(sql, conn);
        cmd.Parameters.Add(":mid", OracleDbType.Decimal, movieId, System.Data.ParameterDirection.Input);
        using var rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            var cap = OracleHelper.GetInt(rdr, "CAPACITY");
            var paid = OracleHelper.GetInt(rdr, "PAIDTICKETS");
            var occ = OracleHelper.GetDecimal(rdr, "OCCUPANCY");
            list.Add(new OccupancyViewModel
            {
                TheaterName = OracleHelper.GetString(rdr, "THEATERNAME"),
                City = OracleHelper.GetString(rdr, "CITY"),
                HallNumber = OracleHelper.GetString(rdr, "HALLNUMBER"),
                Capacity = cap,
                PaidTicketsCount = paid,
                OccupancyPercentage = occ
            });
        }
        return list;
    }
}
