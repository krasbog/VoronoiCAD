// system 
using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;


// ODA
using Teigha.Runtime;
using Teigha.DatabaseServices;
using Teigha.Geometry;

// Bricsys
using Bricscad.ApplicationServices;
using Bricscad.Runtime;
using Bricscad.EditorInput;
using Bricscad.Ribbon;

// com
//using BricscadDb;
//using BricscadApp;

// alias
using _AcRx = Teigha.Runtime;
using _AcAp = Bricscad.ApplicationServices;
using _AcDb = Teigha.DatabaseServices;
using _AcGe = Teigha.Geometry;
using _AcEd = Bricscad.EditorInput;
using _AcGi = Teigha.GraphicsInterface;
using _AcClr = Teigha.Colors;
using _AcWnd = Bricscad.Windows;

using System.Linq;

namespace VoronoiCAD
{
    public class draw3
    {

        public static void b2b()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor acDocEd = acDoc.Editor;
            Database acDocDb = acDoc.Database;

            var pso0 = new PromptSelectionOptions();
            pso0.SingleOnly = true;
            pso0.SinglePickInSpace = true;
            pso0.MessageForAdding = "\nВыбрать блок-источник ";

            TypedValue[] acTypValAr = new TypedValue[1];
            acTypValAr.SetValue(new TypedValue((int)DxfCode.Start, "INSERT"), 0);
            SelectionFilter acSelFtr = new SelectionFilter(acTypValAr);


            PromptSelectionResult psr0;

            while (true)
            {
                psr0 = acDocEd.GetSelection(pso0, acSelFtr);
                if (psr0.Status == PromptStatus.OK)
                    break;
                else acDocEd.WriteMessage("\nБлок-источник не выбран. Повторить ");
            }

            var pso2 = new PromptSelectionOptions();
            pso2.MessageForAdding = "\nВыбрать заменяемые блоки ";

            PromptSelectionResult acSSPrompt = acDocEd.GetSelection(pso2, acSelFtr);
            if (acSSPrompt.Status == PromptStatus.OK)
            {
                SelectionSet acSSet = acSSPrompt.Value;
                if (acSSet.Count > 0)
                {
                    using (Transaction tr = acDocDb.TransactionManager.StartTransaction())
                    {
                        List<Point3d> pts = new List<Point3d>();
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject
                         (SymbolUtilityServices.GetBlockModelSpaceId(acDocDb), OpenMode.ForRead);



                        foreach (ObjectId id in acSSet.GetObjectIds())
                        {
                            Entity currentEntity = tr.GetObject(id, OpenMode.ForWrite, false) as Entity;
                            if (currentEntity is BlockReference)
                            {
                                pts.Add(((BlockReference)currentEntity).Position);
                                currentEntity.Erase(true);
                            }
                        }



                        foreach (ObjectId id in psr0.Value.GetObjectIds())
                        {
                            Entity currentEntity = tr.GetObject(id, OpenMode.ForWrite, false) as Entity;
                            if (currentEntity is BlockReference)
                            {
                                BlockReference sourceBlock = (BlockReference)currentEntity;
                               
                                foreach (var pt in pts)
                                {
                                    BlockReference block = sourceBlock.Clone() as BlockReference;
                                    
                                    Point3d acPt3d = sourceBlock.Position;
                                    Vector3d acVec3d = acPt3d.GetVectorTo(pt);

                                    block.TransformBy(Matrix3d.Displacement(acVec3d));
                                    
                                        btr.UpgradeOpen();
                                    btr.AppendEntity(block);
                                    tr.AddNewlyCreatedDBObject(block, true);

                                }


                            }
                        }


                        tr.Commit();
                    }
                }
            }
        }
    }
}













         