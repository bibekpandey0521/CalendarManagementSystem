using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CalendarManagement.Api.Data;

public interface IDbConnectionFactory
{
    SqlConnection CreateConnection();
}

public sealed class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "The 'ConnectionStrings:DefaultConnection' connection string was not found.");
    }

    public SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
