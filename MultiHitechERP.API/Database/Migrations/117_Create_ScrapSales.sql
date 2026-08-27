-- Scrap (wastage) sales ledger — records off-cuts/wastage sold to the kabadi (scrap dealer).
-- Weight-based manual entry: pick a material, enter kg sold, rate and buyer.
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Stores_ScrapSales')
BEGIN
    CREATE TABLE Stores_ScrapSales (
        Id            INT IDENTITY(1,1) PRIMARY KEY,
        MaterialId    INT NULL,                       -- optional link to Masters_Materials
        MaterialCode  NVARCHAR(100) NULL,
        MaterialName  NVARCHAR(255) NULL,
        WeightKG      DECIMAL(18,3) NOT NULL,
        RatePerKG     DECIMAL(18,2) NOT NULL,
        TotalAmount   DECIMAL(18,2) NOT NULL,
        BuyerName     NVARCHAR(255) NOT NULL,         -- kabadi / dealer, free-text
        SaleDate      DATE NOT NULL,
        Remarks       NVARCHAR(1000) NULL,
        CreatedAt     DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        CreatedBy     NVARCHAR(255) NULL
    );

    CREATE INDEX IX_ScrapSales_MaterialId ON Stores_ScrapSales(MaterialId);
    CREATE INDEX IX_ScrapSales_SaleDate   ON Stores_ScrapSales(SaleDate DESC);
END
