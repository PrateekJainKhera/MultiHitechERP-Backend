using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Dispatch;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    /// <summary>
    /// Delivery Challan Repository implementation using ADO.NET
    /// Manages dispatch records and delivery tracking
    /// </summary>
    public class DeliveryChallanRepository : IDeliveryChallanRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DeliveryChallanRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<DeliveryChallan?> GetByIdAsync(Guid id)
        {
            const string query = "SELECT * FROM Dispatch_DeliveryChallans WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToDeliveryChallan(reader);
            }

            return null;
        }

        public async Task<DeliveryChallan?> GetByChallanNoAsync(string challanNo)
        {
            const string query = "SELECT * FROM Dispatch_DeliveryChallans WHERE ChallanNo = @ChallanNo";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ChallanNo", challanNo);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToDeliveryChallan(reader);
            }

            return null;
        }

        public async Task<IEnumerable<DeliveryChallan>> GetAllAsync()
        {
            const string query = "SELECT * FROM Dispatch_DeliveryChallans ORDER BY ChallanDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var challans = new List<DeliveryChallan>();
            while (await reader.ReadAsync())
            {
                challans.Add(MapToDeliveryChallan(reader));
            }

            return challans;
        }

        public async Task<IEnumerable<DeliveryChallan>> GetByOrderIdAsync(Guid orderId)
        {
            const string query = "SELECT * FROM Dispatch_DeliveryChallans WHERE OrderId = @OrderId ORDER BY ChallanDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@OrderId", orderId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var challans = new List<DeliveryChallan>();
            while (await reader.ReadAsync())
            {
                challans.Add(MapToDeliveryChallan(reader));
            }

            return challans;
        }

        public async Task<IEnumerable<DeliveryChallan>> GetByCustomerIdAsync(Guid customerId)
        {
            const string query = "SELECT * FROM Dispatch_DeliveryChallans WHERE CustomerId = @CustomerId ORDER BY ChallanDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CustomerId", customerId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var challans = new List<DeliveryChallan>();
            while (await reader.ReadAsync())
            {
                challans.Add(MapToDeliveryChallan(reader));
            }

            return challans;
        }

        public async Task<IEnumerable<DeliveryChallan>> GetByStatusAsync(string status)
        {
            const string query = "SELECT * FROM Dispatch_DeliveryChallans WHERE Status = @Status ORDER BY ChallanDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Status", status);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var challans = new List<DeliveryChallan>();
            while (await reader.ReadAsync())
            {
                challans.Add(MapToDeliveryChallan(reader));
            }

            return challans;
        }

        public async Task<Guid> InsertAsync(DeliveryChallan challan)
        {
            const string query = @"
                INSERT INTO Dispatch_DeliveryChallans (
                    Id, ChallanNo, ChallanDate, OrderId, OrderNo,
                    CustomerId, CustomerName, ProductId, ProductName,
                    QuantityDispatched, DeliveryDate, DeliveryAddress,
                    TransportMode, VehicleNumber, DriverName, DriverContact,
                    NumberOfPackages, PackagingType, TotalWeight,
                    Status, DispatchedAt, DeliveredAt,
                    InvoiceNo, InvoiceDate,
                    ReceivedBy, AcknowledgedAt, DeliveryRemarks,
                    Remarks, CreatedAt, CreatedBy
                ) VALUES (
                    @Id, @ChallanNo, @ChallanDate, @OrderId, @OrderNo,
                    @CustomerId, @CustomerName, @ProductId, @ProductName,
                    @QuantityDispatched, @DeliveryDate, @DeliveryAddress,
                    @TransportMode, @VehicleNumber, @DriverName, @DriverContact,
                    @NumberOfPackages, @PackagingType, @TotalWeight,
                    @Status, @DispatchedAt, @DeliveredAt,
                    @InvoiceNo, @InvoiceDate,
                    @ReceivedBy, @AcknowledgedAt, @DeliveryRemarks,
                    @Remarks, @CreatedAt, @CreatedBy
                )";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            var id = Guid.NewGuid();

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@ChallanNo", challan.ChallanNo);
            command.Parameters.AddWithValue("@ChallanDate", challan.ChallanDate);
            command.Parameters.AddWithValue("@OrderId", challan.OrderId);
            command.Parameters.AddWithValue("@OrderNo", (object?)challan.OrderNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@CustomerId", challan.CustomerId);
            command.Parameters.AddWithValue("@CustomerName", (object?)challan.CustomerName ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductId", challan.ProductId);
            command.Parameters.AddWithValue("@ProductName", (object?)challan.ProductName ?? DBNull.Value);
            command.Parameters.AddWithValue("@QuantityDispatched", challan.QuantityDispatched);
            command.Parameters.AddWithValue("@DeliveryDate", (object?)challan.DeliveryDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@DeliveryAddress", (object?)challan.DeliveryAddress ?? DBNull.Value);
            command.Parameters.AddWithValue("@TransportMode", (object?)challan.TransportMode ?? DBNull.Value);
            command.Parameters.AddWithValue("@VehicleNumber", (object?)challan.VehicleNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@DriverName", (object?)challan.DriverName ?? DBNull.Value);
            command.Parameters.AddWithValue("@DriverContact", (object?)challan.DriverContact ?? DBNull.Value);
            command.Parameters.AddWithValue("@NumberOfPackages", (object?)challan.NumberOfPackages ?? DBNull.Value);
            command.Parameters.AddWithValue("@PackagingType", (object?)challan.PackagingType ?? DBNull.Value);
            command.Parameters.AddWithValue("@TotalWeight", (object?)challan.TotalWeight ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", challan.Status);
            command.Parameters.AddWithValue("@DispatchedAt", (object?)challan.DispatchedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@DeliveredAt", (object?)challan.DeliveredAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@InvoiceNo", (object?)challan.InvoiceNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@InvoiceDate", (object?)challan.InvoiceDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReceivedBy", (object?)challan.ReceivedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@AcknowledgedAt", (object?)challan.AcknowledgedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@DeliveryRemarks", (object?)challan.DeliveryRemarks ?? DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", (object?)challan.Remarks ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@CreatedBy", (object?)challan.CreatedBy ?? DBNull.Value);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            return id;
        }

        public async Task<bool> UpdateAsync(DeliveryChallan challan)
        {
            const string query = @"
                UPDATE Dispatch_DeliveryChallans SET
                    ChallanNo = @ChallanNo,
                    ChallanDate = @ChallanDate,
                    OrderId = @OrderId,
                    OrderNo = @OrderNo,
                    CustomerId = @CustomerId,
                    CustomerName = @CustomerName,
                    ProductId = @ProductId,
                    ProductName = @ProductName,
                    QuantityDispatched = @QuantityDispatched,
                    DeliveryDate = @DeliveryDate,
                    DeliveryAddress = @DeliveryAddress,
                    TransportMode = @TransportMode,
                    VehicleNumber = @VehicleNumber,
                    DriverName = @DriverName,
                    DriverContact = @DriverContact,
                    NumberOfPackages = @NumberOfPackages,
                    PackagingType = @PackagingType,
                    TotalWeight = @TotalWeight,
                    Status = @Status,
                    DispatchedAt = @DispatchedAt,
                    DeliveredAt = @DeliveredAt,
                    InvoiceNo = @InvoiceNo,
                    InvoiceDate = @InvoiceDate,
                    ReceivedBy = @ReceivedBy,
                    AcknowledgedAt = @AcknowledgedAt,
                    DeliveryRemarks = @DeliveryRemarks,
                    Remarks = @Remarks
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", challan.Id);
            command.Parameters.AddWithValue("@ChallanNo", challan.ChallanNo);
            command.Parameters.AddWithValue("@ChallanDate", challan.ChallanDate);
            command.Parameters.AddWithValue("@OrderId", challan.OrderId);
            command.Parameters.AddWithValue("@OrderNo", (object?)challan.OrderNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@CustomerId", challan.CustomerId);
            command.Parameters.AddWithValue("@CustomerName", (object?)challan.CustomerName ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductId", challan.ProductId);
            command.Parameters.AddWithValue("@ProductName", (object?)challan.ProductName ?? DBNull.Value);
            command.Parameters.AddWithValue("@QuantityDispatched", challan.QuantityDispatched);
            command.Parameters.AddWithValue("@DeliveryDate", (object?)challan.DeliveryDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@DeliveryAddress", (object?)challan.DeliveryAddress ?? DBNull.Value);
            command.Parameters.AddWithValue("@TransportMode", (object?)challan.TransportMode ?? DBNull.Value);
            command.Parameters.AddWithValue("@VehicleNumber", (object?)challan.VehicleNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@DriverName", (object?)challan.DriverName ?? DBNull.Value);
            command.Parameters.AddWithValue("@DriverContact", (object?)challan.DriverContact ?? DBNull.Value);
            command.Parameters.AddWithValue("@NumberOfPackages", (object?)challan.NumberOfPackages ?? DBNull.Value);
            command.Parameters.AddWithValue("@PackagingType", (object?)challan.PackagingType ?? DBNull.Value);
            command.Parameters.AddWithValue("@TotalWeight", (object?)challan.TotalWeight ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", challan.Status);
            command.Parameters.AddWithValue("@DispatchedAt", (object?)challan.DispatchedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@DeliveredAt", (object?)challan.DeliveredAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@InvoiceNo", (object?)challan.InvoiceNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@InvoiceDate", (object?)challan.InvoiceDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReceivedBy", (object?)challan.ReceivedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@AcknowledgedAt", (object?)challan.AcknowledgedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@DeliveryRemarks", (object?)challan.DeliveryRemarks ?? DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", (object?)challan.Remarks ?? DBNull.Value);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            const string query = "DELETE FROM Dispatch_DeliveryChallans WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> UpdateStatusAsync(Guid id, string status)
        {
            const string query = "UPDATE Dispatch_DeliveryChallans SET Status = @Status WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Status", status);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DispatchChallanAsync(Guid id, DateTime dispatchedAt)
        {
            const string query = @"
                UPDATE Dispatch_DeliveryChallans
                SET Status = 'Dispatched',
                    DispatchedAt = @DispatchedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@DispatchedAt", dispatchedAt);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeliverChallanAsync(Guid id, DateTime deliveredAt, string receivedBy)
        {
            const string query = @"
                UPDATE Dispatch_DeliveryChallans
                SET Status = 'Delivered',
                    DeliveredAt = @DeliveredAt,
                    ReceivedBy = @ReceivedBy,
                    AcknowledgedAt = @AcknowledgedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@DeliveredAt", deliveredAt);
            command.Parameters.AddWithValue("@ReceivedBy", receivedBy);
            command.Parameters.AddWithValue("@AcknowledgedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<DeliveryChallan>> GetPendingChallansAsync()
        {
            const string query = "SELECT * FROM Dispatch_DeliveryChallans WHERE Status = 'Pending' ORDER BY ChallanDate ASC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var challans = new List<DeliveryChallan>();
            while (await reader.ReadAsync())
            {
                challans.Add(MapToDeliveryChallan(reader));
            }

            return challans;
        }

        public async Task<IEnumerable<DeliveryChallan>> GetDispatchedChallansAsync()
        {
            const string query = "SELECT * FROM Dispatch_DeliveryChallans WHERE Status = 'Dispatched' ORDER BY DispatchedAt DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var challans = new List<DeliveryChallan>();
            while (await reader.ReadAsync())
            {
                challans.Add(MapToDeliveryChallan(reader));
            }

            return challans;
        }

        public async Task<IEnumerable<DeliveryChallan>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            const string query = "SELECT * FROM Dispatch_DeliveryChallans WHERE ChallanDate BETWEEN @StartDate AND @EndDate ORDER BY ChallanDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var challans = new List<DeliveryChallan>();
            while (await reader.ReadAsync())
            {
                challans.Add(MapToDeliveryChallan(reader));
            }

            return challans;
        }

        public async Task<IEnumerable<DeliveryChallan>> GetByVehicleNumberAsync(string vehicleNumber)
        {
            const string query = "SELECT * FROM Dispatch_DeliveryChallans WHERE VehicleNumber = @VehicleNumber ORDER BY ChallanDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@VehicleNumber", vehicleNumber);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var challans = new List<DeliveryChallan>();
            while (await reader.ReadAsync())
            {
                challans.Add(MapToDeliveryChallan(reader));
            }

            return challans;
        }

        private static DeliveryChallan MapToDeliveryChallan(IDataReader reader)
        {
            return new DeliveryChallan
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                ChallanNo = reader.GetString(reader.GetOrdinal("ChallanNo")),
                ChallanDate = reader.GetDateTime(reader.GetOrdinal("ChallanDate")),
                OrderId = reader.GetGuid(reader.GetOrdinal("OrderId")),
                OrderNo = reader.IsDBNull(reader.GetOrdinal("OrderNo")) ? null : reader.GetString(reader.GetOrdinal("OrderNo")),
                CustomerId = reader.GetGuid(reader.GetOrdinal("CustomerId")),
                CustomerName = reader.IsDBNull(reader.GetOrdinal("CustomerName")) ? null : reader.GetString(reader.GetOrdinal("CustomerName")),
                ProductId = reader.GetGuid(reader.GetOrdinal("ProductId")),
                ProductName = reader.IsDBNull(reader.GetOrdinal("ProductName")) ? null : reader.GetString(reader.GetOrdinal("ProductName")),
                QuantityDispatched = reader.GetInt32(reader.GetOrdinal("QuantityDispatched")),
                DeliveryDate = reader.IsDBNull(reader.GetOrdinal("DeliveryDate")) ? null : reader.GetDateTime(reader.GetOrdinal("DeliveryDate")),
                DeliveryAddress = reader.IsDBNull(reader.GetOrdinal("DeliveryAddress")) ? null : reader.GetString(reader.GetOrdinal("DeliveryAddress")),
                TransportMode = reader.IsDBNull(reader.GetOrdinal("TransportMode")) ? null : reader.GetString(reader.GetOrdinal("TransportMode")),
                VehicleNumber = reader.IsDBNull(reader.GetOrdinal("VehicleNumber")) ? null : reader.GetString(reader.GetOrdinal("VehicleNumber")),
                DriverName = reader.IsDBNull(reader.GetOrdinal("DriverName")) ? null : reader.GetString(reader.GetOrdinal("DriverName")),
                DriverContact = reader.IsDBNull(reader.GetOrdinal("DriverContact")) ? null : reader.GetString(reader.GetOrdinal("DriverContact")),
                NumberOfPackages = reader.IsDBNull(reader.GetOrdinal("NumberOfPackages")) ? null : reader.GetInt32(reader.GetOrdinal("NumberOfPackages")),
                PackagingType = reader.IsDBNull(reader.GetOrdinal("PackagingType")) ? null : reader.GetString(reader.GetOrdinal("PackagingType")),
                TotalWeight = reader.IsDBNull(reader.GetOrdinal("TotalWeight")) ? null : reader.GetDecimal(reader.GetOrdinal("TotalWeight")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                DispatchedAt = reader.IsDBNull(reader.GetOrdinal("DispatchedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("DispatchedAt")),
                DeliveredAt = reader.IsDBNull(reader.GetOrdinal("DeliveredAt")) ? null : reader.GetDateTime(reader.GetOrdinal("DeliveredAt")),
                InvoiceNo = reader.IsDBNull(reader.GetOrdinal("InvoiceNo")) ? null : reader.GetString(reader.GetOrdinal("InvoiceNo")),
                InvoiceDate = reader.IsDBNull(reader.GetOrdinal("InvoiceDate")) ? null : reader.GetDateTime(reader.GetOrdinal("InvoiceDate")),
                ReceivedBy = reader.IsDBNull(reader.GetOrdinal("ReceivedBy")) ? null : reader.GetString(reader.GetOrdinal("ReceivedBy")),
                AcknowledgedAt = reader.IsDBNull(reader.GetOrdinal("AcknowledgedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("AcknowledgedAt")),
                DeliveryRemarks = reader.IsDBNull(reader.GetOrdinal("DeliveryRemarks")) ? null : reader.GetString(reader.GetOrdinal("DeliveryRemarks")),
                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy"))
            };
        }
    }
}
