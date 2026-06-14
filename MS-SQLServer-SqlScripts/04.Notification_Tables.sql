-- =============================================================================
-- Email Notification — TABLES (drop + create)
-- Run order: 04 (tables) -> 05 (stored procedures) -> 06 (seed data).
--
-- WARNING: this script DROPS and recreates the notification tables, so any
-- existing rows (templates, schedules, queued emails) are removed. Re-run 06
-- afterwards to reseed the 46 catalog templates.
--
--   SA_EMAILTEMPLATE  — admin templates (name, razor view, audience SP, category)
--   SA_EMAILSCHEDULE  — when to send (SendBy 0=NotScheduled,1=Immediate,2=Daily,3=Weekly,4=Monthly)
--   SA_EMAILMASTER    — send queue (one row per email; worker dequeues -> API renders -> SMTP)
--   SA_EMAILQUEUE     — per-schedule queue stats (admin dashboard)
--   SA_SERVICECONFIG  — background worker heartbeat (LAST_RUN)
-- =============================================================================

-- -----------------------------------------------------------------------------
-- DROP (child / FK tables first; SA_EMAILSCHEDULE has an FK to SA_EMAILTEMPLATE)
-- -----------------------------------------------------------------------------
IF OBJECT_ID('dbo.SA_EMAILMASTER',   'U') IS NOT NULL DROP TABLE [dbo].[SA_EMAILMASTER];
IF OBJECT_ID('dbo.SA_EMAILQUEUE',    'U') IS NOT NULL DROP TABLE [dbo].[SA_EMAILQUEUE];
IF OBJECT_ID('dbo.SA_EMAILSCHEDULE', 'U') IS NOT NULL DROP TABLE [dbo].[SA_EMAILSCHEDULE];
IF OBJECT_ID('dbo.SA_SERVICECONFIG', 'U') IS NOT NULL DROP TABLE [dbo].[SA_SERVICECONFIG];
IF OBJECT_ID('dbo.SA_EMAILTEMPLATE', 'U') IS NOT NULL DROP TABLE [dbo].[SA_EMAILTEMPLATE];
GO

-- -----------------------------------------------------------------------------
-- SA_EMAILTEMPLATE
-- -----------------------------------------------------------------------------
CREATE TABLE [dbo].[SA_EMAILTEMPLATE] (
    TEMPLATEID   BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    TENANTID     BIGINT               NULL,           -- NULL = global / all tenants
    TEMPLATENAME NVARCHAR(100)        NOT NULL,
    DESCRIPTION  NVARCHAR(255)        NULL,
    VIEWNAME     NVARCHAR(100)        NULL,           -- razor view / template key
    AUDIENCESP   NVARCHAR(200)        NULL,           -- audience SP for scheduled sends; NULL = event-driven/transactional
    CATEGORY     NVARCHAR(50)         NULL,           -- grouping: Account/Orders/Payments/Cart/Marketing/Reviews/Subscription
    WHATSAPPTEMPLATE NVARCHAR(100)    NULL,           -- Meta-approved WhatsApp template name (separate from VIEWNAME)
    ACTIVE       BIT                 NOT NULL DEFAULT 1,
    CREATEDAT    DATETIME            NOT NULL DEFAULT GETDATE(),
    UPDATEDAT    DATETIME            NOT NULL DEFAULT GETDATE()
);
-- Tenant-scoped list + seed name lookup (SA_GetAllEmailTemplatesAdmin / seed NOT EXISTS)
CREATE NONCLUSTERED INDEX [IX_SA_EMAILTEMPLATE_TENANT_NAME]
    ON [dbo].[SA_EMAILTEMPLATE]([TENANTID], [TEMPLATENAME]) INCLUDE ([ACTIVE]);
PRINT 'Created SA_EMAILTEMPLATE';
GO

-- -----------------------------------------------------------------------------
-- SA_EMAILSCHEDULE
-- -----------------------------------------------------------------------------
CREATE TABLE [dbo].[SA_EMAILSCHEDULE] (
    SCHEDULEID          BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    TENANTID            BIGINT               NULL,
    TEMPLATEID          BIGINT               NOT NULL,
    SCHEDULEDESCRIPTION NVARCHAR(100)        NULL,
    SENDBY              TINYINT              NOT NULL DEFAULT 0,  -- 0=NotScheduled,1=Immediate,2=Daily,3=Weekly,4=Monthly
    DAY                 NVARCHAR(50)         NULL,                -- Weekly: CSV 0..6 (Sun..Sat); Monthly: CSV 1..31
    TIME                TIME                 NULL,                -- e.g. 09:00
    EXCLUDEDUSERIDS     NVARCHAR(MAX)        NULL,                -- CSV of UserIds excluded from this campaign; NULL/empty = all eligible
    COUPONCODE          NVARCHAR(50)         NULL,                -- per-campaign coupon merged into the email as @Model.CouponCode
    CHANNELS            NVARCHAR(50)         NULL,                -- CSV of channels: Email / WhatsApp / Email,WhatsApp (NULL = Email)
    SUBJECT             NVARCHAR(200)        NULL,                -- campaign email subject (overrides template name)
    HEADLINE            NVARCHAR(200)        NULL,                -- campaign headline -> @Model.Headline
    MESSAGE             NVARCHAR(MAX)        NULL,                -- campaign body text -> @Model.Message
    CTATEXT             NVARCHAR(50)         NULL,                -- campaign button label -> @Model.CtaText
    CTAURL              NVARCHAR(300)        NULL,                -- campaign button link -> @Model.CtaUrl
    ACTIVE              BIT                  NOT NULL DEFAULT 1,
    CREATEDAT           DATETIME             NOT NULL DEFAULT GETDATE(),
    UPDATEDAT           DATETIME             NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_EMAILSCHEDULE_TEMPLATE FOREIGN KEY (TEMPLATEID)
        REFERENCES [dbo].[SA_EMAILTEMPLATE](TEMPLATEID)
);
-- FK column (joins + template-delete check)
CREATE NONCLUSTERED INDEX [IX_SA_EMAILSCHEDULE_TEMPLATE]
    ON [dbo].[SA_EMAILSCHEDULE]([TEMPLATEID]);
-- Worker scan: WHERE ACTIVE = 1 AND SENDBY (= 1 / IN 2,3,4)
CREATE NONCLUSTERED INDEX [IX_SA_EMAILSCHEDULE_ACTIVE_SENDBY]
    ON [dbo].[SA_EMAILSCHEDULE]([ACTIVE], [SENDBY])
    INCLUDE ([TEMPLATEID], [TENANTID], [DAY], [TIME]);
PRINT 'Created SA_EMAILSCHEDULE';
GO

-- -----------------------------------------------------------------------------
-- SA_EMAILMASTER  — send queue
-- EMAILSP = JSON {"to":"..","name":"..","subject":"..", <merge fields>}
-- STATUS: 0=Pending, 1=Success, 2=Failed, 3=Retry
-- -----------------------------------------------------------------------------
CREATE TABLE [dbo].[SA_EMAILMASTER] (
    [ID]             BIGINT        IDENTITY(1,1) NOT NULL,
    [TENANTID]       BIGINT        NULL,          -- recipient tenant; passed to API for tenant-scoped template resolution
    [CHANNEL]        NVARCHAR(20)  NOT NULL DEFAULT 'Email',  -- Email | WhatsApp; one row per channel
    [TEMPLATEID]     BIGINT        NOT NULL,
    [EMAILSP]        NVARCHAR(MAX) NOT NULL,       -- JSON payload (recipient + merge fields incl. campaign content)
    [REQUESTTIME]    DATETIME      NOT NULL DEFAULT GETDATE(),
    [STATUS]         TINYINT       NOT NULL DEFAULT 0,
    [RETRY_COUNT]    INT           NOT NULL DEFAULT 0,
    [ERROR_MSG]      VARCHAR(500)  NULL,
    [CORRELATION_ID] VARCHAR(100)  NULL,
    CONSTRAINT [PK_SA_EMAILMASTER] PRIMARY KEY CLUSTERED ([ID] ASC)
);
-- Worker dequeue: WHERE STATUS = @s ORDER BY REQUESTTIME ASC (then filter by TEMPLATEID)
CREATE NONCLUSTERED INDEX [IX_SA_EMAILMASTER_STATUS]
    ON [dbo].[SA_EMAILMASTER]([STATUS], [REQUESTTIME]) INCLUDE ([TEMPLATEID], [TENANTID]);
-- Fire-once dedup: WHERE CORRELATION_ID = @cid
CREATE NONCLUSTERED INDEX [IX_SA_EMAILMASTER_CORRELATION]
    ON [dbo].[SA_EMAILMASTER]([CORRELATION_ID]);
PRINT 'Created SA_EMAILMASTER';
GO

-- NOTE: SA_EMAILQUEUE (an earlier per-schedule stats table) has been removed.
-- Queue stats now come live from SA_EMAILMASTER via SA_GetSchedulePendingStats.
-- The DROP at the top of this script clears it from existing installs.

-- -----------------------------------------------------------------------------
-- SA_SERVICECONFIG  — worker heartbeat
-- -----------------------------------------------------------------------------
CREATE TABLE [dbo].[SA_SERVICECONFIG] (
    SERVICEID   INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    SERVICENAME NVARCHAR(100)     NOT NULL UNIQUE,
    LAST_RUN    DATETIME          NULL
);
PRINT 'Created SA_SERVICECONFIG';
GO

PRINT 'Notification tables ready.';
GO
