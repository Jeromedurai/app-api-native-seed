USE [himalaya_db]
GO

-- ============================================================
-- CONSOLIDATED DATABASE INDEXES
-- ============================================================
-- All performance-optimized indexes for the himalaya_db schema.
-- Organized by table, matched to actual query patterns.
--
-- Index strategy:
--   1. Primary Lookup   — single-column FK / unique lookups
--   2. Composite        — multi-column filters
--   3. Filtered         — WHERE Active = 1 / IS NOT NULL
--   4. Covering         — INCLUDE columns for selected data
--   5. Sort-Optimized   — DESC keys for date-based ordering
--
-- Run AFTER 01.Schema.sql and BEFORE any data insertion.
-- Duplicate / redundant indexes have been removed.
-- ============================================================


-- ============================================================
-- USERS
-- ============================================================
-- Purpose: authentication, user lookups, admin queries

-- Unique login lookups
CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Email
    ON Users (Email)
    WHERE Active = 1 AND Email IS NOT NULL;

CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Phone
    ON Users (Phone)
    WHERE Active = 1 AND Phone IS NOT NULL;

-- Tenant + active (most common filter combination)
CREATE NONCLUSTERED INDEX IX_Users_TenantId_Active
    ON Users (TenantId, Active, UserId)
    WHERE TenantId IS NOT NULL;

-- Recent login tracking
CREATE NONCLUSTERED INDEX IX_Users_LastLogin
    ON Users (LastLogin DESC)
    WHERE Active = 1 AND LastLogin IS NOT NULL;

-- Registration / admin queries
CREATE NONCLUSTERED INDEX IX_Users_CreatedAt
    ON Users (CreatedAt DESC);

-- Account lockout queries
CREATE NONCLUSTERED INDEX IX_Users_AccountLocked
    ON Users (AccountLocked, LoginAttempts, LastLoginAttempt)
    WHERE Active = 1;

-- "Remember Me" auto-login token — OPTIMIZED: was a full table scan
CREATE NONCLUSTERED INDEX IX_Users_RememberMeToken
    ON Users (RememberMeToken)
    WHERE Active = 1 AND RememberMeToken IS NOT NULL;


-- ============================================================
-- ROLES
-- ============================================================
-- Purpose: role management and lookups

CREATE NONCLUSTERED INDEX IX_Roles_Active
    ON Roles (Active, RoleLevel, RoleId);


-- ============================================================
-- USER ROLES  (junction)
-- ============================================================
-- Purpose: user–role relationship queries

CREATE NONCLUSTERED INDEX IX_UserRoles_UserId
    ON UserRoles (UserId, Active);

CREATE NONCLUSTERED INDEX IX_UserRoles_RoleId
    ON UserRoles (RoleId, Active);

CREATE NONCLUSTERED INDEX IX_UserRoles_UserRole
    ON UserRoles (UserId, RoleId, Active);


-- ============================================================
-- PRODUCTS
-- ============================================================
-- Purpose: catalogue browsing, search, filtering, sorting

-- Base product queries (TenantId + Active — most common)
CREATE NONCLUSTERED INDEX IX_Products_TenantId_Active
    ON Products (TenantId, Active, ProductId)
    WHERE TenantId IS NOT NULL;

-- Product code lookup (unique per tenant)
CREATE NONCLUSTERED INDEX IX_Products_ProductCode
    ON Products (ProductCode, TenantId)
    WHERE Active = 1 AND ProductCode IS NOT NULL;

-- Category filtering
CREATE NONCLUSTERED INDEX IX_Products_Category_Active
    ON Products (Category, Active, Price, Rating DESC)
    WHERE Category IS NOT NULL;

-- SKU lookup
CREATE NONCLUSTERED INDEX IX_Products_SKU
    ON Products (SKU)
    WHERE Active = 1 AND SKU IS NOT NULL;

-- Full-text search covering index
-- OPTIMIZED: InStock in key + WHERE Active=1 — tight seek on in-stock listings
CREATE NONCLUSTERED INDEX IX_Products_Search
    ON Products (TenantId, Active, InStock)
    INCLUDE (ProductId, ProductName, ProductDescription, ProductCode,
             Price, Rating, Category, BestSeller, Quantity)
    WHERE Active = 1;

-- Price sorting / range filtering
CREATE NONCLUSTERED INDEX IX_Products_Price
    ON Products (TenantId, Active, Price ASC)
    INCLUDE (ProductId, ProductName, Category, Rating, BestSeller);

-- Rating sorting
CREATE NONCLUSTERED INDEX IX_Products_Rating
    ON Products (TenantId, Active, Rating DESC)
    WHERE Rating > 0;

-- Stock / inventory queries
CREATE NONCLUSTERED INDEX IX_Products_Stock
    ON Products (TenantId, Active, Quantity DESC);

-- Best-seller queries
CREATE NONCLUSTERED INDEX IX_Products_BestSeller
    ON Products (TenantId, Active, BestSeller DESC, UserBuyCount DESC)
    WHERE BestSeller = 1;

-- Trending / new arrivals
CREATE NONCLUSTERED INDEX IX_Products_Trending
    ON Products (TenantId, Active, Trending DESC, Created DESC);

-- Popularity sorting
CREATE NONCLUSTERED INDEX IX_Products_UserBuyCount
    ON Products (TenantId, Active, UserBuyCount DESC);

-- Name / display-order sorting
CREATE NONCLUSTERED INDEX IX_Products_OrderBy
    ON Products (TenantId, Active, OrderBy ASC, ProductName ASC);

-- Composite search-filter covering index
CREATE NONCLUSTERED INDEX IX_Products_SearchFilter
    ON Products (TenantId, Active, Category, BestSeller, Rating DESC)
    INCLUDE (ProductId, ProductName, Price, Quantity, UserBuyCount, Offer);


-- ============================================================
-- PRODUCT IMAGES
-- ============================================================
-- Purpose: image lookups and ordering

CREATE NONCLUSTERED INDEX IX_ProductImages_ProductId
    ON ProductImages (ProductId, Active, OrderBy ASC);

-- Main image lookup
CREATE NONCLUSTERED INDEX IX_ProductImages_Main
    ON ProductImages (ProductId, Main DESC, Active)
    WHERE Active = 1 AND Main = 1;


-- ============================================================
-- PRODUCT REVIEWS
-- ============================================================
-- Purpose: review display, rating calculations

-- All reviews for a product (approved, ordered by rating/date)
CREATE NONCLUSTERED INDEX IX_ProductReviews_ProductId
    ON ProductReviews (ProductId, IsApproved, Active, Rating DESC, CreatedAt DESC);

-- Reviews by user
CREATE NONCLUSTERED INDEX IX_ProductReviews_UserId
    ON ProductReviews (UserId, Active, CreatedAt DESC);

-- Prevent duplicate reviews (user + product)
CREATE NONCLUSTERED INDEX IX_ProductReviews_UserProduct
    ON ProductReviews (UserId, ProductId, Active);


-- ============================================================
-- PRODUCT WISHLIST
-- ============================================================
-- Purpose: wishlist queries

CREATE NONCLUSTERED INDEX IX_ProductWishList_UserId
    ON ProductWishList (UserId, Active, CreatedAt DESC);

CREATE NONCLUSTERED INDEX IX_ProductWishList_ProductId
    ON ProductWishList (ProductId, Active);


-- ============================================================
-- CATEGORIES
-- ============================================================
-- Purpose: category navigation and hierarchy

CREATE NONCLUSTERED INDEX IX_Categories_TenantId_Active
    ON Categories (TenantId, Active, OrderBy ASC)
    WHERE TenantId IS NOT NULL;

CREATE NONCLUSTERED INDEX IX_Categories_ParentId_Active
    ON Categories (ParentCategoryId, Active, OrderBy ASC)
    WHERE ParentCategoryId IS NOT NULL;

CREATE NONCLUSTERED INDEX IX_Categories_MenuId_Active
    ON Categories (MenuId, Active, OrderBy ASC)
    WHERE MenuId IS NOT NULL;


-- ============================================================
-- MENU MASTER
-- ============================================================
-- Purpose: navigation menu structure and ordering

CREATE NONCLUSTERED INDEX IX_MenuMaster_TenantId
    ON MenuMaster (TenantId, Active, OrderBy ASC)
    WHERE TenantId IS NOT NULL;


-- ============================================================
-- CART ITEMS
-- ============================================================
-- Purpose: shopping cart read / write operations

-- User cart fetch (covers UserId + TenantId filter)
CREATE NONCLUSTERED INDEX IX_CartItems_UserId_Active
    ON CartItems (UserId, TenantId, Active, AddedDate DESC)
    INCLUDE (ProductId, Quantity);

-- Cart-to-product JOIN — OPTIMIZED: covers join, eliminates key lookups
CREATE NONCLUSTERED INDEX IX_CartItems_ProductId
    ON CartItems (ProductId, Active)
    INCLUDE (UserId, Quantity, TenantId);

-- Guest cart (session-based)
CREATE NONCLUSTERED INDEX IX_CartItems_SessionId
    ON CartItems (SessionId, Active)
    WHERE SessionId IS NOT NULL;

-- Prevent duplicate cart entries, optimise quantity updates
CREATE NONCLUSTERED INDEX IX_CartItems_UserProduct
    ON CartItems (UserId, ProductId, Active);

-- Expired cart cleanup job
CREATE NONCLUSTERED INDEX IX_CartItems_Expiration
    ON CartItems (ExpiresAt ASC)
    WHERE ExpiresAt IS NOT NULL AND Active = 1;


-- ============================================================
-- ORDERS
-- ============================================================
-- Purpose: order queries, reporting, status tracking

-- "My Orders" page — user order history
CREATE NONCLUSTERED INDEX IX_Orders_UserId_Active
    ON Orders (UserId, Active, CreatedAt DESC);

-- Admin order list — OPTIMIZED: covers full list row, eliminates key lookups
CREATE NONCLUSTERED INDEX IX_Orders_TenantId_Active
    ON Orders (TenantId, Active, CreatedAt DESC)
    INCLUDE (OrderId, OrderNumber, UserId, TotalAmount,
             OrderStatus, PaymentStatus, ShippingAmount);

-- Order number lookup (unique)
CREATE UNIQUE NONCLUSTERED INDEX IX_Orders_OrderNumber
    ON Orders (OrderNumber)
    WHERE Active = 1;

-- Status + payment combined filter
CREATE NONCLUSTERED INDEX IX_Orders_StatusPayment
    ON Orders (OrderStatus, PaymentStatus, Active, CreatedAt DESC);

-- Date-range reporting
CREATE NONCLUSTERED INDEX IX_Orders_CreatedAt
    ON Orders (CreatedAt DESC, Active)
    INCLUDE (TotalAmount, OrderStatus, PaymentStatus, UserId, TenantId, OrderDate);

-- Guest checkout (session-based)
CREATE NONCLUSTERED INDEX IX_Orders_SessionId
    ON Orders (SessionId)
    WHERE SessionId IS NOT NULL;

-- Analytics / reporting composite
CREATE NONCLUSTERED INDEX IX_Orders_Analytics
    ON Orders (TenantId, OrderStatus, PaymentStatus, CreatedAt DESC)
    INCLUDE (OrderId, OrderNumber, UserId, TotalAmount);

-- User + status composite (order history filtering)
CREATE NONCLUSTERED INDEX IX_Orders_UserStatus
    ON Orders (UserId, OrderStatus, Active, CreatedAt DESC)
    INCLUDE (OrderId, OrderNumber, TotalAmount, PaymentStatus);


-- ============================================================
-- ORDER ITEMS
-- ============================================================
-- Purpose: order detail fetch, product sales tracking

-- Order detail page — OPTIMIZED: covers all line-item columns, eliminates N key lookups
CREATE NONCLUSTERED INDEX IX_OrderItems_OrderId
    ON OrderItems (OrderId, Active)
    INCLUDE (ProductId, ProductName, ProductImage, ProductCode,
             Price, Quantity, Total, DiscountAmount);

-- Sales / product reporting
CREATE NONCLUSTERED INDEX IX_OrderItems_ProductId
    ON OrderItems (ProductId, Active, CreatedAt DESC);


-- ============================================================
-- ORDER STATUS HISTORY
-- ============================================================
-- Purpose: order status timeline and audit trail

-- OPTIMIZED: covers status timeline page, eliminates key lookups
CREATE NONCLUSTERED INDEX IX_OrderStatusHistory_OrderId
    ON OrderStatusHistory (OrderId, ChangedAt DESC)
    INCLUDE (PreviousStatus, NewStatus, StatusNote);


-- ============================================================
-- ORDER TRACKING
-- ============================================================
-- Purpose: shipment tracking queries

CREATE NONCLUSTERED INDEX IX_OrderTracking_OrderId
    ON OrderTracking (OrderId, Active);

-- Carrier tracking number lookup
CREATE NONCLUSTERED INDEX IX_OrderTracking_TrackingNumber
    ON OrderTracking (TrackingNumber)
    WHERE TrackingNumber IS NOT NULL AND Active = 1;


-- ============================================================
-- ORDER REFUNDS
-- ============================================================
-- Purpose: refund queries and processing

CREATE NONCLUSTERED INDEX IX_OrderRefunds_OrderId
    ON OrderRefunds (OrderId, Active);

CREATE NONCLUSTERED INDEX IX_OrderRefunds_Status
    ON OrderRefunds (RefundStatus, RequestedAt DESC)
    WHERE Active = 1;


-- ============================================================
-- USER ADDRESSES
-- ============================================================
-- Purpose: checkout address dropdown, default address lookup

-- OPTIMIZED: covers address dropdown, eliminates key lookups
CREATE NONCLUSTERED INDEX IX_UserAddresses_UserId
    ON UserAddresses (UserId, Active, IsDefault DESC)
    INCLUDE (AddressType, Street, City, State, PostalCode, Country);

CREATE NONCLUSTERED INDEX IX_UserAddresses_AddressType
    ON UserAddresses (UserId, AddressType, Active);


-- ============================================================
-- USER ACTIVITY LOG
-- ============================================================
-- Purpose: activity timeline and audit queries

-- OPTIMIZED: covers timeline page, eliminates key lookups
CREATE NONCLUSTERED INDEX IX_UserActivityLog_UserId
    ON UserActivityLog (UserId, CreatedAt DESC)
    INCLUDE (ActivityType, ResourceType, ResourceId);

-- Activity type filtering
CREATE NONCLUSTERED INDEX IX_UserActivityLog_ActivityType
    ON UserActivityLog (ActivityType, CreatedAt DESC);

-- Resource-scoped queries (admin: "all actions on order #123")
CREATE NONCLUSTERED INDEX IX_UserActivityLog_ResourceType
    ON UserActivityLog (ResourceType, ResourceId, CreatedAt DESC)
    WHERE ResourceType IS NOT NULL;


-- ============================================================
-- RAZORPAY ORDERS
-- ============================================================
-- Purpose: payment order lookups and status tracking

CREATE UNIQUE NONCLUSTERED INDEX IX_RazorpayOrders_RazorpayOrderId
    ON RazorpayOrders (RazorpayOrderId)
    WHERE Active = 1;

CREATE NONCLUSTERED INDEX IX_RazorpayOrders_UserId
    ON RazorpayOrders (UserId)
    WHERE UserId IS NOT NULL;

CREATE NONCLUSTERED INDEX IX_RazorpayOrders_TenantId
    ON RazorpayOrders (TenantId)
    WHERE TenantId IS NOT NULL;

CREATE NONCLUSTERED INDEX IX_RazorpayOrders_OrderId
    ON RazorpayOrders (OrderId)
    WHERE OrderId IS NOT NULL;

CREATE NONCLUSTERED INDEX IX_RazorpayOrders_Status
    ON RazorpayOrders (Status, CreatedAt DESC);


-- ============================================================
-- RAZORPAY PAYMENTS
-- ============================================================
-- Purpose: payment verification and lookup

CREATE UNIQUE NONCLUSTERED INDEX IX_RazorpayPayments_RazorpayPaymentId
    ON RazorpayPayments (RazorpayPaymentId)
    WHERE Active = 1;

CREATE NONCLUSTERED INDEX IX_RazorpayPayments_UserId
    ON RazorpayPayments (UserId)
    WHERE UserId IS NOT NULL;

CREATE NONCLUSTERED INDEX IX_RazorpayPayments_TenantId
    ON RazorpayPayments (TenantId)
    WHERE TenantId IS NOT NULL;

CREATE NONCLUSTERED INDEX IX_RazorpayPayments_OrderId
    ON RazorpayPayments (OrderId)
    WHERE OrderId IS NOT NULL;

CREATE NONCLUSTERED INDEX IX_RazorpayPayments_RazorpayOrderId
    ON RazorpayPayments (RazorpayOrderId);

CREATE NONCLUSTERED INDEX IX_RazorpayPayments_Status
    ON RazorpayPayments (Status, CreatedAt DESC);


-- ============================================================
-- COUPONS
-- ============================================================
-- Purpose: coupon validation and admin lookups

CREATE UNIQUE NONCLUSTERED INDEX IX_Coupons_Code_TenantId
    ON Coupons (Code, TenantId);

CREATE NONCLUSTERED INDEX IX_Coupons_TenantId
    ON Coupons (TenantId, Active, StartDate, EndDate);


-- ============================================================
-- COUPON USAGE
-- ============================================================
-- Purpose: usage tracking and per-user limit validation

CREATE NONCLUSTERED INDEX IX_CouponUsage_CouponId
    ON CouponUsage (CouponId, UsedAt DESC);

CREATE NONCLUSTERED INDEX IX_CouponUsage_UserId
    ON CouponUsage (UserId, UsedAt DESC)
    WHERE UserId IS NOT NULL;

CREATE NONCLUSTERED INDEX IX_CouponUsage_OrderId
    ON CouponUsage (OrderId);

CREATE UNIQUE NONCLUSTERED INDEX IX_CouponUsage_CouponId_OrderId
    ON CouponUsage (CouponId, OrderId);


-- ============================================================
-- SHIPPING RATES
-- ============================================================
-- Purpose: shipping rate matrix lookups at checkout

CREATE NONCLUSTERED INDEX IX_ShippingRates_Tenant_Product
    ON ShippingRates (TenantId, ProductType, Active);

CREATE NONCLUSTERED INDEX IX_ShippingRates_State_Courier
    ON ShippingRates (StateCode, CourierType, Active)
    WHERE StateCode IS NOT NULL;


-- ============================================================
-- STATES
-- ============================================================
-- Purpose: state reference lookups (dropdowns, validation)

CREATE NONCLUSTERED INDEX IX_States_TenantId
    ON States (TenantId, Active, OrderBy ASC);

CREATE NONCLUSTERED INDEX IX_States_StateCode
    ON States (StateCode, CountryCode, Active)
    WHERE Active = 1;


-- ============================================================
-- PASSWORD RESET OTPS
-- ============================================================
-- Purpose: OTP validation and expired-OTP cleanup

CREATE NONCLUSTERED INDEX IX_PasswordResetOTPs_UserId
    ON PasswordResetOTPs (UserId, Used, ExpiresAt DESC);

CREATE NONCLUSTERED INDEX IX_PasswordResetOTPs_Email
    ON PasswordResetOTPs (Email, Used, ExpiresAt DESC);

-- Active OTP lookup (validation path only)
CREATE NONCLUSTERED INDEX IX_PasswordResetOTPs_OTP
    ON PasswordResetOTPs (UserId, OTP, ExpiresAt DESC)
    WHERE Used = 0;

GO

-- ============================================================
-- APP SETTINGS
-- ============================================================
-- Purpose: settings read on every page load
-- Note: UX_AppSettings_Key (unique) is defined inline in
--       03.Consolidate_StoreProcedure.sql — this adds a
--       separate active-filtered seek index for the common
--       tenant settings read pattern.

-- OPTIMIZED: every settings read becomes a single-row seek
CREATE NONCLUSTERED INDEX IX_AppSettings_TenantId_Key
    ON AppSettings (TenantId, SettingKey)
    INCLUDE (SettingValue)
    WHERE Active = 1;

GO

-- ============================================================
-- STOCK NOTIFICATIONS
-- ============================================================
-- Purpose: back-in-stock dispatch job
-- OPTIMIZED: filtered to un-notified rows only; Email INCLUDED
--            so the dispatch job needs no key lookup

CREATE NONCLUSTERED INDEX IX_StockNotifications_Pending
    ON [dbo].[StockNotifications] (ProductId, TenantId)
    INCLUDE (Email)
    WHERE Notified = 0;

GO
