using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Tenant.Query.Service.Whatsapp
{
    /// <summary>
    /// Default no-op WhatsApp sender: logs what WOULD be sent. Used until a real
    /// provider + approved templates are configured (WhatsApp:Provider = "Msg91").
    /// </summary>
    public class LoggingWhatsAppService : IWhatsAppService
    {
        private readonly ILogger<LoggingWhatsAppService> _logger;

        public LoggingWhatsAppService(ILogger<LoggingWhatsAppService> logger)
        {
            _logger = logger;
        }

        public Task<bool> SendTemplateAsync(string toPhone, string templateName, IDictionary<string, object> data)
        {
            var fields = data == null ? string.Empty
                : string.Join(", ", data.Where(kv => kv.Value != null).Select(kv => $"{kv.Key}={kv.Value}"));
            _logger.LogInformation(
                "[WhatsApp:Logging] would send template '{Template}' to {Phone} | {Fields}",
                templateName, toPhone, fields);
            return Task.FromResult(true);
        }
    }
}
