using System.Collections.Generic;
using System.Linq;
using static PermissionServer.PermissionServerOptions;

namespace PermissionServer
{
    /// <inheritdoc/>
    internal class InMemoryTokenStore : ITokenStore
    {
        private readonly TokenOptions opts;
        private readonly Dictionary<string, Token> tokens;

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
            tokens = new();
            opts = tokenOptions;
        }

        /// <inheritdoc/>
        public Token Add(string key)
        {
            Purge();
            key = NormalisedKey(key);
            lock (tokens)
            {
                Remove(key);
                var token = new Token(opts.Length, opts.LifetimeMinutes);
                tokens.Add(key, token);
                return token;
            }
        }

        /// <inheritdoc/>
        public Token? Get(string key)
        {
            key = NormalisedKey(key);
            lock (tokens)
            {
                if (tokens.ContainsKey(key) == false) return null;
                var token = tokens[key];
                if (token.IsExpired)
                {
                    Remove(key);
                    return null;
                }
                return token;
            }
        }

        /// <inheritdoc/>
        public void Remove(string key)
        {
            key = NormalisedKey(key);
            lock (tokens)
            {
                if (tokens.ContainsKey(key) == false) return;
                tokens.Remove(key);
            }
        }

        /// <inheritdoc/>
        public void Purge()
        {
            lock (tokens)
            {
                foreach (var token in tokens.Where(x => x.Value.IsExpired).ToList())
                    tokens.Remove(token.Key);
            }
        }

        private string NormalisedKey(string key) => key.Trim().ToLower();
    }
}
