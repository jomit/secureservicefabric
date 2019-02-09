using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2, RemotingClientVersion = RemotingClientVersion.V2)]
namespace BackendService.Domain
{
    public interface IBackendService : IService
    {
        Task<List<string>> GetPlanetsAsync(CancellationToken cancelToken);

        Task AddPlanetAsync(string name);
    }
}
