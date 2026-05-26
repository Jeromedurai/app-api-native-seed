using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.User
{
    public class RequestLoginOtpRequest
    {
        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "Phone number must be 10 digits")]
        public string Phone { get; set; }
    }
}
