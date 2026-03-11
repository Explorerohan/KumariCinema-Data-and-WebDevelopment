using Oracle.ManagedDataAccess.Client;

namespace CinemaTicketing.Data;

/// <summary>
/// Data access for BOOKING - used for dropdowns in Ticket
/// </summary>
public class BookingRepository
{
    private readonly IConfiguration _config;

    public BookingRepository(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Returns list of (BookingId, DisplayText) for dropdown
    /// </summary>
    public List<(decimal BookingId, string Display)> GetForDropdown()
    {
        var list = new List<(decimal, string)>();
        using var conn = OracleHelper.CreateConnection(_config);
        var sql = @"SELECT b.BOOKINGID, c.FULLNAME, b.BOOKINGDATE, m.TITLE
            FROM BOOKING b
            INNER JOIN CUSTOMER c ON b.CUSTOMERID = c.CUSTOMERID
            INNER JOIN SHOWING s ON b.SHOWINGID = s.SHOWINGID
            INNER JOIN MOVIE m ON s.MOVIEID = m.MOVIEID
            ORDER BY b.BOOKINGID DESC";
        using var cmd = new OracleCommand(sql, conn);
        using var rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            var id = OracleHelper.GetDecimal(rdr, "BOOKINGID");
            var name = OracleHelper.GetString(rdr, "FULLNAME") ?? "";
            var dt = OracleHelper.GetDateTime(rdr, "BOOKINGDATE");
            var title = OracleHelper.GetString(rdr, "TITLE") ?? "";
            var display = $"#{id} - {name} - {title} ({dt:yyyy-MM-dd})";
            list.Add((id, display));
        }
        return list;
    }
}
