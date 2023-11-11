using PermissionServer;
using System.Threading.Tasks;

namespace PermissionServer
{
    public class PermissionServer
    {
        private readonly PermissionServerOptions options;
        private readonly ITokenStore tokenStore;
        private readonly Mailer mailer;

        internal PermissionServer(
            PermissionServerOptions options,
            ITokenStore tokenStore,
            Mailer mailer)
        {
            this.options = options;
            this.tokenStore = tokenStore;
            this.mailer = mailer;
        }

        /// <summary>Generates a new limited-lifetime token and emails it.</summary>
        public async Task<bool> StartConfirmation(string emailAddress, string confirmationUrl)
        {
            var token = tokenStore.Add(emailAddress);
            if (token == null) return false;
            await mailer.SendConfirmation(emailAddress, token, confirmationUrl);
            return true;
        }

        /// <summary>
        /// Confirms the token matches an issued one for the email
        /// address (and has not expired). If it doesn't then the
        /// return value states why.
        /// </summary>
        public bool CompleteConfirmation(string emailAddress, string confirmationCode)
        {
            var token = tokenStore.Get(emailAddress, confirmationCode);
            if (token == null) return false;
            if (options.Tokens.SingleUse) tokenStore.Remove(emailAddress, confirmationCode);
            return true;
        }
    }
}
