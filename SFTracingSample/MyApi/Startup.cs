using System.Web.Http;
using Owin;
using Common.Http;
using System.Fabric;

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
