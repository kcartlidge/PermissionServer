using PermissionServer;
using System.Threading.Tasks;

namespace PermissionServer
{
    public class PermissionServer
    {
        private readonly PermissionServerOptions options;
        private readonly ITokenStore tokenStore;
        private readonly Mailer mailer;

        public PermissionServer(
            PermissionServerOptions options,
            ITokenStore tokenStore,
            Mailer mailer)
        {
            this.options = options;
            this.tokenStore = tokenStore;
            this.mailer = mailer;
        }

        /// <summary>Generates a new limited-lifetime token and emails it.</summary>
        public async Task StartConfirmation(string emailAddress, string confirmationUrl)
        {
            var token = tokenStore.Add(emailAddress);
            await mailer.SendConfirmation(emailAddress, token, confirmationUrl);
        }

        /// <summary>
        /// Confirms the token matches an issued one for the email
        /// address (and has not expired). If it doesn't then the
        /// return value states why.
        /// </summary>
        public ConfirmationStatus CompleteConfirmation(string emailAddress, string confirmationCode)
        {
            var token = tokenStore.Get(emailAddress);
            if (token == null) return ConfirmationStatus.NoTokenFound;
            if (token.IsExpired) return ConfirmationStatus.HasExpired;
            if (token.DoesNotMatch(confirmationCode)) return ConfirmationStatus.DoesNotMatchToken;

            if (options.Tokens.SingleUse) tokenStore.Remove(emailAddress);
            return ConfirmationStatus.Okay;
        }
    }
}
