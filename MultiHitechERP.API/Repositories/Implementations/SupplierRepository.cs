using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    /// <summary>
    /// Repository implementation for Supplier operations
    /// </summary>
    public class SupplierRepository : ISupplierRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public SupplierRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Supplier?> GetByIdAsync(Guid id)
        {
            const string query = @"
                SELECT * FROM Masters_Suppliers
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return MapToSupplier(reader);

            return null;
        }

        public async Task<Supplier?> GetByCodeAsync(string supplierCode)
        {
            const string query = @"
                SELECT * FROM Masters_Suppliers
                WHERE SupplierCode = @SupplierCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@SupplierCode", supplierCode);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return MapToSupplier(reader);

            return null;
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            const string query = @"
                SELECT * FROM Masters_Suppliers
                ORDER BY SupplierCode";

            var suppliers = new List<Supplier>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                suppliers.Add(MapToSupplier(reader));
            }

            return suppliers;
        }

        public async Task<Guid> InsertAsync(Supplier supplier)
        {
            const string query = @"
                INSERT INTO Masters_Suppliers (
                    Id, SupplierCode, SupplierName, SupplierType, Category,
                    ContactPerson, ContactNumber, Email, Website,
                    AddressLine1, AddressLine2, City, State, Country, PostalCode,
                    GSTNumber, PANNumber, TaxStatus,
                    PaymentTerms, CreditDays, CreditLimit,
                    BankName, BankAccountNumber, IFSCCode, BankBranch,
                    OnTimeDeliveryRate, QualityRating, TotalOrders, RejectedOrders,
                    ServicesOffered, ProcessCapabilities, MaterialsSupplied,
                    StandardLeadTimeDays, MinimumOrderQuantity,
                    IsActive, IsApproved, Status, ApprovalStatus,
                    Remarks, InternalNotes,
                    CreatedAt, CreatedBy
                )
                VALUES (
                    @Id, @SupplierCode, @SupplierName, @SupplierType, @Category,
                    @ContactPerson, @ContactNumber, @Email, @Website,
                    @AddressLine1, @AddressLine2, @City, @State, @Country, @PostalCode,
                    @GSTNumber, @PANNumber, @TaxStatus,
                    @PaymentTerms, @CreditDays, @CreditLimit,
                    @BankName, @BankAccountNumber, @IFSCCode, @BankBranch,
                    @OnTimeDeliveryRate, @QualityRating, @TotalOrders, @RejectedOrders,
                    @ServicesOffered, @ProcessCapabilities, @MaterialsSupplied,
                    @StandardLeadTimeDays, @MinimumOrderQuantity,
                    @IsActive, @IsApproved, @Status, @ApprovalStatus,
                    @Remarks, @InternalNotes,
                    @CreatedAt, @CreatedBy
                )";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            var id = Guid.NewGuid();
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@SupplierCode", supplier.SupplierCode);
            command.Parameters.AddWithValue("@SupplierName", supplier.SupplierName);
            command.Parameters.AddWithValue("@SupplierType", (object?)supplier.SupplierType ?? DBNull.Value);
            command.Parameters.AddWithValue("@Category", (object?)supplier.Category ?? DBNull.Value);
            command.Parameters.AddWithValue("@ContactPerson", (object?)supplier.ContactPerson ?? DBNull.Value);
            command.Parameters.AddWithValue("@ContactNumber", (object?)supplier.ContactNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@Email", (object?)supplier.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@Website", (object?)supplier.Website ?? DBNull.Value);
            command.Parameters.AddWithValue("@AddressLine1", (object?)supplier.AddressLine1 ?? DBNull.Value);
            command.Parameters.AddWithValue("@AddressLine2", (object?)supplier.AddressLine2 ?? DBNull.Value);
            command.Parameters.AddWithValue("@City", (object?)supplier.City ?? DBNull.Value);
            command.Parameters.AddWithValue("@State", (object?)supplier.State ?? DBNull.Value);
            command.Parameters.AddWithValue("@Country", (object?)supplier.Country ?? DBNull.Value);
            command.Parameters.AddWithValue("@PostalCode", (object?)supplier.PostalCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@GSTNumber", (object?)supplier.GSTNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@PANNumber", (object?)supplier.PANNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@TaxStatus", (object?)supplier.TaxStatus ?? DBNull.Value);
            command.Parameters.AddWithValue("@PaymentTerms", (object?)supplier.PaymentTerms ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreditDays", (object?)supplier.CreditDays ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreditLimit", (object?)supplier.CreditLimit ?? DBNull.Value);
            command.Parameters.AddWithValue("@BankName", (object?)supplier.BankName ?? DBNull.Value);
            command.Parameters.AddWithValue("@BankAccountNumber", (object?)supplier.BankAccountNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@IFSCCode", (object?)supplier.IFSCCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@BankBranch", (object?)supplier.BankBranch ?? DBNull.Value);
            command.Parameters.AddWithValue("@OnTimeDeliveryRate", (object?)supplier.OnTimeDeliveryRate ?? DBNull.Value);
            command.Parameters.AddWithValue("@QualityRating", (object?)supplier.QualityRating ?? DBNull.Value);
            command.Parameters.AddWithValue("@TotalOrders", (object?)supplier.TotalOrders ?? DBNull.Value);
            command.Parameters.AddWithValue("@RejectedOrders", (object?)supplier.RejectedOrders ?? DBNull.Value);
            command.Parameters.AddWithValue("@ServicesOffered", (object?)supplier.ServicesOffered ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProcessCapabilities", (object?)supplier.ProcessCapabilities ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialsSupplied", (object?)supplier.MaterialsSupplied ?? DBNull.Value);
            command.Parameters.AddWithValue("@StandardLeadTimeDays", (object?)supplier.StandardLeadTimeDays ?? DBNull.Value);
            command.Parameters.AddWithValue("@MinimumOrderQuantity", (object?)supplier.MinimumOrderQuantity ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", supplier.IsActive);
            command.Parameters.AddWithValue("@IsApproved", supplier.IsApproved);
            command.Parameters.AddWithValue("@Status", (object?)supplier.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@ApprovalStatus", (object?)supplier.ApprovalStatus ?? DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", (object?)supplier.Remarks ?? DBNull.Value);
            command.Parameters.AddWithValue("@InternalNotes", (object?)supplier.InternalNotes ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@CreatedBy", (object?)supplier.CreatedBy ?? DBNull.Value);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return id;
        }

        public async Task<bool> UpdateAsync(Supplier supplier)
        {
            const string query = @"
                UPDATE Masters_Suppliers
                SET SupplierCode = @SupplierCode,
                    SupplierName = @SupplierName,
                    SupplierType = @SupplierType,
                    Category = @Category,
                    ContactPerson = @ContactPerson,
                    ContactNumber = @ContactNumber,
                    Email = @Email,
                    Website = @Website,
                    AddressLine1 = @AddressLine1,
                    AddressLine2 = @AddressLine2,
                    City = @City,
                    State = @State,
                    Country = @Country,
                    PostalCode = @PostalCode,
                    GSTNumber = @GSTNumber,
                    PANNumber = @PANNumber,
                    TaxStatus = @TaxStatus,
                    PaymentTerms = @PaymentTerms,
                    CreditDays = @CreditDays,
                    CreditLimit = @CreditLimit,
                    BankName = @BankName,
                    BankAccountNumber = @BankAccountNumber,
                    IFSCCode = @IFSCCode,
                    BankBranch = @BankBranch,
                    OnTimeDeliveryRate = @OnTimeDeliveryRate,
                    QualityRating = @QualityRating,
                    TotalOrders = @TotalOrders,
                    RejectedOrders = @RejectedOrders,
                    ServicesOffered = @ServicesOffered,
                    ProcessCapabilities = @ProcessCapabilities,
                    MaterialsSupplied = @MaterialsSupplied,
                    StandardLeadTimeDays = @StandardLeadTimeDays,
                    MinimumOrderQuantity = @MinimumOrderQuantity,
                    IsActive = @IsActive,
                    IsApproved = @IsApproved,
                    Status = @Status,
                    ApprovalStatus = @ApprovalStatus,
                    Remarks = @Remarks,
                    InternalNotes = @InternalNotes,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", supplier.Id);
            command.Parameters.AddWithValue("@SupplierCode", supplier.SupplierCode);
            command.Parameters.AddWithValue("@SupplierName", supplier.SupplierName);
            command.Parameters.AddWithValue("@SupplierType", (object?)supplier.SupplierType ?? DBNull.Value);
            command.Parameters.AddWithValue("@Category", (object?)supplier.Category ?? DBNull.Value);
            command.Parameters.AddWithValue("@ContactPerson", (object?)supplier.ContactPerson ?? DBNull.Value);
            command.Parameters.AddWithValue("@ContactNumber", (object?)supplier.ContactNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@Email", (object?)supplier.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@Website", (object?)supplier.Website ?? DBNull.Value);
            command.Parameters.AddWithValue("@AddressLine1", (object?)supplier.AddressLine1 ?? DBNull.Value);
            command.Parameters.AddWithValue("@AddressLine2", (object?)supplier.AddressLine2 ?? DBNull.Value);
            command.Parameters.AddWithValue("@City", (object?)supplier.City ?? DBNull.Value);
            command.Parameters.AddWithValue("@State", (object?)supplier.State ?? DBNull.Value);
            command.Parameters.AddWithValue("@Country", (object?)supplier.Country ?? DBNull.Value);
            command.Parameters.AddWithValue("@PostalCode", (object?)supplier.PostalCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@GSTNumber", (object?)supplier.GSTNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@PANNumber", (object?)supplier.PANNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@TaxStatus", (object?)supplier.TaxStatus ?? DBNull.Value);
            command.Parameters.AddWithValue("@PaymentTerms", (object?)supplier.PaymentTerms ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreditDays", (object?)supplier.CreditDays ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreditLimit", (object?)supplier.CreditLimit ?? DBNull.Value);
            command.Parameters.AddWithValue("@BankName", (object?)supplier.BankName ?? DBNull.Value);
            command.Parameters.AddWithValue("@BankAccountNumber", (object?)supplier.BankAccountNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@IFSCCode", (object?)supplier.IFSCCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@BankBranch", (object?)supplier.BankBranch ?? DBNull.Value);
            command.Parameters.AddWithValue("@OnTimeDeliveryRate", (object?)supplier.OnTimeDeliveryRate ?? DBNull.Value);
            command.Parameters.AddWithValue("@QualityRating", (object?)supplier.QualityRating ?? DBNull.Value);
            command.Parameters.AddWithValue("@TotalOrders", (object?)supplier.TotalOrders ?? DBNull.Value);
            command.Parameters.AddWithValue("@RejectedOrders", (object?)supplier.RejectedOrders ?? DBNull.Value);
            command.Parameters.AddWithValue("@ServicesOffered", (object?)supplier.ServicesOffered ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProcessCapabilities", (object?)supplier.ProcessCapabilities ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialsSupplied", (object?)supplier.MaterialsSupplied ?? DBNull.Value);
            command.Parameters.AddWithValue("@StandardLeadTimeDays", (object?)supplier.StandardLeadTimeDays ?? DBNull.Value);
            command.Parameters.AddWithValue("@MinimumOrderQuantity", (object?)supplier.MinimumOrderQuantity ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", supplier.IsActive);
            command.Parameters.AddWithValue("@IsApproved", supplier.IsApproved);
            command.Parameters.AddWithValue("@Status", (object?)supplier.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@ApprovalStatus", (object?)supplier.ApprovalStatus ?? DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", (object?)supplier.Remarks ?? DBNull.Value);
            command.Parameters.AddWithValue("@InternalNotes", (object?)supplier.InternalNotes ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)supplier.UpdatedBy ?? DBNull.Value);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            const string query = "DELETE FROM Masters_Suppliers WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Supplier>> GetByTypeAsync(string supplierType)
        {
            const string query = @"
                SELECT * FROM Masters_Suppliers
                WHERE SupplierType = @SupplierType
                  AND IsActive = 1
                ORDER BY SupplierName";

            var suppliers = new List<Supplier>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@SupplierType", supplierType);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                suppliers.Add(MapToSupplier(reader));
            }

            return suppliers;
        }

        public async Task<IEnumerable<Supplier>> GetByCategoryAsync(string category)
        {
            const string query = @"
                SELECT * FROM Masters_Suppliers
                WHERE Category = @Category
                  AND IsActive = 1
                ORDER BY SupplierName";

            var suppliers = new List<Supplier>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Category", category);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                suppliers.Add(MapToSupplier(reader));
            }

            return suppliers;
        }

        public async Task<IEnumerable<Supplier>> GetActiveAsync()
        {
            const string query = @"
                SELECT * FROM Masters_Suppliers
                WHERE IsActive = 1
                ORDER BY SupplierName";

            var suppliers = new List<Supplier>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                suppliers.Add(MapToSupplier(reader));
            }

            return suppliers;
        }

        public async Task<IEnumerable<Supplier>> GetApprovedAsync()
        {
            const string query = @"
                SELECT * FROM Masters_Suppliers
                WHERE IsApproved = 1
                  AND IsActive = 1
                ORDER BY SupplierName";

            var suppliers = new List<Supplier>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                suppliers.Add(MapToSupplier(reader));
            }

            return suppliers;
        }

        public async Task<IEnumerable<Supplier>> GetByStatusAsync(string status)
        {
            const string query = @"
                SELECT * FROM Masters_Suppliers
                WHERE Status = @Status
                ORDER BY SupplierName";

            var suppliers = new List<Supplier>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Status", status);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                suppliers.Add(MapToSupplier(reader));
            }

            return suppliers;
        }

        public async Task<IEnumerable<Supplier>> GetByApprovalStatusAsync(string approvalStatus)
        {
            const string query = @"
                SELECT * FROM Masters_Suppliers
                WHERE ApprovalStatus = @ApprovalStatus
                ORDER BY CreatedAt DESC";

            var suppliers = new List<Supplier>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ApprovalStatus", approvalStatus);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                suppliers.Add(MapToSupplier(reader));
            }

            return suppliers;
        }

        public async Task<IEnumerable<Supplier>> GetByProcessCapabilityAsync(string processCapability)
        {
            const string query = @"
                SELECT * FROM Masters_Suppliers
                WHERE ProcessCapabilities LIKE '%' + @ProcessCapability + '%'
                  AND IsActive = 1
                  AND IsApproved = 1
                ORDER BY QualityRating DESC, OnTimeDeliveryRate DESC";

            var suppliers = new List<Supplier>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProcessCapability", processCapability);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                suppliers.Add(MapToSupplier(reader));
            }

            return suppliers;
        }

        public async Task<bool> UpdatePerformanceMetricsAsync(Guid id, decimal onTimeDeliveryRate, decimal qualityRating, int totalOrders, int rejectedOrders)
        {
            const string query = @"
                UPDATE Masters_Suppliers
                SET OnTimeDeliveryRate = @OnTimeDeliveryRate,
                    QualityRating = @QualityRating,
                    TotalOrders = @TotalOrders,
                    RejectedOrders = @RejectedOrders,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@OnTimeDeliveryRate", onTimeDeliveryRate);
            command.Parameters.AddWithValue("@QualityRating", qualityRating);
            command.Parameters.AddWithValue("@TotalOrders", totalOrders);
            command.Parameters.AddWithValue("@RejectedOrders", rejectedOrders);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Supplier>> GetTopPerformingAsync(int count)
        {
            const string query = @"
                SELECT TOP (@Count) * FROM Masters_Suppliers
                WHERE IsActive = 1
                  AND IsApproved = 1
                  AND QualityRating IS NOT NULL
                ORDER BY QualityRating DESC, OnTimeDeliveryRate DESC";

            var suppliers = new List<Supplier>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Count", count);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                suppliers.Add(MapToSupplier(reader));
            }

            return suppliers;
        }

        public async Task<IEnumerable<Supplier>> GetLowPerformingAsync(int count)
        {
            const string query = @"
                SELECT TOP (@Count) * FROM Masters_Suppliers
                WHERE IsActive = 1
                  AND IsApproved = 1
                  AND QualityRating IS NOT NULL
                ORDER BY QualityRating ASC, OnTimeDeliveryRate ASC";

            var suppliers = new List<Supplier>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Count", count);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                suppliers.Add(MapToSupplier(reader));
            }

            return suppliers;
        }

        public async Task<bool> UpdateStatusAsync(Guid id, string status)
        {
            const string query = @"
                UPDATE Masters_Suppliers
                SET Status = @Status,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Status", status);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> ApproveSupplierAsync(Guid id, string approvedBy)
        {
            const string query = @"
                UPDATE Masters_Suppliers
                SET IsApproved = 1,
                    ApprovalStatus = 'Approved',
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@UpdatedBy", approvedBy);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> RejectSupplierAsync(Guid id, string reason)
        {
            const string query = @"
                UPDATE Masters_Suppliers
                SET ApprovalStatus = 'Rejected',
                    InternalNotes = ISNULL(InternalNotes, '') + CHAR(13) + CHAR(10) + 'Rejected: ' + @Reason,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Reason", reason);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        // Mapping Method
        private static Supplier MapToSupplier(IDataReader reader)
        {
            return new Supplier
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                SupplierCode = reader.GetString(reader.GetOrdinal("SupplierCode")),
                SupplierName = reader.GetString(reader.GetOrdinal("SupplierName")),
                SupplierType = reader.IsDBNull(reader.GetOrdinal("SupplierType")) ? null : reader.GetString(reader.GetOrdinal("SupplierType")),
                Category = reader.IsDBNull(reader.GetOrdinal("Category")) ? null : reader.GetString(reader.GetOrdinal("Category")),
                ContactPerson = reader.IsDBNull(reader.GetOrdinal("ContactPerson")) ? null : reader.GetString(reader.GetOrdinal("ContactPerson")),
                ContactNumber = reader.IsDBNull(reader.GetOrdinal("ContactNumber")) ? null : reader.GetString(reader.GetOrdinal("ContactNumber")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                Website = reader.IsDBNull(reader.GetOrdinal("Website")) ? null : reader.GetString(reader.GetOrdinal("Website")),
                AddressLine1 = reader.IsDBNull(reader.GetOrdinal("AddressLine1")) ? null : reader.GetString(reader.GetOrdinal("AddressLine1")),
                AddressLine2 = reader.IsDBNull(reader.GetOrdinal("AddressLine2")) ? null : reader.GetString(reader.GetOrdinal("AddressLine2")),
                City = reader.IsDBNull(reader.GetOrdinal("City")) ? null : reader.GetString(reader.GetOrdinal("City")),
                State = reader.IsDBNull(reader.GetOrdinal("State")) ? null : reader.GetString(reader.GetOrdinal("State")),
                Country = reader.IsDBNull(reader.GetOrdinal("Country")) ? null : reader.GetString(reader.GetOrdinal("Country")),
                PostalCode = reader.IsDBNull(reader.GetOrdinal("PostalCode")) ? null : reader.GetString(reader.GetOrdinal("PostalCode")),
                GSTNumber = reader.IsDBNull(reader.GetOrdinal("GSTNumber")) ? null : reader.GetString(reader.GetOrdinal("GSTNumber")),
                PANNumber = reader.IsDBNull(reader.GetOrdinal("PANNumber")) ? null : reader.GetString(reader.GetOrdinal("PANNumber")),
                TaxStatus = reader.IsDBNull(reader.GetOrdinal("TaxStatus")) ? null : reader.GetString(reader.GetOrdinal("TaxStatus")),
                PaymentTerms = reader.IsDBNull(reader.GetOrdinal("PaymentTerms")) ? null : reader.GetString(reader.GetOrdinal("PaymentTerms")),
                CreditDays = reader.IsDBNull(reader.GetOrdinal("CreditDays")) ? null : reader.GetInt32(reader.GetOrdinal("CreditDays")),
                CreditLimit = reader.IsDBNull(reader.GetOrdinal("CreditLimit")) ? null : reader.GetDecimal(reader.GetOrdinal("CreditLimit")),
                BankName = reader.IsDBNull(reader.GetOrdinal("BankName")) ? null : reader.GetString(reader.GetOrdinal("BankName")),
                BankAccountNumber = reader.IsDBNull(reader.GetOrdinal("BankAccountNumber")) ? null : reader.GetString(reader.GetOrdinal("BankAccountNumber")),
                IFSCCode = reader.IsDBNull(reader.GetOrdinal("IFSCCode")) ? null : reader.GetString(reader.GetOrdinal("IFSCCode")),
                BankBranch = reader.IsDBNull(reader.GetOrdinal("BankBranch")) ? null : reader.GetString(reader.GetOrdinal("BankBranch")),
                OnTimeDeliveryRate = reader.IsDBNull(reader.GetOrdinal("OnTimeDeliveryRate")) ? null : reader.GetDecimal(reader.GetOrdinal("OnTimeDeliveryRate")),
                QualityRating = reader.IsDBNull(reader.GetOrdinal("QualityRating")) ? null : reader.GetDecimal(reader.GetOrdinal("QualityRating")),
                TotalOrders = reader.IsDBNull(reader.GetOrdinal("TotalOrders")) ? null : reader.GetInt32(reader.GetOrdinal("TotalOrders")),
                RejectedOrders = reader.IsDBNull(reader.GetOrdinal("RejectedOrders")) ? null : reader.GetInt32(reader.GetOrdinal("RejectedOrders")),
                ServicesOffered = reader.IsDBNull(reader.GetOrdinal("ServicesOffered")) ? null : reader.GetString(reader.GetOrdinal("ServicesOffered")),
                ProcessCapabilities = reader.IsDBNull(reader.GetOrdinal("ProcessCapabilities")) ? null : reader.GetString(reader.GetOrdinal("ProcessCapabilities")),
                MaterialsSupplied = reader.IsDBNull(reader.GetOrdinal("MaterialsSupplied")) ? null : reader.GetString(reader.GetOrdinal("MaterialsSupplied")),
                StandardLeadTimeDays = reader.IsDBNull(reader.GetOrdinal("StandardLeadTimeDays")) ? null : reader.GetInt32(reader.GetOrdinal("StandardLeadTimeDays")),
                MinimumOrderQuantity = reader.IsDBNull(reader.GetOrdinal("MinimumOrderQuantity")) ? null : reader.GetInt32(reader.GetOrdinal("MinimumOrderQuantity")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                IsApproved = reader.GetBoolean(reader.GetOrdinal("IsApproved")),
                Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                ApprovalStatus = reader.IsDBNull(reader.GetOrdinal("ApprovalStatus")) ? null : reader.GetString(reader.GetOrdinal("ApprovalStatus")),
                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
                InternalNotes = reader.IsDBNull(reader.GetOrdinal("InternalNotes")) ? null : reader.GetString(reader.GetOrdinal("InternalNotes")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy"))
            };
        }
    }
}
