using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DataService.Domain
{
    public interface IDataService : IService
    {
        Task<List<string>> GetPlanetsAsync(CancellationToken cancelToken);

        Task AddPlanetAsync(string name);
    }
}
