using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using UserActor.Interfaces;

namespace TenantService
{
    internal sealed class TenantService : StatefulService
    {
        IQueueClient queueClient;
        public const string NotificationApplicationName = "fabric:/NotificationApp";

        public TenantService(StatefulServiceContext context)
            : base(context)
        { }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            RegisterQueueHandler();
            return new ServiceReplicaListener[0];
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, "Inside RunAsync for Tenant Service");
                if (cancellationToken.IsCancellationRequested)
                {
                    await queueClient.CloseAsync();
                }
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, "RunAsync Failed, {0}", e);
                throw;
            }
        }


        void RegisterQueueHandler()
        {
            var serviceBusConfig = Context.CodePackageActivationContext.GetConfigurationPackageObject("Config").Settings.Sections["ServiceBusConfig"];
            var serviceBusConnectionString = serviceBusConfig.Parameters["ServiceBusConnectionString"].Value;
            var queueName = serviceBusConfig.Parameters["QueueName"].Value;
            queueClient = new QueueClient(serviceBusConnectionString, queueName);
            queueClient.RegisterMessageHandler(ProcessMessagesAsync, new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 10,
                AutoComplete = false
            });
        }

        async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var partitionKey = (this.Partition.PartitionInfo as Int64RangePartitionInformation).LowKey;
            ServiceEventSource.Current.ServiceMessage(this.Context, $"Partition {partitionKey} received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");
            var userIds = GetUserIdsByTenantId(int.Parse(message.Label));

            foreach (var userId in userIds)
            {
                var currentUser = ActorProxy.Create<IUserActor>(new ActorId(userId), NotificationApplicationName);
                //await currentUser.NotifyAsync(Encoding.UTF8.GetString(message.Body));
                Task.Run(async () => await currentUser.NotifyAsync(Encoding.UTF8.GetString(message.Body))).ConfigureAwait(false);
            }
            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            ServiceEventSource.Current.ServiceMessage(this.Context, $"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            return Task.CompletedTask;
        }

        List<int> GetUserIdsByTenantId(int tenantId)
        {
            return new List<int>()
            {
                tenantId + 1, tenantId + 2, tenantId + 3, tenantId + 4
            };
        }
    }
}
