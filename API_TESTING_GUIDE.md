# MultiHitech ERP API - Testing Guide

**API Base URL:** `http://localhost:5217`
**Last Updated:** 2026-01-20

---

## 🚀 Getting Started

### 1. Start the API
```bash
cd backend/MultiHitechERP.API
dotnet run
```

### 2. Verify API is Running
Open browser: `http://localhost:5217/swagger`

---

## 📋 Testing Order (Follow This Sequence)

1. **Customers** - Create customers first
2. **Materials** - Create materials for production
3. **Machines** - Create machines for processing
4. **Processes** - Create manufacturing processes
5. **Operators** - Create operators for job execution
6. **Products** - Create products (requires materials, processes)
7. **Drawings** - Create drawings for products
8. **Orders** - Create orders (requires customers, products)
9. **Job Cards** - Create job cards with dependencies (requires orders, processes, drawings)

---

## 1️⃣ Customer API Testing

### Base URL: `/api/customers`

### ✅ Create Customer (POST)

**Endpoint:** `POST http://localhost:5217/api/customers`

**Headers:**
```
Content-Type: application/json
```

**Test Data 1 - ABC Industries:**
```json
{
  "customerCode": "CUST001",
  "customerName": "ABC Industries Pvt Ltd",
  "contactPerson": "Rajesh Kumar",
  "email": "rajesh@abcindustries.com",
  "phone": "022-12345678",
  "mobile": "9876543210",
  "address": "Plot No 123, MIDC Area",
  "city": "Mumbai",
  "state": "Maharashtra",
  "country": "India",
  "pinCode": "400001",
  "gstNumber": "27AABCU9603R1ZM",
  "panNumber": "AABCU9603R",
  "customerType": "Manufacturing",
  "industry": "Automobile",
  "creditDays": 45,
  "creditLimit": 500000,
  "paymentTerms": "Net 45 Days",
  "customerRating": "A",
  "classification": "Premium",
  "remarks": "Long-term customer, excellent payment record",
  "createdBy": "Admin"
}
```

**Expected Response:**
```json
{
  "success": true,
  "message": "Customer 'CUST001' created successfully",
  "data": "guid-here",
  "errors": null
}
```

**Test Data 2 - XYZ Engineering:**
```json
{
  "customerCode": "CUST002",
  "customerName": "XYZ Engineering Works",
  "contactPerson": "Suresh Patel",
  "email": "suresh@xyzeng.com",
  "phone": "020-87654321",
  "mobile": "9123456789",
  "address": "Survey No 45, Industrial Estate",
  "city": "Pune",
  "state": "Maharashtra",
  "country": "India",
  "pinCode": "411001",
  "gstNumber": "27XYZEP1234Q1Z5",
  "panNumber": "XYZEP1234Q",
  "customerType": "Trading",
  "industry": "Industrial Equipment",
  "creditDays": 30,
  "creditLimit": 300000,
  "paymentTerms": "Net 30 Days",
  "customerRating": "B",
  "classification": "Standard",
  "remarks": "Regular customer",
  "createdBy": "Admin"
}
```

**Test Data 3 - Quick Rollers Ltd:**
```json
{
  "customerCode": "CUST003",
  "customerName": "Quick Rollers Limited",
  "contactPerson": "Amit Shah",
  "email": "amit@quickrollers.com",
  "phone": "079-11223344",
  "mobile": "9988776655",
  "address": "Block 7, GIDC Estate",
  "city": "Ahmedabad",
  "state": "Gujarat",
  "country": "India",
  "pinCode": "380001",
  "gstNumber": "24QRLMT5678P1Z9",
  "panNumber": "QRLMT5678P",
  "customerType": "OEM",
  "industry": "Printing & Packaging",
  "creditDays": 60,
  "creditLimit": 750000,
  "paymentTerms": "Net 60 Days",
  "customerRating": "A+",
  "classification": "Premium",
  "remarks": "Strategic partner, large volume orders",
  "createdBy": "Admin"
}
```

---

### ✅ Get All Customers (GET)

**Endpoint:** `GET http://localhost:5217/api/customers`

**Expected Response:**
```json
{
  "success": true,
  "message": null,
  "data": [
    {
      "id": "guid",
      "customerCode": "CUST001",
      "customerName": "ABC Industries Pvt Ltd",
      "contactPerson": "Rajesh Kumar",
      "email": "rajesh@abcindustries.com",
      "phone": "022-12345678",
      "mobile": "9876543210",
      "address": "Plot No 123, MIDC Area",
      "city": "Mumbai",
      "state": "Maharashtra",
      "country": "India",
      "pinCode": "400001",
      "gstNumber": "27AABCU9603R1ZM",
      "panNumber": "AABCU9603R",
      "customerType": "Manufacturing",
      "industry": "Automobile",
      "creditDays": 45,
      "creditLimit": 500000,
      "paymentTerms": "Net 45 Days",
      "isActive": true,
      "status": "Active",
      "customerRating": "A",
      "classification": "Premium",
      "remarks": "Long-term customer, excellent payment record",
      "createdAt": "2026-01-19T...",
      "createdBy": "Admin",
      "updatedAt": null,
      "updatedBy": null
    }
  ],
  "errors": null
}
```

---

### ✅ Get Customer by ID (GET)

**Endpoint:** `GET http://localhost:5217/api/customers/{id}`

Replace `{id}` with the GUID from create response.

---

### ✅ Get Customer by Code (GET)

**Endpoint:** `GET http://localhost:5217/api/customers/by-code/CUST001`

---

### ✅ Get Active Customers (GET)

**Endpoint:** `GET http://localhost:5217/api/customers/active`

---

### ✅ Search Customers by Name (GET)

**Endpoint:** `GET http://localhost:5217/api/customers/search?searchTerm=ABC`

---

### ✅ Get Customers by City (GET)

**Endpoint:** `GET http://localhost:5217/api/customers/by-city/Mumbai`

---

### ✅ Get Customers by State (GET)

**Endpoint:** `GET http://localhost:5217/api/customers/by-state/Maharashtra`

---

### ✅ Get Customers by Type (GET)

**Endpoint:** `GET http://localhost:5217/api/customers/by-type/Manufacturing`

---

### ✅ Update Customer (PUT)

**Endpoint:** `PUT http://localhost:5217/api/customers/{id}`

**Body:**
```json
{
  "id": "guid-from-get-request",
  "customerCode": "CUST001",
  "customerName": "ABC Industries Pvt Ltd (Updated)",
  "contactPerson": "Rajesh Kumar",
  "email": "rajesh.new@abcindustries.com",
  "phone": "022-12345678",
  "mobile": "9876543210",
  "address": "Plot No 123, MIDC Area",
  "city": "Mumbai",
  "state": "Maharashtra",
  "country": "India",
  "pinCode": "400001",
  "gstNumber": "27AABCU9603R1ZM",
  "panNumber": "AABCU9603R",
  "customerType": "Manufacturing",
  "industry": "Automobile",
  "creditDays": 45,
  "creditLimit": 500000,
  "paymentTerms": "Net 45 Days",
  "isActive": true,
  "status": "Active",
  "customerRating": "A",
  "classification": "Premium",
  "remarks": "Updated remarks",
  "updatedBy": "Admin"
}
```

---

### ✅ Deactivate Customer (POST)

**Endpoint:** `POST http://localhost:5217/api/customers/{id}/deactivate`

---

### ✅ Activate Customer (POST)

**Endpoint:** `POST http://localhost:5217/api/customers/{id}/activate`

---

### ✅ Delete Customer (DELETE)

**Endpoint:** `DELETE http://localhost:5217/api/customers/{id}`

---

## 2️⃣ Material API Testing

### Base URL: `/api/materials`

### ✅ Create Material (POST)

**Endpoint:** `POST http://localhost:5217/api/materials`

**Test Data 1 - EN8 Steel Rod:**
```json
{
  "materialCode": "MAT001",
  "materialName": "EN8 Steel Rod - 50mm Dia",
  "category": "Steel",
  "subCategory": "Carbon Steel",
  "materialType": "Round Bar",
  "grade": "EN8",
  "specification": "IS 2062 Grade A",
  "description": "Carbon steel round bar for general engineering",
  "hsnCode": "72163100",
  "standardLength": 6000,
  "diameter": 50,
  "thickness": null,
  "width": null,
  "primaryUOM": "KG",
  "secondaryUOM": "MTR",
  "conversionFactor": 15.4,
  "weightPerMeter": 15.4,
  "weightPerPiece": 92.4,
  "density": 7.85,
  "standardCost": 75,
  "lastPurchasePrice": 72,
  "lastPurchaseDate": "2026-01-15T00:00:00Z",
  "minStockLevel": 1000,
  "maxStockLevel": 5000,
  "reorderLevel": 1500,
  "reorderQuantity": 2000,
  "leadTimeDays": 7,
  "preferredSupplierId": null,
  "preferredSupplierName": "Steel Suppliers Co",
  "storageLocation": "Rack A1",
  "storageConditions": "Dry, covered area",
  "remarks": "High-demand material",
  "createdBy": "Admin"
}
```

**Test Data 2 - SS 304 Sheet:**
```json
{
  "materialCode": "MAT002",
  "materialName": "Stainless Steel 304 Sheet",
  "category": "Stainless Steel",
  "subCategory": "SS 304",
  "materialType": "Sheet",
  "grade": "304",
  "specification": "ASTM A240",
  "description": "Stainless steel sheet for corrosion resistance",
  "hsnCode": "72193100",
  "standardLength": 2440,
  "diameter": null,
  "thickness": 2,
  "width": 1220,
  "primaryUOM": "KG",
  "secondaryUOM": "SQM",
  "conversionFactor": 15.7,
  "weightPerMeter": null,
  "weightPerPiece": 93.6,
  "density": 7.93,
  "standardCost": 285,
  "lastPurchasePrice": 280,
  "lastPurchaseDate": "2026-01-10T00:00:00Z",
  "minStockLevel": 500,
  "maxStockLevel": 2000,
  "reorderLevel": 750,
  "reorderQuantity": 1000,
  "leadTimeDays": 15,
  "preferredSupplierId": null,
  "preferredSupplierName": "SS Mart",
  "storageLocation": "Rack B3",
  "storageConditions": "Dry, protected from moisture",
  "remarks": "Premium grade material",
  "createdBy": "Admin"
}
```

**Test Data 3 - Aluminum 6061 Bar:**
```json
{
  "materialCode": "MAT003",
  "materialName": "Aluminum 6061 Round Bar - 30mm",
  "category": "Aluminum",
  "subCategory": "AL 6061",
  "materialType": "Round Bar",
  "grade": "6061-T6",
  "specification": "ASTM B211",
  "description": "Aluminum alloy bar with good strength",
  "hsnCode": "76042100",
  "standardLength": 6000,
  "diameter": 30,
  "thickness": null,
  "width": null,
  "primaryUOM": "KG",
  "secondaryUOM": "MTR",
  "conversionFactor": 1.92,
  "weightPerMeter": 1.92,
  "weightPerPiece": 11.52,
  "density": 2.7,
  "standardCost": 320,
  "lastPurchasePrice": 315,
  "lastPurchaseDate": "2026-01-12T00:00:00Z",
  "minStockLevel": 300,
  "maxStockLevel": 1500,
  "reorderLevel": 500,
  "reorderQuantity": 800,
  "leadTimeDays": 10,
  "preferredSupplierId": null,
  "preferredSupplierName": "Aluminum India Ltd",
  "storageLocation": "Rack C2",
  "storageConditions": "Dry storage",
  "remarks": "Used for lightweight applications",
  "createdBy": "Admin"
}
```

---

### ✅ Get All Materials (GET)

**Endpoint:** `GET http://localhost:5217/api/materials`

---

### ✅ Get Material by ID (GET)

**Endpoint:** `GET http://localhost:5217/api/materials/{id}`

---

### ✅ Get Material by Code (GET)

**Endpoint:** `GET http://localhost:5217/api/materials/by-code/MAT001`

---

### ✅ Get Active Materials (GET)

**Endpoint:** `GET http://localhost:5217/api/materials/active`

---

### ✅ Search Materials by Name (GET)

**Endpoint:** `GET http://localhost:5217/api/materials/search?searchTerm=Steel`

---

### ✅ Get Materials by Category (GET)

**Endpoint:** `GET http://localhost:5217/api/materials/by-category/Steel`

---

### ✅ Get Materials by Grade (GET)

**Endpoint:** `GET http://localhost:5217/api/materials/by-grade/EN8`

---

### ✅ Get Materials by Type (GET)

**Endpoint:** `GET http://localhost:5217/api/materials/by-type/Round Bar`

---

### ✅ Get Low Stock Materials (GET)

**Endpoint:** `GET http://localhost:5217/api/materials/low-stock`

---

## 3️⃣ Machine API Testing

### Base URL: `/api/machines`

### ✅ Create Machine (POST)

**Endpoint:** `POST http://localhost:5217/api/machines`

**Test Data 1 - CNC Lathe:**
```json
{
  "machineCode": "MCH001",
  "machineName": "CNC Lathe Machine - Fanuc",
  "machineType": "CNC Lathe",
  "category": "Turning",
  "manufacturer": "HAAS Automation",
  "model": "ST-30",
  "serialNumber": "HAAS-ST30-2023-1234",
  "yearOfManufacture": 2023,
  "capacity": 300,
  "capacityUnit": "mm",
  "specifications": "Max turning dia: 300mm, Max length: 600mm, 12-station turret",
  "maxWorkpieceLength": 600,
  "maxWorkpieceDiameter": 300,
  "chuckSize": 250,
  "department": "Machining",
  "shopFloor": "Shop Floor 1",
  "location": "Bay A",
  "hourlyRate": 800,
  "powerConsumption": 15,
  "operatorsRequired": 1,
  "purchaseDate": "2023-06-15T00:00:00Z",
  "maintenanceSchedule": "Every 1000 hours",
  "remarks": "High-precision CNC machine",
  "createdBy": "Admin"
}
```

**Test Data 2 - Grinding Machine:**
```json
{
  "machineCode": "MCH002",
  "machineName": "Surface Grinding Machine",
  "machineType": "Surface Grinder",
  "category": "Grinding",
  "manufacturer": "Chevalier",
  "model": "FSG-2A1224",
  "serialNumber": "CHV-FSG-2023-5678",
  "yearOfManufacture": 2022,
  "capacity": 600,
  "capacityUnit": "mm",
  "specifications": "Table size: 300x600mm, Spindle speed: 2850 RPM",
  "maxWorkpieceLength": 600,
  "maxWorkpieceDiameter": null,
  "chuckSize": null,
  "department": "Grinding",
  "shopFloor": "Shop Floor 1",
  "location": "Bay B",
  "hourlyRate": 650,
  "powerConsumption": 12,
  "operatorsRequired": 1,
  "purchaseDate": "2022-03-20T00:00:00Z",
  "maintenanceSchedule": "Every 800 hours",
  "remarks": "For finishing operations",
  "createdBy": "Admin"
}
```

**Test Data 3 - Cutting Saw:**
```json
{
  "machineCode": "MCH003",
  "machineName": "Hydraulic Cutting Saw",
  "machineType": "Cutting Saw",
  "category": "Cutting",
  "manufacturer": "DoAll",
  "model": "C-916SA",
  "serialNumber": "DOALL-C916-2021-9012",
  "yearOfManufacture": 2021,
  "capacity": 250,
  "capacityUnit": "mm",
  "specifications": "Max cutting capacity: 250mm round, 300mm rectangular",
  "maxWorkpieceLength": null,
  "maxWorkpieceDiameter": 250,
  "chuckSize": null,
  "department": "Material Prep",
  "shopFloor": "Shop Floor 1",
  "location": "Bay C",
  "hourlyRate": 400,
  "powerConsumption": 8,
  "operatorsRequired": 1,
  "purchaseDate": "2021-09-10T00:00:00Z",
  "maintenanceSchedule": "Every 500 hours",
  "remarks": "For raw material cutting",
  "createdBy": "Admin"
}
```

---

### ✅ Get All Machines (GET)

**Endpoint:** `GET http://localhost:5217/api/machines`

---

### ✅ Get Machine by ID (GET)

**Endpoint:** `GET http://localhost:5217/api/machines/{id}`

---

### ✅ Get Available Machines (GET)

**Endpoint:** `GET http://localhost:5217/api/machines/available`

---

### ✅ Get Machines by Type (GET)

**Endpoint:** `GET http://localhost:5217/api/machines/by-type/CNC Lathe`

---

### ✅ Get Machines by Department (GET)

**Endpoint:** `GET http://localhost:5217/api/machines/by-department/Machining`

---

### ✅ Get Machines Due for Maintenance (GET)

**Endpoint:** `GET http://localhost:5217/api/machines/maintenance-due`

---

### ✅ Assign Machine to Job Card (POST)

**Endpoint:** `POST http://localhost:5217/api/machines/{id}/assign`

**Headers:**
```
Content-Type: application/json
```

**Body:**
```json
"JOB-001"
```

---

### ✅ Release Machine from Job Card (POST)

**Endpoint:** `POST http://localhost:5217/api/machines/{id}/release`

---

## 4️⃣ Process API Testing

### Base URL: `/api/processes`

### ✅ Create Process (POST)

**Endpoint:** `POST http://localhost:5217/api/processes`

**Test Data 1 - CNC Turning:**
```json
{
  "processCode": "PROC001",
  "processName": "CNC Turning Operation",
  "processType": "Machining",
  "category": "Turning",
  "department": "Machining",
  "description": "CNC lathe turning operation for cylindrical parts",
  "processDetails": "Rough turning, finish turning, grooving, threading",
  "machineType": "CNC Lathe",
  "defaultMachineId": null,
  "defaultMachineName": "CNC Lathe Machine - Fanuc",
  "standardSetupTimeMin": 30,
  "standardCycleTimeMin": 120,
  "standardCycleTimePerPiece": 15,
  "skillLevel": "Advanced",
  "operatorsRequired": 1,
  "hourlyRate": 800,
  "standardCostPerPiece": 200,
  "requiresQC": true,
  "qcCheckpoints": "Dimensional check, surface finish, tolerance verification",
  "isOutsourced": false,
  "preferredVendor": null,
  "remarks": "Critical process requiring skilled operator",
  "createdBy": "Admin"
}
```

**Test Data 2 - Grinding:**
```json
{
  "processCode": "PROC002",
  "processName": "Surface Grinding",
  "processType": "Finishing",
  "category": "Grinding",
  "department": "Grinding",
  "description": "Surface grinding for achieving tight tolerances",
  "processDetails": "Rough grinding, finish grinding, spark-out passes",
  "machineType": "Surface Grinder",
  "defaultMachineId": null,
  "defaultMachineName": "Surface Grinding Machine",
  "standardSetupTimeMin": 20,
  "standardCycleTimeMin": 90,
  "standardCycleTimePerPiece": 12,
  "skillLevel": "Intermediate",
  "operatorsRequired": 1,
  "hourlyRate": 650,
  "standardCostPerPiece": 130,
  "requiresQC": true,
  "qcCheckpoints": "Surface roughness, flatness, dimensional accuracy",
  "isOutsourced": false,
  "preferredVendor": null,
  "remarks": "Finishing operation",
  "createdBy": "Admin"
}
```

**Test Data 3 - Heat Treatment (Outsourced):**
```json
{
  "processCode": "PROC003",
  "processName": "Heat Treatment - Hardening",
  "processType": "Heat Treatment",
  "category": "Thermal",
  "department": "Outsourced",
  "description": "Hardening and tempering heat treatment",
  "processDetails": "Heating to 850°C, quenching in oil, tempering at 200°C",
  "machineType": "Furnace",
  "defaultMachineId": null,
  "defaultMachineName": null,
  "standardSetupTimeMin": 0,
  "standardCycleTimeMin": 480,
  "standardCycleTimePerPiece": 0,
  "skillLevel": "Specialized",
  "operatorsRequired": 0,
  "hourlyRate": 0,
  "standardCostPerPiece": 350,
  "requiresQC": true,
  "qcCheckpoints": "Hardness test, metallurgical analysis",
  "isOutsourced": true,
  "preferredVendor": "Heat Treat India Ltd",
  "remarks": "Outsourced to specialist vendor",
  "createdBy": "Admin"
}
```

**Test Data 4 - Material Cutting:**
```json
{
  "processCode": "PROC004",
  "processName": "Material Cutting - Saw",
  "processType": "Cutting",
  "category": "Material Prep",
  "department": "Material Prep",
  "description": "Cutting raw material to required length",
  "processDetails": "Measure, mark, cut, deburr",
  "machineType": "Cutting Saw",
  "defaultMachineId": null,
  "defaultMachineName": "Hydraulic Cutting Saw",
  "standardSetupTimeMin": 10,
  "standardCycleTimeMin": 30,
  "standardCycleTimePerPiece": 5,
  "skillLevel": "Basic",
  "operatorsRequired": 1,
  "hourlyRate": 400,
  "standardCostPerPiece": 35,
  "requiresQC": false,
  "qcCheckpoints": "Length verification",
  "isOutsourced": false,
  "preferredVendor": null,
  "remarks": "Initial cutting operation",
  "createdBy": "Admin"
}
```

**Test Data 5 - Assembly:**
```json
{
  "processCode": "PROC005",
  "processName": "Roller Assembly",
  "processType": "Assembly",
  "category": "Assembly",
  "department": "Assembly",
  "description": "Final assembly of magnetic roller",
  "processDetails": "Clean components, apply adhesive, press fit, cure, balance",
  "machineType": "Assembly Station",
  "defaultMachineId": null,
  "defaultMachineName": null,
  "standardSetupTimeMin": 15,
  "standardCycleTimeMin": 60,
  "standardCycleTimePerPiece": 45,
  "skillLevel": "Intermediate",
  "operatorsRequired": 2,
  "hourlyRate": 600,
  "standardCostPerPiece": 450,
  "requiresQC": true,
  "qcCheckpoints": "Assembly integrity, alignment, balance test, magnetic field test",
  "isOutsourced": false,
  "preferredVendor": null,
  "remarks": "Final assembly step",
  "createdBy": "Admin"
}
```

---

### ✅ Get All Processes (GET)

**Endpoint:** `GET http://localhost:5217/api/processes`

---

### ✅ Get Process by ID (GET)

**Endpoint:** `GET http://localhost:5217/api/processes/{id}`

---

### ✅ Get Active Processes (GET)

**Endpoint:** `GET http://localhost:5217/api/processes/active`

---

### ✅ Get Processes by Type (GET)

**Endpoint:** `GET http://localhost:5217/api/processes/by-type/Machining`

---

### ✅ Get Processes by Department (GET)

**Endpoint:** `GET http://localhost:5217/api/processes/by-department/Machining`

---

### ✅ Get Processes by Machine Type (GET)

**Endpoint:** `GET http://localhost:5217/api/processes/by-machine-type/CNC Lathe`

---

### ✅ Get Outsourced Processes (GET)

**Endpoint:** `GET http://localhost:5217/api/processes/outsourced`

---

## 5️⃣ Order API Testing

### Base URL: `/api/orders`

### ✅ Create Order (POST)

**Endpoint:** `POST http://localhost:5217/api/orders`

**NOTE:** You must first create a customer and get its ID, then use that ID in the order.

**Test Data 1:**
```json
{
  "orderDate": "2026-01-19T10:00:00Z",
  "dueDate": "2026-02-28T00:00:00Z",
  "customerId": "paste-customer-id-here",
  "productId": "paste-product-id-here",
  "quantity": 50,
  "priority": "High",
  "orderValue": 125000,
  "advancePayment": 50000,
  "createdBy": "Admin"
}
```

---

### ✅ Get All Orders (GET)

**Endpoint:** `GET http://localhost:5217/api/orders`

---

### ✅ Get Order by ID (GET)

**Endpoint:** `GET http://localhost:5217/api/orders/{id}`

---

### ✅ Get Pending Drawing Review (GET)

**Endpoint:** `GET http://localhost:5217/api/orders/pending-drawing-review`

---

### ✅ Approve Drawing Review (POST)

**Endpoint:** `POST http://localhost:5217/api/orders/{id}/drawing-review/approve`

**Body:**
```json
{
  "orderId": "paste-order-id-here",
  "status": "Approved",
  "reviewedBy": "Engineer1",
  "notes": "Drawing approved, ready for production"
}
```

---

## 📝 Postman Collection Setup

### Method 1: Import via JSON

Create a file named `MultiHitech_ERP.postman_collection.json` with all endpoints, then import in Postman.

### Method 2: Manual Setup

1. **Create New Collection:**
   - Open Postman
   - Click "New" → "Collection"
   - Name it "MultiHitech ERP API"

2. **Add Environment Variables:**
   - Click "Environments" → "Create Environment"
   - Name: "MultiHitech Dev"
   - Add variable:
     - `base_url` = `http://localhost:5217`

3. **Create Folders:**
   - Right-click collection → "Add Folder"
   - Create folders: Customers, Materials, Machines, Processes, Orders

4. **Add Requests:**
   - Right-click folder → "Add Request"
   - Set method (GET/POST/PUT/DELETE)
   - Set URL: `{{base_url}}/api/customers`
   - Add body for POST/PUT requests

---

## 🧪 Testing Workflow

### Step 1: Create Master Data
```
1. Create 3 Customers → Save their IDs
2. Create 3 Materials → Save their IDs
3. Create 3 Machines → Save their IDs
4. Create 5 Processes → Save their IDs
```

### Step 2: Verify CRUD Operations
```
1. GET all customers → Should return 3
2. GET customer by ID → Should return one
3. UPDATE customer → Change email
4. GET updated customer → Verify changes
5. DEACTIVATE customer → Check status
6. GET active customers → Should return 2
7. ACTIVATE customer → Check status
```

### Step 3: Test Business Queries
```
1. Search customers by name
2. Get customers by city
3. Get materials by category
4. Get low stock materials
5. Get available machines
6. Get outsourced processes
```

### Step 4: Test Orders (After Product Creation)
```
1. Create order with valid customer ID
2. Get pending drawing review orders
3. Approve drawing review
4. Verify order ready for planning
```

---

## ⚠️ Common Errors & Solutions

### Error 1: 400 Bad Request - Validation Error
**Cause:** Missing required fields or invalid data format
**Solution:** Check all required fields in request body

### Error 2: 404 Not Found
**Cause:** Invalid ID or resource doesn't exist
**Solution:** Verify the ID exists using GET all endpoint first

### Error 3: 500 Internal Server Error
**Cause:** Database connection issue or server error
**Solution:** Check API logs in terminal, verify database is running

### Error 4: "Customer code already exists"
**Cause:** Duplicate customer code
**Solution:** Use a different customer code (CUST004, CUST005, etc.)

---

## 📊 Expected Test Results

After completing all tests, you should have:

- ✅ 3 Customers created
- ✅ 3 Materials created
- ✅ 3 Machines created
- ✅ 5 Processes created
- ✅ All GET endpoints returning data
- ✅ Search/filter endpoints working
- ✅ Update operations successful
- ✅ Activate/Deactivate working

---

## 🎯 Next Steps

1. **Export Postman Collection:**
   - Click "..." on collection → "Export"
   - Save as `MultiHitech_ERP_Collection.json`

2. **Share Collection:**
   - Use for team testing
   - Import in different environments

3. **Create Product Module:**
   - After testing master data
   - Products depend on materials and processes

---

## 9️⃣ Job Card API Testing (Planning Module)

### Base URL: `/api/jobcards`

### ✅ Create Job Card (POST)

**Endpoint:** `POST http://localhost:5217/api/jobcards`

**Headers:**
```
Content-Type: application/json
```

**Test Data 1 - Cutting Job Card (No Dependencies):**
```json
{
  "jobCardNo": "JC-2024-001",
  "creationType": "Manual",
  "orderId": "order-guid-from-orders-api",
  "orderNo": "ORD-001",
  "drawingId": "drawing-guid-from-drawings-api",
  "drawingNumber": "DWG-2024-001",
  "drawingRevision": "A",
  "processId": "process-guid-for-cutting",
  "processName": "Cutting",
  "stepNo": 1,
  "quantity": 10,
  "estimatedSetupTimeMin": 30,
  "estimatedCycleTimeMin": 120,
  "estimatedTotalTimeMin": 150,
  "materialStatus": "Pending",
  "priority": "High",
  "scheduleStatus": "Not Scheduled",
  "createdBy": "Planner",
  "prerequisiteJobCardIds": []
}
```

**Test Data 2 - CNC Turning Job Card (With Dependency):**
```json
{
  "jobCardNo": "JC-2024-002",
  "creationType": "Manual",
  "orderId": "order-guid-from-orders-api",
  "orderNo": "ORD-001",
  "drawingId": "drawing-guid-from-drawings-api",
  "drawingNumber": "DWG-2024-001",
  "drawingRevision": "A",
  "processId": "process-guid-for-cnc-turning",
  "processName": "CNC Turning",
  "stepNo": 2,
  "quantity": 10,
  "estimatedSetupTimeMin": 45,
  "estimatedCycleTimeMin": 180,
  "estimatedTotalTimeMin": 225,
  "materialStatus": "Pending",
  "priority": "High",
  "scheduleStatus": "Not Scheduled",
  "createdBy": "Planner",
  "prerequisiteJobCardIds": ["guid-of-JC-2024-001"]
}
```

**Expected Response:**
```json
{
  "success": true,
  "message": "Job card 'JC-2024-002' created successfully",
  "data": "new-job-card-guid",
  "errors": null
}
```

---

### 📋 Get All Job Cards (GET)

**Endpoint:** `GET http://localhost:5217/api/jobcards`

**Expected Response:**
```json
{
  "success": true,
  "message": null,
  "data": [
    {
      "id": "guid",
      "jobCardNo": "JC-2024-001",
      "status": "Pending",
      "quantity": 10,
      "completedQty": 0,
      "materialStatus": "Pending",
      "scheduleStatus": "Not Scheduled",
      ...
    }
  ],
  "errors": null
}
```

---

### 🔍 Get Job Cards by Order (GET)

**Endpoint:** `GET http://localhost:5217/api/jobcards/by-order/{orderId}`

**Example:** `GET http://localhost:5217/api/jobcards/by-order/order-guid-here`

---

### 🎯 Get Ready for Scheduling (GET)

**Endpoint:** `GET http://localhost:5217/api/jobcards/ready-for-scheduling`

**Returns:** All job cards with:
- Status = "Pending"
- MaterialStatus = "Available"
- No unresolved dependencies

---

### 🚫 Get Blocked Job Cards (GET)

**Endpoint:** `GET http://localhost:5217/api/jobcards/blocked`

**Returns:** Job cards that have unresolved dependencies

---

### ▶️ Start Job Card Execution (POST)

**Endpoint:** `POST http://localhost:5217/api/jobcards/{id}/start`

**Example:** `POST http://localhost:5217/api/jobcards/job-card-guid/start`

**Prerequisites:**
- Job card must have no unresolved dependencies
- MaterialStatus must be "Available" or "Issued"
- Status must be "Pending" or "Ready"

**Expected Response:**
```json
{
  "success": true,
  "message": "Job card execution started",
  "data": true,
  "errors": null
}
```

---

### ✅ Complete Job Card Execution (POST)

**Endpoint:** `POST http://localhost:5217/api/jobcards/{id}/complete`

**Example:** `POST http://localhost:5217/api/jobcards/job-card-guid/complete`

**What Happens:**
1. Job card status → "Completed"
2. Actual time calculated (end - start)
3. All dependencies where this is a prerequisite → auto-resolved
4. Dependent job cards → unblocked

**Expected Response:**
```json
{
  "success": true,
  "message": "Job card execution completed",
  "data": true,
  "errors": null
}
```

---

### 📊 Update Quantities (POST)

**Endpoint:** `POST http://localhost:5217/api/jobcards/{id}/update-quantities`

**Headers:**
```
Content-Type: application/json
```

**Body:**
```json
{
  "completedQty": 8,
  "rejectedQty": 1,
  "reworkQty": 1,
  "inProgressQty": 0
}
```

**Validation:** Total quantities cannot exceed job card quantity

---

### 🏭 Assign Machine (POST)

**Endpoint:** `POST http://localhost:5217/api/jobcards/{id}/assign-machine`

**Headers:**
```
Content-Type: application/json
```

**Body:**
```json
{
  "machineId": "machine-guid",
  "machineName": "CNC-01"
}
```

---

### 👷 Assign Operator (POST)

**Endpoint:** `POST http://localhost:5217/api/jobcards/{id}/assign-operator`

**Headers:**
```
Content-Type: application/json
```

**Body:**
```json
{
  "operatorId": "operator-guid",
  "operatorName": "Rajesh Kumar"
}
```

---

### 🔗 Get Dependencies (GET)

**Get Prerequisites (jobs this one depends on):**
```
GET http://localhost:5217/api/jobcards/{id}/prerequisites
```

**Get Dependents (jobs that depend on this one):**
```
GET http://localhost:5217/api/jobcards/{id}/dependents
```

---

### ➕ Add Dependency (POST)

**Endpoint:** `POST http://localhost:5217/api/jobcards/{dependentId}/add-dependency`

**Headers:**
```
Content-Type: application/json
```

**Body:**
```json
{
  "prerequisiteJobCardId": "prerequisite-job-card-guid"
}
```

**Validation:**
- Prevents circular dependencies
- Prerequisite must exist

---

### 📈 Update Job Card Status (POST)

**Endpoint:** `POST http://localhost:5217/api/jobcards/{id}/status`

**Body:**
```json
{
  "status": "Ready"
}
```

**Valid Statuses:**
- "Pending"
- "Ready"
- "Scheduled"
- "In Progress"
- "Completed"
- "Blocked"

---

### 📦 Update Material Status (POST)

**Endpoint:** `POST http://localhost:5217/api/jobcards/{id}/material-status`

**Body:**
```json
{
  "materialStatus": "Available"
}
```

**Valid Material Statuses:**
- "Pending"
- "Requested"
- "Available"
- "Issued"
- "Consumed"

---

### 📅 Update Schedule Status (POST)

**Endpoint:** `POST http://localhost:5217/api/jobcards/{id}/schedule-status`

**Body:**
```json
{
  "scheduleStatus": "Scheduled",
  "startDate": "2024-01-25T08:00:00Z",
  "endDate": "2024-01-25T11:30:00Z"
}
```

---

### 🎬 Job Card Testing Workflow

**Complete End-to-End Test:**

1. **Create Order** (from Orders API)
2. **Create Drawing** (from Drawings API)
3. **Create Processes** (Cutting, Turning, Grinding)
4. **Create Job Card 1 - Cutting** (no dependencies)
   ```
   POST /api/jobcards
   ```
5. **Create Job Card 2 - Turning** (depends on JC1)
   ```
   POST /api/jobcards
   prerequisiteJobCardIds: [JC1-guid]
   ```
6. **Create Job Card 3 - Grinding** (depends on JC2)
   ```
   POST /api/jobcards
   prerequisiteJobCardIds: [JC2-guid]
   ```
7. **Check Blocked Jobs**
   ```
   GET /api/jobcards/blocked
   → Should show JC2 and JC3 (blocked)
   ```
8. **Update Material Status for JC1**
   ```
   POST /api/jobcards/{jc1-id}/material-status
   { "materialStatus": "Available" }
   ```
9. **Check Ready for Scheduling**
   ```
   GET /api/jobcards/ready-for-scheduling
   → Should show JC1 only
   ```
10. **Start JC1**
    ```
    POST /api/jobcards/{jc1-id}/start
    ```
11. **Complete JC1**
    ```
    POST /api/jobcards/{jc1-id}/complete
    → JC2 should now be unblocked
    ```
12. **Verify JC2 is Unblocked**
    ```
    GET /api/jobcards/blocked
    → Should show only JC3 now
    ```
13. **Update Material for JC2 and Start/Complete**
14. **Verify JC3 is Unblocked**
15. **Continue workflow through JC3**

---

### ⚠️ Common Job Card Errors

**Error: Circular Dependency**
```json
{
  "success": false,
  "message": "Cannot create dependency - would create circular dependency with job card {guid}",
  "errors": ["CircularDependencyDetected"]
}
```

**Error: Unresolved Dependencies**
```json
{
  "success": false,
  "message": "Cannot start - job card has unresolved dependencies",
  "errors": ["UnresolvedDependencies"]
}
```

**Error: Material Not Available**
```json
{
  "success": false,
  "message": "Cannot start - material status is 'Pending'",
  "errors": ["MaterialNotAvailable"]
}
```

**Error: Version Mismatch (Concurrent Edit)**
```json
{
  "success": false,
  "message": "Job card has been modified by another user. Please refresh and try again.",
  "errors": ["OptimisticLockingFailure"]
}
```

**Error: Cannot Delete**
```json
{
  "success": false,
  "message": "Cannot delete job card - 2 other job cards depend on it",
  "errors": ["HasDependents"]
}
```

---

### 📊 Job Card Query Endpoints Summary

| Endpoint | Description |
|----------|-------------|
| `GET /api/jobcards` | All job cards |
| `GET /api/jobcards/{id}` | By ID |
| `GET /api/jobcards/by-job-card-no/{no}` | By job card number |
| `GET /api/jobcards/by-order/{orderId}` | All for an order |
| `GET /api/jobcards/by-process/{processId}` | All for a process |
| `GET /api/jobcards/by-status/{status}` | Filter by status |
| `GET /api/jobcards/by-machine/{machineId}` | Machine workload |
| `GET /api/jobcards/by-operator/{operatorId}` | Operator workload |
| `GET /api/jobcards/ready-for-scheduling` | Ready to schedule |
| `GET /api/jobcards/scheduled` | Scheduled jobs |
| `GET /api/jobcards/in-progress` | Currently running |
| `GET /api/jobcards/blocked` | Blocked by dependencies |

---

## 📞 Support

If you encounter issues:
1. Check API logs in terminal
2. Verify database is running
3. Check [BACKEND_PROGRESS.md](BACKEND_PROGRESS.md) for status
4. Review this guide for correct request format
5. For Job Card issues, check [JOB_CARD_MODULE_SUMMARY.md](JOB_CARD_MODULE_SUMMARY.md)

---

**Happy Testing! 🚀**
