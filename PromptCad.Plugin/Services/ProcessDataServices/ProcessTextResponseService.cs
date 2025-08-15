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
        public void ProcessTextResponse(PromptResponse response)
        {
            try
            {
                if (response == null)
                {
                    throw new ArgumentNullException(nameof(response), "Response cannot be null");
                }
                // Check if the response type is text
                if (response.type_response.Equals("text", StringComparison.OrdinalIgnoreCase))
                {
                    var doc = Application.DocumentManager.MdiActiveDocument;
                    var db = doc.Database;
                    var ed = doc.Editor;

                    // Lock document trước khi thay đổi
                    using (var docLock = doc.LockDocument())
                    {
                        var opts = new PromptPointOptions("\nSelect insertion point for MText: ");
                        var pointResult = ed.GetPoint(opts);
                        if (pointResult.Status != PromptStatus.OK)
                        {
                            ed.WriteMessage("\nCommand cancelled.");
                            return;
                        }
                        Point3d insertionPoint = pointResult.Value;

                        using (var tr = db.TransactionManager.StartTransaction())
                        {
                            var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                            var btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                            var mtext = new MText
                            {
                                Contents = response.result,
                                Location = insertionPoint,
                                TextHeight = 2.5,
                                Attachment = AttachmentPoint.TopLeft
                            };

                            btr.AppendEntity(mtext);
                            tr.AddNewlyCreatedDBObject(mtext, true);

                            tr.Commit();
                        }

                        ed.Regen();
                    }



                }
                else
                {
                    throw new InvalidOperationException("Unsupported response type");
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                // Handle exceptions and display error messages
                var ed = Application.DocumentManager.MdiActiveDocument.Editor;
                ed.WriteMessage($"\nError processing response: {ex.Message}");
            }



        }
    }
}
