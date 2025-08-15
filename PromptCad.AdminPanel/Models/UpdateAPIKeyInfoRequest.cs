using Newtonsoft.Json;
using System;

namespace PromptCad.AdminPanel.Models
{
    public class UpdateAPIKeyInfoRequest
    {
        [JsonProperty("api_key")]
        public string ApiKey { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }
    }
}
