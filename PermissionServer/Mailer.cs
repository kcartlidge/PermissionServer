using System.Net.Mail;
using System.Net;
using System;
using System.Threading.Tasks;
using static PermissionServer.PermissionServerOptions;

namespace PermissionServer
{
    /// <summary>Handles sending standard emails.</summary>
    internal class Mailer
    {
        private readonly EmailOptions opts;

        public Mailer(EmailOptions emailOptions)
        {
            opts = emailOptions;
        }

        /// <summary>Send a standard token verification email.</summary>
        /// <remarks>
        /// This is not marked as async due to the underlying SmtpClient which
        /// does an async send but is not awaitable (fire and forget).
        /// </remarks>
        public Task<(bool OK, string ErrorMessage)> SendConfirmation(
            string recipient,
            Token token,
            string url)
        {
            return SendEmail(
                recipient,
                DoReplacements(opts.Subject, recipient, token, url),
                DoReplacements(opts.Body, recipient, token, url)
            );
        }

        private string DoReplacements(string text, string recipient, Token token, string url)
        {
            return text
                .Replace("{AppName}", opts.AppName)
                .Replace("{Recipient}", recipient)
                .Replace("{ConfirmationCode}", token.ConfirmationCode)
                .Replace("{LifetimeMinutes}", token.LifetimeMinutes.ToString())
                .Replace("{ValidUntil}", token.ValidUntil.ToString("f"))
                .Replace("{URL}", url)
                .Trim();
        }

        private async Task<(bool OK, string ErrorMessage)> SendEmail(
            string recipient,
            string subject,
            string body)
        {
            try
            {
                // Configure the server connection.
                var smtp = new SmtpClient(opts.Hostname, opts.Port);
                var fromEmail = opts.Sender;
                smtp.Credentials = new NetworkCredential(opts.Username, opts.Password);
                smtp.EnableSsl = true;

                // Configure the message.
                var message = new MailMessage(opts.Sender, recipient);
                message.Subject = subject.Trim();
                message.Body = body.Trim();
                message.IsBodyHtml = false;

                // Send.
                await smtp.SendMailAsync(message);
                return (true, "");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
