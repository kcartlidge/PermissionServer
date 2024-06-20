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
        /// <param name="context">Unique-enough identifier for the user context</param>
        /// <remarks>
        /// Including a context marks the token as context-specific for future
        /// comparisons.  Leave it off (or pass null/"") to allow the token to
        /// be matched in ANY context not just the one that issued it.
        /// </remarks>
        Token? Add(string key, string context = "");

        /// <summary>Finds an existing (unexpired) token.</summary>
        /// <param name="key">Email address or other unique identifier.</param>
        /// <param name="confirmationCode">The related confirmation code.</param>
        /// <param name="context">Unique(ish) identifier, eg IP address.</param>
        /// <remarks>
        /// If the token was created with a context then it must be confirmed from
        /// the same context and you must provide a matching value.
        /// Leave it off (or pass null/"") for tokens that can be matched from ANY
        /// context not just the one that issued it.
        /// </remarks>
        Token? Get(string key, string confirmationCode, string context = "");

        /// <summary>Remove any existing matching token.</summary>
        /// <remarks>This does not need to restrict by the context.</remarks>
        void Remove(string key, string confirmationCode);

        /// <summary>
        /// Removes all expired tokens (frees resources).
        /// This happens automatically whenever a new token is added.
        /// </summary>
        void Purge();
    }
}
