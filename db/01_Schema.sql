IF DB_ID(N'CustomerEnrollDb') IS NULL
    CREATE DATABASE CustomerEnrollDb;
GO

USE CustomerEnrollDb;
GO

IF OBJECT_ID(N'dbo.Customers', N'U') IS NOT NULL
    DROP TABLE dbo.Customers;
GO

CREATE TABLE dbo.Customers
(
    CustomerId    INT IDENTITY(1001, 1) NOT NULL PRIMARY KEY,
    NameEnc       VARBINARY(8000) NOT NULL,
    MobileEnc     VARBINARY(8000) NOT NULL,
    EmailEnc      VARBINARY(8000) NOT NULL,
    ProofTypeEnc  VARBINARY(8000) NOT NULL,
    ProofRefEnc   VARBINARY(8000) NOT NULL,
    AddressEnc    VARBINARY(8000) NOT NULL,
    CreatedOnUtc  DATETIME2(0) NOT NULL CONSTRAINT DF_Customers_CreatedOnUtc DEFAULT (SYSUTCDATETIME())
);
GO
