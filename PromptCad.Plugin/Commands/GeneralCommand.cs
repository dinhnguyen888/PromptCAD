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

        //private static System.EventHandler _spinnerHandler;
        //private static ObjectId _spinnerTextId = ObjectId.Null;
        //private static ObjectId _spinnerArcOuterId = ObjectId.Null;
        //private static ObjectId _spinnerArcInnerId = ObjectId.Null;
        //private static Point3d _spinnerCenter = Point3d.Origin;

        //[CommandMethod("LOADINGSPINNER")]
        //public void CreateLoadingSpinner()
        //{
        //    Document doc = Application.DocumentManager.MdiActiveDocument;
        //    Database db = doc.Database;
        //    Editor ed = doc.Editor;

        //    var ppo = new PromptPointOptions("\nChọn vị trí hiển thị spinner: ");
        //    var ppr = ed.GetPoint(ppo);
        //    if (ppr.Status != PromptStatus.OK)
        //    {
        //        ed.WriteMessage("\nHủy.");
        //        return;
        //    }
        //    _spinnerCenter = ppr.Value;

        //    using (var docLock = doc.LockDocument())
        //    using (Transaction tr = db.TransactionManager.StartTransaction())
        //    {
        //        BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
        //        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

        //        // --- Text Loading ---
        //        var text = new DBText
        //        {
        //            Position = _spinnerCenter,
        //            Height = 2.5,
        //            TextString = "Loading...",
        //            HorizontalMode = TextHorizontalMode.TextCenter,
        //            VerticalMode = TextVerticalMode.TextVerticalMid,
        //            AlignmentPoint = _spinnerCenter
        //        };
        //        _spinnerTextId = btr.AppendEntity(text);
        //        tr.AddNewlyCreatedDBObject(text, true);

        //        // --- Tạo cung ngoài ---
        //        double outerRadius = 12.0;
        //        double innerRadius = 9.0;
        //        double startAngle = 0;
        //        double endAngle = Math.PI * 1.5; // 270 độ

        //        var arcOuter = new Arc(_spinnerCenter, outerRadius, startAngle, endAngle);
        //        arcOuter.ColorIndex = 3;
        //        _spinnerArcOuterId = btr.AppendEntity(arcOuter);
        //        tr.AddNewlyCreatedDBObject(arcOuter, true);

        //        // --- Tạo cung trong (offset vào trong) ---
        //        var arcInner = new Arc(_spinnerCenter, innerRadius, startAngle, endAngle);
        //        arcInner.ColorIndex = 3;
        //        _spinnerArcInnerId = btr.AppendEntity(arcInner);
        //        tr.AddNewlyCreatedDBObject(arcInner, true);

        //        tr.Commit();
        //    }

        //    StartSpinnerIdleLoop(doc, db, ed);
        //}

        //[CommandMethod("STOPSPINNER")]
        //public void StopLoadingSpinner()
        //{
        //    if (_spinnerHandler != null)
        //    {
        //        Autodesk.AutoCAD.ApplicationServices.Application.Idle -= _spinnerHandler;
        //        _spinnerHandler = null;
        //    }

        //    var doc = Application.DocumentManager.MdiActiveDocument;
        //    var db = doc.Database;
        //    using (doc.LockDocument())
        //    using (var tr = db.TransactionManager.StartTransaction())
        //    {
        //        EraseIfExists(tr, _spinnerTextId);
        //        EraseIfExists(tr, _spinnerArcOuterId);
        //        EraseIfExists(tr, _spinnerArcInnerId);
        //        tr.Commit();
        //    }

        //    _spinnerTextId = ObjectId.Null;
        //    _spinnerArcOuterId = ObjectId.Null;
        //    _spinnerArcInnerId = ObjectId.Null;
        //}

        //private void StartSpinnerIdleLoop(Document doc, Database db, Editor ed)
        //{
        //    if (_spinnerHandler != null) return;

        //    double stepDeg = 0.5; // quay siêu chậm (0.5° mỗi Idle tick)

        //    _spinnerHandler = (s, e) =>
        //    {
        //        try
        //        {
        //            using (doc.LockDocument())
        //            using (Transaction tr2 = db.TransactionManager.StartTransaction())
        //            {
        //                var arcOuter = tr2.GetObject(_spinnerArcOuterId, OpenMode.ForWrite, false) as Arc;
        //                var arcInner = tr2.GetObject(_spinnerArcInnerId, OpenMode.ForWrite, false) as Arc;

        //                if (arcOuter != null && arcInner != null)
        //                {
        //                    Matrix3d rot = Matrix3d.Rotation(stepDeg * Math.PI / 180.0, Vector3d.ZAxis, _spinnerCenter);
        //                    arcOuter.TransformBy(rot);
        //                    arcInner.TransformBy(rot);
        //                }

        //                tr2.Commit();
        //            }
        //            ed.Regen();
        //        }
        //        catch
        //        {
        //            Autodesk.AutoCAD.ApplicationServices.Application.Idle -= _spinnerHandler;
        //            _spinnerHandler = null;
        //        }
        //    };
        //    Autodesk.AutoCAD.ApplicationServices.Application.Idle += _spinnerHandler;
        //}

        //private void EraseIfExists(Transaction tr, ObjectId id)
        //{
        //    if (id == ObjectId.Null) return;
        //    var ent = tr.GetObject(id, OpenMode.ForWrite, false) as Entity;
        //    if (ent != null && !ent.IsErased)
        //    {
        //        ent.Erase();
        //    }
        //}

    }
}

