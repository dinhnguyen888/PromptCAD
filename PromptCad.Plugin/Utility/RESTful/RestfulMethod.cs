using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PromptCad.Plugin.Utility.RESTful
{
    public static class RestfulMethod
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task<string> GetAsync(string url, Dictionary<string, string> headers = null) =>
            await SendRequestAsync(HttpMethod.Get, url, null, headers);

        public static async Task<string> PostAsync(string url, object data, Dictionary<string, string> headers = null) =>
            await SendRequestAsync(HttpMethod.Post, url, data, headers);

        public static async Task<string> PutAsync(string url, object data, Dictionary<string, string> headers = null) =>
            await SendRequestAsync(HttpMethod.Put, url, data, headers);

        public static async Task<string> DeleteAsync(string url, Dictionary<string, string> headers = null) =>
            await SendRequestAsync(HttpMethod.Delete, url, null, headers);

        public static async Task<string> PatchAsync(string url, object data, Dictionary<string, string> headers = null) =>
            await SendRequestAsync(new HttpMethod("PATCH"), url, data, headers);

        private static async Task<string> SendRequestAsync(HttpMethod method, string url, object data = null, Dictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage(method, url);

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (data != null)
            {
                var json = JsonConvert.SerializeObject(data);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Error: {response.StatusCode} - {content}");

            return content;
        }
    }
}
