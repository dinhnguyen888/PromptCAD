using Newtonsoft.Json;
using System;

namespace PromptCad.AdminPanel.Models
{
    public class DeleteAPIKeyRequest
    {
        [JsonProperty("api_key")]
        public string ApiKey { get; set; }

        [JsonProperty("cleanup_expired")]
        public bool CleanupExpired { get; set; } = false;
    }
}
