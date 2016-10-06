using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using MyBackEnd.Domain;
using Common;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using System.Runtime.Remoting.Messaging;

namespace MyBackEnd
{
    internal sealed class MyBackEnd : StatefulService, IMyBackend
    {
        public MyBackEnd(StatefulServiceContext context)
            : base(context)
        { }


        public async Task<IEnumerable<string>> GetData(string correlationId)
        {
            LogStart("GetData", correlationId);

            //Some processing
            var data = new List<string>();
            for (int i = 0; i < 100; i++)
            {
                data.Add(i.ToString());
            }
            
            LogStop("GetData", correlationId);

            return await Task.FromResult(data);
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] {
                new ServiceReplicaListener(context => this.CreateServiceRemotingListener(context))
            };
        }

        private void LogStart(string methodName, string correlationId)
        {
            var serviceDetails = string.Format("{0}/{1}/{2}",
              this.Context.ServiceName.ToString(),
              this.Context.PartitionId,
              (this.Context as StatefulServiceContext).ReplicaOrInstanceId);

            ServiceEventSource.Current.ServiceRequestStart(methodName, correlationId, serviceDetails);
        }

        private void LogStop(string methodName, string correlationId)
        {
            var serviceDetails = string.Format("{0}/{1}/{2}",
              this.Context.ServiceName.ToString(),
              this.Context.PartitionId,
              (this.Context as StatefulServiceContext).ReplicaOrInstanceId);

            ServiceEventSource.Current.ServiceRequestStop(methodName, correlationId, serviceDetails);
        }
    }
}
