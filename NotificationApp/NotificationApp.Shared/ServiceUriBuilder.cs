using System;
using System.Fabric;

namespace NotificationApp.Shared
{
    public class ServiceUriBuilder
    {
        public ServiceUriBuilder(string serviceInstance)
        {
            this.ServiceInstance = serviceInstance;
        }

        public ServiceUriBuilder(string applicationInstance, string serviceInstance)
        {
            this.ApplicationInstance = applicationInstance;
            this.ServiceInstance = serviceInstance;
        }

        public string ApplicationInstance { get; set; }

        public string ServiceInstance { get; set; }

        public Uri ToUri()
        {
            string applicationInstance = this.ApplicationInstance;

            if (String.IsNullOrEmpty(applicationInstance))
            {
                try
                {
                    // the ApplicationName property here automatically prepends "fabric:/" for us
                    applicationInstance = FabricRuntime.GetActivationContext().ApplicationName.Replace("fabric:/", String.Empty);
                }
                catch (InvalidOperationException)
                {

                }
            }

            return new Uri("fabric:/" + applicationInstance + "/" + this.ServiceInstance);
        }
    }
}
