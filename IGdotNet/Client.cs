using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Web;

using Newtonsoft.Json;

namespace IGdotNet
{
    public class Client
    {
        private HttpClient client = null;

        private string username;
        private string password;
        private string apikey;

        private string cst;
        private string token;

        private bool loggedIn;

        private string baseUri = "https://api.ig.com";

        public Client(string username, string password, string apikey)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(baseUri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            this.username = username;
            this.password = password;
            this.apikey = apikey;

            loggedIn = false;
        }

        public async Task<bool> Login()
        {
            bool success = false;

            InternalObjects.LoginFields loginFields = new InternalObjects.LoginFields();
            loginFields.identifier = username;
            loginFields.password = password;
            loginFields.encryptedPassword = null;

            StringContent content = new StringContent(JsonConvert.SerializeObject(loginFields), Encoding.UTF8, "application/json");
            content.Headers.Add("X-IG-API-KEY", apikey);
            content.Headers.Add("Version", "2");

            HttpResponseMessage response = await client.PostAsync("gateway/deal/session", content);
            if (response.IsSuccessStatusCode)
            {
                cst = response.Headers.GetValues("CST").FirstOrDefault();
                token = response.Headers.GetValues("X-SECURITY-TOKEN").FirstOrDefault();
                loggedIn = true;
                success = true;
            } else
            {
                Debug.WriteLine(response.ReasonPhrase);
            }

            return success;
        }

        public async Task<ResponseObjects.SearchResult> SearchMarkets(string searchTerm)
        {
            if (loggedIn)
            {
                UriBuilder builder = new UriBuilder(baseUri + "/gateway/deal/markets");
                builder.Port = -1;
                var query = HttpUtility.ParseQueryString(builder.Query);
                query["searchTerm"] = searchTerm;
                builder.Query = query.ToString();

                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, builder.ToString());
                message.Headers.Add("X-IG-API-KEY", apikey);
                message.Headers.Add("CST", cst);
                message.Headers.Add("X-SECURITY-TOKEN", token);
                message.Headers.Add("Version", "1");

                HttpResponseMessage response = await client.SendAsync(message);
                if (response.IsSuccessStatusCode)
                {
                    return ResponseObjects.SearchResult.FromHttpResponse(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    Debug.WriteLine(response.ReasonPhrase);
                }
            }

            return null;
        }

        public async Task<ResponseObjects.HistoricPriceResult> HistoricPriceSearch(string epic, ResponseObjects.Resolution resolution, int resolutionCount = 10)
        {
            if (loggedIn)
            {
                UriBuilder builder = new UriBuilder(string.Format("{0}/gateway/deal/prices/{1}", baseUri, epic));
                builder.Port = -1;
                if (resolution != ResponseObjects.Resolution.UNSPECIFIED)
                {
                    var query = HttpUtility.ParseQueryString(builder.Query);
                    query["resolution"] = resolution.ToString();
                    query["max"] = resolutionCount.ToString();
                    query["pageSize"] = 0.ToString();
                    builder.Query = query.ToString();
                }

                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, builder.ToString());
                message.Headers.Add("X-IG-API-KEY", apikey);
                message.Headers.Add("Version", "3");
                message.Headers.Add("CST", cst);
                message.Headers.Add("X-SECURITY-TOKEN", token);

                HttpResponseMessage response = await client.SendAsync(message);
                if (response.IsSuccessStatusCode)
                {
                    return ResponseObjects.HistoricPriceResult.FromHttpResponse(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    Debug.WriteLine(response.ReasonPhrase);
                }
            }

            return null;
        }
    }
}
