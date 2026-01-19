# MultiHitech ERP - Backend

## Tech Stack
- ASP.NET Core Web API (.NET 10)
- SQL Server 2017
- ADO.NET (No Entity Framework)

## Folder Structure

backend/
+-- MultiHitechERP.API/
¦   +-- Controllers/
¦   +-- Models/
¦   +-- DTOs/
¦   +-- Services/
¦   +-- Repositories/
¦   +-- Data/
¦   +-- Enums/
+-- Database/
    +-- Migrations/
    +-- StoredProcedures/
    +-- SeedData/

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
