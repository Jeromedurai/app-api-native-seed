using Microsoft.AspNetCore.Http;

namespace Tenant.Query.Model.User
{
    /// <summary>
    /// Multipart/form-data request for uploading a user's profile image.
    /// Request-only model — never returned to the client.
    /// </summary>
    public class ProfileImageUploadDto
    {
        public long UserId { get; set; }
        public long? TenantId { get; set; }
        public IFormFile File { get; set; }
    }

    /// <summary>
    /// Profile image bytes + content type read back from storage.
    /// </summary>
    public class ProfileImageData
    {
        public byte[] ImageData { get; set; }
        public string ContentType { get; set; }
    }
}
