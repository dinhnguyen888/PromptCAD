using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Newtonsoft.Json.Linq;
using PromptCad.Plugin.Models;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
namespace PromptCad.Plugin.Commands
{
    public class InitAPIKeyCommand
    {
        [CommandMethod("PC_INIT_KEY")]
        public async void InitKeyCommand() 
        {
            var apiKey = new PromptStringOptions("\nNhập API của bạn, chưa có xin liên hệ với admin: ") { AllowSpaces = true };
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptResult result = ed.GetString(apiKey);
            if (result.Status != PromptStatus.OK)
            {
                ed.WriteMessage("\nLệnh bị hủy.");
               
            }
            string key = result.StringResult.Trim();
            if (string.IsNullOrEmpty(key))
            {
                ed.WriteMessage("\nAPI Key không được để trống.");
               
            }

            // Kiểm tra kết nối Internet
            var isConnected = Utility.checkAPIKey.IsInternetConnected();
            if (!isConnected)
            {
                ed.WriteMessage($"\nLỗi rồi ní");

            }

            // Gọi dịch vụ để gửi API Key lên server
            var apiService = new Services.APIServices.APIServices();
            bool isSuccess = await apiService.PostKeyToServerAsync(key);
            if (isSuccess)
            {
                ed.WriteMessage("\nAPI Key đã được nạp thành công.");

            }
            else
            {
                ed.WriteMessage("\nKhông thể nạp API Key, vui lòng thử lại sau.");

            }



        }
    }
}
