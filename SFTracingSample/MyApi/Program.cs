using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.Diagnostics.EventListeners.Fabric;
using Microsoft.Diagnostics.EventListeners;

namespace MyApi
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                const string ApplicationInsightsEventListenerId = "ApplicationInsightsEventListener";
                var configProvider = new FabricConfigurationProvider(ApplicationInsightsEventListenerId);
                ApplicationInsightsEventListener aiListener = null;

                if (configProvider.HasConfiguration)
                {
                    aiListener = new ApplicationInsightsEventListener(configProvider, new FabricHealthReporter(ApplicationInsightsEventListenerId));
                }

                ServiceRuntime.RegisterServiceAsync("MyApiType",
                    context => new MyApi(context)).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(MyApi).Name);

                // Prevents this host process from terminating so services keeps running. 
                Thread.Sleep(Timeout.Infinite);
                GC.KeepAlive(aiListener);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
