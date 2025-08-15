using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptCad.Plugin.Utility
{
    public static class ReadAPIKeyFile
    {
        public static string GetObjectJson(string objectName)
        {
            // get API Key from file
            string apiKeyFilePath = globalAPI.ApiKeyFilePath;
            if (!File.Exists(apiKeyFilePath))
            {
                throw new FileNotFoundException("API Key file not found.", apiKeyFilePath);
            }

            string apiKeyJson = File.ReadAllText(apiKeyFilePath);
            JObject apiKeyObject = JObject.Parse(apiKeyJson);

            // Dùng biến objectName làm key
            string objectVariable = apiKeyObject[objectName]?.ToString();

            if (string.IsNullOrEmpty(objectVariable))
            {
                throw new InvalidOperationException($"Key '{objectName}' is not set or is empty in API Key file.");
            }

            return objectVariable;
        }



    }
}
