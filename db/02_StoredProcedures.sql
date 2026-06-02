USE CustomerEnrollDb;
GO

IF OBJECT_ID(N'dbo.usp_Customer_Insert', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Customer_Insert;
GO

CREATE PROCEDURE dbo.usp_Customer_Insert
    @Passphrase    NVARCHAR(128),
    @Name          NVARCHAR(200),
    @Mobile        NVARCHAR(20),
    @Email         NVARCHAR(200),
    @ProofType     NVARCHAR(50),
    @ProofRef      NVARCHAR(100),
    @Address       NVARCHAR(500),
    @NewCustomerId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        INSERT INTO dbo.Customers
            (NameEnc, MobileEnc, EmailEnc, ProofTypeEnc, ProofRefEnc, AddressEnc)
        VALUES
            (ENCRYPTBYPASSPHRASE(@Passphrase, @Name),
             ENCRYPTBYPASSPHRASE(@Passphrase, @Mobile),
             ENCRYPTBYPASSPHRASE(@Passphrase, @Email),
             ENCRYPTBYPASSPHRASE(@Passphrase, @ProofType),
             ENCRYPTBYPASSPHRASE(@Passphrase, @ProofRef),
             ENCRYPTBYPASSPHRASE(@Passphrase, @Address));

        SET @NewCustomerId = SCOPE_IDENTITY();

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO

IF OBJECT_ID(N'dbo.usp_Customer_GetById', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Customer_GetById;
GO

CREATE PROCEDURE dbo.usp_Customer_GetById
    @CustomerId INT,
    @Passphrase NVARCHAR(128)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CustomerId,
        CONVERT(NVARCHAR(200), DECRYPTBYPASSPHRASE(@Passphrase, NameEnc))      AS Name,
        CONVERT(NVARCHAR(20),  DECRYPTBYPASSPHRASE(@Passphrase, MobileEnc))    AS Mobile,
        CONVERT(NVARCHAR(200), DECRYPTBYPASSPHRASE(@Passphrase, EmailEnc))     AS Email,
        CONVERT(NVARCHAR(50),  DECRYPTBYPASSPHRASE(@Passphrase, ProofTypeEnc)) AS ProofType,
        CONVERT(NVARCHAR(100), DECRYPTBYPASSPHRASE(@Passphrase, ProofRefEnc))  AS ProofRef,
        CONVERT(NVARCHAR(500), DECRYPTBYPASSPHRASE(@Passphrase, AddressEnc))   AS Address,
        CreatedOnUtc
    FROM dbo.Customers
    WHERE CustomerId = @CustomerId;
END
GO
