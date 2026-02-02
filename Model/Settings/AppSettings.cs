using System.Collections.Generic;

namespace Tenant.Query.Model.Settings
{
    public class AppSettingItem
    {
        public string SettingKey { get; set; }
        public string SettingValue { get; set; }
        public long? TenantId { get; set; }
        public long? UserId { get; set; }
    }

    public class GetAppSettingsResponse
    {
        public List<AppSettingItem> Settings { get; set; } = new List<AppSettingItem>();
    }

    public class SaveAppSettingsRequest
    {
        public long? TenantId { get; set; }
        public long? UserId { get; set; }
        public List<AppSettingItem> Settings { get; set; } = new List<AppSettingItem>();
    }

    public class SaveAppSettingsResponse
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = "Settings saved successfully.";
    }
}
