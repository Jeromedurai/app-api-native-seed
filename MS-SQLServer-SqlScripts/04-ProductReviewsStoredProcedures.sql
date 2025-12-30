-- =============================================
-- Product Reviews Stored Procedures
-- =============================================

USE [XTRACHEF_DB_DEV]
GO

-- Drop existing procedures if they exist
IF OBJECT_ID(N'[dbo].[SP_CREATE_PRODUCT_REVIEW]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_CREATE_PRODUCT_REVIEW];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_PRODUCT_REVIEWS]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_PRODUCT_REVIEWS];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_PRODUCT_REVIEW_STATS]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_PRODUCT_REVIEW_STATS];
GO

IF OBJECT_ID(N'[dbo].[SP_UPDATE_PRODUCT_REVIEW]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_UPDATE_PRODUCT_REVIEW];
GO

IF OBJECT_ID(N'[dbo].[SP_DELETE_PRODUCT_REVIEW]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_DELETE_PRODUCT_REVIEW];
GO

IF OBJECT_ID(N'[dbo].[SP_MARK_REVIEW_HELPFUL]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_MARK_REVIEW_HELPFUL];
GO

-- =============================================
-- SP_CREATE_PRODUCT_REVIEW
-- =============================================
CREATE PROCEDURE [dbo].[SP_CREATE_PRODUCT_REVIEW]
	@ProductId BIGINT,
	@UserId BIGINT,
	@TenantId BIGINT,
	@Rating INT,
	@ReviewTitle NVARCHAR(255) = NULL,
	@ReviewText NVARCHAR(MAX) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		BEGIN TRANSACTION;
		
		-- Validate rating
		IF @Rating < 1 OR @Rating > 5
		BEGIN
			RAISERROR('Rating must be between 1 and 5', 16, 1);
			RETURN;
		END
		
		-- Check if user already reviewed this product
		IF EXISTS (
			SELECT 1 FROM ProductReviews 
			WHERE ProductId = @ProductId 
			AND UserId = @UserId 
			AND Active = 1
		)
		BEGIN
			RAISERROR('You have already reviewed this product', 16, 1);
			RETURN;
		END
		
		-- Check if user has purchased this product (for verified purchase)
		DECLARE @IsVerifiedPurchase BIT = 0;
		IF EXISTS (
			SELECT 1 FROM OrderItems oi
			INNER JOIN Orders o ON oi.OrderId = o.OrderId
			WHERE oi.ProductId = @ProductId
			AND o.UserId = @UserId
			AND o.PaymentStatus = 'paid'
			AND o.OrderStatus != 'cancelled'
			AND o.Active = 1
		)
		BEGIN
			SET @IsVerifiedPurchase = 1;
		END
		
		-- Insert review
		INSERT INTO ProductReviews (
			ProductId, UserId, Rating, ReviewTitle, ReviewText,
			IsVerifiedPurchase, IsApproved, HelpfulCount, Active,
			CreatedAt, UpdatedAt
		)
		VALUES (
			@ProductId, @UserId, @Rating, @ReviewTitle, @ReviewText,
			@IsVerifiedPurchase, 1, 0, 1, -- Auto-approve reviews
			GETUTCDATE(), GETUTCDATE()
		);
		
		DECLARE @ReviewId BIGINT = SCOPE_IDENTITY();
		
		-- Update product rating
		UPDATE Products
		SET Rating = (
			SELECT AVG(CAST(Rating AS DECIMAL(3,2)))
			FROM ProductReviews
			WHERE ProductId = @ProductId
			AND Active = 1
			AND IsApproved = 1
		),
		Modified = GETUTCDATE()
		WHERE ProductId = @ProductId;
		
		-- Return created review
		SELECT 
			pr.ReviewId,
			pr.ProductId,
			pr.UserId,
			ISNULL(u.FirstName + ' ' + u.LastName, 'Unknown User') AS UserName,
			u.Email AS UserEmail,
			pr.Rating,
			pr.ReviewTitle AS Title,
			pr.ReviewText AS Comment,
			pr.IsVerifiedPurchase AS Verified,
			pr.HelpfulCount AS Helpful,
			pr.CreatedAt,
			pr.UpdatedAt
		FROM ProductReviews pr
		INNER JOIN Users u ON pr.UserId = u.UserId
		WHERE pr.ReviewId = @ReviewId;
		
		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;
		
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

-- =============================================
-- SP_GET_PRODUCT_REVIEWS
-- =============================================
CREATE PROCEDURE [dbo].[SP_GET_PRODUCT_REVIEWS]
	@ProductId BIGINT,
	@TenantId BIGINT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		SELECT 
			pr.ReviewId,
			pr.ProductId,
			pr.UserId,
			ISNULL(u.FirstName + ' ' + u.LastName, 'Unknown User') AS UserName,
			u.Email AS UserEmail,
			pr.Rating,
			pr.ReviewTitle AS Title,
			pr.ReviewText AS Comment,
			pr.IsVerifiedPurchase AS Verified,
			pr.HelpfulCount AS Helpful,
			pr.CreatedAt,
			pr.UpdatedAt
		FROM ProductReviews pr
		INNER JOIN Users u ON pr.UserId = u.UserId
		INNER JOIN Products p ON pr.ProductId = p.ProductId
		WHERE pr.ProductId = @ProductId
		AND pr.Active = 1
		AND pr.IsApproved = 1
		AND (@TenantId IS NULL OR p.TenantId = @TenantId)
		ORDER BY pr.CreatedAt DESC;
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
-- SP_GET_PRODUCT_REVIEW_STATS
-- =============================================
CREATE PROCEDURE [dbo].[SP_GET_PRODUCT_REVIEW_STATS]
	@ProductId BIGINT,
	@TenantId BIGINT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		DECLARE @TotalReviews INT = 0;
		DECLARE @AverageRating DECIMAL(3,2) = 0;
		
		-- Get total reviews and average rating
		SELECT 
			@TotalReviews = COUNT(*),
			@AverageRating = ISNULL(AVG(CAST(Rating AS DECIMAL(3,2))), 0)
		FROM ProductReviews pr
		INNER JOIN Products p ON pr.ProductId = p.ProductId
		WHERE pr.ProductId = @ProductId
		AND pr.Active = 1
		AND pr.IsApproved = 1
		AND (@TenantId IS NULL OR p.TenantId = @TenantId);
		
		-- Get rating distribution
		SELECT 
			Rating,
			COUNT(*) AS Count
		FROM ProductReviews pr
		INNER JOIN Products p ON pr.ProductId = p.ProductId
		WHERE pr.ProductId = @ProductId
		AND pr.Active = 1
		AND pr.IsApproved = 1
		AND (@TenantId IS NULL OR p.TenantId = @TenantId)
		GROUP BY Rating
		ORDER BY Rating DESC;
		
		-- Return stats
		SELECT 
			@TotalReviews AS TotalReviews,
			@AverageRating AS AverageRating;
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
-- SP_UPDATE_PRODUCT_REVIEW
-- =============================================
CREATE PROCEDURE [dbo].[SP_UPDATE_PRODUCT_REVIEW]
	@ReviewId BIGINT,
	@UserId BIGINT,
	@Rating INT = NULL,
	@ReviewTitle NVARCHAR(255) = NULL,
	@ReviewText NVARCHAR(MAX) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		BEGIN TRANSACTION;
		
		-- Check if review exists and belongs to user
		IF NOT EXISTS (
			SELECT 1 FROM ProductReviews 
			WHERE ReviewId = @ReviewId 
			AND UserId = @UserId 
			AND Active = 1
		)
		BEGIN
			RAISERROR('Review not found or you do not have permission to update it', 16, 1);
			RETURN;
		END
		
		-- Validate rating if provided
		IF @Rating IS NOT NULL AND (@Rating < 1 OR @Rating > 5)
		BEGIN
			RAISERROR('Rating must be between 1 and 5', 16, 1);
			RETURN;
		END
		
		-- Update review
		UPDATE ProductReviews
		SET 
			Rating = ISNULL(@Rating, Rating),
			ReviewTitle = ISNULL(@ReviewTitle, ReviewTitle),
			ReviewText = ISNULL(@ReviewText, ReviewText),
			UpdatedAt = GETUTCDATE()
		WHERE ReviewId = @ReviewId
		AND UserId = @UserId;
		
		DECLARE @ProductId BIGINT;
		SELECT @ProductId = ProductId FROM ProductReviews WHERE ReviewId = @ReviewId;
		
		-- Update product rating
		UPDATE Products
		SET Rating = (
			SELECT AVG(CAST(Rating AS DECIMAL(3,2)))
			FROM ProductReviews
			WHERE ProductId = @ProductId
			AND Active = 1
			AND IsApproved = 1
		),
		Modified = GETUTCDATE()
		WHERE ProductId = @ProductId;
		
		-- Return updated review
		SELECT 
			pr.ReviewId,
			pr.ProductId,
			pr.UserId,
			ISNULL(u.FirstName + ' ' + u.LastName, 'Unknown User') AS UserName,
			u.Email AS UserEmail,
			pr.Rating,
			pr.ReviewTitle AS Title,
			pr.ReviewText AS Comment,
			pr.IsVerifiedPurchase AS Verified,
			pr.CreatedAt,
			pr.UpdatedAt
		FROM ProductReviews pr
		INNER JOIN Users u ON pr.UserId = u.UserId
		WHERE pr.ReviewId = @ReviewId;
		
		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;
		
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

-- =============================================
-- SP_DELETE_PRODUCT_REVIEW
-- =============================================
CREATE PROCEDURE [dbo].[SP_DELETE_PRODUCT_REVIEW]
	@ReviewId BIGINT,
	@UserId BIGINT
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		BEGIN TRANSACTION;
		
		-- Check if review exists and belongs to user
		IF NOT EXISTS (
			SELECT 1 FROM ProductReviews 
			WHERE ReviewId = @ReviewId 
			AND UserId = @UserId 
			AND Active = 1
		)
		BEGIN
			RAISERROR('Review not found or you do not have permission to delete it', 16, 1);
			RETURN;
		END
		
		DECLARE @ProductId BIGINT;
		SELECT @ProductId = ProductId FROM ProductReviews WHERE ReviewId = @ReviewId;
		
		-- Soft delete review
		UPDATE ProductReviews
		SET Active = 0,
			UpdatedAt = GETUTCDATE()
		WHERE ReviewId = @ReviewId
		AND UserId = @UserId;
		
		-- Update product rating
		UPDATE Products
		SET Rating = (
			SELECT AVG(CAST(Rating AS DECIMAL(3,2)))
			FROM ProductReviews
			WHERE ProductId = @ProductId
			AND Active = 1
			AND IsApproved = 1
		),
		Modified = GETUTCDATE()
		WHERE ProductId = @ProductId;
		
		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;
		
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

-- =============================================
-- SP_MARK_REVIEW_HELPFUL
-- =============================================
CREATE PROCEDURE [dbo].[SP_MARK_REVIEW_HELPFUL]
	@ReviewId BIGINT,
	@UserId BIGINT
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		-- Check if review exists
		IF NOT EXISTS (
			SELECT 1 FROM ProductReviews 
			WHERE ReviewId = @ReviewId 
			AND Active = 1
		)
		BEGIN
			RAISERROR('Review not found', 16, 1);
			RETURN;
		END
		
		-- Increment helpful count
		UPDATE ProductReviews
		SET HelpfulCount = HelpfulCount + 1,
			UpdatedAt = GETUTCDATE()
		WHERE ReviewId = @ReviewId;
		
		-- Return updated helpful count
		SELECT HelpfulCount AS Helpful
		FROM ProductReviews
		WHERE ReviewId = @ReviewId;
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

PRINT 'Product Reviews stored procedures created successfully';
GO

