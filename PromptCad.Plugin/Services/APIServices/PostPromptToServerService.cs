using PromptCad.Plugin.Models;
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
       public async Task<PromptResponse> PostPromptService(string prompt)
        {
            // using RestfulMethod to post prompt to server
            // Check Internet connection
            var isConnected = Utility.checkAPIKey.IsInternetConnected();
            if (!isConnected)
            {
                return new PromptResponse
                {
                    result = "không có kết nối internet",
                    type_response = "text"
                };
            }

            // Get session token from file
            string sessionToken = ReadAPIKeyFile.GetObjectJson("SessionToken");

            // post prompt to server
            var apiUrl = globalAPI.ApiUrl + "/generate-entity";
            var headers = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {sessionToken}" }
            };

            try
            {
                var response = await RestfulMethod.PostAsync(apiUrl, new { prompt = prompt }, headers);
                if (string.IsNullOrEmpty(response))
                {
                    throw new Exception("Server không phản hồi.");
                }
                // Parse the JSON response
                var jsonResponse = Newtonsoft.Json.Linq.JObject.Parse(response);
                var result = jsonResponse["result"]?.ToString();
                var typeResponse = jsonResponse["type_response"]?.ToString();
                return new PromptResponse
                {
                    result = result,
                    type_response = typeResponse
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gửi prompt: {ex.Message}");
                return new PromptResponse
                {
                    result = "Có lỗi xảy ra khi gửi prompt",
                    type_response = "text"
                };
            }

        }

    }
}
