using CinemaTicketing.Models;
using Oracle.ManagedDataAccess.Client;

namespace CinemaTicketing.Data;

/// <summary>
/// Data access for HALL - TheaterCityHall via THEATER + HALL join
/// </summary>
public class HallRepository
{
    private readonly IConfiguration _config;

    public HallRepository(IConfiguration config)
    {
        _config = config;
    }

    public List<Hall> GetAll()
    {
        var list = new List<Hall>();
        using var conn = OracleHelper.CreateConnection(_config);
        var sql = @"SELECT h.HALLID, h.THEATERID, h.HALLNUMBER, h.CAPACITY, h.HALLTYPE, t.THEATERNAME, t.CITY
            FROM HALL h INNER JOIN THEATER t ON h.THEATERID = t.THEATERID ORDER BY t.THEATERNAME, h.HALLNUMBER";
        using var cmd = new OracleCommand(sql, conn);
        using var rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            list.Add(new Hall
            {
                HallId = OracleHelper.GetDecimal(rdr, "HALLID"),
                TheaterId = OracleHelper.GetDecimal(rdr, "THEATERID"),
                HallNumber = OracleHelper.GetString(rdr, "HALLNUMBER") ?? "",
                Capacity = OracleHelper.GetInt(rdr, "CAPACITY"),
                HallType = OracleHelper.GetString(rdr, "HALLTYPE"),
                TheaterName = OracleHelper.GetString(rdr, "THEATERNAME"),
                City = OracleHelper.GetString(rdr, "CITY")
            });
        }
        return list;
    }

    public Hall? GetById(decimal id)
    {
        using var conn = OracleHelper.CreateConnection(_config);
        var sql = @"SELECT h.HALLID, h.THEATERID, h.HALLNUMBER, h.CAPACITY, h.HALLTYPE, t.THEATERNAME, t.CITY
            FROM HALL h INNER JOIN THEATER t ON h.THEATERID = t.THEATERID WHERE h.HALLID = :id";
        using var cmd = new OracleCommand(sql, conn);
        cmd.Parameters.Add(":id", OracleDbType.Decimal, id, System.Data.ParameterDirection.Input);
        using var rdr = cmd.ExecuteReader();
        if (!rdr.Read()) return null;
        return new Hall
        {
            HallId = OracleHelper.GetDecimal(rdr, "HALLID"),
            TheaterId = OracleHelper.GetDecimal(rdr, "THEATERID"),
            HallNumber = OracleHelper.GetString(rdr, "HALLNUMBER") ?? "",
            Capacity = OracleHelper.GetInt(rdr, "CAPACITY"),
            HallType = OracleHelper.GetString(rdr, "HALLTYPE"),
            TheaterName = OracleHelper.GetString(rdr, "THEATERNAME"),
            City = OracleHelper.GetString(rdr, "CITY")
        };
    }

    public int Insert(Hall h)
    {
        var sql = "INSERT INTO HALL (THEATERID, HALLNUMBER, CAPACITY, HALLTYPE) VALUES (:t, :hn, :cap, :ht)";
        return OracleHelper.ExecuteNonQuery(sql, _config,
            new OracleParameter(":t", h.TheaterId),
            new OracleParameter(":hn", h.HallNumber),
            new OracleParameter(":cap", h.Capacity),
            new OracleParameter(":ht", (object?)h.HallType ?? DBNull.Value));
    }

    public int Update(Hall h)
    {
        var sql = "UPDATE HALL SET THEATERID=:t, HALLNUMBER=:hn, CAPACITY=:cap, HALLTYPE=:ht WHERE HALLID=:id";
        return OracleHelper.ExecuteNonQuery(sql, _config,
            new OracleParameter(":t", h.TheaterId),
            new OracleParameter(":hn", h.HallNumber),
            new OracleParameter(":cap", h.Capacity),
            new OracleParameter(":ht", (object?)h.HallType ?? DBNull.Value),
            new OracleParameter(":id", h.HallId));
    }

    public int Delete(decimal id)
    {
        return OracleHelper.ExecuteNonQuery("DELETE FROM HALL WHERE HALLID = :id", _config,
            new OracleParameter(":id", id));
    }
}
