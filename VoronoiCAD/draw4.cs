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

using TriangleNet;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TriangleNet.Voronoi;
using TriangleNet.Meshing.Iterators;
using System.Linq;
using ABOS2;

namespace VoronoiCAD
{
    public class draw4
    {
        public static void Abos1()
        {
            //List<CForPileCell> vSvai = new List<CForPileCell>();
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor acDocEd = acDoc.Editor;
            Database acDocDb = acDoc.Database;

            var p = new Polygon();//точки полигона для триангуляции
            List<XYZ> pts = new List<XYZ>();//базовые точки для Abos интерполяции
            double x1 = 0;
            double x2 = 0;
            double y1 = 0;
            double y2 = 0;
            double dx = 0;
            double dy = 0;
            double delta = 0;




            PromptSelectionResult acSSPrompt = acDocEd.GetSelection();
            if (acSSPrompt.Status == PromptStatus.OK)
            {
                SelectionSet acSSet = acSSPrompt.Value;
                if (acSSet.Count > 0)
                {
                    using (Transaction tr = acDocDb.TransactionManager.StartTransaction())
                    {
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject
                           (SymbolUtilityServices.GetBlockModelSpaceId(acDocDb), OpenMode.ForRead);
                        int marker = 0;
                        foreach (ObjectId id in acSSet.GetObjectIds())
                        {
                            marker++;
                            Entity currentEntity = tr.GetObject(id, OpenMode.ForWrite, false) as Entity;

                            if (currentEntity is Polyline)
                            {
                                if (currentEntity.Layer == "Contour")
                                {
                                    int vn = ((Polyline)currentEntity).NumberOfVertices;
                                    List<TriangleNet.Geometry.Vertex> vertexList = new List<TriangleNet.Geometry.Vertex>();
                                    for (int i = 0; i < vn; i++)

                                    {



                                        Point2d pt = ((Polyline)currentEntity).GetPoint2dAt(i);
                                        vertexList.Add(new TriangleNet.Geometry.Vertex(pt.X, pt.Y, marker));

                                        //acDocEd.WriteMessage("\n" + pt.ToString());

                                    }
                                    //if (currentEntity.Layer == "Contour")
                                        p.Add(new Contour(vertexList, marker));

                                    var bbox = currentEntity.Bounds;
                                    x1 = bbox.Value.MinPoint.X;
                                    x2 = bbox.Value.MaxPoint.X;
                                    y1 = bbox.Value.MinPoint.Y;
                                    y2 = bbox.Value.MaxPoint.Y;
                                }
                            }

                            if (currentEntity is BlockReference)
                            {                         
                                var acBlock = currentEntity as BlockReference;
                                if (currentEntity.Layer == "StepXY")
                                {
                                    foreach (ObjectId attId in acBlock.AttributeCollection)
                                    {
                                        var acAtt = tr.GetObject(attId, OpenMode.ForRead) as AttributeReference;
                                        if (acAtt == null) continue;

                                        if (acAtt.Tag.Equals("DX", StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            acAtt.UpgradeOpen();
                                            dx = Convert.ToDouble(acAtt.TextString);
                                        }
                                        if (acAtt.Tag.Equals("DY", StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            acAtt.UpgradeOpen();
                                            dy = Convert.ToDouble(acAtt.TextString);
                                        }
                                        if (acAtt.Tag.Equals("DELTA", StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            acAtt.UpgradeOpen();
                                            delta = Convert.ToDouble(acAtt.TextString);
                                        }


                                    }
                                }

                                if (currentEntity.Layer == "BasePt")
                                {
                                    double x = ((BlockReference)currentEntity).Position.X;
                                    double y = ((BlockReference)currentEntity).Position.Y;
                                    double z = 0;

                                    foreach (ObjectId attId in acBlock.AttributeCollection)
                                    {
                                        var acAtt = tr.GetObject(attId, OpenMode.ForRead) as AttributeReference;
                                        if (acAtt == null) continue;

                                        if (acAtt.Tag.Equals("Z", StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            acAtt.UpgradeOpen();
                                            z = Convert.ToDouble(acAtt.TextString);
                                        }
                                        pts.Add(new XYZ(x, y, z));
                                        


                                    }
                                }
                            }
                        }
                        tr.Commit();
                    }
                }

                //Application.ShowAlertDialog("Number of objects selected: " +
                //                            acSSet.Count.ToString());

            }
            CdMatrix m1 = CAbos.getAbosMatrix(pts, x1, x2, y1, y2, dx, dy, delta);
            
            List<Point3d> AbosPts = new List<Point3d>();
            for(int i=0;i<m1.size1();i++)
            {
                for(int j=0;j<m1.size2();j++)
                {
                    AbosPts.Add(new Point3d(x1+i*dx,y1+j*dy,m1[i,j]));
                }
            }
            if (acSSPrompt.Status == PromptStatus.OK)
            {
                SelectionSet acSSet = acSSPrompt.Value;
                if (acSSet.Count > 0)
                {
                    using (Transaction tr = acDocDb.TransactionManager.StartTransaction())
                    {
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject
                           (SymbolUtilityServices.GetBlockModelSpaceId(acDocDb), OpenMode.ForRead);

                        foreach (ObjectId id in acSSet.GetObjectIds())
                        {

                            Entity currentEntity = tr.GetObject(id, OpenMode.ForWrite, false) as Entity;

                           


                            if (currentEntity is BlockReference)
                            {

                                
                                if (currentEntity.Layer == "AbosPt")
                                {
                                  
                                    var sourceBlock = currentEntity as BlockReference;

                                    foreach (var pt in AbosPts)
                                    {
                                        BlockReference block = sourceBlock.Clone() as BlockReference;

                                        Point3d acPt3d = sourceBlock.Position;
                                        Vector3d acVec3d = acPt3d.GetVectorTo(pt);

                                        block.TransformBy(Matrix3d.Displacement(acVec3d));
                                       
                                        btr.UpgradeOpen();
                                        btr.AppendEntity(block);
                                        tr.AddNewlyCreatedDBObject(block, true);
                                        foreach (ObjectId attId in block.AttributeCollection)
                                        {
                                            var acAtt = tr.GetObject(attId, OpenMode.ForRead) as AttributeReference;
                                            if (acAtt == null) continue;

                                            if (acAtt.Tag.Equals("Z", StringComparison.CurrentCultureIgnoreCase))
                                            {
                                                acAtt.UpgradeOpen();
                                                acAtt.TextString = CAbos.Round(pt.Z, 0.01).ToString();
                                            }

                                        }

                                    }



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