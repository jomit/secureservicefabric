using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using MyBackend.Domain;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;

namespace MyBackend
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class MyBackend : StatelessService, IMyBackend
    {
        public MyBackend(StatelessServiceContext context)
            : base(context)
        { }

        public async Task<IEnumerable<string>> GetData()
        {
            ServiceEventSource.Current.Message("Generating Data");

            var data = new List<string>() { "A", "B", "C" };

            return data;
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext => this.CreateServiceRemotingListener(serviceContext))
            };
        }
    }
}
