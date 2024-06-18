namespace PermissionServer
{
    /// <summary>Defines the behaviour of Permission Server.</summary>
    public class PermissionServerOptions
    {
        public TokenOptions Tokens { get; set; } = new TokenOptions();
        public EmailOptions Emails { get; set; } = new EmailOptions();

        public class TokenOptions
        {
            public int Length { get; set; } = 8;
            public int LifetimeMinutes { get; set; } = 15;
            public bool SingleUse { get; set; } = true;
            public int MaximumActivePerKey { get; set; } = 1;
        }

        public class EmailOptions
        {
            public string AppName { get; set; } = "PermissionServer";
            public string Hostname { get; set; } = "";
            public int Port { get; set; } = 25;
            public bool StartTLS { get; set; } = false;
            public string Username { get; set; } = "";
            public string Password { get; set; } = "";
            public string Sender { get; set; } = "";

            public string Subject { get; set; } = "{AppName} email confirmation";
            public string Body { get; set; } =
                "To confirm this email please visit:  {URL}\n" +
                "Once there enter confirmation code:  {ConfirmationCode}\n\n" +
                "This code is valid for {LifetimeMinutes} minutes from when the email was issued.\n" +
                "Sent to {Recipient} and valid until {ValidUntil} (UTC/GMT). " +
                "If this was not requested you may safely ignore this email.";
        }
    }
}
