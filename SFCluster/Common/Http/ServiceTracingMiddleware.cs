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

            ServiceTracingContext.CreateRequestCorrelationId();
            ServiceTracingContext.SetRequestServiceDetails(serviceDetails);

            await Next.Invoke(context);
        }
    }

    //May need this when using OAuth middleware
    //public class RestoreServiceTracingMiddleware : OwinMiddleware
    //{
    //    public RestoreServiceTracingMiddleware(OwinMiddleware next) : base(next) { }

    //    public async override Task Invoke(IOwinContext context)
    //    {
    //        var correlationId = ServiceTracingContext.GetRequestCorrelationId();
    //        if (correlationId != null)
    //        {
    //            ServiceTracingContext.SetRequestCorrelationId(correlationId);
    //        }
    //        await Next.Invoke(context);
    //    }
    //}

}
