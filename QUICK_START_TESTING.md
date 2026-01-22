# MultiHitech ERP - Quick Start Testing Guide

## ✅ API Status
- **API Running:** http://localhost:5217
- **Swagger UI:** http://localhost:5217/swagger
- **Status:** ✅ Confirmed Working

---

## 🚀 Quick Start (5 Minutes)

### Step 1: Import Postman Collection
1. Open Postman
2. Click **Import** button (top left)
3. Select file: `MultiHitech_ERP_Postman_Collection.json`
4. Collection will be imported with all endpoints

### Step 2: Test Your First API
**Test GET endpoint (no data needed):**
```
GET http://localhost:5217/api/customers
```
Expected Response:
```json
{
  "success": true,
  "message": null,
  "data": [],
  "errors": null
}
```

### Step 3: Create Your First Customer
**In Postman:**
1. Open collection: `MultiHitech ERP API - Master Data Testing`
2. Open folder: `1. Customer Module`
3. Click: `Create Customer`
4. Click: **Send**

**Expected Response:**
```json
{
  "success": true,
  "message": "Customer created successfully",
  "data": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

✅ **Copy the ID from response** - you'll need it!

### Step 4: Verify Customer Created
Click: `Get All Customers` → Send

You should see your customer in the response!

---

## 📝 Recommended Testing Order

### Phase 1: Create Master Data (20-30 minutes)

**1. Customer Module** ✅ Start Here
- ✅ Create Customer
- ✅ Get All Customers
- ✅ Get by Code: `CUST001`

**2. Product Module**
- ✅ Create Product
- ✅ Get All Products
- ✅ Get by Code: `PROD001`
- **💡 Save Product ID** for later use

**3. Material Module**
- ✅ Create Material (EN8 Steel)
- ✅ Get All Materials
- ✅ Get by Code: `MAT001`
- **💡 Save Material ID** for inventory testing

**4. Machine Module**
- ✅ Create Machine (CNC Lathe)
- ✅ Get All Machines
- ✅ Get Available Machines

**5. Process Module**
- ✅ Create Process (CNC Turning)
- ✅ Get All Processes

**6. Operator Module**
- ✅ Create Operator
- ✅ Get All Operators
- ✅ Get Available Operators

**7. Drawing Module**
- ✅ Create Drawing
  - **⚠️ Replace `{{productId}}` with actual Product ID**
- ✅ Get All Drawings

**8. Supplier Module**
- ✅ Create Supplier
- ✅ Get All Suppliers
- ✅ Approve Supplier
  - **⚠️ Replace `{{supplierId}}` with actual Supplier ID**

---

## 🎯 Success Criteria (After Phase 1)

After completing all 8 modules, verify:
- ✅ At least 1 customer created
- ✅ At least 1 product created
- ✅ At least 1 material created
- ✅ At least 1 machine created
- ✅ At least 1 process created
- ✅ At least 1 operator created
- ✅ At least 1 drawing created (linked to product)
- ✅ At least 1 supplier created and approved

---

## 🔍 Testing Each Endpoint

### Pattern for Every Module:

1. **POST - Create**
   - Creates new record
   - Returns ID in response
   - **Save this ID!**

2. **GET - Get All**
   - Returns all records
   - Verify your created record appears

3. **GET - Get by Code**
   - Retrieves specific record by code
   - Confirms data integrity

4. **GET - Get by ID**
   - Use saved ID from create
   - Returns complete record details

---

## 📊 Sample Test Data

### Customer
```json
{
  "customerCode": "CUST001",
  "customerName": "ABC Industries",
  "customerType": "Direct",
  "contactPerson": "Rajesh Kumar",
  "phone": "9876543210",
  "email": "rajesh@abcindustries.com",
  "address": "Plot No. 15, Industrial Area",
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

### Product
```json
{
  "productCode": "PROD001",
  "productName": "Magnetic Roller 250mm",
  "category": "Magnetic Rollers",
  "productType": "Finished Goods",
  "hsnCode": "84439190",
  "unitOfMeasure": "NOS",
  "sellingPrice": 25000,
  "isActive": true,
  "createdBy": "Admin"
}
```

### Material
```json
{
  "materialCode": "MAT001",
  "materialName": "EN8 Round Bar 50mm",
  "category": "Raw Material",
  "materialType": "Steel",
  "grade": "EN8",
  "primaryUOM": "KG",
  "standardCost": 85,
  "reorderLevel": 500,
  "isActive": true,
  "createdBy": "Admin"
}
```

---

## ⚠️ Common Issues & Solutions

### Issue 1: "Customer with code already exists"
**Solution:** Change `customerCode` to `CUST002`, `CUST003`, etc.

### Issue 2: 404 Not Found
**Solution:**
- Check base URL is correct: `http://localhost:5217`
- Verify API is running
- Check endpoint path (case-sensitive)

### Issue 3: "ProductId not found" when creating Drawing
**Solution:**
1. First create a Product
2. Copy the Product ID from response
3. Replace `{{productId}}` in Drawing request with actual ID

### Issue 4: Null/Empty Response
**Solution:** This is normal if no data exists yet. Create some records first!

---

## 🧪 Verification Commands (Optional - Command Line)

### Check if API is running:
```bash
curl http://localhost:5217/api/customers
```

### Quick create customer via curl:
```bash
curl -X POST http://localhost:5217/api/customers \
  -H "Content-Type: application/json" \
  -d "{\"customerCode\":\"CUST001\",\"customerName\":\"Test Customer\",\"contactPerson\":\"John Doe\",\"contactNumber\":\"1234567890\",\"isActive\":true,\"createdBy\":\"Admin\"}"
```

---

## 📚 Next Steps After Master Data

Once you've created master data, you can test:

1. **Inventory Module**
   - Stock In (receive materials)
   - Stock Out (issue materials)
   - Check inventory levels

2. **Order Module**
   - Create orders using Customer ID and Product ID
   - Approve drawings
   - Move to planning

3. **BOM Module**
   - Create BOM linking Product to Materials
   - Add BOM items

4. **Full Workflow**
   - Order → Drawing Approval → Job Card → Material Requisition → Production → QC → Dispatch

---

## 🎯 Quick Checklist

- [ ] API is running on port 5217
- [ ] Postman collection imported
- [ ] Created 1 Customer successfully
- [ ] Created 1 Product successfully
- [ ] Created 1 Material successfully
- [ ] Created 1 Machine successfully
- [ ] Created 1 Process successfully
- [ ] Created 1 Operator successfully
- [ ] Created 1 Drawing (with Product ID)
- [ ] Created 1 Supplier successfully
- [ ] Approved the Supplier
- [ ] Verified all GET endpoints return data

---

## 💡 Pro Tips

1. **Save IDs:** Keep a notepad with all created IDs for cross-module testing
2. **Use Variables:** In Postman, create environment variables for frequently used IDs
3. **Test Order:** Always create dependencies first (e.g., Product before Drawing)
4. **Check Swagger:** Browse `http://localhost:5217/swagger` for all available endpoints
5. **Watch Responses:** Success responses have `"success": true`, errors have `"success": false`

---

## ✅ You're Ready!

**Start with Customer Module and work your way through all 8 modules.**

Good luck! 🚀

---

**Need Help?**
- Check full API documentation: `API_TESTING_POSTMAN.md`
- View Swagger UI: `http://localhost:5217/swagger`
- Review backend progress: `BACKEND_PROGRESS.md`
