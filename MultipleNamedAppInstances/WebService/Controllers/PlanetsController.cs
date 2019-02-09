using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BackendService.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace WebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanetsController : ControllerBase
    {
        private readonly HttpClient httpClient;
        private readonly FabricClient fabricClient;
        private readonly StatelessServiceContext serviceContext;

        public PlanetsController(HttpClient httpClient, StatelessServiceContext context, FabricClient fabricClient)
        {
            this.fabricClient = fabricClient;
            this.httpClient = httpClient;
            this.serviceContext = context;
        }

        [HttpGet]
        public async Task<List<string>> Get()
        {
            Uri backendServiceUri = WebService.GetBackendServiceName(this.serviceContext);
            var allPartitions = await this.fabricClient.QueryManager.GetPartitionListAsync(backendServiceUri);

            var allPlanets = new List<string>();
            foreach (var currentPartition in allPartitions)
            {
                long minKey = (currentPartition.PartitionInformation as Int64RangePartitionInformation).LowKey;
                var backendServiceClient = ServiceProxy.Create<IBackendService>(backendServiceUri, new ServicePartitionKey(minKey));
                var result = await backendServiceClient.GetPlanetsAsync(CancellationToken.None);
                if (result != null)
                {
                    allPlanets.AddRange(result);
                }
            }
            return allPlanets;
        }

        //[HttpPost]
        //[Route("create")]
        //public Task AddPlanet([FromBody] string name)
        //{
        //    Uri backendServiceUri = WebService.GetBackendServiceName(this.serviceContext);
        //    var backendServiceClient = ServiceProxy.Create<IBackendService>(backendServiceUri, new ServicePartitionKey(new Random().Next(1,4)));
        //    try
        //    {
        //        return backendServiceClient.AddPlanetAsync(name);
        //    }
        //    catch (Exception ex)
        //    {
        //        ServiceEventSource.Current.Message("Web Service: Exception creating {0}: {1}", name, ex);
        //        throw;
        //    }
        //}
    }
}
