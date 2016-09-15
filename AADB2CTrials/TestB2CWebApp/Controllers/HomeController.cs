using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace TestB2CWebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Claims()
        {
            System.Security.Claims.Claim displayName = ClaimsPrincipal.Current.FindFirst(ClaimsPrincipal.Current.Identities.First().NameClaimType);
            ViewBag.DisplayName = displayName != null ? displayName.Value : string.Empty;
            return View();
        }

        public ActionResult Error(string message)
        {
            ViewBag.Message = message;

            return View("Error");
        }
    }
}