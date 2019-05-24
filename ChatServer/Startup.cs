using Owin;
using Microsoft.Owin;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartup(typeof(SignalRChat.Startup))]
namespace SignalRChat
{

    public class MyConnectionFactory : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            return request.Cookies["srconnectionid"].Value;
        }
    }
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var idProvider = new MyConnectionFactory();
            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => idProvider);
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
        }
    }
}