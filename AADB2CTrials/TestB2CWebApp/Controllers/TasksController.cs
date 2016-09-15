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
    [Authorize]
    public class TasksController : Controller
    {
        private static string serviceUrl = ConfigurationManager.AppSettings["api:TaskServiceUrl"];

        // GET: TodoList
        public async Task<ActionResult> Index()
        {
            try
            {

                var bootstrapContext = ClaimsPrincipal.Current.Identities.First().BootstrapContext as System.IdentityModel.Tokens.BootstrapContext;

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, serviceUrl + "/api/tasks");

                // Add the token acquired from ADAL to the request headers
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bootstrapContext.Token);
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    String responseString = await response.Content.ReadAsStringAsync();
                    JArray tasks = JArray.Parse(responseString);
                    ViewBag.Tasks = tasks;
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

                return new RedirectResult("/Error?message=An Error Occurred Reading To Do List: " + response.StatusCode);
            }
            catch (Exception ex)
            {
                return new RedirectResult("/Error?message=An Error Occurred Reading To Do List: " + ex.Message);
            }
        }

        // POST: TodoList/Create
        [HttpPost]
        public async Task<ActionResult> Create(string description)
        {
            try
            {
                var bootstrapContext = ClaimsPrincipal.Current.Identities.First().BootstrapContext as System.IdentityModel.Tokens.BootstrapContext;

                HttpContent content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("Text", description) });
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, serviceUrl + "/api/tasks");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bootstrapContext.Token);
                request.Content = content;
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return new RedirectResult("/Tasks");
                }
                else
                {
                    // If the call failed with access denied, show the user an error indicating they might need to sign-in again.
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return new RedirectResult("/Error?message=Error: " + response.ReasonPhrase + " You might need to sign in again.");
                    }
                }

                return new RedirectResult("/Error?message=Error reading your To-Do List.");
            }
            catch (Exception ex)
            {
                return new RedirectResult("/Error?message=Error reading your To-Do List.  " + ex.Message);
            }
        }

        // POST: /TodoList/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var bootstrapContext = ClaimsPrincipal.Current.Identities.First().BootstrapContext as System.IdentityModel.Tokens.BootstrapContext;

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, serviceUrl + "/api/tasks/" + id);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bootstrapContext.Token);
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return new RedirectResult("/Tasks");
                }
                else
                {
                    // If the call failed with access denied, then drop the current access token from the cache, 
                    // and show the user an error indicating they might need to sign-in again.
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return new RedirectResult("/Error?message=Error: " + response.ReasonPhrase + " You might need to sign in again.");
                    }
                }

                return new RedirectResult("/Error?message=Error deleting your To-Do Item.");
            }
            catch (Exception ex)
            {
                return new RedirectResult("/Error?message=Error deleting your To-Do Item.  " + ex.Message);
            }
        }
    }
}