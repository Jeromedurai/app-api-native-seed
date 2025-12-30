using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tenant.API.Base.Repository;
using Tenant.API.Base.Service;

namespace Tenant.Query.Service.AppNotification
{
    public class AppNotificationService : TnBaseService
    {

        #region Variables

        private Repository.AppNotification.AppNotificationRepository _appNotificationRepository;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:XtraChef.System.Query.Service.InAppNotification.InAppNotificationService"/> class.
        /// </summary>
        /// <param name="loggerFactory">Logger factory.</param>
        /// <param name="xcAuditTime">Xc audit time.</param>
        public AppNotificationService(Repository.AppNotification.AppNotificationRepository InAppNotificationRepository, ILoggerFactory loggerFactory, TnAudit xcAudit, TnValidation xcValidation) : base(xcAudit, xcValidation)
        {
            this._appNotificationRepository = InAppNotificationRepository;
            this.Logger = loggerFactory.CreateLogger<Service.AppNotification.AppNotificationService>();
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Get inapp notifications
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="locationId"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageRowCount"></param>
        /// <returns></returns>
        public async Task<Model.AppNotification.Notification> GetInAppNotifications(long tenantId, long locationId, long userId, long roleId, int pageNumber, int pageRowCount)
        {
            try
            {
                //logger
                this.Logger.LogInformation($"Begin get inapp notifications for tenant:{tenantId},location:{locationId},user:{userId},role:{roleId},pageNumber:{pageNumber} and pageRowCount:{pageRowCount}");

                Model.AppNotification.Notification notification = null;

                string spName = this.Configuration["StoredProcedure:GetInAppNotifications"];

                //null check
                if (string.IsNullOrEmpty(spName))
                {
                    //logger
                    this.Logger.LogWarning($"Stored procedure is not configured for GetInAppNotifications");
                    throw new KeyNotFoundException($"Stored procedure is not configured for GetInAppNotifications");
                }

                // Use await instead of .Result to avoid blocking
                List<Model.AppNotification.NotificationMessage> notificationMessages = await this._appNotificationRepository.GetInAppNotifications(tenantId, locationId, userId, roleId, pageNumber, pageRowCount, spName);

                if (notificationMessages == null || notificationMessages.Count <= 0)
                {
                    //logger
                    this.Logger.LogWarning($"Inapp notifications are not found for tenant:{tenantId},location:{locationId},user:{userId},role:{roleId},pageNumber:{pageNumber} and pageRowCount:{pageRowCount}");
                    throw new KeyNotFoundException($"Inapp notifications are not found");
                }

                notification = new Model.AppNotification.Notification()
                {
                    Count = notificationMessages.FirstOrDefault().Count,
                    Messages = notificationMessages
                };

                //logger
                this.Logger.LogInformation($"Inapp notifications are retrieved for tenant:{tenantId},location:{locationId},user:{userId},role:{roleId},pageNumber:{pageNumber} and pageRowCount:{pageRowCount}");

                //result
                return notification;

            }
            catch (Exception ex)
            {
                //logger
                this.Logger.LogInformation($"Error in getting inapp notifications for tenant:{tenantId},location:{locationId},user:{userId},role:{roleId},pageNumber:{pageNumber} and pageRowCount:{pageRowCount} : {ex.Message}");
                this.Logger.LogError($"Inner exception: {ex.InnerException}");
                this.Logger.LogError($"Stack trace: {ex.StackTrace}");
                throw ex;
            }
        }

        #endregion
    }
}
