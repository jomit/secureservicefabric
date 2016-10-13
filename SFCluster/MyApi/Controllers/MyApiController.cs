using Common;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.FabricTransport.Common;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Client;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using MyApi.Filter;
using MyBackend.Domain;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Http;

namespace MyApi.Controllers
{
    [ServiceRequestActionFilter]
    [RoutePrefix("api/v1/myapi")]
    public class MyApiController : ApiController
    {
        private const string BackendServiceName = "MyBackend";
        private static FabricClient fabricClient = new FabricClient();

        [HttpGet]
        [Route("getdata")]
        public async Task<IEnumerable<string>> GetData()
        {
            var backendServiceClient = GetServiceClientWithTransportSettings();
            try
            {
                return await backendServiceClient.GetData(ServiceTracingContext.GetRequestCorrelationId());
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.Message("Web Service: Exception getting data : {0}", ex);
                throw;
            }
        }

        private IMyBackend GetServiceClient()
        {
            var builder = new ServiceUriBuilder(BackendServiceName);
            return ServiceProxy.Create<IMyBackend>(builder.ToUri());
        }

        private IMyBackend GetServiceClientWithTransportSettings()
        {
            var builder = new ServiceUriBuilder(BackendServiceName);

            //var x509Credentials = new X509Credentials
            //{
            //    FindType = X509FindType.FindByThumbprint,
            //    FindValue = "053a87f6c1e3d08ec7fc28522a2cf1921c9daa5e",
            //    StoreLocation = StoreLocation.LocalMachine,
            //    StoreName = "My",
            //    ProtectionLevel = ProtectionLevel.EncryptAndSign
            //};
            //x509Credentials.RemoteCommonNames.Add("jacksch.westus.cloudapp.azure.com");

            //var transportSettings = new FabricTransportSettings
            //{
            //    SecurityCredentials = x509Credentials,
            //    MaxMessageSize = 10000000
            //};

            var serviceProxyFactory = new ServiceProxyFactory((c) =>
                                      new FabricTransportServiceRemotingClientFactory());

            return serviceProxyFactory.CreateServiceProxy<IMyBackend>(builder.ToUri());
        }
    }
}
