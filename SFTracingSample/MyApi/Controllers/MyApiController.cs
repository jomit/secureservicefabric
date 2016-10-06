using Common;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using MyBackEnd.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace MyApi.Controllers
{
    [ServiceRequestActionFilter]
    [RoutePrefix("api/v1/myapi")]
    public class MyApiController : ApiController
    {
        private const string BackendServiceName = "MyBackEnd";

        [HttpGet]
        [Route("getdata")]
        public async Task<IEnumerable<string>> GetData()
        {
            var builder = new ServiceUriBuilder(BackendServiceName);

            var backendServiceClient = ServiceProxy.Create<IMyBackend>(builder.ToUri(), new ServicePartitionKey(1));
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
    }
}
