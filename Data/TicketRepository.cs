using CinemaTicketing.Models;
using Oracle.ManagedDataAccess.Client;

namespace CinemaTicketing.Data;

/// <summary>
/// Data access for TICKET table - CRUD operations with joined display
/// </summary>
public class TicketRepository
{
    private readonly IConfiguration _config;

    public TicketRepository(IConfiguration config)
    {
        _config = config;
    }

    public List<Ticket> GetAll()
    {
        var list = new List<Ticket>();
        using var conn = OracleHelper.CreateConnection(_config);
        var sql = @"SELECT t.TICKETID, t.BOOKINGID, t.SEATID, t.TICKETNUMBER, t.TICKETPRICE, t.TICKETSTATUS, t.ISSUEDATE,
            m.TITLE AS MOVIETITLE, th.THEATERNAME, h.HALLNUMBER, s2.SHOWDATE, s2.SHOWTIME, b.PAYMENTSTATUS
            FROM TICKET t
            INNER JOIN BOOKING b ON t.BOOKINGID = b.BOOKINGID
            INNER JOIN SHOWING s2 ON b.SHOWINGID = s2.SHOWINGID
            INNER JOIN MOVIE m ON s2.MOVIEID = m.MOVIEID
            INNER JOIN HALL h ON s2.HALLID = h.HALLID
            INNER JOIN THEATER th ON h.THEATERID = th.THEATERID
            INNER JOIN SEAT se ON t.SEATID = se.SEATID
            ORDER BY t.TICKETID DESC";
        using var cmd = new OracleCommand(sql, conn);
        using var rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            list.Add(Map(rdr));
        }
        return list;
    }

    public Ticket? GetById(decimal id)
    {
        using var conn = OracleHelper.CreateConnection(_config);
        var sql = @"SELECT t.TICKETID, t.BOOKINGID, t.SEATID, t.TICKETNUMBER, t.TICKETPRICE, t.TICKETSTATUS, t.ISSUEDATE,
            m.TITLE AS MOVIETITLE, th.THEATERNAME, h.HALLNUMBER, s2.SHOWDATE, s2.SHOWTIME, b.PAYMENTSTATUS
            FROM TICKET t
            INNER JOIN BOOKING b ON t.BOOKINGID = b.BOOKINGID
            INNER JOIN SHOWING s2 ON b.SHOWINGID = s2.SHOWINGID
            INNER JOIN MOVIE m ON s2.MOVIEID = m.MOVIEID
            INNER JOIN HALL h ON s2.HALLID = h.HALLID
            INNER JOIN THEATER th ON h.THEATERID = th.THEATERID
            INNER JOIN SEAT se ON t.SEATID = se.SEATID
            WHERE t.TICKETID = :id";
        using var cmd = new OracleCommand(sql, conn);
        cmd.Parameters.Add(":id", OracleDbType.Decimal, id, System.Data.ParameterDirection.Input);
        using var rdr = cmd.ExecuteReader();
        return rdr.Read() ? Map(rdr) : null;
    }

    public int Insert(Ticket t)
    {
        var sql = "INSERT INTO TICKET (BOOKINGID, SEATID, TICKETNUMBER, TICKETPRICE, TICKETSTATUS, ISSUEDATE) VALUES (:b, :s, :tn, :tp, :ts, :idate)";
        return OracleHelper.ExecuteNonQuery(sql, _config,
            new OracleParameter(":b", t.BookingId),
            new OracleParameter(":s", t.SeatId),
            new OracleParameter(":tn", t.TicketNumber),
            new OracleParameter(":tp", t.TicketPrice),
            new OracleParameter(":ts", (object?)t.TicketStatus ?? DBNull.Value),
            new OracleParameter(":idate", (object?)t.IssueDate ?? DBNull.Value));
    }

    public int Update(Ticket t)
    {
        var sql = "UPDATE TICKET SET BOOKINGID=:b, SEATID=:s, TICKETNUMBER=:tn, TICKETPRICE=:tp, TICKETSTATUS=:ts, ISSUEDATE=:idate WHERE TICKETID=:id";
        return OracleHelper.ExecuteNonQuery(sql, _config,
            new OracleParameter(":b", t.BookingId),
            new OracleParameter(":s", t.SeatId),
            new OracleParameter(":tn", t.TicketNumber),
            new OracleParameter(":tp", t.TicketPrice),
            new OracleParameter(":ts", (object?)t.TicketStatus ?? DBNull.Value),
            new OracleParameter(":idate", (object?)t.IssueDate ?? DBNull.Value),
            new OracleParameter(":id", t.TicketId));
    }

    public int Delete(decimal id)
    {
        return OracleHelper.ExecuteNonQuery("DELETE FROM TICKET WHERE TICKETID = :id", _config,
            new OracleParameter(":id", id));
    }

    private static Ticket Map(OracleDataReader rdr)
    {
        return new Ticket
        {
            TicketId = OracleHelper.GetDecimal(rdr, "TICKETID"),
            BookingId = OracleHelper.GetDecimal(rdr, "BOOKINGID"),
            SeatId = OracleHelper.GetDecimal(rdr, "SEATID"),
            TicketNumber = OracleHelper.GetString(rdr, "TICKETNUMBER") ?? "",
            TicketPrice = OracleHelper.GetDecimal(rdr, "TICKETPRICE"),
            TicketStatus = OracleHelper.GetString(rdr, "TICKETSTATUS"),
            IssueDate = OracleHelper.GetDateTime(rdr, "ISSUEDATE"),
            MovieTitle = OracleHelper.GetString(rdr, "MOVIETITLE"),
            TheaterName = OracleHelper.GetString(rdr, "THEATERNAME"),
            HallNumber = OracleHelper.GetString(rdr, "HALLNUMBER"),
            ShowDate = OracleHelper.GetDateTime(rdr, "SHOWDATE"),
            ShowTime = OracleHelper.GetString(rdr, "SHOWTIME"),
            PaymentStatus = OracleHelper.GetString(rdr, "PAYMENTSTATUS")
        };
    }
}
