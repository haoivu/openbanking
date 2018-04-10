using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using openbanking.Models;
using openbanking.Models.APIViewModels;

namespace openbanking.Controllers
{
    [Authorize]
    public class APIController : Controller
    {
        const string ClientId = "216aa0f8-4d76-4f03-ac5f-fd1e65421c65";
        const string ClientSecret = "A6sF0cB4bY6cT3qQ4qD6jS1aJ2eP1hJ1iJ6sV5iS5tU4fF8oB5";
        const string AccountId = "FI6593857450293470-EUR";
        // for testing
        const string RedirectUrl = "http://localhost:5000/auth/nordea";

        // for production
        // const string RedirectUrl = "http://obbudgetting.azurewebsites.net/auth/nordea";

        private async Task<string> GetDatav1(string url)
        {
            using (var client = new HttpClient())
            {
                var accessToken = HttpContext.Session.GetString("AccessToken");

                // Set Client Credentials
                client.DefaultRequestHeaders.Add("X-IBM-Client-Id", ClientId);
                client.DefaultRequestHeaders.Add("X-IBM-Client-Secret", ClientSecret);

                //Set Bearer Token and Authentication Endpoint
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                client.BaseAddress = new Uri("https://api.nordeaopenbanking.com/v1/");

                //Send Get Request and Return the Result as string
                return await client.GetStringAsync(url);
            }
        }

        private async Task<string> GetDatav2(string url)
        {
            using (var client = new HttpClient())
            {
                var accessToken = HttpContext.Session.GetString("AccessToken");

                // Set Client Credentials
                client.DefaultRequestHeaders.Add("X-IBM-Client-Id", ClientId);
                client.DefaultRequestHeaders.Add("X-IBM-Client-Secret", ClientSecret);

                //Set Bearer Token and Authentication Endpoint
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                client.BaseAddress = new Uri("https://api.nordeaopenbanking.com/v2/");

                //Send Get Request and Return the Result as string
                return await client.GetStringAsync(url);
            }
        }

        public async Task<IActionResult> GetAssets()
        {
            //Get assets
            var response = await GetDatav1("assets");
            //Deserialize
            var result = JsonConvert.DeserializeObject<AccountsModel>(response);
            return Content(response);
        }

        public async Task<IActionResult> GetAccounts()
        {
            //Get assets
            var response = await GetDatav2("accounts");
            //Deserialize
            var result = JsonConvert.DeserializeObject<AccountsModel>(response);
            return View();
        }

        public async Task<IActionResult> GetAccounts2()
        {
            //Get assets
            var response = await GetDatav2("accounts");
            //Deserialize
            var result = JsonConvert.DeserializeObject<AccountsModel>(response);
            return Content(response);
        }


        [HttpGet("auth/code")]
        public IActionResult GetAccessCode()
        {
            var url = $"https://api.nordeaopenbanking.com/v1/authentication?&client_id={ClientId}&redirect_uri={RedirectUrl}&X-Response-Scenarios=AuthenticationSkipUI&state=";
            return Redirect(url);
        }


        [Route("auth/nordea")]
        public async Task<IActionResult> GetAccessToken([FromQuery]AccessCodeModel model)
        {
            using (var client = new HttpClient())
            {
                // Set Client Credentials
                client.DefaultRequestHeaders.Add("X-IBM-Client-Id", ClientId);
                client.DefaultRequestHeaders.Add("X-IBM-Client-Secret", ClientSecret);
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                var url = "https://api.nordeaopenbanking.com/v1/authentication/access_token";

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("code", model.Code),
                    new KeyValuePair<string, string>("redirect_uri", RedirectUrl)
                });
                var result = await client.PostAsync(url, content);
                var resultString = await result.Content.ReadAsStringAsync();

                
                var tokenModel = JsonConvert.DeserializeObject<AccessTokenModel>(resultString, new JsonSerializerSettings()
                {
                    ContractResolver = new UnderscorePropertyNamesContractResolver()
                });

                HttpContext.Session.SetString("AccessToken", tokenModel.AccessToken);

                return Json(tokenModel);
            }
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class UnderscorePropertyNamesContractResolver : DefaultContractResolver
    {
        public UnderscorePropertyNamesContractResolver()
        {
            NamingStrategy = new SnakeCaseNamingStrategy();
        }
    }
}
