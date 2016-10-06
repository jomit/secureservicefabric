using Common;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace MyApi
{
    internal class ServiceRequestActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            ServiceEventSource.Current.ServiceRequestStart(actionContext.ActionDescriptor.ActionName,
                ServiceTracingContext.GetRequestCorrelationId(),
                ServiceTracingContext.GetRequestServiceDetails());
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            ServiceEventSource.Current.ServiceRequestStop(actionExecutedContext.ActionContext.ActionDescriptor.ActionName,
                ServiceTracingContext.GetRequestCorrelationId(),
                ServiceTracingContext.GetRequestServiceDetails(),
                actionExecutedContext.Exception?.ToString() ?? string.Empty);
        }
    }
}
