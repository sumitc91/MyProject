using Microsoft.Owin;
using OrbitPageSignalR;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace OrbitPageSignalR
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            app.MapSignalR();
        }
    }
}
