IF OBJECT_ID(N'[dbo].[SP_CREATE_CONTACT_MESSAGE]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SP_CREATE_CONTACT_MESSAGE];
GO

CREATE PROCEDURE [dbo].[SP_CREATE_CONTACT_MESSAGE]
    @UserId BIGINT = NULL,
    @TenantId BIGINT = NULL,
    @Name NVARCHAR(100),
    @Email NVARCHAR(256),
    @Phone NVARCHAR(32) = NULL,
    @Subject NVARCHAR(100) = NULL,
    @Message NVARCHAR(2000),
    @Language NVARCHAR(10) = NULL,
    @Source NVARCHAR(100) = NULL,
    @Id BIGINT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentTime DATETIME2(7) = GETUTCDATE();

    INSERT INTO ContactMessages
    (
        UserId,
        TenantId,
        Name,
        Email,
        Phone,
        Subject,
        Message,
        Language,
        Source,
        CreatedAt
    )
    VALUES
    (
        @UserId,
        @TenantId,
        @Name,
        @Email,
        @Phone,
        @Subject,
        @Message,
        @Language,
        @Source,
        @CurrentTime
    );

    SET @Id = SCOPE_IDENTITY();
END
GO

