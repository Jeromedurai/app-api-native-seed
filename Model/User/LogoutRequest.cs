using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.User
{
    /// <summary>
    /// Model for user logout request
    /// </summary>
    public class LogoutRequest
    {
        /// <summary>
        /// User ID (required)
        /// </summary>
        [Required(ErrorMessage = "User ID is required")]
        public long UserId { get; set; }

        /// <summary>
        /// Device ID for device-specific logout (optional)
        /// </summary>
        [StringLength(255, ErrorMessage = "Device ID cannot exceed 255 characters")]
        public string DeviceId { get; set; }

        /// <summary>
        /// Whether to logout from all devices (default: false)
        /// </summary>
        public bool LogoutFromAllDevices { get; set; } = false;

        /// <summary>
        /// Client IP address (optional, for logging)
        /// </summary>
        [StringLength(45, ErrorMessage = "IP address cannot exceed 45 characters")]
        public string IpAddress { get; set; }

        /// <summary>
        /// User agent string (optional, for logging)
        /// </summary>
        [StringLength(500, ErrorMessage = "User agent cannot exceed 500 characters")]
        public string UserAgent { get; set; }
    }
}
