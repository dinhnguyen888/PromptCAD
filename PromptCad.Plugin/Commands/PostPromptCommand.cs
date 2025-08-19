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

            // Chọn trước điểm đặt
            var pointOpts = new PromptPointOptions("\nChọn điểm đặt (insertion point): ");
            var pointResult = ed.GetPoint(pointOpts);
            if (pointResult.Status != PromptStatus.OK)
            {
                ed.WriteMessage("\nLệnh bị hủy.");
                return;
            }
            Point3d insertionPoint = pointResult.Value;

            // Hiển thị chữ Loading nhấp nháy
            ObjectId loadingTextId = await ShowLoadingText(insertionPoint);

            // Check Internet connection
            var isConnected = Utility.checkAPIKey.IsInternetConnected();
            if (!isConnected)
            {
                ed.WriteMessage("\nKhông có kết nối internet.");
                EraseEntity(loadingTextId);
                return;
            }

            // Get session token from file
            string sessionToken = ReadAPIKeyFile.GetObjectJson("SessionToken");
            if (string.IsNullOrEmpty(sessionToken))
            {
                ed.WriteMessage("\nSession Token không hợp lệ.");
                EraseEntity(loadingTextId);
                return;
            }

            // Ghép toạ độ điểm chèn vào sau prompt để gửi lên server
            string coordText = "<!-- INSERTION_POINT: X="
            + insertionPoint.X.ToString(System.Globalization.CultureInfo.InvariantCulture)
            + ", Y=" + insertionPoint.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)
            + ", Z=" + insertionPoint.Z.ToString(System.Globalization.CultureInfo.InvariantCulture)
            + " -->";
            string promptWithCoords = userPrompt + " " + coordText;

            // Gọi API server
            PromptResponse response = null;
            try
            {
                var apiService = new Services.APIServices.APIServices();
                response = await apiService.PostPromptService(promptWithCoords);
            }
            finally
            {
                // Xóa chữ Loading sau khi có phản hồi
                EraseEntity(loadingTextId);
            }

            if (response != null)
            {
                var processData = new ProcessDataServices();
                if (response.type_response == "text")
                {
                    processData.ProcessTextResponse(response, insertionPoint);
                }
                else if (response.type_response == "object")
                {
                    processData.ProcessDrawResponse(response, insertionPoint);
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

        private async Task<ObjectId> ShowLoadingText(Point3d insertionPoint)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            ObjectId textId = ObjectId.Null;

            using (var docLock = doc.LockDocument())
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                var btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                DBText text = new DBText
                {
                    Position = insertionPoint,
                    Height = 5, // chiều cao chữ
                    TextString = "Loading...",
                    ColorIndex = 1 // đỏ ban đầu
                };

                textId = btr.AppendEntity(text);
                tr.AddNewlyCreatedDBObject(text, true);
                tr.Commit();
            }

            // Chạy task đổi màu liên tục
            _ = Task.Run(async () =>
            {
                int[] colors = { 1, 3, 2, 5 }; // 1=red, 3=green, 2=yellow, 5=purple
                int index = 0;

                while (!textId.IsNull && textId.IsValid)
                {
                    try
                    {
                        using (var docLock = doc.LockDocument())
                        using (var tr = db.TransactionManager.StartTransaction())
                        {
                            var ent = tr.GetObject(textId, OpenMode.ForWrite, false) as DBText;
                            if (ent != null)
                            {
                                ent.ColorIndex = colors[index % colors.Length];
                            }
                            tr.Commit();
                        }
                        index++;
                        await Task.Delay(1000);
                    }
                    catch { break; }
                }
            });

            return textId;
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
                if (obj != null)
                {
                    obj.Erase();
                }
                tr.Commit();
            }
        }
    }
}
