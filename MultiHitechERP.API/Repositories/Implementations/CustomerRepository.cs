using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    /// <summary>
    /// ADO.NET implementation of Customer repository
    /// </summary>
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CustomerRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Customer?> GetByIdAsync(Guid id)
        {
            const string query = "SELECT * FROM Masters_Customers WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapToCustomer(reader);
            }

            return null;
        }

        public async Task<Customer?> GetByCustomerCodeAsync(string customerCode)
        {
            const string query = "SELECT * FROM Masters_Customers WHERE CustomerCode = @CustomerCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@CustomerCode", customerCode);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapToCustomer(reader);
            }

            return null;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            const string query = "SELECT * FROM Masters_Customers ORDER BY CustomerName";

            var customers = new List<Customer>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                customers.Add(MapToCustomer(reader));
            }

            return customers;
        }

        public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
        {
            const string query = "SELECT * FROM Masters_Customers WHERE IsActive = 1 ORDER BY CustomerName";

            var customers = new List<Customer>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                customers.Add(MapToCustomer(reader));
            }

            return customers;
        }

        public async Task<Guid> InsertAsync(Customer customer)
        {
            const string query = @"
                INSERT INTO Masters_Customers (
                    Id, CustomerCode, CustomerName, CustomerType,
                    ContactPerson, Email, Phone,
                    Address, City, State, Country, PinCode,
                    GSTNo, PANNo,
                    CreditDays, CreditLimit, PaymentTerms,
                    IsActive, CreatedAt, CreatedBy
                ) VALUES (
                    @Id, @CustomerCode, @CustomerName, @CustomerType,
                    @ContactPerson, @Email, @Phone,
                    @Address, @City, @State, @Country, @PinCode,
                    @GSTNo, @PANNo,
                    @CreditDays, @CreditLimit, @PaymentTerms,
                    @IsActive, @CreatedAt, @CreatedBy
                )";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            customer.Id = Guid.NewGuid();
            customer.CreatedAt = DateTime.UtcNow;
            customer.IsActive = true;

            AddCustomerParameters(command, customer);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return customer.Id;
        }

        public async Task<bool> UpdateAsync(Customer customer)
        {
            const string query = @"
                UPDATE Masters_Customers SET
                    CustomerName = @CustomerName,
                    CustomerType = @CustomerType,
                    ContactPerson = @ContactPerson,
                    Email = @Email,
                    Phone = @Phone,
                    Address = @Address,
                    City = @City,
                    State = @State,
                    Country = @Country,
                    PinCode = @PinCode,
                    GSTNo = @GSTNo,
                    PANNo = @PANNo,
                    CreditDays = @CreditDays,
                    CreditLimit = @CreditLimit,
                    PaymentTerms = @PaymentTerms,
                    IsActive = @IsActive,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            customer.UpdatedAt = DateTime.UtcNow;

            AddCustomerParameters(command, customer);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            const string query = "DELETE FROM Masters_Customers WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> ActivateAsync(Guid id)
        {
            const string query = @"
                UPDATE Masters_Customers
                SET IsActive = 1, UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> DeactivateAsync(Guid id)
        {
            const string query = @"
                UPDATE Masters_Customers
                SET IsActive = 0, UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Customer>> SearchByNameAsync(string name)
        {
            const string query = @"
                SELECT * FROM Masters_Customers
                WHERE CustomerName LIKE @Name
                ORDER BY CustomerName";

            var customers = new List<Customer>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Name", $"%{name}%");

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                customers.Add(MapToCustomer(reader));
            }

            return customers;
        }

        public async Task<IEnumerable<Customer>> GetByCustomerTypeAsync(string customerType)
        {
            const string query = @"
                SELECT * FROM Masters_Customers
                WHERE CustomerType = @CustomerType
                ORDER BY CustomerName";

            var customers = new List<Customer>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@CustomerType", customerType);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                customers.Add(MapToCustomer(reader));
            }

            return customers;
        }

        public async Task<IEnumerable<Customer>> GetByCityAsync(string city)
        {
            const string query = @"
                SELECT * FROM Masters_Customers
                WHERE City = @City
                ORDER BY CustomerName";

            var customers = new List<Customer>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@City", city);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                customers.Add(MapToCustomer(reader));
            }

            return customers;
        }

        public async Task<IEnumerable<Customer>> GetByStateAsync(string state)
        {
            const string query = @"
                SELECT * FROM Masters_Customers
                WHERE State = @State
                ORDER BY CustomerName";

            var customers = new List<Customer>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@State", state);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                customers.Add(MapToCustomer(reader));
            }

            return customers;
        }

        public async Task<bool> ExistsAsync(string customerCode)
        {
            const string query = "SELECT COUNT(1) FROM Masters_Customers WHERE CustomerCode = @CustomerCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@CustomerCode", customerCode);

            await connection.OpenAsync();
            var count = (int)await command.ExecuteScalarAsync();

            return count > 0;
        }

        // Helper Methods

        private static Customer MapToCustomer(SqlDataReader reader)
        {
            return new Customer
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                CustomerCode = reader.GetString(reader.GetOrdinal("CustomerCode")),
                CustomerName = reader.GetString(reader.GetOrdinal("CustomerName")),

                CustomerType = reader.GetString(reader.GetOrdinal("CustomerType")),

                ContactPerson = reader.IsDBNull(reader.GetOrdinal("ContactPerson"))
                    ? null : reader.GetString(reader.GetOrdinal("ContactPerson")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email"))
                    ? null : reader.GetString(reader.GetOrdinal("Email")),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone"))
                    ? null : reader.GetString(reader.GetOrdinal("Phone")),

                Address = reader.IsDBNull(reader.GetOrdinal("Address"))
                    ? null : reader.GetString(reader.GetOrdinal("Address")),
                City = reader.IsDBNull(reader.GetOrdinal("City"))
                    ? null : reader.GetString(reader.GetOrdinal("City")),
                State = reader.IsDBNull(reader.GetOrdinal("State"))
                    ? null : reader.GetString(reader.GetOrdinal("State")),
                Country = reader.IsDBNull(reader.GetOrdinal("Country"))
                    ? null : reader.GetString(reader.GetOrdinal("Country")),
                PinCode = reader.IsDBNull(reader.GetOrdinal("PinCode"))
                    ? null : reader.GetString(reader.GetOrdinal("PinCode")),

                GSTNo = reader.IsDBNull(reader.GetOrdinal("GSTNo"))
                    ? null : reader.GetString(reader.GetOrdinal("GSTNo")),
                PANNo = reader.IsDBNull(reader.GetOrdinal("PANNo"))
                    ? null : reader.GetString(reader.GetOrdinal("PANNo")),

                CreditDays = reader.GetInt32(reader.GetOrdinal("CreditDays")),
                CreditLimit = reader.GetDecimal(reader.GetOrdinal("CreditLimit")),
                PaymentTerms = reader.IsDBNull(reader.GetOrdinal("PaymentTerms"))
                    ? null : reader.GetString(reader.GetOrdinal("PaymentTerms")),

                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),

                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy"))
                    ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy"))
                    ? null : reader.GetString(reader.GetOrdinal("UpdatedBy"))
            };
        }

        private static void AddCustomerParameters(SqlCommand command, Customer customer)
        {
            command.Parameters.AddWithValue("@Id", customer.Id);
            command.Parameters.AddWithValue("@CustomerCode", customer.CustomerCode);
            command.Parameters.AddWithValue("@CustomerName", customer.CustomerName);
            command.Parameters.AddWithValue("@CustomerType", customer.CustomerType);

            command.Parameters.AddWithValue("@ContactPerson", (object?)customer.ContactPerson ?? DBNull.Value);
            command.Parameters.AddWithValue("@Email", (object?)customer.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@Phone", (object?)customer.Phone ?? DBNull.Value);

            command.Parameters.AddWithValue("@Address", (object?)customer.Address ?? DBNull.Value);
            command.Parameters.AddWithValue("@City", (object?)customer.City ?? DBNull.Value);
            command.Parameters.AddWithValue("@State", (object?)customer.State ?? DBNull.Value);
            command.Parameters.AddWithValue("@Country", (object?)customer.Country ?? DBNull.Value);
            command.Parameters.AddWithValue("@PinCode", (object?)customer.PinCode ?? DBNull.Value);

            command.Parameters.AddWithValue("@GSTNo", (object?)customer.GSTNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@PANNo", (object?)customer.PANNo ?? DBNull.Value);

            command.Parameters.AddWithValue("@CreditDays", customer.CreditDays);
            command.Parameters.AddWithValue("@CreditLimit", customer.CreditLimit);
            command.Parameters.AddWithValue("@PaymentTerms", (object?)customer.PaymentTerms ?? DBNull.Value);

            command.Parameters.AddWithValue("@IsActive", customer.IsActive);

            command.Parameters.AddWithValue("@CreatedAt", customer.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)customer.CreatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", (object?)customer.UpdatedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)customer.UpdatedBy ?? DBNull.Value);
        }
    }
}
