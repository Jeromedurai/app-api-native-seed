using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Tenant.API.Base.Controller;
using Tenant.API.Base.Model;

namespace Tenant.Query.Controllers.AppNotification
{
    [Route("api/1.0/app-notifications")]
    public class AppNotificationController : TnBaseController<Service.AppNotification.AppNotificationService>
    {
        #region constructor

        public AppNotificationController(Service.AppNotification.AppNotificationService service, IConfiguration configuration, ILoggerFactory loggerFactory) : base(service, configuration, loggerFactory)
        {
        }

        #endregion

        #region GET

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
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(Model.AppNotification.Notification))]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        [Route("tenants/{tenantId}/locations/{locationId}/roles/{roleId}/users/{userId}")]
        [HttpGet]
        public async Task<IActionResult> GetInAppNotifications([FromRoute]long tenantId, [FromRoute]long locationId,
                                                            [FromRoute]long userId, [FromRoute]long roleId,
                                                            [FromQuery(Name = "pagenumber")]int pageNumber = 1,
                                                            [FromQuery(Name = "pagerowcount")]int pageRowCount = 10)
        {
            try
            {
                Model.AppNotification.Notification notification = await this.Service.GetInAppNotifications(tenantId, locationId, userId, roleId, pageNumber, pageRowCount);

                return StatusCode(StatusCodes.Status200OK, new ApiResult() { Data = notification });
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new ApiResult() { Exception = ex.Message });
            }
            catch (SystemException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult() { Exception = ex.Message });
            }
        }

        #endregion
    }
}