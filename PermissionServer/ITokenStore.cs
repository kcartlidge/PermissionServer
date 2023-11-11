using System.Collections.Generic;

namespace PermissionServer
{
    /// <summary>
    /// Stores tokens (with lifetimes). These are accessed by `key`, where
    /// the key is a unique identifier like the email address.
    ///
    /// If you want multi-tenant then prefix the key with a tenant id, for
    /// example "APP1|email@example.com" or "SITE2|email@example.com".
    /// </summary>
    internal interface ITokenStore
    {
        /// <summary>
        /// Adds a new time-limited token.
        /// If the `MaximumActivePerKey` is reached no more tokens are issued
        /// and the return value is null. The user will need to wait for the
        /// oldest active key to expire before another will be allowed.
        /// </summary>
        /// <param name="key">Email address or other unique identifier.</param>
        Token? Add(string key);

        /// <summary>Finds an existing (unexpired) token.</summary>
        /// <param name="key">Email address or other unique identifier.</param>
        /// <param name="confirmationCode">The related confirmation code.</param>
        Token? Get(string key, string confirmationCode);

        /// <summary>Remove any existing matching token.</summary>
        void Remove(string key, string confirmationCode);

        /// <summary>
        /// Removes all expired tokens (frees resources).
        /// This happens automatically whenever a new token is added.
        /// </summary>
        void Purge();
    }
}
