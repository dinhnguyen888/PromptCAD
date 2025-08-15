using PromptCad.Plugin.Utility;
using PromptCad.Plugin.Utility.RESTful;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptCad.Plugin.Services.APIServices
{
    public partial class APIServices
    {
        public async Task<bool> PostKeyToServerAsync(string apiKey)
        {
            try
            {
                // Check INternet connection 
                if (!Utility.checkAPIKey.IsInternetConnected())
                {
                    return false;
                }


                var apiUrl = globalAPI.ApiUrl + "/authen-api-key";
                // Call post method to send API key to the server and get session token 
                var response = await RestfulMethod.PostAsync(
                   apiUrl, new { api_key = apiKey }
                );
                // server handle
                if (string.IsNullOrEmpty(response))
                {
                    throw new Exception("Server không phản hồi.");
                }


                if (response.Contains("sessionToken", StringComparison.OrdinalIgnoreCase))
                {

                    //Store the API key and session token in a file

                    var jsonObj = Newtonsoft.Json.Linq.JObject.Parse(response);
                    var sessionToken = jsonObj["sessionToken"]?.ToString();
                    if (string.IsNullOrEmpty(sessionToken))
                    {
                        throw new Exception("Session token không hợp lệ.");
                    }
                    StoreAPIKeyAndSessionToken(apiKey, sessionToken);

                    return true;
                }
                else
                {
                    throw new Exception("API key không post được: " + response);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($": {ex.Message}");
                return false;
            }
        }

        private void StoreAPIKeyAndSessionToken(string apiKey, string sessionToken)
        {
            // check if the file exists in C:\Users\Public\Documents\PromptCad\APIKey.json
            string filePath = globalAPI.ApiKeyFilePath;
            if (!System.IO.File.Exists(filePath))
            {
                // Create the directory if it doesn't exist
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));
            }

            // Create a new object to store the API key and session token
            var apiData = new
            {
                ApiKey = apiKey,
                SessionToken = sessionToken
            };
            // Serialize the object to JSON
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(apiData, Newtonsoft.Json.Formatting.Indented);
            // Write the JSON to the file
            System.IO.File.WriteAllText(filePath, json);

           


        }
    }
}
