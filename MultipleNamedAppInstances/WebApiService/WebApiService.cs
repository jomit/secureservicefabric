using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Data;
using System.Net.Http;

namespace WebApiService
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class WebApiService : StatelessService
    {
        public WebApiService(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        // This won't work with Kestrel as we cannot share a port between multiple instances.
                        //string tenantName = new Uri(serviceContext.CodePackageActivationContext.ApplicationName).Segments.Last();
                        //url += $"/{tenantName}";

                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        return new WebHostBuilder()
                                    .UseKestrel()
                                    .ConfigureServices(
                                        services => services
                                            .AddSingleton<HttpClient>(new HttpClient())
                                            .AddSingleton<FabricClient>(new FabricClient())
                                            .AddSingleton<StatelessServiceContext>(serviceContext))
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseStartup<Startup>()
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url)
                                    .Build();
                    }))
            };
        }
        internal static Uri GetDataServiceName(ServiceContext context)
        {
            return new Uri($"{context.CodePackageActivationContext.ApplicationName}/DataService");
        }
    }
}
