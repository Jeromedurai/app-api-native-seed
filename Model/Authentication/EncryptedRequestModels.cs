using System.ComponentModel.DataAnnotations;

namespace Tenant.Query.Model.Authentication
{
    /// <summary>
    /// Encrypted login request model
    /// </summary>
    public class EncryptedLoginRequest
    {
        [Required(ErrorMessage = "Ciphertext is required")]
        public string Ciphertext { get; set; }

        [Required(ErrorMessage = "Nonce is required")]
        public string Nonce { get; set; }

        [Required(ErrorMessage = "Tag is required")]
        public string Tag { get; set; }
    }

    /// <summary>
    /// Encrypted registration request model
    /// </summary>
    public class EncryptedRegisterRequest
    {
        [Required(ErrorMessage = "Ciphertext is required")]
        public string Ciphertext { get; set; }

        [Required(ErrorMessage = "Nonce is required")]
        public string Nonce { get; set; }

        [Required(ErrorMessage = "Tag is required")]
        public string Tag { get; set; }
    }

    /// <summary>
    /// Encryption key response model
    /// </summary>
    public class EncryptionKeyResponse
    {
        public string Key { get; set; }
        public string Nonce { get; set; }
        public int ExpiresIn { get; set; }
    }
}
