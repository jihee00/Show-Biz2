using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JHK2247A5.Startup))]

namespace JHK2247A5
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
