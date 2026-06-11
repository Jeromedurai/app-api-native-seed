-- =============================================================================
-- SA_EMAILMASTER: Email send queue table
-- Run against himalaya_db after 04.Email_Notification_Setup.sql.
--
-- SA_EMAILTEMPLATE / SA_EMAILSCHEDULE  — admin config (read by API + worker)
-- SA_EMAILMASTER                       — email send queue (enqueued by app, dequeued by worker)
-- SA_SERVICECONFIG                     — worker heartbeat
--
-- STATUS: 0=Pending, 1=Success, 2=Failed, 3=Retry
-- EMAILSP format: JSON {"to":"...","name":"...","subject":"...",...}
-- =============================================================================

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SA_EMAILMASTER')
BEGIN
    CREATE TABLE [dbo].[SA_EMAILMASTER] (
        [ID]             BIGINT        IDENTITY(1,1) NOT NULL,
        [TEMPLATEID]     BIGINT        NOT NULL,
        [EMAILSP]        VARCHAR(500)  NOT NULL,
        [REQUESTTIME]    DATETIME      NOT NULL DEFAULT GETDATE(),
        [STATUS]         TINYINT       NOT NULL DEFAULT 0,
        [RETRY_COUNT]    INT           NOT NULL DEFAULT 0,
        [ERROR_MSG]      VARCHAR(500)  NULL,
        [CORRELATION_ID] VARCHAR(100)  NULL,
        CONSTRAINT [PK_SA_EMAILMASTER] PRIMARY KEY CLUSTERED ([ID] ASC)
    );
    CREATE NONCLUSTERED INDEX [IX_SA_EMAILMASTER_STATUS]
        ON [dbo].[SA_EMAILMASTER]([STATUS]) INCLUDE ([TEMPLATEID]);
    PRINT 'Created SA_EMAILMASTER';
END
ELSE
    PRINT 'SA_EMAILMASTER already exists — skipped.';
GO

PRINT 'SA_EMAILMASTER table script completed.';
GO
