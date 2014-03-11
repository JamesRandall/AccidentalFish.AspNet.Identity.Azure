using Microsoft.Owin;
using MVC5AuthenticationExample.Web;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace MVC5AuthenticationExample.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
