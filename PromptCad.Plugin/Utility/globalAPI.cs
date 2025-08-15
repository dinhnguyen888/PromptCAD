using Autodesk.AutoCAD.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptCad.Plugin.Utility
{
    public class globalAPI
    {
        public static string ApiUrl { get; set; } = "http://127.0.0.1:8000/api";
        public static string ApiKeyFilePath { get; set; } = @"C:\Users\Public\Documents\PromptCad\APIKey.json";
    }
}
