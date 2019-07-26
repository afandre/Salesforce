using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFLicenseRequest_MVC.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SalesforceSharp.Security;
using SalesforceSharp;
using RestSharp;


namespace SFLicenseRequest_MVC.Services
{
    public class REST
    {
        public static REST init()
        {
            return new REST();

        }
        public async Task<IEnumerable<DropDownObject>> GetSearchFilter(string filter)
        {
            // this is a generic call to gather list of values or "filter list"
            // the incoming value is the name of the field that has the list

            HttpRequestMessage request = await RESTCaseConnection(HttpMethod.Get);

            HttpClient queryClient = new HttpClient();
            HttpResponseMessage response = await queryClient.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();

            List<DropDownObject> filterList = GetDropdownList(result, filter);

            return filterList;
        }
        private static async Task<HttpRequestMessage> RESTCaseConnection(HttpMethod method)
        {
            //tls protocol is required
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string oauthToken, serviceUrl;
            //generate a new client to connect to your org
            HttpClient authClient = new HttpClient();
            //content is a dictionary of all the parameters we've set in the web.config
            HttpContent content = GetClientContent();
            //token from our org
            string tokenString = ConfigurationManager.AppSettings["tokenUrl"];
            //combine client and token to create the response message
            HttpResponseMessage message = await authClient.PostAsync(tokenString, content);
            //fire that client as async
            string responseString = await message.Content.ReadAsStringAsync();
            //set up that return object with our access
            JObject obj = JObject.Parse(responseString);
            oauthToken = (string)obj["access_token"];
            serviceUrl = (string)obj["instance_url"];

            //generate service strings for our desired request
            string service = ConfigurationManager.AppSettings.Get("RestService") + "sobjects/Case/describe/";
            string fullRequest = serviceUrl + service;
            //generate that request
            HttpRequestMessage request = new HttpRequestMessage(method, fullRequest);
            //add authorization header
            request.Headers.Add("Authorization", "Bearer " + oauthToken);
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            //send that request to be fired
            return request;
        }

         private List<DropDownObject> GetDropdownList(string result, string filter)
        {
            List<DropDownObject> filterList = new List<DropDownObject>();
            var jsonReturn = JsonConvert.DeserializeObject<PickFieldObjectDescription>(result);

            var orderObj = jsonReturn.fields.FirstOrDefault(f => f.name == filter);
            foreach (var item in orderObj.pickListValues)
            {
                string val = "";
                if (filter == "User_Role__c")
                {
                    val = item.validFor;
                } else
                {
                    val = item.value;
                }
                
                DropDownObject lv = new DropDownObject
                {
                    Key = item.value.ToString(),
                    Value = val
                };
                filterList.Add(lv);
            }
            return filterList;
        }


        public static HttpContent GetClientContent()
        {

            HttpContent content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "password" },
                {"client_id", ConfigurationManager.AppSettings["ConsumerKey"].ToString() },
                {"client_secret", ConfigurationManager.AppSettings["ConsumerSecret"].ToString() },
                {"username", ConfigurationManager.AppSettings.Get("username").ToString() },
                {"password", ConfigurationManager.AppSettings.Get("password").ToString() }
            });
            return content;
        }
    }

}
