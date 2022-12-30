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
namespace VoronoiCAD
{
    public class draw1
    {
        public static void fDr1()
        {
            TriangleNet.Geometry.Vertex[] cont1 =
            new TriangleNet.Geometry.Vertex[4]
        {
        new TriangleNet.Geometry.Vertex(0.0, 0.0, 1),
        new TriangleNet.Geometry.Vertex(3.0, 0.0, 1),
        new TriangleNet.Geometry.Vertex(3.0, 3.0, 1),
        new TriangleNet.Geometry.Vertex(0.0, 3.0, 1)
        };

            TriangleNet.Geometry.Vertex[] cont2 =
                 new TriangleNet.Geometry.Vertex[4]
             {
        new TriangleNet.Geometry.Vertex(1.0, 1.0, 2),
        new TriangleNet.Geometry.Vertex(2.0, 1.0, 2),
        new TriangleNet.Geometry.Vertex(2.0, 2.0, 2),
        new TriangleNet.Geometry.Vertex(1.0, 2.0, 2)
            };
            var p = new Polygon();
            p.Add(new Contour(cont1, 1));
            p.Add(new Contour(cont2, 2), new Point(1.5, 1.5));
            Mesh mesh = p.Triangulate() as Mesh;
            var voronoi2 = new StandardVoronoi(mesh);

            //var voronoi2 = new BoundedVoronoi(mesh);

            foreach (var face in voronoi2.Faces)
            {
                // Get half-edge connected to face.
                var edge = face.Edge;

                // Get the origin of first edge.
                var first = edge.Origin.ID;
                //Console.WriteLine(edge.Origin.ID);
                List<Point2d> ptList = new List<Point2d>();
                do
                {
                    // Traverse edges and corresponding neighbors.
                    if (edge.Twin != null)
                    {
                        // Get neighbor across current edge.
                        var neighbor = edge.Twin.Face;
                    }
                    ptList.Add(new Point2d(edge.Origin.X, edge.Origin.Y));
                    edge = edge.Next;
                }
                while (edge != null && edge.Origin.ID != first);
                DatabaseCAD.do_addPolyLine(true, ptList);
            }


            //foreach (var face in voronoi2.Faces)
            //{
            //    // Get half-edge connected to face.
            //    var edge = face.Edge;
            //    // Get the origin of first edge.
            //   // var first = edge.Origin.ID;
            //    List<Point2d> ptList = new List<Point2d>();
            //    do
            //    {
            //        if (edge.Origin.X != Double.NaN && edge.Origin.Y != Double.NaN)
            //        ptList.Add(new Point2d(edge.Origin.X, edge.Origin.Y));



            //        edge = edge.Next;
            //    }
            //    while (edge != null //&& edge.Origin.ID != first
            //    && edge.Origin.X!=Double.NaN && edge.Origin.Y != Double.NaN);
            //    DatabaseCAD.do_addPolyLine(true, ptList);
            //}

            DatabaseCAD.do_addPolyLine(true, cont1);
            DatabaseCAD.do_addPolyLine(true, cont2);
        }
    }
}