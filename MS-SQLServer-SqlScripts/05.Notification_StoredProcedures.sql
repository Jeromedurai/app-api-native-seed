-- =============================================================================
-- Email Notification — STORED PROCEDURES (drop + create)
-- Run AFTER 04.Notification_Tables.sql.
--
-- Contents:
--   Templates : SA_GetAllEmailTemplatesAdmin, SA_GetEmailTemplateById,
--               SA_UpsertEmailTemplate, SA_DeleteEmailTemplate, SA_SetEmailTemplateActive
--   Schedules : SA_GetAllEmailSchedulesAdmin, SA_UpsertEmailSchedule,
--               SA_DeleteEmailSchedule, SA_SetEmailScheduleActive
--   Stats     : SA_GetSchedulePendingStats
--   Queue     : SA_EmailMasterEntry, SA_UpdateEmailStatus
--   Audience  : SA_Audience_Newsletter, SA_Audience_Birthday, SA_Audience_AbandonedCart
-- =============================================================================

-- -----------------------------------------------------------------------------
-- DROP all procedures
-- -----------------------------------------------------------------------------
IF OBJECT_ID('dbo.SA_GetAllEmailTemplatesAdmin', 'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_GetAllEmailTemplatesAdmin];
IF OBJECT_ID('dbo.SA_GetEmailTemplateById',      'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_GetEmailTemplateById];
IF OBJECT_ID('dbo.SA_UpsertEmailTemplate',       'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_UpsertEmailTemplate];
IF OBJECT_ID('dbo.SA_DeleteEmailTemplate',       'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_DeleteEmailTemplate];
IF OBJECT_ID('dbo.SA_SetEmailTemplateActive',    'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_SetEmailTemplateActive];
IF OBJECT_ID('dbo.SA_GetAllEmailSchedulesAdmin', 'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_GetAllEmailSchedulesAdmin];
IF OBJECT_ID('dbo.SA_UpsertEmailSchedule',       'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_UpsertEmailSchedule];
IF OBJECT_ID('dbo.SA_DeleteEmailSchedule',       'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_DeleteEmailSchedule];
IF OBJECT_ID('dbo.SA_SetEmailScheduleActive',    'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_SetEmailScheduleActive];
IF OBJECT_ID('dbo.SA_GetSchedulePendingStats',   'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_GetSchedulePendingStats];
IF OBJECT_ID('dbo.SA_EmailMasterEntry',          'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_EmailMasterEntry];
IF OBJECT_ID('dbo.SA_UpdateEmailStatus',         'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_UpdateEmailStatus];
IF OBJECT_ID('dbo.SA_Audience_Newsletter',       'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_Audience_Newsletter];
IF OBJECT_ID('dbo.SA_Audience_Birthday',         'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_Audience_Birthday];
IF OBJECT_ID('dbo.SA_Audience_AbandonedCart',    'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_Audience_AbandonedCart];
IF OBJECT_ID('dbo.SA_Audience_ActiveCustomers',  'P') IS NOT NULL DROP PROCEDURE [dbo].[SA_Audience_ActiveCustomers];
GO

-- =====================================================================
-- TEMPLATES
-- =====================================================================

-- SA_GetAllEmailTemplatesAdmin — global + caller-tenant templates (or all if @TenantId IS NULL)
CREATE PROCEDURE [dbo].[SA_GetAllEmailTemplatesAdmin]
    @TenantId BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        TEMPLATEID, TENANTID, TEMPLATENAME, DESCRIPTION, VIEWNAME, AUDIENCESP, CATEGORY, WHATSAPPTEMPLATE, ACTIVE
    FROM [dbo].[SA_EMAILTEMPLATE] WITH (NOLOCK)
    WHERE @TenantId IS NULL
       OR TENANTID IS NULL
       OR TENANTID = @TenantId
    ORDER BY TEMPLATEID;
END
GO

-- SA_GetEmailTemplateById
CREATE PROCEDURE [dbo].[SA_GetEmailTemplateById]
    @TemplateId BIGINT,
    @TenantId   BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1
        TEMPLATEID, TENANTID, TEMPLATENAME, DESCRIPTION, VIEWNAME, AUDIENCESP, CATEGORY, WHATSAPPTEMPLATE, ACTIVE
    FROM [dbo].[SA_EMAILTEMPLATE] WITH (NOLOCK)
    WHERE TEMPLATEID = @TemplateId
      AND (@TenantId IS NULL OR TENANTID IS NULL OR TENANTID = @TenantId);
END
GO

-- SA_UpsertEmailTemplate — insert when @TemplateId = 0, update otherwise; returns TEMPLATEID
CREATE PROCEDURE [dbo].[SA_UpsertEmailTemplate]
    @TemplateId   BIGINT,
    @TemplateName NVARCHAR(100),
    @Description  NVARCHAR(255) = NULL,
    @ViewName     NVARCHAR(100) = NULL,
    @Active       BIT           = 1,
    @TenantId     BIGINT        = NULL,
    @AudienceSp   NVARCHAR(200) = NULL,
    @Category     NVARCHAR(50)  = NULL,
    @WhatsAppTemplate NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @TemplateId = 0 OR NOT EXISTS (SELECT 1 FROM [dbo].[SA_EMAILTEMPLATE] WHERE TEMPLATEID = @TemplateId)
    BEGIN
        INSERT INTO [dbo].[SA_EMAILTEMPLATE] (TENANTID, TEMPLATENAME, DESCRIPTION, VIEWNAME, AUDIENCESP, CATEGORY, WHATSAPPTEMPLATE, ACTIVE)
        VALUES (@TenantId, @TemplateName, @Description, @ViewName, @AudienceSp, @Category, @WhatsAppTemplate, @Active);
        SELECT SCOPE_IDENTITY() AS TEMPLATEID;
    END
    ELSE
    BEGIN
        UPDATE [dbo].[SA_EMAILTEMPLATE]
        SET TEMPLATENAME = @TemplateName,
            DESCRIPTION  = @Description,
            VIEWNAME     = @ViewName,
            AUDIENCESP   = @AudienceSp,
            CATEGORY     = @Category,
            WHATSAPPTEMPLATE = @WhatsAppTemplate,
            ACTIVE       = @Active,
            UPDATEDAT    = GETDATE()
        WHERE TEMPLATEID = @TemplateId
          AND (@TenantId IS NULL OR TENANTID IS NULL OR TENANTID = @TenantId);
        SELECT @TemplateId AS TEMPLATEID;
    END
END
GO

-- SA_DeleteEmailTemplate — blocked if active schedules reference it; returns rows affected
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

-- =====================================================================
-- SCHEDULES
-- =====================================================================

-- SA_GetAllEmailSchedulesAdmin — joins to template for TEMPLATENAME
CREATE PROCEDURE [dbo].[SA_GetAllEmailSchedulesAdmin]
    @TenantId BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        s.SCHEDULEID, s.TENANTID, s.TEMPLATEID,
        t.TEMPLATENAME,
        s.SCHEDULEDESCRIPTION, s.SENDBY, s.DAY, s.TIME, s.EXCLUDEDUSERIDS, s.COUPONCODE, s.CHANNELS,
        s.SUBJECT, s.HEADLINE, s.MESSAGE, s.CTATEXT, s.CTAURL, s.ACTIVE
    FROM [dbo].[SA_EMAILSCHEDULE] s WITH (NOLOCK)
    JOIN [dbo].[SA_EMAILTEMPLATE] t WITH (NOLOCK) ON t.TEMPLATEID = s.TEMPLATEID
    WHERE @TenantId IS NULL
       OR s.TENANTID IS NULL
       OR s.TENANTID = @TenantId
    ORDER BY s.SCHEDULEID;
END
GO

-- SA_UpsertEmailSchedule
CREATE PROCEDURE [dbo].[SA_UpsertEmailSchedule]
    @ScheduleId          BIGINT,
    @TemplateId          BIGINT,
    @ScheduleDescription NVARCHAR(100) = NULL,
    @SendBy              TINYINT       = 0,
    @Day                 NVARCHAR(50)  = NULL,
    @Time                TIME          = NULL,
    @Active              BIT           = 1,
    @TenantId            BIGINT        = NULL,
    @ExcludedUserIds     NVARCHAR(MAX) = NULL,
    @CouponCode          NVARCHAR(50)  = NULL,
    @Channels            NVARCHAR(50)  = NULL,
    @Subject             NVARCHAR(200) = NULL,
    @Headline            NVARCHAR(200) = NULL,
    @Message             NVARCHAR(MAX) = NULL,
    @CtaText             NVARCHAR(50)  = NULL,
    @CtaUrl              NVARCHAR(300) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @ScheduleId = 0 OR NOT EXISTS (SELECT 1 FROM [dbo].[SA_EMAILSCHEDULE] WHERE SCHEDULEID = @ScheduleId)
    BEGIN
        INSERT INTO [dbo].[SA_EMAILSCHEDULE]
            (TENANTID, TEMPLATEID, SCHEDULEDESCRIPTION, SENDBY, DAY, TIME, EXCLUDEDUSERIDS, COUPONCODE, CHANNELS,
             SUBJECT, HEADLINE, MESSAGE, CTATEXT, CTAURL, ACTIVE)
        VALUES
            (@TenantId, @TemplateId, @ScheduleDescription, @SendBy, @Day, @Time, @ExcludedUserIds, @CouponCode, @Channels,
             @Subject, @Headline, @Message, @CtaText, @CtaUrl, @Active);
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
            EXCLUDEDUSERIDS     = @ExcludedUserIds,
            COUPONCODE          = @CouponCode,
            CHANNELS            = @Channels,
            SUBJECT             = @Subject,
            HEADLINE            = @Headline,
            MESSAGE             = @Message,
            CTATEXT             = @CtaText,
            CTAURL              = @CtaUrl,
            ACTIVE              = @Active,
            UPDATEDAT           = GETDATE()
        WHERE SCHEDULEID = @ScheduleId
          AND (@TenantId IS NULL OR TENANTID IS NULL OR TENANTID = @TenantId);
        SELECT @ScheduleId AS SCHEDULEID;
    END
END
GO

-- SA_DeleteEmailSchedule
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

-- =====================================================================
-- STATS
-- =====================================================================

-- SA_GetSchedulePendingStats — live queue counts per schedule, read from the actual
-- send queue (SA_EMAILMASTER). Queue rows carry TEMPLATEID (not SCHEDULEID), so counts
-- are attributed to every schedule that uses the template (if two schedules share a
-- template they show the same template-level totals — acceptable; far better than 0).
-- STATUS: 0=Pending, 1=Success, 2=Failed, 3=Retry. INFLIGHT_COUNT is repurposed as
-- "failed" (the master queue has no in-flight state) to reuse the existing DTO field.
CREATE PROCEDURE [dbo].[SA_GetSchedulePendingStats]
    @TenantId BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        s.SCHEDULEID,
        s.TENANTID,
        s.TEMPLATEID,
        COUNT(CASE WHEN m.STATUS = 0 THEN 1 END) AS PENDING_COUNT,
        COUNT(CASE WHEN m.STATUS = 3 THEN 1 END) AS RETRY_COUNT,
        COUNT(CASE WHEN m.STATUS = 2 THEN 1 END) AS INFLIGHT_COUNT,   -- failed
        MAX(m.REQUESTTIME)                                  AS LAST_ENQUEUED_AT,
        MAX(CASE WHEN m.STATUS = 1 THEN m.REQUESTTIME END)  AS LAST_SUCCESS_AT,
        MAX(CASE WHEN m.STATUS = 2 THEN m.REQUESTTIME END)  AS LAST_FAILURE_AT
    FROM [dbo].[SA_EMAILSCHEDULE] s WITH (NOLOCK)
    LEFT JOIN [dbo].[SA_EMAILMASTER] m WITH (NOLOCK)
        ON m.TEMPLATEID = s.TEMPLATEID
    WHERE @TenantId IS NULL
       OR s.TENANTID IS NULL
       OR s.TENANTID = @TenantId
    GROUP BY s.SCHEDULEID, s.TENANTID, s.TEMPLATEID
    ORDER BY s.SCHEDULEID;
END
GO

-- =====================================================================
-- QUEUE (SA_EMAILMASTER)
-- =====================================================================

-- SA_EmailMasterEntry — enqueue one email; returns NEW_ID
CREATE PROCEDURE [dbo].[SA_EmailMasterEntry]
    @TEMPLATEID     BIGINT,
    @EMAILSP        NVARCHAR(MAX),
    @CORRELATION_ID VARCHAR(100) = NULL,
    @TENANTID       BIGINT       = NULL,
    @CHANNEL        NVARCHAR(20)  = 'Email'
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[SA_EMAILMASTER] (TENANTID, CHANNEL, TEMPLATEID, EMAILSP, REQUESTTIME, STATUS, RETRY_COUNT, CORRELATION_ID)
    VALUES (@TENANTID, ISNULL(@CHANNEL, 'Email'), @TEMPLATEID, @EMAILSP, GETDATE(), 0, 0, @CORRELATION_ID);
    SELECT SCOPE_IDENTITY() AS NEW_ID;
END
GO

-- SA_UpdateEmailStatus — update status with auto-retry (Failed -> Retry until 3 retries)
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

-- =====================================================================
-- AUDIENCE SPs (scheduled sends)
-- Contract: param @TenantId BIGINT = NULL; ONE row per recipient with at least
-- Email (required); plus TenantId / Name / Subject / RecipientId; extra columns
-- pass through to the Razor view as merge fields. Adapt WHERE clauses to your rules.
-- =====================================================================

-- SA_Audience_Newsletter — every active user in the tenant
CREATE PROCEDURE [dbo].[SA_Audience_Newsletter]
    @TenantId   BIGINT = NULL,
    @TemplateId BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        u.UserId                                     AS RecipientId,
        u.TenantId                                   AS TenantId,
        u.Email                                      AS Email,
        u.Phone                                      AS Phone,
        LTRIM(RTRIM(u.FirstName + ' ' + u.LastName)) AS Name,
        N'Our latest news & offers'                  AS Subject
    FROM dbo.Users u WITH (NOLOCK)
    WHERE u.Active = 1
      AND u.Email IS NOT NULL
      AND u.Email <> ''
      AND (@TenantId IS NULL OR u.TenantId = @TenantId);
END
GO

-- SA_Audience_Birthday — users whose birthday (month/day) is today
CREATE PROCEDURE [dbo].[SA_Audience_Birthday]
    @TenantId   BIGINT = NULL,
    @TemplateId BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        u.UserId                                     AS RecipientId,
        u.TenantId                                   AS TenantId,
        u.Email                                      AS Email,
        u.Phone                                      AS Phone,
        LTRIM(RTRIM(u.FirstName + ' ' + u.LastName)) AS Name,
        N'Happy Birthday! A gift inside'             AS Subject,
        u.FirstName                                  AS FirstName
    FROM dbo.Users u WITH (NOLOCK)
    WHERE u.Active = 1
      AND u.DateOfBirth IS NOT NULL
      AND MONTH(u.DateOfBirth) = MONTH(GETDATE())
      AND DAY(u.DateOfBirth)   = DAY(GETDATE())
      AND u.Email IS NOT NULL
      AND u.Email <> ''
      AND (@TenantId IS NULL OR u.TenantId = @TenantId);
END
GO

-- SA_Audience_AbandonedCart — users with active cart items older than 24h
CREATE PROCEDURE [dbo].[SA_Audience_AbandonedCart]
    @TenantId   BIGINT = NULL,
    @TemplateId BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        u.UserId                                     AS RecipientId,
        u.TenantId                                   AS TenantId,
        u.Email                                      AS Email,
        u.Phone                                      AS Phone,
        LTRIM(RTRIM(u.FirstName + ' ' + u.LastName)) AS Name,
        N'You left something in your cart'           AS Subject,
        COUNT(c.CartId)                              AS ItemCount
    FROM dbo.CartItems c WITH (NOLOCK)
    JOIN dbo.Users u WITH (NOLOCK) ON u.UserId = c.UserId
    WHERE c.Active = 1
      AND c.AddedDate < DATEADD(HOUR, -24, GETUTCDATE())
      AND u.Active = 1
      AND u.Email IS NOT NULL
      AND u.Email <> ''
      AND (@TenantId IS NULL OR c.TenantId = @TenantId)
    GROUP BY u.UserId, u.TenantId, u.Email, u.Phone, u.FirstName, u.LastName;
END
GO

-- SA_Audience_ActiveCustomers — every eligible customer (active + has email) in the
-- tenant. Shared by the schedulable OFFER / promotional templates so that when an
-- admin schedules one, the worker sends it to all eligible customers at fire time.
-- Refine the WHERE clause to your eligibility rules (e.g. marketing opt-in, exclude
-- staff roles, recent activity). Subject is intentionally omitted so each template's
-- own name is used as the subject line.
CREATE PROCEDURE [dbo].[SA_Audience_ActiveCustomers]
    @TenantId   BIGINT = NULL,
    @TemplateId BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        u.UserId                                     AS RecipientId,
        u.TenantId                                   AS TenantId,
        u.Email                                      AS Email,
        u.Phone                                      AS Phone,
        LTRIM(RTRIM(u.FirstName + ' ' + u.LastName)) AS Name,
        u.FirstName                                  AS FirstName
    FROM dbo.Users u WITH (NOLOCK)
    WHERE u.Active = 1
      AND u.Email IS NOT NULL
      AND u.Email <> ''
      AND (@TenantId IS NULL OR u.TenantId = @TenantId);
END
GO

PRINT 'Notification stored procedures ready.';
GO
