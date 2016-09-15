using MemberApi.Auth;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Web;
using System.Web.Http;
using TestSFApp.Common;

namespace MemberApi
{
    public static class Startup
    {
        // These values are pulled from web.config
        private static string AADConfigSectionName = "AADConfig";
        public static string aadInstance = "";
        public static string tenant = "";
        public static string clientId = "";
        public static string signUpPolicy = "";
        public static string signInPolicy = "";
        public static string editProfilePolicy = "";

        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public static void ConfigureApp(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            var settingsService = new SettingsService();
            var parameters = settingsService.GetSection(AADConfigSectionName);
            aadInstance = parameters["ida:AadInstance"];
            tenant = parameters["ida:Tenant"];
            clientId = parameters["ida:ClientId"];
            signUpPolicy = parameters["ida:SignUpPolicyId"];
            signInPolicy = parameters["ida:SignInPolicyId"];
            editProfilePolicy = parameters["ida:UserProfilePolicyId"];

            appBuilder.UseOAuthBearerAuthentication(CreateBearerOptionsFromPolicy(signUpPolicy));
            appBuilder.UseOAuthBearerAuthentication(CreateBearerOptionsFromPolicy(signInPolicy));
            appBuilder.UseOAuthBearerAuthentication(CreateBearerOptionsFromPolicy(editProfilePolicy));

            appBuilder.UseWebApi(config);
        }

        public static OAuthBearerAuthenticationOptions CreateBearerOptionsFromPolicy(string policy)
        {
            TokenValidationParameters tvps = new TokenValidationParameters
            {
                // This is where you specify that your API only accepts tokens from its own clients
                ValidAudience = clientId,
                AuthenticationType = policy,
            };

            return new OAuthBearerAuthenticationOptions
            {
                // This SecurityTokenProvider fetches the Azure AD B2C metadata & signing keys from the OpenIDConnect metadata endpoint
                AccessTokenFormat = new JwtFormat(tvps, new OpenIdConnectCachingSecurityTokenProvider(String.Format(aadInstance, tenant, policy))),
            };
        }
    }
}
