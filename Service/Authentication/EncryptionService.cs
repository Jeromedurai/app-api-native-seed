using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tenant.Query.Model.User;

namespace Tenant.Query.Service.Authentication
{
    public struct EncryptionResult
    {
        public string Ciphertext;
        public string Tag;
    }

    public class EncryptionService
    {
        private const int KEY_LENGTH = 32; // 256-bit key for AES-GCM (Web Crypto API compatible)
        private const int NONCE_LENGTH = 12; // 12-byte nonce for GCM
        private const int TAG_LENGTH = 16; // 16-byte authentication tag

        private readonly IConfiguration _configuration;
        private readonly ILogger<EncryptionService> _logger;
        private readonly byte[] _masterKey;

        public EncryptionService(IConfiguration configuration, ILogger<EncryptionService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            
            // Get master key from configuration
            var masterKeyBase64 = _configuration["Encryption:MasterKey"];
            if (string.IsNullOrEmpty(masterKeyBase64))
            {
                _logger.LogError("Encryption:MasterKey is not configured. Set it in User Secrets (Development) or environment variables (Production).");
                throw new InvalidOperationException(
                    "Encryption:MasterKey is required. " +
                    "For Development: Use 'dotnet user-secrets set \"Encryption:MasterKey\" \"your-key\"' " +
                    "For Production: Set Encryption__MasterKey environment variable."
                );
            }
            
            try
            {
                _masterKey = Convert.FromBase64String(masterKeyBase64);
            }
            catch (FormatException ex)
            {
                _logger.LogError($"Invalid Encryption:MasterKey format. Must be Base64 encoded 32-byte key. Error: {ex.Message}");
                throw new InvalidOperationException("Encryption:MasterKey must be a valid Base64 encoded 32-byte key.", ex);
            }
            
            if (_masterKey.Length != KEY_LENGTH)
            {
                throw new InvalidOperationException($"Master key must be {KEY_LENGTH} bytes (256 bits). Current length: {_masterKey.Length}");
            }
        }

        /// <summary>
        /// Generate encryption key and nonce for client-side encryption
        /// Returns the master key (for encryption/decryption) and a new nonce
        /// </summary>
        public (string KeyBase64, string NonceBase64) GenerateEncryptionKey()
        {
            // Generate a new nonce for each request (nonce must be unique)
            byte[] nonce = new byte[NONCE_LENGTH];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(nonce);
            }

            // Return the master key for encryption/decryption
            // Note: In production, consider using key derivation or encrypting the session key
            return (Convert.ToBase64String(_masterKey), Convert.ToBase64String(nonce));
        }

        /// <summary>
        /// Decrypt message using AES-GCM
        /// </summary>
        public string DecryptMessage(string cipherTextStr, string nonceStr, string tagStr)
        {
            try
            {
                byte[] nonce = Convert.FromBase64String(nonceStr);
                byte[] ciphertext = Convert.FromBase64String(cipherTextStr);
                byte[] tag = Convert.FromBase64String(tagStr);

                if (nonce.Length != NONCE_LENGTH)
                {
                    throw new ArgumentException($"Nonce must be {NONCE_LENGTH} bytes");
                }

                if (tag.Length != TAG_LENGTH)
                {
                    throw new ArgumentException($"Tag must be {TAG_LENGTH} bytes");
                }

                byte[] plaintext = new byte[ciphertext.Length];

                using (var aesGcm = new AesGcm(_masterKey))
                {
                    aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);
                }

                return Encoding.UTF8.GetString(plaintext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Decryption failed: {ex.Message}");
                throw new CryptographicException("Failed to decrypt message", ex);
            }
        }

        /// <summary>
        /// Decrypt login request payload
        /// </summary>
        public LoginRequest DecryptLoginRequest(string encryptedPayload, string nonce, string tag)
        {
            try
            {
                var decryptedJson = DecryptMessage(encryptedPayload, nonce, tag);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<LoginRequest>(decryptedJson, options);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to deserialize login request: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Decrypt registration request payload
        /// </summary>
        public RegisterRequest DecryptRegisterRequest(string encryptedPayload, string nonce, string tag)
        {
            try
            {
                var decryptedJson = DecryptMessage(encryptedPayload, nonce, tag);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<RegisterRequest>(decryptedJson, options);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to deserialize registration request: {ex.Message}");
                throw;
            }
        }
    }
}
