using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptCad.Plugin.Utility
{
    public static class HandleLispFile
    {
        static string _filePath = globalAPI.PromptFilePath;
        public static string ReadLispFile()
        {
            if (!System.IO.File.Exists(_filePath))
            {
                throw new FileNotFoundException("Lisp file not found.", _filePath);
            }
            return System.IO.File.ReadAllText(_filePath);
        }
        public static void WriteLispFile(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Content cannot be null or empty.", nameof(content));
            }
            System.IO.File.WriteAllText(_filePath, content);
        }
        public static void AppendLispFile(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Content cannot be null or empty.", nameof(content));
            }
            System.IO.File.AppendAllText(_filePath, content);
        }
    }
}
