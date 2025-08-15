using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PromptCad.AdminPanel.Models
{
    public class GetAPIKeyResponse
    {
        [JsonProperty("api_keys")]
        public List<ApiKeyInfo> ApiKeys { get; set; }
    }

    public class ApiKeyInfo
    {
        [JsonProperty("api_key")]
        public string ApiKey { get; set; }

        [JsonProperty("expires_at")]
        public DateTime ExpiresAt { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("is_active")]
        public bool IsActive { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }
    }
}
