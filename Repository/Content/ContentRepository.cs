using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Threading.Tasks;
using Tenant.API.Base.Repository;
using Tenant.Query.Model.Constant;

namespace Tenant.Query.Repository.Content
{
    public class ContentRepository : TnBaseQueryRepository<Model.AppNotification.AppConfig, Context.Content.ContentContext>
    {

        private string dbConnectionString = string.Empty;

        public ContentRepository(Context.Content.ContentContext dbContext, ILoggerFactory loggerFactory) : base(dbContext, loggerFactory)
        {       
            dbConnectionString = this.DbContext.Database.GetDbConnection().ConnectionString;
        }

        #region Overridden Methods
        public override Task<Model.AppNotification.AppConfig> GetById(string tenantId, string id)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
