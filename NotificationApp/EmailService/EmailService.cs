using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmailService.Domain;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace EmailService
{
    internal sealed class EmailService : StatelessService, IEmailService
    {
        public EmailService(StatelessServiceContext context)
            : base(context)
        { }


        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            //return new ServiceInstanceListener[0];
            // Using V2 endpoint https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-reliable-services-communication-remoting
            return this.CreateServiceRemotingInstanceListeners();

        }

        public async Task SendMessageAsync(string message)
        {
            // simulating processing
            await Task.Delay(TimeSpan.FromSeconds(1));

            ServiceEventSource.Current.ServiceMessage(this.Context, $"Email sent with content : {message}");
        }
    }
}
