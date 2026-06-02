using System.Data;
using CustomerEnrollment.Api.Models;
using Microsoft.Data.SqlClient;

namespace CustomerEnrollment.Api.Data;

public interface ICustomerRepository
{
    Task<int> InsertAsync(CustomerDto customer, CancellationToken ct);
    Task<CustomerDto?> GetByIdAsync(int customerId, CancellationToken ct);
}

public class CustomerRepository : ICustomerRepository
{
    private readonly string _connectionString;
    private readonly string _passphrase;

    public CustomerRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("CustomerDb");
        _passphrase = config["Sql:Passphrase"];

        if (string.IsNullOrWhiteSpace(_connectionString))
            throw new InvalidOperationException("Connection string 'CustomerDb' is missing.");

        if (string.IsNullOrWhiteSpace(_passphrase))
            throw new InvalidOperationException("Sql:Passphrase is missing.");
    }

    public async Task<int> InsertAsync(CustomerDto customer, CancellationToken ct)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync(ct);

        using var tx = (SqlTransaction)await conn.BeginTransactionAsync(ct);

        try
        {
            using var cmd = new SqlCommand("dbo.usp_Customer_Insert", conn, tx);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Passphrase",  SqlDbType.NVarChar, 128).Value = _passphrase;
            cmd.Parameters.Add("@Name",        SqlDbType.NVarChar, 200).Value = customer.Name;
            cmd.Parameters.Add("@Mobile",      SqlDbType.NVarChar, 20).Value  = customer.Mobile;
            cmd.Parameters.Add("@Email",       SqlDbType.NVarChar, 200).Value = customer.Email;
            cmd.Parameters.Add("@ProofType",   SqlDbType.NVarChar, 50).Value  = customer.ProofType;
            cmd.Parameters.Add("@ProofRef",    SqlDbType.NVarChar, 100).Value = customer.ProofRef;
            cmd.Parameters.Add("@Address",     SqlDbType.NVarChar, 500).Value = customer.Address;

            var idParam = new SqlParameter("@NewCustomerId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(idParam);

            await cmd.ExecuteNonQueryAsync(ct);
            await tx.CommitAsync(ct);

            // SCOPE_IDENTITY returns decimal in some cases, force int
            return Convert.ToInt32(idParam.Value);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<CustomerDto?> GetByIdAsync(int customerId, CancellationToken ct)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync(ct);

        using var cmd = new SqlCommand("dbo.usp_Customer_GetById", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = customerId;
        cmd.Parameters.Add("@Passphrase", SqlDbType.NVarChar, 128).Value = _passphrase;

        using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            return null;

        return new CustomerDto
        {
            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
            Name = reader.GetString(reader.GetOrdinal("Name")),
            Mobile = reader.GetString(reader.GetOrdinal("Mobile")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            ProofType = reader.GetString(reader.GetOrdinal("ProofType")),
            ProofRef = reader.GetString(reader.GetOrdinal("ProofRef")),
            Address = reader.GetString(reader.GetOrdinal("Address")),
            CreatedOnUtc = reader.GetDateTime(reader.GetOrdinal("CreatedOnUtc"))
        };
    }
}
