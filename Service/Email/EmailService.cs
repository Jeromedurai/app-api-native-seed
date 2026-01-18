using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Tenant.Query.Model.Email;
using Tenant.Query.Model.User;

namespace Tenant.Query.Service.Email
{
    /// <summary>
    /// Service for sending emails
    /// </summary>
    public class EmailService
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILogger<EmailService> logger)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Renders a Razor view to HTML string
        /// </summary>
        private async Task<string> RenderViewToStringAsync(string viewName, object model)
        {
            try
            {
                var actionContext = GetActionContext();
                var view = FindView(actionContext, viewName);

                using (var output = new StringWriter())
                {
                    var viewContext = new ViewContext(
                        actionContext,
                        view,
                        new ViewDataDictionary(
                            metadataProvider: new EmptyModelMetadataProvider(),
                            modelState: new ModelStateDictionary())
                        {
                            Model = model
                        },
                        new TempDataDictionary(
                            actionContext.HttpContext,
                            _tempDataProvider),
                        output,
                        new HtmlHelperOptions());

                    await view.RenderAsync(viewContext);
                    return output.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error rendering view {viewName}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets ActionContext for view rendering
        /// </summary>
        private ActionContext GetActionContext()
        {
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }

        /// <summary>
        /// Finds a Razor view by name
        /// </summary>  
        private IView FindView(ActionContext actionContext, string viewName)
        {
            var getViewResult = _razorViewEngine.GetView(null, $"~/Views/Content/{viewName}.cshtml", false);

            if (getViewResult.Success)
            {
                return getViewResult.View;
            }

            var searchedLocations = string.Join(", ", getViewResult.SearchedLocations);
            throw new Exception($"View '{viewName}' not found. Searched locations: {searchedLocations}");
        }

        /// <summary>
        /// Generic method to send emails using templates
        /// </summary>
        /// <param name="request">Email request with template name, recipient, subject, and template data</param>
        /// <returns>Email response indicating success or failure</returns>
        public async Task<SendEmailResponse> SendEmail(SendEmailRequest request)
        {
            try
            {
                if (request == null)
                {
                    _logger.LogWarning("Email request is null");
                    return new SendEmailResponse
                    {
                        Success = false,
                        Message = "Email request is null",
                        To = string.Empty,
                        Subject = string.Empty
                    };
                }

                if (string.IsNullOrWhiteSpace(request.To))
                {
                    _logger.LogWarning("Recipient email address is missing");
                    return new SendEmailResponse
                    {
                        Success = false,
                        Message = "Recipient email address (To) is required",
                        To = request.To ?? string.Empty,
                        Subject = request.Subject ?? string.Empty
                    };
                }

                if (string.IsNullOrWhiteSpace(request.TemplateName))
                {
                    _logger.LogWarning("Email template name is missing");
                    return new SendEmailResponse
                    {
                        Success = false,
                        Message = "Email template name is required",
                        To = request.To,
                        Subject = request.Subject ?? string.Empty
                    };
                }

                if (string.IsNullOrWhiteSpace(request.Subject))
                {
                    _logger.LogWarning("Email subject is missing");
                    return new SendEmailResponse
                    {
                        Success = false,
                        Message = "Email subject is required",
                        To = request.To,
                        Subject = string.Empty
                    };
                }

                // Convert template data dictionary to dynamic object for Razor view
                object templateModel = request.TemplateData;
                if (request.TemplateData != null && request.TemplateData.Count > 0)
                {
                    // Create a dynamic object from dictionary
                    var dynamicModel = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;
                    foreach (var kvp in request.TemplateData)
                    {
                        dynamicModel[kvp.Key] = kvp.Value;
                    }
                    templateModel = dynamicModel;
                }

                // Render the email template
                var emailBody = await RenderViewToStringAsync(request.TemplateName, templateModel);

                // Get email configuration
                var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var smtpUsername = _configuration["Email:SmtpUsername"];
                var smtpPassword = _configuration["Email:SmtpPassword"];
                var fromEmail = request.FromEmail ?? _configuration["Email:FromEmail"] ?? smtpUsername;
                var fromName = request.FromName ?? _configuration["Email:FromName"] ?? "xtraCHEF";

                if (string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
                {
                    _logger.LogWarning("Email configuration is missing. Email will not be sent.");
                    return new SendEmailResponse
                    {
                        Success = false,
                        Message = "Email configuration is missing. Check Email:SmtpUsername and Email:SmtpPassword in configuration.",
                        To = request.To,
                        Subject = request.Subject
                    };
                }

                // Send email using System.Net.Mail.SmtpClient (built-in .NET)
                try
                {
                    using (var smtpClient = new SmtpClient(smtpHost, smtpPort))
                    {
                        smtpClient.EnableSsl = true;
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtpClient.Timeout = 30000; // 30 seconds timeout

                        using (var mailMessage = new MailMessage())
                        {
                            mailMessage.From = new MailAddress(fromEmail, fromName);
                            mailMessage.To.Add(new MailAddress(request.To));
                            
                            // Add CC recipients if provided
                            if (request.Cc != null && request.Cc.Any())
                            {
                                foreach (var cc in request.Cc.Where(c => !string.IsNullOrWhiteSpace(c)))
                                {
                                    mailMessage.CC.Add(new MailAddress(cc));
                                }
                            }

                            // Add BCC recipients if provided
                            if (request.Bcc != null && request.Bcc.Any())
                            {
                                foreach (var bcc in request.Bcc.Where(b => !string.IsNullOrWhiteSpace(b)))
                                {
                                    mailMessage.Bcc.Add(new MailAddress(bcc));
                                }
                            }

                            // Set reply-to if provided
                            if (!string.IsNullOrWhiteSpace(request.ReplyTo))
                            {
                                mailMessage.ReplyToList.Add(new MailAddress(request.ReplyTo));
                            }

                            mailMessage.Subject = request.Subject;
                            mailMessage.Body = emailBody;
                            mailMessage.IsBodyHtml = true;
                            mailMessage.Priority = MailPriority.Normal;

                            await smtpClient.SendMailAsync(mailMessage);

                            _logger.LogInformation($"Email sent successfully to {request.To} with subject: {request.Subject} using template: {request.TemplateName}");
                            return new SendEmailResponse
                            {
                                Success = true,
                                Message = "Email sent successfully",
                                To = request.To,
                                Subject = request.Subject
                            };
                        }
                    }
                }
                catch (SmtpException smtpEx)
                {
                    _logger.LogError($"SMTP error sending email to {request.To}: {smtpEx.Message}");
                    return new SendEmailResponse
                    {
                        Success = false,
                        Message = $"SMTP error: {smtpEx.Message}",
                        To = request.To,
                        Subject = request.Subject
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error sending email to {request.To}: {ex.Message}");
                    _logger.LogError($"Stack trace: {ex.StackTrace}");
                    return new SendEmailResponse
                    {
                        Success = false,
                        Message = $"Error sending email: {ex.Message}",
                        To = request.To,
                        Subject = request.Subject
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in SendEmail: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                return new SendEmailResponse
                {
                    Success = false,
                    Message = $"Error processing email request: {ex.Message}",
                    To = request?.To ?? string.Empty,
                    Subject = request?.Subject ?? string.Empty
                };
            }
        }

        /// <summary>
        /// Sends password reset OTP email (convenience method)
        /// </summary>
        public async Task<bool> SendPasswordResetOtpEmail(ForgotPasswordResponse response)
        {
            try
            {
                if (response == null || string.IsNullOrEmpty(response.Email))
                {
                    _logger.LogWarning("Cannot send email: response or email is null/empty");
                    return false;
                }

                var emailRequest = new SendEmailRequest
                {
                    To = response.Email,
                    Subject = "Password Reset OTP - xtraCHEF",
                    TemplateName = "PasswordResetOTP",
                    TemplateData = new Dictionary<string, object>
                    {
                        { "Email", response.Email },
                        { "OTP", response.OTP },
                        { "FirstName", response.FirstName ?? "User" },
                        { "ExpiresInSeconds", response.ExpiresInSeconds }
                    }
                };

                var emailResponse = await SendEmail(emailRequest);

                if (!emailResponse.Success && !string.IsNullOrEmpty(response.OTP))
                {
                    _logger.LogInformation($"OTP for {response.Email}: {response.OTP} (Email sending failed - check email configuration)");
                }

                return emailResponse.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in SendPasswordResetOtpEmail: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }
    }
}
