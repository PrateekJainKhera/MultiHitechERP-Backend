using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class ComponentRepository : IComponentRepository
    {
        private readonly string _connectionString;

        public ComponentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<IEnumerable<Component>> GetAllAsync()
        {
            var components = new List<Component>();
            const string query = "SELECT * FROM Masters_Components ORDER BY ComponentName";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                components.Add(MapToComponent(reader));
            }

            return components;
        }

        public async Task<Component?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM Masters_Components WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapToComponent(reader);
            }

            return null;
        }

        public async Task<Component?> GetByPartNumberAsync(string partNumber)
        {
            const string query = "SELECT * FROM Masters_Components WHERE PartNumber = @PartNumber";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PartNumber", partNumber);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapToComponent(reader);
            }

            return null;
        }

        public async Task<IEnumerable<Component>> GetByCategoryAsync(string category)
        {
            var components = new List<Component>();
            const string query = "SELECT * FROM Masters_Components WHERE Category = @Category ORDER BY ComponentName";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Category", category);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                components.Add(MapToComponent(reader));
            }

            return components;
        }

        public async Task<IEnumerable<Component>> SearchByNameAsync(string searchTerm)
        {
            var components = new List<Component>();
            const string query = @"
                SELECT * FROM Masters_Components
                WHERE ComponentName LIKE @SearchTerm
                   OR PartNumber LIKE @SearchTerm
                   OR Manufacturer LIKE @SearchTerm
                ORDER BY ComponentName";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                components.Add(MapToComponent(reader));
            }

            return components;
        }

        public async Task<int> CreateAsync(Component component)
        {
            const string query = @"
                INSERT INTO Masters_Components (
                    PartNumber, ComponentName, Category, Manufacturer, SupplierName,
                    Specifications, LeadTimeDays, Unit, Notes, IsActive, CreatedAt, CreatedBy, UpdatedAt
                ) VALUES (
                    @PartNumber, @ComponentName, @Category, @Manufacturer, @SupplierName,
                    @Specifications, @LeadTimeDays, @Unit, @Notes, @IsActive, @CreatedAt, @CreatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@PartNumber", component.PartNumber);
            command.Parameters.AddWithValue("@ComponentName", component.ComponentName);
            command.Parameters.AddWithValue("@Category", component.Category);
            command.Parameters.AddWithValue("@Manufacturer", (object?)component.Manufacturer ?? DBNull.Value);
            command.Parameters.AddWithValue("@SupplierName", (object?)component.SupplierName ?? DBNull.Value);
            command.Parameters.AddWithValue("@Specifications", (object?)component.Specifications ?? DBNull.Value);
            command.Parameters.AddWithValue("@LeadTimeDays", component.LeadTimeDays);
            command.Parameters.AddWithValue("@Unit", component.Unit);
            command.Parameters.AddWithValue("@Notes", (object?)component.Notes ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", component.IsActive);
            command.Parameters.AddWithValue("@CreatedAt", component.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)component.CreatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", component.UpdatedAt);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateAsync(int id, Component component)
        {
            const string query = @"
                UPDATE Masters_Components
                SET PartNumber = @PartNumber,
                    ComponentName = @ComponentName,
                    Category = @Category,
                    Manufacturer = @Manufacturer,
                    SupplierName = @SupplierName,
                    Specifications = @Specifications,
                    LeadTimeDays = @LeadTimeDays,
                    Unit = @Unit,
                    Notes = @Notes,
                    IsActive = @IsActive,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@PartNumber", component.PartNumber);
            command.Parameters.AddWithValue("@ComponentName", component.ComponentName);
            command.Parameters.AddWithValue("@Category", component.Category);
            command.Parameters.AddWithValue("@Manufacturer", (object?)component.Manufacturer ?? DBNull.Value);
            command.Parameters.AddWithValue("@SupplierName", (object?)component.SupplierName ?? DBNull.Value);
            command.Parameters.AddWithValue("@Specifications", (object?)component.Specifications ?? DBNull.Value);
            command.Parameters.AddWithValue("@LeadTimeDays", component.LeadTimeDays);
            command.Parameters.AddWithValue("@Unit", component.Unit);
            command.Parameters.AddWithValue("@Notes", (object?)component.Notes ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", component.IsActive);
            command.Parameters.AddWithValue("@UpdatedAt", component.UpdatedAt);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)component.UpdatedBy ?? DBNull.Value);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM Masters_Components WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> PartNumberExistsAsync(string partNumber, int? excludeId = null)
        {
            var query = "SELECT COUNT(1) FROM Masters_Components WHERE PartNumber = @PartNumber";
            if (excludeId.HasValue)
            {
                query += " AND Id != @ExcludeId";
            }

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PartNumber", partNumber);
            if (excludeId.HasValue)
            {
                command.Parameters.AddWithValue("@ExcludeId", excludeId.Value);
            }

            await connection.OpenAsync();
            var count = (int)(await command.ExecuteScalarAsync() ?? 0);
            return count > 0;
        }

        public async Task<int> GetNextSequenceNumberAsync(string category)
        {
            // PartNumber format: CATEGORY-SEQ
            // Example: BRG-0001, SFT-0002, etc.
            string prefix = category.Length >= 3 ? category.Substring(0, 3).ToUpper() : category.ToUpper();

            const string query = @"
                SELECT ISNULL(MAX(CAST(RIGHT(PartNumber, 4) AS INT)), 0) + 1
                FROM Masters_Components
                WHERE PartNumber LIKE @Prefix + '%'";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Prefix", prefix);

            await connection.OpenAsync();
            var nextSequence = (int)(await command.ExecuteScalarAsync() ?? 1);

            return nextSequence;
        }

        private Component MapToComponent(SqlDataReader reader)
        {
            return new Component
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                PartNumber = reader.GetString(reader.GetOrdinal("PartNumber")),
                ComponentName = reader.GetString(reader.GetOrdinal("ComponentName")),
                Category = reader.GetString(reader.GetOrdinal("Category")),
                Manufacturer = reader.IsDBNull(reader.GetOrdinal("Manufacturer")) ? null : reader.GetString(reader.GetOrdinal("Manufacturer")),
                SupplierName = reader.IsDBNull(reader.GetOrdinal("SupplierName")) ? null : reader.GetString(reader.GetOrdinal("SupplierName")),
                Specifications = reader.IsDBNull(reader.GetOrdinal("Specifications")) ? null : reader.GetString(reader.GetOrdinal("Specifications")),
                LeadTimeDays = reader.GetInt32(reader.GetOrdinal("LeadTimeDays")),
                Unit = reader.GetString(reader.GetOrdinal("Unit")),
                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy"))
            };
        }
    }
}
