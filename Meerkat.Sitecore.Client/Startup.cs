using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Meerkat.Sitecore.Client.Startup))]
namespace Meerkat.Sitecore.Client
{

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}