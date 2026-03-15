using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tenant.API.Base.Service;
using Tenant.API.Base.Repository;
using Tenant.Query.Model.Contact;
using Tenant.Query.Repository.Contact;

namespace Tenant.Query.Service.Contact
{
    public class ContactService : TnBaseService
    {
        private readonly ContactRepository _contactRepository;

        public ContactService(
            ContactRepository contactRepository,
            ILoggerFactory loggerFactory,
            TnAudit xcAudit,
            TnValidation xcValidation) : base(xcAudit, xcValidation)
        {
            _contactRepository = contactRepository;
            _contactRepository.Logger = loggerFactory.CreateLogger<ContactRepository>();
            this.Logger = loggerFactory.CreateLogger<ContactService>();
        }

        /// <summary>
        /// Validate and persist a public contact-us message.
        /// </summary>
        public async Task<long> CreateContactMessage(ContactMessageRequest request)
        {
            try
            {
                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                // Basic server-side validation – attributes handle most cases
                if (string.IsNullOrWhiteSpace(request.Name))
                    throw new ArgumentException("Name is required");

                if (string.IsNullOrWhiteSpace(request.Email))
                    throw new ArgumentException("Email is required");

                if (string.IsNullOrWhiteSpace(request.Message))
                    throw new ArgumentException("Message is required");

                Logger.LogInformation("Service: Creating contact message from {Email}", request.Email);
                return await _contactRepository.CreateContactMessage(request);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Service: Error creating contact message");
                throw;
            }
        }
    }
}

