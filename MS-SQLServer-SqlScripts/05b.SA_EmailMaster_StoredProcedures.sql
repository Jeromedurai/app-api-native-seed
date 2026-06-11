-- =============================================================================
-- SA_EMAILMASTER: Stored Procedures
-- Run against himalaya_db AFTER 05a.SA_EmailMaster_Table.sql.
--
-- SA_EmailMasterEntry    — enqueue a new email into SA_EMAILMASTER
-- SA_UpdateEmailStatus   — update status with auto-retry logic (max 3 retries)
-- =============================================================================

-- SP: Enqueue a new email
IF OBJECT_ID('[dbo].[SA_EmailMasterEntry]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SA_EmailMasterEntry];
GO
CREATE PROCEDURE [dbo].[SA_EmailMasterEntry]
    @TEMPLATEID     BIGINT,
    @EMAILSP        VARCHAR(500),
    @CORRELATION_ID VARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[SA_EMAILMASTER] (TEMPLATEID, EMAILSP, REQUESTTIME, STATUS, RETRY_COUNT, CORRELATION_ID)
    VALUES (@TEMPLATEID, @EMAILSP, GETDATE(), 0, 0, @CORRELATION_ID);
    SELECT SCOPE_IDENTITY() AS NEW_ID;
END
GO

-- SP: Update email status with auto retry logic (Failed → Retry until 3 retries)
IF OBJECT_ID('[dbo].[SA_UpdateEmailStatus]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[SA_UpdateEmailStatus];
GO
CREATE PROCEDURE [dbo].[SA_UpdateEmailStatus]
    @Id             BIGINT,
    @STATUS         TINYINT,
    @CORRELATION_ID VARCHAR(100) = NULL,
    @ERROR_MSG      VARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @RETRY_COUNT INT = 0;
    DECLARE @NEW_STATUS  TINYINT = @STATUS;

    SELECT @RETRY_COUNT = RETRY_COUNT
    FROM [dbo].[SA_EMAILMASTER] WITH (NOLOCK)
    WHERE ID = @Id;

    -- Failed + retries remaining → promote to Retry
    IF @STATUS = 2 AND @RETRY_COUNT < 3
    BEGIN
        SET @NEW_STATUS  = 3; -- Retry
        SET @RETRY_COUNT = @RETRY_COUNT + 1;
    END

    UPDATE [dbo].[SA_EMAILMASTER]
    SET STATUS         = @NEW_STATUS,
        RETRY_COUNT    = @RETRY_COUNT,
        ERROR_MSG      = @ERROR_MSG,
        CORRELATION_ID = ISNULL(@CORRELATION_ID, CORRELATION_ID)
    WHERE ID = @Id;

    SELECT @NEW_STATUS AS RESULT_STATUS;
END
GO

PRINT 'SA_EMAILMASTER stored procedures created successfully.';
GO
