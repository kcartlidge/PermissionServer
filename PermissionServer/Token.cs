using System;
using System.Security.Cryptography;
using System.Text;

namespace PermissionServer
{
    internal class Token
    {
        public DateTime CreatedAt { get; }
        public DateTime ValidUntil { get; }
        public int LifetimeMinutes { get; }
        public string Context { get; set; }

        public string ConfirmationCode { get; }

        /// <summary>True if the token has expired.</summary>
        public bool IsExpired => ValidUntil < DateTime.UtcNow;

        /// <summary>True if the token hasn't expired.</summary>
        public bool IsActive => IsExpired == false;

        /// <summary>True if the token has a populated Context.</summary>
        /// <remarks>
        /// A context-specific token should only match if the same context
        /// is passed to comparisons (a null/empty Context should match
        /// regardless).
        /// </remarks>
        public bool HasContext => Context.HasValue();

        /// <summary>
        /// Creates a token with the specified length and lifetime.
        /// The token is a numeric string (using a cryptographic RNG).
        /// Regardless of the tokenLength, a minimum of 5 digits is
        /// enforced.
        /// </summary>
        public Token(int tokenLength, int lifetimeMinutes, string context)
        {
            CreatedAt = DateTime.UtcNow;
            ValidUntil = CreatedAt.AddMinutes(lifetimeMinutes);
            ConfirmationCode = GetTokenString(tokenLength);
            LifetimeMinutes = lifetimeMinutes;
            Context = context;
        }

        /// <summary>
        /// Returns true if the (case-insensitive) confirmation
        /// code does NOT match.
        /// </summary>
        public bool DoesNotMatch(string suggestedConfirmationCode)
            => Matches(suggestedConfirmationCode) == false;

        /// <summary>
        /// Returns true if the (case-insensitive) confirmation
        /// code matches. If the token is context-specific then
        /// the passed-in context is also checked.
        /// </summary>
        public bool Matches(string suggestedConfirmationCode, string context = "")
        {
            var contextOK = true;
            if (HasContext) contextOK = (context == Context);

            var codeOK = string.Equals(
                ConfirmationCode,
                suggestedConfirmationCode.Trim(),
                StringComparison.OrdinalIgnoreCase);

            return contextOK && codeOK;
        }

        private string GetTokenString(int tokenDigits)
        {
            var digits = new StringBuilder();
            for (int i = 0; i < tokenDigits; i++)
            {
                var d = RandomNumberGenerator.GetInt32(0, 10);
                digits.Append(d);
            }
            return digits.ToString();
        }
    }
}
