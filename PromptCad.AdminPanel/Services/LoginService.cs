using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PromptCad.AdminPanel.Models;

namespace PromptCad.AdminPanel.Services
{
    public partial class ProjectServices
    {
        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            using (var httpClient = new HttpClient())
            {
                // Chuyển đối tượng LoginRequest thành JSON
                var json = JsonConvert.SerializeObject(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var apiUrl = globalAPI.ApiUrl + "/admin-login";
                // Gửi POST đến API key (thay URL bằng API thật của bạn)
                var response = await httpClient.PostAsync(apiUrl, content);

                // Đảm bảo response thành công
                response.EnsureSuccessStatusCode();

                // Đọc nội dung trả về dạng string
                var responseString = await response.Content.ReadAsStringAsync();

                // Deserialize JSON thành LoginResponse
                var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseString);

                return loginResponse;
            }
        }
    }
}
