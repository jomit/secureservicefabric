using System.Collections.Generic;
using System.Web.Http;

namespace MyApi.Controllers
{
    [ServiceRequestActionFilter]
    [RoutePrefix("api/v1/myapi")]
    public class MyApiController : ApiController
    {
        [HttpGet]
        [Route("getdata")]
        public IEnumerable<string> GetData()
        {
            return new string[] { "MyData1", "MyData2" };
        }
    }
}
