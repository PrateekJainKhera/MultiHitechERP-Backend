# MultiHitech ERP - API Testing Guide (Postman)

**Last Updated:** 2026-01-22
**Base URL:** `http://localhost:5217`
**API Status:** ✅ Ready for Testing

---

## 📋 Table of Contents

1. [Setup Instructions](#setup-instructions)
2. [Master Data Modules](#master-data-modules)
   - [Customer Module](#1-customer-module)
   - [Product Module](#2-product-module)
   - [Material Module](#3-material-module)
   - [Machine Module](#4-machine-module)
   - [Process Module](#5-process-module)
   - [Operator Module](#6-operator-module)
   - [Drawing Module](#7-drawing-module)
   - [Supplier Module](#8-supplier-module)
3. [BOM & ChildPart Modules](#bom--childpart-modules)
4. [Order & Planning Modules](#order--planning-modules)
5. [Stores & Inventory Modules](#stores--inventory-modules)
6. [Production & Quality Modules](#production--quality-modules)
7. [Dispatch Module](#dispatch-module)

---

## Setup Instructions

### 1. Start the API
```bash
cd backend/MultiHitechERP.API
dotnet run
```

API will run on: `http://localhost:5217`

### 2. Access Swagger (Optional)
Open browser: `http://localhost:5217/swagger`

### 3. Postman Collection Setup
- Create new collection: "MultiHitech ERP API"
- Set base URL variable: `{{baseUrl}}` = `http://localhost:5217`
- Set Content-Type header: `application/json`

---

## Master Data Modules

## 1. Customer Module

**Base Path:** `/api/customers`

### 1.1 Create Customer (POST)
**Endpoint:** `POST {{baseUrl}}/api/customers`

**Request Body:**
```json
{
  "customerCode": "CUST001",
  "customerName": "ABC Industries",
  "contactPerson": "Rajesh Kumar",
  "contactNumber": "9876543210",
  "email": "rajesh@abcindustries.com",
  "addressLine1": "Plot No. 15, Industrial Area",
  "addressLine2": "Phase 2",
  "city": "Mumbai",
  "state": "Maharashtra",
  "country": "India",
  "postalCode": "400001",
  "gstNumber": "27AABCU1234A1Z5",
  "panNumber": "AABCU1234A",
  "creditLimit": 500000,
  "creditDays": 45,
  "paymentTerms": "Net 45",
  "isActive": true,
  "customerType": "Corporate",
  "industry": "Manufacturing",
  "createdBy": "Admin"
}
```

**Expected Response:** `200 OK`
```json
{
  "success": true,
  "message": "Customer created successfully",
  "data": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### 1.2 Get All Customers (GET)
**Endpoint:** `GET {{baseUrl}}/api/customers`

**Expected Response:** `200 OK`
```json
{
  "success": true,
  "message": "Success",
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "customerCode": "CUST001",
      "customerName": "ABC Industries",
      "contactPerson": "Rajesh Kumar",
      "contactNumber": "9876543210",
      "email": "rajesh@abcindustries.com",
      "city": "Mumbai",
      "state": "Maharashtra",
      "isActive": true,
      "createdAt": "2026-01-22T10:30:00Z"
    }
  ]
}
```

### 1.3 Get Customer by ID (GET)
**Endpoint:** `GET {{baseUrl}}/api/customers/{id}`

**Example:** `GET {{baseUrl}}/api/customers/3fa85f64-5717-4562-b3fc-2c963f66afa6`

### 1.4 Get Customer by Code (GET)
**Endpoint:** `GET {{baseUrl}}/api/customers/by-code/CUST001`

### 1.5 Search Customers (GET)
**Endpoint:** `GET {{baseUrl}}/api/customers/search?query=ABC`

### 1.6 Get Active Customers (GET)
**Endpoint:** `GET {{baseUrl}}/api/customers/active`

### 1.7 Update Customer (PUT)
**Endpoint:** `PUT {{baseUrl}}/api/customers/{id}`

**Request Body:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "customerCode": "CUST001",
  "customerName": "ABC Industries Pvt Ltd",
  "contactPerson": "Rajesh Kumar",
  "contactNumber": "9876543210",
  "email": "rajesh@abcindustries.com",
  "addressLine1": "Plot No. 15, Industrial Area",
  "city": "Mumbai",
  "state": "Maharashtra",
  "creditLimit": 600000,
  "creditDays": 60,
  "isActive": true,
  "updatedBy": "Admin"
}
```

### 1.8 Activate Customer (PATCH)
**Endpoint:** `PATCH {{baseUrl}}/api/customers/{id}/activate`

### 1.9 Deactivate Customer (PATCH)
**Endpoint:** `PATCH {{baseUrl}}/api/customers/{id}/deactivate`

### 1.10 Delete Customer (DELETE)
**Endpoint:** `DELETE {{baseUrl}}/api/customers/{id}`

---

## 2. Product Module

**Base Path:** `/api/products`

### 2.1 Create Product (POST)
**Endpoint:** `POST {{baseUrl}}/api/products`

**Request Body:**
```json
{
  "productCode": "PROD001",
  "productName": "Magnetic Roller 250mm",
  "productDescription": "250mm diameter magnetic roller for printing industry",
  "category": "Magnetic Rollers",
  "productType": "Finished Goods",
  "hsnCode": "84439190",
  "unitOfMeasure": "NOS",
  "standardCost": 15000,
  "sellingPrice": 25000,
  "weight": 45.5,
  "length": 250,
  "width": 250,
  "height": 1200,
  "specifications": "Diameter: 250mm, Length: 1200mm, Magnetic Strength: 3000 Gauss",
  "isActive": true,
  "leadTimeDays": 15,
  "createdBy": "Admin"
}
```

### 2.2 Get All Products (GET)
**Endpoint:** `GET {{baseUrl}}/api/products`

### 2.3 Get Product by ID (GET)
**Endpoint:** `GET {{baseUrl}}/api/products/{id}`

### 2.4 Get Product by Code (GET)
**Endpoint:** `GET {{baseUrl}}/api/products/by-code/PROD001`

### 2.5 Search Products (GET)
**Endpoint:** `GET {{baseUrl}}/api/products/search?query=Roller`

### 2.6 Get by Category (GET)
**Endpoint:** `GET {{baseUrl}}/api/products/by-category/Magnetic Rollers`

### 2.7 Get by Type (GET)
**Endpoint:** `GET {{baseUrl}}/api/products/by-type/Finished Goods`

### 2.8 Get Active Products (GET)
**Endpoint:** `GET {{baseUrl}}/api/products/active`

### 2.9 Update Product (PUT)
**Endpoint:** `PUT {{baseUrl}}/api/products/{id}`

**Request Body:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "productCode": "PROD001",
  "productName": "Magnetic Roller 250mm Premium",
  "productDescription": "Premium 250mm magnetic roller",
  "category": "Magnetic Rollers",
  "sellingPrice": 27000,
  "isActive": true,
  "updatedBy": "Admin"
}
```

### 2.10 Delete Product (DELETE)
**Endpoint:** `DELETE {{baseUrl}}/api/products/{id}`

---

## 3. Material Module

**Base Path:** `/api/materials`

### 3.1 Create Material (POST)
**Endpoint:** `POST {{baseUrl}}/api/materials`

**Request Body:**
```json
{
  "materialCode": "MAT001",
  "materialName": "EN8 Round Bar 50mm",
  "materialDescription": "EN8 Steel Round Bar, 50mm diameter",
  "category": "Raw Material",
  "materialType": "Steel",
  "grade": "EN8",
  "primaryUOM": "KG",
  "secondaryUOM": "MTR",
  "conversionFactor": 15.4,
  "standardCost": 85,
  "reorderLevel": 500,
  "minimumStock": 300,
  "maximumStock": 2000,
  "leadTimeDays": 7,
  "supplierName": "Steel Trading Co.",
  "specifications": "Diameter: 50mm, Grade: EN8, Hardness: 180-220 BHN",
  "hsnCode": "72151090",
  "isActive": true,
  "createdBy": "Admin"
}
```

### 3.2 Get All Materials (GET)
**Endpoint:** `GET {{baseUrl}}/api/materials`

### 3.3 Get Material by ID (GET)
**Endpoint:** `GET {{baseUrl}}/api/materials/{id}`

### 3.4 Get Material by Code (GET)
**Endpoint:** `GET {{baseUrl}}/api/materials/by-code/MAT001`

### 3.5 Get by Category (GET)
**Endpoint:** `GET {{baseUrl}}/api/materials/by-category/Raw Material`

### 3.6 Get by Grade (GET)
**Endpoint:** `GET {{baseUrl}}/api/materials/by-grade/EN8`

### 3.7 Get by Type (GET)
**Endpoint:** `GET {{baseUrl}}/api/materials/by-type/Steel`

### 3.8 Get Low Stock Materials (GET)
**Endpoint:** `GET {{baseUrl}}/api/materials/low-stock`

### 3.9 Get Active Materials (GET)
**Endpoint:** `GET {{baseUrl}}/api/materials/active`

### 3.10 Update Material (PUT)
**Endpoint:** `PUT {{baseUrl}}/api/materials/{id}`

**Request Body:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "materialCode": "MAT001",
  "materialName": "EN8 Round Bar 50mm",
  "category": "Raw Material",
  "standardCost": 90,
  "reorderLevel": 600,
  "isActive": true,
  "updatedBy": "Admin"
}
```

### 3.11 Delete Material (DELETE)
**Endpoint:** `DELETE {{baseUrl}}/api/materials/{id}`

---

## 4. Machine Module

**Base Path:** `/api/machines`

### 4.1 Create Machine (POST)
**Endpoint:** `POST {{baseUrl}}/api/machines`

**Request Body:**
```json
{
  "machineCode": "CNC-01",
  "machineName": "CNC Turning Center TC-500",
  "machineType": "CNC Lathe",
  "manufacturer": "DMG MORI",
  "model": "CTX 500",
  "serialNumber": "CTX500-2023-001",
  "yearOfManufacture": 2023,
  "department": "Turning",
  "location": "Shop Floor - Zone A",
  "capacity": "Max Dia: 500mm, Max Length: 1500mm",
  "specifications": "3-Jaw Chuck, Live Tooling, 12-Station Turret",
  "maintenanceSchedule": "Monthly",
  "lastMaintenanceDate": "2026-01-15",
  "nextMaintenanceDate": "2026-02-15",
  "status": "Available",
  "isActive": true,
  "operatingCostPerHour": 450,
  "powerRating": "25 KW",
  "createdBy": "Admin"
}
```

### 4.2 Get All Machines (GET)
**Endpoint:** `GET {{baseUrl}}/api/machines`

### 4.3 Get Machine by ID (GET)
**Endpoint:** `GET {{baseUrl}}/api/machines/{id}`

### 4.4 Get Machine by Code (GET)
**Endpoint:** `GET {{baseUrl}}/api/machines/by-code/CNC-01`

### 4.5 Get by Type (GET)
**Endpoint:** `GET {{baseUrl}}/api/machines/by-type/CNC Lathe`

### 4.6 Get by Department (GET)
**Endpoint:** `GET {{baseUrl}}/api/machines/by-department/Turning`

### 4.7 Get Available Machines (GET)
**Endpoint:** `GET {{baseUrl}}/api/machines/available`

### 4.8 Get Active Machines (GET)
**Endpoint:** `GET {{baseUrl}}/api/machines/active`

### 4.9 Update Machine Status (PATCH)
**Endpoint:** `PATCH {{baseUrl}}/api/machines/{id}/status`

**Request Body:**
```json
{
  "status": "Under Maintenance"
}
```

### 4.10 Assign Machine to Job (POST)
**Endpoint:** `POST {{baseUrl}}/api/machines/{id}/assign`

**Request Body:**
```json
{
  "jobCardId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### 4.11 Release Machine (POST)
**Endpoint:** `POST {{baseUrl}}/api/machines/{id}/release`

### 4.12 Update Machine (PUT)
**Endpoint:** `PUT {{baseUrl}}/api/machines/{id}`

### 4.13 Delete Machine (DELETE)
**Endpoint:** `DELETE {{baseUrl}}/api/machines/{id}`

---

## 5. Process Module

**Base Path:** `/api/processes`

### 5.1 Create Process (POST)
**Endpoint:** `POST {{baseUrl}}/api/processes`

**Request Body:**
```json
{
  "processCode": "TURN-01",
  "processName": "CNC Turning",
  "processDescription": "Precision turning operation on CNC lathe",
  "processType": "Machining",
  "department": "Turning",
  "standardTime": 120,
  "setupTime": 30,
  "costPerHour": 500,
  "isOutsourced": false,
  "isActive": true,
  "qualityCheckRequired": true,
  "skillRequired": "CNC Operator Level 2",
  "machineType": "CNC Lathe",
  "remarks": "Requires coolant and cutting tools",
  "createdBy": "Admin"
}
```

### 5.2 Get All Processes (GET)
**Endpoint:** `GET {{baseUrl}}/api/processes`

### 5.3 Get Process by ID (GET)
**Endpoint:** `GET {{baseUrl}}/api/processes/{id}`

### 5.4 Get Process by Code (GET)
**Endpoint:** `GET {{baseUrl}}/api/processes/by-code/TURN-01`

### 5.5 Get by Type (GET)
**Endpoint:** `GET {{baseUrl}}/api/processes/by-type/Machining`

### 5.6 Get by Department (GET)
**Endpoint:** `GET {{baseUrl}}/api/processes/by-department/Turning`

### 5.7 Get Outsourced Processes (GET)
**Endpoint:** `GET {{baseUrl}}/api/processes/outsourced`

### 5.8 Get Active Processes (GET)
**Endpoint:** `GET {{baseUrl}}/api/processes/active`

### 5.9 Update Process (PUT)
**Endpoint:** `PUT {{baseUrl}}/api/processes/{id}`

**Request Body:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "processCode": "TURN-01",
  "processName": "CNC Turning Operations",
  "standardTime": 130,
  "costPerHour": 550,
  "isActive": true,
  "updatedBy": "Admin"
}
```

### 5.10 Delete Process (DELETE)
**Endpoint:** `DELETE {{baseUrl}}/api/processes/{id}`

---

## 6. Operator Module

**Base Path:** `/api/operators`

### 6.1 Create Operator (POST)
**Endpoint:** `POST {{baseUrl}}/api/operators`

**Request Body:**
```json
{
  "employeeCode": "EMP001",
  "operatorName": "Suresh Patil",
  "contactNumber": "9876543210",
  "email": "suresh.patil@company.com",
  "department": "Turning",
  "shift": "Day",
  "skillLevel": "Level 2",
  "skillSet": "CNC Turning, Manual Lathe",
  "experienceYears": 8,
  "hourlyRate": 250,
  "joiningDate": "2018-03-15",
  "status": "Available",
  "isActive": true,
  "certifications": "CNC Programming, ISO 9001",
  "createdBy": "Admin"
}
```

### 6.2 Get All Operators (GET)
**Endpoint:** `GET {{baseUrl}}/api/operators`

### 6.3 Get Operator by ID (GET)
**Endpoint:** `GET {{baseUrl}}/api/operators/{id}`

### 6.4 Get Operator by Code (GET)
**Endpoint:** `GET {{baseUrl}}/api/operators/by-code/EMP001`

### 6.5 Get by Department (GET)
**Endpoint:** `GET {{baseUrl}}/api/operators/by-department/Turning`

### 6.6 Get by Shift (GET)
**Endpoint:** `GET {{baseUrl}}/api/operators/by-shift/Day`

### 6.7 Get by Skill Level (GET)
**Endpoint:** `GET {{baseUrl}}/api/operators/by-skill/Level 2`

### 6.8 Get Available Operators (GET)
**Endpoint:** `GET {{baseUrl}}/api/operators/available`

### 6.9 Get Active Operators (GET)
**Endpoint:** `GET {{baseUrl}}/api/operators/active`

### 6.10 Update Operator Status (PATCH)
**Endpoint:** `PATCH {{baseUrl}}/api/operators/{id}/status`

**Request Body:**
```json
{
  "status": "On Leave"
}
```

### 6.11 Assign Operator to Job (POST)
**Endpoint:** `POST {{baseUrl}}/api/operators/{id}/assign`

**Request Body:**
```json
{
  "jobCardId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### 6.12 Release Operator (POST)
**Endpoint:** `POST {{baseUrl}}/api/operators/{id}/release`

### 6.13 Update Operator (PUT)
**Endpoint:** `PUT {{baseUrl}}/api/operators/{id}`

### 6.14 Delete Operator (DELETE)
**Endpoint:** `DELETE {{baseUrl}}/api/operators/{id}`

---

## 7. Drawing Module

**Base Path:** `/api/drawings`

### 7.1 Create Drawing (POST)
**Endpoint:** `POST {{baseUrl}}/api/drawings`

**Request Body:**
```json
{
  "drawingNumber": "DRG-2026-001",
  "drawingTitle": "Magnetic Roller Assembly",
  "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "revisionNumber": "A",
  "drawingType": "Assembly",
  "preparedBy": "Design Team",
  "approvedBy": "Chief Engineer",
  "drawingDate": "2026-01-20",
  "approvalDate": "2026-01-22",
  "filePath": "/drawings/DRG-2026-001-A.pdf",
  "fileFormat": "PDF",
  "remarks": "Initial release for production",
  "status": "Approved",
  "isActive": true,
  "createdBy": "Admin"
}
```

### 7.2 Get All Drawings (GET)
**Endpoint:** `GET {{baseUrl}}/api/drawings`

### 7.3 Get Drawing by ID (GET)
**Endpoint:** `GET {{baseUrl}}/api/drawings/{id}`

### 7.4 Get Drawing by Number (GET)
**Endpoint:** `GET {{baseUrl}}/api/drawings/by-number/DRG-2026-001`

### 7.5 Get by Product (GET)
**Endpoint:** `GET {{baseUrl}}/api/drawings/by-product/{productId}`

### 7.6 Get by Type (GET)
**Endpoint:** `GET {{baseUrl}}/api/drawings/by-type/Assembly`

### 7.7 Get Pending Approval (GET)
**Endpoint:** `GET {{baseUrl}}/api/drawings/pending-approval`

### 7.8 Get Active Drawings (GET)
**Endpoint:** `GET {{baseUrl}}/api/drawings/active`

### 7.9 Create New Revision (POST)
**Endpoint:** `POST {{baseUrl}}/api/drawings/{id}/new-revision`

**Request Body:**
```json
{
  "revisionNumber": "B",
  "remarks": "Updated dimensions as per customer feedback",
  "createdBy": "Design Team"
}
```

### 7.10 Update Drawing (PUT)
**Endpoint:** `PUT {{baseUrl}}/api/drawings/{id}`

### 7.11 Delete Drawing (DELETE)
**Endpoint:** `DELETE {{baseUrl}}/api/drawings/{id}`

---

## 8. Supplier Module

**Base Path:** `/api/supplier`

### 8.1 Create Supplier (POST)
**Endpoint:** `POST {{baseUrl}}/api/supplier`

**Request Body:**
```json
{
  "supplierCode": "SUP001",
  "supplierName": "Heat Treatment Services Pvt Ltd",
  "supplierType": "Outsourcing",
  "category": "Heat Treatment",
  "contactPerson": "Amit Shah",
  "contactNumber": "9876543210",
  "email": "amit@heattreatment.com",
  "addressLine1": "Plot 25, MIDC Industrial Area",
  "addressLine2": "Bhosari",
  "city": "Pune",
  "state": "Maharashtra",
  "country": "India",
  "postalCode": "411026",
  "gstNumber": "27AABCH1234B1Z5",
  "paymentTerms": "Net 30",
  "creditDays": 30,
  "processCapabilities": "Hardening, Tempering, Case Hardening, Annealing",
  "standardLeadTimeDays": 5,
  "remarks": "ISO 9001:2015 Certified",
  "createdBy": "Admin"
}
```

### 8.2 Get All Suppliers (GET)
**Endpoint:** `GET {{baseUrl}}/api/supplier`

### 8.3 Get Supplier by ID (GET)
**Endpoint:** `GET {{baseUrl}}/api/supplier/{id}`

### 8.4 Get Supplier by Code (GET)
**Endpoint:** `GET {{baseUrl}}/api/supplier/by-code/SUP001`

### 8.5 Get by Type (GET)
**Endpoint:** `GET {{baseUrl}}/api/supplier/by-type/Outsourcing`

### 8.6 Get by Category (GET)
**Endpoint:** `GET {{baseUrl}}/api/supplier/by-category/Heat Treatment`

### 8.7 Get by Process Capability (GET)
**Endpoint:** `GET {{baseUrl}}/api/supplier/by-process-capability/Hardening`

### 8.8 Get Active Suppliers (GET)
**Endpoint:** `GET {{baseUrl}}/api/supplier/active`

### 8.9 Get Approved Suppliers (GET)
**Endpoint:** `GET {{baseUrl}}/api/supplier/approved`

### 8.10 Get by Status (GET)
**Endpoint:** `GET {{baseUrl}}/api/supplier/by-status/Active`

### 8.11 Get by Approval Status (GET)
**Endpoint:** `GET {{baseUrl}}/api/supplier/by-approval-status/Approved`

### 8.12 Get Top Performing Suppliers (GET)
**Endpoint:** `GET {{baseUrl}}/api/supplier/top-performing?count=10`

### 8.13 Get Low Performing Suppliers (GET)
**Endpoint:** `GET {{baseUrl}}/api/supplier/low-performing?count=10`

### 8.14 Approve Supplier (POST)
**Endpoint:** `POST {{baseUrl}}/api/supplier/{id}/approve`

**Request Body:**
```json
{
  "approvedBy": "Purchase Manager"
}
```

### 8.15 Reject Supplier (POST)
**Endpoint:** `POST {{baseUrl}}/api/supplier/{id}/reject`

**Request Body:**
```json
{
  "reason": "Quality standards not met"
}
```

### 8.16 Update Status (PATCH)
**Endpoint:** `PATCH {{baseUrl}}/api/supplier/{id}/status`

**Request Body:**
```json
{
  "status": "Blacklisted"
}
```

### 8.17 Update Performance Metrics (PUT)
**Endpoint:** `PUT {{baseUrl}}/api/supplier/{id}/performance`

**Request Body:**
```json
{
  "onTimeDeliveryRate": 95.5,
  "qualityRating": 4.5,
  "totalOrders": 50,
  "rejectedOrders": 2
}
```

### 8.18 Update Supplier (PUT)
**Endpoint:** `PUT {{baseUrl}}/api/supplier/{id}`

**Request Body:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "supplierCode": "SUP001",
  "supplierName": "Heat Treatment Services Pvt Ltd",
  "supplierType": "Outsourcing",
  "category": "Heat Treatment",
  "contactPerson": "Amit Shah",
  "contactNumber": "9876543210",
  "email": "amit@heattreatment.com",
  "addressLine1": "Plot 25, MIDC Industrial Area",
  "city": "Pune",
  "state": "Maharashtra",
  "gstNumber": "27AABCH1234B1Z5",
  "paymentTerms": "Net 45",
  "creditDays": 45,
  "processCapabilities": "Hardening, Tempering, Case Hardening, Annealing, Nitriding",
  "isActive": true,
  "remarks": "ISO 9001:2015 & AS9100 Certified",
  "updatedBy": "Admin"
}
```

### 8.19 Delete Supplier (DELETE)
**Endpoint:** `DELETE {{baseUrl}}/api/supplier/{id}`

---

## BOM & ChildPart Modules

## 9. ChildPart Module

**Base Path:** `/api/childparts`

### 9.1 Create ChildPart (POST)
**Endpoint:** `POST {{baseUrl}}/api/childparts`

**Request Body:**
```json
{
  "childPartCode": "CP-SHAFT-001",
  "childPartName": "Main Shaft Assembly",
  "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "materialId": "material-guid-here",
  "drawingId": "drawing-guid-here",
  "partCategory": "Machined Component",
  "partType": "Shaft",
  "makeOrBuy": "Make",
  "description": "Main shaft for magnetic roller",
  "quantity": 1,
  "unitOfMeasure": "NOS",
  "standardCost": 5000,
  "leadTimeDays": 7,
  "processTemplateId": null,
  "remarks": "Critical component - requires heat treatment",
  "isActive": true,
  "createdBy": "Admin"
}
```

### 9.2 Get All ChildParts (GET)
**Endpoint:** `GET {{baseUrl}}/api/childparts`

### 9.3 Get ChildPart by ID (GET)
**Endpoint:** `GET {{baseUrl}}/api/childparts/{id}`

### 9.4 Get ChildPart by Code (GET)
**Endpoint:** `GET {{baseUrl}}/api/childparts/by-code/CP-SHAFT-001`

### 9.5 Get by Product (GET)
**Endpoint:** `GET {{baseUrl}}/api/childparts/by-product/{productId}`

### 9.6 Get by Product Code (GET)
**Endpoint:** `GET {{baseUrl}}/api/childparts/by-product-code/PROD001`

### 9.7 Get by Material (GET)
**Endpoint:** `GET {{baseUrl}}/api/childparts/by-material/{materialId}`

### 9.8 Get by Part Type (GET)
**Endpoint:** `GET {{baseUrl}}/api/childparts/by-part-type/Shaft`

### 9.9 Get by Category (GET)
**Endpoint:** `GET {{baseUrl}}/api/childparts/by-category/Machined Component`

### 9.10 Get by Drawing (GET)
**Endpoint:** `GET {{baseUrl}}/api/childparts/by-drawing/{drawingId}`

### 9.11 Get by Make or Buy (GET)
**Endpoint:** `GET {{baseUrl}}/api/childparts/by-make-or-buy/Make`

### 9.12 Get Active ChildParts (GET)
**Endpoint:** `GET {{baseUrl}}/api/childparts/active`

### 9.13 Update Status (PATCH)
**Endpoint:** `PATCH {{baseUrl}}/api/childparts/{id}/status`

**Request Body:**
```json
{
  "status": "Obsolete"
}
```

### 9.14 Update ChildPart (PUT)
**Endpoint:** `PUT {{baseUrl}}/api/childparts/{id}`

### 9.15 Delete ChildPart (DELETE)
**Endpoint:** `DELETE {{baseUrl}}/api/childparts/{id}`

---

## 10. BOM Module

**Base Path:** `/api/bom`

### 10.1 Create BOM (POST)
**Endpoint:** `POST {{baseUrl}}/api/bom`

**Request Body:**
```json
{
  "bomNumber": "BOM-PROD001-A",
  "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "revisionNumber": "A",
  "bomType": "Manufacturing",
  "status": "Draft",
  "effectiveDate": "2026-02-01",
  "remarks": "Initial BOM for Magnetic Roller 250mm",
  "createdBy": "Planning Engineer"
}
```

### 10.2 Add BOM Item - Material (POST)
**Endpoint:** `POST {{baseUrl}}/api/bom/{bomId}/items`

**Request Body:**
```json
{
  "itemType": "Material",
  "materialId": "material-guid-here",
  "childPartId": null,
  "quantity": 25.5,
  "unitOfMeasure": "KG",
  "scrapPercentage": 5,
  "wastagePercentage": 2,
  "remarks": "EN8 Steel for shaft",
  "createdBy": "Planning Engineer"
}
```

### 10.3 Add BOM Item - ChildPart (POST)
**Endpoint:** `POST {{baseUrl}}/api/bom/{bomId}/items`

**Request Body:**
```json
{
  "itemType": "ChildPart",
  "materialId": null,
  "childPartId": "childpart-guid-here",
  "quantity": 1,
  "unitOfMeasure": "NOS",
  "scrapPercentage": 0,
  "wastagePercentage": 0,
  "remarks": "Main shaft assembly",
  "createdBy": "Planning Engineer"
}
```

### 10.4 Get All BOMs (GET)
**Endpoint:** `GET {{baseUrl}}/api/bom`

### 10.5 Get BOM by ID (GET)
**Endpoint:** `GET {{baseUrl}}/api/bom/{id}`

### 10.6 Get BOM by Number (GET)
**Endpoint:** `GET {{baseUrl}}/api/bom/by-number/BOM-PROD001-A`

### 10.7 Get by Product (GET)
**Endpoint:** `GET {{baseUrl}}/api/bom/by-product/{productId}`

### 10.8 Get by Product Code (GET)
**Endpoint:** `GET {{baseUrl}}/api/bom/by-product-code/PROD001`

### 10.9 Get Latest Revision (GET)
**Endpoint:** `GET {{baseUrl}}/api/bom/by-product/{productId}/latest`

### 10.10 Get Active BOMs (GET)
**Endpoint:** `GET {{baseUrl}}/api/bom/active`

### 10.11 Get by Status (GET)
**Endpoint:** `GET {{baseUrl}}/api/bom/by-status/Approved`

### 10.12 Get by Type (GET)
**Endpoint:** `GET {{baseUrl}}/api/bom/by-type/Manufacturing`

### 10.13 Get BOM Items (GET)
**Endpoint:** `GET {{baseUrl}}/api/bom/{bomId}/items`

### 10.14 Get Material Items Only (GET)
**Endpoint:** `GET {{baseUrl}}/api/bom/{bomId}/items/materials`

### 10.15 Get ChildPart Items Only (GET)
**Endpoint:** `GET {{baseUrl}}/api/bom/{bomId}/items/childparts`

### 10.16 Approve BOM (POST)
**Endpoint:** `POST {{baseUrl}}/api/bom/{id}/approve`

**Request Body:**
```json
{
  "approvedBy": "Production Manager"
}
```

### 10.17 Create New Revision (POST)
**Endpoint:** `POST {{baseUrl}}/api/bom/{id}/revision`

**Request Body:**
```json
{
  "revisionNumber": "B",
  "remarks": "Updated material quantities",
  "createdBy": "Planning Engineer"
}
```

### 10.18 Update BOM Item (PUT)
**Endpoint:** `PUT {{baseUrl}}/api/bom/{bomId}/items/{itemId}`

**Request Body:**
```json
{
  "id": "item-guid-here",
  "bomId": "bom-guid-here",
  "itemType": "Material",
  "materialId": "material-guid-here",
  "quantity": 28.0,
  "scrapPercentage": 4,
  "remarks": "Quantity revised",
  "updatedBy": "Planning Engineer"
}
```

### 10.19 Delete BOM Item (DELETE)
**Endpoint:** `DELETE {{baseUrl}}/api/bom/{bomId}/items/{itemId}`

### 10.20 Update BOM (PUT)
**Endpoint:** `PUT {{baseUrl}}/api/bom/{id}`

### 10.21 Delete BOM (DELETE)
**Endpoint:** `DELETE {{baseUrl}}/api/bom/{id}`

---

## Order & Planning Modules

## 11. Order Module

**Base Path:** `/api/orders`

### 11.1 Create Order (POST)
**Endpoint:** `POST {{baseUrl}}/api/orders`

**Request Body:**
```json
{
  "orderNumber": "ORD-2026-001",
  "customerId": "customer-guid-here",
  "productId": "product-guid-here",
  "orderDate": "2026-01-22",
  "requiredDeliveryDate": "2026-02-28",
  "quantity": 10,
  "unitPrice": 25000,
  "totalAmount": 250000,
  "priority": "High",
  "specialInstructions": "Customer requires custom packaging",
  "salesOrderRef": "SO-2026-ABC-001",
  "createdBy": "Sales Team"
}
```

### 11.2 Get All Orders (GET)
**Endpoint:** `GET {{baseUrl}}/api/orders`

### 11.3 Get Order by ID (GET)
**Endpoint:** `GET {{baseUrl}}/api/orders/{id}`

### 11.4 Get Order by Number (GET)
**Endpoint:** `GET {{baseUrl}}/api/orders/by-number/ORD-2026-001`

### 11.5 Get by Customer (GET)
**Endpoint:** `GET {{baseUrl}}/api/orders/by-customer/{customerId}`

### 11.6 Get by Product (GET)
**Endpoint:** `GET {{baseUrl}}/api/orders/by-product/{productId}`

### 11.7 Get by Status (GET)
**Endpoint:** `GET {{baseUrl}}/api/orders/by-status/InProduction`

### 11.8 Get Pending Drawing Review (GET)
**Endpoint:** `GET {{baseUrl}}/api/orders/pending-drawing-review`

### 11.9 Get Ready for Planning (GET)
**Endpoint:** `GET {{baseUrl}}/api/orders/ready-for-planning`

### 11.10 Approve Drawing (PATCH)
**Endpoint:** `PATCH {{baseUrl}}/api/orders/{id}/drawing-review`

**Request Body:**
```json
{
  "reviewStatus": "Approved",
  "reviewedBy": "Design Head",
  "reviewComments": "Drawing approved for production"
}
```

### 11.11 Update Order (PUT)
**Endpoint:** `PUT {{baseUrl}}/api/orders/{id}`

### 11.12 Delete Order (DELETE)
**Endpoint:** `DELETE {{baseUrl}}/api/orders/{id}`

---

## Stores & Inventory Modules

## 12. Inventory Module

**Base Path:** `/api/inventory`

### 12.1 Stock In (GRN) (POST)
**Endpoint:** `POST {{baseUrl}}/api/inventory/stock-in`

**Request Body:**
```json
{
  "materialId": "material-guid-here",
  "quantity": 500,
  "grnNo": "GRN-2026-001",
  "supplierId": "supplier-guid-here",
  "unitCost": 85,
  "performedBy": "Store Keeper",
  "remarks": "Purchase Order PO-2026-001"
}
```

### 12.2 Stock Out (Material Issue) (POST)
**Endpoint:** `POST {{baseUrl}}/api/inventory/stock-out`

**Request Body:**
```json
{
  "materialId": "material-guid-here",
  "quantity": 25.5,
  "jobCardId": "jobcard-guid-here",
  "requisitionId": "requisition-guid-here",
  "performedBy": "Store Keeper",
  "remarks": "Issued for Job Card JC-2026-001"
}
```

### 12.3 Stock Adjustment (POST)
**Endpoint:** `POST {{baseUrl}}/api/inventory/adjustment`

**Request Body:**
```json
{
  "materialId": "material-guid-here",
  "quantity": -5.2,
  "remarks": "Damaged material - scrap",
  "performedBy": "Store Keeper"
}
```

### 12.4 Stock Reconciliation (POST)
**Endpoint:** `POST {{baseUrl}}/api/inventory/reconcile`

**Request Body:**
```json
{
  "materialId": "material-guid-here",
  "actualQuantity": 485.5,
  "performedBy": "Store Manager",
  "remarks": "Monthly physical stock verification"
}
```

### 12.5 Update Stock Levels (PUT)
**Endpoint:** `PUT {{baseUrl}}/api/inventory/stock-levels`

**Request Body:**
```json
{
  "materialId": "material-guid-here",
  "minimumStock": 300,
  "maximumStock": 2000,
  "reorderLevel": 500,
  "reorderQuantity": 1000
}
```

### 12.6 Get All Inventory (GET)
**Endpoint:** `GET {{baseUrl}}/api/inventory`

### 12.7 Get Inventory by ID (GET)
**Endpoint:** `GET {{baseUrl}}/api/inventory/{id}`

### 12.8 Get by Material (GET)
**Endpoint:** `GET {{baseUrl}}/api/inventory/by-material/{materialId}`

### 12.9 Get Low Stock Items (GET)
**Endpoint:** `GET {{baseUrl}}/api/inventory/low-stock`

### 12.10 Get Out of Stock Items (GET)
**Endpoint:** `GET {{baseUrl}}/api/inventory/out-of-stock`

### 12.11 Get by Category (GET)
**Endpoint:** `GET {{baseUrl}}/api/inventory/by-category/Raw Material`

### 12.12 Get by Location (GET)
**Endpoint:** `GET {{baseUrl}}/api/inventory/by-location/Main Warehouse`

### 12.13 Get Active Inventory (GET)
**Endpoint:** `GET {{baseUrl}}/api/inventory/active`

### 12.14 Get Transactions by Material (GET)
**Endpoint:** `GET {{baseUrl}}/api/inventory/transactions/by-material/{materialId}`

### 12.15 Get Transactions by Type (GET)
**Endpoint:** `GET {{baseUrl}}/api/inventory/transactions/by-type/StockIn`

### 12.16 Get Transactions by Date Range (GET)
**Endpoint:** `GET {{baseUrl}}/api/inventory/transactions/by-date-range?startDate=2026-01-01&endDate=2026-01-31`

### 12.17 Get Recent Transactions (GET)
**Endpoint:** `GET {{baseUrl}}/api/inventory/transactions/recent?count=50`

---

## Production & Quality Modules

_(Continue with JobCard, Production, Quality, and Dispatch modules similarly...)_

---

## 🧪 Testing Sequence (Recommended Order)

### Phase 1: Master Data Setup (Do First!)
1. **Customer** - Create 2-3 test customers
2. **Product** - Create 2-3 test products
3. **Material** - Create 5-10 materials (different categories)
4. **Machine** - Create 5-10 machines (different types)
5. **Process** - Create 5-10 processes
6. **Operator** - Create 5-10 operators
7. **Drawing** - Create drawings for products
8. **Supplier** - Create 2-3 suppliers

### Phase 2: BOM & ChildParts
9. **ChildPart** - Create child parts for products
10. **BOM** - Create BOMs linking products to materials/childparts

### Phase 3: Order Flow
11. **Order** - Create order (will be in Drawing Review)
12. **Approve Drawing** - Move to Ready for Planning

### Phase 4: Planning
13. **JobCard** - Create job cards for order

### Phase 5: Stores
14. **Inventory Stock-In** - Receive materials
15. **Material Requisition** - Create requisition for job card
16. **Allocate Materials** - Allocate to requisition
17. **Issue Materials** - Physical issue to production

### Phase 6: Production
18. **Start Production** - Begin job card execution
19. **Update Quantities** - Record completed/rejected quantities
20. **Complete Production** - Close job card

### Phase 7: Quality & Dispatch
21. **Quality Inspection** - Record QC results
22. **Delivery Challan** - Create dispatch challan
23. **Mark Delivered** - Complete order

---

## 📝 Notes

- All GUIDs in examples are placeholders - use actual GUIDs from your created records
- Response format is consistent: `{ success, message, data }`
- Error responses return 400/404 with error message in `message` field
- All timestamps are in UTC format
- Use Swagger UI at `http://localhost:5217/swagger` for interactive testing

---

## ✅ Success Criteria

After testing all master modules, you should have:
- ✅ At least 3 customers
- ✅ At least 3 products
- ✅ At least 10 materials
- ✅ At least 5 machines
- ✅ At least 5 processes
- ✅ At least 5 operators
- ✅ At least 3 drawings
- ✅ At least 2 suppliers
- ✅ Database populated with test data
- ✅ Ready to test Order → Production → Dispatch flow

---

**Good luck with testing! 🚀**
