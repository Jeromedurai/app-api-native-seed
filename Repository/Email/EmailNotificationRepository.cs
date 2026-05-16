using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Tenant.Query.Model.Constant;
using Tenant.Query.Model.Email;

namespace Tenant.Query.Repository.Email
{
    /// <summary>
    /// Repository for SA_EMAILTEMPLATE / SA_EMAILSCHEDULE admin operations and the
    /// dispatcher's TemplateId lookup.
    ///
    /// All admin/CRUD methods take an optional tenantId:
    ///   * non-NULL → caller is a normal admin; reads see global + their tenant's
    ///                rows, writes are scoped to the caller's tenant.
    ///   * NULL     → super-admin / worker / global context (no scoping).
    ///
    /// The dispatcher passes the queue-row tenant when looking up a template;
    /// the SP returns the most specific match (tenant override beats global).
    /// </summary>
    public class EmailNotificationRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<EmailNotificationRepository> _logger;

        public EmailNotificationRepository(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            var raw = configuration["ConnectionStrings:Default"];
            if (string.IsNullOrWhiteSpace(raw))
            {
                throw new InvalidOperationException("ConnectionStrings:Default is required.");
            }
            _connectionString = Startup.SetConnectionString(raw);
            _logger = loggerFactory.CreateLogger<EmailNotificationRepository>();
        }

        // =====================================================================
        // Templates
        // =====================================================================

        public async Task<List<EmailTemplateDto>> GetAllTemplatesAdminAsync(long? tenantId)
        {
            var list = new List<EmailTemplateDto>();
            await using var conn = new SqlConnection(_connectionString);
            await using var cmd  = new SqlCommand(Constant.StoredProcedures.SA_GET_ALL_EMAIL_TEMPLATES_ADMIN, conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@TenantId", SqlDbType.BigInt) { Value = (object)tenantId ?? DBNull.Value });

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapTemplate(reader));
            }
            return list;
        }

        public async Task<EmailTemplateDto> GetTemplateByIdAsync(long templateId, long? tenantId)
        {
            await using var conn = new SqlConnection(_connectionString);
            await using var cmd  = new SqlCommand(Constant.StoredProcedures.SA_GET_EMAIL_TEMPLATE_BY_ID, conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@TemplateId", SqlDbType.BigInt) { Value = templateId });
            cmd.Parameters.Add(new SqlParameter("@TenantId",   SqlDbType.BigInt) { Value = (object)tenantId ?? DBNull.Value });

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapTemplate(reader) : null;
        }

        public async Task<long> UpsertTemplateAsync(UpsertEmailTemplateRequest req, long? tenantId)
        {
            await using var conn = new SqlConnection(_connectionString);
            await using var cmd  = new SqlCommand(Constant.StoredProcedures.SA_UPSERT_EMAIL_TEMPLATE, conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@TemplateId",   SqlDbType.BigInt)         { Value = req.TemplateId });
            cmd.Parameters.Add(new SqlParameter("@TemplateName", SqlDbType.VarChar, 100)   { Value = (object)req.TemplateName ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Description",  SqlDbType.VarChar, 255)   { Value = (object)req.Description  ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ViewName",     SqlDbType.VarChar, 100)   { Value = (object)req.ViewName     ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Active",       SqlDbType.Bit)            { Value = req.Active });
            cmd.Parameters.Add(new SqlParameter("@TenantId",     SqlDbType.BigInt)         { Value = (object)tenantId ?? DBNull.Value });

            await conn.OpenAsync();
            var scalar = await cmd.ExecuteScalarAsync();
            if (scalar == null || scalar == DBNull.Value)
                throw new InvalidOperationException("Upsert template did not return a TEMPLATEID.");
            return Convert.ToInt64(scalar);
        }

        public async Task<int> DeleteTemplateAsync(long templateId, long? tenantId)
        {
            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await using var cmd  = new SqlCommand(Constant.StoredProcedures.SA_DELETE_EMAIL_TEMPLATE, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@TemplateId", SqlDbType.BigInt) { Value = templateId });
                cmd.Parameters.Add(new SqlParameter("@TenantId",   SqlDbType.BigInt) { Value = (object)tenantId ?? DBNull.Value });

                await conn.OpenAsync();
                var scalar = await cmd.ExecuteScalarAsync();
                return scalar == null || scalar == DBNull.Value ? 0 : Convert.ToInt32(scalar);
            }
            catch (SqlException ex) when (ex.Message.Contains("schedules reference this template"))
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }

        public async Task<int> SetTemplateActiveAsync(long templateId, bool active, long? tenantId)
        {
            await using var conn = new SqlConnection(_connectionString);
            await using var cmd  = new SqlCommand(Constant.StoredProcedures.SA_SET_EMAIL_TEMPLATE_ACTIVE, conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@TemplateId", SqlDbType.BigInt) { Value = templateId });
            cmd.Parameters.Add(new SqlParameter("@Active",     SqlDbType.Bit)    { Value = active });
            cmd.Parameters.Add(new SqlParameter("@TenantId",   SqlDbType.BigInt) { Value = (object)tenantId ?? DBNull.Value });

            await conn.OpenAsync();
            var scalar = await cmd.ExecuteScalarAsync();
            return scalar == null || scalar == DBNull.Value ? 0 : Convert.ToInt32(scalar);
        }

        // =====================================================================
        // Schedules
        // =====================================================================

        public async Task<List<EmailScheduleDto>> GetAllSchedulesAdminAsync(long? tenantId)
        {
            var list = new List<EmailScheduleDto>();
            await using var conn = new SqlConnection(_connectionString);
            await using var cmd  = new SqlCommand(Constant.StoredProcedures.SA_GET_ALL_EMAIL_SCHEDULES_ADMIN, conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@TenantId", SqlDbType.BigInt) { Value = (object)tenantId ?? DBNull.Value });

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapSchedule(reader));
            }
            return list;
        }

        public async Task<long> UpsertScheduleAsync(UpsertEmailScheduleRequest req, long? tenantId)
        {
            object timeValue = DBNull.Value;
            if (!string.IsNullOrWhiteSpace(req.Time) && TimeSpan.TryParse(req.Time, out var ts))
            {
                timeValue = ts;
            }

            await using var conn = new SqlConnection(_connectionString);
            await using var cmd  = new SqlCommand(Constant.StoredProcedures.SA_UPSERT_EMAIL_SCHEDULE, conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@ScheduleId",          SqlDbType.BigInt)        { Value = req.ScheduleId });
            cmd.Parameters.Add(new SqlParameter("@TemplateId",          SqlDbType.BigInt)        { Value = req.TemplateId });
            cmd.Parameters.Add(new SqlParameter("@ScheduleDescription", SqlDbType.VarChar, 100)  { Value = (object)req.ScheduleDescription ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@SendBy",              SqlDbType.TinyInt)       { Value = req.SendBy });
            cmd.Parameters.Add(new SqlParameter("@Day",                 SqlDbType.VarChar, 50)   { Value = (object)req.Day ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Time",                SqlDbType.Time)          { Value = timeValue });
            cmd.Parameters.Add(new SqlParameter("@Active",              SqlDbType.Bit)           { Value = req.Active });
            cmd.Parameters.Add(new SqlParameter("@TenantId",            SqlDbType.BigInt)        { Value = (object)tenantId ?? DBNull.Value });

            await conn.OpenAsync();
            var scalar = await cmd.ExecuteScalarAsync();
            if (scalar == null || scalar == DBNull.Value)
                throw new InvalidOperationException("Upsert schedule did not return a SCHEDULEID.");
            return Convert.ToInt64(scalar);
        }

        public async Task<int> DeleteScheduleAsync(long scheduleId, long? tenantId)
        {
            await using var conn = new SqlConnection(_connectionString);
            await using var cmd  = new SqlCommand(Constant.StoredProcedures.SA_DELETE_EMAIL_SCHEDULE, conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@ScheduleId", SqlDbType.BigInt) { Value = scheduleId });
            cmd.Parameters.Add(new SqlParameter("@TenantId",   SqlDbType.BigInt) { Value = (object)tenantId ?? DBNull.Value });

            await conn.OpenAsync();
            var scalar = await cmd.ExecuteScalarAsync();
            return scalar == null || scalar == DBNull.Value ? 0 : Convert.ToInt32(scalar);
        }

        public async Task<int> SetScheduleActiveAsync(long scheduleId, bool active, long? tenantId)
        {
            await using var conn = new SqlConnection(_connectionString);
            await using var cmd  = new SqlCommand(Constant.StoredProcedures.SA_SET_EMAIL_SCHEDULE_ACTIVE, conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@ScheduleId", SqlDbType.BigInt) { Value = scheduleId });
            cmd.Parameters.Add(new SqlParameter("@Active",     SqlDbType.Bit)    { Value = active });
            cmd.Parameters.Add(new SqlParameter("@TenantId",   SqlDbType.BigInt) { Value = (object)tenantId ?? DBNull.Value });

            await conn.OpenAsync();
            var scalar = await cmd.ExecuteScalarAsync();
            return scalar == null || scalar == DBNull.Value ? 0 : Convert.ToInt32(scalar);
        }

        // =====================================================================
        // Schedule pending stats (admin UI: Pending / Last Activity columns)
        // =====================================================================

        public async Task<List<SchedulePendingStatsDto>> GetSchedulePendingStatsAsync(long? tenantId)
        {
            var list = new List<SchedulePendingStatsDto>();
            await using var conn = new SqlConnection(_connectionString);
            await using var cmd  = new SqlCommand(Constant.StoredProcedures.SA_GET_SCHEDULE_PENDING_STATS, conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@TenantId", SqlDbType.BigInt) { Value = (object)tenantId ?? DBNull.Value });

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new SchedulePendingStatsDto
                {
                    ScheduleId      = ReadInt64(reader, "SCHEDULEID"),
                    TenantId        = ReadNullableInt64(reader, "TENANTID"),
                    TemplateId      = ReadInt64(reader, "TEMPLATEID"),
                    PendingCount    = ReadInt32(reader, "PENDING_COUNT"),
                    RetryCount      = ReadInt32(reader, "RETRY_COUNT"),
                    InFlightCount   = ReadInt32(reader, "INFLIGHT_COUNT"),
                    LastEnqueuedAt  = ReadNullableDateTime(reader, "LAST_ENQUEUED_AT"),
                    LastSuccessAt   = ReadNullableDateTime(reader, "LAST_SUCCESS_AT"),
                    LastFailureAt   = ReadNullableDateTime(reader, "LAST_FAILURE_AT")
                });
            }
            return list;
        }

        // =====================================================================
        // Worker status (LAST_RUN from SA_SERVICECONFIG)
        // =====================================================================

        public async Task<DateTime?> GetWorkerLastRunAsync(string serviceName)
        {
            await using var conn = new SqlConnection(_connectionString);
            await using var cmd  = new SqlCommand(
                "SELECT LAST_RUN FROM SA_SERVICECONFIG WITH (NOLOCK) WHERE SERVICENAME = @name", conn);
            cmd.Parameters.Add(new SqlParameter("@name", SqlDbType.VarChar, 100) { Value = serviceName });

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result == null || result == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(result);
        }

        // =====================================================================
        // Dispatcher: dynamic SP enrichment
        // =====================================================================

        public async Task<IDictionary<string, object>> ExecuteDynamicEmailSpAsync(string spName, IReadOnlyList<string> parameters)
        {
            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await using var cmd  = new SqlCommand(spName, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if (parameters != null)
                {
                    for (int i = 0; i < parameters.Count; i++)
                    {
                        cmd.Parameters.Add(new SqlParameter($"@p{i}", SqlDbType.NVarChar) { Value = (object)parameters[i] ?? DBNull.Value });
                    }
                }

                await conn.OpenAsync();
                await using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                }

                var dict = new Dictionary<string, object>(reader.FieldCount, StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    dict[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                }
                return dict;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dispatcher SP '{SpName}' failed.", spName);
                throw;
            }
        }

        // =====================================================================
        // Mappers
        // =====================================================================

        private static EmailTemplateDto MapTemplate(SqlDataReader reader) => new EmailTemplateDto
        {
            TemplateId   = ReadInt64(reader, "TEMPLATEID"),
            TenantId     = ReadNullableInt64(reader, "TENANTID"),
            TemplateName = ReadString(reader, "TEMPLATENAME"),
            Description  = ReadString(reader, "DESCRIPTION"),
            ViewName     = ReadString(reader, "VIEWNAME"),
            Active       = ReadBool(reader, "ACTIVE")
        };

        private static EmailScheduleDto MapSchedule(SqlDataReader reader)
        {
            byte sendBy = ReadByte(reader, "SENDBY");
            string time = null;
            int timeOrdinal = TryGetOrdinal(reader, "TIME");
            if (timeOrdinal >= 0 && !reader.IsDBNull(timeOrdinal))
            {
                var raw = reader.GetValue(timeOrdinal);
                if (raw is TimeSpan ts) time = ts.ToString(@"hh\:mm\:ss");
                else if (raw is DateTime dt) time = dt.ToString("HH:mm:ss");
                else time = raw.ToString();
            }

            return new EmailScheduleDto
            {
                ScheduleId          = ReadInt64(reader, "SCHEDULEID"),
                TenantId            = ReadNullableInt64(reader, "TENANTID"),
                TemplateId          = ReadInt64(reader, "TEMPLATEID"),
                TemplateName        = ReadString(reader, "TEMPLATENAME"),
                ScheduleDescription = ReadString(reader, "SCHEDULEDESCRIPTION"),
                SendBy              = sendBy,
                SendByLabel         = SendByLabelFor(sendBy),
                Day                 = ReadString(reader, "DAY"),
                Time                = time,
                Active              = ReadBool(reader, "ACTIVE")
            };
        }

        private static string SendByLabelFor(byte sendBy)
        {
            switch (sendBy)
            {
                case 1: return "Immediate";
                case 2: return "Daily";
                case 3: return "Weekly";
                case 4: return "Monthly";
                default: return "Not Scheduled";
            }
        }

        // ---- Reader helpers (defensive against missing columns / nulls) -----

        private static int TryGetOrdinal(SqlDataReader reader, string name)
        {
            try { return reader.GetOrdinal(name); } catch { return -1; }
        }

        private static string ReadString(SqlDataReader reader, string name)
        {
            int ord = TryGetOrdinal(reader, name);
            return ord < 0 || reader.IsDBNull(ord) ? null : reader.GetValue(ord)?.ToString();
        }

        private static long ReadInt64(SqlDataReader reader, string name)
        {
            int ord = TryGetOrdinal(reader, name);
            return ord < 0 || reader.IsDBNull(ord) ? 0L : Convert.ToInt64(reader.GetValue(ord));
        }

        private static long? ReadNullableInt64(SqlDataReader reader, string name)
        {
            int ord = TryGetOrdinal(reader, name);
            return ord < 0 || reader.IsDBNull(ord) ? (long?)null : Convert.ToInt64(reader.GetValue(ord));
        }

        private static int ReadInt32(SqlDataReader reader, string name)
        {
            int ord = TryGetOrdinal(reader, name);
            return ord < 0 || reader.IsDBNull(ord) ? 0 : Convert.ToInt32(reader.GetValue(ord));
        }

        private static byte ReadByte(SqlDataReader reader, string name)
        {
            int ord = TryGetOrdinal(reader, name);
            return ord < 0 || reader.IsDBNull(ord) ? (byte)0 : Convert.ToByte(reader.GetValue(ord));
        }

        private static bool ReadBool(SqlDataReader reader, string name)
        {
            int ord = TryGetOrdinal(reader, name);
            return ord >= 0 && !reader.IsDBNull(ord) && Convert.ToBoolean(reader.GetValue(ord));
        }

        private static DateTime? ReadNullableDateTime(SqlDataReader reader, string name)
        {
            int ord = TryGetOrdinal(reader, name);
            return ord < 0 || reader.IsDBNull(ord) ? (DateTime?)null : Convert.ToDateTime(reader.GetValue(ord));
        }
    }
}
