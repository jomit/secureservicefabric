using System.Collections.Generic;
using System.Web.Http;

namespace SecureApi.Controllers
{
    [RoutePrefix("api/v1/myapi")]
    public class MyApiController : ApiController
    {
        [HttpGet]
        [Route("getall")]
        public IEnumerable<string> GetAll()
        {
            return new string[] { "London", "Newyork", "Seattle" };
        }
    }
}
