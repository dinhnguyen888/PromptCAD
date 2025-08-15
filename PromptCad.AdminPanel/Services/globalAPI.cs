using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptCad.AdminPanel.Services
{
    public static class globalAPI
    {
        public static string ApiUrl { get; set; } = "http://127.0.0.1:8000/api";
        public static string TokenFilePath { get; set; } = "C:\\Users\\Public\\Documents\\AdminPromptCad\\access_token.txt";
    
    }
}
