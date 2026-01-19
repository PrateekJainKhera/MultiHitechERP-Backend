# =====================================================================
# MultiHitech ERP - SIMPLIFIED Backend Setup Script
# ASP.NET Core Web API with ADO.NET
# PowerShell Script
# =====================================================================

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "MultiHitech ERP - SIMPLE Backend Setup" -ForegroundColor Cyan
Write-Host "Creating practical folder structure..." -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan

# Set root backend folder (run this inside multihitech/backend)
if ($PSScriptRoot) {
    $rootPath = $PSScriptRoot
} elseif ($MyInvocation.MyCommand.Path) {
    $rootPath = Split-Path -Parent $MyInvocation.MyCommand.Path
} else {
    # Fallback: use current directory
    $rootPath = Get-Location
}

# Main API Project Folder
$projectName = "MultiHitechERP.API"
$projectPath = Join-Path $rootPath $projectName

# Define ONLY NECESSARY folders (Phase 1)
$folders = @(
    # Root API Project
    "$projectPath",

    # Controllers
    "$projectPath\Controllers",
    "$projectPath\Controllers\Masters",
    "$projectPath\Controllers\Orders",
    "$projectPath\Controllers\Planning",
    "$projectPath\Controllers\Stores",
    "$projectPath\Controllers\Production",
    "$projectPath\Controllers\Quality",
    "$projectPath\Controllers\Dispatch",

    # Models (POCO)
    "$projectPath\Models",
    "$projectPath\Models\Masters",
    "$projectPath\Models\Orders",
    "$projectPath\Models\Planning",
    "$projectPath\Models\Stores",
    "$projectPath\Models\Production",
    "$projectPath\Models\Quality",
    "$projectPath\Models\Dispatch",

    # DTOs
    "$projectPath\DTOs",
    "$projectPath\DTOs\Request",
    "$projectPath\DTOs\Response",

    # Services
    "$projectPath\Services",
    "$projectPath\Services\Interfaces",
    "$projectPath\Services\Implementations",

    # Repositories (ADO.NET ONLY)
    "$projectPath\Repositories",
    "$projectPath\Repositories\Interfaces",
    "$projectPath\Repositories\Implementations",

    # Data (DB Connection)
    "$projectPath\Data",

    # Enums
    "$projectPath\Enums",

    # Database Scripts (outside API project)
    "$rootPath\Database",
    "$rootPath\Database\Migrations",
    "$rootPath\Database\StoredProcedures",
    "$rootPath\Database\SeedData",

    # Documentation
    "$rootPath\Docs"
)

# Create all folders
foreach ($folder in $folders) {
    if (-not (Test-Path -Path $folder)) {
        New-Item -ItemType Directory -Path $folder -Force | Out-Null
        Write-Host "[CREATED] $folder" -ForegroundColor Green
    } else {
        Write-Host "[EXISTS]  $folder" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Creating essential files..." -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan

# Create appsettings.json
$appsettingsPath = Join-Path $projectPath "appsettings.json"
$appsettingsContent = @"
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MultiHitechERP;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
"@

if (-not (Test-Path -Path $appsettingsPath)) {
    Set-Content -Path $appsettingsPath -Value $appsettingsContent
    Write-Host "[CREATED] appsettings.json" -ForegroundColor Green
}

# Create Program.cs (VERY SIMPLE)
$programPath = Join-Path $projectPath "Program.cs"
$programContent = @"
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Enable CORS for frontend
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.MapControllers();
app.Run();
"@

if (-not (Test-Path -Path $programPath)) {
    Set-Content -Path $programPath -Value $programContent
    Write-Host "[CREATED] Program.cs" -ForegroundColor Green
}

# Create README.md
$readmePath = Join-Path $rootPath "README.md"
$readmeContent = @"
# MultiHitech ERP - Backend

## Tech Stack
- ASP.NET Core Web API (.NET 10)
- SQL Server 2017
- ADO.NET (No Entity Framework)

## Folder Structure

backend/
├── MultiHitechERP.API/
│   ├── Controllers/
│   ├── Models/
│   ├── DTOs/
│   ├── Services/
│   ├── Repositories/
│   ├── Data/
│   └── Enums/
└── Database/
    ├── Migrations/
    ├── StoredProcedures/
    └── SeedData/

## How to Run

1. Create Web API project:
   cd MultiHitechERP.API
   dotnet new webapi -n MultiHitechERP.API --framework net10.0

2. Restore packages:
   dotnet restore

3. Run:
   dotnet run

4. Open Swagger:
   https://localhost:5001/swagger
"@

if (-not (Test-Path -Path $readmePath)) {
    Set-Content -Path $readmePath -Value $readmeContent
    Write-Host "[CREATED] README.md" -ForegroundColor Green
}

# Create .gitignore
$gitignorePath = Join-Path $rootPath ".gitignore"
$gitignoreContent = @"
bin/
obj/
.vs/
.vscode/
appsettings.Production.json
*.log
*.mdf
*.ldf
"@

if (-not (Test-Path -Path $gitignorePath)) {
    Set-Content -Path $gitignorePath -Value $gitignoreContent
    Write-Host "[CREATED] .gitignore" -ForegroundColor Green
}

Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Backend structure created successfully!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "1. Navigate to: $projectPath" -ForegroundColor White
Write-Host "2. Create Web API project:" -ForegroundColor White
Write-Host "   cd MultiHitechERP.API" -ForegroundColor Gray
Write-Host "   dotnet new webapi -n MultiHitechERP.API --framework net10.0" -ForegroundColor Gray
Write-Host "3. Create SQL database in SSMS (SQL Server 2017)" -ForegroundColor White
Write-Host "4. Start creating tables for Orders, JobCards, Material, QC, Dispatch" -ForegroundColor White
Write-Host ""
Write-Host "Happy Coding! 🚀" -ForegroundColor Cyan
