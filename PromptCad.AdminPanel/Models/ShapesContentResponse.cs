using Newtonsoft.Json;
using System;

namespace PromptCad.AdminPanel.Models
{
    public class ShapesContentResponse
    {
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("lines")]
        public int Lines { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
