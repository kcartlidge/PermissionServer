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

        public string ConfirmationCode { get; }

        /// <summary>Returns true if the token has expired.</summary>
        public bool IsExpired => ValidUntil < DateTime.UtcNow;

        /// <summary>Returns true if the token hasn't expired.</summary>
        public bool IsActive => IsExpired == false;

        /// <summary>
        /// Creates a token with the specified length and lifetime.
        /// The token is a numeric string (using a cryptographic RNG).
        /// Regardless of the tokenLength, a minimum of 5 digits is
        /// enforced.
        /// </summary>
        public Token(int tokenLength, int lifetimeMinutes)
        {
            CreatedAt = DateTime.UtcNow;
            ValidUntil = CreatedAt.AddMinutes(lifetimeMinutes);
            ConfirmationCode = GetTokenString(tokenLength);
            LifetimeMinutes = lifetimeMinutes;
        }

        /// <summary>
        /// Returns true if the (case-insensitive) confirmation
        /// code does NOT match.
        /// </summary>
        public bool DoesNotMatch(string suggestedConfirmationCode)
            => Matches(suggestedConfirmationCode) == false;

        /// <summary>
        /// Returns true if the (case-insensitive) confirmation
        /// code matches.
        /// </summary>
        public bool Matches(string suggestedConfirmationCode)
        {
            return string.Equals(
                ConfirmationCode,
                suggestedConfirmationCode.Trim(),
                StringComparison.OrdinalIgnoreCase);
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
