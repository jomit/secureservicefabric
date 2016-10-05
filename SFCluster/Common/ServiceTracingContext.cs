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
        private const string CorrelationHeaderName = "__CorrelationId";
        private const string ServiceDetailsKey = "__ServiceDetails";


        public static void CreateRequestCorrelationId()
        {
            CallContext.LogicalSetData(CorrelationHeaderName, GenerateId());
        }

        public static string GetRequestCorrelationId()
        {
            return CallContext.LogicalGetData(CorrelationHeaderName) as string;
        }

        public static void SetRequestCorrelationId(string value)
        {
            CallContext.LogicalSetData(CorrelationHeaderName, value);
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
