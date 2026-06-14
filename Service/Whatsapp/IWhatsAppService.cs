using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tenant.Query.Service.Whatsapp
{
    /// <summary>
    /// Sends WhatsApp Business messages via a pre-approved template.
    /// Mirrors <see cref="Tenant.Query.Service.Sms.ISmsService"/>.
    /// Provider chosen by config "WhatsApp:Provider" (default "Logging").
    /// </summary>
    public interface IWhatsAppService
    {
        /// <summary>
        /// Send an approved WhatsApp template to a recipient.
        /// </summary>
        /// <param name="toPhone">Recipient phone (any format; normalized by the provider).</param>
        /// <param name="templateName">Meta-approved WhatsApp template name (SA_EMAILTEMPLATE.WHATSAPPTEMPLATE).</param>
        /// <param name="data">Merge fields (Name, CouponCode, …) mapped to template variables.</param>
        /// <returns>true if accepted by the provider.</returns>
        Task<bool> SendTemplateAsync(string toPhone, string templateName, IDictionary<string, object> data);
    }
}
