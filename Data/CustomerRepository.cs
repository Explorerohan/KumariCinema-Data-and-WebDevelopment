using CinemaTicketing.Models;
using Oracle.ManagedDataAccess.Client;

namespace CinemaTicketing.Data;

/// <summary>
/// Data access for CUSTOMER table - CRUD operations
/// </summary>
public class CustomerRepository
{
    private readonly IConfiguration _config;

    public CustomerRepository(IConfiguration config)
    {
        _config = config;
    }

    public List<Customer> GetAll()
    {
        var list = new List<Customer>();
        using var conn = OracleHelper.CreateConnection(_config);
        using var cmd = new OracleCommand(
            "SELECT CUSTOMERID, USERNAME, FULLNAME, CUSTOMEREMAIL, PHONENUMBER, CUSTOMERADDRESS, CUSTOMERCITY, DATEOFBIRTH, REGISTRATIONDATE FROM CUSTOMER ORDER BY CUSTOMERID", conn);
        using var rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            list.Add(new Customer
            {
                CustomerId = OracleHelper.GetDecimal(rdr, "CUSTOMERID"),
                Username = OracleHelper.GetString(rdr, "USERNAME") ?? "",
                FullName = OracleHelper.GetString(rdr, "FULLNAME") ?? "",
                CustomerEmail = OracleHelper.GetString(rdr, "CUSTOMEREMAIL") ?? "",
                PhoneNumber = OracleHelper.GetString(rdr, "PHONENUMBER"),
                CustomerAddress = OracleHelper.GetString(rdr, "CUSTOMERADDRESS"),
                CustomerCity = OracleHelper.GetString(rdr, "CUSTOMERCITY"),
                DateOfBirth = OracleHelper.GetDateTime(rdr, "DATEOFBIRTH"),
                RegistrationDate = OracleHelper.GetDateTime(rdr, "REGISTRATIONDATE")
            });
        }
        return list;
    }

    public Customer? GetById(decimal id)
    {
        using var conn = OracleHelper.CreateConnection(_config);
        using var cmd = new OracleCommand(
            "SELECT CUSTOMERID, USERNAME, FULLNAME, CUSTOMEREMAIL, PHONENUMBER, CUSTOMERADDRESS, CUSTOMERCITY, DATEOFBIRTH, REGISTRATIONDATE FROM CUSTOMER WHERE CUSTOMERID = :id", conn);
        cmd.Parameters.Add(":id", OracleDbType.Decimal, id, System.Data.ParameterDirection.Input);
        using var rdr = cmd.ExecuteReader();
        if (!rdr.Read()) return null;
        return new Customer
        {
            CustomerId = OracleHelper.GetDecimal(rdr, "CUSTOMERID"),
            Username = OracleHelper.GetString(rdr, "USERNAME") ?? "",
            FullName = OracleHelper.GetString(rdr, "FULLNAME") ?? "",
            CustomerEmail = OracleHelper.GetString(rdr, "CUSTOMEREMAIL") ?? "",
            PhoneNumber = OracleHelper.GetString(rdr, "PHONENUMBER"),
            CustomerAddress = OracleHelper.GetString(rdr, "CUSTOMERADDRESS"),
            CustomerCity = OracleHelper.GetString(rdr, "CUSTOMERCITY"),
            DateOfBirth = OracleHelper.GetDateTime(rdr, "DATEOFBIRTH"),
            RegistrationDate = OracleHelper.GetDateTime(rdr, "REGISTRATIONDATE")
        };
    }

    public int Insert(Customer c)
    {
        var sql = @"INSERT INTO CUSTOMER (USERNAME, FULLNAME, CUSTOMEREMAIL, PHONENUMBER, CUSTOMERADDRESS, CUSTOMERCITY, DATEOFBIRTH, REGISTRATIONDATE)
            VALUES (:u, :f, :e, :p, :a, :city, :dob, :reg)";
        return OracleHelper.ExecuteNonQuery(sql, _config,
            new OracleParameter(":u", c.Username),
            new OracleParameter(":f", c.FullName),
            new OracleParameter(":e", c.CustomerEmail),
            new OracleParameter(":p", (object?)c.PhoneNumber ?? DBNull.Value),
            new OracleParameter(":a", (object?)c.CustomerAddress ?? DBNull.Value),
            new OracleParameter(":city", (object?)c.CustomerCity ?? DBNull.Value),
            new OracleParameter(":dob", (object?)c.DateOfBirth ?? DBNull.Value),
            new OracleParameter(":reg", (object?)c.RegistrationDate ?? DBNull.Value));
    }

    public int Update(Customer c)
    {
        var sql = @"UPDATE CUSTOMER SET USERNAME=:u, FULLNAME=:f, CUSTOMEREMAIL=:e, PHONENUMBER=:p, CUSTOMERADDRESS=:a, CUSTOMERCITY=:city, DATEOFBIRTH=:dob, REGISTRATIONDATE=:reg
            WHERE CUSTOMERID=:id";
        return OracleHelper.ExecuteNonQuery(sql, _config,
            new OracleParameter(":u", c.Username),
            new OracleParameter(":f", c.FullName),
            new OracleParameter(":e", c.CustomerEmail),
            new OracleParameter(":p", (object?)c.PhoneNumber ?? DBNull.Value),
            new OracleParameter(":a", (object?)c.CustomerAddress ?? DBNull.Value),
            new OracleParameter(":city", (object?)c.CustomerCity ?? DBNull.Value),
            new OracleParameter(":dob", (object?)c.DateOfBirth ?? DBNull.Value),
            new OracleParameter(":reg", (object?)c.RegistrationDate ?? DBNull.Value),
            new OracleParameter(":id", c.CustomerId));
    }

    public int Delete(decimal id)
    {
        return OracleHelper.ExecuteNonQuery("DELETE FROM CUSTOMER WHERE CUSTOMERID = :id", _config,
            new OracleParameter(":id", id));
    }

    /// <summary>
    /// Check if username already exists (for duplicate prevention)
    /// </summary>
    public bool UsernameExists(string username, decimal? excludeId = null)
    {
        object? count;
        if (excludeId.HasValue)
        {
            var sql = "SELECT COUNT(*) FROM CUSTOMER WHERE USERNAME = :u AND CUSTOMERID <> :id";
            count = OracleHelper.ExecuteScalar(sql, _config, new OracleParameter(":u", username), new OracleParameter(":id", excludeId.Value));
        }
        else
        {
            var sql = "SELECT COUNT(*) FROM CUSTOMER WHERE USERNAME = :u";
            count = OracleHelper.ExecuteScalar(sql, _config, new OracleParameter(":u", username));
        }
        return Convert.ToInt32(count ?? 0) > 0;
    }
}
