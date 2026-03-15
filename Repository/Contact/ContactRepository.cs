using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sa.Common.ADO.DataAccess;
using Tenant.Query.Context.Product;
using Tenant.Query.Model.Contact;
using Tenant.Query.Model.Constant;

namespace Tenant.Query.Repository.Contact
{
    public class ContactRepository
    {
        private readonly DataAccess _dataAccess;
        public ILogger<ContactRepository> Logger { get; set; }

        public ContactRepository(ProductContext dbContext, ILoggerFactory loggerFactory, DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            Logger = loggerFactory.CreateLogger<ContactRepository>();
        }

        /// <summary>
        /// Persist a contact-us message using the SP_CREATE_CONTACT_MESSAGE stored procedure.
        /// </summary>
        public async Task<long> CreateContactMessage(ContactMessageRequest request)
        {
            try
            {
                Logger.LogInformation("Repository: Creating contact message from {Email}", request.Email);

                var cmd = await Task.Run(() => _dataAccess.ExecuteNonQueryCMD(
                    Constant.StoredProcedures.SP_CREATE_CONTACT_MESSAGE,
                    request.UserId ?? (object)DBNull.Value,
                    request.TenantId ?? (object)DBNull.Value,
                    request.Name,
                    request.Email,
                    string.IsNullOrWhiteSpace(request.Phone) ? (object)DBNull.Value : request.Phone,
                    string.IsNullOrWhiteSpace(request.Subject) ? (object)DBNull.Value : request.Subject,
                    request.Message,
                    string.IsNullOrWhiteSpace(request.Language) ? (object)DBNull.Value : request.Language,
                    string.IsNullOrWhiteSpace(request.Source) ? (object)DBNull.Value : request.Source,
                    DBNull.Value // @Id OUTPUT - will be auto-discovered
                ));

                if (!cmd.Parameters.Contains("@Id"))
                {
                    // If SP does not expose output yet, just return 0 and let caller ignore id.
                    return 0;
                }

                var idValue = cmd.Parameters["@Id"].Value;
                var id = idValue != null && idValue != DBNull.Value ? Convert.ToInt64(idValue) : 0;
                return id;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Repository: Error creating contact message");
                throw;
            }
        }
    }
}

