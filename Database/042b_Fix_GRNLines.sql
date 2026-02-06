-- Fix GRNLines table creation (LineNo is a reserved keyword)
USE MultiHitechERP;
GO

-- Drop if exists
IF OBJECT_ID('dbo.Stores_GRNLines', 'U') IS NOT NULL
BEGIN
    DROP TABLE Stores_GRNLines;
END
GO

-- Recreate with correct syntax
CREATE TABLE Stores_GRNLines (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    GRNId INT NOT NULL,
    SequenceNo INT NOT NULL, -- Changed from LineNo

    -- Material Reference
    MaterialId INT NOT NULL,
    MaterialName NVARCHAR(200),
    Grade NVARCHAR(100),

    -- Material Type & Dimensions
    MaterialType NVARCHAR(20) NOT NULL, -- 'Rod', 'Pipe', 'Sheet', 'Forged'
    Diameter DECIMAL(10,2), -- For Rod (solid)
    OuterDiameter DECIMAL(10,2), -- For Pipe
    InnerDiameter DECIMAL(10,2), -- For Pipe
    Width DECIMAL(10,2), -- For Sheet
    Thickness DECIMAL(10,2), -- For Sheet

    -- Material Properties
    MaterialDensity DECIMAL(10,4), -- g/cm³ (7.85 for MS/EN8, 7.9 for SS)

    -- Quantities
    TotalWeightKG DECIMAL(10,4) NOT NULL,
    CalculatedLengthMM DECIMAL(10,2),
    WeightPerMeterKG DECIMAL(10,4),

    -- Piece Breakdown
    NumberOfPieces INT NOT NULL,
    LengthPerPieceMM DECIMAL(10,2),

    -- Pricing
    UnitPrice DECIMAL(18,4), -- Price per KG
    LineTotal DECIMAL(18,2),

    -- Remarks
    Remarks NVARCHAR(500),

    CONSTRAINT FK_GRNLines_GRN FOREIGN KEY (GRNId)
        REFERENCES Stores_GRN(Id) ON DELETE CASCADE,

    CONSTRAINT FK_GRNLines_Material FOREIGN KEY (MaterialId)
        REFERENCES Masters_Materials(Id),

    CONSTRAINT CHK_GRNLines_MaterialType
        CHECK (MaterialType IN ('Rod', 'Pipe', 'Sheet', 'Forged')),

    CONSTRAINT UQ_GRNLine UNIQUE (GRNId, SequenceNo)
);
GO

CREATE INDEX IX_GRNLines_GRN ON Stores_GRNLines(GRNId);
CREATE INDEX IX_GRNLines_Material ON Stores_GRNLines(MaterialId);
GO

PRINT 'Stores_GRNLines table created successfully!';
GO
