using System;
using System.Security.Cryptography;
using System.Text;

namespace PermissionServer
{
    public class Token
    {
        public DateTime CreatedAt { get; }
        public DateTime ValidUntil { get; }
        public int LifetimeMinutes { get; }

        public string ConfirmationCode { get; }

        public bool IsExpired => ValidUntil < DateTime.UtcNow;

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
        public bool DoesNotMatch(string suggestedValidationCode)
            => Matches(suggestedValidationCode) == false;

        /// <summary>
        /// Returns true if the (case-insensitive) confirmation
        /// code matches.
        /// </summary>
        public bool Matches(string suggestedValidationCode)
        {
            return string.Equals(
                ConfirmationCode,
                suggestedValidationCode.Trim(),
                StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns true if the token hasn't expired.
        /// </summary>
        public bool IsActive(string suggestedValidationCode)
        {
            return IsExpired == false;
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
