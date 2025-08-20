using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using PromptCad.Plugin.Models;
using PromptCad.Plugin.Services.ProcessDataServices;
using PromptCad.Plugin.Utility;
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

            // Chọn điểm đặt
            var pointOpts = new PromptPointOptions("\nChọn điểm đặt (insertion point): ");
            var pointResult = ed.GetPoint(pointOpts);
            if (pointResult.Status != PromptStatus.OK)
            {
                ed.WriteMessage("\nLệnh bị hủy.");
                return;
            }
            Point3d insertionPoint = pointResult.Value;

            // Hiển thị chữ "Loading..." màu xanh lá
            ObjectId loadingTextId = ObjectId.Null;
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            using (var docLock = doc.LockDocument())
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                var btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                var dbText = new DBText
                {
                    Position = insertionPoint,
                    Height = 1.5,
                    TextString = "Load...",
                    ColorIndex = 3 // 3 = green
                };

                loadingTextId = btr.AppendEntity(dbText);
                tr.AddNewlyCreatedDBObject(dbText, true);
                tr.Commit();
            }

            // Check Internet connection
            var isConnected = Utility.checkAPIKey.IsInternetConnected();
            if (!isConnected)
            {
                ed.WriteMessage("\nKhông có kết nối internet.");
                EraseEntity(loadingTextId);
                return;
            }

            // Lấy Session Token
            string sessionToken = ReadAPIKeyFile.GetObjectJson("SessionToken");
            if (string.IsNullOrEmpty(sessionToken))
            {
                ed.WriteMessage("\nSession Token không hợp lệ.");
                EraseEntity(loadingTextId);
                return;
            }

            // Ghép toạ độ vào prompt
            string coordText = $"<!-- INSERTION_POINT: X={insertionPoint.X}, Y={insertionPoint.Y}, Z={insertionPoint.Z} -->";
            string promptWithCoords = userPrompt + " " + coordText;

            // Gọi API
            PromptResponse response = null;
            try
            {
                var apiService = new Services.APIServices.APIServices();
                response = await apiService.PostPromptService(promptWithCoords);
            }
            finally
            {
                // Xóa chữ "Loading..."
                EraseEntity(loadingTextId);
            }

            if (response != null)
            {
                var processData = new ProcessDataServices();
                if (response.type_response == "text")
                    processData.ProcessTextResponse(response, insertionPoint);
                else if (response.type_response == "object")
                    processData.ProcessObjectResponse(response, insertionPoint);
                else
                    ed.WriteMessage("\nLoại phản hồi không hợp lệ.");
            }
            else
            {
                ed.WriteMessage("\nCó lỗi xảy ra khi gửi prompt.");
            }
        }

        private void EraseEntity(ObjectId entityId)
        {
            if (entityId == ObjectId.Null) return;
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            using (var docLock = doc.LockDocument())
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var obj = tr.GetObject(entityId, OpenMode.ForWrite, false) as Entity;
                if (obj != null) obj.Erase();
                tr.Commit();
            }
        }
    }
}
