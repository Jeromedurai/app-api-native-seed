-- =============================================
-- Customer Address Management Stored Procedures
-- =============================================

USE [XTRACHEF_DB_DEV]
GO

-- Drop existing procedures if they exist
IF OBJECT_ID(N'[dbo].[SP_GET_USER_ADDRESSES]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_GET_USER_ADDRESSES];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_ADDRESS_BY_ID]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_GET_ADDRESS_BY_ID];
GO

IF OBJECT_ID(N'[dbo].[SP_CREATE_USER_ADDRESS]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_CREATE_USER_ADDRESS];
GO

IF OBJECT_ID(N'[dbo].[SP_UPDATE_USER_ADDRESS]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_UPDATE_USER_ADDRESS];
GO

IF OBJECT_ID(N'[dbo].[SP_DELETE_USER_ADDRESS]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_DELETE_USER_ADDRESS];
GO

IF OBJECT_ID(N'[dbo].[SP_SET_DEFAULT_ADDRESS]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_SET_DEFAULT_ADDRESS];
GO

IF OBJECT_ID(N'[dbo].[SP_ADMIN_GET_ALL_ADDRESSES]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_ADMIN_GET_ALL_ADDRESSES];
GO

IF OBJECT_ID(N'[dbo].[SP_VALIDATE_ADDRESS]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_VALIDATE_ADDRESS];
GO

-- =============================================
-- SP_GET_USER_ADDRESSES
-- =============================================
CREATE PROCEDURE [dbo].[SP_GET_USER_ADDRESSES]
    @UserId BIGINT,
    @ActiveOnly BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        AddressId,
        UserId,
        AddressType,
        Street,
        City,
        State,
        PostalCode,
        Country,
        IsDefault,
        Active,
        CreatedAt,
        UpdatedAt
    FROM UserAddresses
    WHERE UserId = @UserId
        AND (@ActiveOnly = 0 OR Active = 1)
    ORDER BY IsDefault DESC, CreatedAt DESC;
END
GO

-- =============================================
-- SP_GET_ADDRESS_BY_ID
-- =============================================
CREATE PROCEDURE [dbo].[SP_GET_ADDRESS_BY_ID]
    @AddressId BIGINT,
    @UserId BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        AddressId,
        UserId,
        AddressType,
        Street,
        City,
        State,
        PostalCode,
        Country,
        IsDefault,
        Active,
        CreatedAt,
        UpdatedAt
    FROM UserAddresses
    WHERE AddressId = @AddressId
        AND (@UserId IS NULL OR UserId = @UserId)
        AND Active = 1;
END
GO

-- =============================================
-- SP_CREATE_USER_ADDRESS
-- =============================================
CREATE PROCEDURE [dbo].[SP_CREATE_USER_ADDRESS]
    @UserId BIGINT,
    @AddressType NVARCHAR(50) = 'Home',
    @Street NVARCHAR(255),
    @City NVARCHAR(100),
    @State NVARCHAR(100),
    @PostalCode NVARCHAR(20),
    @Country NVARCHAR(100) = 'IN',
    @IsDefault BIT = 0,
    @AddressId BIGINT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @CurrentTime DATETIME2(7) = GETUTCDATE();
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validate user exists
        IF NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId AND Active = 1)
        BEGIN
            RAISERROR('User not found or inactive.', 16, 1);
            RETURN;
        END
        
        -- If setting as default, unset other default addresses
        IF @IsDefault = 1
        BEGIN
            UPDATE UserAddresses
            SET IsDefault = 0
            WHERE UserId = @UserId AND Active = 1;
        END
        
        -- If this is the first address, set as default
        IF NOT EXISTS (SELECT 1 FROM UserAddresses WHERE UserId = @UserId AND Active = 1)
        BEGIN
            SET @IsDefault = 1;
        END
        
        -- Insert new address
        INSERT INTO UserAddresses (
            UserId,
            AddressType,
            Street,
            City,
            State,
            PostalCode,
            Country,
            IsDefault,
            Active,
            CreatedAt,
            UpdatedAt
        ) VALUES (
            @UserId,
            @AddressType,
            @Street,
            @City,
            @State,
            @PostalCode,
            @Country,
            @IsDefault,
            1,
            @CurrentTime,
            @CurrentTime
        );
        
        SET @AddressId = SCOPE_IDENTITY();
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- =============================================
-- SP_UPDATE_USER_ADDRESS
-- =============================================
CREATE PROCEDURE [dbo].[SP_UPDATE_USER_ADDRESS]
    @AddressId BIGINT,
    @UserId BIGINT,
    @AddressType NVARCHAR(50) = NULL,
    @Street NVARCHAR(255) = NULL,
    @City NVARCHAR(100) = NULL,
    @State NVARCHAR(100) = NULL,
    @PostalCode NVARCHAR(20) = NULL,
    @Country NVARCHAR(100) = NULL,
    @IsDefault BIT = NULL,
    @Active BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @CurrentTime DATETIME2(7) = GETUTCDATE();
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validate address exists and belongs to user
        IF NOT EXISTS (SELECT 1 FROM UserAddresses WHERE AddressId = @AddressId AND UserId = @UserId)
        BEGIN
            RAISERROR('Address not found or does not belong to this user.', 16, 1);
            RETURN;
        END
        
        -- If setting as default, unset other default addresses
        IF @IsDefault = 1
        BEGIN
            UPDATE UserAddresses
            SET IsDefault = 0
            WHERE UserId = @UserId AND AddressId != @AddressId AND Active = 1;
        END
        
        -- Update address
        UPDATE UserAddresses
        SET 
            AddressType = ISNULL(@AddressType, AddressType),
            Street = ISNULL(@Street, Street),
            City = ISNULL(@City, City),
            State = ISNULL(@State, State),
            PostalCode = ISNULL(@PostalCode, PostalCode),
            Country = ISNULL(@Country, Country),
            IsDefault = CASE WHEN @IsDefault IS NOT NULL THEN @IsDefault ELSE IsDefault END,
            Active = CASE WHEN @Active IS NOT NULL THEN @Active ELSE Active END,
            UpdatedAt = @CurrentTime
        WHERE AddressId = @AddressId AND UserId = @UserId;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- =============================================
-- SP_DELETE_USER_ADDRESS
-- =============================================
CREATE PROCEDURE [dbo].[SP_DELETE_USER_ADDRESS]
    @AddressId BIGINT,
    @UserId BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validate address exists and belongs to user
        IF NOT EXISTS (SELECT 1 FROM UserAddresses WHERE AddressId = @AddressId AND UserId = @UserId)
        BEGIN
            RAISERROR('Address not found or does not belong to this user.', 16, 1);
            RETURN;
        END
        
        -- Soft delete (set Active = 0)
        UPDATE UserAddresses
        SET Active = 0,
            UpdatedAt = GETUTCDATE()
        WHERE AddressId = @AddressId AND UserId = @UserId;
        
        -- If this was the default address, set another address as default
        IF EXISTS (SELECT 1 FROM UserAddresses WHERE AddressId = @AddressId AND IsDefault = 1)
        BEGIN
            DECLARE @NewDefaultId BIGINT;
            SELECT TOP 1 @NewDefaultId = AddressId
            FROM UserAddresses
            WHERE UserId = @UserId AND AddressId != @AddressId AND Active = 1
            ORDER BY CreatedAt DESC;
            
            IF @NewDefaultId IS NOT NULL
            BEGIN
                UPDATE UserAddresses
                SET IsDefault = 1
                WHERE AddressId = @NewDefaultId;
            END
        END
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- =============================================
-- SP_SET_DEFAULT_ADDRESS
-- =============================================
CREATE PROCEDURE [dbo].[SP_SET_DEFAULT_ADDRESS]
    @AddressId BIGINT,
    @UserId BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validate address exists and belongs to user
        IF NOT EXISTS (SELECT 1 FROM UserAddresses WHERE AddressId = @AddressId AND UserId = @UserId AND Active = 1)
        BEGIN
            RAISERROR('Address not found or does not belong to this user.', 16, 1);
            RETURN;
        END
        
        -- Unset all other default addresses
        UPDATE UserAddresses
        SET IsDefault = 0
        WHERE UserId = @UserId AND AddressId != @AddressId AND Active = 1;
        
        -- Set this address as default
        UPDATE UserAddresses
        SET IsDefault = 1,
            UpdatedAt = GETUTCDATE()
        WHERE AddressId = @AddressId AND UserId = @UserId;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- =============================================
-- SP_ADMIN_GET_ALL_ADDRESSES
-- =============================================
CREATE PROCEDURE [dbo].[SP_ADMIN_GET_ALL_ADDRESSES]
    @TenantId BIGINT = NULL,
    @UserId BIGINT = NULL,
    @ActiveOnly BIT = 1,
    @PageNumber INT = 1,
    @PageSize INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    -- Get total count
    SELECT COUNT(*) AS TotalCount
    FROM UserAddresses ua
    INNER JOIN Users u ON ua.UserId = u.UserId
    WHERE (@TenantId IS NULL OR u.TenantId = @TenantId)
        AND (@UserId IS NULL OR ua.UserId = @UserId)
        AND (@ActiveOnly = 0 OR ua.Active = 1);
    
    -- Get addresses with user info
    SELECT 
        ua.AddressId,
        ua.UserId,
        u.FirstName + ' ' + u.LastName AS UserName,
        u.Email AS UserEmail,
        ua.AddressType,
        ua.Street,
        ua.City,
        ua.State,
        ua.PostalCode,
        ua.Country,
        ua.IsDefault,
        ua.Active,
        ua.CreatedAt,
        ua.UpdatedAt
    FROM UserAddresses ua
    INNER JOIN Users u ON ua.UserId = u.UserId
    WHERE (@TenantId IS NULL OR u.TenantId = @TenantId)
        AND (@UserId IS NULL OR ua.UserId = @UserId)
        AND (@ActiveOnly = 0 OR ua.Active = 1)
    ORDER BY ua.CreatedAt DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO

-- =============================================
-- SP_VALIDATE_ADDRESS
-- =============================================
CREATE PROCEDURE [dbo].[SP_VALIDATE_ADDRESS]
    @Street NVARCHAR(255),
    @City NVARCHAR(100),
    @State NVARCHAR(100),
    @PostalCode NVARCHAR(20),
    @Country NVARCHAR(100) = 'IN',
    @IsValid BIT OUTPUT,
    @ValidationMessage NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    SET @IsValid = 1;
    SET @ValidationMessage = '';
    
    -- Validate required fields
    IF @Street IS NULL OR LEN(LTRIM(RTRIM(@Street))) = 0
    BEGIN
        SET @IsValid = 0;
        SET @ValidationMessage = 'Street address is required.';
        RETURN;
    END
    
    IF @City IS NULL OR LEN(LTRIM(RTRIM(@City))) = 0
    BEGIN
        SET @IsValid = 0;
        SET @ValidationMessage = 'City is required.';
        RETURN;
    END
    
    IF @State IS NULL OR LEN(LTRIM(RTRIM(@State))) = 0
    BEGIN
        SET @IsValid = 0;
        SET @ValidationMessage = 'State is required.';
        RETURN;
    END
    
    IF @PostalCode IS NULL OR LEN(LTRIM(RTRIM(@PostalCode))) = 0
    BEGIN
        SET @IsValid = 0;
        SET @ValidationMessage = 'Postal code is required.';
        RETURN;
    END
    
    -- Validate postal code format (Indian postal codes are 6 digits)
    IF @Country = 'IN' AND NOT (@PostalCode LIKE '[0-9][0-9][0-9][0-9][0-9][0-9]')
    BEGIN
        SET @IsValid = 0;
        SET @ValidationMessage = 'Postal code must be 6 digits for India.';
        RETURN;
    END
    
    -- Validate state exists in States table
    IF NOT EXISTS (SELECT 1 FROM States WHERE StateCode = @State AND Active = 1)
    BEGIN
        SET @IsValid = 0;
        SET @ValidationMessage = 'Invalid state code.';
        RETURN;
    END
    
    -- Address is valid
    SET @IsValid = 1;
    SET @ValidationMessage = 'Address is valid.';
END
GO

