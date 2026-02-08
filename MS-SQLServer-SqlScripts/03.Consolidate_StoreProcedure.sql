USE [XTRACHEF_DB_DEV]
IF OBJECT_ID(N'[dbo].[SP_USER_LOGOUT]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_USER_LOGOUT];
GO

IF OBJECT_ID(N'[dbo].[SP_ADD_PRODUCT]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_ADD_PRODUCT];
GO

IF OBJECT_ID(N'[dbo].[SP_DELETE_PRODUCT]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_DELETE_PRODUCT];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_MENU_MASTER]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_MENU_MASTER];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_PRODUCT_BY_ID]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_PRODUCT_BY_ID];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_USER_CART]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_USER_CART];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_USER_PROFILE]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_USER_PROFILE];
GO

IF OBJECT_ID(N'[dbo].[SP_RESET_PASSWORD]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_RESET_PASSWORD];
GO

IF OBJECT_ID(N'[dbo].[SP_REQUEST_PASSWORD_RESET]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_REQUEST_PASSWORD_RESET];
GO

IF OBJECT_ID(N'[dbo].[SP_VERIFY_RESET_OTP]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_VERIFY_RESET_OTP];
GO

IF OBJECT_ID(N'[dbo].[SP_RESEND_RESET_OTP]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_RESEND_RESET_OTP];
GO

IF OBJECT_ID(N'[dbo].[PasswordResetOTPs]', N'U') IS NOT NULL
	DROP TABLE [dbo].[PasswordResetOTPs];
GO

IF OBJECT_ID(N'[dbo].[SP_SEARCH_PRODUCTS]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_SEARCH_PRODUCTS];
GO

IF OBJECT_ID(N'[dbo].[SP_UPDATE_PRODUCT]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_UPDATE_PRODUCT];
GO

IF OBJECT_ID(N'[dbo].[SP_UPDATE_USER_PROFILE]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_UPDATE_USER_PROFILE];
GO

IF OBJECT_ID(N'[dbo].[SP_USER_LOGIN]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_USER_LOGIN];
GO

IF OBJECT_ID(N'[dbo].[SP_USER_REGISTER]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_USER_REGISTER];
GO

IF OBJECT_ID(N'[dbo].[SP_ADD_ITEM_TO_CART]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_ADD_ITEM_TO_CART];
GO

IF OBJECT_ID(N'[dbo].[SP_REMOVE_ITEM_FROM_CART]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_REMOVE_ITEM_FROM_CART];
GO

IF OBJECT_ID(N'[dbo].[SP_REMOVE_ITEM_FROM_CART]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_REMOVE_ITEM_FROM_CART];
GO

IF OBJECT_ID(N'[dbo].[SP_CLEAR_CART]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_CLEAR_CART];
GO

IF OBJECT_ID(N'[dbo].[SP_CREATE_ORDER]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_CREATE_ORDER];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_ORDERS]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_ORDERS];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_ORDER_BY_ID]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_ORDER_BY_ID];
GO

IF OBJECT_ID(N'[dbo].[SP_CANCEL_ORDER]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_CANCEL_ORDER];
GO

IF OBJECT_ID(N'[dbo].[SP_UPDATE_ORDER_STATUS]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_UPDATE_ORDER_STATUS];
GO

IF OBJECT_ID(N'[dbo].[SP_ADMIN_GET_ALL_USERS]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_ADMIN_GET_ALL_USERS];
GO

IF OBJECT_ID(N'[dbo].[SP_ADMIN_UPDATE_USER_ROLE]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_ADMIN_UPDATE_USER_ROLE];
GO

IF OBJECT_ID(N'[dbo].[SP_ADMIN_GET_ALL_ORDERS]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_ADMIN_GET_ALL_ORDERS];
GO

IF OBJECT_ID(N'[dbo].[SP_ADD_PRODUCT_IMAGES]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_ADD_PRODUCT_IMAGES];
GO

IF OBJECT_ID(N'[dbo].[SP_UPDATE_PRODUCT_IMAGE]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_UPDATE_PRODUCT_IMAGE];
GO

IF OBJECT_ID(N'[dbo].[SP_DELETE_PRODUCT_IMAGE]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_DELETE_PRODUCT_IMAGE];
GO

IF OBJECT_ID(N'[dbo].[SP_CREATE_RAZORPAY_ORDER]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_CREATE_RAZORPAY_ORDER];
GO

IF OBJECT_ID(N'[dbo].[SP_VERIFY_RAZORPAY_PAYMENT]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_VERIFY_RAZORPAY_PAYMENT];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_ALL_CATEGORIES]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_ALL_CATEGORIES];
GO

IF OBJECT_ID(N'[dbo].[SP_ADD_CATEGORY]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_ADD_CATEGORY];
GO

IF OBJECT_ID(N'[dbo].[SP_UPDATE_CATEGORY]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_UPDATE_CATEGORY];
GO

IF OBJECT_ID(N'[dbo].[SP_ADD_IMAGE_LIST]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_ADD_IMAGE_LIST];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_IMAGE_BY_ID]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_IMAGE_BY_ID];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_PRODUCT_LIST_FILTERED]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_PRODUCT_LIST_FILTERED];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_IMAGES_BY_PRODUCT_ID]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_IMAGES_BY_PRODUCT_ID];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_USER_ADDRESSES]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_USER_ADDRESSES];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_ADDRESS_BY_ID]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_ADDRESS_BY_ID];
GO

IF OBJECT_ID(N'[dbo].[SP_CREATE_USER_ADDRESS]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_CREATE_USER_ADDRESS];
GO

IF OBJECT_ID(N'[dbo].[SP_UPDATE_USER_ADDRESS]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_UPDATE_USER_ADDRESS];
GO

IF OBJECT_ID(N'[dbo].[SP_DELETE_USER_ADDRESS]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_DELETE_USER_ADDRESS];
GO

IF OBJECT_ID(N'[dbo].[SP_SET_DEFAULT_ADDRESS]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_SET_DEFAULT_ADDRESS];
GO

IF OBJECT_ID(N'[dbo].[SP_ADMIN_GET_ALL_ADDRESSES]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_ADMIN_GET_ALL_ADDRESSES];
GO

IF OBJECT_ID(N'[dbo].[SP_VALIDATE_ADDRESS]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_VALIDATE_ADDRESS];
GO

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

IF OBJECT_ID(N'[dbo].[SP_UPSERT_WISHLIST]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_UPSERT_WISHLIST];
GO

IF OBJECT_ID(N'[dbo].[SP_REMOVE_WISHLIST]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_REMOVE_WISHLIST];
GO

IF OBJECT_ID(N'[dbo].[SP_CLEAR_WISHLIST]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_CLEAR_WISHLIST];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_USER_WISHLIST_ITEMS]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_USER_WISHLIST_ITEMS];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_STATES]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_STATES];
GO

IF OBJECT_ID(N'[dbo].[SP_CALCULATE_SHIPPING_CHARGE]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_CALCULATE_SHIPPING_CHARGE];
GO

IF OBJECT_ID(N'[dbo].[SP_CALCULATE_MIXED_SHIPPING]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_CALCULATE_MIXED_SHIPPING];
GO

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

IF OBJECT_ID(N'[dbo].[SP_CREATE_COUPON]', N'P') IS NOT NULL
DROP PROCEDURE [dbo].[SP_CREATE_COUPON];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_COUPONS]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_COUPONS];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_COUPON_BY_ID]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_COUPON_BY_ID];
GO

IF OBJECT_ID(N'[dbo].[SP_UPDATE_COUPON]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_UPDATE_COUPON];
GO

IF OBJECT_ID(N'[dbo].[SP_DELETE_COUPON]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_DELETE_COUPON];
GO

IF OBJECT_ID(N'[dbo].[SP_VALIDATE_COUPON]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_VALIDATE_COUPON];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_COUPON_USAGE]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_COUPON_USAGE];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_ORDER_ID_BY_RAZORPAY_ORDER_ID]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_ORDER_ID_BY_RAZORPAY_ORDER_ID];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_ORDER_ID_BY_ORDER_NUMBER]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_ORDER_ID_BY_ORDER_NUMBER];
GO

IF OBJECT_ID(N'[dbo].[SP_UPDATE_ORDER_STATUS_WITH_PAYMENT]', N'P') IS NOT NULL
DROP PROCEDURE [dbo].[SP_UPDATE_ORDER_STATUS_WITH_PAYMENT];
GO

IF OBJECT_ID(N'[dbo].[SP_SET_MAIN_PRODUCT_IMAGE]', N'P') IS NOT NULL
DROP PROCEDURE [dbo].[SP_SET_MAIN_PRODUCT_IMAGE];
GO

IF OBJECT_ID(N'[dbo].[SP_DELETE_PRODUCT_IMAGE]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_DELETE_PRODUCT_IMAGE];
GO

IF OBJECT_ID(N'[dbo].[SP_SEARCH_PRODUCTS_ENHANCED]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_SEARCH_PRODUCTS_ENHANCED];
GO

IF OBJECT_ID(N'[dbo].[SP_GET_SEARCH_SUGGESTIONS]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_GET_SEARCH_SUGGESTIONS];
GO


-- Drop and recreate SP_DELETE_CATEGORY
IF OBJECT_ID(N'[dbo].[SP_DELETE_CATEGORY]', N'P') IS NOT NULL
	DROP PROCEDURE [dbo].[SP_DELETE_CATEGORY];
GO

CREATE PROCEDURE [dbo].[SP_DELETE_CATEGORY]
	@CategoryId BIGINT,
	@TenantId BIGINT,
	@UserId BIGINT
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		BEGIN TRANSACTION;
		
		-- Check if category exists and belongs to the tenant
		IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryId = @CategoryId AND TenantId = @TenantId)
		BEGIN
			RAISERROR('Category not found or does not belong to this tenant.', 16, 1);
			RETURN;
		END
		
		-- Check if category has child categories
		IF EXISTS (SELECT 1 FROM Categories WHERE ParentCategoryId = @CategoryId AND Active = 1)
		BEGIN
			RAISERROR('Cannot delete category with active child categories. Please delete or reassign child categories first.', 16, 1);
			RETURN;
		END
		
		-- Check if category is used by any products
		IF EXISTS (SELECT 1 FROM Products WHERE Category = @CategoryId AND Active = 1)
		BEGIN
			-- Soft delete - just mark as inactive
			UPDATE Categories
			SET 
				Active = 0,
				Modified = GETUTCDATE(),
				ModifiedBy = @UserId
			WHERE CategoryId = @CategoryId
				AND TenantId = @TenantId;
		END
		ELSE
		BEGIN
			-- Hard delete - no products are using this category
			DELETE FROM Categories 
			WHERE CategoryId = @CategoryId 
				AND TenantId = @TenantId;
		END
		
		-- Return success
		SELECT @CategoryId AS CategoryId;
		
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

CREATE PROCEDURE [dbo].[SP_DELETE_PRODUCT_IMAGE]
				@ProductId BIGINT,
				@ImageId BIGINT,
				@UserId BIGINT = NULL,
				@TenantId BIGINT = NULL,
				@HardDelete BIT = 0,
				@IpAddress NVARCHAR(45) = NULL,
				@UserAgent NVARCHAR(500) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		BEGIN TRANSACTION;
		
		DECLARE @CurrentTime DATETIME = GETUTCDATE();
		DECLARE @ImageName NVARCHAR(255);
		DECLARE @IsMain BIT;
		DECLARE @IsActive BIT;
		
		-- Validate product exists and is active
		IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductId = @ProductId AND Active = 1)
		BEGIN
			RAISERROR('Product not found or inactive.', 16, 1);
			RETURN;
		END
		
		-- Validate image exists and belongs to product
		SELECT 
			@ImageName = ImageName,
			@IsMain = Main,
			@IsActive = Active
		FROM ProductImages 
		WHERE ImageId = @ImageId AND ProductId = @ProductId;
		
		IF @ImageName IS NULL
		BEGIN
			RAISERROR('Image not found or does not belong to this product.', 16, 1);
			RETURN;
		END
		
		-- Check if image is already deleted (soft delete)
		IF @IsActive = 0 AND @HardDelete = 0
		BEGIN
			RAISERROR('Image is already deleted.', 16, 1);
			RETURN;
		END
		
		-- Validate user if provided
		IF @UserId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId AND Active = 1)
		BEGIN
			RAISERROR('User not found or inactive.', 16, 1);
			RETURN;
		END
		
		-- Check if this is the last active image for the product
		DECLARE @ActiveImageCount INT;
		SELECT @ActiveImageCount = COUNT(*)
		FROM ProductImages 
		WHERE ProductId = @ProductId AND Active = 1 AND ImageId != @ImageId;
		
		-- Prevent deletion of the last image if it's the main image
		IF @ActiveImageCount = 0 AND @IsMain = 1
		BEGIN
			RAISERROR('Cannot delete the last active main image. Please add another image first.', 16, 1);
			RETURN;
		END
		
		-- Perform deletion
		IF @HardDelete = 1
		BEGIN
			-- Hard delete: Remove from database completely
			DELETE FROM ProductImages
			WHERE ImageId = @ImageId AND ProductId = @ProductId;
		END
		ELSE
		BEGIN
			-- Soft delete: Mark as inactive
			UPDATE ProductImages
			SET 
				Active = 0,
				Main = 0, -- Remove main status if it was main
				Modified = @CurrentTime,
				ModifiedBy = @UserId,
				DeletedAt = @CurrentTime,
				DeletedBy = @UserId
			WHERE ImageId = @ImageId AND ProductId = @ProductId;
		END
		
		-- If deleted image was main, set another active image as main
		IF @IsMain = 1 AND @ActiveImageCount > 0
		BEGIN
			UPDATE ProductImages
			SET 
				Main = 1,
				Modified = @CurrentTime
			WHERE ProductId = @ProductId 
				AND Active = 1 
				AND ImageId = (
					SELECT TOP 1 ImageId 
					FROM ProductImages 
					WHERE ProductId = @ProductId AND Active = 1 
					ORDER BY OrderBy, ImageId
				);
		END
		
		-- Update product modified date
		UPDATE Products
		SET Modified = @CurrentTime
		WHERE ProductId = @ProductId;
		
		-- Log the image deletion activity
		IF @UserId IS NOT NULL
		BEGIN
			INSERT INTO UserActivityLog (
				UserId,
				ActivityType,
				ActivityDescription,
				IpAddress,
				UserAgent,
				CreatedAt
			) VALUES (
				@UserId,
				CASE WHEN @HardDelete = 1 THEN 'PRODUCT_IMAGE_HARD_DELETED' ELSE 'PRODUCT_IMAGE_SOFT_DELETED' END,
				CASE WHEN @HardDelete = 1 THEN 'Permanently deleted' ELSE 'Soft deleted' END + 
				' image ' + @ImageName + ' from product ID: ' + CAST(@ProductId AS VARCHAR(20)),
				@IpAddress,
				@UserAgent,
				@CurrentTime
			);
		END
		
		-- Return deletion confirmation
		SELECT 
			@ImageId AS ImageId,
			@ProductId AS ProductId,
			@ImageName AS ImageName,
			@IsMain AS WasMain,
			@HardDelete AS HardDeleted,
			@CurrentTime AS DeletedAt,
			@UserId AS DeletedBy,
			CASE WHEN @HardDelete = 1 THEN 'Image permanently deleted' ELSE 'Image soft deleted' END AS Message,
			@ActiveImageCount AS RemainingActiveImages;
		
		-- Return current active images for the product
		SELECT 
			pi.ImageId,
			pi.ProductId,
			pi.Poster,
			pi.Main,
			pi.Active,
			pi.OrderBy,
			pi.CreatedAt AS Created,
			pi.Modified
		FROM ProductImages pi
		WHERE pi.ProductId = @ProductId AND pi.Active = 1
		ORDER BY pi.OrderBy, pi.ImageId;
		
		COMMIT TRANSACTION;
		
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;
			
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		-- Log the failed image deletion
		IF @UserId IS NOT NULL
		BEGIN
			INSERT INTO UserActivityLog (
				UserId,
				ActivityType,
				ActivityDescription,
				IpAddress,
				UserAgent,
				CreatedAt
			) VALUES (
				@UserId,
				'PRODUCT_IMAGE_DELETE_FAILED',
				'Failed to delete image ID: ' + CAST(@ImageId AS VARCHAR(20)) + ' from product ID: ' + CAST(@ProductId AS VARCHAR(20)) + ' - ' + @ErrorMessage,
				@IpAddress,
				@UserAgent,
				GETUTCDATE()
			);
		END
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO
CREATE PROCEDURE [dbo].[SP_UPDATE_CATEGORY]
	@CategoryId BIGINT,
	@TenantId BIGINT,
	@CategoryName NVARCHAR(255),
	@Description NVARCHAR(MAX) = NULL,
	@Active BIT = 1,
	@ParentCategoryId BIGINT = NULL,
	@OrderBy INT = 0,
	@Icon NVARCHAR(255) = NULL,
	@HasSubMenu BIT = 0,
	@Link NVARCHAR(500) = NULL,
	@UserId BIGINT,
	@MenuId BIGINT
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		BEGIN TRANSACTION;
		
		-- Check if category exists and belongs to the tenant
		IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryId = @CategoryId AND TenantId = @TenantId)
		BEGIN
			RAISERROR('Category not found or does not belong to this tenant.', 16, 1);
			RETURN;
		END
		
		-- Check if new category name already exists for this tenant (excluding current category)
		IF EXISTS (SELECT 1 FROM Categories WHERE CategoryName = @CategoryName AND TenantId = @TenantId AND CategoryId != @CategoryId)
		BEGIN
			RAISERROR('Category name already exists for this tenant.', 16, 1);
			RETURN;
		END
		
		-- Validate parent category if provided
		IF @ParentCategoryId IS NOT NULL
		BEGIN
			IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryId = @ParentCategoryId AND TenantId = @TenantId)
			BEGIN
				RAISERROR('Parent category does not exist.', 16, 1);
				RETURN;
			END
			
			-- Prevent circular reference (category cannot be its own parent or grandparent)
			IF @ParentCategoryId = @CategoryId
			BEGIN
				RAISERROR('Category cannot be its own parent.', 16, 1);
				RETURN;
			END
		END
		
		-- Update category
		UPDATE Categories
		SET 
			CategoryName = @CategoryName,
			Description = @Description,
			Active = @Active,
			ParentCategoryId = @ParentCategoryId,
			OrderBy = @OrderBy,
			Icon = @Icon,
			HasSubMenu = @HasSubMenu,
			Link = @Link,
			Modified = GETUTCDATE(),
			ModifiedBy = @UserId,
			MenuId = @MenuId
		WHERE CategoryId = @CategoryId
			AND TenantId = @TenantId;
		
		-- Return the updated category ID
		SELECT @CategoryId AS CategoryId;
		
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

CREATE or ALTER PROCEDURE [dbo].[SP_ADD_CATEGORY]
	@TenantId BIGINT,
	@CategoryName NVARCHAR(255),
	@Description NVARCHAR(MAX) = NULL,
	@Active BIT = 1,
	@ParentCategoryId BIGINT = NULL,
	@OrderBy INT = 0,
	@Icon NVARCHAR(255) = NULL,
	@HasSubMenu BIT = 0,
	@Link NVARCHAR(500) = NULL,
	@UserId BIGINT,
	@MenuId BIGINT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		BEGIN TRANSACTION;

		declare 
				@Menu NVARCHAR(100) = NULL;
		
		
		IF NOT EXISTS (
			SELECT 1 FROM Categories WHERE CategoryName = @CategoryName AND TenantId = @TenantId
		)
		BEGIN
			-- Insert new category
			INSERT INTO Categories (
				TenantId,
				CategoryName,
				Description,
				Active,
				ParentCategoryId,
				OrderBy,
				Icon,
				HasSubMenu,
				Menu,
				Link,
				Created,
				CreatedBy,
				Modified,
				ModifiedBy,
				MenuId
			) VALUES (
				@TenantId,
				@CategoryName,
				@Description,
				@Active,
				@ParentCategoryId,
				@OrderBy,
				@Icon,
				@HasSubMenu,
				@Menu,
				@Link,
				GETUTCDATE(),
				@UserId,
				GETUTCDATE(),
				@UserId,
				@MenuId
			);
		END
		ELSE
		BEGIN
			UPDATE Categories
			SET 
				CategoryName = @CategoryName,
				Description = @Description,
				Active = @Active,
				ParentCategoryId = @ParentCategoryId,
				OrderBy = @OrderBy,
				Icon = @Icon,
				HasSubMenu = @HasSubMenu,
				Link = @Link,
				Modified = GETUTCDATE(),
				ModifiedBy = @UserId,
				MenuId = @MenuId
			WHERE CategoryName = @CategoryName
			AND TenantId = @TenantId;
		END
		
		-- Return the new category ID
		SELECT CategoryId from Categories WHERE CategoryName = @CategoryName;
		
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

CREATE PROCEDURE [dbo].[SP_GET_ALL_CATEGORIES]
	@TenantId BIGINT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		-- Get all categories with optional tenant filtering
		SELECT 
			c.CategoryId,
			c.CategoryName AS Category,
			c.Active,
			c.HasSubMenu AS SubMenu,
			c.Created,
			c.Modified,
			c.OrderBy,
			c.Description,
			c.Icon,
			c.ParentCategoryId,
			c.TenantId,
			c.MenuId
		FROM Categories c
		WHERE (@TenantId IS NULL OR c.TenantId = @TenantId)
			AND c.Active = 1
		ORDER BY c.OrderBy, c.CategoryName;
		
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

CREATE PROCEDURE [dbo].[SP_USER_LOGOUT]
	@UserId BIGINT,
	@DeviceId NVARCHAR(255) = NULL,
	@LogoutFromAllDevices BIT = 0
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		BEGIN TRANSACTION;
		
		-- Validate user exists and is active
		IF NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId AND Active = 1)
		BEGIN
			RAISERROR('User not found or inactive.', 16, 1);
			RETURN;
		END
		
		-- If logout from all devices is requested
		IF @LogoutFromAllDevices = 1
		BEGIN
			-- Clear remember me sessions
			UPDATE Users 
			SET RememberMeToken = NULL,
				RememberMeExpiry = NULL,
				LastLogout = GETUTCDATE(),
				LastLogin = NULL
			WHERE UserId = @UserId;
		END
		ELSE
		BEGIN
			-- Update last logout time
			UPDATE Users 
			SET LastLogout = GETUTCDATE(),
			LastLogin = NULL
			WHERE UserId = @UserId;
		END
		
		-- Log the logout activity
		INSERT INTO UserActivityLog (
			UserId,
			ActivityType,
			ActivityDescription,
			IPAddress,
			UserAgent,
			DeviceId,
			CreatedAt
		) VALUES (
			@UserId,
			'LOGOUT',
			CASE 
			WHEN @LogoutFromAllDevices = 1 THEN 'Logout from all devices'
				WHEN @DeviceId IS NOT NULL THEN 'Logout from device: ' + @DeviceId
				ELSE 'User logout'
			END,
			NULL, -- IP Address would be passed from application
			NULL, -- User Agent would be passed from application
			@DeviceId,
			GETUTCDATE()
		);
		
		-- Return success status
		SELECT 
			@UserId AS UserId,
			'Logged out successfully' AS Message,
			GETUTCDATE() AS LogoutTime,
			@LogoutFromAllDevices AS LogoutFromAllDevices;
		
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

CREATE PROCEDURE [dbo].[SP_GET_MENU_MASTER]
	@TenantId BIGINT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		-- Get menu master with categories
		SELECT 
			m.MenuId,
			m.MenuName,
			m.OrderBy,
			m.Active,
			m.Image,
			m.SubMenu,
			m.TenantId,
			m.Created,
			m.Modified,
			-- Category information
			c.CategoryId,
			c.CategoryName AS Category,
			c.Active AS CategoryActive,
			c.OrderBy AS CategoryOrderBy,
			c.Icon AS CategoryIcon,
			c.Description AS CategoryDescription
		FROM MenuMaster m
		LEFT JOIN Categories c ON m.MenuId = c.MenuId 
			AND c.Active = 1
			AND (@TenantId IS NULL OR c.TenantId = @TenantId)
		WHERE m.Active = 1
			AND (@TenantId IS NULL OR m.TenantId = @TenantId)
		ORDER BY m.OrderBy, m.MenuName, c.OrderBy, c.CategoryName;
		
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_USER_REGISTER]
	@Name NVARCHAR(255),
	@Email NVARCHAR(255),
	@Phone NVARCHAR(50),
	@Password NVARCHAR(100),
	@TenantId BIGINT = 1,
	@AgreeToTerms BIT = 1
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		BEGIN TRANSACTION;
		
		DECLARE @UserId BIGINT;
		DECLARE @Salt NVARCHAR(100);
		DECLARE @PasswordHash NVARCHAR(100);
		DECLARE @DefaultRoleId BIGINT = 2; -- Assuming 2 is the default user role
		
		-- Check if email already exists
		IF EXISTS (SELECT 1 FROM Users WHERE Email = @Email)
		BEGIN
			RAISERROR('Email address is already registered.', 16, 1);
			RETURN;
		END
		
		-- Check if phone already exists
		IF EXISTS (SELECT 1 FROM Users WHERE Phone = @Phone)
		BEGIN
			RAISERROR('Phone number is already registered.', 16, 1);
			RETURN;
		END
		
		-- Validate terms agreement
		IF @AgreeToTerms = 0
		BEGIN
			RAISERROR('You must agree to the terms and conditions.', 16, 1);
			RETURN;
		END
		
		-- Generate salt and hash password
		SET @Salt = CONVERT(NVARCHAR(50), NEWID());
		SET @PasswordHash = CONVERT(NVARCHAR(100), HASHBYTES('SHA2_256', cast(@Password as NVARCHAR(100))+ @Salt), 2);

		-- Parse name into first and last name
		DECLARE @FirstName NVARCHAR(255);
		DECLARE @LastName NVARCHAR(255);
		DECLARE @SpaceIndex INT = CHARINDEX(' ', @Name);
		
		IF @SpaceIndex > 0
		BEGIN
			SET @FirstName = LEFT(@Name, @SpaceIndex - 1);
			SET @LastName = RIGHT(@Name, LEN(@Name) - @SpaceIndex);
		END
		ELSE
		BEGIN
			SET @FirstName = @Name;
			SET @LastName = '';
		END
		
		-- Insert new user
		INSERT INTO Users (
			FirstName,
			LastName,
			Email,
			Phone,
			PasswordHash,
			Salt,
			TenantId,
			Active,
			EmailVerified,
			PhoneVerified,
			LoginAttempts,
			AccountLocked,
			CreatedAt,
			UpdatedAt,
			AgreeToTerms,
			RoleId
		) VALUES (
			@FirstName,
			@LastName,
			@Email,
			@Phone,
			@PasswordHash,
			@Salt,
			@TenantId,
			1, -- Active
			0, -- EmailVerified
			0, -- PhoneVerified
			0, -- LoginAttempts
			0, -- AccountLocked
			GETUTCDATE(),
			GETUTCDATE(),
			@AgreeToTerms,
			5
		);
		
		-- Get the new user ID
		SET @UserId = SCOPE_IDENTITY();
		
		-- -- Assign default role to user
		-- INSERT INTO UserRoles (UserId, RoleId, CreatedAt)
		-- VALUES (@UserId, @DefaultRoleId, GETUTCDATE());
		
		-- Return user information with role
		SELECT 
			u.UserId,
			u.FirstName,
			u.LastName,
			u.Email,
			u.Phone,
			u.Active,
			u.TenantId,
			u.EmailVerified,
			u.PhoneVerified,
			u.CreatedAt,
			r.RoleId,
			r.RoleName,
			r.RoleDescription
		FROM Users u
		-- LEFT JOIN UserRoles ur ON u.UserId = ur.UserId
		INNER JOIN Roles r ON r.RoleId = u.RoleId
		WHERE u.UserId = @UserId;
		
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


CREATE PROCEDURE [dbo].[SP_USER_LOGIN]
	@EmailOrPhone NVARCHAR(100),
	@Password NVARCHAR(100),
	@RememberMe BIT = 0
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		DECLARE @UserId BIGINT = NULL;
		DECLARE @IsActive BIT = 0;
		DECLARE @PasswordHash NVARCHAR(255);
		DECLARE @Salt NVARCHAR(100);
		DECLARE @LoginAttempts INT = 0;
		DECLARE @AccountLocked BIT = 0;
		DECLARE @LastLoginAttempt DATETIME;
		
		-- Find user by email or phone
		SELECT 
			@UserId = UserId,
			@IsActive = Active,
			@PasswordHash = PasswordHash,
			@Salt = Salt,
			@LoginAttempts = LoginAttempts,
			@AccountLocked = AccountLocked,
			@LastLoginAttempt = LastLoginAttempt
		FROM Users 
		WHERE (Email = @EmailOrPhone OR Phone = @EmailOrPhone)
			AND Active = 1;
		
		-- Check if user exists
		IF @UserId IS NULL
		BEGIN
			RAISERROR('Invalid email/phone or password.', 16, 1);
			RETURN;
		END
		
		-- Check if account is locked
		IF @AccountLocked = 1
		BEGIN
			-- Check if lock period has expired (30 minutes)
			IF DATEDIFF(MINUTE, @LastLoginAttempt, GETUTCDATE()) < 30
			BEGIN
				RAISERROR('Account is temporarily locked due to multiple failed login attempts. Please try again later.', 16, 1);
				RETURN;
			END
			ELSE
			BEGIN
				-- Unlock account
				UPDATE Users 
				SET AccountLocked = 0, LoginAttempts = 0, LastLogout=NULL
				WHERE UserId = @UserId;
				SET @AccountLocked = 0;
				SET @LoginAttempts = 0;
			END
		END
		
		-- Verify password (In real implementation, you would hash the input password with salt)
		-- For demo purposes, we'll do a simple comparison
		IF @PasswordHash != CONVERT(NVARCHAR(100), HASHBYTES('SHA2_256', cast(@Password as NVARCHAR(100))+ @Salt), 2)
		BEGIN
			-- Increment login attempts
			SET @LoginAttempts = @LoginAttempts + 1;
			
			-- Lock account after 5 failed attempts
			IF @LoginAttempts >= 5
			BEGIN
				UPDATE Users 
				SET LoginAttempts = @LoginAttempts, 
					AccountLocked = 1, 
					LastLoginAttempt = GETUTCDATE()
				WHERE UserId = @UserId;
				
				RAISERROR('Account has been locked due to multiple failed login attempts.', 16, 1);
				RETURN;
			END
			ELSE
			BEGIN
				UPDATE Users 
				SET LoginAttempts = @LoginAttempts, 
					LastLoginAttempt = GETUTCDATE()
				WHERE UserId = @UserId;
				
				RAISERROR('Invalid email/phone or password.', 16, 1);
				RETURN;
			END
		END
		
		-- Successful login - reset attempts and update last login
		UPDATE Users 
		SET LoginAttempts = 0, 
			LastLogin = GETUTCDATE(),
			LastLoginAttempt = GETUTCDATE(),
			LastLogout=NULL,
			AccountLocked = 0
		WHERE UserId = @UserId;
		
		-- Return user information
		SELECT 
			u.UserId,
			u.FirstName,
			u.LastName,
			u.Email,
			u.Phone,
			u.Active,
			u.TenantId,
			u.LastLogin,
			u.RoleId,
			r.RoleName,
			r.RoleDescription,
			@RememberMe AS RememberMe
		FROM Users u
		-- LEFT JOIN UserRoles ur ON u.UserId = ur.UserId
		LEFT JOIN Roles r ON u.RoleId = r.RoleId
		WHERE u.UserId = @UserId
			AND u.Active = 1;
		
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_GET_USER_PROFILE]
	@UserId BIGINT,
	@TenantId BIGINT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		-- Validate user exists and is active
		IF NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId AND Active = 1)
		BEGIN
			RAISERROR('User not found or inactive.', 16, 1);
			RETURN;
		END
		
		-- Additional tenant validation if provided
		IF @TenantId IS NOT NULL
		BEGIN
			IF NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId AND TenantId = @TenantId AND Active = 1)
			BEGIN
				RAISERROR('User not found in the specified tenant.', 16, 1);
				RETURN;
			END
		END
		
		-- Get user profile information
		SELECT 
			u.UserId,
			u.FirstName,
			u.LastName,
			u.Email,
			u.Phone,
			u.Active,
			u.TenantId,
			u.EmailVerified,
			u.PhoneVerified,
			u.CreatedAt,
			u.UpdatedAt,
			u.LastLogin,
			u.LastLogout,
			u.ProfilePicture,
			u.DateOfBirth,
			u.Gender,
			u.Timezone,
			u.Language,
			u.Country,
			u.City,
			u.State,
			u.PostalCode,
			u.Bio,
			u.Website,
			u.CompanyName,
			u.JobTitle,
			u.PreferredContactMethod,
			u.NotificationSettings,
			u.PrivacySettings,
			-- User roles
			ur.RoleId,
			-- r.RoleName,
			-- r.RoleDescription,
			-- User addresses
			addr.AddressId,
			addr.AddressType,
			addr.Street,
			addr.City AS AddressCity,
			addr.State AS AddressState,
			addr.PostalCode AS AddressPostalCode,
			addr.Country AS AddressCountry,
			addr.IsDefault AS IsDefaultAddress
			-- User preferences
			-- up.PreferenceKey,
			-- up.PreferenceValue,
			-- up.PreferenceType
		FROM Users u
		LEFT JOIN UserRoles ur ON u.UserId = ur.UserId
		-- LEFT JOIN Roles r ON ur.RoleId = r.RoleId
		LEFT JOIN UserAddresses addr ON u.UserId = addr.UserId AND addr.Active = 1
		-- LEFT JOIN UserPreferences up ON u.UserId = up.UserId AND up.Active = 1
		WHERE u.UserId = @UserId 
			AND u.Active = 1
			AND (@TenantId IS NULL OR u.TenantId = @TenantId)
		ORDER BY ur.RoleId, addr.IsDefault DESC;
		
		-- Get user statistics (optional)
		SELECT 
			'LOGIN_COUNT' AS StatType,
			COUNT(*) AS StatValue
		FROM UserActivityLog 
		WHERE UserId = @UserId 
			AND ActivityType = 'LOGIN'
			AND CreatedAt >= DATEADD(MONTH, -12, GETUTCDATE())
		
		UNION ALL
		
		SELECT 
			'LAST_ACTIVITY' AS StatType,
			DATEDIFF(DAY, MAX(CreatedAt), GETUTCDATE()) AS StatValue
		FROM UserActivityLog 
		WHERE UserId = @UserId
		
		UNION ALL
		
		SELECT 
			'PROFILE_COMPLETION' AS StatType,
			CASE 
				WHEN ProfilePicture IS NOT NULL THEN 10 ELSE 0 END +
				CASE 
				WHEN DateOfBirth IS NOT NULL THEN 10 ELSE 0 END +
				CASE 
				WHEN Bio IS NOT NULL THEN 10 ELSE 0 END +
				CASE 
				WHEN Phone IS NOT NULL THEN 10 ELSE 0 END +
				CASE 
				WHEN EmailVerified = 1 THEN 20 ELSE 0 END +
				CASE 
				WHEN PhoneVerified = 1 THEN 20 ELSE 0 END +
				20 AS StatValue -- Base score for having name and email
		FROM Users 
		WHERE UserId = @UserId;
		
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

CREATE PROCEDURE [dbo].[SP_UPDATE_USER_PROFILE]
	@UserId BIGINT,
	@Name NVARCHAR(255) = NULL,
	@Phone NVARCHAR(50) = NULL,
	@DateOfBirth DATETIME = NULL,
	@Gender NVARCHAR(20) = NULL,
	@Bio NVARCHAR(MAX) = NULL,
	@Website NVARCHAR(255) = NULL,
	@CompanyName NVARCHAR(255) = NULL,
	@JobTitle NVARCHAR(255) = NULL,
	@Country NVARCHAR(100) = NULL,
	@City NVARCHAR(100) = NULL,
	@State NVARCHAR(100) = NULL,
	@PostalCode NVARCHAR(20) = NULL,
	@Timezone NVARCHAR(100) = NULL,
	@Language NVARCHAR(10) = NULL,
	@PreferredContactMethod NVARCHAR(50) = NULL,
	-- Address Information
	@AddressStreet NVARCHAR(255) = NULL,
	@AddressCity NVARCHAR(100) = NULL,
	@AddressState NVARCHAR(100) = NULL,
	@AddressZipCode NVARCHAR(20) = NULL,
	@AddressCountry NVARCHAR(100) = NULL,
	@AddressType NVARCHAR(50) = 'Home',
	@UpdateAddressIfExists BIT = 1
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		BEGIN TRANSACTION;
		
		-- Validate user exists and is active
		IF NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId AND Active = 1)
		BEGIN
			RAISERROR('User not found or inactive.', 16, 1);
			RETURN;
		END
		
		-- Parse name into first and last name if provided
		DECLARE @FirstName NVARCHAR(255) = NULL;
		DECLARE @LastName NVARCHAR(255) = NULL;
		
		IF @Name IS NOT NULL
		BEGIN
			DECLARE @SpaceIndex INT = CHARINDEX(' ', @Name);
			IF @SpaceIndex > 0
			BEGIN
				SET @FirstName = LEFT(@Name, @SpaceIndex - 1);
				SET @LastName = RIGHT(@Name, LEN(@Name) - @SpaceIndex);
			END
			ELSE
			BEGIN
				SET @FirstName = @Name;
				SET @LastName = '';
			END
		END
		
		-- Check if phone number already exists for another user
		IF @Phone IS NOT NULL AND EXISTS (SELECT 1 FROM Users WHERE Phone = @Phone AND UserId != @UserId)
		BEGIN
			RAISERROR('Phone number is already registered to another user.', 16, 1);
			RETURN;
		END
		
		-- Update user profile
		UPDATE Users
		SET 
			FirstName = ISNULL(@FirstName, FirstName),
			LastName = ISNULL(@LastName, LastName),
			Phone = ISNULL(@Phone, Phone),
			DateOfBirth = ISNULL(@DateOfBirth, DateOfBirth),
			Gender = ISNULL(@Gender, Gender),
			Bio = ISNULL(@Bio, Bio),
			Website = ISNULL(@Website, Website),
			CompanyName = ISNULL(@CompanyName, CompanyName),
			JobTitle = ISNULL(@JobTitle, JobTitle),
			Country = ISNULL(@Country, Country),
			City = ISNULL(@City, City),
			State = ISNULL(@State, State),
			PostalCode = ISNULL(@PostalCode, PostalCode),
			Timezone = ISNULL(@Timezone, Timezone),
			Language = ISNULL(@Language, Language),
			PreferredContactMethod = ISNULL(@PreferredContactMethod, PreferredContactMethod),
			UpdatedAt = GETUTCDATE()
		WHERE UserId = @UserId;
		
		-- Handle address update/creation if address information is provided
		IF @AddressStreet IS NOT NULL OR @AddressCity IS NOT NULL OR @AddressState IS NOT NULL 
			OR @AddressZipCode IS NOT NULL OR @AddressCountry IS NOT NULL
		BEGIN
			DECLARE @ExistingAddressId BIGINT = NULL;
			
			-- Check if user has an existing address of the specified type
			SELECT @ExistingAddressId = AddressId 
			FROM UserAddresses 
			WHERE UserId = @UserId 
				AND AddressType = @AddressType 
				AND Active = 1;
			
			IF @ExistingAddressId IS NOT NULL AND @UpdateAddressIfExists = 1
			BEGIN
				-- Update existing address
				UPDATE UserAddresses
				SET 
					Street = ISNULL(@AddressStreet, Street),
					City = ISNULL(@AddressCity, City),
					State = ISNULL(@AddressState, State),
					PostalCode = ISNULL(@AddressZipCode, PostalCode),
					Country = ISNULL(@AddressCountry, Country),
					UpdatedAt = GETUTCDATE()
				WHERE AddressId = @ExistingAddressId;
			END
			ELSE IF @ExistingAddressId IS NULL
			BEGIN
				-- Create new address
				INSERT INTO UserAddresses (
					UserId,
					AddressType,
					Street,
					City,
					State,
					PostalCode,
					Country,
					IsDefault,
					Active,
					CreatedAt,
					UpdatedAt
				) VALUES (
					@UserId,
					@AddressType,
					@AddressStreet,
					@AddressCity,
					@AddressState,
					@AddressZipCode,
					@AddressCountry,
					1, -- Set as default if it's the first address
					1,
					GETUTCDATE(),
					GETUTCDATE()
				);
			END
		END
		
		-- Log the profile update activity
		INSERT INTO UserActivityLog (
			UserId,
			ActivityType,
			ActivityDescription,
			CreatedAt
		) VALUES (
			@UserId,
			'PROFILE_UPDATE',
			'User profile updated',
			GETUTCDATE()
		);
		
		-- Return success status
		SELECT 
			@UserId AS UserId,
			'Profile updated successfully' AS Message,
			GETUTCDATE() AS UpdatedAt;
		
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

CREATE OR ALTER PROCEDURE [dbo].[SP_RESET_PASSWORD]
	@UserId BIGINT,
	@NewPassword NVARCHAR(100),
	@IpAddress NVARCHAR(45) = NULL,
	@UserAgent NVARCHAR(500) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		BEGIN TRANSACTION;

		DECLARE @TokenExpiry DATETIME = NULL;
		DECLARE @TokenUsed BIT = 0;
		DECLARE @Email NVARCHAR(255) = NULL;
		DECLARE @Salt NVARCHAR(50) = NULL;
		DECLARE @HashedPassword NVARCHAR(255) = NULL;
		DECLARE @CurrentTime DATETIME = GETUTCDATE();
		
		-- Validate reset token and get user information
		SELECT 
			@UserId = UserId
		FROM Users 
		WHERE Active = 1 AND UserId=@UserId;
		
		-- Check if token exists
		IF @UserId IS NULL
		BEGIN
			RAISERROR('Invalid or expired reset USER.', 16, 1);
			RETURN;
		END
		
		-- Generate a new salt and hash the password
		SET @Salt = CONVERT(NVARCHAR(50), NEWID());						  
		SET @HashedPassword = CONVERT(NVARCHAR(100), HASHBYTES('SHA2_256', cast(@NewPassword as NVARCHAR(100))+ @Salt), 2);
		
		-- Update user password	
		UPDATE Users
		SET 
			PasswordHash = @HashedPassword,
			Salt = @Salt,
			PasswordChangedAt = @CurrentTime,
			LastPasswordReset = @CurrentTime,
			UpdatedAt = @CurrentTime,
			-- Reset failed login attempts since password was successfully reset
			LoginAttempts = 0,
			AccountLocked = 0
		WHERE UserId = @UserId;		
		
		-- Log the password reset activity
		INSERT INTO UserActivityLog (
			UserId,
			ActivityType,
			ActivityDescription,
			IpAddress,
			UserAgent,
			CreatedAt
		) VALUES (
			@UserId,
			'PASSWORD_RESET',
			'Password reset successfully using reset token',
			@IpAddress,
			@UserAgent,
			@CurrentTime
		);
		
		-- -- Optional: Send notification email (placeholder for email service integration)
		-- -- This would typically trigger an email notification about successful password reset
		-- INSERT INTO UserNotifications (
		-- 	UserId,
		-- 	NotificationType,
		-- 	Title,
		-- 	Message,
		-- 	IsRead,
		-- 	CreatedAt
		-- ) VALUES (
		-- 	@UserId,
		-- 	'SECURITY_ALERT',
		-- 	'Password Reset Successful',
		-- 	'Your password has been successfully reset. If you did not perform this action, please contact support immediately.',
		-- 	0,
		-- 	@CurrentTime
		-- );
		
		-- Return success information
		SELECT 
			@UserId AS UserId,
			@Email AS Email,
			'Password reset successfully' AS Message,
			@CurrentTime AS ResetDate;
		
		COMMIT TRANSACTION;
		
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;
			
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		-- Log the failed password reset attempt
		IF @UserId IS NOT NULL
		BEGIN
			INSERT INTO UserActivityLog (
				UserId,
				ActivityType,
				ActivityDescription,
				IpAddress,
				UserAgent,
				CreatedAt
			) VALUES (
				@UserId,
				'PASSWORD_RESET_FAILED',
				'Failed password reset attempt: ' + @ErrorMessage,
				@IpAddress,
				@UserAgent,
				GETUTCDATE()
			);
		END
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

CREATE PROCEDURE [dbo].[SP_SEARCH_PRODUCTS]
	@TenantId BIGINT,
	@Page INT = 1,
	@Limit INT = 10,
	@Offset INT = 0,
	@Search NVARCHAR(255) = '',
	@Category INT = NULL,
	@MinPrice DECIMAL(18,2) = NULL,
	@MaxPrice DECIMAL(18,2) = NULL,
	@Rating INT = NULL,
	@InStock BIT = NULL,
	@BestSeller BIT = NULL,
	@HasOffer BIT = NULL,
	@SortBy NVARCHAR(50) = 'created',
	@SortOrder NVARCHAR(10) = 'desc'
AS
BEGIN
	SET NOCOUNT ON;
	
	-- Declare variables
	DECLARE @SQL NVARCHAR(MAX);
	DECLARE @WhereClause NVARCHAR(MAX) = '';
	DECLARE @OrderByClause NVARCHAR(MAX) = '';
	DECLARE @TotalCount INT = 0;
	
	-- Build WHERE clause dynamically
	SET @WhereClause = 'WHERE p.TenantId = ' + CAST(@TenantId AS NVARCHAR) + ' ';
	
	-- Search filter
	IF @Search IS NOT NULL AND @Search != ''
	BEGIN
		SET @WhereClause = @WhereClause + 
			'AND (p.ProductName LIKE ''%' + @Search + '%'' 
			OR p.ProductDescription LIKE ''%' + @Search + '%'' 
			OR p.ProductCode LIKE ''%' + @Search + '%'') ';
	END
	
	-- Category filter
	IF @Category IS NOT NULL
	BEGIN
		SET @WhereClause = @WhereClause + 'AND p.Category = ' + CAST(@Category AS NVARCHAR) + ' ';
	END
	
	-- Price range filter
	IF @MinPrice IS NOT NULL
	BEGIN
		SET @WhereClause = @WhereClause + 'AND p.Price >= ' + CAST(@MinPrice AS NVARCHAR) + ' ';
	END
	
	IF @MaxPrice IS NOT NULL
	BEGIN
		SET @WhereClause = @WhereClause + 'AND p.Price <= ' + CAST(@MaxPrice AS NVARCHAR) + ' ';
	END
	
	-- Rating filter
	IF @Rating IS NOT NULL
	BEGIN
		SET @WhereClause = @WhereClause + 'AND p.Rating >= ' + CAST(@Rating AS NVARCHAR) + ' ';
	END
	
	-- Stock filter
	IF @InStock IS NOT NULL
	BEGIN
		IF @InStock = 1
			SET @WhereClause = @WhereClause + 'AND p.Quantity > 0 ';
		ELSE
			SET @WhereClause = @WhereClause + 'AND p.Quantity = 0 ';
	END
	
	-- Best seller filter
	IF @BestSeller IS NOT NULL AND @BestSeller = 1
	BEGIN
		SET @WhereClause = @WhereClause + 'AND p.BestSeller = 1 ';
	END
	
	-- Has offer filter
	IF @HasOffer IS NOT NULL AND @HasOffer = 1
	BEGIN
		SET @WhereClause = @WhereClause + 'AND (p.Offer IS NOT NULL AND p.Offer != '''') ';
	END
	
	-- Active products only
	SET @WhereClause = @WhereClause + 'AND p.Active = 1 ';
	
	-- Build ORDER BY clause
	SET @OrderByClause = 'ORDER BY ';
	
	IF @SortBy = 'productName'
		SET @OrderByClause = @OrderByClause + 'p.ProductName ';
	ELSE IF @SortBy = 'price'
		SET @OrderByClause = @OrderByClause + 'p.Price ';
	ELSE IF @SortBy = 'rating'
		SET @OrderByClause = @OrderByClause + 'p.Rating ';
	ELSE IF @SortBy = 'userBuyCount'
		SET @OrderByClause = @OrderByClause + 'p.UserBuyCount ';
	ELSE
		SET @OrderByClause = @OrderByClause + 'p.Created ';
	
	IF @SortOrder = 'asc'
		SET @OrderByClause = @OrderByClause + 'ASC ';
	ELSE
		SET @OrderByClause = @OrderByClause + 'DESC ';
	
	-- Get total count first
	SET @SQL = '
	SELECT COUNT(*) as TotalCount
	FROM Products p 
	' + @WhereClause;
	
	-- Create temp table for count
	CREATE TABLE #TempCount (TotalCount INT);
	INSERT INTO #TempCount
	EXEC sp_executesql @SQL;
	
	SELECT @TotalCount = TotalCount FROM #TempCount;
	DROP TABLE #TempCount;
	
	-- Get paginated results
	SET @SQL = '
	SELECT 
		p.ProductId,
		p.TenantId,
		p.ProductName,
		p.ProductDescription,
		p.ProductCode,
		p.FullDescription,
		p.Specification,
		p.Story,
		p.PackQuantity,
		p.Quantity,
		p.Total,
		p.Price,
		p.Category,
		p.Rating,
		p.Active,
		p.Trending,
		p.UserBuyCount,
		p.[Return],
		p.Created,
		p.Modified,
		CASE WHEN p.Quantity > 0 THEN 1 ELSE 0 END as InStock,
		p.BestSeller,
		p.DeliveryDate,
		p.Offer,
		p.OrderBy,
		p.UserId,
		p.Overview,
		p.LongDescription
	FROM Products p 
	' + @WhereClause + @OrderByClause + '
	OFFSET ' + CAST(@Offset AS VARCHAR) + ' ROWS
	FETCH NEXT ' + CAST(@Limit AS VARCHAR) + ' ROWS ONLY';
	
	-- Execute main query
	EXEC sp_executesql @SQL;
	
	-- Return total count as second result set
	SELECT @TotalCount as TotalCount;
	
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_GET_IMAGE_BY_ID]
	@ImageId BIGINT
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		-- Validate input parameter
		IF @ImageId IS NULL OR @ImageId <= 0
		BEGIN
			RAISERROR('Invalid ImageId parameter.', 16, 1);
			RETURN;
		END
		
		-- Select image data by ImageId
		SELECT 
			pi.ImageId,
			pi.ProductId,
			pi.ImageName,
			pi.ContentType,
			pi.FileSize,
			pi.ImageData,        -- Full image data
			pi.ThumbnailData,    -- Thumbnail image data (can be NULL)
			pi.Poster,
			pi.Main,
			pi.Active,
			pi.OrderBy,
			pi.CreatedAt,
			pi.Modified,
			pi.CreatedBy
		FROM ProductImages pi
		WHERE pi.ImageId = @ImageId 
		AND pi.Active = 1;
		
		-- Note: If no rows are returned, the application will handle it as "image not found"
		
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO
-- USING
CREATE OR ALTER PROCEDURE [dbo].[SP_ADD_IMAGE_LIST]
	@ProductId BIGINT,
	@ImageName NVARCHAR(255),
	@ContentType NVARCHAR(100),
	@ImageData VARBINARY(MAX),
	@ThumbnailData VARBINARY(MAX) = NULL,
	@FileSize INT
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		BEGIN TRANSACTION;
		
		DECLARE @CurrentTime DATETIME = GETUTCDATE();
		DECLARE @NewImageId BIGINT;
		
		-- Validate that the product exists and is active
		IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductId = @ProductId AND Active = 1)
		BEGIN
			RAISERROR('Product not found or inactive.', 16, 1);
			RETURN -1;
		END
		
		-- Insert the new image
		INSERT INTO ProductImages (
			ProductId,
			ImageName,
			ContentType,
			FileSize,
			ImageData,
			ThumbnailData,
			Poster,
			Main,
			Active,
			OrderBy,
			CreatedAt,
			Modified,
			CreatedBy
		) VALUES (
			@ProductId,
			@ImageName,
			@ContentType,
			@FileSize,
			@ImageData,
			@ThumbnailData,
			NULL, -- Poster URL will be generated by the application
			0,    -- Not main by default
			1,    -- Active
			(SELECT ISNULL(MAX(OrderBy), 0) + 1 FROM ProductImages WHERE ProductId = @ProductId AND Active = 1),
			@CurrentTime,
			@CurrentTime,
			NULL  -- No user context in this simple endpoint
		);
		
		-- Get the newly inserted image ID
		SET @NewImageId = SCOPE_IDENTITY();
		
		-- Update product modified date
		UPDATE Products 
		SET Modified = @CurrentTime 
		WHERE ProductId = @ProductId;
		
		COMMIT TRANSACTION;
		
		-- Return the new image ID
		RETURN @NewImageId;
		
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;
			
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		RAISERROR(@ErrorMessage, 16, 1);
		RETURN -1;
	END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_GET_PRODUCT_LIST_FILTERED]
	@TenantId BIGINT,
	@CategoryId BIGINT = NULL,
	@Active BIT = NULL,
	@Page INT = 1,
	@Limit INT = 10,
	@SortBy NVARCHAR(50) = 'Created',
	@SortOrder NVARCHAR(10) = 'DESC'
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		-- Declare variables
		DECLARE @Offset INT;
		DECLARE @SQL NVARCHAR(MAX);
		DECLARE @WhereClause NVARCHAR(MAX) = '';
		DECLARE @OrderByClause NVARCHAR(MAX) = '';
		DECLARE @TotalCount INT = 0;
		
		-- Calculate offset for pagination
		SET @Offset = (@Page - 1) * @Limit;
		
		-- Build WHERE clause
		SET @WhereClause = 'WHERE p.TenantId = ' + CAST(@TenantId AS NVARCHAR(20)) + ' ';
		
		-- Category filter
		IF @CategoryId IS NOT NULL
		BEGIN
			SET @WhereClause = @WhereClause + 'AND p.Category = ' + CAST(@CategoryId AS NVARCHAR(20)) + ' ';
		END
		
		-- Active status filter
		IF @Active IS NOT NULL
		BEGIN
			SET @WhereClause = @WhereClause + 'AND p.Active = ' + CAST(@Active AS NVARCHAR(1)) + ' ';
		END
		ELSE
		BEGIN
			-- Default to active products only if not specified
			SET @WhereClause = @WhereClause + 'AND p.Active = 1 ';
		END
		
		-- Build ORDER BY clause
		SET @OrderByClause = 'ORDER BY ';
		
		IF LOWER(@SortBy) = 'productname'
			SET @OrderByClause = @OrderByClause + 'p.ProductName ';
		ELSE IF LOWER(@SortBy) = 'price'
			SET @OrderByClause = @OrderByClause + 'p.Price ';
		ELSE IF LOWER(@SortBy) = 'rating'
			SET @OrderByClause = @OrderByClause + 'p.Rating ';
		ELSE IF LOWER(@SortBy) = 'category'
			SET @OrderByClause = @OrderByClause + 'p.Category ';
		ELSE IF LOWER(@SortBy) = 'quantity'
			SET @OrderByClause = @OrderByClause + 'p.Quantity ';
		ELSE
			SET @OrderByClause = @OrderByClause + 'p.Created ';
		
		IF LOWER(@SortOrder) = 'asc'
			SET @OrderByClause = @OrderByClause + 'ASC ';
		ELSE
			SET @OrderByClause = @OrderByClause + 'DESC ';
		
		-- Get total count first
		SET @SQL = '
		SELECT COUNT(*) as TotalCount
		FROM Products p 
		' + @WhereClause;
		
		-- Create temp table for count
		CREATE TABLE #TempCount (TotalCount INT);
		INSERT INTO #TempCount
		EXEC sp_executesql @SQL;
		
		SELECT @TotalCount = TotalCount FROM #TempCount;
		DROP TABLE #TempCount;
		
		-- Get paginated product results
		SET @SQL = '
		SELECT 
			p.ProductId,
			p.TenantId,
			p.ProductName,
			p.ProductDescription,
			p.ProductCode,
			p.FullDescription,
			p.Specification,
			p.Story,
			p.PackQuantity,
			p.Quantity,
			p.Total,
			p.Price,
			p.Category,
			p.Rating,
			p.Active,
			p.Trending,
			p.UserBuyCount,
			p.[Return],
			p.Created,
			p.Modified,
			CASE WHEN p.Quantity > 0 THEN 1 ELSE 0 END as InStock,
			p.BestSeller,
			p.DeliveryDate,
			p.Offer,
			p.OrderBy,
			p.UserId,
			p.Overview,
			p.LongDescription,
			-- Category information (if needed)
			ISNULL(c.CategoryName, ''Unknown'') as CategoryName
		FROM Products p 
		LEFT JOIN Categories c ON p.Category = c.CategoryId AND c.TenantId = p.TenantId
		' + @WhereClause + @OrderByClause + '
		OFFSET ' + CAST(@Offset AS VARCHAR(10)) + ' ROWS
		FETCH NEXT ' + CAST(@Limit AS VARCHAR(10)) + ' ROWS ONLY';
		
		-- Execute main query
		EXEC sp_executesql @SQL;
		
		-- Return total count as second result set for pagination
		SELECT @TotalCount as TotalCount;
		
	END TRY
	BEGIN CATCH
		-- Error handling
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

CREATE PROCEDURE [dbo].[SP_GET_PRODUCT_BY_ID]
(
	@ProductId BIGINT
)
AS
BEGIN
		SELECT 
			p.ProductId,
			p.TenantId,
			p.ProductName,
			p.ProductDescription,
			p.ProductCode,
			p.FullDescription,
			p.Specification,
			p.Story,
			p.PackQuantity,
			p.Quantity,
			p.Total,
			p.Price,
			p.Category,
			p.Rating,
			p.Active,
			p.Trending,
			p.UserBuyCount,
			p.[Return],
			p.Created,
			p.Modified,
			CASE WHEN p.Quantity > 0 THEN 1 ELSE 0 END as InStock,
			p.BestSeller,
			p.DeliveryDate,
			p.Offer,
			p.OrderBy,
			p.UserId,
			p.Overview,
			p.LongDescription
		FROM Products p WITH (NOLOCK)
		WHERE p.ProductId = @ProductId
			AND p.Active = 1;

		-- Get product images
		SELECT 
			i.ImageId,
			i.ImageName as Poster,
			i.Main as [Main],
			i.Active,
			i.OrderBy
		FROM ProductImages i WITH (NOLOCK)
		WHERE i.ProductId = @ProductId
			AND i.Active = 1
		ORDER BY i.OrderBy;
	END
	GO

CREATE PROCEDURE [dbo].[SP_ADD_PRODUCT]
			@TenantId BIGINT,
			@ProductName NVARCHAR(255),
			@ProductDescription NVARCHAR(500),
			@ProductCode NVARCHAR(100),
			@FullDescription NVARCHAR(MAX),
			@Specification NVARCHAR(MAX),
			@Story NVARCHAR(MAX),
			@PackQuantity INT,
			@Quantity INT,
			@Total INT,
			@Price DECIMAL(18,2),
			@Category INT,
			@Rating INT,
			@Active BIT,
			@Trending INT,
			@UserBuyCount INT,
			@Return INT,
			@BestSeller BIT,
			@DeliveryDate INT,
			@Offer NVARCHAR(100),
			@OrderBy INT,
			@UserId BIGINT,
			@CreatedBy BIGINT
		AS
		BEGIN
			SET NOCOUNT ON;
			
			BEGIN TRY
				BEGIN TRANSACTION;
				
				-- Check if product code already exists for this tenant
				IF EXISTS (SELECT 1 FROM Products WHERE TenantId = @TenantId AND ProductCode = @ProductCode)
				BEGIN
					RAISERROR('Product code already exists for this tenant.', 16, 1);
					RETURN;
				END
				
				-- Insert new product
				INSERT INTO Products (
					TenantId,
					ProductName,
					ProductDescription,
					ProductCode,
					FullDescription,
					Specification,
					Story,
					PackQuantity,
					Quantity,
					Total,
					Price,
					Category,
					Rating,
					Active,
					Trending,
					UserBuyCount,
					[Return],
					BestSeller,
					DeliveryDate,
					Offer,
					OrderBy,
					UserId,
					Created,
					Modified,
					CreatedBy,
					ModifiedBy
				)
				VALUES (
					@TenantId,
					@ProductName,
					@ProductDescription,
					@ProductCode,
					@FullDescription,
					@Specification,
					@Story,
					@PackQuantity,
					@Quantity,
					@Total,
					@Price,
					@Category,
					@Rating,
					@Active,
					@Trending,
					@UserBuyCount,
					@Return,
					@BestSeller,
					@DeliveryDate,
					@Offer,
					@OrderBy,
					@UserId,
					GETUTCDATE(),
					GETUTCDATE(),
					@CreatedBy,
					@CreatedBy
				);
				
				-- Get the new product ID
				DECLARE @ProductId BIGINT = SCOPE_IDENTITY();
				
				-- Return the new product ID
				SELECT @ProductId AS ProductId;
				
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

	CREATE PROCEDURE [dbo].[SP_UPDATE_PRODUCT]
		@ProductId BIGINT,
		@TenantId BIGINT,
		@ProductName NVARCHAR(255),
		@ProductDescription NVARCHAR(500),
		@ProductCode NVARCHAR(100),
		@FullDescription NVARCHAR(MAX),
		@Specification NVARCHAR(MAX),
		@Story NVARCHAR(MAX),
		@PackQuantity INT,
		@Quantity INT,
		@Total INT,
		@Price DECIMAL(18,2),
		@Category INT,
		@Rating INT,
		@Active BIT,
		@Trending INT,
		@UserBuyCount INT,
		@Return INT,
		@BestSeller BIT,
		@DeliveryDate INT,
		@Offer NVARCHAR(100),
		@OrderBy INT,
		@UserId BIGINT,
		@ModifiedBy BIGINT
	AS
	BEGIN
		SET NOCOUNT ON;
		
		BEGIN TRY
			BEGIN TRANSACTION;
			
			-- Check if product exists and belongs to the tenant
			IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductId = @ProductId AND TenantId = @TenantId)
			BEGIN
				RAISERROR('Product not found or does not belong to this tenant.', 16, 1);
				RETURN;
			END
			
			-- Check if product code already exists for another product of this tenant
			IF EXISTS (
				SELECT 1 
				FROM Products 
				WHERE TenantId = @TenantId 
					AND ProductCode = @ProductCode 
					AND ProductId != @ProductId
			)
			BEGIN
				RAISERROR('Product code already exists for another product.', 16, 1);
				RETURN;
			END
			
			-- Update product
			UPDATE Products
			SET
				ProductName = @ProductName,
				ProductDescription = @ProductDescription,
				ProductCode = @ProductCode,
				FullDescription = @FullDescription,
				Specification = @Specification,
				Story = @Story,
				PackQuantity = @PackQuantity,
				Quantity = @Quantity,
				Total = @Total,
				Price = @Price,
				Category = @Category,
				Rating = @Rating,
				Active = @Active,
				Trending = @Trending,
				UserBuyCount = @UserBuyCount,
				[Return] = @Return,
				BestSeller = @BestSeller,
				DeliveryDate = @DeliveryDate,
				Offer = @Offer,
				OrderBy = @OrderBy,
				UserId = @UserId,
				Modified = GETUTCDATE(),
				ModifiedBy = @ModifiedBy
			WHERE ProductId = @ProductId
				AND TenantId = @TenantId;
			
			-- Return success
			SELECT @ProductId AS ProductId;
			
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


		CREATE PROCEDURE [dbo].[SP_DELETE_PRODUCT]
			@ProductId BIGINT,
			@TenantId BIGINT,
			@UserId BIGINT
		AS
		BEGIN
			SET NOCOUNT ON;
			
			BEGIN TRY
				BEGIN TRANSACTION;
				
				-- Check if product exists and belongs to the tenant
				IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductId = @ProductId AND TenantId = @TenantId)
				BEGIN
					RAISERROR('Product not found or does not belong to this tenant.', 16, 1);
					RETURN;
				END
				
				-- Check if product is referenced in any orders or carts
				IF EXISTS (
					SELECT 1 
					FROM CartItems 
					WHERE ProductId = @ProductId AND Active = 1
					OR EXISTS (SELECT 1 FROM OrderItems WHERE ProductId = @ProductId AND Active = 1)
				)
				BEGIN
					-- Soft delete - just mark as inactive
					UPDATE Products
					SET 
						Active = 0,
						Modified = GETUTCDATE(),
						ModifiedBy = @UserId
					WHERE ProductId = @ProductId
						AND TenantId = @TenantId;
				END
				ELSE
				BEGIN
					-- Hard delete - first delete related records
					DELETE FROM ProductImages WHERE ProductId = @ProductId;
					DELETE FROM ProductReviews WHERE ProductId = @ProductId;
					DELETE FROM ProductWishList WHERE ProductId = @ProductId;
					
					-- Then delete the product
					DELETE FROM Products 
					WHERE ProductId = @ProductId 
						AND TenantId = @TenantId;
				END
				
				-- Return success
				SELECT @ProductId AS ProductId;
				
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


				
		Create PROCEDURE [dbo].[SP_ADD_ITEM_TO_CART]
			@UserId BIGINT,
			@ProductId BIGINT,
			@Quantity INT, -- Can be positive (add) or negative (subtract)
			@TenantId BIGINT = NULL,
			@SessionId NVARCHAR(255) = NULL,
			@IpAddress NVARCHAR(45) = NULL,
			@UserAgent NVARCHAR(500) = NULL
		AS
		BEGIN
			SET NOCOUNT ON;
			SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
			
			BEGIN TRY
				BEGIN TRANSACTION;
				
				DECLARE @ExistingCartId BIGINT = NULL;
				DECLARE @ExistingQuantity INT = 0;
				DECLARE @ProductPrice DECIMAL(18,2) = 0;
				DECLARE @ProductName NVARCHAR(255) = '';
				DECLARE @AvailableStock INT = 0;
				DECLARE @ProductActive BIT = 0;
				DECLARE @CurrentTime DATETIME = GETUTCDATE();
				DECLARE @NewQuantity INT = 0;
				DECLARE @OperationType NVARCHAR(20) = '';
				
				-- Validate user exists and is active (with lock to ensure consistency)
				IF NOT EXISTS (SELECT 1 FROM Users WITH (UPDLOCK, ROWLOCK) WHERE UserId = @UserId AND Active = 1)
				BEGIN
					IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
					RAISERROR('User not found or inactive.', 16, 1);
					RETURN;
				END
				
				-- Validate product exists, is active, and get product details (with lock to prevent deadlock)
				SELECT 
					@ProductPrice = Price,
					@ProductName = ProductName,
					@AvailableStock = Quantity,
					@ProductActive = Active
				FROM Products WITH (UPDLOCK, ROWLOCK)
				WHERE ProductId = @ProductId
					AND (@TenantId IS NULL OR TenantId = @TenantId);
				
				IF @ProductActive IS NULL OR @ProductActive = 0
				BEGIN
					IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
					RAISERROR('Product not found or inactive.', 16, 1);
					RETURN;
				END
				
				-- Validate quantity is not zero
				IF @Quantity = 0
				BEGIN
					IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
					RAISERROR('Quantity cannot be zero.', 16, 1);
					RETURN;
				END
				
				-- Check if item already exists in cart (with UPDLOCK to prevent deadlock and ensure exclusive access)
				SELECT 
					@ExistingCartId = CartId,
					@ExistingQuantity = Quantity
				FROM CartItems WITH (UPDLOCK, ROWLOCK)
				WHERE UserId = @UserId 
					AND ProductId = @ProductId 
					AND Active = 1
					AND (@TenantId IS NULL OR TenantId = @TenantId);
				
				-- Calculate new quantity
				SET @NewQuantity = @ExistingQuantity + @Quantity;
				
				-- Determine operation type for logging
				IF @Quantity > 0
					SET @OperationType = 'INCREASE';
				ELSE
					SET @OperationType = 'DECREASE';
				
				-- Validate new quantity is not negative
				IF @NewQuantity < 0
				BEGIN
					IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
					RAISERROR('Cannot reduce quantity below zero. Current quantity',16,1);
					RETURN;
				END
				
				-- If new quantity is 0, remove the item from cart
				IF @NewQuantity = 0
				BEGIN
					IF @ExistingCartId IS NOT NULL
					BEGIN
						DELETE FROM CartItems WHERE CartId = @ExistingCartId;
						
						-- Log the cart item removal
						INSERT INTO UserActivityLog (
							UserId,
							ActivityType,
							ActivityDescription,
							IpAddress,
							UserAgent,
							CreatedAt
						) VALUES (
							@UserId,
							'CART_ITEM_REMOVED',
							'Removed from cart: ' + @ProductName + ' (Quantity reduced to 0)',
							@IpAddress,
							@UserAgent,
							@CurrentTime
						);
						
						-- Return removal confirmation
						SELECT 
							@ExistingCartId AS CartId,
							@UserId AS UserId,
							@ProductId AS ProductId,
							@ProductName AS ProductName,
							0 AS Quantity,
							@ProductPrice AS Price,
							0 AS ItemTotal,
							'Product removed from cart (quantity reduced to 0)' AS Message,
							@CurrentTime AS UpdatedDate,
							'REMOVED' AS OperationType;
					END
					ELSE
					BEGIN
						IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
						RAISERROR('Product not found in cart.', 16, 1);
						RETURN;
					END
				END
				ELSE
				BEGIN
					-- Check stock availability for positive quantities
					IF @NewQuantity > @AvailableStock
					BEGIN
						IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
						RAISERROR('Insufficient stock. Available quantity: %d, Requested quantity: %d.', 16, 1, @AvailableStock, @NewQuantity);
						RETURN;
					END
				
					IF @ExistingCartId IS NOT NULL
					BEGIN
						-- Update existing cart item
						UPDATE CartItems
						SET 
							Quantity = @NewQuantity,
							UpdatedDate = @CurrentTime,
							SessionId = ISNULL(@SessionId, SessionId)
						WHERE CartId = @ExistingCartId;
						
						-- Log the cart update activity
						INSERT INTO UserActivityLog (
							UserId,
							ActivityType,
							ActivityDescription,
							IpAddress,
							UserAgent,
							CreatedAt
						) VALUES (
							@UserId,
							CASE 
								WHEN @OperationType = 'INCREASE' THEN 'CART_QUANTITY_INCREASED'
								ELSE 'CART_QUANTITY_DECREASED'
							END,
							@OperationType + ' cart item: ' + @ProductName + ' (From: ' + CAST(@ExistingQuantity AS VARCHAR(10)) + ' To: ' + CAST(@NewQuantity AS VARCHAR(10)) + ')',
							@IpAddress,
							@UserAgent,
							@CurrentTime
						);
						
						-- Return updated cart item info
						SELECT 
							@ExistingCartId AS CartId,
							@UserId AS UserId,
							@ProductId AS ProductId,
							@ProductName AS ProductName,
							@NewQuantity AS Quantity,
							@ProductPrice AS Price,
							(@NewQuantity * @ProductPrice) AS ItemTotal,
							CASE 
								WHEN @OperationType = 'INCREASE' THEN 'Product quantity increased in cart'
								ELSE 'Product quantity decreased in cart'
							END AS Message,
							@CurrentTime AS UpdatedDate,
							@OperationType AS OperationType;
					END
					ELSE
					BEGIN
						-- Can only add new items with positive quantities
						IF @Quantity <= 0
						BEGIN
							IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
							RAISERROR('Cannot add new item with zero or negative quantity. Product not found in cart.', 16, 1);
							RETURN;
						END
						
						-- Insert new cart item
						INSERT INTO CartItems (
							UserId,
							ProductId,
							Quantity,
							TenantId,
							SessionId,
							Active,
							AddedDate,
							UpdatedDate
						) VALUES (
							@UserId,
							@ProductId,
							@Quantity,
							@TenantId,
							@SessionId,
							1,
							@CurrentTime,
							@CurrentTime
						);
						
						SET @ExistingCartId = SCOPE_IDENTITY();
						
						-- Log the cart addition activity
						INSERT INTO UserActivityLog (
							UserId,
							ActivityType,
							ActivityDescription,
							IpAddress,
							UserAgent,
							CreatedAt
						) VALUES (
							@UserId,
							'CART_ADD',
							'Added to cart: ' + @ProductName + ' (Quantity: ' + CAST(@Quantity AS VARCHAR(10)) + ')',
							@IpAddress,
							@UserAgent,
							@CurrentTime
						);
						
						-- Return new cart item info
						SELECT 
							@ExistingCartId AS CartId,
							@UserId AS UserId,
							@ProductId AS ProductId,
							@ProductName AS ProductName,
							@Quantity AS Quantity,
							@ProductPrice AS Price,
							(@Quantity * @ProductPrice) AS ItemTotal,
							'Product added to cart successfully' AS Message,
							@CurrentTime AS AddedDate,
							'INCREASE' AS OperationType;
					END
				END
				
				-- Get updated cart summary (moved before cleanup to reduce lock time)
				SELECT 
					COUNT(*) AS TotalUniqueItems,
					ISNULL(SUM(ci.Quantity), 0) AS TotalQuantity,
					ISNULL(SUM(ci.Quantity * p.Price), 0) AS TotalAmount
				FROM CartItems ci WITH (READPAST)
				INNER JOIN Products p WITH (READPAST) ON ci.ProductId = p.ProductId
				WHERE ci.UserId = @UserId 
					AND ci.Active = 1
					AND p.Active = 1
					AND (@TenantId IS NULL OR ci.TenantId = @TenantId);
				
				COMMIT TRANSACTION;
				
				-- Optional: Clean up old inactive cart items for this user (housekeeping - done outside transaction to avoid deadlocks)
				-- This is a best-effort cleanup that won't block other transactions
				BEGIN TRY
					DELETE FROM CartItems 
					WHERE UserId = @UserId 
						AND Active = 0 
						AND UpdatedDate < DATEADD(DAY, -30, @CurrentTime);
				END TRY
				BEGIN CATCH
					-- Ignore cleanup errors - they're not critical
				END CATCH
				
			END TRY
			BEGIN CATCH
				IF @@TRANCOUNT > 0
					ROLLBACK TRANSACTION;
					
				DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
				DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
				DECLARE @ErrorState INT = ERROR_STATE();
				
				-- Log the failed cart operation with more specific details
				IF @UserId IS NOT NULL
				BEGIN
					INSERT INTO UserActivityLog (
						UserId,
						ActivityType,
						ActivityDescription,
						IpAddress,
						UserAgent,
						CreatedAt
					) VALUES (
						@UserId,
						CASE 
							WHEN @Quantity > 0 THEN 'CART_INCREASE_FAILED'
							WHEN @Quantity < 0 THEN 'CART_DECREASE_FAILED'
							ELSE 'CART_OPERATION_FAILED'
						END,
						'Failed cart operation (ProductId: ' + CAST(@ProductId AS VARCHAR(10)) + ', Quantity: ' + CAST(@Quantity AS VARCHAR(10)) + '): ' + @ErrorMessage,
						@IpAddress,
						@UserAgent,
						GETUTCDATE()
					);
				END
				
				RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
			END CATCH
		END
		GO

		CREATE PROCEDURE [dbo].[SP_REMOVE_ITEM_FROM_CART]
				@UserId BIGINT,
				@ProductId BIGINT,
				@TenantId BIGINT = NULL,
				@RemoveCompletely BIT = 1,
				@IpAddress NVARCHAR(45) = NULL,
				@UserAgent NVARCHAR(500) = NULL
			AS
			BEGIN
				SET NOCOUNT ON;
				
				BEGIN TRY
					BEGIN TRANSACTION;
					
					DECLARE @ExistingCartId BIGINT = NULL;
					DECLARE @CurrentQuantity INT = 0;
					DECLARE @ProductName NVARCHAR(255) = '';
					DECLARE @ProductPrice DECIMAL(18,2) = 0;
					DECLARE @CurrentTime DATETIME = GETUTCDATE();
					DECLARE @RemovedQuantity INT = 0;
					DECLARE @ItemTotal DECIMAL(18,2) = 0;
					
					-- Validate user exists and is active
					IF NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId AND Active = 1)
					BEGIN
						RAISERROR('User not found or inactive.', 16, 1);
						RETURN;
					END
					
					-- Check if item exists in cart and get details
					SELECT 
						@ExistingCartId = ci.CartId,
						@CurrentQuantity = ci.Quantity,
						@ProductName = p.ProductName,
						@ProductPrice = p.Price
					FROM CartItems ci
					INNER JOIN Products p ON ci.ProductId = p.ProductId
					WHERE ci.UserId = @UserId 
						AND ci.ProductId = @ProductId 
						AND ci.Active = 1
						AND p.Active = 1
						AND (@TenantId IS NULL OR ci.TenantId = @TenantId);
					
					-- Check if cart item exists
					IF @ExistingCartId IS NULL
					BEGIN
						RAISERROR('Product not found in cart.', 16, 1);
						RETURN;
					END
					
					-- Calculate removed quantity and item total
					SET @RemovedQuantity = @CurrentQuantity;
					SET @ItemTotal = @RemovedQuantity * @ProductPrice;
					
					-- Remove the item from cart (mark as inactive or delete)
					IF @RemoveCompletely = 1
					BEGIN
						-- Permanently delete the cart item
						DELETE FROM CartItems 
						WHERE CartId = @ExistingCartId;
					END
					ELSE
					BEGIN
						-- Mark as inactive (soft delete)
						UPDATE CartItems
						SET 
							Active = 0,
							UpdatedDate = @CurrentTime
						WHERE CartId = @ExistingCartId;
					END
					
					-- Log the cart removal activity
					INSERT INTO UserActivityLog (
						UserId,
						ActivityType,
						ActivityDescription,
						IpAddress,
						UserAgent,
						CreatedAt
					) VALUES (
						@UserId,
						'CART_REMOVE',
						'Removed from cart: ' + @ProductName + ' (Quantity: ' + CAST(@RemovedQuantity AS VARCHAR(10)) + ')',
						@IpAddress,
						@UserAgent,
						@CurrentTime
					);
					
					-- Return removed item details
					SELECT 
						@ExistingCartId AS CartId,
						@UserId AS UserId,
						@ProductId AS ProductId,
						@ProductName AS ProductName,
						@RemovedQuantity AS RemovedQuantity,
						@ProductPrice AS Price,
						@ItemTotal AS ItemTotal,
						'Product removed from cart successfully' AS Message,
						@CurrentTime AS RemovedDate;
					
					-- Get updated cart summary
					SELECT 
						COUNT(*) AS TotalUniqueItems,
						ISNULL(SUM(ci.Quantity), 0) AS TotalQuantity,
						ISNULL(SUM(ci.Quantity * p.Price), 0) AS TotalAmount
					FROM CartItems ci
					INNER JOIN Products p ON ci.ProductId = p.ProductId
					WHERE ci.UserId = @UserId 
						AND ci.Active = 1
						AND p.Active = 1
						AND (@TenantId IS NULL OR ci.TenantId = @TenantId);
					
					-- Optional: Get recommended products to replace the removed item
					SELECT TOP 5
						p.ProductId,
						p.ProductName,
						p.Price,
						p.Rating,
						p.BestSeller,
						p.Offer,
						pi.Poster AS ImageUrl
					FROM Products p
					LEFT JOIN ProductImages pi ON p.ProductId = pi.ProductId AND pi.Main = 1 AND pi.Active = 1
					WHERE p.Active = 1
						AND p.ProductId != @ProductId  -- Exclude the removed product
						AND p.Category = (
							SELECT Category 
							FROM Products 
							WHERE ProductId = @ProductId
						)
						AND (@TenantId IS NULL OR p.TenantId = @TenantId)
					ORDER BY p.Rating DESC, p.UserBuyCount DESC;
					
					-- Optional: Clean up old inactive cart items for this user (housekeeping)
					DELETE FROM CartItems 
					WHERE UserId = @UserId 
						AND Active = 0 
						AND UpdatedDate < DATEADD(DAY, -30, @CurrentTime);
					
					COMMIT TRANSACTION;
					
				END TRY
				BEGIN CATCH
					IF @@TRANCOUNT > 0
						ROLLBACK TRANSACTION;
						
					DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
					DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
					DECLARE @ErrorState INT = ERROR_STATE();
					
					-- Log the failed cart removal
					IF @UserId IS NOT NULL
					BEGIN
						INSERT INTO UserActivityLog (
							UserId,
							ActivityType,
							ActivityDescription,
							IpAddress,
							UserAgent,
							CreatedAt
						) VALUES (
							@UserId,
							'CART_REMOVE_FAILED',
							'Failed to remove item from cart: ' + @ErrorMessage,
							@IpAddress,
							@UserAgent,
							GETUTCDATE()
						);
					END
					
					RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
				END CATCH
			END
			GO

			CREATE OR ALTER PROCEDURE [dbo].[SP_CLEAR_CART]
				@UserId BIGINT,
				@TenantId BIGINT = NULL,
				@ClearCompletely BIT = 1,
				@IpAddress NVARCHAR(45) = NULL,
				@UserAgent NVARCHAR(500) = NULL
			AS
			BEGIN
				SET NOCOUNT ON;
				
				BEGIN TRY
					BEGIN TRANSACTION;
					
					DECLARE @CartItemCount INT = 0;
					DECLARE @TotalQuantity INT = 0;
					DECLARE @TotalValue DECIMAL(18,2) = 0;
					DECLARE @CurrentTime DATETIME = GETUTCDATE();
					DECLARE @AffectedRows INT = 0;
					
					-- Validate user exists and is active
					IF NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId AND Active = 1)
					BEGIN
						RAISERROR('User not found or inactive.', 16, 1);
						RETURN;
					END
					
					-- Get current cart statistics before clearing
					SELECT 
						@CartItemCount = COUNT(*),
						@TotalQuantity = ISNULL(SUM(ci.Quantity), 0),
						@TotalValue = ISNULL(SUM(ci.Quantity * p.Price), 0)
					FROM CartItems ci
					INNER JOIN Products p ON ci.ProductId = p.ProductId
					WHERE ci.UserId = @UserId 
						-- AND ci.Active = 1
						AND p.Active = 1
						AND (@TenantId IS NULL OR ci.TenantId = @TenantId);
					
					-- Check if cart has any items
					IF @CartItemCount = 0
					BEGIN
						RAISERROR('Cart is already empty.', 16, 1);
						RETURN;
					END
					
					-- Clear the cart based on strategy
					IF @ClearCompletely = 1
					BEGIN
						-- Permanently delete all cart items
						DELETE FROM CartItems 
						WHERE UserId = @UserId 
							-- AND Active = 1
							AND (@TenantId IS NULL OR TenantId = @TenantId);
						
						SET @AffectedRows = @@ROWCOUNT;
					END
					ELSE
					BEGIN
						-- Mark all cart items as inactive (soft delete)
						UPDATE CartItems
						SET 
							Active = 0,
							UpdatedDate = @CurrentTime
						WHERE UserId = @UserId 
							-- AND Active = 1
							AND (@TenantId IS NULL OR TenantId = @TenantId);
						
						SET @AffectedRows = @@ROWCOUNT;
					END
					
					-- Verify that items were actually cleared
					IF @AffectedRows = 0
					BEGIN
						RAISERROR('Failed to clear cart items.', 16, 1);
						RETURN;
					END
					
					-- Log the cart clearing activity
					INSERT INTO UserActivityLog (
						UserId,
						ActivityType,
						ActivityDescription,
						IpAddress,
						UserAgent,
						CreatedAt
					) VALUES (
						@UserId,
						'CART_CLEAR',
						'Cart cleared - ' + CAST(@CartItemCount AS VARCHAR(10)) + ' items removed (Total Value: $' + CAST(@TotalValue AS VARCHAR(20)) + ')',
						@IpAddress,
						@UserAgent,
						@CurrentTime
					);
					
					-- Return clearing operation details
					SELECT 
						@UserId AS UserId,
						@CartItemCount AS ClearedItemCount,
						@TotalQuantity AS ClearedQuantity,
						@TotalValue AS ClearedValue,
						'Cart cleared successfully' AS Message,
						@CurrentTime AS ClearedDate,
						@ClearCompletely AS WasHardDelete;
					
					-- Get updated cart summary (should be empty)
					SELECT 
						COUNT(*) AS TotalUniqueItems,
						ISNULL(SUM(ci.Quantity), 0) AS TotalQuantity,
						ISNULL(SUM(ci.Quantity * p.Price), 0) AS TotalAmount
					FROM CartItems ci
					INNER JOIN Products p ON ci.ProductId = p.ProductId
					WHERE ci.UserId = @UserId 
						AND ci.Active = 1
						AND p.Active = 1
						AND (@TenantId IS NULL OR ci.TenantId = @TenantId);
					
					-- Optional: Get popular products for cart rebuilding suggestions
					SELECT TOP 10
						p.ProductId,
						p.ProductName,
						p.Price,
						p.Rating,
						p.BestSeller,
						p.Trending,
						p.UserBuyCount,
						p.Offer,
						pi.Poster AS ImageUrl,
						cat.CategoryName AS CategoryName
					FROM Products p
					LEFT JOIN ProductImages pi ON p.ProductId = pi.ProductId AND pi.Main = 1 AND pi.Active = 1
					LEFT JOIN Categories cat ON p.Category = cat.CategoryId
					WHERE p.Active = 1
						AND p.InStock = 1
						AND (@TenantId IS NULL OR p.TenantId = @TenantId)
					ORDER BY 
						p.BestSeller DESC,
						p.Rating DESC, 
						p.UserBuyCount DESC,
						p.Trending DESC;
					
					-- Optional: Clean up old inactive cart items for this user (housekeeping)
					DELETE FROM CartItems 
					WHERE UserId = @UserId 
						AND Active = 0 
						AND UpdatedDate < DATEADD(DAY, -30, @CurrentTime);
					
					-- -- Optional: Record user behavior for analytics
					-- INSERT INTO UserBehaviorAnalytics (
					-- 	UserId,
					-- 	ActionType,
					-- 	ActionDetails,
					-- 	ItemCount,
					-- 	TotalValue,
					-- 	CreatedAt
					-- ) VALUES (
					-- 	@UserId,
					-- 	'CART_CLEARED',
					-- 	'User cleared entire cart',
					-- 	@CartItemCount,
					-- 	@TotalValue,
					-- 	@CurrentTime
					-- );
					
					COMMIT TRANSACTION;
					
				END TRY
				BEGIN CATCH
					IF @@TRANCOUNT > 0
						ROLLBACK TRANSACTION;
						
					DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
					DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
					DECLARE @ErrorState INT = ERROR_STATE();
					
					-- Log the failed cart clearing
					IF @UserId IS NOT NULL
					BEGIN
						INSERT INTO UserActivityLog (
							UserId,
							ActivityType,
							ActivityDescription,
							IpAddress,
							UserAgent,
							CreatedAt
						) VALUES (
							@UserId,
							'CART_CLEAR_FAILED',
							'Failed to clear cart: ' + @ErrorMessage,
							@IpAddress,
							@UserAgent,
							GETUTCDATE()
						);
					END
					
					RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
				END CATCH
			END
			GO

			CREATE PROCEDURE [dbo].[SP_GET_USER_CART]
				@UserId BIGINT,
				@TenantId BIGINT = NULL
			AS
			BEGIN
				SET NOCOUNT ON;
				
				BEGIN TRY
					-- Validate user exists and is active
					IF NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId AND Active = 1)
					BEGIN
						RAISERROR('User not found or inactive.', 16, 1);
						RETURN;
					END
					
					-- Get cart items with full product details
					SELECT 
						-- Cart Information
						c.CartId,
						c.UserId,
						c.TenantId,
						c.Quantity,
						c.AddedDate,
						c.UpdatedDate,
						c.SessionId,
						
						-- Product Information
						p.ProductId,
						p.ProductName,
						p.ProductDescription,
						p.ProductCode,
						p.FullDescription,
						p.Specification,
						p.Story,
						p.PackQuantity,
						p.Quantity AS ProductAvailableQuantity,
						p.Total AS ProductTotal,
						p.Price,
						p.Category AS CategoryId,
						p.Rating,
						p.Active AS ProductActive,
						p.Trending,
						p.UserBuyCount,
						p.[Return] AS ReturnPolicy,
						p.Created AS ProductCreated,
						p.Modified AS ProductModified,
						p.InStock,
						p.BestSeller,
						p.DeliveryDate,
						p.Offer,
						p.OrderBy AS ProductOrderBy,
						p.UserId AS ProductUserId,
						p.Overview,
						p.LongDescription,
						
						-- Category Information
						cat.CategoryName AS CategoryName,
						cat.Active AS CategoryActive,
						cat.OrderBy AS CategoryOrderBy,
						cat.Description AS CategoryDescription,
						cat.Icon AS CategoryIcon,
						cat.HasSubMenu AS CategorySubMenu,
						
						-- Calculated Fields
						(c.Quantity * p.Price) AS ItemTotal,
						CASE 
							WHEN p.Quantity >= c.Quantity THEN 1 
							ELSE 0 
						END AS IsAvailable,
						
						-- Product Images (will be handled in a separate query or joined)
						pi.ImageId,
						pi.Poster AS ImageUrl,
						pi.Main AS IsMainImage,
						pi.Active AS ImageActive,
						pi.OrderBy AS ImageOrderBy
						
					FROM CartItems c
					INNER JOIN Products p ON c.ProductId = p.ProductId
					LEFT JOIN Categories cat ON p.Category = cat.CategoryId
					LEFT JOIN ProductImages pi ON p.ProductId = pi.ProductId AND pi.Active = 1
					WHERE c.UserId = @UserId 
						AND c.Active = 1
						AND p.Active = 1
						AND (@TenantId IS NULL OR c.TenantId = @TenantId)
					ORDER BY c.AddedDate DESC, pi.OrderBy ASC;
					
					-- Get cart summary
					SELECT 
						COUNT(*) AS TotalItems,
						SUM(c.Quantity) AS TotalQuantity,
						SUM(c.Quantity * p.Price) AS TotalAmount,
						SUM(CASE WHEN p.Quantity >= c.Quantity THEN (c.Quantity * p.Price) ELSE 0 END) AS AvailableItemsTotal,
						COUNT(CASE WHEN p.Quantity < c.Quantity THEN 1 END) AS UnavailableItems
					FROM CartItems c
					INNER JOIN Products p ON c.ProductId = p.ProductId
					WHERE c.UserId = @UserId 
						AND c.Active = 1
						AND p.Active = 1
						AND (@TenantId IS NULL OR c.TenantId = @TenantId);
						
					-- Get related/recommended products (optional - based on cart items)
					SELECT TOP 10
						p.ProductId,
						p.ProductName,
						p.Price,
						p.Rating,
						p.BestSeller,
						p.Offer,
						pi.Poster AS ImageUrl
					FROM Products p
					LEFT JOIN ProductImages pi ON p.ProductId = pi.ProductId AND pi.Main = 1 AND pi.Active = 1
					WHERE p.Active = 1
						AND p.ProductId NOT IN (
							SELECT c.ProductId 
							FROM CartItems c 
							WHERE c.UserId = @UserId AND c.Active = 1
						)
						AND p.Category IN (
							SELECT DISTINCT p2.Category
							FROM CartItems c2
							INNER JOIN Products p2 ON c2.ProductId = p2.ProductId
							WHERE c2.UserId = @UserId AND c2.Active = 1
						)
						AND (@TenantId IS NULL OR p.TenantId = @TenantId)
					ORDER BY p.Rating DESC, p.UserBuyCount DESC;
					
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
-- SP_GET_USER_WISHLIST_ITEMS
-- Get user's wishlist items with full product details
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[SP_GET_USER_WISHLIST_ITEMS]
	@UserId BIGINT,
	@TenantId BIGINT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		-- Validate user exists and is active
		IF NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId AND Active = 1)
		BEGIN
			RAISERROR('User not found or inactive.', 16, 1);
			RETURN;
		END
		
		-- Get wishlist items with full product details
		SELECT 
			-- Wishlist Information
			w.WishListId,
			w.UserId,
			w.ProductId,
			w.TenantId,
			w.Priority,
			w.Notes,
			w.Active AS WishlistItemActive,
			w.CreatedAt AS AddedAt,
			w.UpdatedAt,
			
			-- Product Information
			p.ProductId AS ProductId,
			p.ProductName,
			p.ProductDescription,
			p.ProductCode,
			p.FullDescription,
			p.Specification,
			p.Story,
			p.PackQuantity,
			p.Quantity AS ProductAvailableQuantity,
			p.Total AS ProductTotal,
			p.Price AS CurrentPrice,
			p.Price AS OriginalPrice, -- Can be updated if you have a separate original price field
			p.Category AS CategoryId,
			p.Rating,
			p.Active AS ProductActive,
			p.Trending,
			p.UserBuyCount,
			p.[Return] AS ReturnPolicy,
			p.Created AS ProductCreated,
			p.Modified AS ProductModified,
			-- p.InStock removed - using calculated InStock field below that checks both InStock flag and quantity
			p.BestSeller,
			p.DeliveryDate,
			p.Offer,
			p.OrderBy AS ProductOrderBy,
			p.UserId AS ProductUserId,
			p.Overview,
			p.LongDescription,
			
			-- Category Information
			cat.CategoryName,
			cat.Active AS CategoryActive,
			cat.OrderBy AS CategoryOrderBy,
			cat.Description AS CategoryDescription,
			cat.Icon AS CategoryIcon,
			cat.HasSubMenu AS CategorySubMenu,
			
		-- Product Images (main image, or first active image if no main image)
		pi.ImageId,
		pi.Poster AS ProductImage,
		pi.Main AS IsMainImage,
		pi.Active AS ImageActive,
		pi.OrderBy AS ImageOrderBy,
		
		-- Calculated Fields - In stock if quantity > 0 (quantity is the source of truth)
		CASE 
			WHEN p.Quantity > 0 THEN 1 
			ELSE 0 
		END AS InStock,
		CASE 
			WHEN p.Offer IS NOT NULL AND p.Offer != '' THEN 1 
			ELSE 0 
		END AS OnSale,
		ISNULL(p.Rating, 0) AS Rating,
		0 AS TotalReviews, -- Can be calculated from ProductReviews table if needed
		CASE 
			WHEN p.Offer IS NOT NULL AND p.Offer != '' AND ISNUMERIC(p.Offer) = 1 
			THEN CAST(p.Offer AS DECIMAL(5,2))
			ELSE 0 
		END AS DiscountPercentage
		
	FROM ProductWishList w
	INNER JOIN Products p ON w.ProductId = p.ProductId
	LEFT JOIN Categories cat ON p.Category = cat.CategoryId
	LEFT JOIN (
		-- Get main image if exists, otherwise get first active image
		SELECT 
			pi1.ProductId,
			pi1.ImageId,
			pi1.Poster,
			pi1.Main,
			pi1.Active,
			pi1.OrderBy,
			ROW_NUMBER() OVER (PARTITION BY pi1.ProductId ORDER BY CASE WHEN pi1.Main = 1 THEN 0 ELSE 1 END, pi1.OrderBy) AS rn
		FROM ProductImages pi1
		WHERE pi1.Active = 1
	) pi ON p.ProductId = pi.ProductId AND pi.rn = 1
		WHERE w.UserId = @UserId 
			AND w.Active = 1
			AND p.Active = 1
			AND (@TenantId IS NULL OR w.TenantId = @TenantId)
		ORDER BY w.CreatedAt DESC;
		
		-- Get wishlist summary
		SELECT 
			COUNT(*) AS TotalItems,
			SUM(p.Price) AS TotalValue,
			COUNT(CASE WHEN p.Quantity > 0 THEN 1 END) AS InStockItems,
			COUNT(CASE WHEN p.Quantity = 0 THEN 1 END) AS OutOfStockItems
		FROM ProductWishList w
		INNER JOIN Products p ON w.ProductId = p.ProductId
		WHERE w.UserId = @UserId 
			AND w.Active = 1
			AND p.Active = 1
			AND (@TenantId IS NULL OR w.TenantId = @TenantId);
			
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

CREATE PROCEDURE [dbo].[SP_UPSERT_WISHLIST]
	@UserId BIGINT,
	@ProductId BIGINT,
	@Quantity BIGINT = 1
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		DECLARE @CurrentTime DATETIME = GETUTCDATE();
		DECLARE @WishListId BIGINT = NULL;
		DECLARE @TenantId BIGINT = NULL;
		
		-- Get tenant ID from product
		SELECT @TenantId = TenantId
		FROM Products
		WHERE ProductId = @ProductId AND Active = 1;
		
		IF @TenantId IS NULL
		BEGIN
			RAISERROR('Product not found or inactive.', 16, 1);
			RETURN;
		END
		
		-- Check if wishlist item already exists (regardless of Active status to handle unique constraint)
		SELECT @WishListId = WishListId
		FROM ProductWishList
		WHERE UserId = @UserId 
			AND ProductId = @ProductId;
		
		IF @WishListId IS NOT NULL
		BEGIN
			-- Update existing wishlist item (reactivate if it was soft-deleted)
			UPDATE ProductWishList
			SET 
				Active = 1, -- Reactivate if it was soft-deleted
				UpdatedAt = @CurrentTime,
				Priority = 3, -- Default priority
				TenantId = @TenantId -- Update tenant ID in case it changed
			WHERE WishListId = @WishListId;
		END
		ELSE
		BEGIN
			-- Insert new wishlist item
			INSERT INTO ProductWishList (
				UserId,
				ProductId,
				TenantId,
				Priority,
				Notes,
				Active,
				CreatedAt,
				UpdatedAt
			)
			VALUES (
				@UserId,
				@ProductId,
				@TenantId,
				3, -- Default priority (Low)
				NULL, -- No notes by default
				1, -- Active
				@CurrentTime,
				@CurrentTime
			);
			
			SET @WishListId = SCOPE_IDENTITY();
		END
		
		-- Return wishlist item with product details (to avoid extra call in service)
		SELECT 
			w.WishListId,
			w.UserId,
			w.ProductId,
			w.TenantId,
			w.Priority,
			w.Notes,
			w.Active,
			w.CreatedAt,
			w.UpdatedAt,
			@Quantity AS Quantity,
			-- Product details
			p.ProductName,
			p.Price,
			p.ProductDescription,
			p.Rating,
			p.InStock,
			p.BestSeller,
			p.Offer,
			-- Product image (main image)
			pi.ImageId,
			pi.Poster AS ImageUrl,
			pi.Main AS IsMainImage
		FROM ProductWishList w
		INNER JOIN Products p ON w.ProductId = p.ProductId
		LEFT JOIN ProductImages pi ON p.ProductId = pi.ProductId AND pi.Main = 1 AND pi.Active = 1
		WHERE w.WishListId = @WishListId;
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
-- SP_REMOVE_WISHLIST
-- Remove product from wishlist (soft delete by setting Active = 0)
-- Returns result set with WishListId instead of OUTPUT parameter
-- =============================================
CREATE PROCEDURE [dbo].[SP_REMOVE_WISHLIST]
	@UserId BIGINT,
	@ProductId BIGINT
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		DECLARE @CurrentTime DATETIME = GETUTCDATE();
		DECLARE @WishListId BIGINT = NULL;
		DECLARE @RowsAffected INT = 0;
		
		-- Check if wishlist item exists
		SELECT @WishListId = WishListId
		FROM ProductWishList
		WHERE UserId = @UserId 
			AND ProductId = @ProductId 
			AND Active = 1;
		
		IF @WishListId IS NULL
		BEGIN
			-- Return 0 if item not found
			SELECT 0 AS WishListId;
			RETURN;
		END
		
		-- Soft delete: Set Active = 0
		UPDATE ProductWishList
		SET 
			Active = 0,
			UpdatedAt = @CurrentTime
		WHERE WishListId = @WishListId;
		
		SET @RowsAffected = @@ROWCOUNT;
		
		-- Return the WishListId (should be > 0 if successful)
		SELECT @WishListId AS WishListId;
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		-- Return 0 on error
		SELECT 0 AS WishListId;
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

-- =============================================
-- SP_CLEAR_WISHLIST
-- Clear entire wishlist for a user (soft delete by setting Active = 0 or hard delete)
-- Returns result set with clearing statistics
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[SP_CLEAR_WISHLIST]
	@UserId BIGINT,
	@TenantId BIGINT = NULL,
	@ClearCompletely BIT = 1,
	@IpAddress NVARCHAR(45) = NULL,
	@UserAgent NVARCHAR(500) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		BEGIN TRANSACTION;
		
		DECLARE @WishlistItemCount INT = 0;
		DECLARE @CurrentTime DATETIME = GETUTCDATE();
		DECLARE @AffectedRows INT = 0;
		
		-- Validate user exists and is active
		IF NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId AND Active = 1)
		BEGIN
			RAISERROR('User not found or inactive.', 16, 1);
			RETURN;
		END
		
		-- Get current wishlist statistics before clearing
		SELECT 
			@WishlistItemCount = COUNT(*)
		FROM ProductWishList pw
		INNER JOIN Products p ON pw.ProductId = p.ProductId
		WHERE pw.UserId = @UserId 
			AND pw.Active = 1
			AND p.Active = 1
			AND (@TenantId IS NULL OR pw.TenantId = @TenantId);
		
		-- Check if wishlist has any items
		IF @WishlistItemCount = 0
		BEGIN
			RAISERROR('Wishlist is already empty.', 16, 1);
			RETURN;
		END
		
		-- Clear the wishlist based on strategy
		IF @ClearCompletely = 1
		BEGIN
			-- Permanently delete all wishlist items
			DELETE FROM ProductWishList 
			WHERE UserId = @UserId 
				AND Active = 1
				AND (@TenantId IS NULL OR TenantId = @TenantId);
			
			SET @AffectedRows = @@ROWCOUNT;
		END
		ELSE
		BEGIN
			-- Mark all wishlist items as inactive (soft delete)
			UPDATE ProductWishList
			SET 
				Active = 0,
				UpdatedAt = @CurrentTime
			WHERE UserId = @UserId 
				AND Active = 1
				AND (@TenantId IS NULL OR TenantId = @TenantId);
			
			SET @AffectedRows = @@ROWCOUNT;
		END
		
		-- Verify that items were actually cleared
		IF @AffectedRows = 0
		BEGIN
			RAISERROR('Failed to clear wishlist items.', 16, 1);
			RETURN;
		END
		
		-- Log the wishlist clearing activity
		INSERT INTO UserActivityLog (
			UserId,
			ActivityType,
			ActivityDescription,
			IpAddress,
			UserAgent,
			CreatedAt
		) VALUES (
			@UserId,
			'WISHLIST_CLEAR',
			'Wishlist cleared - ' + CAST(@WishlistItemCount AS VARCHAR(10)) + ' items removed',
			@IpAddress,
			@UserAgent,
			@CurrentTime
		);
		
		-- Return clearing operation details
		SELECT 
			@UserId AS UserId,
			@WishlistItemCount AS ClearedItemCount,
			'Wishlist cleared successfully' AS Message,
			@CurrentTime AS ClearedDate,
			@ClearCompletely AS WasHardDelete;
		
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

CREATE PROCEDURE [dbo].[SP_GET_IMAGES_BY_PRODUCT_ID]
	@ProductId BIGINT
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		-- Validate input parameter
		IF @ProductId IS NULL OR @ProductId <= 0
		BEGIN
			RAISERROR('Invalid ProductId parameter.', 16, 1);
			RETURN;
		END
		
		-- Check if product exists
		IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductId = @ProductId AND Active = 1)
		BEGIN
			RAISERROR('Product not found or inactive.', 16, 1);
			RETURN;
		END
		
		-- Get all active images for the product
		SELECT 
			pi.ImageId,
			pi.ProductId,
			pi.ImageName,
			pi.ContentType,
			pi.FileSize,
			pi.Poster,
			pi.Main,
			pi.Active,
			pi.OrderBy,
			pi.AltText,
			pi.Caption,
			pi.CreatedAt,
			pi.Modified,
			p.TenantId
		FROM ProductImages pi
		INNER JOIN Products p ON pi.ProductId = p.ProductId
		WHERE pi.ProductId = @ProductId 
			AND pi.Active = 1
		ORDER BY pi.OrderBy, pi.CreatedAt;
		
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

CREATE PROCEDURE [dbo].[SP_GET_ORDERS]
			@UserId BIGINT,
			@TenantId BIGINT = NULL,
			@Page INT = 1,
			@Limit INT = 10,
			@Status NVARCHAR(50) = NULL,
			@Search NVARCHAR(100) = NULL
		AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		DECLARE @Offset INT;
		DECLARE @TotalCount INT;
		
		-- Calculate offset for pagination
		SET @Offset = (@Page - 1) * @Limit;
		
		-- Validate user exists and is active
		IF NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId AND Active = 1)
		BEGIN
			RAISERROR('User not found or inactive.', 16, 1);
			RETURN;
		END
		
		-- Get total count for pagination
		SELECT @TotalCount = COUNT(*)
		FROM Orders o
		WHERE o.UserId = @UserId
			AND o.Active = 1
			AND (@TenantId IS NULL OR o.TenantId = @TenantId)
			AND (@Status IS NULL OR o.OrderStatus = @Status)
			AND (@Search IS NULL OR o.OrderNumber LIKE '%' + @Search + '%');
		
		-- Get orders with pagination
		SELECT 
			o.OrderId,
			o.OrderNumber,
			o.OrderStatus,
			o.PaymentStatus,
			o.TotalAmount,
			o.Subtotal,
			o.ShippingAmount,
			o.TaxAmount,
			o.DiscountAmount,
			o.CouponId,
			o.CouponCode,
			o.CouponDiscountAmount,
			o.Notes,
			o.CreatedAt,
			o.UpdatedAt,
			-- Calculate item count and total quantity
			COUNT(oi.OrderItemId) AS TotalItems,
			ISNULL(SUM(oi.Quantity), 0) AS TotalQuantity,
			-- Pagination info
			@TotalCount AS TotalCount,
			@Page AS CurrentPage,
			@Limit AS PageSize,
			CEILING(CAST(@TotalCount AS FLOAT) / @Limit) AS TotalPages,
			CASE WHEN @Page < CEILING(CAST(@TotalCount AS FLOAT) / @Limit) THEN 1 ELSE 0 END AS HasNext,
			CASE WHEN @Page > 1 THEN 1 ELSE 0 END AS HasPrevious
		FROM Orders o
		LEFT JOIN OrderItems oi ON o.OrderId = oi.OrderId AND oi.Active = 1
		WHERE o.UserId = @UserId
			AND o.Active = 1
			AND (@TenantId IS NULL OR o.TenantId = @TenantId)
			AND (@Status IS NULL OR o.OrderStatus = @Status)
			AND (@Search IS NULL OR o.OrderNumber LIKE '%' + @Search + '%')
		GROUP BY o.OrderId, o.OrderNumber, o.OrderStatus, o.PaymentStatus, o.TotalAmount, 
					o.Subtotal, o.ShippingAmount, o.TaxAmount, o.DiscountAmount, o.CouponId,
					o.CouponCode, o.CouponDiscountAmount, o.Notes, o.CreatedAt, o.UpdatedAt
		ORDER BY o.CreatedAt DESC
		OFFSET @Offset ROWS
		FETCH NEXT @Limit ROWS ONLY;
		
		-- Get order items for each order in the result set
		SELECT 
			oi.OrderId,
			oi.OrderItemId,
			oi.ProductId,
			oi.ProductName,
			oi.ProductImage,
			oi.Price,
			oi.Quantity,
			oi.Total,
			p.ProductCode,
			p.Category,
			p.Rating,
			p.Offer
		FROM OrderItems oi
		LEFT JOIN Products p ON oi.ProductId = p.ProductId
		WHERE oi.OrderId IN (
			SELECT o.OrderId
			FROM Orders o
			WHERE o.UserId = @UserId
				AND o.Active = 1
				AND (@TenantId IS NULL OR o.TenantId = @TenantId)
				AND (@Status IS NULL OR o.OrderStatus = @Status)
				AND (@Search IS NULL OR o.OrderNumber LIKE '%' + @Search + '%')
			ORDER BY o.CreatedAt DESC
			OFFSET @Offset ROWS
			FETCH NEXT @Limit ROWS ONLY
		)
		AND oi.Active = 1
		ORDER BY oi.OrderId, oi.OrderItemId;
		
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_CREATE_ORDER]
	@UserId BIGINT,
	@TenantId BIGINT,
	@SessionId NVARCHAR(255) = NULL,
	@OrderType NVARCHAR(50) = 'registered',
	@OrderStatus NVARCHAR(50) = 'pending',
	@PaymentStatus NVARCHAR(50) = 'pending',
	@Subtotal DECIMAL(18,2),
	@ShippingAmount DECIMAL(18,2) = 0,
	@TaxAmount DECIMAL(18,2) = 0,
	@DiscountAmount DECIMAL(18,2) = 0,
	@TotalAmount DECIMAL(18,2),
	@CurrencyCode NVARCHAR(3) = 'INR',
	@Notes NVARCHAR(1000) = NULL,
	@SpecialInstructions NVARCHAR(1000) = NULL,
	@OrderDate DATETIME2(7) = NULL,
	@Source NVARCHAR(50) = 'web',
	@IpAddress NVARCHAR(45) = NULL,
	@UserAgent NVARCHAR(500) = NULL,
	@Referrer NVARCHAR(500) = NULL,
	@ShippingAddress NVARCHAR(MAX), -- JSON
	@PaymentMethod NVARCHAR(MAX), -- JSON
	@ShippingMethod NVARCHAR(MAX), -- JSON
	@AppliedDiscount NVARCHAR(MAX) = NULL, -- JSON
	@OrderItems NVARCHAR(MAX), -- JSON array of order items
	@OrderNumber NVARCHAR(50) OUTPUT,
	@OrderId BIGINT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @CurrentTime DATETIME2(7) = GETUTCDATE();
	DECLARE @TempOrderItems TABLE (
		ProductId BIGINT,
		ProductName NVARCHAR(255),
		ProductImage NVARCHAR(500),
		ProductCode NVARCHAR(100),
		Price DECIMAL(18,2),
		Quantity INT,
		Total DECIMAL(18,2)
	);

	BEGIN TRY
		BEGIN TRANSACTION;
		
		-- Validate user exists
		IF NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId AND Active = 1)
		BEGIN
			RAISERROR('User not found or inactive.', 16, 1);
			RETURN;
		END
		
		-- Validate tenant exists
		IF NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId AND TenantId = @TenantId)
		BEGIN
			RAISERROR('User does not belong to the specified tenant.', 16, 1);
			RETURN;
		END
		
		-- Parse and validate order items
		INSERT INTO @TempOrderItems (ProductId, ProductName, ProductImage, ProductCode, Price, Quantity, Total)
		SELECT 
			CAST(JSON_VALUE(value, '$.ProductId') AS BIGINT),
			JSON_VALUE(value, '$.ProductName'),
			JSON_VALUE(value, '$.ProductImage'),
			JSON_VALUE(value, '$.ProductCode'),
			CAST(JSON_VALUE(value, '$.Price') AS DECIMAL(18,2)),
			CAST(JSON_VALUE(value, '$.Quantity') AS INT),
			CAST(JSON_VALUE(value, '$.Total') AS DECIMAL(18,2))
		FROM OPENJSON(@OrderItems);
		
		-- -- Validate all products exist and are active
		-- IF EXISTS (
		--     SELECT 1 
		--     FROM @TempOrderItems t
		--     LEFT JOIN Products p ON t.ProductId = p.ProductId
		--     WHERE p.ProductId IS NULL OR p.Active = 0
		-- )
		-- BEGIN
		--     RAISERROR('One or more products not found, inactive, or do not belong to the tenant.', 16, 1);
		--     RETURN;
		-- END
		
		-- Check stock availability before creating order
		IF EXISTS (
			SELECT 1 
			FROM @TempOrderItems t
			INNER JOIN Products p ON t.ProductId = p.ProductId
			WHERE p.Quantity < t.Quantity
		)
		BEGIN
			RAISERROR('Insufficient stock for one or more products.', 16, 1);
			RETURN;
		END
		
		-- Generate order number if not provided
		IF @OrderNumber IS NULL OR @OrderNumber = ''
		BEGIN
			SET @OrderNumber = 'ORD-' + FORMAT(@CurrentTime, 'yyyyMMdd') + '-' + RIGHT('000000' + CAST(ABS(CHECKSUM(NEWID())) % 1000000 AS VARCHAR), 6);
		END
		
		-- Set order date if not provided
		IF @OrderDate IS NULL
		BEGIN
			SET @OrderDate = @CurrentTime;
		END
		
		-- Extract coupon information from AppliedDiscount JSON if provided
		DECLARE @CouponId BIGINT = NULL;
		DECLARE @CouponCode NVARCHAR(50) = NULL;
		DECLARE @CouponDiscountAmount DECIMAL(18,2) = NULL;
		
		IF @AppliedDiscount IS NOT NULL AND @AppliedDiscount != ''
		BEGIN
			-- First, try to get couponId directly from JSON (preferred method)
			SET @CouponId = CAST(JSON_VALUE(@AppliedDiscount, '$.CouponId') AS BIGINT);
			SET @CouponCode = JSON_VALUE(@AppliedDiscount, '$.Code');
			
			-- If couponId is not in JSON, look it up by code
			IF @CouponId IS NULL AND @CouponCode IS NOT NULL
			BEGIN
				SELECT @CouponId = CouponId
				FROM Coupons
				WHERE Code = @CouponCode AND TenantId = @TenantId AND Active = 1;
			END
			
			-- Get discount amount from JSON or use the discount amount parameter
			SET @CouponDiscountAmount = CAST(JSON_VALUE(@AppliedDiscount, '$.DiscountAmount') AS DECIMAL(18,2));
			IF @CouponDiscountAmount IS NULL OR @CouponDiscountAmount = 0
			BEGIN
				SET @CouponDiscountAmount = @DiscountAmount;
			END
		END
		
		-- Insert into Orders table
		INSERT INTO Orders (
			UserId, TenantId, OrderNumber, OrderStatus, PaymentStatus, TotalAmount, Subtotal,
			ShippingAmount, TaxAmount, DiscountAmount, CurrencyCode, Notes, SpecialInstructions,
			OrderDate, Source, IpAddress, UserAgent, Referrer, SessionId, OrderType,
			ShippingAddress, PaymentMethod, ShippingMethod, AppliedDiscount,
			CouponId, CouponCode, CouponDiscountAmount,
			CreatedAt, UpdatedAt, Active, CreatedBy
		)
		VALUES (
			@UserId, @TenantId, @OrderNumber, @OrderStatus, @PaymentStatus, @TotalAmount, @Subtotal,
			@ShippingAmount, @TaxAmount, @DiscountAmount, @CurrencyCode, @Notes, @SpecialInstructions,
			@OrderDate, @Source, @IpAddress, @UserAgent, @Referrer, @SessionId, @OrderType,
			@ShippingAddress, @PaymentMethod, @ShippingMethod, @AppliedDiscount,
			@CouponId, @CouponCode, @CouponDiscountAmount,
			@CurrentTime, @CurrentTime, 1, @UserId
		);
		
		-- Get the inserted OrderId
		SET @OrderId = SCOPE_IDENTITY();
		
		-- Record coupon usage if coupon was applied
		-- Check if we have a valid coupon (either by ID or by having a discount amount)
		IF @CouponId IS NOT NULL
		BEGIN
			-- Ensure we have a discount amount (use @DiscountAmount if @CouponDiscountAmount is NULL or 0)
			IF @CouponDiscountAmount IS NULL OR @CouponDiscountAmount = 0
			BEGIN
				SET @CouponDiscountAmount = @DiscountAmount;
			END
			
			-- Only insert usage if we have a valid discount amount
			IF @CouponDiscountAmount IS NOT NULL AND @CouponDiscountAmount > 0
			BEGIN
				-- Check if CouponUsage table exists before inserting
				IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'CouponUsage')
				BEGIN
					BEGIN TRY
						-- Insert into CouponUsage table
						INSERT INTO CouponUsage (
							CouponId, OrderId, UserId, DiscountAmount, OrderAmount, UsedAt
						)
						VALUES (
							@CouponId, @OrderId, @UserId, @CouponDiscountAmount, @TotalAmount, @CurrentTime
						);
					END TRY
					BEGIN CATCH
						-- Log error but don't fail the order if CouponUsage insert fails
						-- This allows orders to complete even if usage tracking has issues
						DECLARE @UsageErrorMsg NVARCHAR(4000) = ERROR_MESSAGE();
						-- Continue with order creation even if usage tracking fails
					END CATCH
				END
				
				-- Update coupon usage count (this should always work if coupon exists)
				BEGIN TRY
					UPDATE Coupons
					SET UsageCount = UsageCount + 1,
						UpdatedAt = @CurrentTime
					WHERE CouponId = @CouponId;
				END TRY
				BEGIN CATCH
					-- Log error but don't fail the order
					DECLARE @CouponUpdateErrorMsg NVARCHAR(4000) = ERROR_MESSAGE();
					-- Continue with order creation
				END CATCH
			END
		END
		
		-- Insert order items
		INSERT INTO OrderItems (
			OrderId, ProductId, ProductName, ProductImage, ProductCode, Price, Quantity, Total,
			DiscountAmount, TaxAmount, Active, CreatedAt, UpdatedAt
		)
		SELECT 
			@OrderId, ProductId, ProductName, ProductImage, ProductCode, Price, Quantity, Total,
			0, 0, 1, @CurrentTime, @CurrentTime
		FROM @TempOrderItems;
		
		-- INVENTORY MANAGEMENT: Update product inventory (reduce stock)
		UPDATE p
		SET 
			p.Quantity = p.Quantity - oi.Quantity,
			p.UserBuyCount = p.UserBuyCount + oi.Quantity,
			p.Modified = @CurrentTime
		FROM Products p
		INNER JOIN OrderItems oi ON p.ProductId = oi.ProductId
		WHERE oi.OrderId = @OrderId 
			AND oi.Active = 1
			AND p.Active = 1;
		
		-- Double-check for insufficient stock after update
		IF EXISTS (
			SELECT 1 
			FROM Products p
			INNER JOIN OrderItems oi ON p.ProductId = oi.ProductId
			WHERE oi.OrderId = @OrderId 
				AND p.Quantity < 0
		)
		BEGIN
			RAISERROR('Insufficient stock for one or more products after inventory update.', 16, 1);
			RETURN;
		END
		
		-- Clear user's cart items that were ordered
		UPDATE CartItems
		SET 
			Active = 0,
			UpdatedDate = @CurrentTime
		WHERE UserId = @UserId 
			AND ProductId IN (
				SELECT ProductId 
				FROM OrderItems 
				WHERE OrderId = @OrderId AND Active = 1
			)
			AND Active = 1
			AND (@TenantId IS NULL OR TenantId = @TenantId);
		
		-- ADDRESS MANAGEMENT: Insert shipping address into UserAddresses table
		-- Check if this address already exists for the user
		IF NOT EXISTS (
			SELECT 1 FROM UserAddresses 
			WHERE UserId = @UserId 
				AND Street = JSON_VALUE(@ShippingAddress, '$.Address1')
				AND City = JSON_VALUE(@ShippingAddress, '$.City')
				AND State = JSON_VALUE(@ShippingAddress, '$.State')
				AND PostalCode = JSON_VALUE(@ShippingAddress, '$.ZipCode')
				AND Country = JSON_VALUE(@ShippingAddress, '$.Country')
				AND Active = 1
		)
		BEGIN
			-- Insert new shipping address
			INSERT INTO UserAddresses (
				UserId, AddressType, Street, City, State, PostalCode, Country,
				IsDefault, Active, CreatedAt, UpdatedAt
			)
			VALUES (
				@UserId,
				'Shipping',
				JSON_VALUE(@ShippingAddress, '$.Address1'),
				JSON_VALUE(@ShippingAddress, '$.City'),
				JSON_VALUE(@ShippingAddress, '$.State'),
				JSON_VALUE(@ShippingAddress, '$.ZipCode'),
				JSON_VALUE(@ShippingAddress, '$.Country'),
				0, -- Not default address
				1, -- Active
				@CurrentTime,
				@CurrentTime
			);
			
			-- Log address creation activity
			INSERT INTO UserActivityLog (
				UserId, ActivityType, ActivityDescription, IpAddress, UserAgent,
				ResourceType, ResourceId, SessionId, PerformedBy, CreatedAt
			)
			VALUES (
				@UserId, 'ADDRESS_CREATED',
				'New shipping address added for order: ' + @OrderNumber,
				@IpAddress, @UserAgent, 'Address', SCOPE_IDENTITY(), @SessionId, @UserId, @CurrentTime
			);
		END;
        
        -- -- Insert initial status history
        -- INSERT INTO OrderStatusHistory (
        --     OrderId, PreviousStatus, NewStatus, StatusNote, ChangedBy, ChangedAt, CreatedAt
        -- )
        -- VALUES (
        --     @OrderId, NULL, @OrderStatus, 'Order created successfully', @UserId, @CurrentTime, @CurrentTime
        -- );
        
        -- -- Insert initial tracking record
        -- INSERT INTO OrderTracking (
        --     OrderId, TrackingNumber, Carrier, TrackingStatus, EstimatedDelivery, ShippingCost,
        --     Active, CreatedAt, UpdatedAt
        -- )
        -- SELECT 
        --     @OrderId, NULL, JSON_VALUE(@ShippingMethod, '$.carrier'), 'pending',
        --     CASE 
        --         WHEN JSON_VALUE(@ShippingMethod, '$.estimatedDays') IS NOT NULL 
        --         THEN DATEADD(DAY, CAST(JSON_VALUE(@ShippingMethod, '$.estimatedDays') AS INT), @CurrentTime)
        --         ELSE NULL
        --     END,
        --     @ShippingAmount, 1, @CurrentTime, @CurrentTime;
        
        -- -- Log the order creation activity
        -- INSERT INTO UserActivityLog (
        --     UserId, ActivityType, ActivityDescription, IpAddress, UserAgent,
        --     ResourceType, ResourceId, SessionId, PerformedBy, CreatedAt
        -- )
        -- VALUES (
        --     @UserId, 'ORDER_CREATED',
        --     'Order created with Order Number: ' + @OrderNumber + ' containing ' + CAST((SELECT COUNT(*) FROM @TempOrderItems) AS NVARCHAR(10)) + ' items',
        --     @IpAddress, @UserAgent, 'Order', @OrderId, @SessionId, @UserId, @CurrentTime
        -- );
        
        -- -- Log inventory update activity for each product
        -- INSERT INTO UserActivityLog (
        --     UserId, ActivityType, ActivityDescription, IpAddress, UserAgent,
        --     ResourceType, ResourceId, SessionId, PerformedBy, CreatedAt
        -- )
        -- SELECT 
        --     @UserId, 'INVENTORY_UPDATED',
        --     'Inventory reduced for product: ' + p.ProductName + ' by ' + CAST(oi.Quantity AS NVARCHAR(10)) + ' units (Order: ' + @OrderNumber + ')',
        --     @IpAddress, @UserAgent, 'Product', p.ProductId, @SessionId, @UserId, @CurrentTime
        -- FROM Products p
        -- INNER JOIN OrderItems oi ON p.ProductId = oi.ProductId
        -- WHERE oi.OrderId = @OrderId AND oi.Active = 1;
        
        COMMIT TRANSACTION;
        
        -- Return success with order details
        SELECT 
            @OrderId AS OrderId,
            @OrderNumber AS OrderNumber,
            @UserId AS UserId,
            (SELECT COUNT(*) FROM @TempOrderItems) AS ItemCount,
            @TotalAmount AS TotalAmount,
            @OrderStatus AS OrderStatus,
            @PaymentStatus AS PaymentStatus,
            'Order created successfully' AS Message,
            @CurrentTime AS CreatedDate;
            
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        -- Log the error
        INSERT INTO UserActivityLog (
            UserId, ActivityType, ActivityDescription, IpAddress, UserAgent,
            SessionId, PerformedBy, CreatedAt
        )
        VALUES (
            @UserId, 'ORDER_CREATE_ERROR',
            'Error creating order: ' + ERROR_MESSAGE() + ' (Line: ' + CAST(ERROR_LINE() AS NVARCHAR(10)) + ')',
            @IpAddress, @UserAgent, @SessionId, @UserId, @CurrentTime
        );
        
        -- Re-raise the error
        THROW;
    END CATCH
END
GO


CREATE OR ALTER PROCEDURE [dbo].[SP_ADMIN_GET_ALL_USERS]
    @AdminUserId BIGINT,
    @TenantId BIGINT = NULL,
    @Page INT = 1,
    @Limit INT = 10,
    @Search NVARCHAR(100) = NULL,
    @Status NVARCHAR(50) = NULL,   -- 'active' | 'locked' | 'inactive' | NULL
    @Role NVARCHAR(50) = NULL      -- Role name or NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @Offset INT = (@Page - 1) * @Limit;
        DECLARE @TotalCount INT;

        -- Validate admin user (must be active and have Admin/SuperAdmin role)
        IF NOT EXISTS (
            SELECT 1
            FROM Users u
            JOIN Roles r ON u.RoleId = r.RoleId
            WHERE u.UserId = @AdminUserId
              AND r.RoleName IN ('Admin', 'SuperAdmin')
        )
        BEGIN
            RAISERROR('User not found or insufficient privileges.', 16, 1);
            RETURN;
        END

        ---------------------------------------------------------------------
        -- Total count (do not hard-filter Users.Active = 1; let @Status decide)
        ---------------------------------------------------------------------
        SELECT @TotalCount = COUNT(*)
        FROM Users u
        LEFT JOIN Roles r ON u.RoleId = r.RoleId AND r.Active = 1
        WHERE (@TenantId IS NULL OR u.TenantId = @TenantId)

        SELECT
            u.UserId,
            u.FirstName,
            u.LastName,
            u.Email,
            u.Phone,
            ISNULL(r.RoleName, 'Customer') AS Role,
            r.RoleId,
            u.EmailVerified,
            u.PhoneVerified,
            CASE
                WHEN u.AccountLocked = 1 THEN 'locked'
                WHEN u.Active = 0 THEN 'inactive'
                ELSE 'active'
            END AS Status,
            u.CreatedAt,
            u.LastLogin,
            u.TenantId,
            u.ProfilePicture AS Avatar,
            u.Active,
            -- Pagination metadata
            @TotalCount AS TotalCount,
            @Page       AS CurrentPage,
            @Limit      AS PageSize,
            CEILING(CAST(@TotalCount AS FLOAT) / @Limit) AS TotalPages,
            CASE WHEN @Page < CEILING(CAST(@TotalCount AS FLOAT) / @Limit) THEN 1 ELSE 0 END AS HasNext,
            CASE WHEN @Page > 1 THEN 1 ELSE 0 END AS HasPrevious
        FROM Users u
        LEFT JOIN Roles r ON u.RoleId = r.RoleId AND r.Active = 1
        WHERE (@TenantId IS NULL OR u.TenantId = @TenantId)

        ORDER BY u.CreatedAt DESC
        OFFSET @Offset ROWS
        FETCH NEXT @Limit ROWS ONLY;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_ADMIN_UPDATE_USER_STATUS]
				@AdminUserId BIGINT,
				@UserId BIGINT,
				@TenantId BIGINT = NULL,
				@Status NVARCHAR(50),
				@Reason NVARCHAR(500) = NULL,
				@IpAddress NVARCHAR(45) = NULL,
				@UserAgent NVARCHAR(500) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
	BEGIN TRANSACTION;
	
	DECLARE @CurrentTime DATETIME = GETUTCDATE();
	DECLARE @TargetUserName NVARCHAR(200);
	DECLARE @OldStatus NVARCHAR(50);
	DECLARE @NewActive BIT;
	
	-- Validate admin user exists and has admin role
	IF NOT EXISTS (
		SELECT 1 FROM Users u
		INNER JOIN Roles r ON u.RoleId = r.RoleId
		WHERE u.UserId = @AdminUserId 
			AND r.RoleName IN ('Admin', 'SuperAdmin')
	)
	BEGIN
		RAISERROR('Admin user not found or insufficient privileges.', 16, 1);
		RETURN;
	END
	
	-- Validate target user exists
	SELECT 
		@TargetUserName = u.FirstName + ' ' + u.LastName,
		@OldStatus = CASE 
			WHEN u.AccountLocked = 1 THEN 'locked'
			WHEN u.Active = 0 THEN 'inactive'
			ELSE 'active'
		END
	FROM Users u
	WHERE u.UserId = @UserId
		AND (@TenantId IS NULL OR u.TenantId = @TenantId);
	
	IF @TargetUserName IS NULL
	BEGIN
		RAISERROR('Target user not found.', 16, 1);
		RETURN;
	END
	
	-- Validate status value
	IF @Status NOT IN ('active', 'inactive')
	BEGIN
		RAISERROR('Invalid status. Status must be either ''active'' or ''inactive''.', 16, 1);
		RETURN;
	END
	
	-- Determine new Active value based on status
	SET @NewActive = CASE WHEN @Status = 'active' THEN 1 ELSE 0 END;
	
	-- Update user status
	UPDATE Users
	SET 
		Active = @NewActive,
		AccountLocked = CASE WHEN @Status = 'active' THEN 0 ELSE AccountLocked END,
		UpdatedAt = @CurrentTime
	WHERE UserId = @UserId;
	
	-- If deactivating (status = 'inactive'), logout the user
	IF @Status = 'inactive'
	BEGIN
		-- Invalidate all tokens for this user
		-- UPDATE UserTokens 
		-- SET IsRevoked = 1, 
		-- 	RevokedAt = @CurrentTime,
		-- 	RevokedReason = 'Account deactivated by admin',
		-- 	UpdatedAt = @CurrentTime
		-- WHERE UserId = @UserId 
		-- 	AND IsRevoked = 0;
		
		-- -- Deactivate all active sessions
		-- UPDATE UserSessions 
		-- SET IsActive = 0,
		-- 	LoggedOutAt = @CurrentTime,
		-- 	UpdatedAt = @CurrentTime
		-- WHERE UserId = @UserId 
		-- 	AND IsActive = 1;
		
		-- Deactivate all UserRoles entries for this user
		-- IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserRoles')
		-- BEGIN
		-- 	UPDATE UserRoles 
		-- 	SET Active = 0,
		-- 		UpdatedAt = @CurrentTime
		-- 	WHERE UserId = @UserId 
		-- 		AND Active = 1;
		-- END
		
		-- Clear remember me sessions
		UPDATE Users 
		SET RememberMeToken = NULL,
			RememberMeExpiry = NULL,
			LastLogout = @CurrentTime
		WHERE UserId = @UserId;
	END
	
-- -- Log the status change activity
-- INSERT INTO UserActivityLog (
-- 	UserId,
-- 	ActivityType,
-- 	ActivityDescription,
-- 	IpAddress,
-- 	UserAgent,
-- 	CreatedAt,
-- 	PerformedBy
-- ) VALUES (
-- 	@UserId,
-- 	'STATUS_UPDATED',
-- 	'User status updated from ' + ISNULL(@OldStatus, 'unknown') + ' to ' + @Status + 
-- 		CASE WHEN @Reason IS NOT NULL THEN '. Reason: ' + @Reason ELSE '' END + ' by admin',
-- 	@IpAddress,
-- 	@UserAgent,
-- 	@CurrentTime,
-- 	@AdminUserId
-- );

-- -- Log admin activity
-- INSERT INTO UserActivityLog (
-- 	UserId,
-- 	ActivityType,
-- 	ActivityDescription,
-- 	IpAddress,
-- 	UserAgent,
-- 	CreatedAt
-- ) VALUES (
-- 	@AdminUserId,
-- 	'ADMIN_STATUS_UPDATE',
-- 	'Updated status for user ' + @TargetUserName + ' from ' + ISNULL(@OldStatus, 'unknown') + ' to ' + @Status +
-- 		CASE WHEN @Reason IS NOT NULL THEN '. Reason: ' + @Reason ELSE '' END,
-- 	@IpAddress,
-- 	@UserAgent,
-- 	@CurrentTime
-- );

COMMIT TRANSACTION;

-- Return success message
SELECT 'User updated successfully' AS Message;
	
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

CREATE OR ALTER PROCEDURE [dbo].[SP_ADMIN_GET_ALL_ORDERS]
	@AdminUserId BIGINT,
	@TenantId BIGINT = NULL,
	@Page INT = 1,
	@Limit INT = 10,
	@Status NVARCHAR(50) = NULL,
	@Search NVARCHAR(100) = NULL,
	@StartDate DATETIME = NULL,
	@EndDate DATETIME = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		DECLARE @Offset INT;
		DECLARE @TotalCount INT;
		
		-- Calculate offset for pagination
		SET @Offset = (@Page - 1) * @Limit;
		
		-- Validate admin user exists and has admin role
		IF NOT EXISTS (
			SELECT 1 FROM Users u
			INNER JOIN Roles r ON u.RoleId = r.RoleId
			WHERE u.UserId = @AdminUserId 
				AND u.Active = 1 
				AND r.RoleName IN ('Admin', 'SuperAdmin')
		)
		BEGIN
			RAISERROR('User not found or insufficient privileges.', 16, 1);
			RETURN;
		END
		
		-- Set default date range if not provided
		IF @StartDate IS NULL
			SET @StartDate = DATEADD(MONTH, -6, GETUTCDATE()); -- Last 6 months
		
		IF @EndDate IS NULL
			SET @EndDate = GETUTCDATE();

		SET @EndDate = @EndDate + 1
		
		-- Get total count for pagination
		SELECT @TotalCount = COUNT(*)
		FROM Orders o
		INNER JOIN Users u ON o.UserId = u.UserId
		WHERE o.Active = 1
			AND (@TenantId IS NULL OR o.TenantId = @TenantId)
			AND (@Status IS NULL OR o.OrderStatus = @Status)
			AND (@Search IS NULL OR 
				o.OrderNumber LIKE '%' + @Search + '%' OR 
				u.FirstName + ' ' + u.LastName LIKE '%' + @Search + '%' OR
				u.Email LIKE '%' + @Search + '%'
			)
			AND o.CreatedAt >= @StartDate
			AND o.CreatedAt <= @EndDate;
		
		-- Get orders with pagination
		SELECT 
			o.OrderId,
			o.OrderNumber,
			u.FirstName + ' ' + u.LastName AS CustomerName,
			u.Email AS CustomerEmail,
			u.Phone AS CustomerPhone,
			o.OrderStatus AS Status,
			o.PaymentStatus,
			o.TotalAmount AS Total,
			o.Subtotal,
			o.ShippingAmount,
			o.TaxAmount,
			o.DiscountAmount,
			o.CouponId,
			o.CouponCode,
			o.CouponDiscountAmount,
			-- Calculate item statistics
			ISNULL(orderItems.ItemCount, 0) AS ItemCount,
			ISNULL(orderItems.TotalQuantity, 0) AS TotalQuantity,
			o.CreatedAt,
			o.UpdatedAt,
			o.ShippedAt,
			o.DeliveredAt,
			o.ShippingAddress,
			o.PaymentMethod,
			o.ShippingMethod,
			o.Notes,
			-- Estimated delivery calculation
			CASE 
				WHEN o.OrderStatus = 'Delivered' THEN o.DeliveredAt
				WHEN o.OrderStatus = 'Shipped' THEN DATEADD(DAY, 3, ISNULL(o.ShippedAt, o.UpdatedAt))
				WHEN o.OrderStatus IN ('Processing', 'Confirmed') THEN DATEADD(DAY, 5, o.CreatedAt)
				ELSE DATEADD(DAY, 7, o.CreatedAt)
			END AS EstimatedDelivery,
			-- Customer information
			u.UserId AS CustomerId,
			u.TenantId AS CustomerTenantId,
			-- Pagination info
			@TotalCount AS TotalCount,
			@Page AS CurrentPage,
			@Limit AS PageSize,
			CEILING(CAST(@TotalCount AS FLOAT) / @Limit) AS TotalPages,
			CASE WHEN @Page < CEILING(CAST(@TotalCount AS FLOAT) / @Limit) THEN 1 ELSE 0 END AS HasNext,
			CASE WHEN @Page > 1 THEN 1 ELSE 0 END AS HasPrevious
		FROM Orders o
		INNER JOIN Users u ON o.UserId = u.UserId
		LEFT JOIN (
			SELECT 
				oi.OrderId,
				COUNT(oi.OrderItemId) AS ItemCount,
				SUM(oi.Quantity) AS TotalQuantity
			FROM OrderItems oi
			WHERE oi.Active = 1
			GROUP BY oi.OrderId
		) orderItems ON o.OrderId = orderItems.OrderId
		WHERE o.Active = 1
			AND (@TenantId IS NULL OR o.TenantId = @TenantId)
			AND (@Status IS NULL OR o.OrderStatus = @Status)
			AND (@Search IS NULL OR 
				o.OrderNumber LIKE '%' + @Search + '%' OR 
				u.FirstName + ' ' + u.LastName LIKE '%' + @Search + '%' OR
				u.Email LIKE '%' + @Search + '%'
			)
			AND o.CreatedAt >= @StartDate
			AND o.CreatedAt <= @EndDate
		ORDER BY o.CreatedAt DESC
		OFFSET @Offset ROWS
		FETCH NEXT @Limit ROWS ONLY;
		
		-- Get order items for each order in the result set
		SELECT 
			oi.OrderId,
			oi.OrderItemId,
			oi.ProductId,
			oi.ProductName,
			oi.ProductImage,
			oi.Price,
			oi.Quantity,
			oi.Total,
			p.ProductCode,
			p.Category,
			p.Rating,
			p.Offer
		FROM OrderItems oi
		LEFT JOIN Products p ON oi.ProductId = p.ProductId
		WHERE oi.OrderId IN (
			SELECT o.OrderId
			FROM Orders o
			INNER JOIN Users u ON o.UserId = u.UserId
			WHERE o.Active = 1
				AND (@TenantId IS NULL OR o.TenantId = @TenantId)
				AND (@Status IS NULL OR o.OrderStatus = @Status)
				AND (@Search IS NULL OR 
					o.OrderNumber LIKE '%' + @Search + '%' OR 
					u.FirstName + ' ' + u.LastName LIKE '%' + @Search + '%' OR
					u.Email LIKE '%' + @Search + '%'
				)
				AND o.CreatedAt >= @StartDate
				AND o.CreatedAt <= @EndDate
			ORDER BY o.CreatedAt DESC
			OFFSET @Offset ROWS
			FETCH NEXT @Limit ROWS ONLY
		)
		AND oi.Active = 1
		ORDER BY oi.OrderId, oi.OrderItemId;
		
		-- Get order statistics summary
		SELECT 
			COUNT(*) AS TotalOrders,
			SUM(o.TotalAmount) AS TotalRevenue,
			AVG(o.TotalAmount) AS AverageOrderValue,
			COUNT(DISTINCT o.UserId) AS UniqueCustomers,
			SUM(CASE WHEN o.OrderStatus = 'Pending' THEN 1 ELSE 0 END) AS PendingOrders,
			SUM(CASE WHEN o.OrderStatus = 'Processing' THEN 1 ELSE 0 END) AS ProcessingOrders,
			SUM(CASE WHEN o.OrderStatus = 'Shipped' THEN 1 ELSE 0 END) AS ShippedOrders,
			SUM(CASE WHEN o.OrderStatus = 'Delivered' THEN 1 ELSE 0 END) AS DeliveredOrders,
			SUM(CASE WHEN o.OrderStatus = 'Cancelled' THEN 1 ELSE 0 END) AS CancelledOrders
		FROM Orders o
		WHERE o.Active = 1
			AND (@TenantId IS NULL OR o.TenantId = @TenantId)
			AND o.CreatedAt >= @StartDate
			AND o.CreatedAt <= @EndDate;
		
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_CREATE_RAZORPAY_ORDER]
	@Amount BIGINT,
	@Currency NVARCHAR(3) = 'INR',
	@Receipt NVARCHAR(255) = NULL,
	@RazorpayOrderId NVARCHAR(255),
	@Status NVARCHAR(50) = 'created',
	@UserId BIGINT = NULL,
	@TenantId BIGINT = NULL,
	@OrderId BIGINT = NULL,
	@Notes NVARCHAR(500) = NULL,
	@RazorpayResponse NVARCHAR(MAX) = NULL,
	@Id BIGINT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		BEGIN TRANSACTION;
		
		DECLARE @CurrentTime DATETIME2(7) = GETUTCDATE();
		
		-- Validate required parameters
		IF @Amount IS NULL OR @Amount <= 0
		BEGIN
			RAISERROR('Amount must be greater than 0', 16, 1);
			RETURN;
		END
		
		IF @Currency IS NULL OR LEN(@Currency) <> 3
		BEGIN
			RAISERROR('Currency must be a valid 3-character code', 16, 1);
			RETURN;
		END
		
		IF @RazorpayOrderId IS NULL OR LEN(@RazorpayOrderId) = 0
		BEGIN
			RAISERROR('Razorpay Order ID is required', 16, 1);
			RETURN;
		END
		
		-- -- Validate UserId if provided
		-- IF @UserId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId AND Active = 1)
		-- BEGIN
		-- 	RAISERROR('Invalid User ID provided', 16, 1);
		-- 	RETURN;
		-- END
		
		-- -- Validate OrderId if provided
		-- IF @OrderId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Orders WHERE OrderId = @OrderId AND Active = 1)
		-- BEGIN
		-- 	RAISERROR('Invalid Order ID provided', 16, 1);
		-- 	RETURN;
		-- END
		
		-- -- Check if Razorpay Order ID already exists
		-- IF EXISTS (SELECT 1 FROM RazorpayOrders WHERE RazorpayOrderId = @RazorpayOrderId)
		-- BEGIN
		-- 	RAISERROR('Razorpay Order ID already exists', 16, 1);
		-- 	RETURN;
		-- END
		
		-- Generate receipt if not provided
		IF @Receipt IS NULL OR LEN(@Receipt) = 0
		BEGIN
			SET @Receipt = 'RCP' + FORMAT(@CurrentTime, 'yyyyMMddHHmmss') + FORMAT(ABS(CHECKSUM(NEWID())) % 10000, '0000');
		END
		
		-- Insert Razorpay order record
		INSERT INTO RazorpayOrders (
			RazorpayOrderId,
			Amount,
			Currency,
			Receipt,
			Status,
			UserId,
			TenantId,
			OrderId,
			Notes,
			RazorpayResponse,
			CreatedAt,
			UpdatedAt,
			Active
		)
		VALUES (
			@RazorpayOrderId,
			@Amount,
			@Currency,
			@Receipt,
			@Status,
			@UserId,
			@TenantId,
			@OrderId,
			@Notes,
			@RazorpayResponse,
			@CurrentTime,
			@CurrentTime,
			1
		);
		
		SET @Id = SCOPE_IDENTITY();
		
		COMMIT TRANSACTION;
		
		-- Return the created record
		SELECT 
			Id,
			RazorpayOrderId AS OrderId,
			Amount,
			Currency,
			Receipt,
			Status,
			CreatedAt
		FROM RazorpayOrders
		WHERE Id = @Id;
		
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

CREATE OR ALTER PROCEDURE [dbo].[SP_VERIFY_RAZORPAY_PAYMENT]
	@RazorpayOrderId NVARCHAR(255),
	@RazorpayPaymentId NVARCHAR(255),
	@Signature NVARCHAR(500),
	@SignatureVerified BIT,
	@Status NVARCHAR(50) = 'captured',
	@Amount BIGINT = NULL,
	@Currency NVARCHAR(3) = 'INR',
	@PaymentMethod NVARCHAR(100) = NULL,
	@UserId BIGINT = NULL,
	@TenantId BIGINT = NULL,
	@OrderId BIGINT = NULL,
	@RazorpayOrderRecordId BIGINT = NULL,
	@RazorpayResponse NVARCHAR(MAX) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		BEGIN TRANSACTION;
		
		DECLARE @CurrentTime DATETIME2(7) = GETUTCDATE();
		DECLARE @ExistingPaymentId BIGINT = NULL, 	@Id BIGINT;
		
		-- Validate required parameters
		IF @RazorpayOrderId IS NULL OR LEN(@RazorpayOrderId) = 0
		BEGIN
			RAISERROR('Razorpay Order ID is required', 16, 1);
			RETURN;
		END
		
		IF @RazorpayPaymentId IS NULL OR LEN(@RazorpayPaymentId) = 0
		BEGIN
			RAISERROR('Razorpay Payment ID is required', 16, 1);
			RETURN;
		END
		
		IF @Signature IS NULL OR LEN(@Signature) = 0
		BEGIN
			RAISERROR('Signature is required', 16, 1);
			RETURN;
		END
		
		-- Check if payment already exists
		SELECT @ExistingPaymentId = Id 
		FROM RazorpayPayments 
		WHERE RazorpayPaymentId = @RazorpayPaymentId;
		
		IF @ExistingPaymentId IS NOT NULL
		BEGIN
			-- Update existing payment record
			UPDATE RazorpayPayments
			SET 
				Signature = @Signature,
				SignatureVerified = @SignatureVerified,
				Status = @Status,
				VerificationAttempts = VerificationAttempts + 1,
				VerifiedAt = CASE WHEN @SignatureVerified = 1 AND VerifiedAt IS NULL THEN @CurrentTime ELSE VerifiedAt END,
				UpdatedAt = @CurrentTime,
				Amount = ISNULL(@Amount, Amount),
				Currency = ISNULL(@Currency, Currency),
				PaymentMethod = ISNULL(@PaymentMethod, PaymentMethod),
				RazorpayResponse = ISNULL(@RazorpayResponse, RazorpayResponse)
			WHERE Id = @ExistingPaymentId;
			
			SET @Id = @ExistingPaymentId;
		END
		ELSE
		BEGIN
			-- Validate UserId if provided
			IF @UserId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId AND Active = 1)
			BEGIN
				RAISERROR('Invalid User ID provided', 16, 1);
				RETURN;
			END
			
			-- Validate OrderId if provided
			IF @OrderId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Orders WHERE OrderId = @OrderId AND Active = 1)
			BEGIN
				RAISERROR('Invalid Order ID provided', 16, 1);
				RETURN;
			END
			
			-- Look up RazorpayOrderRecordId from RazorpayOrders table if not provided
			IF @RazorpayOrderRecordId IS NULL
			BEGIN
				SELECT @RazorpayOrderRecordId = Id
				FROM RazorpayOrders
				WHERE RazorpayOrderId = @RazorpayOrderId AND Active = 1;
			END
			
			-- Validate RazorpayOrderRecordId if provided
			IF @RazorpayOrderRecordId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RazorpayOrders WHERE Id = @RazorpayOrderRecordId AND Active = 1)
			BEGIN
				RAISERROR('Invalid Razorpay Order Record ID provided', 16, 1);
				RETURN;
			END
			
			-- Get amount from RazorpayOrders if not provided
			IF @Amount IS NULL AND @RazorpayOrderRecordId IS NOT NULL
			BEGIN
				SELECT @Amount = Amount, @Currency = Currency
				FROM RazorpayOrders
				WHERE Id = @RazorpayOrderRecordId;
			END
			
			-- Insert new payment record
			INSERT INTO RazorpayPayments (
				RazorpayPaymentId,
				RazorpayOrderId,
				Amount,
				Currency,
				[Status],
				[Signature],
				SignatureVerified,
				VerificationAttempts,
				UserId,
				TenantId,
				OrderId,
				RazorpayOrderRecordId,
				PaymentMethod,
				RazorpayResponse,
				CreatedAt,
				UpdatedAt,
				VerifiedAt,
				Active
			)
			VALUES (
				@RazorpayPaymentId,
				@RazorpayOrderId,
				ISNULL(@Amount, 0),
				@Currency,
				@Status,
				@Signature,
				@SignatureVerified,
				1,
				@UserId,
				@TenantId,
				@OrderId,
				@RazorpayOrderRecordId,
				@PaymentMethod,
				@RazorpayResponse,
				@CurrentTime,
				@CurrentTime,
				CASE WHEN @SignatureVerified = 1 THEN @CurrentTime ELSE NULL END,
				1
			);
			
			SET @Id = SCOPE_IDENTITY();
		END
		
		COMMIT TRANSACTION;
		
		-- Return the payment record
		SELECT 
			Id,
			RazorpayPaymentId AS PaymentId,
			RazorpayOrderId AS OrderId,
			Amount,
			Currency,
			Status,
			SignatureVerified,
			VerifiedAt,
			CreatedAt
		FROM RazorpayPayments
		WHERE Id = @Id;
		
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

CREATE OR ALTER PROCEDURE [dbo].[SP_UPDATE_ORDER_STATUS_WITH_PAYMENT]
	@OrderId BIGINT = NULL,
	@OrderNumber NVARCHAR(50) = NULL,
	@Status NVARCHAR(50) = NULL,
	@PaymentStatus NVARCHAR(50) = NULL,
	@RazorpayPaymentId NVARCHAR(255) = NULL,
	@RazorpayOrderId NVARCHAR(255) = NULL,
	@RazorpaySignature NVARCHAR(500) = NULL,
	@Notes NVARCHAR(1000) = NULL,
	@UpdatedBy BIGINT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		BEGIN TRANSACTION;
		
		DECLARE @CurrentTime DATETIME2(7) = GETUTCDATE();
		DECLARE @ResolvedOrderId BIGINT = NULL;
		DECLARE @CurrentOrderStatus NVARCHAR(50);
		DECLARE @CurrentPaymentStatus NVARCHAR(50);
		DECLARE @OrderUserId BIGINT;
		DECLARE @OrderTenantId BIGINT;
		DECLARE @ResolvedOrderNumber NVARCHAR(50);
		DECLARE @ResolvedRazorpayOrderId NVARCHAR(255) = NULL;
		DECLARE @OrderCouponId BIGINT = NULL;
		
		-- Resolve OrderId from OrderNumber if OrderId is not provided
		IF @OrderId IS NULL AND @OrderNumber IS NOT NULL
		BEGIN
			SELECT @ResolvedOrderId = OrderId
			FROM Orders
			WHERE OrderNumber = @OrderNumber AND Active = 1;
			
			IF @ResolvedOrderId IS NULL
			BEGIN
				RAISERROR('Order not found with the provided order number.', 16, 1);
				RETURN;
			END
		END
		ELSE IF @OrderId IS NOT NULL
		BEGIN
			SET @ResolvedOrderId = @OrderId;
		END
		-- Resolve OrderId from RazorpayOrderId if OrderId and OrderNumber are both NULL
		ELSE IF @OrderId IS NULL AND @OrderNumber IS NULL AND @RazorpayOrderId IS NOT NULL
		BEGIN
			-- Try to get OrderId from RazorpayOrders table
			SELECT TOP 1 @ResolvedOrderId = OrderId
			FROM RazorpayOrders
			WHERE RazorpayOrderId = @RazorpayOrderId AND Active = 1
			ORDER BY CreatedAt DESC;
			
			-- If not found in RazorpayOrders, try RazorpayPayments
			IF @ResolvedOrderId IS NULL
			BEGIN
				SELECT TOP 1 @ResolvedOrderId = OrderId
				FROM RazorpayPayments
				WHERE RazorpayOrderId = @RazorpayOrderId AND OrderId IS NOT NULL
				ORDER BY CreatedAt DESC;
			END
			
			-- If OrderId is still NULL, it means order doesn't exist yet (e.g., payment cancelled before order creation)
			-- In this case, we'll just update RazorpayOrders/RazorpayPayments tables and return success
			-- This is a valid scenario for payment cancellations
		END
		ELSE
		BEGIN
			RAISERROR('Either OrderId, OrderNumber, or RazorpayOrderId must be provided.', 16, 1);
			RETURN;
		END
		
		-- Get current order information (only if OrderId exists)
		IF @ResolvedOrderId IS NOT NULL
		BEGIN
			SELECT 
				@CurrentOrderStatus = OrderStatus,
				@CurrentPaymentStatus = PaymentStatus,
				@OrderUserId = UserId,
				@OrderTenantId = TenantId,
				@ResolvedOrderNumber = OrderNumber,
				@OrderCouponId = CouponId
			FROM Orders
			WHERE OrderId = @ResolvedOrderId AND Active = 1;
			
			IF @CurrentOrderStatus IS NULL
			BEGIN
				RAISERROR('Order not found or inactive.', 16, 1);
				RETURN;
			END
			
			-- SIMPLIFIED INVENTORY RESTORATION: Restore stock when payment fails/cancels
			-- Simple logic: If payment is cancelled/failed AND order exists AND wasn't already cancelled, restore stock
			IF @ResolvedOrderId IS NOT NULL
				AND (
					@PaymentStatus IN ('failed', 'cancelled', 'Failed', 'Cancelled')
					OR @Status IN ('cancelled', 'Cancelled')
				)
				AND UPPER(LTRIM(RTRIM(ISNULL(@CurrentPaymentStatus, '')))) NOT IN ('FAILED', 'CANCELLED')
				AND UPPER(LTRIM(RTRIM(ISNULL(@CurrentOrderStatus, '')))) NOT IN ('CANCELLED')
			BEGIN
				-- Restore product stock using simple JOIN-based UPDATE
				UPDATE p
				SET 
					p.Quantity = p.Quantity + oi.Quantity,
					p.UserBuyCount = CASE 
						WHEN p.UserBuyCount >= oi.Quantity THEN p.UserBuyCount - oi.Quantity
						ELSE 0
					END,
					p.Modified = @CurrentTime
				FROM Products p
				INNER JOIN OrderItems oi ON p.ProductId = oi.ProductId
				WHERE oi.OrderId = @ResolvedOrderId 
					AND oi.Active = 1
					AND p.Active = 1;
				
				-- Restore coupon if used
				IF @OrderCouponId IS NOT NULL
				BEGIN
					BEGIN TRY
						UPDATE Coupons
						SET UsageCount = CASE WHEN UsageCount > 0 THEN UsageCount - 1 ELSE 0 END,
							UpdatedAt = @CurrentTime
						WHERE CouponId = @OrderCouponId;
						
						IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'CouponUsage')
						BEGIN
							DELETE FROM CouponUsage
							WHERE OrderId = @ResolvedOrderId AND CouponId = @OrderCouponId;
						END
					END TRY
					BEGIN CATCH
						-- Ignore coupon restore errors
					END CATCH
				END
			END
			
			-- Update Orders table
			UPDATE Orders
			SET 
				OrderStatus = ISNULL(@Status, OrderStatus),
				PaymentStatus = ISNULL(@PaymentStatus, PaymentStatus),
				PaymentTransactionId = ISNULL(@RazorpayPaymentId, PaymentTransactionId),
				Notes = ISNULL(@Notes, Notes),
				UpdatedAt = @CurrentTime
			WHERE OrderId = @ResolvedOrderId;
		END
		
		-- Resolve RazorpayOrderId from RazorpayPayments if only RazorpayPaymentId is provided
		IF @RazorpayOrderId IS NULL AND @RazorpayPaymentId IS NOT NULL
		BEGIN
			SELECT @ResolvedRazorpayOrderId = RazorpayOrderId
			FROM RazorpayPayments
			WHERE RazorpayPaymentId = @RazorpayPaymentId;
		END
		ELSE IF @RazorpayOrderId IS NOT NULL
		BEGIN
			SET @ResolvedRazorpayOrderId = @RazorpayOrderId;
		END
		
		-- Update RazorpayOrders table if RazorpayOrderId is available
		IF @ResolvedRazorpayOrderId IS NOT NULL
		BEGIN
			UPDATE RazorpayOrders
			SET 
				OrderId = @ResolvedOrderId,
				Status = CASE 
					WHEN @PaymentStatus = 'paid' THEN 'paid'
					WHEN @PaymentStatus = 'failed' THEN 'failed'
					WHEN @PaymentStatus = 'cancelled' THEN 'cancelled'
					WHEN @Status = 'cancelled' THEN 'cancelled'
					ELSE Status
				END,
				UpdatedAt = @CurrentTime
			WHERE RazorpayOrderId = @ResolvedRazorpayOrderId AND Active = 1;
		END
		
		-- Update RazorpayPayments table if RazorpayPaymentId is provided
		IF @RazorpayPaymentId IS NOT NULL
		BEGIN
			UPDATE RazorpayPayments
			SET 
				OrderId = @ResolvedOrderId,
				Status = CASE 
					WHEN @PaymentStatus = 'paid' THEN 'captured'
					WHEN @PaymentStatus = 'failed' THEN 'failed'
					WHEN @PaymentStatus = 'cancelled' THEN 'cancelled'
					ELSE Status
				END,
				Signature = ISNULL(@RazorpaySignature, Signature),
				UpdatedAt = @CurrentTime
			WHERE RazorpayPaymentId = @RazorpayPaymentId;
		END
		
		-- Note: Inventory restoration is now handled BEFORE updating Orders table (see above)
		-- This ensures we check the original status before it's changed
		
		-- Add status history record if status changed (only if order exists)
		IF @ResolvedOrderId IS NOT NULL AND @Status IS NOT NULL AND @Status != @CurrentOrderStatus
		BEGIN
			INSERT INTO OrderStatusHistory (
				OrderId,
				PreviousStatus,
				NewStatus,
				StatusNote,
				ChangedBy,
				ChangedAt,
				CreatedAt
			) VALUES (
				@ResolvedOrderId,
				@CurrentOrderStatus,
				@Status,
				@Notes,
				@UpdatedBy,
				@CurrentTime,
				@CurrentTime
			);
		END
		
		COMMIT TRANSACTION;
		
		-- Return the updated order information (if order exists)
		IF @ResolvedOrderId IS NOT NULL
		BEGIN
			SELECT 
				OrderId,
				OrderNumber,
				OrderStatus AS Status,
				PaymentStatus,
				PaymentTransactionId AS RazorpayPaymentId,
				@ResolvedRazorpayOrderId AS RazorpayOrderId,
				UpdatedAt,
				@UpdatedBy AS UpdatedBy,
				'Order status updated successfully' AS Message
			FROM Orders
			WHERE OrderId = @ResolvedOrderId;
		END
		ELSE
		BEGIN
			-- Return success response even if order doesn't exist (payment cancelled before order creation)
			SELECT 
				NULL AS OrderId,
				NULL AS OrderNumber,
				@Status AS Status,
				@PaymentStatus AS PaymentStatus,
				@RazorpayPaymentId AS RazorpayPaymentId,
				@RazorpayOrderId AS RazorpayOrderId,
				@CurrentTime AS UpdatedAt,
				@UpdatedBy AS UpdatedBy,
				'Payment status updated successfully (order not created yet)' AS Message;
		END
		
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

CREATE PROCEDURE [dbo].[SP_GET_ORDER_BY_ID]
				@OrderId BIGINT,
				@UserId BIGINT,
				@TenantId BIGINT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		-- Validate user exists and is active
		IF NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId AND Active = 1)
		BEGIN
			RAISERROR('User not found or inactive.', 16, 1);
			RETURN;
		END
		
		-- Validate order exists and belongs to user
		IF NOT EXISTS (
			SELECT 1 FROM Orders 
			WHERE OrderId = @OrderId 
				AND UserId = @UserId 
				AND Active = 1
				AND (@TenantId IS NULL OR TenantId = @TenantId)
		)
		BEGIN
			RAISERROR('Order not found or does not belong to user.', 16, 1);
			RETURN;
		END
		
		-- Get order details
		SELECT 
			o.OrderId,
			o.OrderNumber,
			o.OrderStatus,
			o.PaymentStatus,
			o.TotalAmount,
			o.Subtotal,
			o.ShippingAmount,
			o.TaxAmount,
			o.DiscountAmount,
			o.CouponId,
			o.CouponCode,
			o.CouponDiscountAmount,
			o.Notes,
			o.ShippingAddress,
			o.PaymentMethod,
			o.ShippingMethod,
			o.AppliedDiscount,
			o.CreatedAt,
			o.UpdatedAt,
			-- User information
			u.FirstName + ' ' + u.LastName AS CustomerName,
			u.Email AS CustomerEmail,
			u.Phone AS CustomerPhone,
			-- Calculate totals
			COUNT(oi.OrderItemId) AS TotalItems,
			ISNULL(SUM(oi.Quantity), 0) AS TotalQuantity
		FROM Orders o
		LEFT JOIN Users u ON o.UserId = u.UserId
		LEFT JOIN OrderItems oi ON o.OrderId = oi.OrderId AND oi.Active = 1
		WHERE o.OrderId = @OrderId
			AND o.UserId = @UserId
			AND o.Active = 1
			AND (@TenantId IS NULL OR o.TenantId = @TenantId)
		GROUP BY o.OrderId, o.OrderNumber, o.OrderStatus, o.PaymentStatus, o.TotalAmount,
					o.Subtotal, o.ShippingAmount, o.TaxAmount, o.DiscountAmount, o.CouponId,
					o.CouponCode, o.CouponDiscountAmount, o.Notes, o.ShippingAddress, 
				    o.PaymentMethod, o.ShippingMethod, o.AppliedDiscount,
					o.CreatedAt, o.UpdatedAt, u.FirstName, u.LastName, u.Email, u.Phone;
		
		-- Get order items
		SELECT 
			oi.OrderItemId,
			oi.ProductId,
			oi.ProductName,
			oi.ProductImage,
			oi.Price,
			oi.Quantity,
			oi.Total,
			oi.CreatedAt,
			-- Product details
			p.ProductCode,
			p.ProductDescription,
			p.Category,
			p.Rating,
			p.Offer,
			p.InStock,
			p.BestSeller
		FROM OrderItems oi
		LEFT JOIN Products p ON oi.ProductId = p.ProductId
		WHERE oi.OrderId = @OrderId
			AND oi.Active = 1
		ORDER BY oi.OrderItemId;
		
		-- Get order status history (if available)
		SELECT 
			osh.StatusHistoryId,
			osh.PreviousStatus,
			osh.NewStatus,
			osh.StatusNote,
			osh.ChangedBy,
			osh.ChangedAt,
			u.FirstName + ' ' + u.LastName AS ChangedByName
		FROM OrderStatusHistory osh
		LEFT JOIN Users u ON osh.ChangedBy = u.UserId
		WHERE osh.OrderId = @OrderId
		ORDER BY osh.ChangedAt DESC;
		
		-- Get order tracking information (if available)
		SELECT 
			ot.TrackingId,
			ot.TrackingNumber,
			ot.Carrier,
			ot.TrackingStatus,
			ot.EstimatedDelivery,
			ot.ActualDelivery,
			ot.TrackingUrl,
			ot.CreatedAt,
			ot.UpdatedAt
		FROM OrderTracking ot
		WHERE ot.OrderId = @OrderId
			AND ot.Active = 1
		ORDER BY ot.CreatedAt DESC;
		
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_UPDATE_ORDER_STATUS]
	@OrderId BIGINT,
	@UserId BIGINT = NULL,
	@TenantId BIGINT = NULL,
	@NewStatus NVARCHAR(50),
	@StatusNote NVARCHAR(1000) = NULL,
	@TrackingNumber NVARCHAR(100) = NULL,
	@Carrier NVARCHAR(100) = NULL,
	@EstimatedDelivery DATETIME = NULL,
	@UpdatedBy BIGINT = NULL,
	@IpAddress NVARCHAR(45) = NULL,
	@UserAgent NVARCHAR(500) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		BEGIN TRANSACTION;
		
		DECLARE @CurrentStatus NVARCHAR(50);
		DECLARE @CurrentTime DATETIME = GETUTCDATE();
		DECLARE @OrderNumber NVARCHAR(50);
		DECLARE @PaymentStatus NVARCHAR(50);
		DECLARE @IsValidTransition BIT = 0;
		DECLARE @IsAdminUpdate BIT = 0;
		DECLARE @TrackingId BIGINT;
		
		-- Check if this is an admin update (when @UserId IS NULL or @UpdatedBy is provided)
		IF @UserId IS NULL OR @UpdatedBy IS NOT NULL
		BEGIN
			SET @IsAdminUpdate = 1;
		END
		
		-- Validate user exists and is active (for customer updates only)
		IF @UserId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId AND Active = 1)
		BEGIN
			RAISERROR('User not found or inactive.', 16, 1);
			RETURN;
		END
		
		-- Validate order exists
		SELECT 
			@CurrentStatus = OrderStatus,
			@OrderNumber = OrderNumber,
			@PaymentStatus = PaymentStatus
		FROM Orders 
		WHERE OrderId = @OrderId 
			AND (@UserId IS NULL OR UserId = @UserId)  -- Allow admin updates without user restriction
			AND Active = 1
			AND (@TenantId IS NULL OR TenantId = @TenantId);
		
		IF @CurrentStatus IS NULL
		BEGIN
			RAISERROR('Order not found or access denied.', 16, 1);
			RETURN;
		END
		
		-- Validate status transition
		-- For admin updates, allow any valid status transition (more flexible)
		-- For customer updates, enforce strict transition rules
		IF @IsAdminUpdate = 1
		BEGIN
			-- Admin can make any transition to a valid status (except going backwards in most cases)
			-- Allow: any forward progression, cancellation from any status, or same status update
			IF (@CurrentStatus = @NewStatus) -- Same status with note update
				OR (@NewStatus IN ('Cancelled', 'Refunded')) -- Can cancel/refund from any status
				OR (@CurrentStatus = 'Pending' AND @NewStatus IN ('Confirmed', 'Processing', 'Shipped', 'Delivered', 'Cancelled'))
				OR (@CurrentStatus = 'Confirmed' AND @NewStatus IN ('Processing', 'Shipped', 'Delivered', 'Cancelled'))
				OR (@CurrentStatus = 'Processing' AND @NewStatus IN ('Shipped', 'Delivered', 'Cancelled'))
				OR (@CurrentStatus = 'Shipped' AND @NewStatus IN ('Delivered', 'Returned', 'Cancelled'))
				OR (@CurrentStatus = 'Delivered' AND @NewStatus IN ('Returned', 'Refunded'))
			BEGIN
				SET @IsValidTransition = 1;
			END
		END
		ELSE
		BEGIN
			-- Customer updates: strict transition rules
			IF (@CurrentStatus = 'Pending' AND @NewStatus IN ('Confirmed', 'Cancelled'))
				OR (@CurrentStatus = 'Confirmed' AND @NewStatus IN ('Processing', 'Cancelled'))
				OR (@CurrentStatus = 'Processing' AND @NewStatus IN ('Shipped', 'Cancelled'))
				OR (@CurrentStatus = 'Shipped' AND @NewStatus IN ('Delivered', 'Returned'))
				OR (@CurrentStatus = 'Delivered' AND @NewStatus = 'Returned')
				OR (@CurrentStatus = @NewStatus) -- Allow same status with note updates
			BEGIN
				SET @IsValidTransition = 1;
			END
		END
		
		IF @IsValidTransition = 0
		BEGIN
			RAISERROR('Invalid status transition from %s to %s', 16, 1, @CurrentStatus, @NewStatus);
			RETURN;
		END
		
		-- Update order status
		UPDATE Orders
		SET 
			OrderStatus = @NewStatus,
			UpdatedAt = @CurrentTime,
			-- Update specific fields based on status
			ShippedAt = CASE WHEN @NewStatus = 'Shipped' THEN @CurrentTime ELSE ShippedAt END,
			DeliveredAt = CASE WHEN @NewStatus = 'Delivered' THEN @CurrentTime ELSE DeliveredAt END,
			PaymentStatus = CASE 
				WHEN @NewStatus = 'Delivered' AND @PaymentStatus = 'Pending' THEN 'Paid'
				ELSE PaymentStatus
			END
		WHERE OrderId = @OrderId;
		
		-- Add status history record
		IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'OrderStatusHistory')
		BEGIN
			INSERT INTO OrderStatusHistory (
				OrderId,
				PreviousStatus,
				NewStatus,
				StatusNote,
				ChangedBy,
				ChangedAt,
				CreatedAt
			) VALUES (
				@OrderId,
				@CurrentStatus,
				@NewStatus,
				@StatusNote,
				ISNULL(@UpdatedBy, @UserId),
				@CurrentTime,
				@CurrentTime
			);
		END
		
		-- Add tracking information if provided
		IF @TrackingNumber IS NOT NULL AND EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'OrderTracking')
		BEGIN
			INSERT INTO OrderTracking (
				OrderId,
				TrackingNumber,
				Carrier,
				EstimatedDelivery,
				TrackingStatus,
				Active,
				CreatedAt,
				UpdatedAt
			) VALUES (
				@OrderId,
				@TrackingNumber,
				@Carrier,
				@EstimatedDelivery,
				'In Transit',
				1,
				@CurrentTime,
				@CurrentTime
			);
		END
		
		COMMIT TRANSACTION;
		
		-- Return updated order status
		SELECT 
			@OrderId AS OrderId,
			@OrderNumber AS OrderNumber,
			@CurrentStatus AS PreviousStatus,
			@NewStatus AS NewStatus,
			@StatusNote AS StatusNote,
			ISNULL(@UpdatedBy, @UserId) AS UpdatedBy,
			@CurrentTime AS UpdatedDate
		FROM Orders
		WHERE OrderId = @OrderId;
		
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

CREATE PROCEDURE [dbo].[SP_CREATE_COUPON]
	@Code NVARCHAR(50),
	@Type NVARCHAR(20),
	@Value DECIMAL(18,2),
	@MinAmount DECIMAL(18,2) = NULL,
	@MaxDiscount DECIMAL(18,2) = NULL,
	@Description NVARCHAR(500) = NULL,
	@StartDate DATETIME2,
	@EndDate DATETIME2,
	@UsageLimit INT = NULL,
	@UsageLimitPerUser INT = NULL,
	@Active BIT = 1,
	@TenantId BIGINT,
	@CreatedBy BIGINT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		BEGIN TRANSACTION;
		
		-- Validate code uniqueness for tenant
		IF EXISTS (SELECT 1 FROM Coupons WHERE Code = @Code AND TenantId = @TenantId)
		BEGIN
			RAISERROR('Coupon code already exists for this tenant.', 16, 1);
			RETURN;
		END
		
		-- Validate dates
		IF @EndDate <= @StartDate
		BEGIN
			RAISERROR('End date must be after start date.', 16, 1);
			RETURN;
		END
		
		-- Validate type-specific constraints
		IF @Type = 'percentage' AND (@Value < 0 OR @Value > 100)
		BEGIN
			RAISERROR('Percentage value must be between 0 and 100.', 16, 1);
			RETURN;
		END
		
		IF @Type = 'percentage' AND @MaxDiscount IS NULL
		BEGIN
			RAISERROR('MaxDiscount is required for percentage coupons.', 16, 1);
			RETURN;
		END
		
		-- Insert coupon
		INSERT INTO Coupons (
			Code, Type, Value, MinAmount, MaxDiscount, Description,
			StartDate, EndDate, UsageLimit, UsageLimitPerUser, Active,
			TenantId, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
		)
		VALUES (
			@Code, @Type, @Value, @MinAmount, @MaxDiscount, @Description,
			@StartDate, @EndDate, @UsageLimit, @UsageLimitPerUser, @Active,
			@TenantId, @CreatedBy, @CreatedBy, GETUTCDATE(), GETUTCDATE()
		);
		
		DECLARE @CouponId BIGINT = SCOPE_IDENTITY();
		
		COMMIT TRANSACTION;
		
		-- Return created coupon
		SELECT 
			CouponId, Code, Type, Value, MinAmount, MaxDiscount, Description,
			StartDate, EndDate, UsageLimit, UsageLimitPerUser, UsageCount,
			Active, TenantId, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
		FROM Coupons
		WHERE CouponId = @CouponId;
		
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

CREATE PROCEDURE [dbo].[SP_GET_COUPONS]
	@TenantId BIGINT
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		-- Get all coupons for tenant
		SELECT 
			CouponId, Code, Type, Value, MinAmount, MaxDiscount, Description,
			StartDate, EndDate, UsageLimit, UsageLimitPerUser, UsageCount,
			Active, TenantId, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt,
			CASE 
				WHEN GETUTCDATE() < StartDate THEN 'upcoming'
				WHEN GETUTCDATE() > EndDate THEN 'expired'
				ELSE 'active'
			END AS Status
		FROM Coupons
		WHERE TenantId = @TenantId
		ORDER BY CreatedAt DESC;
		
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

CREATE PROCEDURE [dbo].[SP_GET_COUPON_BY_ID]
	@CouponId BIGINT,
	@TenantId BIGINT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		SELECT 
			CouponId, Code, Type, Value, MinAmount, MaxDiscount, Description,
			StartDate, EndDate, UsageLimit, UsageLimitPerUser, UsageCount,
			Active, TenantId, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt,
			CASE 
				WHEN GETUTCDATE() < StartDate THEN 'upcoming'
				WHEN GETUTCDATE() > EndDate THEN 'expired'
				ELSE 'active'
			END AS Status
		FROM Coupons
		WHERE CouponId = @CouponId
			AND (@TenantId IS NULL OR TenantId = @TenantId);
		
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

CREATE PROCEDURE [dbo].[SP_UPDATE_COUPON]
	@CouponId BIGINT,
	@Code NVARCHAR(50) = NULL,
	@Type NVARCHAR(20) = NULL,
	@Value DECIMAL(18,2) = NULL,
	@MinAmount DECIMAL(18,2) = NULL,
	@MaxDiscount DECIMAL(18,2) = NULL,
	@Description NVARCHAR(500) = NULL,
	@StartDate DATETIME2 = NULL,
	@EndDate DATETIME2 = NULL,
	@UsageLimit INT = NULL,
	@UsageLimitPerUser INT = NULL,
	@Active BIT = NULL,
	@TenantId BIGINT,
	@UpdatedBy BIGINT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		BEGIN TRANSACTION;
		
		-- Validate coupon exists and belongs to tenant
		IF NOT EXISTS (SELECT 1 FROM Coupons WHERE CouponId = @CouponId AND TenantId = @TenantId)
		BEGIN
			RAISERROR('Coupon not found or does not belong to this tenant.', 16, 1);
			RETURN;
		END
		
		-- Validate code uniqueness if changing code
		IF @Code IS NOT NULL AND EXISTS (
			SELECT 1 FROM Coupons 
			WHERE Code = @Code AND TenantId = @TenantId AND CouponId != @CouponId
		)
		BEGIN
			RAISERROR('Coupon code already exists for this tenant.', 16, 1);
			RETURN;
		END
		
		-- Validate dates if provided
		IF (@StartDate IS NOT NULL AND @EndDate IS NOT NULL) AND @EndDate <= @StartDate
		BEGIN
			RAISERROR('End date must be after start date.', 16, 1);
			RETURN;
		END
		
		-- Update coupon
		UPDATE Coupons
		SET 
			Code = ISNULL(@Code, Code),
			Type = ISNULL(@Type, Type),
			Value = ISNULL(@Value, Value),
			MinAmount = @MinAmount,
			MaxDiscount = @MaxDiscount,
			Description = ISNULL(@Description, Description),
			StartDate = ISNULL(@StartDate, StartDate),
			EndDate = ISNULL(@EndDate, EndDate),
			UsageLimit = @UsageLimit,
			UsageLimitPerUser = @UsageLimitPerUser,
			Active = ISNULL(@Active, Active),
			UpdatedBy = @UpdatedBy,
			UpdatedAt = GETUTCDATE()
		WHERE CouponId = @CouponId AND TenantId = @TenantId;
		
		COMMIT TRANSACTION;
		
		-- Return updated coupon
		SELECT 
			CouponId, Code, Type, Value, MinAmount, MaxDiscount, Description,
			StartDate, EndDate, UsageLimit, UsageLimitPerUser, UsageCount,
			Active, TenantId, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
		FROM Coupons
		WHERE CouponId = @CouponId;
		
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

CREATE PROCEDURE [dbo].[SP_DELETE_COUPON]
	@CouponId BIGINT,
	@TenantId BIGINT,
	@DeletedBy BIGINT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		BEGIN TRANSACTION;
		
		-- Validate coupon exists and belongs to tenant
		IF NOT EXISTS (SELECT 1 FROM Coupons WHERE CouponId = @CouponId AND TenantId = @TenantId)
		BEGIN
			RAISERROR('Coupon not found or does not belong to this tenant.', 16, 1);
			RETURN;
		END
		
		-- Soft delete (mark as inactive)
		UPDATE Coupons
		SET 
			Active = 0,
			UpdatedBy = @DeletedBy,
			UpdatedAt = GETUTCDATE()
		WHERE CouponId = @CouponId AND TenantId = @TenantId;
		
		COMMIT TRANSACTION;
		
		SELECT @CouponId AS CouponId, 'Coupon deleted successfully' AS Message;
		
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

CREATE OR ALTER PROCEDURE [dbo].[SP_VALIDATE_COUPON]
	@Code NVARCHAR(50),
	@Amount DECIMAL(18,2),
	@UserId BIGINT = NULL,
	@TenantId BIGINT
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		DECLARE @CouponId BIGINT;
		DECLARE @Type NVARCHAR(20);
		DECLARE @Value DECIMAL(18,2);
		DECLARE @MinAmount DECIMAL(18,2);
		DECLARE @MaxDiscount DECIMAL(18,2);
		DECLARE @UsageLimit INT;
		DECLARE @UsageLimitPerUser INT;
		DECLARE @UsageCount INT;
		DECLARE @StartDate DATETIME2;
		DECLARE @EndDate DATETIME2;
		DECLARE @Active BIT;
		DECLARE @UserUsageCount INT = 0;
		DECLARE @DiscountAmount DECIMAL(18,2) = 0;
		DECLARE @IsValid BIT = 0;
		DECLARE @Message NVARCHAR(500) = '';
		
		-- Get coupon details
		SELECT 
			@CouponId = CouponId,
			@Type = Type,
			@Value = Value,
			@MinAmount = MinAmount,
			@MaxDiscount = MaxDiscount,
			@UsageLimit = UsageLimit,
			@UsageLimitPerUser = UsageLimitPerUser,
			@UsageCount = UsageCount,
			@StartDate = StartDate,
			@EndDate = EndDate,
			@Active = Active
		FROM Coupons
		WHERE Code = @Code AND TenantId = @TenantId;
		
		-- Check if coupon exists
		IF @CouponId IS NULL
		BEGIN
			SET @Message = 'Invalid coupon code.';
			SELECT 
				@IsValid AS Valid,
				@Message AS Message,
				NULL AS CouponId,
				NULL AS Code,
				NULL AS Type,
				NULL AS Value,
				NULL AS DiscountAmount;
			RETURN;
		END
		
		-- Check if active
		IF @Active = 0
		BEGIN
			SET @Message = 'This coupon is inactive.';
			SELECT 
				@IsValid AS Valid,
				@Message AS Message,
				@CouponId AS CouponId,
				@Code AS Code,
				@Type AS Type,
				@Value AS Value,
				NULL AS DiscountAmount;
			RETURN;
		END
		
		-- Check date validity
		DECLARE @CurrentDate DATETIME2 = GETUTCDATE();
		IF @CurrentDate < @StartDate
		BEGIN
			SET @Message = 'This coupon is not yet active.';
			SELECT 
				@IsValid AS Valid,
				@Message AS Message,
				@CouponId AS CouponId,
				@Code AS Code,
				@Type AS Type,
				@Value AS Value,
				NULL AS DiscountAmount;
			RETURN;
		END
		
		IF @CurrentDate > @EndDate
		BEGIN
			SET @Message = 'This coupon has expired.';
			SELECT 
				@IsValid AS Valid,
				@Message AS Message,
				@CouponId AS CouponId,
				@Code AS Code,
				@Type AS Type,
				@Value AS Value,
				NULL AS DiscountAmount;
			RETURN;
		END
		
		-- Check minimum amount
		IF @MinAmount IS NOT NULL AND @Amount < @MinAmount
		BEGIN
			SET @Message = 'Minimum order amount of ' + CAST(@MinAmount AS NVARCHAR(20)) + ' required.';
			SELECT 
				@IsValid AS Valid,
				@Message AS Message,
				@CouponId AS CouponId,
				@Code AS Code,
				@Type AS Type,
				@Value AS Value,
				NULL AS DiscountAmount;
			RETURN;
		END
		
		-- Check usage limit
		IF @UsageLimit IS NOT NULL AND @UsageCount >= @UsageLimit
		BEGIN
			SET @Message = 'This coupon has reached its usage limit.';
			SELECT 
				@IsValid AS Valid,
				@Message AS Message,
				@CouponId AS CouponId,
				@Code AS Code,
				@Type AS Type,
				@Value AS Value,
				NULL AS DiscountAmount;
			RETURN;
		END
		
		-- Check per-user usage limit (only if CouponUsage table exists and UserId is provided)
		IF @UserId IS NOT NULL
		BEGIN
			-- Check if CouponUsage table exists
			IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'CouponUsage')
			BEGIN
				-- Get user's usage count for this coupon
				SELECT @UserUsageCount = COUNT(*)
				FROM [dbo].[CouponUsage]
				WHERE CouponId = @CouponId AND UserId = @UserId;
				
				-- Check if user has already used this coupon
				IF @UsageLimitPerUser IS NOT NULL
				BEGIN
					-- Check if user has reached the per-user limit
					IF @UserUsageCount >= @UsageLimitPerUser
					BEGIN
						IF @UsageLimitPerUser = 1
						BEGIN
							SET @Message = 'You have already used this coupon code.';
						END
						ELSE
						BEGIN
							SET @Message = 'You have already used this coupon the maximum number of times (' + CAST(@UsageLimitPerUser AS NVARCHAR(10)) + ' time(s)).';
						END
						SELECT 
							@IsValid AS Valid,
							@Message AS Message,
							@CouponId AS CouponId,
							@Code AS Code,
							@Type AS Type,
							@Value AS Value,
							NULL AS DiscountAmount;
						RETURN;
					END
				END
				ELSE IF @UserUsageCount > 0
				BEGIN
					-- If no per-user limit is set but user has already used it once, prevent reuse
					-- This ensures one-time use coupons work even without explicit limit
					SET @Message = 'You have already used this coupon code.';
					SELECT 
						@IsValid AS Valid,
						@Message AS Message,
						@CouponId AS CouponId,
						@Code AS Code,
						@Type AS Type,
						@Value AS Value,
						NULL AS DiscountAmount;
					RETURN;
				END
			END
		END
		
		-- Calculate discount amount
		IF @Type = 'percentage'
		BEGIN
			SET @DiscountAmount = (@Amount * @Value / 100);
			IF @MaxDiscount IS NOT NULL AND @DiscountAmount > @MaxDiscount
				SET @DiscountAmount = @MaxDiscount;
		END
		ELSE IF @Type = 'fixed'
		BEGIN
			SET @DiscountAmount = @Value;
			IF @DiscountAmount > @Amount
				SET @DiscountAmount = @Amount;
		END
		
		-- Coupon is valid
		SET @IsValid = 1;
		SET @Message = 'Coupon applied successfully.';
		
		SELECT 
			@IsValid AS Valid,
			@Message AS Message,
			@CouponId AS CouponId,
			@Code AS Code,
			@Type AS Type,
			@Value AS Value,
			@DiscountAmount AS DiscountAmount;
		
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		-- Handle CouponUsage table not found error gracefully
		IF @ErrorMessage LIKE '%Invalid object name%CouponUsage%'
		BEGIN
			-- If CouponUsage table doesn't exist, skip per-user limit check and continue
			-- This allows the coupon validation to work even if usage tracking table is missing
			-- Recalculate discount if we got this far
			IF @Type = 'percentage'
			BEGIN
				SET @DiscountAmount = (@Amount * @Value / 100);
				IF @MaxDiscount IS NOT NULL AND @DiscountAmount > @MaxDiscount
					SET @DiscountAmount = @MaxDiscount;
			END
			ELSE IF @Type = 'fixed'
			BEGIN
				SET @DiscountAmount = @Value;
				IF @DiscountAmount > @Amount
					SET @DiscountAmount = @Amount;
			END
			
			SET @IsValid = 1;
			SET @Message = 'Coupon applied successfully.';
			
			SELECT 
				@IsValid AS Valid,
				@Message AS Message,
				@CouponId AS CouponId,
				@Code AS Code,
				@Type AS Type,
				@Value AS Value,
				@DiscountAmount AS DiscountAmount;
			RETURN;
		END
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

CREATE PROCEDURE [dbo].[SP_GET_COUPON_USAGE]
	@CouponId BIGINT,
	@TenantId BIGINT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		-- Check if CouponUsage table exists
		IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'CouponUsage')
		BEGIN
			-- Return empty result sets if table doesn't exist
			SELECT 
				CAST(NULL AS BIGINT) AS UsageId,
				CAST(NULL AS BIGINT) AS OrderId,
				CAST(NULL AS BIGINT) AS UserId,
				CAST(NULL AS DECIMAL(18,2)) AS DiscountAmount,
				CAST(NULL AS DECIMAL(18,2)) AS OrderAmount,
				CAST(NULL AS DATETIME2) AS UsedAt,
				CAST(NULL AS NVARCHAR(50)) AS OrderNumber,
				CAST(NULL AS NVARCHAR(255)) AS UserName,
				CAST(NULL AS NVARCHAR(255)) AS UserEmail
			WHERE 1 = 0; -- Return no rows
			
			SELECT 
				CAST(0 AS INT) AS TotalUsage,
				CAST(0 AS DECIMAL(18,2)) AS TotalDiscountGiven,
				CAST(0 AS DECIMAL(18,2)) AS TotalOrderAmount;
			RETURN;
		END
		
		-- Get coupon usage records
		SELECT 
			cu.UsageId,
			cu.OrderId,
			cu.UserId,
			cu.DiscountAmount,
			cu.OrderAmount,
			cu.UsedAt,
			o.OrderNumber,
			ISNULL(u.FirstName + ' ' + u.LastName, 'Unknown User') AS UserName,
			ISNULL(u.Email, '') AS UserEmail
		FROM [dbo].[CouponUsage] cu
		INNER JOIN [dbo].[Coupons] c ON cu.CouponId = c.CouponId
		LEFT JOIN [dbo].[Orders] o ON cu.OrderId = o.OrderId
		LEFT JOIN [dbo].[Users] u ON cu.UserId = u.UserId
		WHERE cu.CouponId = @CouponId
			AND (@TenantId IS NULL OR c.TenantId = @TenantId)
		ORDER BY cu.UsedAt DESC;
		
		-- Return summary statistics
		SELECT 
			COUNT(*) AS TotalUsage,
			ISNULL(SUM(cu.DiscountAmount), 0) AS TotalDiscountGiven,
			ISNULL(SUM(cu.OrderAmount), 0) AS TotalOrderAmount
		FROM [dbo].[CouponUsage] cu
		INNER JOIN [dbo].[Coupons] c ON cu.CouponId = c.CouponId
		WHERE cu.CouponId = @CouponId
			AND (@TenantId IS NULL OR c.TenantId = @TenantId);
		
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		-- If table doesn't exist, return empty results instead of error
		IF @ErrorMessage LIKE '%Invalid object name%CouponUsage%'
		BEGIN
			SELECT 
				CAST(NULL AS BIGINT) AS UsageId,
				CAST(NULL AS BIGINT) AS OrderId,
				CAST(NULL AS BIGINT) AS UserId,
				CAST(NULL AS DECIMAL(18,2)) AS DiscountAmount,
				CAST(NULL AS DECIMAL(18,2)) AS OrderAmount,
				CAST(NULL AS DATETIME2) AS UsedAt,
				CAST(NULL AS NVARCHAR(50)) AS OrderNumber,
				CAST(NULL AS NVARCHAR(255)) AS UserName,
				CAST(NULL AS NVARCHAR(255)) AS UserEmail
			WHERE 1 = 0;
			
			SELECT 
				CAST(0 AS INT) AS TotalUsage,
				CAST(0 AS DECIMAL(18,2)) AS TotalDiscountGiven,
				CAST(0 AS DECIMAL(18,2)) AS TotalOrderAmount;
			RETURN;
		END
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

CREATE PROCEDURE [dbo].[SP_GET_ORDER_ID_BY_RAZORPAY_ORDER_ID]
	@RazorpayOrderId NVARCHAR(255)
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		SELECT TOP 1 OrderId
		FROM RazorpayOrders
		WHERE RazorpayOrderId = @RazorpayOrderId
			AND Active = 1
		ORDER BY CreatedAt DESC;
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

CREATE PROCEDURE [dbo].[SP_GET_ORDER_ID_BY_ORDER_NUMBER]
	@OrderNumber NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		SELECT OrderId
		FROM Orders
		WHERE OrderNumber = @OrderNumber
			AND Active = 1;
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

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

-- =============================================
-- SP ADDRESS
-- =============================================
CREATE PROCEDURE [dbo].[SP_GET_USER_ADDRESSES]
    @UserId BIGINT,
    @ActiveOnly BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        AddressId,
        UserId,
        AddressType,
        Street,
        City,
        State,
        PostalCode,
        Country,
        IsDefault,
        Active,
        CreatedAt,
        UpdatedAt
    FROM UserAddresses
    WHERE UserId = @UserId
        AND (@ActiveOnly = 0 OR Active = 1)
    ORDER BY IsDefault DESC, CreatedAt DESC;
END
GO

CREATE PROCEDURE [dbo].[SP_GET_ADDRESS_BY_ID]
    @AddressId BIGINT,
    @UserId BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        AddressId,
        UserId,
        AddressType,
        Street,
        City,
        State,
        PostalCode,
        Country,
        IsDefault,
        Active,
        CreatedAt,
        UpdatedAt
    FROM UserAddresses
    WHERE AddressId = @AddressId
        AND (@UserId IS NULL OR UserId = @UserId)
        AND Active = 1;
END
GO

CREATE PROCEDURE [dbo].[SP_CREATE_USER_ADDRESS]
    @UserId BIGINT,
    @AddressType NVARCHAR(50) = 'Home',
    @Street NVARCHAR(255),
    @City NVARCHAR(100),
    @State NVARCHAR(100),
    @PostalCode NVARCHAR(20),
    @Country NVARCHAR(100) = 'IN',
    @IsDefault BIT = 0,
    @AddressId BIGINT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @CurrentTime DATETIME2(7) = GETUTCDATE();
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validate user exists
        IF NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId AND Active = 1)
        BEGIN
            RAISERROR('User not found or inactive.', 16, 1);
            RETURN;
        END
        
        -- If setting as default, unset other default addresses
        IF @IsDefault = 1
        BEGIN
            UPDATE UserAddresses
            SET IsDefault = 0
            WHERE UserId = @UserId AND Active = 1;
        END
        
        -- If this is the first address, set as default
        IF NOT EXISTS (SELECT 1 FROM UserAddresses WHERE UserId = @UserId AND Active = 1)
        BEGIN
            SET @IsDefault = 1;
        END
        
        -- Insert new address
        INSERT INTO UserAddresses (
            UserId,
            AddressType,
            Street,
            City,
            State,
            PostalCode,
            Country,
            IsDefault,
            Active,
            CreatedAt,
            UpdatedAt
        ) VALUES (
            @UserId,
            @AddressType,
            @Street,
            @City,
            @State,
            @PostalCode,
            @Country,
            @IsDefault,
            1,
            @CurrentTime,
            @CurrentTime
        );
        
        SET @AddressId = SCOPE_IDENTITY();
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

CREATE PROCEDURE [dbo].[SP_UPDATE_USER_ADDRESS]
    @AddressId BIGINT,
    @UserId BIGINT,
    @AddressType NVARCHAR(50) = NULL,
    @Street NVARCHAR(255) = NULL,
    @City NVARCHAR(100) = NULL,
    @State NVARCHAR(100) = NULL,
    @PostalCode NVARCHAR(20) = NULL,
    @Country NVARCHAR(100) = NULL,
    @IsDefault BIT = NULL,
    @Active BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @CurrentTime DATETIME2(7) = GETUTCDATE();
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validate address exists and belongs to user
        IF NOT EXISTS (SELECT 1 FROM UserAddresses WHERE AddressId = @AddressId AND UserId = @UserId)
        BEGIN
            RAISERROR('Address not found or does not belong to this user.', 16, 1);
            RETURN;
        END
        
        -- If setting as default, unset other default addresses
        IF @IsDefault = 1
        BEGIN
            UPDATE UserAddresses
            SET IsDefault = 0
            WHERE UserId = @UserId AND AddressId != @AddressId AND Active = 1;
        END
        
        -- Update address
        UPDATE UserAddresses
        SET 
            AddressType = ISNULL(@AddressType, AddressType),
            Street = ISNULL(@Street, Street),
            City = ISNULL(@City, City),
            State = ISNULL(@State, State),
            PostalCode = ISNULL(@PostalCode, PostalCode),
            Country = ISNULL(@Country, Country),
            IsDefault = CASE WHEN @IsDefault IS NOT NULL THEN @IsDefault ELSE IsDefault END,
            Active = CASE WHEN @Active IS NOT NULL THEN @Active ELSE Active END,
            UpdatedAt = @CurrentTime
        WHERE AddressId = @AddressId AND UserId = @UserId;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

CREATE PROCEDURE [dbo].[SP_DELETE_USER_ADDRESS]
    @AddressId BIGINT,
    @UserId BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validate address exists and belongs to user
        IF NOT EXISTS (SELECT 1 FROM UserAddresses WHERE AddressId = @AddressId AND UserId = @UserId)
        BEGIN
            RAISERROR('Address not found or does not belong to this user.', 16, 1);
            RETURN;
        END
        
        -- Soft delete (set Active = 0)
        UPDATE UserAddresses
        SET Active = 0,
            UpdatedAt = GETUTCDATE()
        WHERE AddressId = @AddressId AND UserId = @UserId;
        
        -- If this was the default address, set another address as default
        IF EXISTS (SELECT 1 FROM UserAddresses WHERE AddressId = @AddressId AND IsDefault = 1)
        BEGIN
            DECLARE @NewDefaultId BIGINT;
            SELECT TOP 1 @NewDefaultId = AddressId
            FROM UserAddresses
            WHERE UserId = @UserId AND AddressId != @AddressId AND Active = 1
            ORDER BY CreatedAt DESC;
            
            IF @NewDefaultId IS NOT NULL
            BEGIN
                UPDATE UserAddresses
                SET IsDefault = 1
                WHERE AddressId = @NewDefaultId;
            END
        END
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_SET_MAIN_PRODUCT_IMAGE]
    @ProductId BIGINT,
    @ImageId BIGINT,
    @UserId BIGINT = NULL,
    @TenantId BIGINT = NULL,
    @IpAddress NVARCHAR(45) = NULL,
    @UserAgent NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @CurrentTime DATETIME = GETUTCDATE();
        DECLARE @ImageName NVARCHAR(255);
        
        -- Validate product exists and is active
        IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductId = @ProductId AND Active = 1)
        BEGIN
            RAISERROR('Product not found or inactive.', 16, 1);
            RETURN;
        END
        
        -- Validate image exists and belongs to product
        SELECT @ImageName = ImageName
        FROM ProductImages 
        WHERE ImageId = @ImageId AND ProductId = @ProductId AND Active = 1;
        
        IF @ImageName IS NULL
        BEGIN
            RAISERROR('Image not found, inactive, or does not belong to this product.', 16, 1);
            RETURN;
        END
        
        -- Validate user if provided
        IF @UserId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId AND Active = 1)
        BEGIN
            RAISERROR('User not found or inactive.', 16, 1);
            RETURN;
        END
        
        -- Unset all other main images for this product
        UPDATE ProductImages
        SET 
            Main = 0,
            Modified = @CurrentTime
        WHERE ProductId = @ProductId AND ImageId != @ImageId AND Active = 1;
        
        -- Set this image as main
        UPDATE ProductImages
        SET 
            Main = 1,
            Modified = @CurrentTime
        WHERE ImageId = @ImageId AND ProductId = @ProductId;
        
        -- Update product modified date
        UPDATE Products
        SET Modified = @CurrentTime
        WHERE ProductId = @ProductId;
        
        -- Log activity if user provided
        IF @UserId IS NOT NULL
        BEGIN
            INSERT INTO UserActivityLog (
                UserId,
                ActivityType,
                ActivityDescription,
                IpAddress,
                UserAgent,
                CreatedAt
            ) VALUES (
                @UserId,
                'PRODUCT_IMAGE_SET_MAIN',
                'Set image ' + CAST(@ImageId AS VARCHAR(20)) + ' as main for product ' + CAST(@ProductId AS VARCHAR(20)),
                @IpAddress,
                @UserAgent,
                @CurrentTime
            );
        END
        
        -- Return updated image information
        SELECT 
            pi.ImageId,
            pi.ProductId,
            pi.Poster,
            pi.Main,
            pi.Active,
            pi.OrderBy,
            pi.CreatedAt AS Created,
            pi.Modified,
            pi.ImageName,
            pi.ContentType,
            pi.FileSize,
            'Image set as main successfully' AS Message
        FROM ProductImages pi
        WHERE pi.ImageId = @ImageId AND pi.ProductId = @ProductId;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

CREATE PROCEDURE [dbo].[SP_SET_DEFAULT_ADDRESS]
    @AddressId BIGINT,
    @UserId BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validate address exists and belongs to user
        IF NOT EXISTS (SELECT 1 FROM UserAddresses WHERE AddressId = @AddressId AND UserId = @UserId AND Active = 1)
        BEGIN
            RAISERROR('Address not found or does not belong to this user.', 16, 1);
            RETURN;
        END
        
        -- Unset all other default addresses
        UPDATE UserAddresses
        SET IsDefault = 0
        WHERE UserId = @UserId AND AddressId != @AddressId AND Active = 1;
        
        -- Set this address as default
        UPDATE UserAddresses
        SET IsDefault = 1,
            UpdatedAt = GETUTCDATE()
        WHERE AddressId = @AddressId AND UserId = @UserId;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

CREATE PROCEDURE [dbo].[SP_ADMIN_GET_ALL_ADDRESSES]
    @TenantId BIGINT = NULL,
    @UserId BIGINT = NULL,
    @ActiveOnly BIT = 1,
    @PageNumber INT = 1,
    @PageSize INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    -- Get total count
    SELECT COUNT(*) AS TotalCount
    FROM UserAddresses ua
    INNER JOIN Users u ON ua.UserId = u.UserId
    WHERE (@TenantId IS NULL OR u.TenantId = @TenantId)
        AND (@UserId IS NULL OR ua.UserId = @UserId)
        AND (@ActiveOnly = 0 OR ua.Active = 1);
    
    -- Get addresses with user info
    SELECT 
        ua.AddressId,
        ua.UserId,
        u.FirstName + ' ' + u.LastName AS UserName,
        u.Email AS UserEmail,
        ua.AddressType,
        ua.Street,
        ua.City,
        ua.State,
        ua.PostalCode,
        ua.Country,
        ua.IsDefault,
        ua.Active,
        ua.CreatedAt,
        ua.UpdatedAt
    FROM UserAddresses ua
    INNER JOIN Users u ON ua.UserId = u.UserId
    WHERE (@TenantId IS NULL OR u.TenantId = @TenantId)
        AND (@UserId IS NULL OR ua.UserId = @UserId)
        AND (@ActiveOnly = 0 OR ua.Active = 1)
    ORDER BY ua.CreatedAt DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO

CREATE PROCEDURE [dbo].[SP_VALIDATE_ADDRESS]
    @Street NVARCHAR(255),
    @City NVARCHAR(100),
    @State NVARCHAR(100),
    @PostalCode NVARCHAR(20),
    @Country NVARCHAR(100) = 'IN',
    @IsValid BIT OUTPUT,
    @ValidationMessage NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    SET @IsValid = 1;
    SET @ValidationMessage = '';
    
    -- Validate required fields
    IF @Street IS NULL OR LEN(LTRIM(RTRIM(@Street))) = 0
    BEGIN
        SET @IsValid = 0;
        SET @ValidationMessage = 'Street address is required.';
        RETURN;
    END
    
    IF @City IS NULL OR LEN(LTRIM(RTRIM(@City))) = 0
    BEGIN
        SET @IsValid = 0;
        SET @ValidationMessage = 'City is required.';
        RETURN;
    END
    
    IF @State IS NULL OR LEN(LTRIM(RTRIM(@State))) = 0
    BEGIN
        SET @IsValid = 0;
        SET @ValidationMessage = 'State is required.';
        RETURN;
    END
    
    IF @PostalCode IS NULL OR LEN(LTRIM(RTRIM(@PostalCode))) = 0
    BEGIN
        SET @IsValid = 0;
        SET @ValidationMessage = 'Postal code is required.';
        RETURN;
    END
    
    -- Validate postal code format (Indian postal codes are 6 digits)
    IF @Country = 'IN' AND NOT (@PostalCode LIKE '[0-9][0-9][0-9][0-9][0-9][0-9]')
    BEGIN
        SET @IsValid = 0;
        SET @ValidationMessage = 'Postal code must be 6 digits for India.';
        RETURN;
    END
    
    -- Validate state exists in States table
    IF NOT EXISTS (SELECT 1 FROM States WHERE StateCode = @State AND Active = 1)
    BEGIN
        SET @IsValid = 0;
        SET @ValidationMessage = 'Invalid state code.';
        RETURN;
    END
    
    -- Address is valid
    SET @IsValid = 1;
    SET @ValidationMessage = 'Address is valid.';
END
GO

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

CREATE OR ALTER PROCEDURE [dbo].[SP_GET_DASHBOARD_TOP_PRODUCTS]
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

CREATE OR ALTER PROCEDURE [dbo].[SP_GET_DASHBOARD_RECENT_ORDERS]
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
			ISNULL(LTRIM(RTRIM(ISNULL(u.FirstName, '') + ' ' + ISNULL(u.LastName, ''))), 'Unknown User') AS UserName,
			ISNULL(u.Email, '') AS UserEmail,
			o.TotalAmount,
			ISNULL(o.OrderStatus, 'Pending') AS OrderStatus,
			ISNULL(o.PaymentStatus, 'Pending') AS PaymentStatus,
			o.OrderDate,
			ISNULL(COUNT(oi.OrderItemId), 0) AS ItemCount
		FROM Orders o
		INNER JOIN Users u ON o.UserId = u.UserId
		LEFT JOIN OrderItems oi ON o.OrderId = oi.OrderId AND oi.Active = 1
		WHERE o.TenantId = @TenantId
		AND o.Active = 1
		GROUP BY o.OrderId, o.OrderNumber, o.UserId, u.FirstName, u.LastName, u.Email, 
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

-- =============================================
-- SP SHOPPING
-- =============================================
CREATE PROCEDURE [dbo].[SP_GET_STATES]
    @TenantId BIGINT = NULL,
    @CountryCode NVARCHAR(10) = 'IN',
    @ActiveOnly BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Check if States table exists
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'States')
        BEGIN
            -- Return empty result set if table doesn't exist
            SELECT 
                CAST(NULL AS BIGINT) AS StateId,
                CAST(NULL AS NVARCHAR(10)) AS StateCode,
                CAST(NULL AS NVARCHAR(100)) AS StateName,
                CAST(NULL AS NVARCHAR(10)) AS CountryCode,
                CAST(NULL AS INT) AS OrderBy
            WHERE 1 = 0; -- Return no rows
            RETURN;
        END
        
        SELECT 
            StateId,
            StateCode,
            StateName,
            CountryCode,
            OrderBy
        FROM [dbo].[States]
        WHERE (@TenantId IS NULL OR TenantId = @TenantId OR TenantId IS NULL)
            AND CountryCode = @CountryCode
            AND (@ActiveOnly = 0 OR Active = 1)
        ORDER BY OrderBy, StateName;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        -- If table doesn't exist, return empty result instead of error
        IF @ErrorMessage LIKE '%Invalid object name%States%'
        BEGIN
            SELECT 
                CAST(NULL AS BIGINT) AS StateId,
                CAST(NULL AS NVARCHAR(10)) AS StateCode,
                CAST(NULL AS NVARCHAR(100)) AS StateName,
                CAST(NULL AS NVARCHAR(10)) AS CountryCode,
                CAST(NULL AS INT) AS OrderBy
            WHERE 1 = 0;
            RETURN;
        END
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO

CREATE PROCEDURE [dbo].[SP_CALCULATE_SHIPPING_CHARGE]
    @TenantId BIGINT,
    @ProductType NVARCHAR(50), -- 'Seed' or 'Plant'
    @StateCode NVARCHAR(10),
    @CourierType NVARCHAR(50), -- 'Postal' or 'Other'
    @Subtotal DECIMAL(18,2),
    @TotalQuantity INT = 1,
    @ShippingCharge DECIMAL(18,2) OUTPUT,
    @FreeShipping BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @BaseCharge DECIMAL(18,2);
    DECLARE @PerUnitCharge DECIMAL(18,2) = 0;
    DECLARE @MinCharge DECIMAL(18,2);
    DECLARE @MaxCharge DECIMAL(18,2) = NULL;
    DECLARE @FreeShippingThreshold DECIMAL(18,2) = NULL;
    DECLARE @CalculatedCharge DECIMAL(18,2) = 0;
    
    -- Determine if Tamil Nadu or Other States
    DECLARE @RateStateCode NVARCHAR(10);
    IF @StateCode = 'TN'
        SET @RateStateCode = 'TN'; -- Tamil Nadu rate
    ELSE
        SET @RateStateCode = NULL; -- All other states rate
    
    -- Get shipping rate
    SELECT TOP 1
        @BaseCharge = BaseCharge,
        @PerUnitCharge = PerUnitCharge,
        @MinCharge = MinCharge,
        @MaxCharge = MaxCharge,
        @FreeShippingThreshold = FreeShippingThreshold
    FROM ShippingRates
    WHERE TenantId = @TenantId
        AND ProductType = @ProductType
        AND StateCode = @RateStateCode
        AND CourierType = @CourierType
        AND Active = 1;
    
    -- No fallback: if no rate found, return NULL so application can show error
    IF @BaseCharge IS NULL
    BEGIN
        SET @ShippingCharge = NULL;
        SET @FreeShipping = 0;
        RETURN;
    END
    
    -- Calculate charge: Base + (Quantity × PerUnitCharge)
    SET @CalculatedCharge = @BaseCharge + (@TotalQuantity * @PerUnitCharge);
    
    -- Apply min/max limits
    IF @CalculatedCharge < @MinCharge
        SET @CalculatedCharge = @MinCharge;
    
    IF @MaxCharge IS NOT NULL AND @CalculatedCharge > @MaxCharge
        SET @CalculatedCharge = @MaxCharge;
    
    -- Check free shipping
    SET @FreeShipping = CASE 
        WHEN @FreeShippingThreshold IS NOT NULL AND @Subtotal >= @FreeShippingThreshold THEN 1
        ELSE 0
    END;
    
    IF @FreeShipping = 1
        SET @ShippingCharge = 0;
    ELSE
        SET @ShippingCharge = @CalculatedCharge;
END
GO

CREATE PROCEDURE [dbo].[SP_CALCULATE_MIXED_SHIPPING]
    @TenantId BIGINT,
    @StateCode NVARCHAR(10),
    @CourierType NVARCHAR(50), -- 'Postal' or 'Other'
    @SeedSubtotal DECIMAL(18,2),
    @PlantSubtotal DECIMAL(18,2),
    @SeedQuantity INT = 0,
    @PlantQuantity INT = 0,
    @TotalSubtotal DECIMAL(18,2),
    @ShippingCharge DECIMAL(18,2) OUTPUT,
    @FreeShipping BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @SeedCharge DECIMAL(18,2) = 0;
    DECLARE @PlantCharge DECIMAL(18,2) = 0;
    DECLARE @SeedFreeShipping BIT = 0;
    DECLARE @PlantFreeShipping BIT = 0;
    
    -- Initialize output parameters
    SET @ShippingCharge = 0;
    SET @FreeShipping = 0;
    
    -- Calculate seed shipping
    IF @SeedSubtotal > 0 AND @SeedQuantity > 0
    BEGIN
        EXEC SP_CALCULATE_SHIPPING_CHARGE 
            @TenantId, 'Seed', @StateCode, @CourierType, @SeedSubtotal, @SeedQuantity,
            @SeedCharge OUTPUT, @SeedFreeShipping OUTPUT;
    END
    
    -- Calculate plant shipping
    IF @PlantSubtotal > 0 AND @PlantQuantity > 0
    BEGIN
        EXEC SP_CALCULATE_SHIPPING_CHARGE 
            @TenantId, 'Plant', @StateCode, @CourierType, @PlantSubtotal, @PlantQuantity,
            @PlantCharge OUTPUT, @PlantFreeShipping OUTPUT;
    END
    
    -- If any required rate was not found, return NULL so application can show error
    IF @SeedCharge IS NULL OR @PlantCharge IS NULL
    BEGIN
        SET @ShippingCharge = NULL;
        SET @FreeShipping = 0;
        RETURN;
    END
    
    -- Use the higher charge (plants usually cost more)
    -- If both are 0, shipping charge remains 0
    SET @ShippingCharge = CASE 
        WHEN @PlantCharge > @SeedCharge THEN @PlantCharge
        WHEN @SeedCharge > 0 THEN @SeedCharge
        WHEN @PlantCharge > 0 THEN @PlantCharge
        ELSE 0
    END;
    
    -- Get dynamic free delivery threshold from settings
    DECLARE @MaxThreshold DECIMAL(18,2);
    SELECT @MaxThreshold = CAST(SettingValue AS DECIMAL(18,2))
    FROM AppSettings 
    WHERE SettingKey = 'FREE_DELIVERY' AND Active = 1;
    
    -- Fallback to default if not found
    IF @MaxThreshold IS NULL
        SET @MaxThreshold = 2000;
    
    -- Free shipping applies ONLY if total subtotal meets the threshold
    IF @TotalSubtotal >= @MaxThreshold
        SET @FreeShipping = 1;
    ELSE
        SET @FreeShipping = 0;
    
    -- If free shipping applies, set charge to 0
    IF @FreeShipping = 1
        SET @ShippingCharge = 0;
END
GO


-- =============================================
-- SP PRODUCT REVIEW
-- =============================================
CREATE or alter PROCEDURE [dbo].[SP_CREATE_PRODUCT_REVIEW]
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
		
		-- -- Check if user already reviewed this product
		-- IF EXISTS (
		-- 	SELECT 1 FROM ProductReviews 
		-- 	WHERE ProductId = @ProductId 
		-- 	AND UserId = @UserId 
		-- 	AND Active = 1
		-- )
		-- BEGIN
		-- 	RAISERROR('You have already reviewed this product', 16, 1);
		-- 	RETURN;
		-- END
		
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
			@AverageRating = ISNULL(AVG(CAST(pr.Rating AS DECIMAL(3,2))), 0)
		FROM ProductReviews pr
		INNER JOIN Products p ON pr.ProductId = p.ProductId
		WHERE pr.ProductId = @ProductId
		AND pr.Active = 1
		AND pr.IsApproved = 1
		AND (@TenantId IS NULL OR p.TenantId = @TenantId);
		
		-- Get rating distribution
		SELECT 
			pr.Rating,
			COUNT(*) AS Count
		FROM ProductReviews pr
		INNER JOIN Products p ON pr.ProductId = p.ProductId
		WHERE pr.ProductId = @ProductId
		AND pr.Active = 1
		AND pr.IsApproved = 1
		AND (@TenantId IS NULL OR p.TenantId = @TenantId)
		GROUP BY pr.Rating
		ORDER BY pr.Rating DESC;
		
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

-- =============================================
-- Request Password Reset (Generate and Send OTP)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[SP_REQUEST_PASSWORD_RESET]
	@Email NVARCHAR(255),
	@IpAddress NVARCHAR(45) = NULL,
	@UserAgent NVARCHAR(500) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		DECLARE @UserId BIGINT = NULL;
		DECLARE @FirstName NVARCHAR(100) = NULL;
		DECLARE @IsActive BIT = 0;
		DECLARE @EmailVerified BIT = 0;
		DECLARE @CurrentTime DATETIME2(7) = GETUTCDATE();
		DECLARE @OTP NVARCHAR(6);
		DECLARE @ExpiresAt DATETIME2(7);
		DECLARE @RecentRequestCount INT = 0;
		
		-- Find user by email
		SELECT 
			@UserId = UserId,
			@FirstName = FirstName,
			@IsActive = Active,
			@EmailVerified = EmailVerified
		FROM Users 
		WHERE Email = @Email AND Active = 1;
		
		-- Always return success message (prevent email enumeration)
		-- But only process if user exists
		IF @UserId IS NULL
		BEGIN
			SELECT 
				'If email exists, OTP has been sent to your email address.' AS Message,
				600 AS ExpiresInSeconds,
				0 AS UserId,
				'' AS OTP,
				'' AS Email
			RETURN;
		END
		
		-- Rate limiting: Check recent requests (max 3 per hour)
		SELECT @RecentRequestCount = COUNT(*)
		FROM PasswordResetOTPs
		WHERE Email = @Email 
			AND CreatedAt > DATEADD(HOUR, -1, @CurrentTime)
			AND Used = 0;
		
		IF @RecentRequestCount >= 3
		BEGIN
			SELECT 
				'Too many reset requests. Please wait before requesting again.' AS Message,
				0 AS ExpiresInSeconds,
				0 AS UserId,
				'' AS OTP,
				'' AS Email
			RETURN;
		END
		
		-- Invalidate any existing unused OTPs for this email
		UPDATE PasswordResetOTPs
		SET Used = 1,
			UsedAt = @CurrentTime
		WHERE Email = @Email 
			AND Used = 0
			AND ExpiresAt > @CurrentTime;
		
		-- Generate 6-digit OTP
		SET @OTP = RIGHT('000000' + CAST(ABS(CHECKSUM(NEWID())) % 1000000 AS NVARCHAR(6)), 6);
		SET @ExpiresAt = DATEADD(MINUTE, 10, @CurrentTime); -- 10 minutes expiration
		
		-- Insert OTP record
		INSERT INTO PasswordResetOTPs (
			UserId,
			Email,
			OTP,
			ExpiresAt,
			IpAddress,
			UserAgent,
			CreatedAt
		) VALUES (
			@UserId,
			@Email,
			@OTP,
			@ExpiresAt,
			@IpAddress,
			@UserAgent,
			@CurrentTime
		);
		
		-- Log the password reset request
		INSERT INTO UserActivityLog (
			UserId,
			ActivityType,
			ActivityDescription,
			IpAddress,
			UserAgent,
			CreatedAt
		) VALUES (
			@UserId,
			'PASSWORD_RESET_REQUESTED',
			'Password reset OTP requested',
			@IpAddress,
			@UserAgent,
			@CurrentTime
		);
		
		-- Return success (OTP will be sent via email service)
		SELECT 
			'If email exists, OTP has been sent to your email address.' AS Message,
			600 AS ExpiresInSeconds,
			@UserId AS UserId,
			@OTP AS OTP, -- Only for development/testing - remove in production
			@Email AS Email,
			@FirstName AS FirstName
		
	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		-- Return generic error message
		SELECT 
			'An error occurred while processing your request.' AS Message,
			0 AS ExpiresInSeconds,
			0 AS UserId,
			'' AS OTP,
			'' AS Email
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
GO

-- =============================================
-- Verify OTP and Reset Password
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[SP_VERIFY_RESET_OTP]
	@Email NVARCHAR(255),
	@OTP NVARCHAR(6),
	@NewPassword NVARCHAR(100),
	@IpAddress NVARCHAR(45) = NULL,
	@UserAgent NVARCHAR(500) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY
		BEGIN TRANSACTION;
		
		DECLARE @UserId BIGINT = NULL;
		DECLARE @OtpId BIGINT = NULL;
		DECLARE @ExpiresAt DATETIME2(7) = NULL;
		DECLARE @Used BIT = 0;
		DECLARE @Attempts INT = 0;
		DECLARE @CurrentTime DATETIME2(7) = GETUTCDATE();
		DECLARE @Salt NVARCHAR(50) = NULL;
		DECLARE @HashedPassword NVARCHAR(255) = NULL;
		
		-- Find valid OTP
		SELECT 
			@OtpId = OtpId,
			@UserId = UserId,
			@ExpiresAt = ExpiresAt,
			@Used = Used,
			@Attempts = Attempts
		FROM PasswordResetOTPs
		WHERE Email = @Email 
			AND OTP = @OTP
			AND Used = 0;
		
		-- Check if OTP exists
		IF @OtpId IS NULL OR @UserId IS NULL
		BEGIN
			-- Increment attempts if OTP record exists but is wrong
			IF EXISTS (SELECT 1 FROM PasswordResetOTPs WHERE Email = @Email AND Used = 0)
			BEGIN
				UPDATE PasswordResetOTPs
				SET Attempts = Attempts + 1
				WHERE Email = @Email AND Used = 0;
			END
			
			RAISERROR('Invalid OTP. Please check and try again.', 16, 1);
			RETURN;
		END
		
		-- Check if OTP has been used
		IF @Used = 1
		BEGIN
			RAISERROR('This OTP has already been used. Please request a new one.', 16, 1);
			RETURN;
		END
		
		-- Check if OTP has expired
		IF @ExpiresAt < @CurrentTime
		BEGIN
			UPDATE PasswordResetOTPs
			SET Used = 1,
				UsedAt = @CurrentTime
			WHERE OtpId = @OtpId;
			
			RAISERROR('OTP has expired. Please request a new one.', 16, 1);
			RETURN;
		END
		
		-- Check max attempts (5 attempts max)
		IF @Attempts >= 5
		BEGIN
			UPDATE PasswordResetOTPs
			SET Used = 1,
				UsedAt = @CurrentTime
			WHERE OtpId = @OtpId;
			
			RAISERROR('Too many failed attempts. Please request a new OTP.', 16, 1);
			RETURN;
		END
		
		-- Verify user is active
		IF NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId AND Active = 1)
		BEGIN
			RAISERROR('User account is not active.', 16, 1);
			RETURN;
		END
		
		-- Generate new salt and hash password
		SET @Salt = CONVERT(NVARCHAR(50), NEWID());
		SET @HashedPassword = CONVERT(NVARCHAR(100), HASHBYTES('SHA2_256', CAST(@NewPassword AS NVARCHAR(100)) + @Salt), 2);
		
		-- Update user password
		UPDATE Users
		SET 
			PasswordHash = @HashedPassword,
			Salt = @Salt,
			PasswordChangedAt = @CurrentTime,
			LastPasswordReset = @CurrentTime,
			UpdatedAt = @CurrentTime,
			LoginAttempts = 0,
			AccountLocked = 0
		WHERE UserId = @UserId;
		
		-- Mark OTP as used
		UPDATE PasswordResetOTPs
		SET 
			Used = 1,
			UsedAt = @CurrentTime
		WHERE OtpId = @OtpId;
		
		-- Invalidate all other unused OTPs for this user
		UPDATE PasswordResetOTPs
		SET Used = 1,
			UsedAt = @CurrentTime
		WHERE UserId = @UserId 
			AND Used = 0
			AND OtpId != @OtpId;
		
		-- Log the password reset activity
		INSERT INTO UserActivityLog (
			UserId,
			ActivityType,
			ActivityDescription,
			IpAddress,
			UserAgent,
			CreatedAt
		) VALUES (
			@UserId,
			'PASSWORD_RESET',
			'Password reset successfully using OTP',
			@IpAddress,
			@UserAgent,
			@CurrentTime
		);
		
		-- Return success
		SELECT 
			@UserId AS UserId,
			'Password reset successful. You can now login with your new password.' AS Message,
			@CurrentTime AS ResetAt
		
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
-- Resend Password Reset OTP
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[SP_RESEND_RESET_OTP]
	@Email NVARCHAR(255),
	@IpAddress NVARCHAR(45) = NULL,
	@UserAgent NVARCHAR(500) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	-- Reuse the same logic as request password reset
	EXEC [dbo].[SP_REQUEST_PASSWORD_RESET] 
		@Email = @Email,
		@IpAddress = @IpAddress,
		@UserAgent = @UserAgent
END
GO

-- =============================================
-- App Settings Table + Procedures
-- =============================================
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AppSettings]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[AppSettings](
		[SettingId] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[SettingKey] NVARCHAR(100) NOT NULL,
		[SettingValue] NVARCHAR(255) NOT NULL,
		[TenantId] BIGINT NULL,
		[UserId] BIGINT NULL,
		[Active] BIT NOT NULL DEFAULT(1),
		[CreatedAt] DATETIME NOT NULL DEFAULT(GETDATE()),
		[UpdatedAt] DATETIME NULL
	);

	CREATE UNIQUE INDEX UX_AppSettings_Key ON [dbo].[AppSettings]([SettingKey], [TenantId], [UserId]);
END;
GO

IF OBJECT_ID('dbo.SP_GET_APP_SETTINGS', 'P') IS NOT NULL
	DROP PROCEDURE dbo.SP_GET_APP_SETTINGS;
GO
CREATE PROCEDURE dbo.SP_GET_APP_SETTINGS
	@TenantId BIGINT = NULL,
	@UserId BIGINT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT SettingKey, SettingValue, TenantId, UserId
	FROM AppSettings
	WHERE Active = 1
	  AND (@TenantId IS NULL OR TenantId = @TenantId)
	  AND (@UserId IS NULL OR UserId = @UserId)
	ORDER BY SettingKey;
END;
GO

IF OBJECT_ID('dbo.SP_UPSERT_APP_SETTING', 'P') IS NOT NULL
	DROP PROCEDURE dbo.SP_UPSERT_APP_SETTING;
GO
CREATE PROCEDURE dbo.SP_UPSERT_APP_SETTING
	@SettingKey NVARCHAR(100),
	@SettingValue NVARCHAR(255),
	@TenantId BIGINT = NULL,
	@UserId BIGINT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
		SELECT 1 FROM AppSettings
		WHERE SettingKey = @SettingKey
		  AND ISNULL(TenantId,0) = ISNULL(@TenantId,0)
		  AND ISNULL(UserId,0) = ISNULL(@UserId,0)
	)
	BEGIN
		UPDATE AppSettings
		SET SettingValue = @SettingValue,
			UpdatedAt = GETDATE()
		WHERE SettingKey = @SettingKey
		  AND ISNULL(TenantId,0) = ISNULL(@TenantId,0)
		  AND ISNULL(UserId,0) = ISNULL(@UserId,0);
	END
	ELSE
	BEGIN
		INSERT INTO AppSettings (SettingKey, SettingValue, TenantId, UserId, Active)
		VALUES (@SettingKey, @SettingValue, @TenantId, @UserId, 1);
	END
END;
GO

-- Seed defaults
IF NOT EXISTS (SELECT 1 FROM AppSettings WHERE SettingKey = 'MIN_ORDER')
	INSERT INTO AppSettings (SettingKey, SettingValue, Active) VALUES ('MIN_ORDER', '300', 1);

IF NOT EXISTS (SELECT 1 FROM AppSettings WHERE SettingKey = 'FREE_DELIVERY')
	INSERT INTO AppSettings (SettingKey, SettingValue, Active) VALUES ('FREE_DELIVERY', '1500', 1);
GO

-- =============================================
-- ENHANCED PRODUCT SEARCH WITH ADVANCED FILTERS
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[SP_SEARCH_PRODUCTS_ENHANCED]
	@TenantId BIGINT,
	@Page INT = 1,
	@Limit INT = 10,
	@Offset INT = 0,
	@Search NVARCHAR(255) = '',
	@Category INT = NULL,
	@MinPrice DECIMAL(18,2) = NULL,
	@MaxPrice DECIMAL(18,2) = NULL,
	@Rating INT = NULL,
	@InStock BIT = NULL,
	@BestSeller BIT = NULL,
	@HasOffer BIT = NULL,
	@NewArrivalsDays INT = NULL,      -- 7 or 30 for new arrivals filter
	@Trending BIT = NULL,             -- Filter trending products (high UserBuyCount)
	@TrendingThreshold INT = 10,      -- Min UserBuyCount for trending
	@FuzzySearch BIT = 0,             -- Enable fuzzy/typo-tolerant search
	@SortBy NVARCHAR(50) = 'relevance',
	@SortOrder NVARCHAR(10) = 'desc'
AS
BEGIN
	SET NOCOUNT ON;
	
	-- Create temp table for search results with relevance scoring
	CREATE TABLE #SearchResults (
		ProductId BIGINT,
		RelevanceScore INT DEFAULT 0,
		NameMatchPos INT DEFAULT 0,       -- Position of match in name (for highlighting)
		DescMatchPos INT DEFAULT 0        -- Position of match in description
	);
	
	DECLARE @TotalCount INT = 0;
	DECLARE @SearchTerm NVARCHAR(255) = LTRIM(RTRIM(ISNULL(@Search, '')));
	
	-- Insert all products that match base criteria
	INSERT INTO #SearchResults (ProductId, RelevanceScore, NameMatchPos, DescMatchPos)
	SELECT 
		p.ProductId,
		-- Calculate relevance score
		CASE 
			-- Exact name match (highest priority)
			WHEN @SearchTerm != '' AND p.ProductName = @SearchTerm THEN 100
			-- Name starts with search term
			WHEN @SearchTerm != '' AND p.ProductName LIKE @SearchTerm + '%' THEN 80
			-- Name contains search term
			WHEN @SearchTerm != '' AND p.ProductName LIKE '%' + @SearchTerm + '%' THEN 60
			-- Product code exact match
			WHEN @SearchTerm != '' AND p.ProductCode = @SearchTerm THEN 70
			-- Description contains search term
			WHEN @SearchTerm != '' AND p.ProductDescription LIKE '%' + @SearchTerm + '%' THEN 40
			-- MetaKeywords contains search term
			WHEN @SearchTerm != '' AND p.MetaKeywords LIKE '%' + @SearchTerm + '%' THEN 30
			-- Fuzzy match on name (SOUNDEX)
			WHEN @FuzzySearch = 1 AND @SearchTerm != '' AND DIFFERENCE(p.ProductName, @SearchTerm) >= 3 THEN 20
			ELSE 0
		END +
		-- Boost for popular products
		CASE WHEN p.UserBuyCount > 50 THEN 10 
			 WHEN p.UserBuyCount > 20 THEN 5 
			 ELSE 0 END +
		-- Boost for best sellers
		CASE WHEN p.BestSeller = 1 THEN 5 ELSE 0 END +
		-- Boost for products with offers
		CASE WHEN p.Offer IS NOT NULL AND p.Offer != '' THEN 3 ELSE 0 END
		AS RelevanceScore,
		-- Find match position in name for highlighting
		CASE WHEN @SearchTerm != '' THEN CHARINDEX(@SearchTerm, p.ProductName) ELSE 0 END,
		-- Find match position in description for highlighting
		CASE WHEN @SearchTerm != '' THEN CHARINDEX(@SearchTerm, p.ProductDescription) ELSE 0 END
	FROM Products p
	WHERE p.TenantId = @TenantId
		AND p.Active = 1
		-- Text search filter (includes fuzzy)
		AND (
			@SearchTerm = ''
			OR p.ProductName LIKE '%' + @SearchTerm + '%'
			OR p.ProductDescription LIKE '%' + @SearchTerm + '%'
			OR p.ProductCode LIKE '%' + @SearchTerm + '%'
			OR p.MetaKeywords LIKE '%' + @SearchTerm + '%'
			OR (@FuzzySearch = 1 AND DIFFERENCE(p.ProductName, @SearchTerm) >= 3)
			OR (@FuzzySearch = 1 AND DIFFERENCE(p.ProductCode, @SearchTerm) >= 3)
		)
		-- Category filter
		AND (@Category IS NULL OR p.Category = @Category)
		-- Price range filter
		AND (@MinPrice IS NULL OR p.Price >= @MinPrice)
		AND (@MaxPrice IS NULL OR p.Price <= @MaxPrice)
		-- Rating filter
		AND (@Rating IS NULL OR p.Rating >= @Rating)
		-- Stock filter
		AND (@InStock IS NULL OR (@InStock = 1 AND p.Quantity > 0) OR (@InStock = 0 AND p.Quantity = 0))
		-- Best seller filter
		AND (@BestSeller IS NULL OR @BestSeller = 0 OR p.BestSeller = 1)
		-- Has offer filter
		AND (@HasOffer IS NULL OR @HasOffer = 0 OR (p.Offer IS NOT NULL AND p.Offer != ''))
		-- New arrivals filter (products created within X days)
		AND (@NewArrivalsDays IS NULL OR p.Created >= DATEADD(DAY, -@NewArrivalsDays, GETUTCDATE()))
		-- Trending filter (products with high purchase count)
		AND (@Trending IS NULL OR @Trending = 0 OR p.UserBuyCount >= @TrendingThreshold);
	
	-- Get total count
	SELECT @TotalCount = COUNT(*) FROM #SearchResults;
	
	-- Return paginated results with full product data
	SELECT 
		p.ProductId,
		p.TenantId,
		p.ProductName,
		p.ProductDescription,
		p.ProductCode,
		p.FullDescription,
		p.Specification,
		p.Story,
		p.PackQuantity,
		p.Quantity,
		p.Total,
		p.Price,
		p.Category,
		p.Rating,
		p.Active,
		p.Trending,
		p.UserBuyCount,
		p.[Return],
		p.Created,
		p.Modified,
		CASE WHEN p.Quantity > 0 THEN 1 ELSE 0 END as InStock,
		p.BestSeller,
		p.DeliveryDate,
		p.Offer,
		p.OrderBy,
		p.UserId,
		p.Overview,
		p.LongDescription,
		p.OriginalPrice,
		p.DiscountPercentage,
		p.MetaKeywords,
		sr.RelevanceScore,
		sr.NameMatchPos,
		sr.DescMatchPos
	FROM #SearchResults sr
	INNER JOIN Products p ON sr.ProductId = p.ProductId
	ORDER BY
		CASE WHEN @SortBy = 'relevance' AND @SortOrder = 'desc' THEN sr.RelevanceScore END DESC,
		CASE WHEN @SortBy = 'relevance' AND @SortOrder = 'asc' THEN sr.RelevanceScore END ASC,
		CASE WHEN @SortBy = 'productName' AND @SortOrder = 'asc' THEN p.ProductName END ASC,
		CASE WHEN @SortBy = 'productName' AND @SortOrder = 'desc' THEN p.ProductName END DESC,
		CASE WHEN @SortBy = 'price' AND @SortOrder = 'asc' THEN p.Price END ASC,
		CASE WHEN @SortBy = 'price' AND @SortOrder = 'desc' THEN p.Price END DESC,
		CASE WHEN @SortBy = 'rating' AND @SortOrder = 'desc' THEN p.Rating END DESC,
		CASE WHEN @SortBy = 'rating' AND @SortOrder = 'asc' THEN p.Rating END ASC,
		CASE WHEN @SortBy = 'userBuyCount' AND @SortOrder = 'desc' THEN p.UserBuyCount END DESC,
		CASE WHEN @SortBy = 'userBuyCount' AND @SortOrder = 'asc' THEN p.UserBuyCount END ASC,
		CASE WHEN @SortBy = 'created' AND @SortOrder = 'desc' THEN p.Created END DESC,
		CASE WHEN @SortBy = 'created' AND @SortOrder = 'asc' THEN p.Created END ASC,
		CASE WHEN @SortBy = 'bestSeller' THEN p.BestSeller END DESC,
		-- Default fallback: sort by relevance and name
		sr.RelevanceScore DESC,
		p.ProductName ASC
	OFFSET @Offset ROWS
	FETCH NEXT @Limit ROWS ONLY;
	
	-- Return pagination metadata
	SELECT 
		@TotalCount AS TotalCount,
		@Page AS CurrentPage,
		@Limit AS PageSize,
		CEILING(CAST(@TotalCount AS FLOAT) / @Limit) AS TotalPages,
		CASE WHEN @Page < CEILING(CAST(@TotalCount AS FLOAT) / @Limit) THEN 1 ELSE 0 END AS HasNext,
		CASE WHEN @Page > 1 THEN 1 ELSE 0 END AS HasPrevious;
	
	DROP TABLE #SearchResults;
END;
GO

-- =============================================
-- SEARCH SUGGESTIONS FOR AUTOCOMPLETE
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[SP_GET_SEARCH_SUGGESTIONS]
	@TenantId BIGINT,
	@Query NVARCHAR(100),
	@Limit INT = 8
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @SearchTerm NVARCHAR(100) = LTRIM(RTRIM(ISNULL(@Query, '')));
	
	IF @SearchTerm = '' OR LEN(@SearchTerm) < 2
	BEGIN
		-- Return empty result if query too short
		SELECT TOP 0 
			'' AS SuggestionText,
			'' AS SuggestionType,
			0 AS ProductId,
			0 AS CategoryId,
			0 AS MatchScore;
		RETURN;
	END
	
	-- Create temp table for all suggestions
	CREATE TABLE #Suggestions (
		SuggestionText NVARCHAR(255),
		SuggestionType NVARCHAR(20),  -- 'product', 'category', 'keyword'
		ProductId BIGINT NULL,
		CategoryId BIGINT NULL,
		MatchScore INT,
		ImageUrl NVARCHAR(500) NULL,
		Price DECIMAL(18,2) NULL
	);
	
	-- Insert matching product names
	INSERT INTO #Suggestions (SuggestionText, SuggestionType, ProductId, CategoryId, MatchScore, Price)
	SELECT TOP (@Limit)
		p.ProductName,
		'product',
		p.ProductId,
		p.Category,
		CASE
			WHEN p.ProductName LIKE @SearchTerm + '%' THEN 100  -- Starts with
			WHEN p.ProductName LIKE '%' + @SearchTerm + '%' THEN 80  -- Contains
			ELSE 50
		END +
		CASE WHEN p.BestSeller = 1 THEN 10 ELSE 0 END +
		CASE WHEN p.UserBuyCount > 10 THEN 5 ELSE 0 END,
		p.Price
	FROM Products p
	WHERE p.TenantId = @TenantId
		AND p.Active = 1
		AND p.Quantity > 0
		AND (
			p.ProductName LIKE '%' + @SearchTerm + '%'
			OR p.ProductCode LIKE @SearchTerm + '%'
		)
	ORDER BY
		CASE WHEN p.ProductName LIKE @SearchTerm + '%' THEN 0 ELSE 1 END,
		p.UserBuyCount DESC;
	
	-- Insert matching category names
	INSERT INTO #Suggestions (SuggestionText, SuggestionType, ProductId, CategoryId, MatchScore)
	SELECT TOP 3
		c.CategoryName,
		'category',
		NULL,
		c.CategoryId,
		CASE
			WHEN c.CategoryName LIKE @SearchTerm + '%' THEN 90
			ELSE 70
		END
	FROM Categories c
	WHERE c.TenantId = @TenantId
		AND c.Active = 1
		AND c.CategoryName LIKE '%' + @SearchTerm + '%';
	
	-- Insert matching keywords from MetaKeywords
	INSERT INTO #Suggestions (SuggestionText, SuggestionType, ProductId, CategoryId, MatchScore)
	SELECT TOP 3
		LTRIM(RTRIM(value)) AS Keyword,
		'keyword',
		NULL,
		NULL,
		60
	FROM Products p
	CROSS APPLY STRING_SPLIT(p.MetaKeywords, ',')
	WHERE p.TenantId = @TenantId
		AND p.Active = 1
		AND p.MetaKeywords IS NOT NULL
		AND LTRIM(RTRIM(value)) LIKE '%' + @SearchTerm + '%'
		AND LEN(LTRIM(RTRIM(value))) > 2
	GROUP BY LTRIM(RTRIM(value));
	
	-- Return deduplicated results ordered by score
	SELECT TOP (@Limit)
		SuggestionText,
		SuggestionType,
		ProductId,
		CategoryId,
		MatchScore,
		Price
	FROM #Suggestions
	GROUP BY SuggestionText, SuggestionType, ProductId, CategoryId, MatchScore, Price
	ORDER BY MatchScore DESC, SuggestionText;
	
	DROP TABLE #Suggestions;
END;
GO
