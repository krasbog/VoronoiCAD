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
    public class VoronoiPile
    {
        public static List<CPileCell> GetPileCells(SelectionSet acSSet)
        {
            if (acSSet.Count > 0)
            {

            List<CForPileCell> vSvai = new List<CForPileCell>();
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor acDocEd = acDoc.Editor;
            Database acDocDb = acDoc.Database;

            var p = new Polygon();
                //2018-04-01
                List<Kust> kusts = new List<Kust>();
                //2018-04-01





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
                            if (currentEntity.Layer == "Contour" || currentEntity.Layer == "Hole")
                            {
                                int vn = ((Polyline)currentEntity).NumberOfVertices;
                                List<TriangleNet.Geometry.Vertex> vertexList = new List<TriangleNet.Geometry.Vertex>();
                                for (int i = 0; i < vn; i++)

                                {



                                    Point2d pt = ((Polyline)currentEntity).GetPoint2dAt(i);
                                    vertexList.Add(new TriangleNet.Geometry.Vertex(pt.X, pt.Y, marker));



                                }
                                if (currentEntity.Layer == "Contour")
                                    p.Add(new Contour(vertexList, marker));
                                if (currentEntity.Layer == "Hole")
                                    p.Add(new Contour(vertexList, marker));
                            }
                            //2018-04-01
                            if (currentEntity.Layer == "Kust")
                            {
                                Kust kust = new Kust();
                                int vn = ((Polyline)currentEntity).NumberOfVertices;
                                List<Point2d> vertexList = new List<Point2d>();
                                for (int i = 0; i < vn; i++)
                                {
                                    Point2d pt = ((Polyline)currentEntity).GetPoint2dAt(i);
                                    vertexList.Add(pt);
                                }
                                kust.Pts = vertexList;
                                kusts.Add(kust);

                            }
                            //2018-04-01


                        }

                        if (currentEntity is BlockReference)
                    {



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
                        {
                            p.Add(new TriangleNet.Geometry.Vertex(x, y, marker));
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






            Mesh mesh = p.Triangulate() as Mesh;





            var voronoi = new StandardVoronoi(mesh);

            List<double> AlphaXs = new List<double>();
            List<List<TriangleNet.Topology.DCEL.Face>> Li = new List<List<TriangleNet.Topology.DCEL.Face>>();
            List<CPileCell> PileCells = new List<CPileCell>();
            foreach (var face in voronoi.Faces)
            {
                List<Point2d> ptList = DatabaseCAD.getFacePolygon(face);

                    //2018-03-30->
                    //Определение очень длинных фейсов
                    bool isVeryLongFace=false;
                    for(int i=0;i< ptList.Count-1;i++)
                    {
                        if (ptList[i].GetDistanceTo(ptList[i + 1]) > 5) { isVeryLongFace = true; break; }
                    }
                    //2018-03-30<-


                Point2d pt0 = new Point2d();
                foreach (var pt in vSvai)
                {
                    if (DatabaseCAD.IsPointInFace(pt.pt, face)
                            //2018-03-30->
                            && !isVeryLongFace
                            //2018-03-30
                            )
                    {
                        DatabaseCAD.do_addPolyLine(true, ptList);
                        pt0 = pt.pt;
                        break;
                    }
                }





                List<TriangleNet.Topology.DCEL.Face> neighborFaces2 = DatabaseCAD.getNeighborFaces2(face);
                    //List<TriangleNet.Topology.DCEL.Face> neighborFaces2 = DatabaseCAD.getNeighborFaces3(face, voronoi, 2);







                    CPileCell PileCell = new CPileCell();
                bool isPileCell = false;
                foreach (var pt in vSvai)

                {
                    if (DatabaseCAD.IsPointInFace(pt.pt, face)
                            //2018-03-30->
                            && !isVeryLongFace
                            //2018-03-30<-
                            )
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
                            //2018-03-30->
                            //Определение очень длинных фейсов
                            List<Point2d> ptList_i = DatabaseCAD.getFacePolygon(face);
                            bool isVeryLongFace_i = false;
                            for (int i = 0; i < ptList_i.Count - 1; i++)
                            {
                                if (ptList_i[i].GetDistanceTo(ptList_i[i + 1]) > 5) { isVeryLongFace_i = true; break; }
                            }
                            //2018-03-30<-

                            if (DatabaseCAD.IsPointInFace(pt.pt, face_i)
                            //2018-03-30->
                            && !isVeryLongFace_i
                            //2018-03-30<-
                            )
                        {
                                bool isNoDublicate = true;
                            foreach (var pti in PileCell.ptNeighborPiles)
                            {
                                if (pt.pt.IsEqualTo(pti)) isNoDublicate = false;
                            }

                            if (isNoDublicate)
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
                    double AlphaiY = pc.GammaC;
                    double d = pc.d / 1000;
                foreach (var npc in pc.ptNeighborPiles)
                {
                    Point2d pt0 = pc.ptPile;
                    Point2d pti = npc;
                    AlphaiX *= DatabaseCAD.getAlphaiX(pt0.X / 1000, pt0.Y / 1000, d, pti.X / 1000, pti.Y / 1000);
                    AlphaiY *= DatabaseCAD.getAlphaiX(pt0.Y / 1000, pt0.X / 1000, d, pti.Y / 1000, pti.X / 1000);

                    }

                    pc.AlphaiX = AlphaiX;
                    pc.AlphaiY = AlphaiY;
                    str += "\n" + AlphaiX.ToString();



            }
                //2018-04-01
for (int i=0; i< kusts.Count; i++)
                {
                    kusts[i].Name = i.ToString();
                }
                foreach (var pc in PileCells)
                {
                    foreach (var kust in kusts)
                    {
                        if (DatabaseCAD.IsPointInPolygon(pc.ptPile, kust.Pts)) pc.KustName = kust.Name;
                    }
                }

                    //2018-04-01



                    return PileCells;

        }
            return null;  
           
        }
        public static void DrawPileCells(SelectionSet acSSet, List<CPileCell> PileCells,
            List<ExpLayer.CPile> piles)
        {
            if (acSSet.Count > 0)
            {
                Document acDoc = Application.DocumentManager.MdiActiveDocument;
                Editor acDocEd = acDoc.Editor;
                Database acDocDb = acDoc.Database;
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
                            strRes += "\\P4. Сумм.жесткость - " + Math.Round(sumAlphaiX, 4).ToString();
                            strRes += "\\P5. Площ.полигонов - " + Math.Round((sumArea / 1000000), 4).ToString() + " м2";

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
                                            //if (acAtt.Tag.Equals("RADIUS", StringComparison.CurrentCultureIgnoreCase))
                                            //{
                                            //    acAtt.UpgradeOpen();
                                            //    acAtt.TextString = Math.Round(eachPile.Radius, 0).ToString();
                                            //}
                                            if (acAtt.Tag.Equals("RADIUS", StringComparison.CurrentCultureIgnoreCase))
                                            {
                                               
                                                foreach(var pile in piles)
                                                {
                                                    if(pile.pileCell.ptPile.IsEqualTo(eachPile.ptPile))
                                                    {
                                                        acAtt.UpgradeOpen();
                                                        acAtt.TextString = Math.Round(pile.Fd_Dopustim, 2).ToString();
                                                    }
                                                }
                                                
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
            }
}