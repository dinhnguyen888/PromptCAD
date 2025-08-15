using PromptCad.AdminPanel.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace PromptCad.AdminPanel.Utils
{
    public static class CheckAccessToken
    {
        public static async Task<bool> IsTokenValidAsync()
        {
            try
            {
                // Read the token from the file
                string token = System.IO.File.ReadAllText(globalAPI.TokenFilePath).Trim();
                // Check if the token is empty
                if (string.IsNullOrEmpty(token))
                {
                    return false; // Token is empty, not valid
                }
                using (var httpClient = new HttpClient())
                {
                    // Set the Authorization header with the token
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    
                    // Make a request to a protected endpoint (replace with your actual endpoint)
                    var response = await httpClient.GetAsync(globalAPI.ApiUrl + "/check-admin-token");
                    // Check if the response is successful
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., network issues, etc.)
                Console.WriteLine($"Error checking token validity: {ex.Message}");
                return false;
            }
        }
    }
}
