USE [himalaya_db]
GO

-- ======================================
-- DROP EXISTING TABLES
-- Order matters: child tables before parent tables (FK dependencies)
-- ======================================

IF OBJECT_ID('Users',               'U') IS NOT NULL DROP TABLE Users;
IF OBJECT_ID('UserActivityLog',     'U') IS NOT NULL DROP TABLE UserActivityLog;
IF OBJECT_ID('UserAddresses',       'U') IS NOT NULL DROP TABLE UserAddresses;
IF OBJECT_ID('OrderTracking',       'U') IS NOT NULL DROP TABLE OrderTracking;
IF OBJECT_ID('OrderStatusHistory',  'U') IS NOT NULL DROP TABLE OrderStatusHistory;
IF OBJECT_ID('OrderRefunds',        'U') IS NOT NULL DROP TABLE OrderRefunds;
IF OBJECT_ID('OrderItems',          'U') IS NOT NULL DROP TABLE OrderItems;
IF OBJECT_ID('Orders',              'U') IS NOT NULL DROP TABLE Orders;
IF OBJECT_ID('CartItems',           'U') IS NOT NULL DROP TABLE CartItems;
IF OBJECT_ID('ProductWishList',     'U') IS NOT NULL DROP TABLE ProductWishList;
IF OBJECT_ID('ProductReviews',      'U') IS NOT NULL DROP TABLE ProductReviews;
IF OBJECT_ID('ProductImages',       'U') IS NOT NULL DROP TABLE ProductImages;
IF OBJECT_ID('Products',            'U') IS NOT NULL DROP TABLE Products;
IF OBJECT_ID('Categories',          'U') IS NOT NULL DROP TABLE Categories;
IF OBJECT_ID('MenuMaster',          'U') IS NOT NULL DROP TABLE MenuMaster;
IF OBJECT_ID('UserRoles',           'U') IS NOT NULL DROP TABLE UserRoles;
IF OBJECT_ID('Roles',               'U') IS NOT NULL DROP TABLE Roles;
IF OBJECT_ID('Coupons',             'U') IS NOT NULL DROP TABLE Coupons;
IF OBJECT_ID('CouponUsage',         'U') IS NOT NULL DROP TABLE CouponUsage;
IF OBJECT_ID('RazorpayOrders',      'U') IS NOT NULL DROP TABLE RazorpayOrders;
IF OBJECT_ID('RazorpayPayments',    'U') IS NOT NULL DROP TABLE RazorpayPayments;
IF OBJECT_ID('ShippingRates',       'U') IS NOT NULL DROP TABLE ShippingRates;
IF OBJECT_ID('States',              'U') IS NOT NULL DROP TABLE States;
IF OBJECT_ID('PasswordResetOTPs',   'U') IS NOT NULL DROP TABLE PasswordResetOTPs;

-- Unused / future tables (commented out):
-- IF OBJECT_ID('ProductCategories',       'U') IS NOT NULL DROP TABLE ProductCategories;
-- IF OBJECT_ID('UserPreferences',         'U') IS NOT NULL DROP TABLE UserPreferences;
-- IF OBJECT_ID('UserBehaviorAnalytics',   'U') IS NOT NULL DROP TABLE UserBehaviorAnalytics;
-- IF OBJECT_ID('PasswordResetTokens',     'U') IS NOT NULL DROP TABLE PasswordResetTokens;
-- IF OBJECT_ID('OrderAnalytics',          'U') IS NOT NULL DROP TABLE OrderAnalytics;
-- IF OBJECT_ID('NotificationQueue',       'U') IS NOT NULL DROP TABLE NotificationQueue;
-- IF OBJECT_ID('UserCustomPermissions',   'U') IS NOT NULL DROP TABLE UserCustomPermissions;
-- IF OBJECT_ID('RolePermissions',         'U') IS NOT NULL DROP TABLE RolePermissions;
-- IF OBJECT_ID('UserTokens',              'U') IS NOT NULL DROP TABLE UserTokens;
-- IF OBJECT_ID('UserSessions',            'U') IS NOT NULL DROP TABLE UserSessions;
-- IF OBJECT_ID('UserNotifications',       'U') IS NOT NULL DROP TABLE UserNotifications;
-- IF OBJECT_ID('TenantConfig',            'U') IS NOT NULL DROP TABLE TenantConfig;

GO

-- ======================================
-- CORE SYSTEM TABLES
-- ======================================

-- Users — core user information, authentication, profile
CREATE TABLE Users (
    UserId                  BIGINT          IDENTITY(1,1)               NOT NULL,
    -- Authentication
    FirstName               NVARCHAR(100)                               NOT NULL,
    LastName                NVARCHAR(100)                               NOT NULL,
    Email                   NVARCHAR(255)                               NOT NULL,
    Phone                   NVARCHAR(20)                                    NULL,
    PasswordHash            NVARCHAR(255)                               NOT NULL,
    Salt                    NVARCHAR(255)                               NOT NULL,
    TenantId                BIGINT                                          NULL,
    RoleId                  BIGINT                                          NULL,
    Active                  BIT             DEFAULT 1                   NOT NULL,
    EmailVerified           BIT             DEFAULT 0                   NOT NULL,
    PhoneVerified           BIT             DEFAULT 0                   NOT NULL,
    LoginAttempts           INT             DEFAULT 0                   NOT NULL,
    AccountLocked           BIT             DEFAULT 0                   NOT NULL,
    LastLoginAttempt        DATETIME2(7)                                    NULL,
    LastLogin               DATETIME2(7)                                    NULL,
    LastLogout              DATETIME2(7)                                    NULL,
    CreatedAt               DATETIME2(7)    DEFAULT GETUTCDATE()        NOT NULL,
    UpdatedAt               DATETIME2(7)    DEFAULT GETUTCDATE()        NOT NULL,
    -- Extended Profile
    ProfilePicture          NVARCHAR(500)                                   NULL,
    DateOfBirth             DATE                                            NULL,
    Gender                  NVARCHAR(20)                                    NULL,
    Timezone                NVARCHAR(100)   DEFAULT 'UTC'                   NULL,
    Language                NVARCHAR(10)    DEFAULT 'en'                    NULL,
    Country                 NVARCHAR(100)                                   NULL,
    City                    NVARCHAR(100)                                   NULL,
    State                   NVARCHAR(100)                                   NULL,
    PostalCode              NVARCHAR(20)                                    NULL,
    Bio                     NVARCHAR(MAX)                                   NULL,
    Website                 NVARCHAR(255)                                   NULL,
    CompanyName             NVARCHAR(255)                                   NULL,
    JobTitle                NVARCHAR(255)                                   NULL,
    PreferredContactMethod  NVARCHAR(50)    DEFAULT 'Email'                 NULL,
    NotificationSettings    NVARCHAR(MAX)                                   NULL,  -- JSON
    PrivacySettings         NVARCHAR(MAX)                                   NULL,  -- JSON
    -- Security
    PasswordChangedAt       DATETIME2(7)                                    NULL,
    LastPasswordReset       DATETIME2(7)                                    NULL,
    RememberMeToken         NVARCHAR(255)                                   NULL,
    RememberMeExpiry        DATETIME2(7)                                    NULL,
    AgreeToTerms            BIT             DEFAULT 0                   NOT NULL,
    TermsAcceptedAt         DATETIME2(7)                                    NULL,

    CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (UserId)
);

GO

-- Roles — user role definitions and hierarchy
CREATE TABLE Roles (
    RoleId          BIGINT          IDENTITY(1,1)           NOT NULL,
    RoleName        NVARCHAR(50)                            NOT NULL,
    RoleDescription NVARCHAR(255)                               NULL,
    RoleLevel       INT             DEFAULT 1               NOT NULL,  -- Hierarchy level
    IsSystemRole    BIT             DEFAULT 0               NOT NULL,
    Active          BIT             DEFAULT 1               NOT NULL,
    CreatedAt       DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    UpdatedAt       DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    CreatedBy       BIGINT                                      NULL,
    UpdatedBy       BIGINT                                      NULL,

    CONSTRAINT PK_Roles PRIMARY KEY CLUSTERED (RoleId)
);

GO

-- UserRoles — junction table linking users to roles
CREATE TABLE UserRoles (
    UserRoleId  BIGINT          IDENTITY(1,1)           NOT NULL,
    UserId      BIGINT                                  NOT NULL,
    RoleId      BIGINT                                  NOT NULL,
    AssignedBy  BIGINT                                      NULL,
    AssignedAt  DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    ExpiresAt   DATETIME2(7)                                NULL,
    Active      BIT             DEFAULT 1               NOT NULL,
    CreatedAt   DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    UpdatedAt   DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,

    CONSTRAINT PK_UserRoles PRIMARY KEY CLUSTERED (UserRoleId)
);

GO

-- ======================================
-- NAVIGATION / CATALOGUE TABLES
-- ======================================

-- MenuMaster — top-level navigation menu items
CREATE TABLE MenuMaster (
    MenuId      BIGINT          IDENTITY(1,1)           NOT NULL,
    MenuName    NVARCHAR(255)                           NOT NULL,
    OrderBy     INT             DEFAULT 0               NOT NULL,
    Active      BIT             DEFAULT 1               NOT NULL,
    Image       NVARCHAR(500)                               NULL,
    SubMenu     BIT             DEFAULT 0               NOT NULL,
    TenantId    BIGINT                                      NULL,
    Created     DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    Modified    DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    CreatedBy   BIGINT                                      NULL,
    ModifiedBy  BIGINT                                      NULL,

    CONSTRAINT PK_MenuMaster PRIMARY KEY CLUSTERED (MenuId)
);

GO

-- Categories — product category hierarchy
CREATE TABLE Categories (
    CategoryId      BIGINT          IDENTITY(1,1)           NOT NULL,
    CategoryName    NVARCHAR(255)                           NOT NULL,
    Description     NVARCHAR(MAX)                               NULL,
    Active          BIT             DEFAULT 1               NOT NULL,
    ParentCategoryId BIGINT                                     NULL,
    OrderBy         INT             DEFAULT 0               NOT NULL,
    Icon            NVARCHAR(255)                               NULL,
    HasSubMenu      BIT             DEFAULT 0               NOT NULL,
    Link            NVARCHAR(500)                               NULL,
    TenantId        BIGINT                                      NULL,
    Menu            VARCHAR(50)                                 NULL,
    MenuId          BIGINT                                      NULL,
    Created         DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    Modified        DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    CreatedBy       BIGINT                                      NULL,
    ModifiedBy      BIGINT                                      NULL,

    CONSTRAINT PK_Categories PRIMARY KEY CLUSTERED (CategoryId)
);

GO

-- ======================================
-- PRODUCT TABLES
-- ======================================

-- Products — core product catalogue
CREATE TABLE Products (
    ProductId           BIGINT          IDENTITY(1,1)           NOT NULL,
    TenantId            BIGINT                                      NULL,
    -- Identity
    ProductName         NVARCHAR(255)                           NOT NULL,
    ProductDescription  NVARCHAR(500)                               NULL,
    ProductCode         NVARCHAR(100)                           NOT NULL,
    FullDescription     NVARCHAR(MAX)                               NULL,
    Specification       NVARCHAR(MAX)                               NULL,
    Story               NVARCHAR(MAX)                               NULL,
    Overview            NVARCHAR(MAX)                               NULL,
    LongDescription     NVARCHAR(MAX)                               NULL,
    -- Inventory
    PackQuantity        INT             DEFAULT 1               NOT NULL,
    Quantity            INT             DEFAULT 0               NOT NULL,
    Total               INT             DEFAULT 0               NOT NULL,
    InStock             BIT             DEFAULT 0               NOT NULL,
    MinStockLevel       INT             DEFAULT 0               NOT NULL,
    MaxStockLevel       INT                                         NULL,
    ReorderPoint        INT             DEFAULT 0               NOT NULL,
    SKU                 NVARCHAR(100)                               NULL,
    Barcode             NVARCHAR(100)                               NULL,
    Weight              DECIMAL(10,3)                               NULL,
    Dimensions          NVARCHAR(100)                               NULL,  -- L x W x H
    -- Pricing
    Price               DECIMAL(18,2)                           NOT NULL,
    CostPrice           DECIMAL(18,2)                               NULL,
    OriginalPrice       DECIMAL(18,2)                               NULL,
    DiscountPercentage  DECIMAL(5,2)    DEFAULT 0               NOT NULL,
    Offer               NVARCHAR(100)                               NULL,
    -- Classification
    Category            BIGINT                                      NULL,
    Rating              DECIMAL(3,2)    DEFAULT 0               NOT NULL,
    Active              BIT             DEFAULT 1               NOT NULL,
    Trending            INT             DEFAULT 0               NOT NULL,
    UserBuyCount        INT             DEFAULT 0               NOT NULL,
    BestSeller          BIT             DEFAULT 0               NOT NULL,
    [Return]            INT             DEFAULT 30              NOT NULL,  -- Return policy in days
    DeliveryDate        INT             DEFAULT 7               NOT NULL,  -- Estimated delivery days
    OrderBy             INT             DEFAULT 0               NOT NULL,
    UserId              BIGINT                                      NULL,
    -- SEO
    MetaTitle           NVARCHAR(255)                               NULL,
    MetaDescription     NVARCHAR(500)                               NULL,
    MetaKeywords        NVARCHAR(500)                               NULL,
    Slug                NVARCHAR(255)                               NULL,
    -- Timestamps
    Created             DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    Modified            DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    CreatedBy           BIGINT                                      NULL,
    ModifiedBy          BIGINT                                      NULL,
    DeletedAt           DATETIME2(7)                                NULL,
    DeletedBy           BIGINT                                      NULL,

    CONSTRAINT PK_Products PRIMARY KEY CLUSTERED (ProductId)
);

GO

-- ProductImages — product image files (binary + metadata)
CREATE TABLE ProductImages (
    ImageId         BIGINT          IDENTITY(1,1)           NOT NULL,
    ProductId       BIGINT                                  NOT NULL,
    ImageName       NVARCHAR(255)                           NOT NULL,
    ContentType     NVARCHAR(100)                           NOT NULL,
    FileSize        BIGINT                                  NOT NULL,
    ImageData       VARBINARY(MAX)                              NULL,
    LargeData       VARBINARY(MAX)                              NULL,
    ThumbnailData   VARBINARY(MAX)                              NULL,
    Poster          NVARCHAR(500)                               NULL,  -- URL / path
    Main            BIT             DEFAULT 0               NOT NULL,
    Active          BIT             DEFAULT 1               NOT NULL,
    OrderBy         INT             DEFAULT 0               NOT NULL,
    AltText         NVARCHAR(255)                               NULL,
    Caption         NVARCHAR(500)                               NULL,
    CreatedAt       DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    Modified        DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    CreatedBy       BIGINT                                      NULL,
    ModifiedBy      BIGINT                                      NULL,
    DeletedAt       DATETIME2(7)                                NULL,
    DeletedBy       BIGINT                                      NULL,

    CONSTRAINT PK_ProductImages PRIMARY KEY CLUSTERED (ImageId)
);

GO

-- ProductReviews — customer ratings and reviews
CREATE TABLE ProductReviews (
    ReviewId            BIGINT          IDENTITY(1,1)           NOT NULL,
    ProductId           BIGINT                                  NOT NULL,
    UserId              BIGINT                                  NOT NULL,
    TenantId            BIGINT          DEFAULT 1               NOT NULL,
    Rating              INT                                     NOT NULL,
    ReviewTitle         NVARCHAR(255)                               NULL,
    ReviewText          NVARCHAR(MAX)                               NULL,
    IsVerifiedPurchase  BIT             DEFAULT 0               NOT NULL,
    IsApproved          BIT             DEFAULT 0               NOT NULL,
    HelpfulCount        INT             DEFAULT 0               NOT NULL,
    Active              BIT             DEFAULT 1               NOT NULL,
    CreatedAt           DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    UpdatedAt           DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    ApprovedAt          DATETIME2(7)                                NULL,
    ApprovedBy          BIGINT                                      NULL,

    CONSTRAINT PK_ProductReviews PRIMARY KEY CLUSTERED (ReviewId)
);

GO

-- ProductWishList — user saved / favourited products
IF OBJECT_ID('ProductWishList', 'U') IS NOT NULL
    DROP TABLE ProductWishList;

GO

CREATE TABLE ProductWishList (
    WishListId  BIGINT          IDENTITY(1,1)           NOT NULL,
    UserId      BIGINT                                  NOT NULL,
    ProductId   BIGINT                                  NOT NULL,
    TenantId    BIGINT                                      NULL,
    Priority    INT             DEFAULT 0               NOT NULL,
    Notes       NVARCHAR(500)                               NULL,
    Active      BIT             DEFAULT 1               NOT NULL,
    CreatedAt   DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    UpdatedAt   DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,

    CONSTRAINT PK_ProductWishList     PRIMARY KEY CLUSTERED (WishListId),
    CONSTRAINT FK_ProductWishList_Users    FOREIGN KEY (UserId)    REFERENCES Users(UserId),
    CONSTRAINT FK_ProductWishList_Products FOREIGN KEY (ProductId) REFERENCES Products(ProductId) ON DELETE CASCADE,
    CONSTRAINT UQ_ProductWishList_UserProduct UNIQUE (UserId, ProductId)
);

GO

-- ======================================
-- CART TABLE
-- ======================================

-- CartItems — active shopping cart contents per user / session
CREATE TABLE CartItems (
    CartId      BIGINT          IDENTITY(1,1)           NOT NULL,
    UserId      BIGINT                                  NOT NULL,
    ProductId   BIGINT                                  NOT NULL,
    Quantity    INT                                     NOT NULL,
    TenantId    BIGINT                                      NULL,
    SessionId   NVARCHAR(255)                               NULL,
    Active      BIT             DEFAULT 1               NOT NULL,
    AddedDate   DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    UpdatedDate DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    ExpiresAt   DATETIME2(7)                                NULL,  -- Cart expiration

    CONSTRAINT PK_CartItems PRIMARY KEY CLUSTERED (CartId)
);

GO

-- ======================================
-- ORDER TABLES
-- ======================================

-- Orders — master order record
CREATE TABLE Orders (
    OrderId                 BIGINT          IDENTITY(1,1)               NOT NULL,
    -- Identity
    UserId                  BIGINT                                      NOT NULL,
    TenantId                BIGINT                                      NOT NULL,
    SessionId               NVARCHAR(255)                                   NULL,
    OrderType               NVARCHAR(50)    DEFAULT 'registered'        NOT NULL,
    OrderNumber             NVARCHAR(50)                                NOT NULL,
    -- Status
    OrderStatus             NVARCHAR(50)    DEFAULT 'pending'           NOT NULL,
    PaymentStatus           NVARCHAR(50)    DEFAULT 'pending'           NOT NULL,
    -- Financials
    TotalAmount             DECIMAL(18,2)                               NOT NULL,
    Subtotal                DECIMAL(18,2)                               NOT NULL,
    ShippingAmount          DECIMAL(18,2)   DEFAULT 0                   NOT NULL,
    TaxAmount               DECIMAL(18,2)   DEFAULT 0                   NOT NULL,
    DiscountAmount          DECIMAL(18,2)   DEFAULT 0                   NOT NULL,
    CurrencyCode            NVARCHAR(3)     DEFAULT 'INR'               NOT NULL,
    -- Details
    Notes                   NVARCHAR(1000)                                  NULL,
    SpecialInstructions     NVARCHAR(1000)                                  NULL,
    OrderDate               DATETIME2(7)                                    NULL,
    -- Address / Payment / Shipping (JSON blobs)
    ShippingAddress         NVARCHAR(MAX)                                   NULL,
    PaymentMethod           NVARCHAR(MAX)                                   NULL,
    ShippingMethod          NVARCHAR(MAX)                                   NULL,
    AppliedDiscount         NVARCHAR(MAX)                                   NULL,
    -- Coupon
    CouponId                BIGINT                                          NULL,
    CouponCode              NVARCHAR(50)                                    NULL,
    CouponDiscountAmount    DECIMAL(18,2)   DEFAULT 0                       NULL,
    -- Payment / Shipping references
    PaymentTransactionId    NVARCHAR(255)                                   NULL,
    ShippingTrackingNumber  NVARCHAR(255)                                   NULL,
    -- Tracking metadata
    Source                  NVARCHAR(50)    DEFAULT 'web'               NOT NULL,
    IpAddress               NVARCHAR(45)                                    NULL,
    UserAgent               NVARCHAR(500)                                   NULL,
    Referrer                NVARCHAR(500)                                   NULL,
    -- Timestamps
    CreatedAt               DATETIME2(7)    DEFAULT GETUTCDATE()        NOT NULL,
    UpdatedAt               DATETIME2(7)    DEFAULT GETUTCDATE()        NOT NULL,
    ShippedAt               DATETIME2(7)                                    NULL,
    DeliveredAt             DATETIME2(7)                                    NULL,
    CancelledAt             DATETIME2(7)                                    NULL,
    -- Cancellation
    CancelReason            NVARCHAR(500)                                   NULL,
    CancelledBy             BIGINT                                          NULL,
    -- System
    Active                  BIT             DEFAULT 1                   NOT NULL,
    CreatedBy               BIGINT                                          NULL,
    UpdatedBy               BIGINT                                          NULL,

    CONSTRAINT PK_Orders PRIMARY KEY CLUSTERED (OrderId)
);

GO

-- OrderItems — line items belonging to an order
CREATE TABLE OrderItems (
    OrderItemId     BIGINT          IDENTITY(1,1)           NOT NULL,
    OrderId         BIGINT                                  NOT NULL,
    ProductId       BIGINT                                  NOT NULL,
    ProductName     NVARCHAR(255)                           NOT NULL,
    ProductImage    NVARCHAR(500)                               NULL,
    ProductCode     NVARCHAR(100)                               NULL,
    Price           DECIMAL(18,2)                           NOT NULL,
    OriginalPrice   DECIMAL(18,2)                               NULL,
    Quantity        INT                                     NOT NULL,
    Total           DECIMAL(18,2)                           NOT NULL,
    DiscountAmount  DECIMAL(18,2)   DEFAULT 0               NOT NULL,
    TaxAmount       DECIMAL(18,2)   DEFAULT 0               NOT NULL,
    Active          BIT             DEFAULT 1               NOT NULL,
    CreatedAt       DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    UpdatedAt       DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,

    CONSTRAINT PK_OrderItems             PRIMARY KEY CLUSTERED (OrderItemId),
    CONSTRAINT CK_OrderItems_Price       CHECK (Price    >= 0),
    CONSTRAINT CK_OrderItems_Quantity    CHECK (Quantity  > 0),
    CONSTRAINT CK_OrderItems_Total       CHECK (Total    >= 0)
);

GO

-- OrderStatusHistory — audit trail of every status change
CREATE TABLE OrderStatusHistory (
    StatusHistoryId BIGINT          IDENTITY(1,1)           NOT NULL,
    OrderId         BIGINT                                  NOT NULL,
    PreviousStatus  NVARCHAR(50)                                NULL,
    NewStatus       NVARCHAR(50)                            NOT NULL,
    StatusNote      NVARCHAR(1000)                              NULL,
    ChangedBy       BIGINT                                      NULL,
    ChangedAt       DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    CreatedAt       DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,

    CONSTRAINT PK_OrderStatusHistory PRIMARY KEY CLUSTERED (StatusHistoryId)
);

GO

-- OrderTracking — shipment / carrier tracking records
CREATE TABLE OrderTracking (
    TrackingId          BIGINT          IDENTITY(1,1)           NOT NULL,
    OrderId             BIGINT                                  NOT NULL,
    TrackingNumber      NVARCHAR(100)                               NULL,
    Carrier             NVARCHAR(100)                               NULL,
    TrackingStatus      NVARCHAR(50)    DEFAULT 'Pending'       NOT NULL,
    EstimatedDelivery   DATETIME2(7)                                NULL,
    ActualDelivery      DATETIME2(7)                                NULL,
    TrackingUrl         NVARCHAR(500)                               NULL,
    ShippingCost        DECIMAL(18,2)                               NULL,
    Active              BIT             DEFAULT 1               NOT NULL,
    CreatedAt           DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    UpdatedAt           DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,

    CONSTRAINT PK_OrderTracking PRIMARY KEY CLUSTERED (TrackingId)
);

GO

-- OrderRefunds — refund requests and their processing state
CREATE TABLE OrderRefunds (
    RefundId            BIGINT          IDENTITY(1,1)           NOT NULL,
    OrderId             BIGINT                                  NOT NULL,
    RefundAmount        DECIMAL(18,2)                           NOT NULL,
    RefundReason        NVARCHAR(500)                               NULL,
    RefundStatus        NVARCHAR(50)    DEFAULT 'Pending'       NOT NULL,
    RefundMethod        NVARCHAR(100)                               NULL,
    RefundTransactionId NVARCHAR(255)                               NULL,
    RequestedBy         BIGINT                                  NOT NULL,
    RequestedAt         DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    ProcessedAt         DATETIME2(7)                                NULL,
    ProcessedBy         BIGINT                                      NULL,
    Active              BIT             DEFAULT 1               NOT NULL,
    CreatedAt           DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    UpdatedAt           DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,

    CONSTRAINT PK_OrderRefunds PRIMARY KEY CLUSTERED (RefundId)
);

GO

-- ======================================
-- ADDRESS TABLE
-- ======================================

-- UserAddresses — saved delivery / billing addresses per user
CREATE TABLE UserAddresses (
    AddressId   BIGINT          IDENTITY(1,1)           NOT NULL,
    UserId      BIGINT                                  NOT NULL,
    AddressType NVARCHAR(50)    DEFAULT 'Home'          NOT NULL,  -- Home, Work, Shipping, Billing
    Street      NVARCHAR(255)                           NOT NULL,
    City        NVARCHAR(100)                           NOT NULL,
    State       NVARCHAR(100)                           NOT NULL,
    PostalCode  NVARCHAR(20)                            NOT NULL,
    Country     NVARCHAR(100)                           NOT NULL,
    IsDefault   BIT             DEFAULT 0               NOT NULL,
    Active      BIT             DEFAULT 1               NOT NULL,
    CreatedAt   DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    UpdatedAt   DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,

    CONSTRAINT PK_UserAddresses PRIMARY KEY CLUSTERED (AddressId)
);

GO

-- ======================================
-- ACTIVITY LOG
-- ======================================

-- UserActivityLog — audit log of all user and admin actions
CREATE TABLE UserActivityLog (
    ActivityLogId           BIGINT          IDENTITY(1,1)           NOT NULL,
    UserId                  BIGINT                                  NOT NULL,
    ActivityType            NVARCHAR(100)                           NOT NULL,
    ActivityDescription     NVARCHAR(MAX)                               NULL,
    IpAddress               NVARCHAR(45)                                NULL,
    UserAgent               NVARCHAR(500)                               NULL,
    DeviceId                NVARCHAR(255)                               NULL,
    ResourceType            NVARCHAR(50)                                NULL,  -- Product, Order, User, etc.
    ResourceId              BIGINT                                      NULL,
    SessionId               NVARCHAR(255)                               NULL,
    PerformedBy             BIGINT                                      NULL,  -- For admin actions
    CreatedAt               DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,

    CONSTRAINT PK_UserActivityLog PRIMARY KEY CLUSTERED (ActivityLogId)
);

GO

-- ======================================
-- PAYMENT TABLES (RAZORPAY)
-- ======================================

-- RazorpayOrders — Razorpay checkout order records
CREATE TABLE RazorpayOrders (
    Id                  BIGINT          IDENTITY(1,1)           NOT NULL,
    RazorpayOrderId     NVARCHAR(255)                           NOT NULL,  -- starts with "order_"
    Amount              BIGINT                                  NOT NULL,  -- amount in paise
    Currency            NVARCHAR(3)     DEFAULT 'INR'           NOT NULL,
    Receipt             NVARCHAR(255)                               NULL,  -- unique receipt identifier
    Status              NVARCHAR(50)    DEFAULT 'created'       NOT NULL,  -- created, attempted, paid, failed
    UserId              BIGINT                                      NULL,
    TenantId            BIGINT                                      NULL,
    OrderId             BIGINT                                      NULL,  -- FK → Orders
    Notes               NVARCHAR(500)                               NULL,
    RazorpayResponse    NVARCHAR(MAX)                               NULL,  -- full Razorpay API response (JSON)
    CreatedAt           DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    UpdatedAt           DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    Active              BIT             DEFAULT 1               NOT NULL,

    CONSTRAINT PK_RazorpayOrders PRIMARY KEY CLUSTERED (Id)
);

GO

-- RazorpayPayments — Razorpay payment capture and verification records
CREATE TABLE RazorpayPayments (
    Id                      BIGINT          IDENTITY(1,1)           NOT NULL,
    RazorpayPaymentId       NVARCHAR(255)                           NOT NULL,  -- starts with "pay_"
    RazorpayOrderId         NVARCHAR(255)                           NOT NULL,  -- starts with "order_"
    Amount                  BIGINT                                  NOT NULL,  -- amount in paise
    Currency                NVARCHAR(3)     DEFAULT 'INR'           NOT NULL,
    Status                  NVARCHAR(50)    DEFAULT 'created'       NOT NULL,  -- created, authorized, captured, failed
    Signature               NVARCHAR(500)                           NOT NULL,  -- payment signature
    SignatureVerified        BIT             DEFAULT 0               NOT NULL,
    VerificationAttempts    INT             DEFAULT 0               NOT NULL,
    UserId                  BIGINT                                      NULL,
    TenantId                BIGINT                                      NULL,
    OrderId                 BIGINT                                      NULL,  -- FK → Orders
    RazorpayOrderRecordId   BIGINT                                      NULL,  -- FK → RazorpayOrders
    PaymentMethod           NVARCHAR(100)                               NULL,  -- card, netbanking, wallet, upi
    PaymentDescription      NVARCHAR(500)                               NULL,
    RazorpayResponse        NVARCHAR(MAX)                               NULL,  -- full Razorpay API response (JSON)
    CreatedAt               DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    UpdatedAt               DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    VerifiedAt              DATETIME2(7)                                NULL,
    Active                  BIT             DEFAULT 1               NOT NULL,

    CONSTRAINT PK_RazorpayPayments                      PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_RazorpayPayments_RazorpayPaymentId    UNIQUE (RazorpayPaymentId),
    CONSTRAINT CK_RazorpayPayments_Amount               CHECK (Amount > 0),
    CONSTRAINT CK_RazorpayPayments_Currency             CHECK (Currency IN ('INR', 'USD', 'EUR', 'GBP'))
);

GO

-- ======================================
-- SHIPPING TABLES
-- ======================================

-- States — reference list of Indian states / territories
CREATE TABLE States (
    StateId     BIGINT          IDENTITY(1,1)           NOT NULL,
    TenantId    BIGINT                                      NULL,  -- NULL = global
    StateCode   NVARCHAR(10)                            NOT NULL,  -- 'MH', 'KA', 'TN', etc.
    StateName   NVARCHAR(100)                           NOT NULL,  -- 'Maharashtra', 'Karnataka', etc.
    CountryCode NVARCHAR(10)    DEFAULT 'IN'            NOT NULL,
    Active      BIT             DEFAULT 1               NOT NULL,
    OrderBy     INT             DEFAULT 0               NOT NULL,
    CreatedAt   DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    UpdatedAt   DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,

    CONSTRAINT PK_States            PRIMARY KEY (StateId),
    CONSTRAINT UQ_States_Tenant_Code UNIQUE (TenantId, StateCode)
);

GO

-- ShippingRates — per-product-type, per-state, per-courier shipping charge matrix
CREATE TABLE ShippingRates (
    ShippingRateId          BIGINT          IDENTITY(1,1)   NOT NULL,
    TenantId                BIGINT                          NOT NULL,
    ProductType             NVARCHAR(50)                    NOT NULL,  -- 'Seed', 'Plant', 'Default'
    StateCode               NVARCHAR(10)                        NULL,  -- 'TN' = Tamil Nadu, NULL = all other states
    CourierType             NVARCHAR(50)                    NOT NULL,  -- 'Postal' or 'Other'
    -- Pricing
    BaseCharge              DECIMAL(18,2)                   NOT NULL,
    PerUnitCharge           DECIMAL(18,2)   DEFAULT 0           NULL,  -- charge per unit of quantity
    MinCharge               DECIMAL(18,2)                   NOT NULL,
    MaxCharge               DECIMAL(18,2)                       NULL,
    FreeShippingThreshold   DECIMAL(18,2)                       NULL,  -- order value for free shipping
    Active                  BIT             DEFAULT 1       NOT NULL,
    CreatedAt               DATETIME2(7)    DEFAULT GETUTCDATE() NOT NULL,
    UpdatedAt               DATETIME2(7)    DEFAULT GETUTCDATE() NOT NULL,

    CONSTRAINT PK_ShippingRates PRIMARY KEY (ShippingRateId),
    CONSTRAINT UQ_ShippingRates_Tenant_Product_State_Courier
        UNIQUE (TenantId, ProductType, StateCode, CourierType)
);

GO

-- ======================================
-- COUPON TABLES
-- ======================================

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Coupons')
BEGIN
    CREATE TABLE [dbo].[Coupons] (
        [CouponId]          BIGINT          IDENTITY(1,1)                   NOT NULL,
        [Code]              NVARCHAR(50)                                    NOT NULL,
        [Type]              NVARCHAR(20)                                    NOT NULL,
        [Value]             DECIMAL(18,2)                                   NOT NULL,
        [MinAmount]         DECIMAL(18,2)                                       NULL,
        [MaxDiscount]       DECIMAL(18,2)                                       NULL,
        [Description]       NVARCHAR(500)                                       NULL,
        [StartDate]         DATETIME2                                       NOT NULL,
        [EndDate]           DATETIME2                                       NOT NULL,
        [UsageLimit]        INT                                                 NULL,
        [UsageLimitPerUser] INT                                                 NULL,
        [UsageCount]        INT             DEFAULT 0                       NOT NULL,
        [Active]            BIT             DEFAULT 1                       NOT NULL,
        [TenantId]          BIGINT                                          NOT NULL,
        [CreatedBy]         BIGINT                                              NULL,
        [UpdatedBy]         BIGINT                                              NULL,
        [CreatedAt]         DATETIME2       DEFAULT GETUTCDATE()            NOT NULL,
        [UpdatedAt]         DATETIME2       DEFAULT GETUTCDATE()            NOT NULL,

        CONSTRAINT PK_Coupons               PRIMARY KEY CLUSTERED ([CouponId]),
        CONSTRAINT CK_Coupons_Type          CHECK ([Type]              IN ('percentage', 'fixed')),
        CONSTRAINT CK_Coupons_Value         CHECK ([Value]              > 0),
        CONSTRAINT CK_Coupons_MinAmount     CHECK ([MinAmount]         >= 0),
        CONSTRAINT CK_Coupons_MaxDiscount   CHECK ([MaxDiscount]       >= 0),
        CONSTRAINT CK_Coupons_UsageLimit    CHECK ([UsageLimit]         > 0),
        CONSTRAINT CK_Coupons_PerUser       CHECK ([UsageLimitPerUser]  > 0),
        CONSTRAINT CK_Coupons_UsageCount    CHECK ([UsageCount]        >= 0)
    );
    -- Indexes → see 02.Indexes.sql
END

GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CouponUsage')
BEGIN
    CREATE TABLE [dbo].[CouponUsage] (
        [UsageId]       BIGINT          IDENTITY(1,1)               NOT NULL,
        [CouponId]      BIGINT                                      NOT NULL,
        [OrderId]       BIGINT                                      NOT NULL,
        [UserId]        BIGINT                                          NULL,
        [DiscountAmount] DECIMAL(18,2)                              NOT NULL,
        [OrderAmount]   DECIMAL(18,2)                               NOT NULL,
        [UsedAt]        DATETIME2       DEFAULT GETUTCDATE()        NOT NULL,

        CONSTRAINT PK_CouponUsage           PRIMARY KEY CLUSTERED ([UsageId]),
        CONSTRAINT CK_CouponUsage_Discount  CHECK ([DiscountAmount] >= 0),
        CONSTRAINT CK_CouponUsage_Order     CHECK ([OrderAmount]    >= 0)
    );
    -- Indexes → see 02.Indexes.sql
    PRINT 'Table CouponUsage created successfully.';
END

GO

-- ======================================
-- SECURITY TABLES
-- ======================================

-- PasswordResetOTPs — one-time passwords for password reset flow
CREATE TABLE [dbo].[PasswordResetOTPs] (
    OtpId       BIGINT          IDENTITY(1,1)           NOT NULL,
    UserId      BIGINT                                  NOT NULL,
    Email       NVARCHAR(255)                           NOT NULL,
    OTP         NVARCHAR(6)                             NOT NULL,
    ExpiresAt   DATETIME2(7)                            NOT NULL,
    Used        BIT             DEFAULT 0               NOT NULL,
    Attempts    INT             DEFAULT 0               NOT NULL,
    CreatedAt   DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,
    UsedAt      DATETIME2(7)                                NULL,
    IpAddress   NVARCHAR(45)                                NULL,
    UserAgent   NVARCHAR(500)                               NULL,

    CONSTRAINT PK_PasswordResetOTPs         PRIMARY KEY CLUSTERED (OtpId),
    CONSTRAINT FK_PasswordResetOTPs_Users   FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

GO

-- ======================================
-- NOTIFICATION TABLES
-- ======================================

-- StockNotifications — back-in-stock email subscription
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'StockNotifications')
BEGIN
    CREATE TABLE [dbo].[StockNotifications] (
        [Id]        BIGINT          IDENTITY(1,1)               NOT NULL,
        [ProductId] BIGINT                                      NOT NULL,
        [TenantId]  BIGINT                                      NOT NULL,
        [Email]     NVARCHAR(255)                               NOT NULL,
        [Notified]  BIT             DEFAULT 0                   NOT NULL,
        [CreatedAt] DATETIME2(7)    DEFAULT GETUTCDATE()        NOT NULL,
        [NotifiedAt] DATETIME2(7)                                   NULL,

        CONSTRAINT PK_StockNotifications PRIMARY KEY CLUSTERED ([Id])
    );

END

GO

-- ======================================
-- CONTACT / SUPPORT TABLES
-- ======================================

-- ContactMessages — inbound messages from the contact form
IF OBJECT_ID('dbo.ContactMessages', 'U') IS NULL
BEGIN
    CREATE TABLE ContactMessages (
        Id          BIGINT          IDENTITY(1,1)           NOT NULL,
        UserId      BIGINT                                      NULL,
        TenantId    BIGINT                                      NULL,
        Name        NVARCHAR(100)                           NOT NULL,
        Email       NVARCHAR(256)                           NOT NULL,
        Phone       NVARCHAR(32)                                NULL,
        Subject     NVARCHAR(100)                               NULL,
        Message     NVARCHAR(2000)                          NOT NULL,
        Language    NVARCHAR(10)                                NULL,
        Source      NVARCHAR(100)                               NULL,
        CreatedAt   DATETIME2(7)    DEFAULT GETUTCDATE()    NOT NULL,

        CONSTRAINT PK_ContactMessages PRIMARY KEY CLUSTERED (Id)
    );
END;

GO

-- ======================================
-- NOTE: All non-inline indexes are defined in 02.Indexes.sql
-- ======================================
