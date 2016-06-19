using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SteamApp.Startup))]
namespace SteamApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
