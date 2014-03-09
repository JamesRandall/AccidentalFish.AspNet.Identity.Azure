using System;
using System.Configuration;
using AccidentalFish.AspNet.Identity.Azure;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using MVC5AuthenticationExample.Web.Models;
using Owin;

namespace MVC5AuthenticationExample.Web
{
    public partial class Startup
    {
        static readonly string ConnectionString = ConfigurationManager.AppSettings["my-connection-string"];
        static Func<UserManager<ApplicationUser>> _userManagerFactory;

        public static Func<UserManager<ApplicationUser>> UserManagerFactory
        {
            get
            {
                return _userManagerFactory ??
                    (_userManagerFactory = () => new UserManager<ApplicationUser>(new TableUserStore<ApplicationUser>(ConnectionString)));
            }
        }

        static Startup()
        {
            PublicClientId = "self";

            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new GenericApplicationOAuthProvider<ApplicationUser>(PublicClientId, UserManagerFactory),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = true
            };
        }

        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });

            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication();
        }
    }
}