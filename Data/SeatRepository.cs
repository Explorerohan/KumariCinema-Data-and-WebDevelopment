using CinemaTicketing.Models;
using Oracle.ManagedDataAccess.Client;

namespace CinemaTicketing.Data;

/// <summary>
/// Data access for SEAT table - used for dropdowns in Ticket
/// </summary>
public class SeatRepository
{
    private readonly IConfiguration _config;

    public SeatRepository(IConfiguration config)
    {
        _config = config;
    }

    public List<Seat> GetAll()
    {
        var list = new List<Seat>();
        using var conn = OracleHelper.CreateConnection(_config);
        using var cmd = new OracleCommand(
            "SELECT SEATID, HALLID, SEATNUMBER, ROWNUMBER, SEATTYPE FROM SEAT ORDER BY HALLID, ROWNUMBER, SEATNUMBER", conn);
        using var rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            list.Add(new Seat
            {
                SeatId = OracleHelper.GetDecimal(rdr, "SEATID"),
                HallId = OracleHelper.GetDecimal(rdr, "HALLID"),
                SeatNumber = OracleHelper.GetString(rdr, "SEATNUMBER") ?? "",
                RowNumber = OracleHelper.GetString(rdr, "ROWNUMBER"),
                SeatType = OracleHelper.GetString(rdr, "SEATTYPE")
            });
        }
        return list;
    }

    /// <summary>
    /// Returns seats with theater/hall display for dropdown
    /// </summary>
    public List<(decimal SeatId, string Display)> GetForDropdown()
    {
        var list = new List<(decimal, string)>();
        using var conn = OracleHelper.CreateConnection(_config);
        var sql = @"SELECT se.SEATID, se.SEATNUMBER, se.ROWNUMBER, h.HALLNUMBER, t.THEATERNAME
            FROM SEAT se INNER JOIN HALL h ON se.HALLID = h.HALLID INNER JOIN THEATER t ON h.THEATERID = t.THEATERID
            ORDER BY t.THEATERNAME, h.HALLNUMBER, se.ROWNUMBER, se.SEATNUMBER";
        using var cmd = new OracleCommand(sql, conn);
        using var rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            var id = OracleHelper.GetDecimal(rdr, "SEATID");
            var sn = OracleHelper.GetString(rdr, "SEATNUMBER") ?? "";
            var rn = OracleHelper.GetString(rdr, "ROWNUMBER") ?? "";
            var hn = OracleHelper.GetString(rdr, "HALLNUMBER") ?? "";
            var tn = OracleHelper.GetString(rdr, "THEATERNAME") ?? "";
            list.Add((id, $"{tn} - Hall {hn} - Row {rn} Seat {sn}"));
        }
        return list;
    }
}
