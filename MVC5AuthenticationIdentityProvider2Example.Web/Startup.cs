using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVC5AuthenticationIdentityProvider2Example.Web.Startup))]
namespace MVC5AuthenticationIdentityProvider2Example.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
