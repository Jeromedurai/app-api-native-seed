using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        /// <summary>
        /// Retrieve contact-us messages using the SP_GET_CONTACT_MESSAGES stored procedure.
        /// When tenantId is null, returns messages across all tenants.
        /// </summary>
        public async Task<List<ContactMessageInfo>> GetContactMessages(long? tenantId)
        {
            try
            {
                Logger.LogInformation("Repository: Getting contact messages for tenant {TenantId}", tenantId);

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_GET_CONTACT_MESSAGES,
                    tenantId ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    return new List<ContactMessageInfo>();
                }

                return result.Tables[0].AsEnumerable()
                    .Select(MapToContactMessageInfo)
                    .ToList();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Repository: Error getting contact messages");
                throw;
            }
        }

        /// <summary>
        /// Mark a contact message as viewed (read) by an admin.
        /// Returns the number of rows updated (0 if already viewed or not found).
        /// </summary>
        public async Task<int> MarkContactMessageViewed(long id, long? viewedBy, long? tenantId)
        {
            try
            {
                Logger.LogInformation("Repository: Marking contact message {Id} viewed by {ViewedBy}", id, viewedBy);

                var result = await Task.Run(() => _dataAccess.ExecuteDataset(
                    Constant.StoredProcedures.SP_MARK_CONTACT_MESSAGE_VIEWED,
                    id,
                    viewedBy ?? (object)DBNull.Value,
                    tenantId ?? (object)DBNull.Value
                ));

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    return 0;
                }

                var value = result.Tables[0].Rows[0]["RowsAffected"];
                return value != null && value != DBNull.Value ? Convert.ToInt32(value) : 0;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Repository: Error marking contact message {Id} viewed", id);
                throw;
            }
        }

        private static ContactMessageInfo MapToContactMessageInfo(DataRow row)
        {
            var columns = row.Table.Columns;
            return new ContactMessageInfo
            {
                Id = Convert.ToInt64(row["Id"]),
                UserId = row["UserId"] != DBNull.Value ? Convert.ToInt64(row["UserId"]) : (long?)null,
                TenantId = row["TenantId"] != DBNull.Value ? Convert.ToInt64(row["TenantId"]) : (long?)null,
                Name = row["Name"] as string,
                Email = row["Email"] as string,
                Phone = row["Phone"] as string,
                Subject = row["Subject"] as string,
                Message = row["Message"] as string,
                Language = row["Language"] as string,
                Source = row["Source"] as string,
                IsViewed = columns.Contains("IsViewed") && row["IsViewed"] != DBNull.Value && Convert.ToBoolean(row["IsViewed"]),
                ViewedAt = columns.Contains("ViewedAt") && row["ViewedAt"] != DBNull.Value ? Convert.ToDateTime(row["ViewedAt"]) : (DateTime?)null,
                ViewedBy = columns.Contains("ViewedBy") && row["ViewedBy"] != DBNull.Value ? Convert.ToInt64(row["ViewedBy"]) : (long?)null,
                CreatedAt = Convert.ToDateTime(row["CreatedAt"])
            };
        }
    }
}

