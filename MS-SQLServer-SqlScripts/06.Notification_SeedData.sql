-- =============================================================================
-- Email Notification — SEED DATA (46 catalog templates)
-- Run AFTER 04.Notification_Tables.sql and 05.Notification_StoredProcedures.sql.
--
-- Rows are scoped to TENANTID = @TenantId (this is a single-tenant app, id 1), so
-- they are OWNED by the tenant and remain editable/deletable in the admin UI.
-- ACTIVE = 1. VIEWNAME points at a branded Razor view in Views/Content/<ViewName>.cshtml
-- (all share the _BrandedEmail layout). Idempotent + self-healing:
--   * inserts templates not already present for this tenant, and
--   * backfills VIEWNAME / CATEGORY / AUDIENCESP where still NULL (upgrades earlier
--     seeds without clobbering admin edits — COALESCE keeps any value already set).
--
-- AUDIENCESP: NULL = event-driven/transactional (app sends via API /send).
-- 'SA_Audience_*' = scheduled audience send (procs in 05.Notification_StoredProcedures.sql).
-- =============================================================================

DECLARE @TenantId BIGINT = 1;   -- this application's tenant

IF OBJECT_ID('tempdb..#Seed') IS NOT NULL DROP TABLE #Seed;
CREATE TABLE #Seed (
    TemplateName NVARCHAR(100),
    Category     NVARCHAR(50),
    AudienceSp   NVARCHAR(200),
    ViewName     NVARCHAR(100)
);

INSERT INTO #Seed (TemplateName, Category, AudienceSp, ViewName) VALUES
-- ===== Account / Auth =====
(N'Order Notification',             N'Orders',       NULL, N'OrderNotification'),
(N'Offer Notification',             N'Marketing',    N'SA_Audience_ActiveCustomers', N'OfferNotification'),
(N'Reset Password',                 N'Account',      NULL, N'ResetPassword'),
(N'Login',                          N'Account',      NULL, N'LoginAlert'),
(N'Change Password',                N'Account',      NULL, N'ChangePassword'),
(N'Sale Notification',              N'Marketing',    N'SA_Audience_ActiveCustomers', N'SaleNotification'),
(N'Lower Price Notification',       N'Marketing',    N'SA_Audience_ActiveCustomers', N'LowerPriceNotification'),
(N'User Password Reset',            N'Account',      NULL, N'UserPasswordReset'),
(N'Two-Factor Authentication Code', N'Account',      NULL, N'TwoFactorCode'),
-- ===== Orders =====
(N'Order Confirmed',                N'Orders',       NULL, N'OrderConfirmed'),
(N'Order Packed',                   N'Orders',       NULL, N'OrderPacked'),
(N'Order Shipped',                  N'Orders',       NULL, N'OrderShippedNotice'),
(N'Out for Delivery',               N'Orders',       NULL, N'OutForDelivery'),
(N'Order Delivered',                N'Orders',       NULL, N'OrderDeliveredNotice'),
(N'Order Cancelled',                N'Orders',       NULL, N'OrderCancelled'),
(N'Order Delayed',                  N'Orders',       NULL, N'OrderDelayed'),
(N'Return Requested',               N'Orders',       NULL, N'ReturnRequested'),
(N'Return Approved',                N'Orders',       NULL, N'ReturnApproved'),
(N'Invoice / Receipt',              N'Orders',       NULL, N'InvoiceReceipt'),
-- ===== Payments =====
(N'Payment Successful',             N'Payments',     NULL, N'PaymentSuccessful'),
(N'Payment Failed',                 N'Payments',     NULL, N'PaymentFailed'),
(N'Payment Pending',                N'Payments',     NULL, N'PaymentPending'),
(N'Refund Initiated',               N'Payments',     NULL, N'RefundInitiated'),
(N'Refund Completed',               N'Payments',     NULL, N'RefundCompleted'),
(N'Wallet Credited',                N'Payments',     NULL, N'WalletCredited'),
(N'EMI / Installment Reminder',     N'Payments',     NULL, N'EmiReminder'),
-- ===== Cart / Wishlist =====
(N'Abandoned Cart Reminder',        N'Cart',         N'SA_Audience_AbandonedCart', N'AbandonedCart'),
(N'Wishlist Item Back in Stock',    N'Cart',         NULL, N'WishlistBackInStock'),
(N'Wishlist Price Drop',            N'Cart',         NULL, N'WishlistPriceDrop'),
-- ===== Marketing =====
(N'New Product Launch',             N'Marketing',    N'SA_Audience_ActiveCustomers', N'NewProductLaunch'),
(N'Coupon / Voucher',               N'Marketing',    N'SA_Audience_ActiveCustomers', N'CouponVoucher'),
(N'Flash Sale',                     N'Marketing',    N'SA_Audience_ActiveCustomers', N'FlashSale'),
(N'Loyalty Points Earned',          N'Marketing',    NULL, N'LoyaltyPointsEarned'),
(N'Referral Reward',                N'Marketing',    NULL, N'ReferralReward'),
(N'Festive / Seasonal Offer',       N'Marketing',    N'SA_Audience_ActiveCustomers', N'FestiveOffer'),
(N'Birthday / Anniversary Offer',   N'Marketing',    N'SA_Audience_Birthday', N'BirthdayOffer'),
(N'Cashback Credited',              N'Marketing',    NULL, N'CashbackCredited'),
(N'Back in Stock Alert',            N'Marketing',    NULL, N'BackInStock'),
-- ===== Reviews / Feedback =====
(N'Product Review Request',         N'Reviews',      NULL, N'ReviewRequest'),
(N'Review Approved',                N'Reviews',      NULL, N'ReviewApproved'),
(N'Question Answered (Q&A)',        N'Reviews',      NULL, N'QuestionAnswered'),
-- ===== Subscription / Support =====
(N'Subscription Renewal Reminder',  N'Subscription', NULL, N'SubscriptionRenewal'),
(N'Subscription Expired',           N'Subscription', NULL, N'SubscriptionExpired'),
(N'Gift Card Received',             N'Subscription', NULL, N'GiftCardReceived'),
(N'Support Ticket Update',          N'Subscription', NULL, N'SupportTicketUpdate'),
(N'Newsletter',                     N'Marketing',    N'SA_Audience_Newsletter', N'Newsletter');

-- 1) Insert templates not yet present for this tenant
INSERT INTO [dbo].[SA_EMAILTEMPLATE]
    (TENANTID, TEMPLATENAME, DESCRIPTION, VIEWNAME, AUDIENCESP, CATEGORY, ACTIVE)
SELECT @TenantId, s.TemplateName, s.TemplateName, s.ViewName, s.AudienceSp, s.Category, 0
FROM #Seed s
WHERE NOT EXISTS (
    SELECT 1 FROM [dbo].[SA_EMAILTEMPLATE] t
    WHERE t.TEMPLATENAME = s.TemplateName AND t.TENANTID = @TenantId
);
DECLARE @inserted INT = @@ROWCOUNT;

-- 2) Backfill blanks on existing tenant rows (COALESCE preserves admin-set values)
UPDATE t
SET t.VIEWNAME   = COALESCE(t.VIEWNAME,   s.ViewName),
    t.CATEGORY   = COALESCE(t.CATEGORY,   s.Category),
    t.AUDIENCESP = COALESCE(t.AUDIENCESP, s.AudienceSp),
    t.UPDATEDAT  = GETDATE()
FROM [dbo].[SA_EMAILTEMPLATE] t
JOIN #Seed s ON s.TemplateName = t.TEMPLATENAME AND t.TENANTID = @TenantId;

DROP TABLE #Seed;
PRINT CONCAT('Seed complete (TenantId=', @TenantId, '). Inserted: ', @inserted, '. Existing rows backfilled.');
GO
