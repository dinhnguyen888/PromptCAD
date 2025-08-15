using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Newtonsoft.Json.Linq;
using PromptCad.Plugin.Models;
using PromptCad.Plugin.Utility;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace PromptCad.Plugin.Services.ProcessDataServices
{
    public partial class ProcessDataServices
    {
        public void ProcessDrawResponse(PromptResponse response)
        {
            try
            {
                if (response == null)
                    throw new ArgumentNullException(nameof(response), "Response cannot be null");

                if (response.type_response.Equals("object", StringComparison.OrdinalIgnoreCase))
                {
                    var doc = Application.DocumentManager.MdiActiveDocument;

                    // result là Lisp code thuần → xử lý trực tiếp
                    string lispCode = response.result;

                    if (string.IsNullOrWhiteSpace(lispCode))
                        throw new System.Exception("No Lisp code found in response.");

                    string escaped = FilterAutolispCode(lispCode);

                    doc.SendStringToExecute(escaped, true, false, false);
                }
                else
                {
                    Console.WriteLine("Response type is not 'object'. No action taken.");
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                Console.WriteLine($"Error processing object response: {ex.Message}");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
            }
        }

        private string FilterAutolispCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return string.Empty;

            // 1. Xóa xuống dòng và tab
            string result = code.Replace("\r", "").Replace("\n", " ").Replace("\t", " ");

            // 2. Thêm (princ) nếu chưa có
            if (!result.Contains("(princ)"))
                result = result.TrimEnd() + " (princ)";

            // Trả về Lisp code thuần, không escape
            return result;
        }

    }


}
