using CinemaTicketing.Models;
using Oracle.ManagedDataAccess.Client;

namespace CinemaTicketing.Data;

/// <summary>
/// Data access for SHOWING table - CRUD operations
/// </summary>
public class ShowingRepository
{
    private readonly IConfiguration _config;

    public ShowingRepository(IConfiguration config)
    {
        _config = config;
    }

    public List<Showing> GetAll()
    {
        var list = new List<Showing>();
        using var conn = OracleHelper.CreateConnection(_config);
        var sql = @"SELECT s.SHOWINGID, s.HALLID, s.MOVIEID, s.SHOWDATE, s.SHOWTIME, s.STATUS,
            h.HALLNUMBER, m.TITLE AS MOVIETITLE, t.THEATERNAME
            FROM SHOWING s
            INNER JOIN HALL h ON s.HALLID = h.HALLID
            INNER JOIN THEATER t ON h.THEATERID = t.THEATERID
            INNER JOIN MOVIE m ON s.MOVIEID = m.MOVIEID
            ORDER BY s.SHOWDATE DESC, s.SHOWTIME";
        using var cmd = new OracleCommand(sql, conn);
        using var rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            list.Add(Map(rdr));
        }
        return list;
    }

    public Showing? GetById(decimal id)
    {
        using var conn = OracleHelper.CreateConnection(_config);
        var sql = @"SELECT s.SHOWINGID, s.HALLID, s.MOVIEID, s.SHOWDATE, s.SHOWTIME, s.STATUS,
            h.HALLNUMBER, m.TITLE AS MOVIETITLE, t.THEATERNAME
            FROM SHOWING s
            INNER JOIN HALL h ON s.HALLID = h.HALLID
            INNER JOIN THEATER t ON h.THEATERID = t.THEATERID
            INNER JOIN MOVIE m ON s.MOVIEID = m.MOVIEID
            WHERE s.SHOWINGID = :id";
        using var cmd = new OracleCommand(sql, conn);
        cmd.Parameters.Add(":id", OracleDbType.Decimal, id, System.Data.ParameterDirection.Input);
        using var rdr = cmd.ExecuteReader();
        return rdr.Read() ? Map(rdr) : null;
    }

    public int Insert(Showing s)
    {
        var nextId = GetNextShowingId();
        var sql = "INSERT INTO SHOWING (SHOWINGID, HALLID, MOVIEID, SHOWDATE, SHOWTIME, STATUS) VALUES (:id, :h, :m, :sd, :st, :status)";
        return OracleHelper.ExecuteNonQuery(sql, _config,
            new OracleParameter(":id", nextId),
            new OracleParameter(":h", s.HallId),
            new OracleParameter(":m", s.MovieId),
            new OracleParameter(":sd", s.ShowDate),
            new OracleParameter(":st", (object?)s.ShowTime ?? DBNull.Value),
            new OracleParameter(":status", (object?)s.Status ?? DBNull.Value));
    }

    private decimal GetNextShowingId()
    {
        using var conn = OracleHelper.CreateConnection(_config);
        using var cmd = new OracleCommand("SELECT NVL(MAX(SHOWINGID), 0) + 1 FROM SHOWING", conn);
        var result = cmd.ExecuteScalar();
        return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 1;
    }

    public int Update(Showing s)
    {
        var sql = "UPDATE SHOWING SET HALLID=:h, MOVIEID=:m, SHOWDATE=:sd, SHOWTIME=:st, STATUS=:status WHERE SHOWINGID=:id";
        return OracleHelper.ExecuteNonQuery(sql, _config,
            new OracleParameter(":h", s.HallId),
            new OracleParameter(":m", s.MovieId),
            new OracleParameter(":sd", s.ShowDate),
            new OracleParameter(":st", (object?)s.ShowTime ?? DBNull.Value),
            new OracleParameter(":status", (object?)s.Status ?? DBNull.Value),
            new OracleParameter(":id", s.ShowingId));
    }

    public int Delete(decimal id)
    {
        return OracleHelper.ExecuteNonQuery("DELETE FROM SHOWING WHERE SHOWINGID = :id", _config,
            new OracleParameter(":id", id));
    }

    private static Showing Map(OracleDataReader rdr)
    {
        return new Showing
        {
            ShowingId = OracleHelper.GetDecimal(rdr, "SHOWINGID"),
            HallId = OracleHelper.GetDecimal(rdr, "HALLID"),
            MovieId = OracleHelper.GetDecimal(rdr, "MOVIEID"),
            ShowDate = OracleHelper.GetDateTime(rdr, "SHOWDATE") ?? DateTime.Today,
            ShowTime = OracleHelper.GetString(rdr, "SHOWTIME") ?? "",
            Status = OracleHelper.GetString(rdr, "STATUS"),
            HallNumber = OracleHelper.GetString(rdr, "HALLNUMBER"),
            MovieTitle = OracleHelper.GetString(rdr, "MOVIETITLE"),
            TheaterName = OracleHelper.GetString(rdr, "THEATERNAME")
        };
    }
}
