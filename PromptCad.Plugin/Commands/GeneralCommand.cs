using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

using Autodesk.AutoCAD.Runtime;
using Newtonsoft.Json.Linq;
using PromptCad.Plugin.Utility;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AutoCADGeminiPlugin
{
    public class Commands : IExtensionApplication
    {
        public async void Initialize() {
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\n Đang Load Plugin!\n");
            // Check API Key status
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nKiểm tra API Key!\n");
            var (isConnected, errorMessage) = PromptCad.Plugin.Utility.checkAPIKey.InitAPIKeyStatus();
            if (!isConnected)
            {
                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"\nLỗi: {errorMessage}");
                return;
            }
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nAPI Key đã được nạp thành công.\n");
            // Check sessionToken
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nKiểm tra Session Token!\n");
            //get session token from file
            var file = globalAPI.ApiKeyFilePath;
            if (!File.Exists(file))
            {
                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nKhông tìm thấy file Json.");
                return;
            }
            string sessionToken = ReadAPIKeyFile.GetObjectJson("SessionToken");
            var checkSession = await PromptCad.Plugin.Utility.CheckSessionToken.IsSessionTokenValid(sessionToken);
            if (checkSession == false)
            {
               
                string apiKeyFilePath = globalAPI.ApiKeyFilePath;
                if (!File.Exists(apiKeyFilePath))
                {
                    Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nKhông tìm thấy file API Key.");
                    return;
                }
              
                string apiKey = ReadAPIKeyFile.GetObjectJson("ApiKey");
                if (string.IsNullOrEmpty(apiKey))
                {
                    Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nAPI Key không hợp lệ.");
                    return;
                }
                // try to call service to push into server
                var apiService = new PromptCad.Plugin.Services.APIServices.APIServices();
                bool isSuccess = await apiService.PostKeyToServerAsync(apiKey);
                if (isSuccess)
                {
                    Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nAPI Key đã được gửi lại thành công.");
                }
                else
                {
                    Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nKhông thể gửi lại API Key, vui lòng thử lại sau.");
                    return;

                }
     
            }
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nToken ok!\n");

        }


        public void Terminate() { }

        //[CommandMethod("PROMPTS")]
        //public async void PromptCommand()
        //{
        //    var ed = Application.DocumentManager.MdiActiveDocument.Editor;
        //    var opts = new PromptStringOptions("\nEnter your prompt: ") { AllowSpaces = true };
        //    PromptResult result = ed.GetString(opts);

        //    if (result.Status != PromptStatus.OK) return;

        //    string userPrompt = result.StringResult;
        //    string apiKey = File.ReadAllText(@"D:\WORKSPACE\PROJECT\LLM_CAD\APiKey.txt").Trim();

        //    // Gọi API nhưng phải tránh await trực tiếp vì prompt là đồng bộ
        //    string response = await CallGeminiAPI(apiKey, userPrompt);

        //    ed.WriteMessage($"\nGemini says: {response}");
        //}

        //private async Task<string> CallGeminiAPI(string apiKey, string prompt)
        //{
        //    using var client = new HttpClient();
        //    var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";
        //    var payload = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
        //    var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        //    var resp = await client.PostAsync(url, content);
        //    var str = await resp.Content.ReadAsStringAsync();
        //    var j = JObject.Parse(str);
        //    return j["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString() ?? "No response.";
        //}

        //[CommandMethod("DrawStar")]
        //public void DrawStar()
        //{
        //    Document doc = Application.DocumentManager.MdiActiveDocument;
        //    Editor ed = doc.Editor;
        //    Database db = doc.Database;

        //    // Nhập bán kính ngoài
        //    PromptDoubleOptions outerOpt = new PromptDoubleOptions("\nNhập bán kính ngoài của ngôi sao: ");
        //    outerOpt.DefaultValue = 10;
        //    PromptDoubleResult outerRes = ed.GetDouble(outerOpt);
        //    if (outerRes.Status != PromptStatus.OK) return;
        //    double R = outerRes.Value;

        //    // Nhập bán kính trong
        //    PromptDoubleOptions innerOpt = new PromptDoubleOptions("\nNhập bán kính trong của ngôi sao: ");
        //    innerOpt.DefaultValue = R / 2.5;
        //    PromptDoubleResult innerRes = ed.GetDouble(innerOpt);
        //    if (innerRes.Status != PromptStatus.OK) return;
        //    double r = innerRes.Value;

        //    // Tính các điểm ngôi sao
        //    Point2d center = new Point2d(0, 0);
        //    Point2dCollection points = new Point2dCollection();

        //    for (int i = 0; i < 10; i++)
        //    {
        //        double angle = Math.PI / 5 * i;
        //        double radius = (i % 2 == 0) ? R : r;

        //        double x = center.X + radius * Math.Cos(angle);
        //        double y = center.Y + radius * Math.Sin(angle);
        //        points.Add(new Point2d(x, y));
        //    }

        //    // Vẽ ngôi sao
        //    using (Transaction tr = db.TransactionManager.StartTransaction())
        //    {
        //        BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
        //        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);

        //        Polyline star = new Polyline();
        //        for (int i = 0; i < points.Count; i++)
        //        {
        //            star.AddVertexAt(i, points[i], 0, 0, 0);
        //        }
        //        star.Closed = true;

        //        btr.AppendEntity(star);
        //        tr.AddNewlyCreatedDBObject(star, true);
        //        tr.Commit();
        //    }
        //}
    }
}
