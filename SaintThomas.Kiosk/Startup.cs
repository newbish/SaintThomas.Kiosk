using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SaintThomas.Kiosk.Startup))]
namespace SaintThomas.Kiosk
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
