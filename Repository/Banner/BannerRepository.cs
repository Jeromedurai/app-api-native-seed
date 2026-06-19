using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Sa.Common.ADO.DataAccess;
using Tenant.Query.Context.Product;
using Tenant.Query.Model.Banner;
using Tenant.Query.Model.Constant;

namespace Tenant.Query.Repository.Banner
{
    /// <summary>
    /// Data access for homepage/campaign banners. Mirrors the coupon repository style
    /// (DataAccess.ExecuteDataset + SP constants). Fully separate from product images.
    /// </summary>
    public class BannerRepository
    {
        private readonly DataAccess _dataAccess;
        public ILogger<BannerRepository> Logger { get; set; }

        public BannerRepository(ProductContext dbContext, ILoggerFactory loggerFactory, DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            Logger = loggerFactory.CreateLogger<BannerRepository>();
        }

        public async Task<BannerResponse> CreateBanner(long tenantId, CreateBannerRequest request,
            string imageName, string contentType, long fileSize, byte[] imageData, long? createdBy)
        {
            var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                Constant.StoredProcedures.SP_CREATE_BANNER,
                tenantId,
                (object)request.Title ?? DBNull.Value,
                (object)request.CtaText ?? DBNull.Value,
                (object)request.CtaLink ?? DBNull.Value,
                imageName,
                contentType,
                fileSize,
                imageData,
                request.OrderBy,
                (object)request.StartDate ?? DBNull.Value,
                (object)request.EndDate ?? DBNull.Value,
                request.Active,
                createdBy ?? (object)DBNull.Value
            ));

            if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                throw new InvalidOperationException("Failed to create banner");

            return MapToBannerResponse(result.Tables[0].Rows[0]);
        }

        public async Task<BannerResponse> UpdateBanner(long bannerId, long tenantId, UpdateBannerRequest request,
            string imageName, string contentType, long? fileSize, byte[] imageData, long? updatedBy)
        {
            var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                Constant.StoredProcedures.SP_UPDATE_BANNER,
                bannerId,
                tenantId,
                (object)request.Title ?? DBNull.Value,
                (object)request.CtaText ?? DBNull.Value,
                (object)request.CtaLink ?? DBNull.Value,
                request.OrderBy,
                (object)request.StartDate ?? DBNull.Value,
                (object)request.EndDate ?? DBNull.Value,
                request.Active,
                (object)imageName ?? DBNull.Value,
                (object)contentType ?? DBNull.Value,
                (object)fileSize ?? DBNull.Value,
                (object)imageData ?? DBNull.Value,
                updatedBy ?? (object)DBNull.Value
            ));

            // UPDATE then SELECT — find the table that has the banner row.
            DataTable table = null;
            if (result != null)
            {
                for (int i = result.Tables.Count - 1; i >= 0; i--)
                {
                    if (result.Tables[i].Rows.Count > 0 && result.Tables[i].Columns.Contains("BannerId"))
                    {
                        table = result.Tables[i];
                        break;
                    }
                }
            }
            if (table == null || table.Rows.Count == 0)
                throw new KeyNotFoundException($"Banner with ID {bannerId} not found");

            return MapToBannerResponse(table.Rows[0]);
        }

        public async Task<bool> DeleteBanner(long bannerId, long tenantId)
        {
            var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                Constant.StoredProcedures.SP_DELETE_BANNER, bannerId, tenantId));
            if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0) return false;
            var rows = result.Tables[0].Rows[0]["ROWSAFFECTED"];
            return rows != DBNull.Value && Convert.ToInt32(rows) > 0;
        }

        public async Task<List<BannerResponse>> GetBannersAdmin(long tenantId)
        {
            var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                Constant.StoredProcedures.SP_GET_BANNERS_ADMIN, tenantId));
            return MapList(result);
        }

        public async Task<List<BannerResponse>> GetActiveBanners(long tenantId)
        {
            var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                Constant.StoredProcedures.SP_GET_ACTIVE_BANNERS, tenantId));
            return MapList(result);
        }

        public async Task<BannerImageBinary> GetBannerImage(long bannerId)
        {
            var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                Constant.StoredProcedures.SP_GET_BANNER_IMAGE_BY_ID, bannerId));
            if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                return null;

            var row = result.Tables[0].Rows[0];
            return new BannerImageBinary
            {
                BannerId = Convert.ToInt64(row["BannerId"]),
                ImageName = row["ImageName"]?.ToString() ?? "banner",
                ContentType = row["ContentType"]?.ToString() ?? "image/jpeg",
                ImageData = row["ImageData"] as byte[]
            };
        }

        #region Helpers
        private static List<BannerResponse> MapList(DataSet result)
        {
            if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                return new List<BannerResponse>();
            return result.Tables[0].AsEnumerable().Select(MapToBannerResponse).ToList();
        }

        private static BannerResponse MapToBannerResponse(DataRow row)
        {
            return new BannerResponse
            {
                BannerId = Convert.ToInt64(row["BannerId"]),
                TenantId = Convert.ToInt64(row["TenantId"]),
                Title = row["Title"]?.ToString(),
                CtaText = row["CtaText"]?.ToString(),
                CtaLink = row["CtaLink"]?.ToString(),
                ImageName = row["ImageName"]?.ToString(),
                ContentType = row["ContentType"]?.ToString(),
                FileSize = row["FileSize"] != DBNull.Value ? Convert.ToInt64(row["FileSize"]) : (long?)null,
                OrderBy = row["OrderBy"] != DBNull.Value ? Convert.ToInt32(row["OrderBy"]) : 0,
                StartDate = row["StartDate"] != DBNull.Value ? Convert.ToDateTime(row["StartDate"]) : (DateTime?)null,
                EndDate = row["EndDate"] != DBNull.Value ? Convert.ToDateTime(row["EndDate"]) : (DateTime?)null,
                Active = row["Active"] != DBNull.Value && Convert.ToBoolean(row["Active"]),
                CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                UpdatedAt = Convert.ToDateTime(row["UpdatedAt"]),
            };
        }
        #endregion
    }
}
