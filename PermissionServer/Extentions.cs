using Microsoft.Extensions.DependencyInjection;

namespace PermissionServer
{
    public static class Extentions
    {
        /// <summary>
        /// Register and configure the Permission Server dependencies.
        /// </summary>
        public static void AddPermissionServer(
            this IServiceCollection services,
            PermissionServerOptions options)
        {
            var tokenStore = new InMemoryTokenStore(options.Tokens);
            var mailer = new Mailer(options.Emails);
            var permissionServer = new PermissionServer(options, tokenStore, mailer);
            services.AddSingleton(permissionServer);
        }
    }
}
