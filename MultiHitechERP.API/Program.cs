using MultiHitechERP.API.Data;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Repositories.Implementations;
using MultiHitechERP.API.Services.Interfaces;
using MultiHitechERP.API.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "MultiHitech ERP API", Version = "v1" });
});

// Database Connection Factory
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

// Register Repositories
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
builder.Services.AddScoped<IMachineRepository, MachineRepository>();
builder.Services.AddScoped<IProcessRepository, ProcessRepository>();
builder.Services.AddScoped<IOperatorRepository, OperatorRepository>();
builder.Services.AddScoped<IDrawingRepository, DrawingRepository>();
builder.Services.AddScoped<IJobCardDependencyRepository, JobCardDependencyRepository>();
builder.Services.AddScoped<IJobCardRepository, JobCardRepository>();
builder.Services.AddScoped<IMaterialRequisitionRepository, MaterialRequisitionRepository>();
builder.Services.AddScoped<IMaterialPieceRepository, MaterialPieceRepository>();
builder.Services.AddScoped<IMaterialIssueRepository, MaterialIssueRepository>();
builder.Services.AddScoped<IJobCardExecutionRepository, JobCardExecutionRepository>();
builder.Services.AddScoped<IQCResultRepository, QCResultRepository>();
builder.Services.AddScoped<IDeliveryChallanRepository, DeliveryChallanRepository>();
builder.Services.AddScoped<IBOMRepository, BOMRepository>();
builder.Services.AddScoped<IChildPartRepository, ChildPartRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();

// Register Services
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IMachineService, MachineService>();
builder.Services.AddScoped<IProcessService, ProcessService>();
builder.Services.AddScoped<IOperatorService, OperatorService>();
builder.Services.AddScoped<IDrawingService, DrawingService>();
builder.Services.AddScoped<IJobCardService, JobCardService>();
builder.Services.AddScoped<IMaterialRequisitionService, MaterialRequisitionService>();
builder.Services.AddScoped<IProductionService, ProductionService>();
builder.Services.AddScoped<IQualityService, QualityService>();
builder.Services.AddScoped<IDispatchService, DispatchService>();
builder.Services.AddScoped<IBOMService, BOMService>();
builder.Services.AddScoped<IChildPartService, ChildPartService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

// CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
