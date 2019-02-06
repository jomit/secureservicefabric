using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using UserActor.Interfaces;
using EmailService.Domain;
using NotificationApp.Shared;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace UserActor
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class UserActor : Actor, IUserActor
    {
        private const string UserStateName = "UserData";
        private const string EmailServiceName = "EmailService";

        public UserActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, $"User Actor {this.Id} activated.");
            return this.StateManager.TryAddStateAsync(UserStateName, new UserActorState(true));
        }

        public async Task NotifyAsync(string message)
        {
            var state = await this.StateManager.GetStateAsync<UserActorState>(UserStateName);
            if (state.IsEmailEnabled)
            {
                ActorEventSource.Current.ActorMessage(this, $"Calling Email Service for User Actor {this.Id}.");
                var builder = new ServiceUriBuilder(EmailServiceName);
                var emailServiceClient = ServiceProxy.Create<IEmailService>(builder.ToUri());
                //await emailServiceClient.SendMessageAsync(message);
                Task.Run(async () => await emailServiceClient.SendMessageAsync(message)).ConfigureAwait(false);
            }
        }
    }
}
