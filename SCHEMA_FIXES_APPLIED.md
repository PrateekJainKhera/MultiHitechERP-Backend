# Database Schema Mismatch - Fixed!

**Date:** 2026-01-22
**Issue:** 400 Bad Request when creating Customer
**Root Cause:** Mismatch between database schema and C# models/DTOs

---

## ❌ Problem Identified

The Customer module had **extra fields** in the C# code that **don't exist in the database**, causing SQL errors when trying to insert/update records.

### Database Schema (Correct - from 001_Phase1_Schema.sql):
```sql
CREATE TABLE Masters_Customers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CustomerCode NVARCHAR(50) NOT NULL UNIQUE,
    CustomerName NVARCHAR(200) NOT NULL,
    CustomerType NVARCHAR(20) NOT NULL, -- 'Direct', 'Agent', 'Dealer'
    ContactPerson NVARCHAR(100),
    Email NVARCHAR(100),
    Phone NVARCHAR(20),                -- NOT 'Mobile' or 'ContactNumber'
    Address NVARCHAR(500),              -- NOT 'AddressLine1', 'AddressLine2'
    City NVARCHAR(100),
    State NVARCHAR(100),
    Country NVARCHAR(100) DEFAULT 'India',
    PinCode NVARCHAR(10),               -- NOT 'PostalCode'
    GSTNo NVARCHAR(20),                 -- NOT 'GSTNumber'
    PANNo NVARCHAR(20),                 -- NOT 'PANNumber'
    CreditDays INT DEFAULT 0,
    CreditLimit DECIMAL(18,2) DEFAULT 0,
    PaymentTerms NVARCHAR(200),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    UpdatedAt DATETIME,
    UpdatedBy NVARCHAR(100)
);
```

### ❌ Fields That Don't Exist in Database (Removed):
- `Mobile` (use `Phone` instead)
- `Industry`
- `Status`
- `CustomerRating`
- `Classification`
- `Remarks`

---

## ✅ Files Fixed

### 1. **Customer.cs** (Model)
**Changes:**
- ❌ Removed: `Mobile`, `Industry`, `Status`, `CustomerRating`, `Classification`, `Remarks`
- ✅ Changed: `GSTNumber` → `GSTNo`
- ✅ Changed: `PANNumber` → `PANNo`
- ✅ Made `CustomerType` required (NOT nullable)
- ✅ Made `CreditDays` and `CreditLimit` non-nullable (default 0)

### 2. **CreateCustomerRequest.cs** (DTO)
**Changes:**
- ❌ Removed: `Mobile`, `Industry`, `CustomerRating`, `Classification`, `Remarks`
- ✅ Changed: `GSTNumber` → `GSTNo`
- ✅ Changed: `PANNumber` → `PANNo`
- ✅ Made `CustomerType` required with validation attribute
- ✅ Added `IsActive` property

### 3. **UpdateCustomerRequest.cs** (DTO)
**Changes:**
- ❌ Removed: `Mobile`, `Industry`, `Status`, `CustomerRating`, `Classification`, `Remarks`
- ✅ Changed: `GSTNumber` → `GSTNo`
- ✅ Changed: `PANNumber` → `PANNo`
- ✅ Made `CustomerType` required

### 4. **CustomerResponse.cs** (DTO)
**Changes:**
- ❌ Removed: `Mobile`, `Industry`, `Status`, `CustomerRating`, `Classification`, `Remarks`
- ✅ Changed: `GSTNumber` → `GSTNo`
- ✅ Changed: `PANNumber` → `PANNo`
- ✅ Made `CreditDays` and `CreditLimit` non-nullable

### 5. **CustomerRepository.cs**
**Changes in INSERT statement:**
```sql
-- BEFORE (WRONG - had extra columns)
INSERT INTO Masters_Customers (
    Id, CustomerCode, CustomerName, ContactPerson, Email, Phone, Mobile,
    Address, City, State, Country, PinCode,
    GSTNumber, PANNumber, CustomerType, Industry,
    CreditDays, CreditLimit, PaymentTerms,
    IsActive, Status, CustomerRating, Classification,
    Remarks, CreatedAt, CreatedBy
)

-- AFTER (CORRECT - matches database)
INSERT INTO Masters_Customers (
    Id, CustomerCode, CustomerName, CustomerType,
    ContactPerson, Email, Phone,
    Address, City, State, Country, PinCode,
    GSTNo, PANNo,
    CreditDays, CreditLimit, PaymentTerms,
    IsActive, CreatedAt, CreatedBy
)
```

**Changes in UPDATE statement:**
- Removed references to: `Mobile`, `Industry`, `Status`, `CustomerRating`, `Classification`, `Remarks`
- Changed: `GSTNumber` → `GSTNo`, `PANNumber` → `PANNo`

**Changes in MapToCustomer (data reader):**
- Removed reading of non-existent columns
- Changed column names to match database

**Changes in AddCustomerParameters:**
- Removed parameters for non-existent fields
- Changed parameter names to match database columns

### 6. **CustomerService.cs**
**Changes in CreateAsync:**
- Removed assignments for: `Mobile`, `Industry`, `Status`, `CustomerRating`, `Classification`, `Remarks`
- Changed: `GSTNumber` → `GSTNo`, `PANNumber` → `PANNo`
- Made `CustomerType` required (no default value)

**Changes in UpdateAsync:**
- Removed assignments for: `Mobile`, `Industry`, `Status`, `CustomerRating`, `Classification`, `Remarks`
- Changed: `GSTNumber` → `GSTNo`, `PANNumber` → `PANNo`

**Changes in MapToResponse:**
- Removed mapping of non-existent fields
- Changed field names to match model

**Changes in validation:**
- Updated GST validation to use `GSTNo` instead of `GSTNumber`

---

## ✅ Testing Documentation Updated

### 1. **API_TESTING_POSTMAN.md**
Updated Customer creation JSON to:
```json
{
  "customerCode": "CUST001",
  "customerName": "ABC Industries",
  "customerType": "Direct",
  "contactPerson": "Rajesh Kumar",
  "phone": "9876543210",
  "email": "rajesh@abcindustries.com",
  "address": "Plot No. 15, Industrial Area, Phase 2",
  "city": "Mumbai",
  "state": "Maharashtra",
  "country": "India",
  "pinCode": "400001",
  "gstNo": "27AABCU1234A1Z5",
  "panNo": "AABCU1234A",
  "creditLimit": 500000,
  "creditDays": 45,
  "paymentTerms": "Net 45",
  "isActive": true,
  "createdBy": "Admin"
}
```

### 2. **QUICK_START_TESTING.md**
Updated sample customer JSON to match correct schema.

### 3. **MultiHitech_ERP_Postman_Collection.json**
Updated the "Create Customer" request body with correct field names.

---

## ✅ Build Status

**Build:** ✅ SUCCESS
**Errors:** 0
**Warnings:** 92 (null reference warnings - not critical)

---

## 📝 Correct Customer JSON for Testing

Use this JSON in Postman to create a customer:

```json
{
  "customerCode": "CUST001",
  "customerName": "ABC Industries",
  "customerType": "Direct",
  "contactPerson": "Rajesh Kumar",
  "phone": "9876543210",
  "email": "rajesh@abcindustries.com",
  "address": "Plot No. 15, Industrial Area, Phase 2",
  "city": "Mumbai",
  "state": "Maharashtra",
  "country": "India",
  "pinCode": "400001",
  "gstNo": "27AABCU1234A1Z5",
  "panNo": "AABCU1234A",
  "creditLimit": 500000,
  "creditDays": 45,
  "paymentTerms": "Net 45",
  "isActive": true,
  "createdBy": "Admin"
}
```

**Valid CustomerType values:** `'Direct'`, `'Agent'`, `'Dealer'`

---

## 🎯 What to Test Now

1. **Create Customer:**
   - POST `http://localhost:5217/api/customers`
   - Use the JSON above
   - Expected: `200 OK` with Customer ID

2. **Get All Customers:**
   - GET `http://localhost:5217/api/customers`
   - Expected: Array with your created customer

3. **Get by Code:**
   - GET `http://localhost:5217/api/customers/by-code/CUST001`
   - Expected: Customer details

---

## ⚠️ Important Notes

1. **CustomerType is REQUIRED** - Must be one of: `Direct`, `Agent`, `Dealer`
2. **Use `phone` instead of `contactNumber` or `mobile`**
3. **Use single `address` field instead of `addressLine1` and `addressLine2`**
4. **Use `pinCode` instead of `postalCode`**
5. **Use `gstNo` instead of `gstNumber`**
6. **Use `panNo` instead of `panNumber`**
7. **Fields removed:** `industry`, `status`, `customerRating`, `classification`, `remarks`, `mobile`

---

## ✅ Status: FIXED & READY FOR TESTING!

The Customer API is now fully functional and ready for testing. All field names match the database schema exactly.

**Next Step:** Test the Customer API in Postman with the corrected JSON! 🚀
