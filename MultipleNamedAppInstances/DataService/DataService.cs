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
using DataService.Domain;

namespace DataService
{
    internal sealed class DataService : StatefulService, IDataService
    {
        private const string PlanetsDictionaryName = "planets";
        public DataService(StatefulServiceContext context)
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
            //planets.Add("Earth");
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
            //return new ServiceReplicaListener[0];
            //return this.CreateServiceRemotingReplicaListeners();
            //return new[] {
            //    new ServiceReplicaListener(context => this.CreateServiceRemotingReplicaListeners(context))
            //};
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
