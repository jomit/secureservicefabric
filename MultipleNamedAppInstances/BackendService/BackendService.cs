using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using BackendService.Domain;

namespace BackendService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class BackendService : StatefulService, IBackendService
    {
        private const string PlanetsDictionaryName = "planets";
        public BackendService(StatefulServiceContext context)
            : base(context)
        { }

        public async Task AddPlanetAsync(string name)
        {
            var backendItems = await this.StateManager.GetOrAddAsync<IReliableDictionary2<string, string>>(PlanetsDictionaryName);
            using (var transaction = this.StateManager.CreateTransaction())
            {
                await backendItems.AddOrUpdateAsync(transaction, name.ToLower(), name, (key, oldValue) => name);
                await transaction.CommitAsync();
            }
        }

        public async Task<List<string>> GetPlanetsAsync(CancellationToken cancelToken)
        {
            var backendItems = await this.StateManager.GetOrAddAsync<IReliableDictionary2<string, string>>(PlanetsDictionaryName);
            var planets = new List<string>();
            planets.Add("Testing");
            using (var transaction = this.StateManager.CreateTransaction())
            {
                var planetEnumerator = (await backendItems.CreateEnumerableAsync(transaction)).GetAsyncEnumerator();
                while (await planetEnumerator.MoveNextAsync(cancelToken))
                {
                    planets.Add(planetEnumerator.Current.Value);
                }
            }
            return planets;
        }

        // https://aka.ms/servicefabricservicecommunication
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, "Inside RunAsync for Backend Service");
                if (cancellationToken.IsCancellationRequested)
                {
                    // TODO
                }
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, "RunAsync Failed, {0}", e);
                throw;
            }
        }
    }
}
