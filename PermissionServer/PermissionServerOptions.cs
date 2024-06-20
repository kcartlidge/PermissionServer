namespace PermissionServer
{
    /// <summary>Defines the behaviour of Permission Server.</summary>
    public class PermissionServerOptions
    {
        public TokenOptions Tokens { get; set; } = new TokenOptions();
        public EmailOptions Emails { get; set; } = new EmailOptions();

        public class TokenOptions
        {
            /// <summary>Confirmation tokens will be this length.</summary>
            /// <remarks>The default is 8.</remarks>
            public int Length { get; set; } = 8;

            /// <summary>Confirmation tokens will remain valid for this duration.</summary>
            /// <remarks>The default is 15.</remarks>
            public int LifetimeMinutes { get; set; } = 15;

            /// <summary>Can a (valid) token be used more than once?</summary>
            /// <remarks>The default is true.</remarks>
            public bool SingleUse { get; set; } = true;

            /// <summary>
            /// How many confirmation tokens can a user request?
            /// Expired and used tokens do not count towards this total.
            /// </summary>
            /// <remarks>The default is 1.</remarks>
            public int MaximumActivePerKey { get; set; } = 1;
        }

        public class EmailOptions
        {
            /// <summary>The applicatio name for text replacements when sending emails.</summary>
            /// <remarks>The default is "PermissionServer".</remarks>
            public string AppName { get; set; } = "PermissionServer";

            /// <summary>The SMTP hostname used when sending emails.</summary>
            public string Hostname { get; set; } = "";

            /// <summary>The SMTP port used when sending emails.</summary>
            /// <remarks>The default is 25.</remarks>
            public int Port { get; set; } = 25;

            /// <summary>The SMTP TLS option used when sending emails.</summary>
            /// <remarks>The default is false.</remarks>
            public bool StartTLS { get; set; } = false;

            /// <summary>The SMTP username used when sending emails.</summary>
            public string Username { get; set; } = "";

            /// <summary>The SMTP password used when sending emails.</summary>
            public string Password { get; set; } = "";

            /// <summary>The sender email address used when sending emails.</summary>
            /// <remarks>
            /// This can be a simple email address or take the form "Name <email@example.com>".
            /// </remarks>
            public string Sender { get; set; } = "";

            /// <summary>The subject used when sending emails.</summary>
            /// <remarks>The default is "{AppName} email confirmation".</remarks>
            public string Subject { get; set; } = "{AppName} email confirmation";

            /// <summary>The content used when sending emails.</summary>
            /// <remarks>
            /// This may contain text replacement parameters for "{URL}", "{ConfirmationCode}",
            /// "{LifetimeMinutes}", "{Recipient}", and/or "{ValidUntil}".
            /// The default is a generic email confirmation that uses all these replacements.
            /// </remarks>
            public string Body { get; set; } =
                "To confirm this email please visit:  {URL}\n" +
                "Once there enter confirmation code:  {ConfirmationCode}\n\n" +
                "This code is valid for {LifetimeMinutes} minutes from when the email was issued.\n" +
                "Sent to {Recipient} and valid until {ValidUntil} (UTC/GMT). " +
                "If this was not requested you may safely ignore this email.";
        }
    }
}
