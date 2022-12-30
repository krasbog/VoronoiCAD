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

namespace VoronoiCAD
{
    public class draw2
    {
        public static void fDr2()
        {
            //List<Point2d> vSvai = new List<Point2d>();
            List<CForPileCell> vSvai = new List<CForPileCell>();
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor acDocEd = acDoc.Editor;
            Database acDocDb = acDoc.Database;

            var p = new Polygon();




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
                            //if (currentEntity.GetType().IsSubclassOf(typeof(Teigha.DatabaseServices.Polyline)))
                                if (currentEntity is Polyline)
                            {
                                int vn = ((Polyline)currentEntity).NumberOfVertices;
                                List<TriangleNet.Geometry.Vertex> vertexList = new List<TriangleNet.Geometry.Vertex>();
                                for (int i = 0; i < vn; i++)

                                {

                                   

                                    Point2d pt = ((Polyline)currentEntity).GetPoint2dAt(i);
                                    vertexList.Add(new TriangleNet.Geometry.Vertex(pt.X, pt.Y, marker));

                                    //acDocEd.WriteMessage("\n" + pt.ToString());

                                }
                                if (currentEntity.Layer == "Contour")
                                    p.Add(new Contour(vertexList, marker));
                                if (currentEntity.Layer == "Hole")
                                    p.Add(new Contour(vertexList, marker));

                            }

                            if (currentEntity is BlockReference)
                            {
                               
                                //BlockTableRecord block = null;
                                //block = tr.GetObject(((BlockReference)currentEntity).BlockTableRecord,
                                //OpenMode.ForRead) as BlockTableRecord;

                                double x = ((BlockReference)currentEntity).Position.X;
                                double y = ((BlockReference)currentEntity).Position.Y;
                                double d = 0;
                                double GammaC = 0;
                                var acBlock = currentEntity as BlockReference;
                                foreach (ObjectId attId in acBlock.AttributeCollection)
                                {
                                    var acAtt = tr.GetObject(attId, OpenMode.ForRead) as AttributeReference;
                                    if (acAtt == null) continue;

                                    if (acAtt.Tag.Equals("D", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        acAtt.UpgradeOpen();
                                       d = Convert.ToDouble(acAtt.TextString);
                                    }
                                    if (acAtt.Tag.Equals("GAMMAC", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        acAtt.UpgradeOpen();
                                        GammaC = Convert.ToDouble(acAtt.TextString);
                                    }

                                }


                                if (currentEntity.Layer == "Vertex")
                                { p.Add(new TriangleNet.Geometry.Vertex(x, y, marker));
                                    CForPileCell fpc = new CForPileCell();
                                    fpc.pt = new Point2d(x, y);
                                    fpc.d = d;
                                    fpc.GammaC = GammaC;
                                    vSvai.Add(fpc);
                                }
                                    
                                if (currentEntity.Layer == "Hole")
                                    p.Holes.Add(new TriangleNet.Geometry.Point(x, y));
                                
                            }
                            }
                        tr.Commit();
                    }
                }

                //Application.ShowAlertDialog("Number of objects selected: " +
                //                            acSSet.Count.ToString());

            }
           
            Mesh mesh = p.Triangulate() as Mesh;




            //foreach (var triang in mesh.Triangles)
            //{

            //    DatabaseCAD.do_addPolyLine(true,
            //       new Point2d(triang.GetVertex(0).X, triang.GetVertex(0).Y),
            //       new Point2d(triang.GetVertex(1).X, triang.GetVertex(1).Y),
            //       new Point2d(triang.GetVertex(2).X, triang.GetVertex(2).Y)
            //        );
            //}
            var voronoi = new StandardVoronoi(mesh);
            //var voronoi = new BoundedVoronoi(mesh);
            List<double> AlphaXs = new List<double>();
            List<List<TriangleNet.Topology.DCEL.Face>> Li = new List<List<TriangleNet.Topology.DCEL.Face>>();
            List<CPileCell> PileCells = new List<CPileCell>();
            foreach (var face in voronoi.Faces)
            {
                List<Point2d> ptList = DatabaseCAD.getFacePolygon(face);
                
               


                // DatabaseCAD.do_addPolyLine(true, ptList);
                Point2d pt0= new Point2d();
                foreach (var pt in vSvai)
                {
                    if (DatabaseCAD.IsPointInFace(pt.pt, face))
                    {
                        DatabaseCAD.do_addPolyLine(true, ptList);
                        pt0 = pt.pt;
                        break;
                    }
                }
                

                
                
                //CPileCell PileCell = new CPileCell();
                //foreach (var pt in vSvai)
                //{
                //    if (DatabaseCAD.IsPointInFace(pt, face))
                //    {

                //        PileCell.ptPile = pt;
                //        break;
                //    }
                //}
                List<TriangleNet.Topology.DCEL.Face> neighborFaces2 = DatabaseCAD.getNeighborFaces2(face);

                //if (DatabaseCAD.IsPointInFace(new Point2d(0,0), face))
                //{
                //    foreach (var face_i in neighborFaces2)
                //    {
                //        List<Point2d> ptList_i = DatabaseCAD.getFacePolygon(face_i);
                //        DatabaseCAD.do_addPolyLine(true, ptList_i);
                //    }

                //}





                CPileCell PileCell = new CPileCell();
                bool isPileCell = false;
                foreach (var pt in vSvai)
                    
                    {
                    if (DatabaseCAD.IsPointInFace(pt.pt, face))
                    {
                        PileCell.ptPile = pt.pt;
                        PileCell.d = pt.d;
                        PileCell.GammaC = pt.GammaC;
                        _AcGe.PolylineCurve2d ge_pl = new PolylineCurve2d
                    (new Point2dCollection(ptList.ToArray()));
                     double area = ge_pl.GetArea(ge_pl.StartParameter, ge_pl.EndParameter);
                        PileCell.Area = area;
                        PileCell.Radius = Math.Sqrt(area / Math.PI);

                        isPileCell = true;
                    }
                        foreach (var face_i in neighborFaces2)
                    {

                        if (DatabaseCAD.IsPointInFace(pt.pt, face_i))
                        {
                            bool isNoDublicate = true;
                            foreach (var pti in PileCell.ptNeighborPiles)
                            {
                                if (pt.pt.IsEqualTo(pti)) isNoDublicate = false;
                            }
                            
                            if(isNoDublicate)
                            PileCell.ptNeighborPiles.Add(pt.pt);


                        }
                    }
                }
                if (isPileCell) PileCells.Add(PileCell);






                }
            string str = null;
            foreach (var pc in PileCells)
            {
                double AlphaiX = pc.GammaC;
                double d = pc.d/1000;
                foreach(var npc in pc.ptNeighborPiles)
                {
                    Point2d pt0 = pc.ptPile;
                    Point2d pti = npc;
                    AlphaiX *= DatabaseCAD.getAlphaiX(pt0.X / 1000, pt0.Y / 1000, d, pti.X / 1000, pti.Y / 1000);

                }

                pc.AlphaiX = AlphaiX;
                str += "\n" + AlphaiX.ToString();



            }
             //System.Windows.Forms.MessageBox.Show(str);



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

                            if (currentEntity is MText)
                            {
                                MText myText = (MText)currentEntity;
                                int numPiles = PileCells.Count;
                                double minAlphaiX = PileCells.Min(x => x.AlphaiX);
                                double maxAlphaiX = PileCells.Max(x => x.AlphaiX);
                                double sumAlphaiX = PileCells.Sum(x => x.AlphaiX);
                                double averAlphaiX = sumAlphaiX / numPiles;
                                double sumArea = PileCells.Sum(x => x.Area);

                                string strRes = null;
                                strRes += "1. Число свай - " + numPiles.ToString();
                                strRes += "\\P2 .Жесткость - ";
                                strRes += Math.Round(minAlphaiX, 4).ToString();
                                strRes += "...";
                                strRes += Math.Round(maxAlphaiX, 4).ToString();

                                

                                strRes += "\\P3. Сред.жесткость - " + Math.Round(averAlphaiX, 4).ToString();
                                strRes += "\\P4. Сумм.жесткость - " + Math.Round(sumAlphaiX,4).ToString();
                                strRes += "\\P5. Площ.полигонов - " + Math.Round((sumArea/1000000),4).ToString() + " м2";
                               
                                myText.Contents = strRes;








                            }


                                if (currentEntity is BlockReference)
                            {

                                double x = ((BlockReference)currentEntity).Position.X;
                                double y = ((BlockReference)currentEntity).Position.Y;
                                if (currentEntity.Layer == "Vertex")
                                {
                                    var acBlock = currentEntity as BlockReference;
                                    foreach (var eachPile in PileCells)
                                    {
                                        if (eachPile.ptPile.IsEqualTo(new Point2d(x, y)))
                                        {
                                            foreach (ObjectId attId in acBlock.AttributeCollection)
                                            {
                                                var acAtt = tr.GetObject(attId, OpenMode.ForRead) as AttributeReference;
                                                if (acAtt == null) continue;

                                                if (acAtt.Tag.Equals("STIFFNESS", StringComparison.CurrentCultureIgnoreCase))
                                                {
                                                    acAtt.UpgradeOpen();
                                                    acAtt.TextString = Math.Round(eachPile.AlphaiX, 3).ToString();
                                                }
                                                if (acAtt.Tag.Equals("RADIUS", StringComparison.CurrentCultureIgnoreCase))
                                                {
                                                    acAtt.UpgradeOpen();
                                                    acAtt.TextString =Math.Round( eachPile.Radius, 0).ToString();
                                                }
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

            int iii = 1;
            //    foreach (var face in voronoi.Faces)
            //{
            //    // Get half-edge connected to face.
            //    var edge = face.Edge;
            //    // Get neighbor across current edge.
            //    var neighborFace = edge.Twin.Face;
               
                
            //      // Get the origin of first edge.
            //      var first = edge.Origin.ID;
            //    //Console.WriteLine(edge.Origin.ID);
            //    List<Point2d> ptList = new List<Point2d>();
            //    do
            //    {
                   
            //        ptList.Add(new Point2d(edge.Origin.X, edge.Origin.Y));
            //        edge = edge.Next;
            //    }
            //    while (edge != null && edge.Origin.ID != first);
            //    foreach (var pt in vSvai)
            //    {
            //       if( DatabaseCAD.IsPointInPolygon(pt, ptList))
            //        {
            //            DatabaseCAD.do_addPolyLine(true, ptList);
            //            break;
            //        }
            //    }
               
            //}
        }
    }
}