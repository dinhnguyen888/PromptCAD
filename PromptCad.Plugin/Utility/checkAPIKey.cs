using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PromptCad.Plugin.Utility
{
    public static class checkAPIKey
    {
        public static (bool, string?) InitAPIKeyStatus()
        {
            if (!IsInternetConnected())
            {
                return (false, "Chưa kết nối mạng kìa Thảo ơi.");
            }
            try
            {

                // Get APIKey from File instead of global variable
                string apiKeyFilePath = globalAPI.ApiKeyFilePath;
                if (!System.IO.File.Exists(apiKeyFilePath))
                {
                    return (false, "Không tìm thấy file API Key. Hãy nạp API Key trước đã.");
                }
                string apiKey = ReadAPIKeyFile.GetObjectJson("ApiKey");
                if (apiKey == null)
                {
                    return (false, "File API Key không hợp lệ. Hãy nạp lại API Key.");
                }
                

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"Có lỗi mất tiu rồi: {ex.Message}");
            }
        }

        public static bool IsInternetConnected()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://www.google.com"))
                    return true;
            }
            catch
            {
                return false;
            }
        }
    }
   

    }
