using CinemaTicketing.Models;
using Oracle.ManagedDataAccess.Client;

namespace CinemaTicketing.Data;

/// <summary>
/// Data access for MOVIE table - CRUD operations
/// </summary>
public class MovieRepository
{
    private readonly IConfiguration _config;

    public MovieRepository(IConfiguration config)
    {
        _config = config;
    }

    public List<Movie> GetAll()
    {
        var list = new List<Movie>();
        using var conn = OracleHelper.CreateConnection(_config);
        using var cmd = new OracleCommand(
            "SELECT MOVIEID, TITLE, DURATION, LANGUAGE, GENRE, RELEASEDATE FROM MOVIE ORDER BY TITLE", conn);
        using var rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            list.Add(Map(rdr));
        }
        return list;
    }

    public Movie? GetById(decimal id)
    {
        using var conn = OracleHelper.CreateConnection(_config);
        using var cmd = new OracleCommand(
            "SELECT MOVIEID, TITLE, DURATION, LANGUAGE, GENRE, RELEASEDATE FROM MOVIE WHERE MOVIEID = :id", conn);
        cmd.Parameters.Add(":id", OracleDbType.Decimal, id, System.Data.ParameterDirection.Input);
        using var rdr = cmd.ExecuteReader();
        return rdr.Read() ? Map(rdr) : null;
    }

    public int Insert(Movie m)
    {
        var sql = "INSERT INTO MOVIE (MOVIEID, TITLE, DURATION, LANGUAGE, GENRE, RELEASEDATE) VALUES ((SELECT NVL(MAX(MOVIEID),0)+1 FROM MOVIE), :t, :d, :l, :g, :rd)";
        return OracleHelper.ExecuteNonQuery(sql, _config,
            new OracleParameter(":t", m.Title),
            new OracleParameter(":d", m.Duration),
            new OracleParameter(":l", (object?)m.Language ?? DBNull.Value),
            new OracleParameter(":g", (object?)m.Genre ?? DBNull.Value),
            new OracleParameter(":rd", (object?)m.ReleaseDate ?? DBNull.Value));
    }

    public int Update(Movie m)
    {
        var sql = "UPDATE MOVIE SET TITLE=:t, DURATION=:d, LANGUAGE=:l, GENRE=:g, RELEASEDATE=:rd WHERE MOVIEID=:id";
        return OracleHelper.ExecuteNonQuery(sql, _config,
            new OracleParameter(":t", m.Title),
            new OracleParameter(":d", m.Duration),
            new OracleParameter(":l", (object?)m.Language ?? DBNull.Value),
            new OracleParameter(":g", (object?)m.Genre ?? DBNull.Value),
            new OracleParameter(":rd", (object?)m.ReleaseDate ?? DBNull.Value),
            new OracleParameter(":id", m.MovieId));
    }

    public int Delete(decimal id)
    {
        return OracleHelper.ExecuteNonQuery("DELETE FROM MOVIE WHERE MOVIEID = :id", _config,
            new OracleParameter(":id", id));
    }

    private static Movie Map(OracleDataReader rdr)
    {
        return new Movie
        {
            MovieId = OracleHelper.GetDecimal(rdr, "MOVIEID"),
            Title = OracleHelper.GetString(rdr, "TITLE") ?? "",
            Duration = OracleHelper.GetInt(rdr, "DURATION"),
            Language = OracleHelper.GetString(rdr, "LANGUAGE"),
            Genre = OracleHelper.GetString(rdr, "GENRE"),
            ReleaseDate = OracleHelper.GetDateTime(rdr, "RELEASEDATE")
        };
    }
}
