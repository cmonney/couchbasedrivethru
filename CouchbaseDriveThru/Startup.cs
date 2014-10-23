using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CouchbaseDriveThru.Startup))]
namespace CouchbaseDriveThru
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
