using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PromptCad.AdminPanel.Models
{
    public class PromptDto
    {
      
        public string prompt_text { get; set; }  = string.Empty;

        public DateTime? created_at { get; set; } = null;

        [JsonPropertyName("metadata")]
        public string Metadata { get; set; } = string.Empty;
    }

    public class GetAllPromptsResponse
    {
        [JsonPropertyName("prompts")]
        public List<PromptDto> Prompts { get; set; } = new();

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
