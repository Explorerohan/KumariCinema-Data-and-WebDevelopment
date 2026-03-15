using CinemaTicketing.Models;
using Oracle.ManagedDataAccess.Client;

namespace CinemaTicketing.Data;

/// <summary>
/// Data access for THEATER table - used for dropdowns
/// </summary>
public class TheaterRepository
{
    private readonly IConfiguration _config;

    public TheaterRepository(IConfiguration config)
    {
        _config = config;
    }

    public List<Theater> GetAll()
    {
        var list = new List<Theater>();
        using var conn = OracleHelper.CreateConnection(_config);
        using var cmd = new OracleCommand(
            "SELECT THEATERID, THEATERNAME, CITY, ADDRESS, CONTACTNUMBER, EMAIL FROM THEATER ORDER BY THEATERID", conn);
        using var rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            list.Add(new Theater
            {
                TheaterId = OracleHelper.GetDecimal(rdr, "THEATERID"),
                TheaterName = OracleHelper.GetString(rdr, "THEATERNAME") ?? "",
                City = OracleHelper.GetString(rdr, "CITY"),
                Address = OracleHelper.GetString(rdr, "ADDRESS"),
                ContactNumber = OracleHelper.GetString(rdr, "CONTACTNUMBER"),
                Email = OracleHelper.GetString(rdr, "EMAIL")
            });
        }
        return list;
    }
}
