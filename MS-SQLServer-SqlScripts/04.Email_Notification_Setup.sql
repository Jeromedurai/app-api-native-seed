-- =============================================================================
-- Email Notification Setup: Tables + Stored Procedures
-- Run once against the main API database (same DB as the rest of the app).
-- Safe to re-run: CREATE TABLE IF NOT EXISTS / DROP+CREATE SPs.
-- =============================================================================

-- -----------------------------------------------------------------------------
-- 1. Tables
-- -----------------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SA_EMAILTEMPLATE')
BEGIN
    CREATE TABLE [dbo].[SA_EMAILTEMPLATE] (
        TEMPLATEID   BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TENANTID     BIGINT               NULL,           -- NULL = global / all tenants
        TEMPLATENAME NVARCHAR(100)        NOT NULL,
        DESCRIPTION  NVARCHAR(255)        NULL,
        VIEWNAME     NVARCHAR(100)        NULL,           -- razor view / template key
        ACTIVE       BIT                 NOT NULL DEFAULT 1,
        CREATEDAT    DATETIME            NOT NULL DEFAULT GETDATE(),
        UPDATEDAT    DATETIME            NOT NULL DEFAULT GETDATE()
    );
    PRINT 'Created SA_EMAILTEMPLATE';
END

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SA_EMAILSCHEDULE')
BEGIN
    CREATE TABLE [dbo].[SA_EMAILSCHEDULE] (
        SCHEDULEID          BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TENANTID            BIGINT               NULL,
        TEMPLATEID          BIGINT               NOT NULL,
        SCHEDULEDESCRIPTION NVARCHAR(100)        NULL,
        SENDBY              TINYINT              NOT NULL DEFAULT 0,  -- 0=daily,1=weekly,2=monthly
        DAY                 NVARCHAR(50)         NULL,                -- e.g. "Monday" or "1"
        TIME                TIME                 NULL,                -- e.g. 09:00
        ACTIVE              BIT                  NOT NULL DEFAULT 1,
        CREATEDAT           DATETIME             NOT NULL DEFAULT GETDATE(),
        UPDATEDAT           DATETIME             NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_EMAILSCHEDULE_TEMPLATE FOREIGN KEY (TEMPLATEID)
            REFERENCES [dbo].[SA_EMAILTEMPLATE](TEMPLATEID)
    );
    PRINT 'Created SA_EMAILSCHEDULE';
END

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SA_EMAILQUEUE')
BEGIN
    CREATE TABLE [dbo].[SA_EMAILQUEUE] (
        QUEUEID      BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        SCHEDULEID   BIGINT               NOT NULL,
        TENANTID     BIGINT               NULL,
        TEMPLATEID   BIGINT               NOT NULL,
        STATUS       NVARCHAR(20)         NOT NULL DEFAULT 'pending',  -- pending/inflight/success/failed/retry
        RETRYCOUNT   INT                  NOT NULL DEFAULT 0,
        ENQUEUEDAT   DATETIME             NOT NULL DEFAULT GETDATE(),
        PROCESSEDAT  DATETIME             NULL,
        SUCCEEDEDAT  DATETIME             NULL,
        FAILEDAT     DATETIME             NULL,
        ERRORMESSAGE NVARCHAR(MAX)        NULL
    );
    PRINT 'Created SA_EMAILQUEUE';
END

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SA_SERVICECONFIG')
BEGIN
    CREATE TABLE [dbo].[SA_SERVICECONFIG] (
        SERVICEID   INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        SERVICENAME NVARCHAR(100)     NOT NULL UNIQUE,
        LAST_RUN    DATETIME          NULL
    );
    PRINT 'Created SA_SERVICECONFIG';
END

-- -----------------------------------------------------------------------------
-- 2. Stored Procedures
-- -----------------------------------------------------------------------------

-- SA_GetAllEmailTemplatesAdmin
-- Returns global templates + caller-tenant templates (or all if @TenantId IS NULL)
IF OBJECT_ID('[dbo].[SA_GetAllEmailTemplatesAdmin]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_GetAllEmailTemplatesAdmin];
GO
CREATE PROCEDURE [dbo].[SA_GetAllEmailTemplatesAdmin]
    @TenantId BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        TEMPLATEID, TENANTID, TEMPLATENAME, DESCRIPTION, VIEWNAME, ACTIVE
    FROM [dbo].[SA_EMAILTEMPLATE] WITH (NOLOCK)
    WHERE @TenantId IS NULL
       OR TENANTID IS NULL
       OR TENANTID = @TenantId
    ORDER BY TEMPLATEID;
END
GO

-- SA_GetEmailTemplateById
IF OBJECT_ID('[dbo].[SA_GetEmailTemplateById]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_GetEmailTemplateById];
GO
CREATE PROCEDURE [dbo].[SA_GetEmailTemplateById]
    @TemplateId BIGINT,
    @TenantId   BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1
        TEMPLATEID, TENANTID, TEMPLATENAME, DESCRIPTION, VIEWNAME, ACTIVE
    FROM [dbo].[SA_EMAILTEMPLATE] WITH (NOLOCK)
    WHERE TEMPLATEID = @TemplateId
      AND (@TenantId IS NULL OR TENANTID IS NULL OR TENANTID = @TenantId);
END
GO

-- SA_UpsertEmailTemplate
-- Insert when @TemplateId = 0, update when @TemplateId > 0
-- Returns the TEMPLATEID
IF OBJECT_ID('[dbo].[SA_UpsertEmailTemplate]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_UpsertEmailTemplate];
GO
CREATE PROCEDURE [dbo].[SA_UpsertEmailTemplate]
    @TemplateId   BIGINT,
    @TemplateName NVARCHAR(100),
    @Description  NVARCHAR(255) = NULL,
    @ViewName     NVARCHAR(100) = NULL,
    @Active       BIT           = 1,
    @TenantId     BIGINT        = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @TemplateId = 0 OR NOT EXISTS (SELECT 1 FROM [dbo].[SA_EMAILTEMPLATE] WHERE TEMPLATEID = @TemplateId)
    BEGIN
        INSERT INTO [dbo].[SA_EMAILTEMPLATE] (TENANTID, TEMPLATENAME, DESCRIPTION, VIEWNAME, ACTIVE)
        VALUES (@TenantId, @TemplateName, @Description, @ViewName, @Active);
        SELECT SCOPE_IDENTITY() AS TEMPLATEID;
    END
    ELSE
    BEGIN
        UPDATE [dbo].[SA_EMAILTEMPLATE]
        SET TEMPLATENAME = @TemplateName,
            DESCRIPTION  = @Description,
            VIEWNAME     = @ViewName,
            ACTIVE       = @Active,
            UPDATEDAT    = GETDATE()
        WHERE TEMPLATEID = @TemplateId
          AND (@TenantId IS NULL OR TENANTID IS NULL OR TENANTID = @TenantId);
        SELECT @TemplateId AS TEMPLATEID;
    END
END
GO

-- SA_DeleteEmailTemplate
-- Raises error if active schedules reference it; returns rows affected
IF OBJECT_ID('[dbo].[SA_DeleteEmailTemplate]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_DeleteEmailTemplate];
GO
CREATE PROCEDURE [dbo].[SA_DeleteEmailTemplate]
    @TemplateId BIGINT,
    @TenantId   BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 FROM [dbo].[SA_EMAILSCHEDULE]
        WHERE TEMPLATEID = @TemplateId AND ACTIVE = 1
    )
    BEGIN
        RAISERROR('schedules reference this template', 16, 1);
        RETURN;
    END

    DELETE FROM [dbo].[SA_EMAILTEMPLATE]
    WHERE TEMPLATEID = @TemplateId
      AND (@TenantId IS NULL OR TENANTID IS NULL OR TENANTID = @TenantId);

    SELECT @@ROWCOUNT AS ROWSAFFECTED;
END
GO

-- SA_SetEmailTemplateActive
IF OBJECT_ID('[dbo].[SA_SetEmailTemplateActive]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_SetEmailTemplateActive];
GO
CREATE PROCEDURE [dbo].[SA_SetEmailTemplateActive]
    @TemplateId BIGINT,
    @Active     BIT,
    @TenantId   BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[SA_EMAILTEMPLATE]
    SET ACTIVE    = @Active,
        UPDATEDAT = GETDATE()
    WHERE TEMPLATEID = @TemplateId
      AND (@TenantId IS NULL OR TENANTID IS NULL OR TENANTID = @TenantId);
    SELECT @@ROWCOUNT AS ROWSAFFECTED;
END
GO

-- SA_GetAllEmailSchedulesAdmin
-- Joins to template for TEMPLATENAME
IF OBJECT_ID('[dbo].[SA_GetAllEmailSchedulesAdmin]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_GetAllEmailSchedulesAdmin];
GO
CREATE PROCEDURE [dbo].[SA_GetAllEmailSchedulesAdmin]
    @TenantId BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        s.SCHEDULEID, s.TENANTID, s.TEMPLATEID,
        t.TEMPLATENAME,
        s.SCHEDULEDESCRIPTION, s.SENDBY, s.DAY, s.TIME, s.ACTIVE
    FROM [dbo].[SA_EMAILSCHEDULE] s WITH (NOLOCK)
    JOIN [dbo].[SA_EMAILTEMPLATE] t WITH (NOLOCK) ON t.TEMPLATEID = s.TEMPLATEID
    WHERE @TenantId IS NULL
       OR s.TENANTID IS NULL
       OR s.TENANTID = @TenantId
    ORDER BY s.SCHEDULEID;
END
GO

-- SA_UpsertEmailSchedule
IF OBJECT_ID('[dbo].[SA_UpsertEmailSchedule]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_UpsertEmailSchedule];
GO
CREATE PROCEDURE [dbo].[SA_UpsertEmailSchedule]
    @ScheduleId          BIGINT,
    @TemplateId          BIGINT,
    @ScheduleDescription NVARCHAR(100) = NULL,
    @SendBy              TINYINT       = 0,
    @Day                 NVARCHAR(50)  = NULL,
    @Time                TIME          = NULL,
    @Active              BIT           = 1,
    @TenantId            BIGINT        = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @ScheduleId = 0 OR NOT EXISTS (SELECT 1 FROM [dbo].[SA_EMAILSCHEDULE] WHERE SCHEDULEID = @ScheduleId)
    BEGIN
        INSERT INTO [dbo].[SA_EMAILSCHEDULE]
            (TENANTID, TEMPLATEID, SCHEDULEDESCRIPTION, SENDBY, DAY, TIME, ACTIVE)
        VALUES
            (@TenantId, @TemplateId, @ScheduleDescription, @SendBy, @Day, @Time, @Active);
        SELECT SCOPE_IDENTITY() AS SCHEDULEID;
    END
    ELSE
    BEGIN
        UPDATE [dbo].[SA_EMAILSCHEDULE]
        SET TEMPLATEID          = @TemplateId,
            SCHEDULEDESCRIPTION = @ScheduleDescription,
            SENDBY              = @SendBy,
            DAY                 = @Day,
            TIME                = @Time,
            ACTIVE              = @Active,
            UPDATEDAT           = GETDATE()
        WHERE SCHEDULEID = @ScheduleId
          AND (@TenantId IS NULL OR TENANTID IS NULL OR TENANTID = @TenantId);
        SELECT @ScheduleId AS SCHEDULEID;
    END
END
GO

-- SA_DeleteEmailSchedule
IF OBJECT_ID('[dbo].[SA_DeleteEmailSchedule]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_DeleteEmailSchedule];
GO
CREATE PROCEDURE [dbo].[SA_DeleteEmailSchedule]
    @ScheduleId BIGINT,
    @TenantId   BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM [dbo].[SA_EMAILSCHEDULE]
    WHERE SCHEDULEID = @ScheduleId
      AND (@TenantId IS NULL OR TENANTID IS NULL OR TENANTID = @TenantId);
    SELECT @@ROWCOUNT AS ROWSAFFECTED;
END
GO

-- SA_SetEmailScheduleActive
IF OBJECT_ID('[dbo].[SA_SetEmailScheduleActive]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_SetEmailScheduleActive];
GO
CREATE PROCEDURE [dbo].[SA_SetEmailScheduleActive]
    @ScheduleId BIGINT,
    @Active     BIT,
    @TenantId   BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[SA_EMAILSCHEDULE]
    SET ACTIVE    = @Active,
        UPDATEDAT = GETDATE()
    WHERE SCHEDULEID = @ScheduleId
      AND (@TenantId IS NULL OR TENANTID IS NULL OR TENANTID = @TenantId);
    SELECT @@ROWCOUNT AS ROWSAFFECTED;
END
GO

-- SA_GetSchedulePendingStats
-- Returns queue health per schedule: pending / retry / inflight counts + timestamps
IF OBJECT_ID('[dbo].[SA_GetSchedulePendingStats]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_GetSchedulePendingStats];
GO
CREATE PROCEDURE [dbo].[SA_GetSchedulePendingStats]
    @TenantId BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        s.SCHEDULEID,
        s.TENANTID,
        s.TEMPLATEID,
        COUNT(CASE WHEN q.STATUS = 'pending'  THEN 1 END) AS PENDING_COUNT,
        COUNT(CASE WHEN q.STATUS = 'retry'    THEN 1 END) AS RETRY_COUNT,
        COUNT(CASE WHEN q.STATUS = 'inflight' THEN 1 END) AS INFLIGHT_COUNT,
        MAX(q.ENQUEUEDAT)  AS LAST_ENQUEUED_AT,
        MAX(q.SUCCEEDEDAT) AS LAST_SUCCESS_AT,
        MAX(q.FAILEDAT)    AS LAST_FAILURE_AT
    FROM [dbo].[SA_EMAILSCHEDULE] s WITH (NOLOCK)
    LEFT JOIN [dbo].[SA_EMAILQUEUE] q WITH (NOLOCK)
        ON q.SCHEDULEID = s.SCHEDULEID
    WHERE @TenantId IS NULL
       OR s.TENANTID IS NULL
       OR s.TENANTID = @TenantId
    GROUP BY s.SCHEDULEID, s.TENANTID, s.TEMPLATEID
    ORDER BY s.SCHEDULEID;
END
GO

PRINT 'Email notification tables and stored procedures created successfully.';
