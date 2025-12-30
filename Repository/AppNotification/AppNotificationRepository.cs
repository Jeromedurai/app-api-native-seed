using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tenant.API.Base.Repository;
using Tenant.Query.Model.AppNotification;

namespace Tenant.Query.Repository.AppNotification
{
    public class AppNotificationRepository : TnBaseQueryRepository<Model.AppNotification.AppNotification, Context.AppNotification.AppNotificationContext>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:XtraChef.System.Query.Repository.InAppNotification.InAppNotification"/> class.
        /// </summary>
        /// <param name="dbContext">Db context.</param>
        /// <param name="loggerFactory">Logger factory.</param>
        public AppNotificationRepository(Context.AppNotification.AppNotificationContext dbContext, ILoggerFactory loggerFactory) : base(dbContext, loggerFactory)
        {
        }
        #endregion

        #region Overridden Methods
        public override Task<Model.AppNotification.AppNotification> GetById(string tenantId, string id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Get inapp notifications
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="locationId"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageRowCount"></param>
        /// <param name="spName"></param>
        /// <returns></returns>
        public async Task<List<Model.AppNotification.NotificationMessage>> GetInAppNotifications(long tenantId, long locationId, long userId, long roleId, int pageNumber, int pageRowCount, string spName)
        {
            try
            {
                //logger
                this.Logger.LogInformation($"Calling stored procedure to get inapp notifications for tenant:{tenantId},location:{locationId},user:{userId},role:{roleId},pageNumber:{pageNumber} and pageRowCount:{pageRowCount}");

                //set sql params
                SqlParameter TenantId = new SqlParameter
                {
                    ParameterName = "@TENANTID",
                    SqlDbType = SqlDbType.BigInt,
                    Value = tenantId
                };

                SqlParameter LocationId = new SqlParameter
                {
                    ParameterName = "@LOCATIONID",
                    SqlDbType = SqlDbType.BigInt,
                    Value = locationId
                };

                SqlParameter UserId = new SqlParameter
                {
                    ParameterName = "@USERID",
                    SqlDbType = SqlDbType.BigInt,
                    Value = userId
                };

                SqlParameter RoleId = new SqlParameter
                {
                    ParameterName = "@ROLEID",
                    SqlDbType = SqlDbType.BigInt,
                    Value = roleId
                };

                SqlParameter PageRowCount = new SqlParameter
                {
                    ParameterName = "@PAGE_ROWS_COUNT",
                    SqlDbType = SqlDbType.Int,
                    Value = pageRowCount
                };

                SqlParameter PageNumber = new SqlParameter
                {
                    ParameterName = "@PAGE_NUMBER",
                    SqlDbType = SqlDbType.Int,
                    Value = pageNumber
                };

                //get inapp notifications
                List<Model.AppNotification.NotificationMessage> notificationMessages = await this.DbContext.NotificationMessages.FromSqlRaw($"exec {spName} @TENANTID, @LOCATIONID,@USERID,@ROLEID, @PAGE_ROWS_COUNT, @PAGE_NUMBER", TenantId, LocationId, UserId, RoleId, PageRowCount, PageNumber).AsNoTracking().ToListAsync();

                return notificationMessages;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
