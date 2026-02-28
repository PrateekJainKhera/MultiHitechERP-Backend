using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class VendorRepository : IVendorRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public VendorRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Vendor>> GetAllAsync()
        {
            const string query = @"
                SELECT Id, VendorCode, VendorName, VendorType, ContactPerson, Email, Phone,
                       Address, City, State, Country, PinCode, GSTNo, PANNo,
                       CreditDays, CreditLimit, PaymentTerms, IsActive,
                       CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                FROM Masters_Vendors
                ORDER BY VendorName";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var vendors = new List<Vendor>();
            while (await reader.ReadAsync())
                vendors.Add(MapToVendor(reader));
            return vendors;
        }

        public async Task<IEnumerable<Vendor>> GetActiveAsync()
        {
            const string query = @"
                SELECT Id, VendorCode, VendorName, VendorType, ContactPerson, Email, Phone,
                       Address, City, State, Country, PinCode, GSTNo, PANNo,
                       CreditDays, CreditLimit, PaymentTerms, IsActive,
                       CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                FROM Masters_Vendors
                WHERE IsActive = 1
                ORDER BY VendorName";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var vendors = new List<Vendor>();
            while (await reader.ReadAsync())
                vendors.Add(MapToVendor(reader));
            return vendors;
        }

        public async Task<Vendor?> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT Id, VendorCode, VendorName, VendorType, ContactPerson, Email, Phone,
                       Address, City, State, Country, PinCode, GSTNo, PANNo,
                       CreditDays, CreditLimit, PaymentTerms, IsActive,
                       CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                FROM Masters_Vendors WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapToVendor(reader) : null;
        }

        public async Task<Vendor?> GetByVendorCodeAsync(string vendorCode)
        {
            const string query = @"
                SELECT Id, VendorCode, VendorName, VendorType, ContactPerson, Email, Phone,
                       Address, City, State, Country, PinCode, GSTNo, PANNo,
                       CreditDays, CreditLimit, PaymentTerms, IsActive,
                       CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                FROM Masters_Vendors WHERE VendorCode = @VendorCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@VendorCode", vendorCode);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapToVendor(reader) : null;
        }

        public async Task<IEnumerable<Vendor>> SearchByNameAsync(string searchTerm)
        {
            const string query = @"
                SELECT Id, VendorCode, VendorName, VendorType, ContactPerson, Email, Phone,
                       Address, City, State, Country, PinCode, GSTNo, PANNo,
                       CreditDays, CreditLimit, PaymentTerms, IsActive,
                       CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                FROM Masters_Vendors
                WHERE VendorName LIKE @SearchTerm OR VendorCode LIKE @SearchTerm
                ORDER BY VendorName";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var vendors = new List<Vendor>();
            while (await reader.ReadAsync())
                vendors.Add(MapToVendor(reader));
            return vendors;
        }

        public async Task<int> InsertAsync(Vendor vendor)
        {
            const string query = @"
                INSERT INTO Masters_Vendors
                    (VendorCode, VendorName, VendorType, ContactPerson, Email, Phone,
                     Address, City, State, Country, PinCode, GSTNo, PANNo,
                     CreditDays, CreditLimit, PaymentTerms, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
                VALUES
                    (@VendorCode, @VendorName, @VendorType, @ContactPerson, @Email, @Phone,
                     @Address, @City, @State, @Country, @PinCode, @GSTNo, @PANNo,
                     @CreditDays, @CreditLimit, @PaymentTerms, @IsActive, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy);
                SELECT SCOPE_IDENTITY();";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            AddVendorParameters(command, vendor);
            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateAsync(Vendor vendor)
        {
            const string query = @"
                UPDATE Masters_Vendors SET
                    VendorName = @VendorName, VendorType = @VendorType,
                    ContactPerson = @ContactPerson, Email = @Email, Phone = @Phone,
                    Address = @Address, City = @City, State = @State, Country = @Country,
                    PinCode = @PinCode, GSTNo = @GSTNo, PANNo = @PANNo,
                    CreditDays = @CreditDays, CreditLimit = @CreditLimit,
                    PaymentTerms = @PaymentTerms, IsActive = @IsActive,
                    UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", vendor.Id);
            AddVendorParameters(command, vendor);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM Masters_Vendors WHERE Id = @Id";
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<int> GetNextSequenceNumberAsync()
        {
            const string query = "SELECT COUNT(1) FROM Masters_Vendors";
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            var count = (int)(await command.ExecuteScalarAsync() ?? 0);
            return count + 1;
        }

        private static void AddVendorParameters(SqlCommand command, Vendor vendor)
        {
            command.Parameters.AddWithValue("@VendorCode", vendor.VendorCode);
            command.Parameters.AddWithValue("@VendorName", vendor.VendorName);
            command.Parameters.AddWithValue("@VendorType", vendor.VendorType);
            command.Parameters.AddWithValue("@ContactPerson", (object?)vendor.ContactPerson ?? DBNull.Value);
            command.Parameters.AddWithValue("@Email", (object?)vendor.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@Phone", (object?)vendor.Phone ?? DBNull.Value);
            command.Parameters.AddWithValue("@Address", (object?)vendor.Address ?? DBNull.Value);
            command.Parameters.AddWithValue("@City", (object?)vendor.City ?? DBNull.Value);
            command.Parameters.AddWithValue("@State", (object?)vendor.State ?? DBNull.Value);
            command.Parameters.AddWithValue("@Country", vendor.Country);
            command.Parameters.AddWithValue("@PinCode", (object?)vendor.PinCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@GSTNo", (object?)vendor.GSTNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@PANNo", (object?)vendor.PANNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreditDays", (object?)vendor.CreditDays ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreditLimit", (object?)vendor.CreditLimit ?? DBNull.Value);
            command.Parameters.AddWithValue("@PaymentTerms", vendor.PaymentTerms);
            command.Parameters.AddWithValue("@IsActive", vendor.IsActive);
            command.Parameters.AddWithValue("@CreatedAt", vendor.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)vendor.CreatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", vendor.UpdatedAt);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)vendor.UpdatedBy ?? DBNull.Value);
        }

        private static Vendor MapToVendor(SqlDataReader reader)
        {
            return new Vendor
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                VendorCode = reader.GetString(reader.GetOrdinal("VendorCode")),
                VendorName = reader.GetString(reader.GetOrdinal("VendorName")),
                VendorType = reader.GetString(reader.GetOrdinal("VendorType")),
                ContactPerson = reader.IsDBNull(reader.GetOrdinal("ContactPerson")) ? null : reader.GetString(reader.GetOrdinal("ContactPerson")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone")),
                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString(reader.GetOrdinal("Address")),
                City = reader.IsDBNull(reader.GetOrdinal("City")) ? null : reader.GetString(reader.GetOrdinal("City")),
                State = reader.IsDBNull(reader.GetOrdinal("State")) ? null : reader.GetString(reader.GetOrdinal("State")),
                Country = reader.GetString(reader.GetOrdinal("Country")),
                PinCode = reader.IsDBNull(reader.GetOrdinal("PinCode")) ? null : reader.GetString(reader.GetOrdinal("PinCode")),
                GSTNo = reader.IsDBNull(reader.GetOrdinal("GSTNo")) ? null : reader.GetString(reader.GetOrdinal("GSTNo")),
                PANNo = reader.IsDBNull(reader.GetOrdinal("PANNo")) ? null : reader.GetString(reader.GetOrdinal("PANNo")),
                CreditDays = reader.IsDBNull(reader.GetOrdinal("CreditDays")) ? null : reader.GetInt32(reader.GetOrdinal("CreditDays")),
                CreditLimit = reader.IsDBNull(reader.GetOrdinal("CreditLimit")) ? null : reader.GetDecimal(reader.GetOrdinal("CreditLimit")),
                PaymentTerms = reader.GetString(reader.GetOrdinal("PaymentTerms")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy")),
            };
        }
    }
}
