-- =============================================
-- Admin Dashboard Stored Procedures
-- =============================================

USE [XTRACHEF_DB_DEV]
GO

-- Drop existing procedures if they exist
IF OBJECT_ID(N'[dbo].[SP_GET_DASHBOARD_STATS]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_DASHBOARD_STATS];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_DASHBOARD_SALES_OVER_TIME]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_DASHBOARD_SALES_OVER_TIME];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_DASHBOARD_TOP_PRODUCTS]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_DASHBOARD_TOP_PRODUCTS];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_DASHBOARD_RECENT_ORDERS]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_DASHBOARD_RECENT_ORDERS];
GO

-- =============================================
-- SP_GET_DASHBOARD_STATS
-- =============================================
CREATE PROCEDURE [dbo].[SP_GET_DASHBOARD_STATS]
	@TenantId BIGINT,
	@StartDate DATETIME2(7) = NULL,
	@EndDate DATETIME2(7) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		-- Set default date range (last 30 days if not provided)
		IF @StartDate IS NULL
			SET @StartDate = DATEADD(DAY, -30, GETUTCDATE());
		IF @EndDate IS NULL
			SET @EndDate = GETUTCDATE();
		
		-- Total Orders
		DECLARE @TotalOrders INT = 0;
		SELECT @TotalOrders = COUNT(*)
		FROM Orders
		WHERE TenantId = @TenantId
		AND Active = 1
		AND OrderDate BETWEEN @StartDate AND @EndDate;
		
		-- Total Revenue
		DECLARE @TotalRevenue DECIMAL(18,2) = 0;
		SELECT @TotalRevenue = ISNULL(SUM(TotalAmount), 0)
		FROM Orders
		WHERE TenantId = @TenantId
		AND Active = 1
		AND PaymentStatus = 'paid'
		AND OrderDate BETWEEN @StartDate AND @EndDate;
		
		-- Total Customers
		DECLARE @TotalCustomers INT = 0;
		SELECT @TotalCustomers = COUNT(DISTINCT UserId)
		FROM Orders
		WHERE TenantId = @TenantId
		AND Active = 1
		AND OrderDate BETWEEN @StartDate AND @EndDate;
		
		-- Total Products
		DECLARE @TotalProducts INT = 0;
		SELECT @TotalProducts = COUNT(*)
		FROM Products
		WHERE TenantId = @TenantId
		AND Active = 1;
		
		-- Pending Orders
		DECLARE @PendingOrders INT = 0;
		SELECT @PendingOrders = COUNT(*)
		FROM Orders
		WHERE TenantId = @TenantId
		AND Active = 1
		AND OrderStatus = 'pending'
		AND OrderDate BETWEEN @StartDate AND @EndDate;
		
		-- Completed Orders
		DECLARE @CompletedOrders INT = 0;
		SELECT @CompletedOrders = COUNT(*)
		FROM Orders
		WHERE TenantId = @TenantId
		AND Active = 1
		AND OrderStatus = 'completed'
		AND OrderDate BETWEEN @StartDate AND @EndDate;
		
		-- Average Order Value
		DECLARE @AverageOrderValue DECIMAL(18,2) = 0;
		SELECT @AverageOrderValue = CASE 
			WHEN @TotalOrders > 0 THEN @TotalRevenue / @TotalOrders
			ELSE 0
		END;
		
		-- Return stats
		SELECT 
			@TotalOrders AS TotalOrders,
			@TotalRevenue AS TotalRevenue,
			@TotalCustomers AS TotalCustomers,
			@TotalProducts AS TotalProducts,
			@PendingOrders AS PendingOrders,
			@CompletedOrders AS CompletedOrders,
			@AverageOrderValue AS AverageOrderValue;
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

-- =============================================
-- SP_GET_DASHBOARD_SALES_OVER_TIME
-- =============================================
CREATE PROCEDURE [dbo].[SP_GET_DASHBOARD_SALES_OVER_TIME]
	@TenantId BIGINT,
	@StartDate DATETIME2(7) = NULL,
	@EndDate DATETIME2(7) = NULL,
	@GroupBy NVARCHAR(20) = 'day' -- 'day', 'week', 'month'
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		-- Set default date range (last 30 days if not provided)
		IF @StartDate IS NULL
			SET @StartDate = DATEADD(DAY, -30, GETUTCDATE());
		IF @EndDate IS NULL
			SET @EndDate = GETUTCDATE();
		
		IF @GroupBy = 'day'
		BEGIN
			SELECT 
				CAST(OrderDate AS DATE) AS Date,
				COUNT(*) AS OrderCount,
				SUM(TotalAmount) AS Revenue,
				SUM(CASE WHEN PaymentStatus = 'paid' THEN TotalAmount ELSE 0 END) AS PaidRevenue
			FROM Orders
			WHERE TenantId = @TenantId
			AND Active = 1
			AND OrderDate BETWEEN @StartDate AND @EndDate
			GROUP BY CAST(OrderDate AS DATE)
			ORDER BY Date ASC;
		END
		ELSE IF @GroupBy = 'week'
		BEGIN
			SELECT 
				CAST(DATEADD(WEEK, DATEDIFF(WEEK, 0, OrderDate), 0) AS DATE) AS Date,
				COUNT(*) AS OrderCount,
				SUM(TotalAmount) AS Revenue,
				SUM(CASE WHEN PaymentStatus = 'paid' THEN TotalAmount ELSE 0 END) AS PaidRevenue
			FROM Orders
			WHERE TenantId = @TenantId
			AND Active = 1
			AND OrderDate BETWEEN @StartDate AND @EndDate
			GROUP BY CAST(DATEADD(WEEK, DATEDIFF(WEEK, 0, OrderDate), 0) AS DATE)
			ORDER BY Date ASC;
		END
		ELSE IF @GroupBy = 'month'
		BEGIN
			SELECT 
				CAST(DATEADD(MONTH, DATEDIFF(MONTH, 0, OrderDate), 0) AS DATE) AS Date,
				COUNT(*) AS OrderCount,
				SUM(TotalAmount) AS Revenue,
				SUM(CASE WHEN PaymentStatus = 'paid' THEN TotalAmount ELSE 0 END) AS PaidRevenue
			FROM Orders
			WHERE TenantId = @TenantId
			AND Active = 1
			AND OrderDate BETWEEN @StartDate AND @EndDate
			GROUP BY CAST(DATEADD(MONTH, DATEDIFF(MONTH, 0, OrderDate), 0) AS DATE)
			ORDER BY Date ASC;
		END
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

-- =============================================
-- SP_GET_DASHBOARD_TOP_PRODUCTS
-- =============================================
CREATE PROCEDURE [dbo].[SP_GET_DASHBOARD_TOP_PRODUCTS]
	@TenantId BIGINT,
	@StartDate DATETIME2(7) = NULL,
	@EndDate DATETIME2(7) = NULL,
	@TopCount INT = 10
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		-- Set default date range (last 30 days if not provided)
		IF @StartDate IS NULL
			SET @StartDate = DATEADD(DAY, -30, GETUTCDATE());
		IF @EndDate IS NULL
			SET @EndDate = GETUTCDATE();
		
		SELECT TOP (@TopCount)
			p.ProductId,
			p.ProductName,
			p.ProductCode,
			SUM(oi.Quantity) AS TotalQuantitySold,
			SUM(oi.Total) AS TotalRevenue,
			COUNT(DISTINCT oi.OrderId) AS OrderCount,
			p.Price,
			p.Quantity AS StockQuantity
		FROM OrderItems oi
		INNER JOIN Orders o ON oi.OrderId = o.OrderId
		INNER JOIN Products p ON oi.ProductId = p.ProductId
		WHERE o.TenantId = @TenantId
		AND o.Active = 1
		AND oi.Active = 1
		AND o.OrderDate BETWEEN @StartDate AND @EndDate
		GROUP BY p.ProductId, p.ProductName, p.ProductCode, p.Price, p.Quantity
		ORDER BY TotalQuantitySold DESC, TotalRevenue DESC;
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

-- =============================================
-- SP_GET_DASHBOARD_RECENT_ORDERS
-- =============================================
CREATE PROCEDURE [dbo].[SP_GET_DASHBOARD_RECENT_ORDERS]
	@TenantId BIGINT,
	@Count INT = 10
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		SELECT TOP (@Count)
			o.OrderId,
			o.OrderNumber,
			o.UserId,
			u.UserName,
			u.Email AS UserEmail,
			o.TotalAmount,
			o.OrderStatus,
			o.PaymentStatus,
			o.OrderDate,
			COUNT(oi.OrderItemId) AS ItemCount
		FROM Orders o
		INNER JOIN Users u ON o.UserId = u.UserId
		LEFT JOIN OrderItems oi ON o.OrderId = oi.OrderId AND oi.Active = 1
		WHERE o.TenantId = @TenantId
		AND o.Active = 1
		GROUP BY o.OrderId, o.OrderNumber, o.UserId, u.UserName, u.Email, 
			o.TotalAmount, o.OrderStatus, o.PaymentStatus, o.OrderDate
		ORDER BY o.OrderDate DESC;
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

PRINT 'Dashboard stored procedures created successfully';
GO

