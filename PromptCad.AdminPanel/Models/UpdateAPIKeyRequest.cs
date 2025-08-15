using Newtonsoft.Json;
using System;

namespace PromptCad.AdminPanel.Models
{
    public class UpdateAPIKeyRequest
    {
        [JsonProperty("api_key")]
        public string ApiKey { get; set; }

        [JsonProperty("duration_months")]
        public int DurationMonths { get; set; }
    }
}
