using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptCad.AdminPanel.Models
{
    public class CreateAPIKeyResponse
    {
        [JsonProperty("api_key")]
        public string ApiKey { get; set; }
        
        [JsonProperty("expires_at")]
        public DateTime ExpiresAt { get; set; }
    }
}
