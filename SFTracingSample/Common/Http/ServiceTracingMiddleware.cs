using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Http
{
    public class ServiceTracingMiddleware : OwinMiddleware
    {
        ServiceContext serviceContext;

        public ServiceTracingMiddleware(OwinMiddleware next, ServiceContext context) : base(next)
        {
            this.serviceContext = context;
        }

        public async override Task Invoke(IOwinContext context)
        {
            var serviceDetails = string.Format("{0}/{1}/{2}",
                serviceContext != null ? serviceContext.ServiceName.ToString() : null,
                serviceContext != null ? serviceContext.PartitionId : Guid.Empty,
                serviceContext != null ? (serviceContext as StatelessServiceContext).InstanceId : 0);

            // For end-to-end tracing we can also generate the CorrelationId in the web app, pass it in header and extract it here.
            ServiceTracingContext.CreateRequestCorrelationId();
            ServiceTracingContext.SetRequestServiceDetails(serviceDetails);

            await Next.Invoke(context);
        }
    }
}
