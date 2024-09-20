using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVC_Project.Startup))]
namespace MVC_Project
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
