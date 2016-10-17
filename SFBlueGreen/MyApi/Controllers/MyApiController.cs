using System.Collections.Generic;
using System.Web.Http;

namespace MyApi.Controllers
{
    [ServiceRequestActionFilter]
    [RoutePrefix("api/myapi")]
    public class MyApiController : ApiController
    {
        //V1
        [HttpGet]
        [Route("getcities")]
        public IEnumerable<string> GetCities()
        {
            return new string[] { "Seattle", "London" };
        }

        //V2
        //[HttpGet]
        //[Route("getcities")]
        //public IEnumerable<string> GetCities()
        //{
        //    return new string[] { "Seattle", "London", "Delhi", "Berlin" };
        //}
    }
}
