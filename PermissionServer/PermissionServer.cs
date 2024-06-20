using System;
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

        /// <summary>
        /// Generates a new limited-lifetime token and emails it.
        /// A return value of false can mean EITHER the failure to create and
        /// store a token OR the failure to send it. For security reasons the
        /// specifics are NOT returned as the end user should never be told.
        /// </summary>
        /// <param name="context">
        /// Passing in a context will tie the generated token to the value
        /// provided, effectively meaning it will only be matched if confirmed
        /// from the same context.  Leave it off to allow tokens to be confirmed
        /// in any context.
        /// 
        /// You could use an IP address for a web site. Another option is a
        /// browser 'fingerprint', or a GUID from a secure cookie works well and
        /// restricts to the same session not just context.
        /// </param>
        public async Task<bool> StartConfirmation(
            string emailAddress,
            string confirmationUrl,
            string context = "")
        {
            var token = tokenStore.Add(emailAddress, context);
            if (token == null) return false;
            (var ok, var error) = await mailer.SendConfirmation(emailAddress, token, confirmationUrl);
            if (!ok)
            {
                Console.WriteLine($"Error sending email to {emailAddress} => {error}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Confirms the token matches an issued one for the email
        /// address (and has not expired). If it doesn't then the
        /// return value states why.
        /// </summary>
        /// <param name="context">
        /// If you generated the token including a context then
        /// it will only match if the same context is also passed
        /// in here, effectively restricting token confirmations to
        /// the same context that issued them (for extra security).
        /// If no context was used during token generation, leave
        /// it off here.
        /// </param>
        public bool CompleteConfirmation(
            string emailAddress,
            string confirmationCode,
            string context = "")
        {
            var token = tokenStore.Get(emailAddress, confirmationCode, context);
            if (token == null) return false;
            if (options.Tokens.SingleUse) tokenStore.Remove(emailAddress, confirmationCode);
            return true;
        }
    }
}
