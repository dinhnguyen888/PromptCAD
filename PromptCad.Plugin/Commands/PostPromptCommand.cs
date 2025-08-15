using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Newtonsoft.Json.Linq;
using PromptCad.Plugin.Models;
using PromptCad.Plugin.Services.ProcessDataServices;
using PromptCad.Plugin.Utility;
using System.IO;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PromptCad.Plugin.Commands
{
    public class PostPromptCommand
    {
        [CommandMethod("PCP")]
        public async void PostPromptCommandMethod()
        {
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;
            var opts = new PromptStringOptions("\nNhập yêu cầu của bạn: ") { AllowSpaces = true };
            PromptResult result = ed.GetString(opts);
            if (result.Status != PromptStatus.OK)
            {
                ed.WriteMessage("\nLệnh bị hủy.");
                return;
            }
            string userPrompt = result.StringResult.Trim();
            if (string.IsNullOrEmpty(userPrompt))
            {
                ed.WriteMessage("\nPrompt không được để trống.");
                return;
            }
            // Check Internet connection
            var isConnected = Utility.checkAPIKey.IsInternetConnected();
            if (!isConnected)
            {
                ed.WriteMessage("\nKhông có kết nối internet.");
                return;
            }
            // Get session token from file
            string sessionToken = ReadAPIKeyFile.GetObjectJson("SessionToken");
            if (string.IsNullOrEmpty(sessionToken))
            {
                ed.WriteMessage("\nSession Token không hợp lệ.");
                return;
            }
            // Call the service to post prompt to server
            var apiService = new Services.APIServices.APIServices();
            PromptResponse response = await apiService.PostPromptService(userPrompt);
            if (response != null)
            {
                var processData = new ProcessDataServices();
                if (response.type_response == "text")
                {
                    processData.ProcessTextResponse(response);
                }
                else if (response.type_response == "object")
                {
                    processData.ProcessDrawResponse(response);
                }
                else
                {
                    ed.WriteMessage("\nLoại phản hồi không hợp lệ.");
                }
            }
            else
            {
                ed.WriteMessage("\nCó lỗi xảy ra khi gửi prompt.");
            }

        }
    }
}
