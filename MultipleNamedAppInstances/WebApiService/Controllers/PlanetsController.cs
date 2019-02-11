using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DataService.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace WebApiService.Controllers
{
    [Route("api/[controller]")]
    public class PlanetsController : Controller
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
            Uri backendServiceUri = WebApiService.GetDataServiceName(this.serviceContext);
            var allPartitions = await this.fabricClient.QueryManager.GetPartitionListAsync(backendServiceUri);

            var allPlanets = new List<string>();
            foreach (var currentPartition in allPartitions)
            {
                long minKey = (currentPartition.PartitionInformation as Int64RangePartitionInformation).LowKey;
                var dataServiceClient = ServiceProxy.Create<IDataService>(backendServiceUri, new ServicePartitionKey(minKey));
                var result = await dataServiceClient.GetPlanetsAsync(CancellationToken.None);
                if (result != null)
                {
                    allPlanets.AddRange(result);
                }
            }
            return allPlanets;
        }

        [HttpPost]
        public Task Post([FromBody] string name)
        {
            Uri backendServiceUri = WebApiService.GetDataServiceName(this.serviceContext);
            var backendServiceClient = ServiceProxy.Create<IDataService>(backendServiceUri, new ServicePartitionKey(GetPartitionId()));
            try
            {
                return backendServiceClient.AddPlanetAsync(name);
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.Message("Web Service: Exception creating {0}: {1}", name, ex);
                throw;
            }
        }

        internal int GetPartitionId()
        {
            //TODO: VSDebug
            //return 1;
            return new Random().Next(1, 4);
        }
    }
}
