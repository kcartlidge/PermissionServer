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
        }
    }
}
