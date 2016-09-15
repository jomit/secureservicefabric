using MemberApi.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Http;
using System.Linq;

namespace MemberApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/v1/member")]
    public class MemberController : ApiController
    {
        [HttpGet]
        [Route("get")]
        public Member Get()
        {
            string owner = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            return new Member()
            {
                Identifier = owner,
                Name = "Jomit"
            };
        }
    }
}
