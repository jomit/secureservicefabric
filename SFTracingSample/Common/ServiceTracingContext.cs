using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting;

namespace Common
{
    public static class ServiceTracingContext
    {
        const string CorrelationKey = "CorrelationId";
        const string ServiceDetailsKey = "ServiceDetails";


        public static void CreateRequestCorrelationId()
        {
            CallContext.LogicalSetData(CorrelationKey, GenerateId());
        }

        public static string GetRequestCorrelationId()
        {
            return CallContext.LogicalGetData(CorrelationKey) as string;
        }

        public static void SetRequestCorrelationId(string value)
        {
            CallContext.LogicalSetData(CorrelationKey, value);
        }


        public static string GetRequestServiceDetails()
        {
            return CallContext.LogicalGetData(ServiceDetailsKey) as string;
        }

        public static void SetRequestServiceDetails(string value)
        {
            CallContext.LogicalSetData(ServiceDetailsKey, value);
        }

        private static string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
