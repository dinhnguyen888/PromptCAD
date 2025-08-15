using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptCad.AdminPanel.Services
{
    public partial class ProjectServices
    {
        public string? GetToken()
        {
            // Get file path for storing the token
            string filePath = globalAPI.TokenFilePath;
            // Read the token from the file
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    string token = System.IO.File.ReadAllText(filePath);
                    return token;
                }
                else
                {
                    Console.WriteLine("Token file does not exist.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during file reading
                Console.WriteLine($"Error retrieving token: {ex.Message}");
                return null;
            }
        }
        public void StoreToken(string token)
        {
            // Get file path for storing the token
            string filePath = globalAPI.TokenFilePath;

            // Write the token to the file
            try
            {
                // if the directory does not exist, create it
                string directory = System.IO.Path.GetDirectoryName(filePath);
                if (!System.IO.Directory.Exists(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                }

                // Write the token to the file
                System.IO.File.WriteAllText(filePath, token);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during file writing
                Console.WriteLine($"Error storing token: {ex.Message}");
            }
        }
    }
}
