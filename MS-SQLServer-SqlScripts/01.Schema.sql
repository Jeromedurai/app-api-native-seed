USE [himalaya_db]

-- Drop existing tables in correct order (foreign key dependencies)
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;
IF OBJECT_ID('UserActivityLog', 'U') IS NOT NULL DROP TABLE UserActivityLog;
IF OBJECT_ID('UserAddresses', 'U') IS NOT NULL DROP TABLE UserAddresses;
IF OBJECT_ID('OrderTracking', 'U') IS NOT NULL DROP TABLE OrderTracking;
IF OBJECT_ID('OrderStatusHistory', 'U') IS NOT NULL DROP TABLE OrderStatusHistory;
IF OBJECT_ID('OrderRefunds', 'U') IS NOT NULL DROP TABLE OrderRefunds;
IF OBJECT_ID('OrderItems', 'U') IS NOT NULL DROP TABLE OrderItems;
IF OBJECT_ID('Orders', 'U') IS NOT NULL DROP TABLE Orders;
IF OBJECT_ID('CartItems', 'U') IS NOT NULL DROP TABLE CartItems;
IF OBJECT_ID('ProductWishList', 'U') IS NOT NULL DROP TABLE ProductWishList;
IF OBJECT_ID('ProductReviews', 'U') IS NOT NULL DROP TABLE ProductReviews;
IF OBJECT_ID('ProductImages', 'U') IS NOT NULL DROP TABLE ProductImages;
IF OBJECT_ID('Products', 'U') IS NOT NULL DROP TABLE Products;
IF OBJECT_ID('Categories', 'U') IS NOT NULL DROP TABLE Categories;
IF OBJECT_ID('MenuMaster', 'U') IS NOT NULL DROP TABLE MenuMaster;
IF OBJECT_ID('UserRoles', 'U') IS NOT NULL DROP TABLE UserRoles;
IF OBJECT_ID('Roles', 'U') IS NOT NULL DROP TABLE Roles;
IF OBJECT_ID('Coupons', 'U') IS NOT NULL DROP TABLE Coupons;
IF OBJECT_ID('CouponUsage', 'U') IS NOT NULL DROP TABLE CouponUsage;
IF OBJECT_ID('RazorpayOrders', 'U') IS NOT NULL DROP TABLE RazorpayOrders;
IF OBJECT_ID('RazorpayPayments', 'U') IS NOT NULL DROP TABLE RazorpayPayments;
IF OBJECT_ID('ShippingRates', 'U') IS NOT NULL DROP TABLE ShippingRates;
IF OBJECT_ID('States', 'U') IS NOT NULL DROP TABLE States;
IF OBJECT_ID('PasswordResetOTPs', 'U') IS NOT NULL DROP TABLE PasswordResetOTPs;

-- IF OBJECT_ID('ProductCategories', 'U') IS NOT NULL DROP TABLE ProductCategories;
-- IF OBJECT_ID('UserPreferences', 'U') IS NOT NULL DROP TABLE UserPreferences;
-- IF OBJECT_ID('UserBehaviorAnalytics', 'U') IS NOT NULL DROP TABLE UserBehaviorAnalytics;
-- IF OBJECT_ID('PasswordResetTokens', 'U') IS NOT NULL DROP TABLE PasswordResetTokens;
-- IF OBJECT_ID('OrderAnalytics', 'U') IS NOT NULL DROP TABLE OrderAnalytics;
-- IF OBJECT_ID('NotificationQueue', 'U') IS NOT NULL DROP TABLE NotificationQueue;
-- IF OBJECT_ID('UserCustomPermissions', 'U') IS NOT NULL DROP TABLE UserCustomPermissions;
-- IF OBJECT_ID('RolePermissions', 'U') IS NOT NULL DROP TABLE RolePermissions;
-- IF OBJECT_ID('UserTokens', 'U') IS NOT NULL DROP TABLE UserTokens;
-- IF OBJECT_ID('UserSessions', 'U') IS NOT NULL DROP TABLE UserSessions;
-- IF OBJECT_ID('UserNotifications', 'U') IS NOT NULL DROP TABLE UserNotifications;

-- IF OBJECT_ID('TenantConfig', 'U') IS NOT NULL DROP TABLE TenantConfig;

-- ====================================== 
-- CORE SYSTEM TABLES
-- ====================================== 
-- Users Table - Core user information
CREATE TABLE Users (
	UserId BIGINT IDENTITY(1,1) NOT NULL,
	FirstName NVARCHAR(100) NOT NULL,
	LastName NVARCHAR(100) NOT NULL,
	Email NVARCHAR(255) NOT NULL,
	Phone NVARCHAR(20) NULL,
	PasswordHash NVARCHAR(255) NOT NULL,
	Salt NVARCHAR(255) NOT NULL,
	TenantId BIGINT NULL,
	RoleId BIGINT NULL,
	Active BIT DEFAULT 1 NOT NULL,
	EmailVerified BIT DEFAULT 0 NOT NULL,
	PhoneVerified BIT DEFAULT 0 NOT NULL,
	LoginAttempts INT DEFAULT 0 NOT NULL,
	AccountLocked BIT DEFAULT 0 NOT NULL,
	LastLoginAttempt DATETIME2(7) NULL,
	LastLogin DATETIME2(7) NULL,
	LastLogout DATETIME2(7) NULL,
	CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	UpdatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	-- Extended Profile Fields
	ProfilePicture NVARCHAR(500) NULL,
	DateOfBirth DATE NULL,
	Gender NVARCHAR(20) NULL,
	Timezone NVARCHAR(100) DEFAULT 'UTC' NULL,
	Language NVARCHAR(10) DEFAULT 'en' NULL,
	Country NVARCHAR(100) NULL,
	City NVARCHAR(100) NULL,
	State NVARCHAR(100) NULL,
	PostalCode NVARCHAR(20) NULL,
	Bio NVARCHAR(MAX) NULL,
	Website NVARCHAR(255) NULL,
	CompanyName NVARCHAR(255) NULL,
	JobTitle NVARCHAR(255) NULL,
	PreferredContactMethod NVARCHAR(50) DEFAULT 'Email' NULL,
	NotificationSettings NVARCHAR(MAX) NULL, -- JSON
	PrivacySettings NVARCHAR(MAX) NULL, -- JSON
	-- Security Fields
	PasswordChangedAt DATETIME2(7) NULL,
	LastPasswordReset DATETIME2(7) NULL,
	RememberMeToken NVARCHAR(255) NULL,
	RememberMeExpiry DATETIME2(7) NULL,
	AgreeToTerms BIT DEFAULT 0 NOT NULL,
	TermsAcceptedAt DATETIME2(7) NULL,
	CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (UserId)
);

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'StockNotifications')
BEGIN
    CREATE TABLE [dbo].[StockNotifications] (
        [Id]          INT            IDENTITY(1,1) PRIMARY KEY,
        [ProductId]   INT            NOT NULL,
        [TenantId]    INT            NOT NULL,
        [Email]       NVARCHAR(255)  NOT NULL,
        [Notified]    BIT            NOT NULL DEFAULT 0,
        [CreatedAt]   DATETIME       NOT NULL DEFAULT GETUTCDATE(),
        [NotifiedAt]  DATETIME       NULL
    );
    CREATE INDEX IX_StockNotifications_ProductId ON [dbo].[StockNotifications] ([ProductId], [Notified]);
END
GO

CREATE TABLE Roles (
	RoleId BIGINT IDENTITY(1,1) NOT NULL,
	RoleName NVARCHAR(50) NOT NULL,
	RoleDescription NVARCHAR(255) NULL,
	RoleLevel INT DEFAULT 1 NOT NULL, -- Hierarchy level
	IsSystemRole BIT DEFAULT 0 NOT NULL,
	Active BIT DEFAULT 1 NOT NULL,
	CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	UpdatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	CreatedBy BIGINT NULL,
	UpdatedBy BIGINT NULL,
	CONSTRAINT PK_Roles PRIMARY KEY CLUSTERED (RoleId)
);

-- Menu Master Table
CREATE TABLE MenuMaster (
	MenuId BIGINT IDENTITY(1,1) NOT NULL,
	MenuName NVARCHAR(255) NOT NULL,
	OrderBy INT DEFAULT 0 NOT NULL,
	Active BIT DEFAULT 1 NOT NULL,
	Image NVARCHAR(500) NULL,
	SubMenu BIT DEFAULT 0 NOT NULL,
	TenantId BIGINT NULL,
	Created DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	Modified DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	CreatedBy BIGINT NULL,
	ModifiedBy BIGINT NULL,
	CONSTRAINT PK_MenuMaster PRIMARY KEY CLUSTERED (MenuId)
);

-- Categories Table
CREATE TABLE Categories (
	CategoryId BIGINT IDENTITY(1,1) NOT NULL,
	CategoryName NVARCHAR(255) NOT NULL,
	Description NVARCHAR(MAX) NULL,
	Active BIT DEFAULT 1 NOT NULL,
	ParentCategoryId BIGINT NULL,
	OrderBy INT DEFAULT 0 NOT NULL,
	Icon NVARCHAR(255) NULL,
	HasSubMenu BIT DEFAULT 0 NOT NULL,
	Link NVARCHAR(500) NULL,
	TenantId BIGINT NULL,
	Menu VARCHAR(50) NULL, 
	MenuId BIGINT NULL,
	Created DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	Modified DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	CreatedBy BIGINT NULL,
	ModifiedBy BIGINT NULL,
	CONSTRAINT PK_Categories PRIMARY KEY CLUSTERED (CategoryId)
);

-- Products Table - Core product information
CREATE TABLE Products (
	ProductId BIGINT IDENTITY(1,1) NOT NULL,
	TenantId BIGINT NULL,
	ProductName NVARCHAR(255) NOT NULL,
	ProductDescription NVARCHAR(500) NULL,
	ProductCode NVARCHAR(100) NOT NULL,
	FullDescription NVARCHAR(MAX) NULL,
	Specification NVARCHAR(MAX) NULL,
	Story NVARCHAR(MAX) NULL,
	PackQuantity INT DEFAULT 1 NOT NULL,
	Quantity INT DEFAULT 0 NOT NULL,
	Total INT DEFAULT 0 NOT NULL,
	Price DECIMAL(18,2) NOT NULL,
	Category BIGINT NULL,
	Rating DECIMAL(3,2) DEFAULT 0 NOT NULL,
	Active BIT DEFAULT 1 NOT NULL,
	Trending INT DEFAULT 0 NOT NULL,
	UserBuyCount INT DEFAULT 0 NOT NULL,
	[Return] INT DEFAULT 30 NOT NULL, -- Return policy in days
	InStock BIT DEFAULT 0,
	BestSeller BIT DEFAULT 0 NOT NULL,
	DeliveryDate INT DEFAULT 7 NOT NULL, -- Delivery days
	Offer NVARCHAR(100) NULL,
	OrderBy INT DEFAULT 0 NOT NULL,
	UserId BIGINT NULL,
	Overview NVARCHAR(MAX) NULL,
	LongDescription NVARCHAR(MAX) NULL,
	-- SEO and Marketing
	MetaTitle NVARCHAR(255) NULL,
	MetaDescription NVARCHAR(500) NULL,
	MetaKeywords NVARCHAR(500) NULL,
	Slug NVARCHAR(255) NULL,
	-- Inventory Management
	SKU NVARCHAR(100) NULL,
	Barcode NVARCHAR(100) NULL,
	Weight DECIMAL(10,3) NULL,
	Dimensions NVARCHAR(100) NULL, -- L x W x H
	MinStockLevel INT DEFAULT 0 NOT NULL,
	MaxStockLevel INT NULL,
	ReorderPoint INT DEFAULT 0 NOT NULL,
	-- Pricing
	CostPrice DECIMAL(18,2) NULL,
	OriginalPrice DECIMAL(18,2) NULL,
	DiscountPercentage DECIMAL(5,2) DEFAULT 0 NOT NULL,
	-- Timestamps
	Created DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	Modified DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	CreatedBy BIGINT NULL,
	ModifiedBy BIGINT NULL,
	DeletedAt DATETIME2(7) NULL,
	DeletedBy BIGINT NULL,
	CONSTRAINT PK_Products PRIMARY KEY CLUSTERED (ProductId)
);

-- Product Images Table
CREATE TABLE ProductImages (
	ImageId BIGINT IDENTITY(1,1) NOT NULL,
	ProductId BIGINT NOT NULL,
	ImageName NVARCHAR(255) NOT NULL,
	ContentType NVARCHAR(100) NOT NULL,
	FileSize BIGINT NOT NULL,
	ImageData VARBINARY(MAX) NULL,
	LargeData VARBINARY(MAX) NULL,
	ThumbnailData VARBINARY(MAX) NULL,
	Poster NVARCHAR(500) NULL, -- URL/Path
	Main BIT DEFAULT 0 NOT NULL,
	Active BIT DEFAULT 1 NOT NULL,
	OrderBy INT DEFAULT 0 NOT NULL,
	AltText NVARCHAR(255) NULL,
	Caption NVARCHAR(500) NULL,
	CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	Modified DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	CreatedBy BIGINT NULL,
	ModifiedBy BIGINT NULL,
	DeletedAt DATETIME2(7) NULL,
	DeletedBy BIGINT NULL,
	CONSTRAINT PK_ProductImages PRIMARY KEY CLUSTERED (ImageId)
);

-- Product Reviews Table
CREATE TABLE ProductReviews (
	ReviewId BIGINT IDENTITY(1,1) NOT NULL,
	ProductId BIGINT NOT NULL,
	UserId BIGINT NOT NULL,
	TenantId BIGINT NOT NULL DEFAULT 1,
	Rating INT NOT NULL,
	ReviewTitle NVARCHAR(255) NULL,
	ReviewText NVARCHAR(MAX) NULL,
	IsVerifiedPurchase BIT DEFAULT 0 NOT NULL,
	IsApproved BIT DEFAULT 0 NOT NULL,
	HelpfulCount INT DEFAULT 0 NOT NULL,
	Active BIT DEFAULT 1 NOT NULL,
	CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	UpdatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	ApprovedAt DATETIME2(7) NULL,
	ApprovedBy BIGINT NULL,
	CONSTRAINT PK_ProductReviews PRIMARY KEY CLUSTERED (ReviewId)
);

-- Product Wishlist Table
IF OBJECT_ID('ProductWishList', 'U') IS NOT NULL 
	DROP TABLE ProductWishList;
GO

CREATE TABLE ProductWishList (
	WishListId BIGINT IDENTITY(1,1) NOT NULL,
	UserId BIGINT NOT NULL,
	ProductId BIGINT NOT NULL,
	TenantId BIGINT NULL,
	Priority INT DEFAULT 0 NOT NULL,
	Notes NVARCHAR(500) NULL,
	Active BIT DEFAULT 1 NOT NULL,
	CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	UpdatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	CONSTRAINT PK_ProductWishList PRIMARY KEY CLUSTERED (WishListId),
	CONSTRAINT FK_ProductWishList_Users FOREIGN KEY (UserId) REFERENCES Users(UserId),
	CONSTRAINT FK_ProductWishList_Products FOREIGN KEY (ProductId) REFERENCES Products(ProductId) ON DELETE CASCADE,
	CONSTRAINT UQ_ProductWishList_UserProduct UNIQUE (UserId, ProductId)
);
GO 

-- Cart Items Table
CREATE TABLE CartItems (
	CartId BIGINT IDENTITY(1,1) NOT NULL,
	UserId BIGINT NOT NULL,
	ProductId BIGINT NOT NULL,
	Quantity INT NOT NULL,
	TenantId BIGINT NULL,
	SessionId NVARCHAR(255) NULL,
	Active BIT DEFAULT 1 NOT NULL,
	AddedDate DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	UpdatedDate DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	ExpiresAt DATETIME2(7) NULL, -- Cart expiration
	CONSTRAINT PK_CartItems PRIMARY KEY CLUSTERED (CartId)
);

CREATE TABLE Orders (
    -- Primary Key
    OrderId BIGINT IDENTITY(1,1) NOT NULL,
    
    -- User and Tenant Information
    UserId BIGINT NOT NULL,
    TenantId BIGINT NOT NULL,
    SessionId NVARCHAR(255) NULL,
    OrderType NVARCHAR(50) DEFAULT 'registered' NOT NULL,
    
    -- Order Identification
    OrderNumber NVARCHAR(50) NOT NULL,
    
    -- Order Status
    OrderStatus NVARCHAR(50) DEFAULT 'pending' NOT NULL,
    PaymentStatus NVARCHAR(50) DEFAULT 'pending' NOT NULL,
    
    -- Financial Information
    TotalAmount DECIMAL(18,2) NOT NULL,
    Subtotal DECIMAL(18,2) NOT NULL,
    ShippingAmount DECIMAL(18,2) DEFAULT 0 NOT NULL,
    TaxAmount DECIMAL(18,2) DEFAULT 0 NOT NULL,
    DiscountAmount DECIMAL(18,2) DEFAULT 0 NOT NULL,
    CurrencyCode NVARCHAR(3) DEFAULT 'INR' NOT NULL,
    
    -- Order Details
    Notes NVARCHAR(1000) NULL,
    SpecialInstructions NVARCHAR(1000) NULL,
    OrderDate DATETIME2(7) NULL,
    
    -- Address Information (JSON)
    ShippingAddress NVARCHAR(MAX) NULL,
    
    -- Payment and Shipping Information (JSON)
    PaymentMethod NVARCHAR(MAX) NULL,
    ShippingMethod NVARCHAR(MAX) NULL,
    AppliedDiscount NVARCHAR(MAX) NULL,
    
    -- Coupon Information
    CouponId BIGINT NULL,
    CouponCode NVARCHAR(50) NULL,
    CouponDiscountAmount DECIMAL(18,2) DEFAULT 0 NULL,
    
    -- Payment Transaction Details
    PaymentTransactionId NVARCHAR(255) NULL,
    ShippingTrackingNumber NVARCHAR(255) NULL,
    
    -- Tracking Information
    Source NVARCHAR(50) DEFAULT 'web' NOT NULL,
    IpAddress NVARCHAR(45) NULL,
    UserAgent NVARCHAR(500) NULL,
    Referrer NVARCHAR(500) NULL,
    
    -- Important Timestamps
    CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
    UpdatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
    ShippedAt DATETIME2(7) NULL,
    DeliveredAt DATETIME2(7) NULL,
    CancelledAt DATETIME2(7) NULL,
    
    -- Cancellation Information
    CancelReason NVARCHAR(500) NULL,
    CancelledBy BIGINT NULL,
    
    -- System Fields
    Active BIT DEFAULT 1 NOT NULL,
    CreatedBy BIGINT NULL,
    UpdatedBy BIGINT NULL,
    
    -- Primary Key Constraint
    CONSTRAINT PK_Orders PRIMARY KEY CLUSTERED (OrderId)
);

-- Order Items Table
CREATE TABLE OrderItems (
    OrderItemId BIGINT IDENTITY(1,1) NOT NULL,
    OrderId BIGINT NOT NULL,
    ProductId BIGINT NOT NULL,
    ProductName NVARCHAR(255) NOT NULL,
    ProductImage NVARCHAR(500) NULL,
    ProductCode NVARCHAR(100) NULL,
    Price DECIMAL(18,2) NOT NULL,
    OriginalPrice DECIMAL(18,2) NULL,
    Quantity INT NOT NULL,
    Total DECIMAL(18,2) NOT NULL,
    DiscountAmount DECIMAL(18,2) DEFAULT 0 NOT NULL,
    TaxAmount DECIMAL(18,2) DEFAULT 0 NOT NULL,
    Active BIT DEFAULT 1 NOT NULL,
    CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
    UpdatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
    
    CONSTRAINT PK_OrderItems PRIMARY KEY CLUSTERED (OrderItemId),
    CONSTRAINT CK_OrderItems_Price CHECK (Price >= 0),
    CONSTRAINT CK_OrderItems_Quantity CHECK (Quantity > 0),
    CONSTRAINT CK_OrderItems_Total CHECK (Total >= 0)
);


CREATE TABLE UserAddresses (
    AddressId BIGINT IDENTITY(1,1) NOT NULL,
    UserId BIGINT NOT NULL,
    AddressType NVARCHAR(50) DEFAULT 'Home' NOT NULL, -- Home, Work, Shipping, Billing
    Street NVARCHAR(255) NOT NULL,
    City NVARCHAR(100) NOT NULL,
    State NVARCHAR(100) NOT NULL,
    PostalCode NVARCHAR(20) NOT NULL,
    Country NVARCHAR(100) NOT NULL,
    IsDefault BIT DEFAULT 0 NOT NULL,
    Active BIT DEFAULT 1 NOT NULL,
    CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
    UpdatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
    
    CONSTRAINT PK_UserAddresses PRIMARY KEY CLUSTERED (AddressId)
);

-- Orders table indexes
-- Note: All indexes have been moved to 02.Indexes.sql for centralized management

-- OrderItems table indexes
-- Note: All indexes have been moved to 02.Indexes.sql for centralized management

-- UserAddresses table indexes
-- Note: All indexes have been moved to 02.Indexes.sql for centralized management

-- Order Status History Table
CREATE TABLE OrderStatusHistory (
	StatusHistoryId BIGINT IDENTITY(1,1) NOT NULL,
	OrderId BIGINT NOT NULL,
	PreviousStatus NVARCHAR(50) NULL,
	NewStatus NVARCHAR(50) NOT NULL,
	StatusNote NVARCHAR(1000) NULL,
	ChangedBy BIGINT NULL,
	ChangedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	CONSTRAINT PK_OrderStatusHistory PRIMARY KEY CLUSTERED (StatusHistoryId)
);

-- Order Tracking Table
CREATE TABLE OrderTracking (
	TrackingId BIGINT IDENTITY(1,1) NOT NULL,
	OrderId BIGINT NOT NULL,
	TrackingNumber NVARCHAR(100) NULL,
	Carrier NVARCHAR(100) NULL,
	TrackingStatus NVARCHAR(50) DEFAULT 'Pending' NOT NULL,
	EstimatedDelivery DATETIME2(7) NULL,
	ActualDelivery DATETIME2(7) NULL,
	TrackingUrl NVARCHAR(500) NULL,
	ShippingCost DECIMAL(18,2) NULL,
	Active BIT DEFAULT 1 NOT NULL,
	CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	UpdatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	CONSTRAINT PK_OrderTracking PRIMARY KEY CLUSTERED (TrackingId)
);

-- Order Refunds Table
CREATE TABLE OrderRefunds (
	RefundId BIGINT IDENTITY(1,1) NOT NULL,
	OrderId BIGINT NOT NULL,
	RefundAmount DECIMAL(18,2) NOT NULL,
	RefundReason NVARCHAR(500) NULL,
	RefundStatus NVARCHAR(50) DEFAULT 'Pending' NOT NULL,
	RefundMethod NVARCHAR(100) NULL,
	RefundTransactionId NVARCHAR(255) NULL,
	RequestedBy BIGINT NOT NULL,
	RequestedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	ProcessedAt DATETIME2(7) NULL,
	ProcessedBy BIGINT NULL,
	Active BIT DEFAULT 1 NOT NULL,
	CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	UpdatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	CONSTRAINT PK_OrderRefunds PRIMARY KEY CLUSTERED (RefundId)
);

-- User Activity Log Table
CREATE TABLE UserActivityLog (
	ActivityLogId BIGINT IDENTITY(1,1) NOT NULL,
	UserId BIGINT NOT NULL,
	ActivityType NVARCHAR(100) NOT NULL,
	ActivityDescription NVARCHAR(MAX) NULL,
	IpAddress NVARCHAR(45) NULL,
	UserAgent NVARCHAR(500) NULL,
	DeviceId NVARCHAR(255) NULL,
	ResourceType NVARCHAR(50) NULL, -- Product, Order, User, etc.
	ResourceId BIGINT NULL,
	SessionId NVARCHAR(255) NULL,
	PerformedBy BIGINT NULL, -- For admin actions
	CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	CONSTRAINT PK_UserActivityLog PRIMARY KEY CLUSTERED (ActivityLogId)
);

-- User Roles Junction Table
CREATE TABLE UserRoles (
	UserRoleId BIGINT IDENTITY(1,1) NOT NULL,
	UserId BIGINT NOT NULL,
	RoleId BIGINT NOT NULL,
	AssignedBy BIGINT NULL,
	AssignedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	ExpiresAt DATETIME2(7) NULL,
	Active BIT DEFAULT 1 NOT NULL,
	CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	UpdatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
	CONSTRAINT PK_UserRoles PRIMARY KEY CLUSTERED (UserRoleId)
);

CREATE TABLE RazorpayOrders (
    -- Primary Key
    Id BIGINT IDENTITY(1,1) NOT NULL,
    
    -- Razorpay Order Information
    RazorpayOrderId NVARCHAR(255) NOT NULL, -- Razorpay order ID (starts with "order_")
    
    -- Amount and Currency
    Amount BIGINT NOT NULL, -- Amount in paise
    Currency NVARCHAR(3) DEFAULT 'INR' NOT NULL,
    
    -- Receipt Information
    Receipt NVARCHAR(255) NULL, -- Unique receipt identifier
    
    -- Order Status
    Status NVARCHAR(50) DEFAULT 'created' NOT NULL, -- created, attempted, paid, failed, etc.
    
    -- User and Tenant Information (Optional)
    UserId BIGINT NULL,
    TenantId BIGINT NULL,
    
    -- Link to System Order (Optional)
    OrderId BIGINT NULL, -- Link to Orders table
    
    -- Additional Information
    Notes NVARCHAR(500) NULL,
    
    -- Razorpay Response Data (JSON)
    RazorpayResponse NVARCHAR(MAX) NULL, -- Full Razorpay API response
    
    -- Timestamps
    CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
    UpdatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
    
    -- System Fields
    Active BIT DEFAULT 1 NOT NULL,
    
    -- Primary Key Constraint
    CONSTRAINT PK_RazorpayOrders PRIMARY KEY CLUSTERED (Id)
);
GO

-- Razorpay Payments Table
CREATE TABLE RazorpayPayments (
    -- Primary Key
    Id BIGINT IDENTITY(1,1) NOT NULL,
    
    -- Razorpay Payment Information
    RazorpayPaymentId NVARCHAR(255) NOT NULL, -- Razorpay payment ID (starts with "pay_")
    RazorpayOrderId NVARCHAR(255) NOT NULL, -- Razorpay order ID (starts with "order_")
    
    -- Payment Details
    Amount BIGINT NOT NULL, -- Amount in paise
    Currency NVARCHAR(3) DEFAULT 'INR' NOT NULL,
    
    -- Payment Status
    Status NVARCHAR(50) DEFAULT 'created' NOT NULL, -- created, authorized, captured, failed, etc.
    
    -- Signature Verification
    Signature NVARCHAR(500) NOT NULL, -- Payment signature
    SignatureVerified BIT DEFAULT 0 NOT NULL, -- Whether signature was verified
    VerificationAttempts INT DEFAULT 0 NOT NULL, -- Number of verification attempts
    
    -- User and Tenant Information (Optional)
    UserId BIGINT NULL,
    TenantId BIGINT NULL,
    
    -- Link to System Order (Optional)
    OrderId BIGINT NULL, -- Link to Orders table
    RazorpayOrderRecordId BIGINT NULL, -- Link to RazorpayOrders table
    
    -- Additional Information
    PaymentMethod NVARCHAR(100) NULL, -- card, netbanking, wallet, upi, etc.
    PaymentDescription NVARCHAR(500) NULL,
    
    -- Razorpay Response Data (JSON)
    RazorpayResponse NVARCHAR(MAX) NULL, -- Full Razorpay API response
    
    -- Timestamps
    CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
    UpdatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
    VerifiedAt DATETIME2(7) NULL, -- When payment was verified
    
    -- System Fields
    Active BIT DEFAULT 1 NOT NULL,
    
    -- Primary Key Constraint
    CONSTRAINT PK_RazorpayPayments PRIMARY KEY CLUSTERED (Id),
    
    -- Unique Constraint on Razorpay Payment ID
    CONSTRAINT UQ_RazorpayPayments_RazorpayPaymentId UNIQUE (RazorpayPaymentId),
    
    -- Check Constraints
    CONSTRAINT CK_RazorpayPayments_Amount CHECK (Amount > 0),
    CONSTRAINT CK_RazorpayPayments_Currency CHECK (Currency IN ('INR', 'USD', 'EUR', 'GBP'))
);

CREATE TABLE States (
    StateId BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NULL, -- NULL = global states
    StateCode NVARCHAR(10) NOT NULL, -- 'MH', 'KA', 'TN', etc.
    StateName NVARCHAR(100) NOT NULL, -- 'Maharashtra', 'Karnataka', 'Tamil Nadu'
    CountryCode NVARCHAR(10) DEFAULT 'IN' NOT NULL,
    Active BIT DEFAULT 1 NOT NULL,
    OrderBy INT DEFAULT 0 NOT NULL,
    CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
    UpdatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
    CONSTRAINT PK_States PRIMARY KEY (StateId),
    CONSTRAINT UQ_States_Tenant_Code UNIQUE (TenantId, StateCode)
);

-- Shipping Rates Table
CREATE TABLE ShippingRates (
    ShippingRateId BIGINT IDENTITY(1,1) NOT NULL,
    TenantId BIGINT NOT NULL,
    ProductType NVARCHAR(50) NOT NULL, -- 'Seed', 'Plant', 'Default'
    StateCode NVARCHAR(10) NULL, -- 'TN' for Tamil Nadu, NULL for all other states
    CourierType NVARCHAR(50) NOT NULL, -- 'Postal' or 'Other'
    
    -- Pricing
    BaseCharge DECIMAL(18,2) NOT NULL,
    PerUnitCharge DECIMAL(18,2) DEFAULT 0, -- Per quantity
    MinCharge DECIMAL(18,2) NOT NULL,
    MaxCharge DECIMAL(18,2) NULL,
    
    -- Free Shipping
    FreeShippingThreshold DECIMAL(18,2) NULL, -- Order value for free shipping
    
    Active BIT DEFAULT 1 NOT NULL,
    CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
    UpdatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
    
    CONSTRAINT PK_ShippingRates PRIMARY KEY (ShippingRateId),
    CONSTRAINT UQ_ShippingRates_Tenant_Product_State_Courier UNIQUE (TenantId, ProductType, StateCode, CourierType)
);

-- Indexes for faster lookups
-- Note: All indexes have been moved to 02.Indexes.sql for centralized management
GO

-- Create Indexes for better query performance
-- Note: All indexes have been moved to 02.Indexes.sql for centralized management
GO


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Coupons')
BEGIN
    CREATE TABLE [dbo].[Coupons] (
        [CouponId] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Code] NVARCHAR(50) NOT NULL,
        [Type] NVARCHAR(20) NOT NULL CHECK ([Type] IN ('percentage', 'fixed')),
        [Value] DECIMAL(18,2) NOT NULL CHECK ([Value] > 0),
        [MinAmount] DECIMAL(18,2) NULL CHECK ([MinAmount] >= 0),
        [MaxDiscount] DECIMAL(18,2) NULL CHECK ([MaxDiscount] >= 0),
        [Description] NVARCHAR(500) NULL,
        [StartDate] DATETIME2 NOT NULL,
        [EndDate] DATETIME2 NOT NULL,
        [UsageLimit] INT NULL CHECK ([UsageLimit] > 0),
        [UsageLimitPerUser] INT NULL CHECK ([UsageLimitPerUser] > 0),
        [UsageCount] INT NOT NULL DEFAULT 0 CHECK ([UsageCount] >= 0),
        [Active] BIT NOT NULL DEFAULT 1,
        [TenantId] BIGINT NOT NULL,
        [CreatedBy] BIGINT NULL,
        [UpdatedBy] BIGINT NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );

    -- Create unique index on Code + TenantId
    -- Note: Index creation moved to 02.Indexes.sql for centralized management

    -- Create index on TenantId for faster queries
    -- Note: Index creation moved to 02.Indexes.sql for centralized management
END
GO

--SELECT * FROM [dbo].[Coupons]

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CouponUsage')
BEGIN
    CREATE TABLE [dbo].[CouponUsage] (
        [UsageId] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [CouponId] BIGINT NOT NULL,
        [OrderId] BIGINT NOT NULL,
        [UserId] BIGINT NULL,
        [DiscountAmount] DECIMAL(18,2) NOT NULL CHECK ([DiscountAmount] >= 0),
        [OrderAmount] DECIMAL(18,2) NOT NULL CHECK ([OrderAmount] >= 0),
        [UsedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );

    -- Create index on CouponId for usage tracking
    -- Note: Index creation moved to 02.Indexes.sql for centralized management

    -- Create index on UserId for user-specific queries
    -- Note: Index creation moved to 02.Indexes.sql for centralized management

    -- Create index on OrderId
    -- Note: Index creation moved to 02.Indexes.sql for centralized management

    -- Create unique constraint on CouponId + OrderId to prevent duplicate usage
    -- Note: Index creation moved to 02.Indexes.sql for centralized management

    PRINT 'Table CouponUsage created successfully';
END
GO

CREATE TABLE [dbo].[PasswordResetOTPs] (
    OtpId BIGINT IDENTITY(1,1) PRIMARY KEY,
    UserId BIGINT NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    OTP NVARCHAR(6) NOT NULL,
    ExpiresAt DATETIME2(7) NOT NULL,
    Used BIT DEFAULT 0 NOT NULL,
    Attempts INT DEFAULT 0 NOT NULL,
    CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
    UsedAt DATETIME2(7) NULL,
    IpAddress NVARCHAR(45) NULL,
    UserAgent NVARCHAR(500) NULL,
    CONSTRAINT FK_PasswordResetOTPs_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);
GO

IF OBJECT_ID('dbo.ContactMessages', 'U') IS NULL
BEGIN
    CREATE TABLE ContactMessages
    (
        Id BIGINT IDENTITY(1,1) NOT NULL,
        UserId BIGINT NULL,
        TenantId BIGINT NULL,
        Name NVARCHAR(100) NOT NULL,
        Email NVARCHAR(256) NOT NULL,
        Phone NVARCHAR(32) NULL,
        Subject NVARCHAR(100) NULL,
        Message NVARCHAR(2000) NOT NULL,
        Language NVARCHAR(10) NULL,
        Source NVARCHAR(100) NULL,
        CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
        CONSTRAINT PK_ContactMessages PRIMARY KEY CLUSTERED (Id)
    );
END;

-- -- Order Analytics Table
-- CREATE TABLE OrderAnalytics (
-- 	AnalyticsId BIGINT IDENTITY(1,1) NOT NULL,
-- 	OrderId BIGINT NOT NULL,
-- 	UserId BIGINT NOT NULL,
-- 	OrderNumber NVARCHAR(50) NOT NULL,
-- 	TotalAmount DECIMAL(18,2) NOT NULL,
-- 	ItemCount INT NOT NULL,
-- 	OrderDate DATETIME2(7) NOT NULL,
-- 	TenantId BIGINT NULL,
-- 	OrderSource NVARCHAR(50) DEFAULT 'Web' NULL, -- Web, Mobile, API
-- 	DeviceType NVARCHAR(50) NULL,
-- 	UserAgent NVARCHAR(500) NULL,
-- 	CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	CONSTRAINT PK_OrderAnalytics PRIMARY KEY CLUSTERED (AnalyticsId)
-- );


-- -- Product Categories Table (Alternative structure for flexibility)
-- CREATE TABLE ProductCategories (
-- 	CategoryId BIGINT IDENTITY(1,1) NOT NULL,
-- 	Category NVARCHAR(255) NOT NULL,
-- 	Active BIT DEFAULT 1 NOT NULL,
-- 	OrderBy INT DEFAULT 0 NOT NULL,
-- 	Description NVARCHAR(MAX) NULL,
-- 	Icon NVARCHAR(255) NULL,
-- 	SubMenu BIT DEFAULT 0 NOT NULL,
-- 	TenantId BIGINT NULL,
-- 	Created DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	Modified DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	CONSTRAINT PK_ProductCategories PRIMARY KEY CLUSTERED (CategoryId)
-- );


-- -- Application Configuration Table
-- CREATE TABLE TenantConfig (
-- 	ConfigId BIGINT IDENTITY(1,1) NOT NULL,
-- 	ConfigKey NVARCHAR(255) NOT NULL,
-- 	ConfigValue NVARCHAR(MAX) NULL,
-- 	Description NVARCHAR(500) NULL,
-- 	CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	UpdatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	CreatedBy BIGINT NULL,
-- 	UpdatedBy BIGINT NULL,
-- 	Active BIT DEFAULT 1 NOT NULL,
-- 	CONSTRAINT PK_TenantConfig PRIMARY KEY CLUSTERED (ConfigId),
-- 	CONSTRAINT UQ_TenantConfig_ConfigKey UNIQUE (ConfigKey)
-- );

-- -- Permissions Table - System permissions
-- CREATE TABLE Permissions (
-- 	PermissionId BIGINT IDENTITY(1,1) NOT NULL,
-- 	PermissionName NVARCHAR(100) NOT NULL,
-- 	PermissionDescription NVARCHAR(255) NULL,
-- 	PermissionCategory NVARCHAR(100) DEFAULT 'General' NULL,
-- 	ResourceType NVARCHAR(50) NULL, -- Product, Order, User, etc.
-- 	ActionType NVARCHAR(50) NULL, -- View, Create, Update, Delete
-- 	Active BIT DEFAULT 1 NOT NULL,
-- 	CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	UpdatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	CONSTRAINT PK_Permissions PRIMARY KEY CLUSTERED (PermissionId),
-- 	CONSTRAINT UQ_Permissions_PermissionName UNIQUE (PermissionName)
-- );

-- -- Role Permissions Junction Table
-- CREATE TABLE RolePermissions (
-- 	RolePermissionId BIGINT IDENTITY(1,1) NOT NULL,
-- 	RoleId BIGINT NOT NULL,
-- 	PermissionId BIGINT NOT NULL,
-- 	Active BIT DEFAULT 1 NOT NULL,
-- 	CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	UpdatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	CONSTRAINT PK_RolePermissions PRIMARY KEY CLUSTERED (RolePermissionId),
-- 	CONSTRAINT FK_RolePermissions_Roles FOREIGN KEY (RoleId) REFERENCES Roles(RoleId),
-- 	CONSTRAINT FK_RolePermissions_Permissions FOREIGN KEY (PermissionId) REFERENCES Permissions(PermissionId),
-- 	CONSTRAINT UQ_RolePermissions_RolePermission UNIQUE (RoleId, PermissionId)
-- );


-- -- User Custom Permissions Table
-- CREATE TABLE UserCustomPermissions (
-- 	UserCustomPermissionId BIGINT IDENTITY(1,1) NOT NULL,
-- 	UserId BIGINT NOT NULL,
-- 	PermissionId BIGINT NOT NULL,
-- 	GrantedBy BIGINT NOT NULL,
-- 	GrantedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	ExpiresAt DATETIME2(7) NULL,
-- 	Active BIT DEFAULT 1 NOT NULL,
-- 	CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	UpdatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	CONSTRAINT PK_UserCustomPermissions PRIMARY KEY CLUSTERED (UserCustomPermissionId),
-- 	CONSTRAINT FK_UserCustomPermissions_Users FOREIGN KEY (UserId) REFERENCES Users(UserId),
-- 	CONSTRAINT FK_UserCustomPermissions_Permissions FOREIGN KEY (PermissionId) REFERENCES Permissions(PermissionId),
-- 	CONSTRAINT FK_UserCustomPermissions_GrantedBy FOREIGN KEY (GrantedBy) REFERENCES Users(UserId),
-- 	CONSTRAINT UQ_UserCustomPermissions_UserPermission UNIQUE (UserId, PermissionId)
-- );

-- -- User Preferences Table
-- CREATE TABLE UserPreferences (
-- 	PreferenceId BIGINT IDENTITY(1,1) NOT NULL,
-- 	UserId BIGINT NOT NULL,
-- 	PreferenceKey NVARCHAR(100) NOT NULL,
-- 	PreferenceValue NVARCHAR(MAX) NULL,
-- 	PreferenceType NVARCHAR(50) DEFAULT 'String' NOT NULL, -- String, Number, Boolean, JSON
-- 	Category NVARCHAR(100) DEFAULT 'General' NULL,
-- 	Active BIT DEFAULT 1 NOT NULL,
-- 	CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	UpdatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	CONSTRAINT PK_UserPreferences PRIMARY KEY CLUSTERED (PreferenceId)
-- );

-- -- User Tokens Table
-- CREATE TABLE UserTokens (
-- 	TokenId BIGINT IDENTITY(1,1) NOT NULL,
-- 	UserId BIGINT NOT NULL,
-- 	Token NVARCHAR(MAX) NOT NULL,
-- 	RefreshToken NVARCHAR(MAX) NULL,
-- 	TokenType NVARCHAR(50) DEFAULT 'JWT' NOT NULL,
-- 	DeviceId NVARCHAR(255) NULL,
-- 	DeviceInfo NVARCHAR(500) NULL,
-- 	IpAddress NVARCHAR(45) NULL,
-- 	UserAgent NVARCHAR(500) NULL,
-- 	ExpiresAt DATETIME2(7) NOT NULL,
-- 	IsRevoked BIT DEFAULT 0 NOT NULL,
-- 	RevokedAt DATETIME2(7) NULL,
-- 	RevokedReason NVARCHAR(255) NULL,
-- 	CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	UpdatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	CONSTRAINT PK_UserTokens PRIMARY KEY CLUSTERED (TokenId)
-- );

-- User Sessions Table
-- CREATE TABLE UserSessions (
-- 	SessionId BIGINT IDENTITY(1,1) NOT NULL,
-- 	UserId BIGINT NOT NULL,
-- 	SessionToken NVARCHAR(255) NOT NULL,
-- 	DeviceId NVARCHAR(255) NULL,
-- 	IpAddress NVARCHAR(45) NULL,
-- 	UserAgent NVARCHAR(500) NULL,
-- 	LoginAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	LastActivityAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	LoggedOutAt DATETIME2(7) NULL,
-- 	ExpiresAt DATETIME2(7) NOT NULL,
-- 	IsActive BIT DEFAULT 1 NOT NULL,
-- 	CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	UpdatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	CONSTRAINT PK_UserSessions PRIMARY KEY CLUSTERED (SessionId)
-- );

-- -- Password Reset Tokens Table
-- CREATE TABLE PasswordResetTokens (
-- 	ResetTokenId BIGINT IDENTITY(1,1) NOT NULL,
-- 	UserId BIGINT NOT NULL,
-- 	ResetToken NVARCHAR(255) NOT NULL,
-- 	ExpiresAt DATETIME2(7) NOT NULL,
-- 	IsUsed BIT DEFAULT 0 NOT NULL,
-- 	UsedAt DATETIME2(7) NULL,
-- 	IpAddress NVARCHAR(45) NULL,
-- 	UserAgent NVARCHAR(500) NULL,
-- 	CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	UpdatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	CONSTRAINT PK_PasswordResetTokens PRIMARY KEY CLUSTERED (ResetTokenId)
-- );

-- Contact Messages (public contact us form)


-- -- User Notifications Table
-- CREATE TABLE UserNotifications (
-- 	NotificationId BIGINT IDENTITY(1,1) NOT NULL,
-- 	UserId BIGINT NOT NULL,
-- 	NotificationType NVARCHAR(50) NOT NULL,
-- 	Title NVARCHAR(255) NOT NULL,
-- 	Message NVARCHAR(MAX) NOT NULL,
-- 	IsRead BIT DEFAULT 0 NOT NULL,
-- 	ReadAt DATETIME2(7) NULL,
-- 	Priority NVARCHAR(20) DEFAULT 'Normal' NOT NULL, -- Low, Normal, High, Critical
-- 	ExpiresAt DATETIME2(7) NULL,
-- 	ActionUrl NVARCHAR(500) NULL,
-- 	ActionText NVARCHAR(100) NULL,
-- 	CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	UpdatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	CONSTRAINT PK_UserNotifications PRIMARY KEY CLUSTERED (NotificationId)
-- );

-- -- Notification Queue Table
-- CREATE TABLE NotificationQueue (
-- 	QueueId BIGINT IDENTITY(1,1) NOT NULL,
-- 	UserId BIGINT NOT NULL,
-- 	NotificationType NVARCHAR(50) NOT NULL,
-- 	Subject NVARCHAR(255) NOT NULL,
-- 	Message NVARCHAR(MAX) NOT NULL,
-- 	OrderId BIGINT NULL,
-- 	ProductId BIGINT NULL,
-- 	Priority NVARCHAR(20) DEFAULT 'Normal' NOT NULL,
-- 	Status NVARCHAR(50) DEFAULT 'Pending' NOT NULL, -- Pending, Sent, Failed
-- 	ScheduledAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	SentAt DATETIME2(7) NULL,
-- 	AttemptCount INT DEFAULT 0 NOT NULL,
-- 	LastAttemptAt DATETIME2(7) NULL,
-- 	ErrorMessage NVARCHAR(MAX) NULL,
-- 	CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	UpdatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	CONSTRAINT PK_NotificationQueue PRIMARY KEY CLUSTERED (QueueId)
-- );

-- -- User Behavior Analytics Table
-- CREATE TABLE UserBehaviorAnalytics (
-- 	BehaviorId BIGINT IDENTITY(1,1) NOT NULL,
-- 	UserId BIGINT NOT NULL,
-- 	ActionType NVARCHAR(100) NOT NULL,
-- 	ActionDetails NVARCHAR(MAX) NULL,
-- 	ItemCount INT NULL,
-- 	TotalValue DECIMAL(18,2) NULL,
-- 	SessionId NVARCHAR(255) NULL,
-- 	IpAddress NVARCHAR(45) NULL,
-- 	UserAgent NVARCHAR(500) NULL,
-- 	DeviceType NVARCHAR(50) NULL,
-- 	ReferrerUrl NVARCHAR(500) NULL,
-- 	CreatedAt DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
-- 	CONSTRAINT PK_UserBehaviorAnalytics PRIMARY KEY CLUSTERED (BehaviorId)
-- );

-- Migration: Add LargeData column to ProductImages (run once on existing databases)
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('ProductImages') AND name = 'LargeData'
)
    ALTER TABLE ProductImages ADD LargeData VARBINARY(MAX) NULL;
GO