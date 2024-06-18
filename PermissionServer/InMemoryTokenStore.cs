using System.Collections.Generic;
using System.Linq;
using static PermissionServer.PermissionServerOptions;

namespace PermissionServer
{
    /// <inheritdoc/>
    internal class InMemoryTokenStore : ITokenStore
    {
        private readonly TokenOptions opts;
        private readonly List<(string Key, Token Token)> keyedTokens;

        /// <summary>
        /// Creates a new token store.
        /// Keys are trimmed and are case-insensitive.
        /// 
        /// The store is in-memory; app restarts or new releases will clear
        /// down (and hence invalidate) all tokens.
        /// The only impact is tokens which are in-flight (such as an email
        /// sent but whose verification link has not yet been clicked).
        /// </summary>
        public InMemoryTokenStore(TokenOptions tokenOptions)
        {
            keyedTokens = new List<(string Key, Token Token)> ();
            opts = tokenOptions;
        }

        /// <inheritdoc/>
        public Token? Add(string key)
        {
            Purge();
            key = NormalisedKey(key);
            lock (keyedTokens)
            {
                var existing = keyedTokens.Count(x => x.Key == key && x.Token.IsActive);
                if (existing >= opts.MaximumActivePerKey) return null;

                var token = new Token(opts.Length, opts.LifetimeMinutes);
                keyedTokens.Add((key, token));
                return token;
            }
        }

        /// <inheritdoc/>
        public Token? Get(string key, string confirmationCode)
        {
            key = NormalisedKey(key);
            lock (keyedTokens)
            {
                foreach (var token in keyedTokens.Where(x => x.Key == key))
                {
                    if (token.Token.IsExpired) continue;
                    if (token.Token.Matches(confirmationCode)) return token.Token;
                }
                return null;
            }
        }

        /// <inheritdoc/>
        public void Remove(string key, string confirmationCode)
        {
            key = NormalisedKey(key);
            lock (keyedTokens)
            {
                for (var i = keyedTokens.Count - 1; i >= 0; i--)
                {
                    if (keyedTokens[i].Key != key) continue;
                    if (keyedTokens[i].Token.Matches(confirmationCode))
                        keyedTokens.RemoveAt(i);
                }
            }
        }

        /// <inheritdoc/>
        public void Purge()
        {
            lock (keyedTokens)
            {
                for (var i = keyedTokens.Count - 1; i >= 0; i--)
                    if (keyedTokens[i].Token.IsExpired)
                        keyedTokens.RemoveAt(i);
            }
        }

        private string NormalisedKey(string key) => key.Trim().ToLower();
    }
}
