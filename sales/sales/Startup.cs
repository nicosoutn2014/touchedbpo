using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(sales.Startup))]
namespace sales
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
