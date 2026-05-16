using System;

namespace Tenant.Query.Model.Email
{
    /// <summary>
    /// Worker / server diagnostics surfaced to the admin UI banner.
    /// All times are UTC; the UI converts to the admin's browser TZ.
    /// </summary>
    public class NotificationServerInfoDto
    {
        public string TimeZoneId          { get; set; }
        public int    TimeZoneOffsetMinutes { get; set; }
        public DateTime ServerNowUtc      { get; set; }
        public string ServerNowLocal      { get; set; }
        public string WorkerServiceName   { get; set; }
        public DateTime? WorkerLastRunUtc { get; set; }
        public double PollingIntervalMinutes { get; set; }
    }
}
