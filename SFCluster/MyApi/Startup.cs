using System.Web.Http;
using Owin;
using System.Fabric;
using Common.Http;

namespace MyApi
{
    public static class Startup
    {
        public static void ConfigureApp(IAppBuilder appBuilder, ServiceContext context)
        {
            var config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();
            
            appBuilder.Use<ServiceTracingMiddleware>(context);
            
            appBuilder.UseWebApi(config);
        }
    }
}
