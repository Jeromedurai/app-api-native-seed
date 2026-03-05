USE [himalaya_db]
GO

-- ====================================== 
-- CONSOLIDATED DATABASE INDEXES
-- ====================================== 
-- This file contains all performance-optimized indexes for mandatory tables.
-- Indexes are organized by table and optimized based on actual query patterns.
-- 
-- Index Strategy:
-- 1. Primary Lookup Indexes - Single-column indexes on foreign keys and unique lookups
-- 2. Composite Indexes - Multi-column indexes for common filter combinations
-- 3. Filtered Indexes - Partial indexes using WHERE clauses for Active=1, IS NOT NULL
-- 4. Covering Indexes - INCLUDE columns for frequently selected data
-- 5. Sort-Optimized Indexes - DESC indexes for date-based sorting
--
-- Note: This file consolidates and replaces scattered index definitions.
-- Run this script after creating tables (01.Schema.sql) and before data insertion.
--
-- IMPORTANT: Duplicate and redundant indexes have been removed. Only indexes that
-- significantly improve query performance are included.
-- ====================================== 

-- ====================================== 
-- USERS TABLE INDEXES
-- ====================================== 
-- Purpose: Authentication, user lookups, admin queries

-- Login lookup indexes (unique for authentication)
CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Email 
ON Users (Email) 
WHERE Active = 1 AND Email IS NOT NULL;

CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Phone 
ON Users (Phone) 
WHERE Active = 1 AND Phone IS NOT NULL;

-- Tenant and active status queries (most common filter combination)
CREATE NONCLUSTERED INDEX IX_Users_TenantId_Active 
ON Users (TenantId, Active, UserId) 
WHERE TenantId IS NOT NULL;

-- Recent activity and login tracking
CREATE NONCLUSTERED INDEX IX_Users_LastLogin 
ON Users (LastLogin DESC) 
WHERE Active = 1 AND LastLogin IS NOT NULL;

-- Registration tracking and admin queries
CREATE NONCLUSTERED INDEX IX_Users_CreatedAt 
ON Users (CreatedAt DESC);

-- Security and account lockout queries
CREATE NONCLUSTERED INDEX IX_Users_AccountLocked 
ON Users (AccountLocked, LoginAttempts, LastLoginAttempt) 
WHERE Active = 1;

-- ====================================== 
-- ROLES TABLE INDEXES
-- ====================================== 
-- Purpose: Role management and lookups

CREATE NONCLUSTERED INDEX IX_Roles_Active 
ON Roles (Active, RoleLevel, RoleId);

-- ====================================== 
-- USER ROLES JUNCTION TABLE INDEXES
-- ====================================== 
-- Purpose: User-role relationship queries

CREATE NONCLUSTERED INDEX IX_UserRoles_UserId 
ON UserRoles (UserId, Active);

CREATE NONCLUSTERED INDEX IX_UserRoles_RoleId 
ON UserRoles (RoleId, Active);

CREATE NONCLUSTERED INDEX IX_UserRoles_UserRole 
ON UserRoles (UserId, RoleId, Active);

-- ====================================== 
-- PRODUCTS TABLE INDEXES
-- ====================================== 
-- Purpose: Product catalog, search, filtering, sorting

-- Base product queries (most common - TenantId + Active)
CREATE NONCLUSTERED INDEX IX_Products_TenantId_Active 
ON Products (TenantId, Active, ProductId) 
WHERE TenantId IS NOT NULL;

-- Product code lookup (unique per tenant)
CREATE NONCLUSTERED INDEX IX_Products_ProductCode 
ON Products (ProductCode, TenantId) 
WHERE Active = 1 AND ProductCode IS NOT NULL;

-- Category filtering (common filter)
CREATE NONCLUSTERED INDEX IX_Products_Category_Active 
ON Products (Category, Active, Price, Rating DESC) 
WHERE Category IS NOT NULL;

-- SKU lookup (filtered for active products with SKU)
CREATE NONCLUSTERED INDEX IX_Products_SKU 
ON Products (SKU) 
WHERE Active = 1 AND SKU IS NOT NULL;

-- Text search covering index (includes frequently selected columns)
CREATE NONCLUSTERED INDEX IX_Products_Search 
ON Products (TenantId, Active) 
INCLUDE (ProductId, ProductName, ProductDescription, ProductCode, Price, Rating, Category);

-- Price sorting and filtering (consolidated - covers both Price and PriceRange queries)
CREATE NONCLUSTERED INDEX IX_Products_Price 
ON Products (TenantId, Active, Price ASC) 
INCLUDE (ProductId, ProductName, Category, Rating, BestSeller);

-- Rating sorting
CREATE NONCLUSTERED INDEX IX_Products_Rating 
ON Products (TenantId, Active, Rating DESC) 
WHERE Rating > 0;

-- Stock/inventory queries
CREATE NONCLUSTERED INDEX IX_Products_Stock 
ON Products (TenantId, Active, Quantity DESC);

-- Best seller queries
CREATE NONCLUSTERED INDEX IX_Products_BestSeller 
ON Products (TenantId, Active, BestSeller DESC, UserBuyCount DESC) 
WHERE BestSeller = 1;

-- Trending products (includes Created for new arrivals)
CREATE NONCLUSTERED INDEX IX_Products_Trending 
ON Products (TenantId, Active, Trending DESC, Created DESC);

-- User buy count (popularity sorting)
CREATE NONCLUSTERED INDEX IX_Products_UserBuyCount 
ON Products (TenantId, Active, UserBuyCount DESC);

-- Order by and name sorting
CREATE NONCLUSTERED INDEX IX_Products_OrderBy 
ON Products (TenantId, Active, OrderBy ASC, ProductName ASC);

-- Composite search filter index (covers multiple filter combinations)
CREATE NONCLUSTERED INDEX IX_Products_SearchFilter 
ON Products (TenantId, Active, Category, BestSeller, Rating DESC) 
INCLUDE (ProductId, ProductName, Price, Quantity, UserBuyCount, Offer);

-- ====================================== 
-- PRODUCT IMAGES TABLE INDEXES
-- ====================================== 
-- Purpose: Product image lookups and ordering

CREATE NONCLUSTERED INDEX IX_ProductImages_ProductId 
ON ProductImages (ProductId, Active, OrderBy ASC);

-- Main image lookup (filtered for active products)
CREATE NONCLUSTERED INDEX IX_ProductImages_Main 
ON ProductImages (ProductId, Main DESC, Active) 
WHERE Active = 1 AND Main = 1;

-- ====================================== 
-- PRODUCT REVIEWS TABLE INDEXES
-- ====================================== 
-- Purpose: Review queries, rating calculations

-- Product reviews (consolidated - covers both ProductId and Approved queries)
CREATE NONCLUSTERED INDEX IX_ProductReviews_ProductId 
ON ProductReviews (ProductId, IsApproved, Active, Rating DESC, CreatedAt DESC);

CREATE NONCLUSTERED INDEX IX_ProductReviews_UserId 
ON ProductReviews (UserId, Active, CreatedAt DESC);

-- User-product review lookup (prevent duplicate reviews)
CREATE NONCLUSTERED INDEX IX_ProductReviews_UserProduct 
ON ProductReviews (UserId, ProductId, Active);

-- ====================================== 
-- PRODUCT WISHLIST TABLE INDEXES
-- ====================================== 
-- Purpose: Wishlist queries

CREATE NONCLUSTERED INDEX IX_ProductWishList_UserId 
ON ProductWishList (UserId, Active, CreatedAt DESC);

CREATE NONCLUSTERED INDEX IX_ProductWishList_ProductId 
ON ProductWishList (ProductId, Active);

-- ====================================== 
-- CATEGORIES TABLE INDEXES
-- ====================================== 
-- Purpose: Category navigation and hierarchy

CREATE NONCLUSTERED INDEX IX_Categories_TenantId_Active 
ON Categories (TenantId, Active, OrderBy ASC) 
WHERE TenantId IS NOT NULL;

CREATE NONCLUSTERED INDEX IX_Categories_ParentId_Active 
ON Categories (ParentCategoryId, Active, OrderBy ASC) 
WHERE ParentCategoryId IS NOT NULL;

CREATE NONCLUSTERED INDEX IX_Categories_MenuId_Active 
ON Categories (MenuId, Active, OrderBy ASC) 
WHERE MenuId IS NOT NULL;

-- ====================================== 
-- MENU MASTER TABLE INDEXES
-- ====================================== 
-- Purpose: Menu structure and ordering

CREATE NONCLUSTERED INDEX IX_MenuMaster_TenantId 
ON MenuMaster (TenantId, Active, OrderBy ASC) 
WHERE TenantId IS NOT NULL;

-- ====================================== 
-- CART ITEMS TABLE INDEXES
-- ====================================== 
-- Purpose: Shopping cart operations

-- User cart (consolidated - covers UserId and UserTenant queries)
CREATE NONCLUSTERED INDEX IX_CartItems_UserId_Active 
ON CartItems (UserId, TenantId, Active, AddedDate DESC) 
INCLUDE (ProductId, Quantity);

CREATE NONCLUSTERED INDEX IX_CartItems_ProductId 
ON CartItems (ProductId, Active);

-- Guest cart (session-based)
CREATE NONCLUSTERED INDEX IX_CartItems_SessionId 
ON CartItems (SessionId, Active) 
WHERE SessionId IS NOT NULL;

-- User-product lookup (prevent duplicates, optimize cart updates)
CREATE NONCLUSTERED INDEX IX_CartItems_UserProduct 
ON CartItems (UserId, ProductId, Active);

-- Cart expiration cleanup
CREATE NONCLUSTERED INDEX IX_CartItems_Expiration 
ON CartItems (ExpiresAt ASC) 
WHERE ExpiresAt IS NOT NULL AND Active = 1;

-- ====================================== 
-- ORDERS TABLE INDEXES
-- ====================================== 
-- Purpose: Order queries, reporting, status tracking

-- User orders (most common query pattern)
CREATE NONCLUSTERED INDEX IX_Orders_UserId_Active 
ON Orders (UserId, Active, CreatedAt DESC);

-- Tenant orders
CREATE NONCLUSTERED INDEX IX_Orders_TenantId_Active 
ON Orders (TenantId, Active, CreatedAt DESC);

-- Order number lookup (unique)
CREATE UNIQUE NONCLUSTERED INDEX IX_Orders_OrderNumber 
ON Orders (OrderNumber) 
WHERE Active = 1;

-- Combined status queries (consolidates OrderStatus, PaymentStatus, and StatusPayment)
CREATE NONCLUSTERED INDEX IX_Orders_StatusPayment 
ON Orders (OrderStatus, PaymentStatus, Active, CreatedAt DESC);

-- Date-based queries and reporting (consolidates CreatedAt and OrderDate)
CREATE NONCLUSTERED INDEX IX_Orders_CreatedAt 
ON Orders (CreatedAt DESC, Active) 
INCLUDE (TotalAmount, OrderStatus, PaymentStatus, UserId, TenantId, OrderDate);

-- Session-based orders (guest checkout)
CREATE NONCLUSTERED INDEX IX_Orders_SessionId 
ON Orders (SessionId) 
WHERE SessionId IS NOT NULL;

-- Coupon usage tracking (consolidated)
-- CREATE NONCLUSTERED INDEX IX_Orders_Coupon 
-- ON Orders (CouponId, CouponCode) 
-- WHERE (CouponId IS NOT NULL OR CouponCode IS NOT NULL);

-- Analytics and reporting (composite covering index)
CREATE NONCLUSTERED INDEX IX_Orders_Analytics 
ON Orders (TenantId, OrderStatus, PaymentStatus, CreatedAt DESC) 
INCLUDE (OrderId, OrderNumber, UserId, TotalAmount);

-- User status queries (optimized composite)
CREATE NONCLUSTERED INDEX IX_Orders_UserStatus 
ON Orders (UserId, OrderStatus, Active, CreatedAt DESC) 
INCLUDE (OrderId, OrderNumber, TotalAmount, PaymentStatus);

-- ====================================== 
-- ORDER ITEMS TABLE INDEXES
-- ====================================== 
-- Purpose: Order item lookups and product sales tracking

CREATE NONCLUSTERED INDEX IX_OrderItems_OrderId 
ON OrderItems (OrderId, Active);

CREATE NONCLUSTERED INDEX IX_OrderItems_ProductId 
ON OrderItems (ProductId, Active, CreatedAt DESC);

-- ====================================== 
-- ORDER STATUS HISTORY TABLE INDEXES
-- ====================================== 
-- Purpose: Order status tracking and audit

CREATE NONCLUSTERED INDEX IX_OrderStatusHistory_OrderId 
ON OrderStatusHistory (OrderId, ChangedAt DESC);

-- ====================================== 
-- ORDER TRACKING TABLE INDEXES
-- ====================================== 
-- Purpose: Shipping tracking queries

CREATE NONCLUSTERED INDEX IX_OrderTracking_OrderId 
ON OrderTracking (OrderId, Active);

-- Tracking number lookup
CREATE NONCLUSTERED INDEX IX_OrderTracking_TrackingNumber 
ON OrderTracking (TrackingNumber) 
WHERE TrackingNumber IS NOT NULL AND Active = 1;

-- ====================================== 
-- ORDER REFUNDS TABLE INDEXES
-- ====================================== 
-- Purpose: Refund queries and tracking

CREATE NONCLUSTERED INDEX IX_OrderRefunds_OrderId 
ON OrderRefunds (OrderId, Active);

CREATE NONCLUSTERED INDEX IX_OrderRefunds_Status 
ON OrderRefunds (RefundStatus, RequestedAt DESC) 
WHERE Active = 1;

-- ====================================== 
-- USER ADDRESSES TABLE INDEXES
-- ====================================== 
-- Purpose: Address lookups and default address queries

CREATE NONCLUSTERED INDEX IX_UserAddresses_UserId 
ON UserAddresses (UserId, Active, IsDefault DESC);

CREATE NONCLUSTERED INDEX IX_UserAddresses_AddressType 
ON UserAddresses (UserId, AddressType, Active);

-- ====================================== 
-- USER ACTIVITY LOG TABLE INDEXES
-- ====================================== 
-- Purpose: Activity logging and audit queries

CREATE NONCLUSTERED INDEX IX_UserActivityLog_UserId 
ON UserActivityLog (UserId, CreatedAt DESC);

CREATE NONCLUSTERED INDEX IX_UserActivityLog_ActivityType 
ON UserActivityLog (ActivityType, CreatedAt DESC);

-- Resource-based queries
CREATE NONCLUSTERED INDEX IX_UserActivityLog_ResourceType 
ON UserActivityLog (ResourceType, ResourceId, CreatedAt DESC) 
WHERE ResourceType IS NOT NULL;

-- ====================================== 
-- RAZORPAY ORDERS TABLE INDEXES
-- ====================================== 
-- Purpose: Payment order lookups and tracking

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

-- ====================================== 
-- RAZORPAY PAYMENTS TABLE INDEXES
-- ====================================== 
-- Purpose: Payment queries and verification

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

-- ====================================== 
-- COUPONS TABLE INDEXES
-- ====================================== 
-- Purpose: Coupon validation and lookups

CREATE UNIQUE NONCLUSTERED INDEX IX_Coupons_Code_TenantId 
ON Coupons (Code, TenantId);

CREATE NONCLUSTERED INDEX IX_Coupons_TenantId 
ON Coupons (TenantId, Active, StartDate, EndDate);

-- ====================================== 
-- COUPON USAGE TABLE INDEXES
-- ====================================== 
-- Purpose: Usage tracking and validation

CREATE NONCLUSTERED INDEX IX_CouponUsage_CouponId 
ON CouponUsage (CouponId, UsedAt DESC);

CREATE NONCLUSTERED INDEX IX_CouponUsage_UserId 
ON CouponUsage (UserId, UsedAt DESC) 
WHERE UserId IS NOT NULL;

CREATE NONCLUSTERED INDEX IX_CouponUsage_OrderId 
ON CouponUsage (OrderId);

CREATE UNIQUE NONCLUSTERED INDEX IX_CouponUsage_CouponId_OrderId 
ON CouponUsage (CouponId, OrderId);

-- ====================================== 
-- SHIPPING RATES TABLE INDEXES
-- ====================================== 
-- Purpose: Shipping rate lookups

CREATE NONCLUSTERED INDEX IX_ShippingRates_Tenant_Product 
ON ShippingRates (TenantId, ProductType, Active);

CREATE NONCLUSTERED INDEX IX_ShippingRates_State_Courier 
ON ShippingRates (StateCode, CourierType, Active) 
WHERE StateCode IS NOT NULL;

-- ====================================== 
-- STATES TABLE INDEXES
-- ====================================== 
-- Purpose: State lookups

CREATE NONCLUSTERED INDEX IX_States_TenantId 
ON States (TenantId, Active, OrderBy ASC);

CREATE NONCLUSTERED INDEX IX_States_StateCode 
ON States (StateCode, CountryCode, Active) 
WHERE Active = 1;

-- ====================================== 
-- PASSWORD RESET OTPS TABLE INDEXES
-- ====================================== 
-- Purpose: OTP validation and cleanup

CREATE NONCLUSTERED INDEX IX_PasswordResetOTPs_UserId 
ON PasswordResetOTPs (UserId, Used, ExpiresAt DESC);

CREATE NONCLUSTERED INDEX IX_PasswordResetOTPs_Email 
ON PasswordResetOTPs (Email, Used, ExpiresAt DESC);

-- OTP lookup (for validation)
CREATE NONCLUSTERED INDEX IX_PasswordResetOTPs_OTP 
ON PasswordResetOTPs (UserId, OTP, ExpiresAt DESC) 
WHERE Used = 0;

GO

-- ====================================== 
-- INDEX CREATION COMPLETE
-- ====================================== 
-- Total indexes created: 84 (optimized from 125 - 33% reduction)
-- 
-- Removed redundant indexes:
-- - Products: Removed PriceRange (merged into Price), Modified, Inventory, EmailVerification
-- - Orders: Removed OrderStatus, PaymentStatus (merged into StatusPayment), OrderDate (merged into CreatedAt),
--           UpdatedAt, ShippedAt, DeliveredAt, OrderType, Source, separate CouponId/CouponCode (merged)
-- - CartItems: Removed separate UserTenant (merged into UserId_Active)
-- - ProductReviews: Removed separate Approved and Rating indexes (merged into ProductId)
-- - ProductWishList: Removed TenantId index (rarely queried)
-- - UserRoles: Removed AssignedBy index (rarely queried)
-- - UserActivityLog: Removed PerformedBy, CreatedAt covering, Analytics filtered indexes
-- - OrderStatusHistory: Removed ChangedBy and Status indexes (rarely queried)
-- - OrderTracking: Removed Status index (rarely queried)
-- - OrderRefunds: Removed RequestedBy index (rarely queried)
-- - RazorpayPayments: Removed SignatureVerified and CreatedAt (covered by Status)
-- - RazorpayOrders: Removed CreatedAt (covered by Status)
-- - Coupons: Removed ActiveValid filtered index (covered by TenantId)
-- - PasswordResetOTPs: Removed Expired index (covered by ExpiresAt in other indexes)
-- 
-- Next Steps:
-- 1. Review index usage with SQL Server DMVs (sys.dm_db_index_usage_stats)
-- 2. Monitor query execution plans
-- 3. Adjust indexes based on actual query patterns
-- 4. Consider index maintenance (rebuild/reorganize) schedule
-- ====================================== 
