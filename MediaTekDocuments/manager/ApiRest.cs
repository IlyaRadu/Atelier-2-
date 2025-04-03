using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace MediaTekDocuments.manager
{
    class ApiRest
    {
        private static ApiRest instance = null;
        private readonly HttpClient httpClient;
        private HttpResponseMessage httpResponse;

        private ApiRest(string uriApi, string authenticationString = "")
        {
            httpClient = new HttpClient() { BaseAddress = new Uri(uriApi) };
            if (!string.IsNullOrEmpty(authenticationString))
            {
                string base64 = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(authenticationString));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + base64);
            }
        }

        public static ApiRest GetInstance(string uriApi, string authenticationString)
        {
            if (instance == null)
            {
                instance = new ApiRest(uriApi, authenticationString);
            }
            return instance;
        }

        public JObject RecupDistant(string methode, string message, string parametres)
        {
            StringContent content = null;
            if (!(parametres is null))
            {
                content = new StringContent(parametres, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
            }
            switch (methode)
            {
                case "GET":
                    httpResponse = httpClient.GetAsync(message).Result;
                    break;
                case "POST":
                    httpResponse = httpClient.PostAsync(message, content).Result;
                    break;
                case "PUT":
                    httpResponse = httpClient.PutAsync(message, content).Result;
                    break;
                case "DELETE":
                    httpResponse = httpClient.DeleteAsync(message).Result;
                    break;
                default:
                    return new JObject();
            }
            return httpResponse.Content.ReadAsAsync<JObject>().Result;
        }
    }
}
