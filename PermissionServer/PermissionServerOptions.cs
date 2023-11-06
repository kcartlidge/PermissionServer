namespace PermissionServer
{
    /// <summary>Defines the behaviour of Permission Server.</summary>
    public class PermissionServerOptions
    {
        public TokenOptions Tokens { get; set; } = new TokenOptions();

        public class TokenOptions
        {
            public int Length { get; set; } = 8;
            public int LifetimeMinutes { get; set; } = 15;
            public bool SingleUse { get; set; } = true;
        }
    }
}
