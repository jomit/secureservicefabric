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
using Common;
using System.Runtime.Remoting.Messaging;
using Microsoft.ServiceFabric.Services.Communication.FabricTransport.Runtime;
using System.Security.Cryptography.X509Certificates;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;

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

        public async Task<IEnumerable<string>> GetData(string correlationId)
        {
            ServiceEventSource.Current.ServiceMessage(this, "Generating Data", correlationId);

            var data = new List<string>() { "A", "B", "C" };

            return data;
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            FabricTransportListenerSettings listenerSettings = new FabricTransportListenerSettings
            {
                MaxMessageSize = 10000000,
                SecurityCredentials = GetSecurityCredentials()
            };

            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener((context) => new FabricTransportServiceRemotingListener(context, this, listenerSettings))
            };
        }

        // Create Listener with 'NO' Transport Settings
        //protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        //{
        //    return new ServiceInstanceListener[]
        //    {
        //        new ServiceInstanceListener(serviceContext => this.CreateServiceRemotingListener(serviceContext))
        //    };
        //}

        private static SecurityCredentials GetSecurityCredentials()
        {
            var x509Credentials = new X509Credentials
            {
                FindType = X509FindType.FindByThumbprint,
                FindValue = "053a87f6c1e3d08ec7fc28522a2cf1921c9daa5e",
                StoreLocation = StoreLocation.LocalMachine,
                StoreName = "My",
                ProtectionLevel = ProtectionLevel.EncryptAndSign
            };
            x509Credentials.RemoteCommonNames.Add("jacksch.westus.cloudapp.azure.com");
            return x509Credentials;
        }
    }
}
