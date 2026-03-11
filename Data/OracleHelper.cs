using Oracle.ManagedDataAccess.Client;

namespace CinemaTicketing.Data;

/// <summary>
/// Reusable helper for Oracle database connection and query execution.
/// Uses parameterized queries to prevent SQL injection.
/// </summary>
public static class OracleHelper
{
    /// <summary>
    /// Gets connection string from configuration
    /// </summary>
    public static string GetConnectionString(IConfiguration configuration)
    {
        var connStr = configuration.GetConnectionString("OracleConnection");
        if (string.IsNullOrEmpty(connStr))
            throw new InvalidOperationException("Oracle connection string not found in appsettings.json");
        return connStr;
    }

    /// <summary>
    /// Creates and opens an Oracle connection
    /// </summary>
    public static OracleConnection CreateConnection(IConfiguration configuration)
    {
        var conn = new OracleConnection(GetConnectionString(configuration));
        conn.Open();
        return conn;
    }

    /// <summary>
    /// Executes a non-query (INSERT, UPDATE, DELETE) with parameters
    /// Returns number of rows affected
    /// </summary>
    public static int ExecuteNonQuery(string sql, IConfiguration config, params OracleParameter[] parameters)
    {
        using var conn = CreateConnection(config);
        using var cmd = new OracleCommand(sql, conn);
        if (parameters != null && parameters.Length > 0)
            cmd.Parameters.AddRange(parameters);
        return cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Executes a scalar query and returns single value
    /// </summary>
    public static object? ExecuteScalar(string sql, IConfiguration config, params OracleParameter[] parameters)
    {
        using var conn = CreateConnection(config);
        using var cmd = new OracleCommand(sql, conn);
        if (parameters != null && parameters.Length > 0)
            cmd.Parameters.AddRange(parameters);
        return cmd.ExecuteScalar();
    }

    /// <summary>
    /// Helper to safely get decimal from reader
    /// </summary>
    public static decimal GetDecimal(OracleDataReader reader, string columnName)
    {
        var idx = reader.GetOrdinal(columnName);
        return reader.IsDBNull(idx) ? 0 : reader.GetDecimal(idx);
    }

    /// <summary>
    /// Helper to safely get int from reader
    /// </summary>
    public static int GetInt(OracleDataReader reader, string columnName)
    {
        var idx = reader.GetOrdinal(columnName);
        return reader.IsDBNull(idx) ? 0 : reader.GetInt32(idx);
    }

    /// <summary>
    /// Helper to safely get string from reader
    /// </summary>
    public static string? GetString(OracleDataReader reader, string columnName)
    {
        var idx = reader.GetOrdinal(columnName);
        return reader.IsDBNull(idx) ? null : reader.GetString(idx);
    }

    /// <summary>
    /// Helper to safely get DateTime from reader
    /// </summary>
    public static DateTime? GetDateTime(OracleDataReader reader, string columnName)
    {
        var idx = reader.GetOrdinal(columnName);
        return reader.IsDBNull(idx) ? null : reader.GetDateTime(idx);
    }

    /// <summary>
    /// Helper to safely get TimeSpan from reader (Oracle INTERVAL)
    /// </summary>
    public static TimeSpan? GetTimeSpan(OracleDataReader reader, string columnName)
    {
        var idx = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(idx)) return null;
        var val = reader.GetValue(idx);
        if (val is TimeSpan ts) return ts;
        if (val is DateTime dt) return dt.TimeOfDay;
        return null;
    }
}
