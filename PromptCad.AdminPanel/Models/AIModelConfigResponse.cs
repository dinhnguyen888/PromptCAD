using Newtonsoft.Json;
using System.Collections.Generic;

namespace PromptCad.AdminPanel.Models
{
    public class AIModelConfigResponse
    {
        [JsonProperty("models")]
        public Dictionary<string, ModelInfo> Models { get; set; }
    }

    public class ModelInfo
    {
        [JsonProperty("has_api_key")]
        public bool HasApiKey { get; set; }

        [JsonProperty("key_length")]
        public int KeyLength { get; set; }
    }
}
