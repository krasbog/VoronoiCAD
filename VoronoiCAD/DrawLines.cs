// system 
using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;


// ODA
using Teigha.Runtime;
using Teigha.DatabaseServices;
using Teigha.Geometry;

// Bricsys
using Bricscad.ApplicationServices;
using Bricscad.Runtime;
using Bricscad.EditorInput;



// alias
using _AcAp = Bricscad.ApplicationServices;
using _AcDb = Teigha.DatabaseServices;
using _AcGe = Teigha.Geometry;
using _AcEd = Bricscad.EditorInput;

using TriangleNet;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TriangleNet.Voronoi;

namespace VoronoiCAD
{
    public class DatabaseCAD
    {
        public static void do_addLine()
        {
            Editor editor = _AcAp.Application.DocumentManager.MdiActiveDocument.Editor;
            try
            {
                Point3d startPoint = new Point3d(0, 0, 0);
                Point3d endPoint = new Point3d(100, 100, 0);
                Line line = new Line(startPoint, endPoint);
                AddToModelSpace(line);
            }
            catch (System.Exception ex)
            {
                _AcAp.Application.ShowAlertDialog(
                    string.Format("\nError: {0}\nStackTrace: {1}", ex.Message, ex.StackTrace));
            }
        }
        public static void SetLayerCurrent(string LayerName)
        {
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Layer table for read
                LayerTable acLyrTbl;
                acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId,
                                                OpenMode.ForRead) as LayerTable;

                string sLayerName = LayerName;

                if (acLyrTbl.Has(sLayerName) == true)
                {
                    // Set the layer Center current
                    acCurDb.Clayer = acLyrTbl[sLayerName];

                    // Save the changes
                    acTrans.Commit();
                }

                // Dispose of the transaction
            }
        }
        public static void do_addLayer(string LayerName, short colorindex)
        {
            Editor editor = _AcAp.Application.DocumentManager.MdiActiveDocument.Editor;
            try
            {
                // create a new layer
                LayerTableRecord myLayer = new LayerTableRecord();
                myLayer.Name = LayerName;
                myLayer.Color = Teigha.Colors.Color.FromColorIndex(Teigha.Colors.ColorMethod.ByAci, colorindex);
                

                // get the database and start a new transaction
                Database database = HostApplicationServices.WorkingDatabase;
                _AcDb.TransactionManager manager = database.TransactionManager;

                using (Transaction action = manager.StartTransaction())
                {
                    LayerTable layerTable =
                        action.GetObject(database.LayerTableId, OpenMode.ForWrite) as LayerTable;

                    if (layerTable == null)
                        throw new System.NullReferenceException("LayerTable == null");

                    if (!layerTable.Has(myLayer.Name))
                        layerTable.Add(myLayer);

                    action.AddNewlyCreatedDBObject(myLayer, true);
                    action.Commit();
                }
            }
            catch (System.Exception ex)
            {
                _AcAp.Application.ShowAlertDialog(
                    string.Format("\nError: {0}\nStackTrace: {1}", ex.Message, ex.StackTrace));
            }
        }

        public static void do_addFace(Point3d pt1, Point3d pt2, Point3d pt3, string layerName)
        {


            Editor editor = _AcAp.Application.DocumentManager.MdiActiveDocument.Editor;
            try
            {
                Face face = new Face(pt1, pt2, pt3,true,true,true,true);
                face.Layer = layerName;




                AddToModelSpace(face);
            }
            catch (System.Exception ex)
            {
                _AcAp.Application.ShowAlertDialog(
                    string.Format("\nError: {0}\nStackTrace: {1}", ex.Message, ex.StackTrace));
            }
        }


        public static void do_addPolyLine(bool isClosed, params TriangleNet.Geometry.Vertex[] list)
        {
           

            Editor editor = _AcAp.Application.DocumentManager.MdiActiveDocument.Editor;
            try
            {
                Polyline pline1 = new Polyline();
                int i = 0;
                foreach (var vertex in list)
                {
                    pline1.AddVertexAt(i, new Point2d(vertex.X, vertex.Y), 0, 0, 0);
                    i++;
                    
                }
               if (isClosed) pline1.Closed = true;

                AddToModelSpace(pline1);
            }
            catch (System.Exception ex)
            {
                _AcAp.Application.ShowAlertDialog(
                    string.Format("\nError: {0}\nStackTrace: {1}", ex.Message, ex.StackTrace));
            }
        }


        public static void do_addPolyLine(bool isClosed, List<Point2d> list)
        {


            Editor editor = _AcAp.Application.DocumentManager.MdiActiveDocument.Editor;
            try
            {
                Polyline pline1 = new Polyline();
                int i = 0;
                foreach (var vertex in list)
                {
                    pline1.AddVertexAt(i, vertex, 0, 0, 0);
                    i++;

                }
                if (isClosed) pline1.Closed = true;

                AddToModelSpace(pline1);
            }
            catch (System.Exception ex)
            {
                _AcAp.Application.ShowAlertDialog(
                    string.Format("\nError: {0}\nStackTrace: {1}", ex.Message, ex.StackTrace));
            }
        }

        public static void do_addPolyLine(bool isClosed, params Point2d[] list)
        {


            Editor editor = _AcAp.Application.DocumentManager.MdiActiveDocument.Editor;
            try
            {
                Polyline pline1 = new Polyline();
                int i = 0;
                foreach (var vertex in list)
                {
                    pline1.AddVertexAt(i, vertex, 0, 0, 0);
                    i++;

                }
                if (isClosed) pline1.Closed = true;

                AddToModelSpace(pline1);
            }
            catch (System.Exception ex)
            {
                _AcAp.Application.ShowAlertDialog(
                    string.Format("\nError: {0}\nStackTrace: {1}", ex.Message, ex.StackTrace));
            }
        }

        //helper Function to add items to ModelSpace;
        public static ObjectIdCollection AddToModelSpace(params Entity[] list)
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            Database database = HostApplicationServices.WorkingDatabase;
            _AcDb.TransactionManager manager = database.TransactionManager;
            using (Transaction action = manager.StartTransaction())
            {
                BlockTable blockTable =
                    action.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
                if (blockTable == null)
                    throw new System.NullReferenceException("blockTable == null");

                BlockTableRecord blockTableRecord =
                    action.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                if (blockTableRecord == null)
                    throw new System.NullReferenceException("blockTableRecord == null");

                foreach (Entity ent in list)
                {
                    ids.Add(blockTableRecord.AppendEntity(ent));
                    action.AddNewlyCreatedDBObject(ent, true);
                }
                action.Commit();
            }
            return ids;
        }
        
        public static bool IsPointInPolygon(Point2d p, List<Point2d> polygon)
        {
            double minX = polygon[0].X;
            double maxX = polygon[0].X;
            double minY = polygon[0].Y;
            double maxY = polygon[0].Y; 
            for (int i = 1; i < polygon.Count; i++)
            {
                Point2d q = polygon[i];
                minX = Math.Min(q.X, minX);
                maxX = Math.Max(q.X, maxX);
                minY = Math.Min(q.Y, minY);
                maxY = Math.Max(q.Y, maxY);
            }

            if (p.X < minX || p.X > maxX || p.Y < minY || p.Y > maxY)
            {
                return false;
            }

            // http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
            bool inside = false;
            for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
            {
                if ((polygon[i].Y > p.Y) != (polygon[j].Y > p.Y) &&
                     p.X < (polygon[j].X - polygon[i].X) * (p.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X)
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        public static List<Point2d> getFacePolygon (TriangleNet.Topology.DCEL.Face face)
            {
            List<Point2d> ptList = new List<Point2d>();       
            var edge = face.Edge;
            var first = edge.Origin.ID;
            do
            {
                ptList.Add(new Point2d(edge.Origin.X, edge.Origin.Y));
                edge = edge.Next;
            }
            while (edge != null && edge.Origin.ID != first);
            return ptList;
        }

        public static int getNumberGeneralPoints (TriangleNet.Topology.DCEL.Face face0,
            TriangleNet.Topology.DCEL.Face face1)
        {
            List<Point2d> ptList0 = getFacePolygon(face0);
            List<Point2d> ptList1 = getFacePolygon(face1);
            int numGenPts = 0;
            foreach (var item0 in ptList0)
            {
                foreach (var item1 in ptList1)
                {
                    //if (item0.IsEqualTo(item1)) numGenPts++;
                    if (Math.Abs(item0.X-item1.X)<0.01 && Math.Abs(item0.Y-item1.Y)<0.01) numGenPts++;





                }


            }
            
            return numGenPts;


        }

        public static bool IsPointInFace(Point2d pt, TriangleNet.Topology.DCEL.Face face)
        {
            List<Point2d> facePolygonList = getFacePolygon(face);

            if (IsPointInPolygon(pt, facePolygonList)) return true;
            else return false;
                
            }

        public static List<TriangleNet.Topology.DCEL.Face> getNeighborFaces(TriangleNet.Topology.DCEL.Face face)
        {
            List<TriangleNet.Topology.DCEL.Face> neighborFaces = new List<TriangleNet.Topology.DCEL.Face>();
            var edge = face.Edge;
           
            var first = edge.Origin.ID;
            do
            {
                if (edge.Twin != null)
                {
                    neighborFaces.Add( edge.Twin.Face);
                }
                  
                edge = edge.Next;
            }
            while (edge != null && edge.Origin.ID != first);
            return neighborFaces;
        }
        public static bool isNoFaceInList(List<TriangleNet.Topology.DCEL.Face> neighborFaces,
           TriangleNet.Topology.DCEL.Face face )
        {
            bool isFcInLst = false;
            foreach (var face_i in neighborFaces)
            {
                if(getNumberGeneralPoints(face, face_i) < 2)
                {
                    isFcInLst = true; break;
                }
            }
            return isFcInLst;
            }
        public static List<TriangleNet.Topology.DCEL.Face> getNeighborFaces3(TriangleNet.Topology.DCEL.Face face,
             StandardVoronoi voronoi, int iNumberGeneralPoints)
        {
            List<TriangleNet.Topology.DCEL.Face> neighborFaces3 = new List<TriangleNet.Topology.DCEL.Face>();
            foreach (var eachface in voronoi.Faces)
            {
                if (getNumberGeneralPoints(face, eachface) == iNumberGeneralPoints)
                {
                    neighborFaces3.Add(eachface);
                }
            }
            return neighborFaces3;
            }
       

        public static List<TriangleNet.Topology.DCEL.Face> getNeighborFaces2(TriangleNet.Topology.DCEL.Face face)
        {
            List<TriangleNet.Topology.DCEL.Face> neighborFaces = DatabaseCAD.getNeighborFaces(face);
            List<TriangleNet.Topology.DCEL.Face> neighborFaces2 = DatabaseCAD.getNeighborFaces(face);
            foreach (var face_i in neighborFaces)
            {
                List<TriangleNet.Topology.DCEL.Face> neighborFaces_i = DatabaseCAD.getNeighborFaces(face_i);
                foreach (var face_ii in neighborFaces_i)
                {
                    //if (!isFaceInList(neighborFaces2, face_ii))
                    //{ if (getNumberGeneralPoints(face, face_ii) == 1) neighborFaces2.Add(face_ii); }
                  
                     if (getNumberGeneralPoints(face, face_ii) == 1) neighborFaces2.Add(face_ii); 
                }
            }
            return neighborFaces2;
         }


            static double getDistance(double xi, double yi, double xj, double yj)
        {
            return Math.Sqrt((xi - xj) * (xi - xj) + (yi - yj) * (yi - yj));
        }
        
        public static double getAlphaiX (double xi, double yi, double d, double xj, double yj)
        {
            double rij = getDistance(xi, yi, xj, yj);
            return 1 - d / rij * (1.17 + 0.36 * (xj - xi) / rij - 0.15 * ((xj - xi) / rij) * ((xj - xi) / rij));
        }
               


    }

    public class Kust
    {
        public List<Point2d> Pts { get; set; }
        public string Name { get; set; }
    }
    public class CPileCell
    {
        public string KustName { get; set; }
        public Point2d ptPile { get; set; }
        public List<Point2d> ptNeighborPiles { get; set; }
        public double AlphaiX { get; set; }
        public double AlphaiY { get; set; }
        public double Radius { get; set; }
        public double Area { get; set; }
        public double d { get; set; }
        public double GammaC { get; set; }
        public CPileCell() { ptNeighborPiles = new List<Point2d>();
            
        }

    }
    public class CForPileCell
    {
        public Point2d pt { get; set; }
        public double d { get; set; }
        public double GammaC { get; set; }
      



    }
}
