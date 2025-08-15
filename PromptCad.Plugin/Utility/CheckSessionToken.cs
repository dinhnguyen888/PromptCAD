using PromptCad.Plugin.Utility.RESTful;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptCad.Plugin.Utility
{
    public class CheckSessionToken
    {
        public static async  Task<bool> IsSessionTokenValid(string sessionToken)
        {
            
            if (string.IsNullOrEmpty(sessionToken))
            {
                return false;
            }
           
            //Try to call the server with the session token 
            var apiUrl = globalAPI.ApiUrl + "/check-session-token";

            // Read session token from file
            string token = sessionToken.Trim();
            


            var headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                };

            try
            {
             
                var response = await RestfulMethod.GetAsync(apiUrl, headers);
                // Parse the JSON response to check "valid" property
                try
                {
                    var json = System.Text.Json.JsonDocument.Parse(response);
                    if (json.RootElement.TryGetProperty("valid", out var validProp) && validProp.GetBoolean())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi kiểm tra session token: {ex.Message}");
                return false;
            }

       
        }
    }
}
