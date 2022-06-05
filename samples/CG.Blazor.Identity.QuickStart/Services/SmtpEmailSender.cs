using System.Net;
using System.Net.Mail;

namespace CG.Blazor.Identity.QuickStart.Services
{
    /// <summary>
    /// This class is an SMTP implementation of the <see cref="IEmailSender"/>
    /// interface.
    /// </summary>
    public class SmtpEmailSender : IEmailSender
    {
        // *******************************************************************
        // Fields.
        // *******************************************************************

        #region Fields

        /// <summary>
        /// This field contains the logger for this email sender.
        /// </summary>
        protected readonly ILogger<SmtpEmailSender> _logger;

        /// <summary>
        /// This field contains the configuration for this email sender.
        /// </summary>
        protected readonly IConfiguration _configuration;

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="SmtpEmailSender"/>
        /// class.
        /// </summary>
        /// <param name="_configuration">The configuration to use with this 
        /// email sender.</param>
        /// <param name="logger">The logger to use with this emial sender.</param>
        public SmtpEmailSender(
            IConfiguration configuration,
            ILogger<SmtpEmailSender> logger
            )
        {
            // Save the reference(s).
            _configuration = configuration;
            _logger = logger;
        }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method sends email messages.
        /// </summary>
        /// <param name="email">The email address to use for the operation.</param>
        /// <param name="subject">The subject to use for the operation.</param>
        /// <param name="htmlMessage">The email message to use for the operation.</param>
        /// <returns>A task to perform the operation.</returns>
        public Task SendEmailAsync(
            string email,
            string subject,
            string htmlMessage
            )
        {
            try
            {
                using var message = new MailMessage(
                    _configuration["Email:From"],
                    email
                    );

                message.Subject = subject;
                message.Body = htmlMessage;
                message.IsBodyHtml = true;

                using SmtpClient client = new SmtpClient(_configuration["Email:Server"]);

                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(
                    _configuration["Email:Credentials:UserName"],
                    _configuration["Email:Credentials:Password"]
                    );

                client.Send(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "failed to send an email.");
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
