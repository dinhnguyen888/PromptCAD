using PromptCad.AdminPanel.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace PromptCad.AdminPanel.Services
{
    public partial class ProjectServices
    {
        private string BaseUrl => globalAPI.ApiUrl;
        private string AccessToken => this.GetToken();

        private HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);
            return httpClient;
        }

       

        // Refresh admin session token
        public async Task<string> RefreshAdminSessionToken()
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.PostAsync($"{BaseUrl}/refresh-admin-session-token", null);
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error refreshing admin session token: {response.ReasonPhrase}");

                var json = await response.Content.ReadAsStringAsync();
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);
                return result.sessionToken;
            }
        }

        // Lấy tất cả API Key
        public async Task<GetAPIKeyResponse> GetAllAPIKey()
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.GetAsync($"{BaseUrl}/get-all-api-keys-admin");
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error fetching API keys: {response.ReasonPhrase}");

                var json = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<GetAPIKeyResponse>(json);
            }
        }

        // Tạo mới API Key
        public async Task<CreateAPIKeyResponse> CreateAPIKey(CreateAPIKeyRequest request)
        {
            using (var httpClient = CreateHttpClient())
            {
                var jsonContent = new StringContent(
                    Newtonsoft.Json.JsonConvert.SerializeObject(request),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await httpClient.PostAsync($"{BaseUrl}/create-api-key", jsonContent);
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error creating API key: {response.ReasonPhrase}");

                var json = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<CreateAPIKeyResponse>(json);
            }
        }

        // Cập nhật thời hạn API Key
        public async Task<bool> UpdateAPIKey(UpdateAPIKeyRequest request)
        {
            using (var httpClient = CreateHttpClient())
            {
                var jsonContent = new StringContent(
                    Newtonsoft.Json.JsonConvert.SerializeObject(request),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await httpClient.PostAsync($"{BaseUrl}/update-api-key", jsonContent);
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error updating API key: {response.ReasonPhrase}");

                return true;
            }
        }

        // Cập nhật thông tin API Key
        public async Task<bool> UpdateAPIKeyInfo(UpdateAPIKeyInfoRequest request)
        {
            using (var httpClient = CreateHttpClient())
            {
                var jsonContent = new StringContent(
                    Newtonsoft.Json.JsonConvert.SerializeObject(request),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await httpClient.PostAsync($"{BaseUrl}/update-api-key-info", jsonContent);
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error updating API key info: {response.ReasonPhrase}");

                return true;
            }
        }

        // Xóa API Key
        public async Task<bool> DeleteAPIKey(DeleteAPIKeyRequest request)
        {
            using (var httpClient = CreateHttpClient())
            {
                var jsonContent = new StringContent(
                    Newtonsoft.Json.JsonConvert.SerializeObject(request),
                    Encoding.UTF8,
                    "application/json"
                );

                var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"{BaseUrl}/delete-api-key")
                {
                    Content = jsonContent
                };

                var response = await httpClient.SendAsync(requestMessage);
                
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error deleting API key: {response.ReasonPhrase}");

                return true;
            }
        }

        // Export prompts to Excel
        public async Task<string> ExportPromptsToExcel()
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.GetAsync($"{BaseUrl}/export-prompts-to-excel");
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error exporting prompts: {response.ReasonPhrase}");

                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    FileName = $"prompts_export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var fileBytes = await response.Content.ReadAsByteArrayAsync();
                    File.WriteAllBytes(saveFileDialog.FileName, fileBytes);
                    return saveFileDialog.FileName;
                }

                return null;
            }
        }

        // Import prompts from Excel
        public async Task<bool> ImportPromptsFromExcel(string filePath)
        {
            using (var httpClient = CreateHttpClient())
            using (var fileStream = File.OpenRead(filePath))
            {
                var formData = new MultipartFormDataContent();
                var fileContent = new StreamContent(fileStream);
                formData.Add(fileContent, "file", Path.GetFileName(filePath));

                var response = await httpClient.PostAsync($"{BaseUrl}/import-prompts-from-excel", formData);
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error importing prompts: {response.ReasonPhrase}");

                return true;
            }
        }

        // Get shapes content
        public async Task<ShapesContentResponse> GetShapesContent()
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.GetAsync($"{BaseUrl}/get-shapes-content");
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error getting shapes content: {response.ReasonPhrase}");

                var json = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<ShapesContentResponse>(json);
            }
        }

        // Get AI model config
        public async Task<AIModelConfigResponse> GetAIModelConfig()
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.GetAsync($"{BaseUrl}/get-ai-model-config");
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error getting AI model config: {response.ReasonPhrase}");

                var json = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<AIModelConfigResponse>(json);
            }
        }

        // Update AI model API key
        public async Task<bool> UpdateAIModelAPIKey(string modelName, string newApiKey)
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.PostAsync($"{BaseUrl}/update-ai-model-api-key?model_name={modelName}&new_api_key={newApiKey}", null);
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error updating AI model API key: {response.ReasonPhrase}");

                return true;
            }
        }

        // Cleanup expired API keys
        public async Task<bool> CleanupExpiredAPIKeys()
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.DeleteAsync($"{BaseUrl}/cleanup-expired-api-keys");
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error cleaning up expired API keys: {response.ReasonPhrase}");

                return true;
            }
        }

        // Get expired API keys
        public async Task<GetAPIKeyResponse> GetExpiredAPIKeys()
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.GetAsync($"{BaseUrl}/get-expired-api-keys");
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error getting expired API keys: {response.ReasonPhrase}");

                var json = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<GetAPIKeyResponse>(json);
            }
        }

        // Delete all prompts
        public async Task<bool> DeleteAllPrompts()
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.DeleteAsync($"{BaseUrl}/delete-all-prompts");
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error deleting all prompts: {response.ReasonPhrase}");

                return true;
            }
        }
    }
}
