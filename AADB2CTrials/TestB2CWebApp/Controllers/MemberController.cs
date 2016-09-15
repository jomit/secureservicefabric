using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TestB2CWebApp.Controllers
{
    public class MemberController : Controller
    {
        private static string serviceUrl = ConfigurationManager.AppSettings["api:MemberServiceUrl"];

        public async Task<ActionResult> Index()
        {
            try
            {

                var bootstrapContext = ClaimsPrincipal.Current.Identities.First().BootstrapContext as System.IdentityModel.Tokens.BootstrapContext;

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, serviceUrl + "/api/v1/member/get");

                // Add the token acquired from ADAL to the request headers
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bootstrapContext.Token);
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    String responseString = await response.Content.ReadAsStringAsync();
                    ViewBag.Member = JObject.Parse(responseString);
                    return View();
                }
                else
                {
                    // If the call failed with access denied, show the user an error indicating they might need to sign-in again.
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return new RedirectResult("/Error?message=Error: " + response.ReasonPhrase + " You might need to sign in again.");
                    }
                }

                return new RedirectResult("/Error?message=An Error Occurred Reading Member List: " + response.StatusCode);
            }
            catch (Exception ex)
            {
                return new RedirectResult("/Error?message=An Error Occurred Reading Member List: " + ex.Message);
            }
        }
    }
}