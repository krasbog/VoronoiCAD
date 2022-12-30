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

[assembly: CommandClass(typeof(VoronoiCAD.Commands))]

namespace VoronoiCAD
{
    public class Commands : IExtensionApplication
    {
        public Commands()
        {
        }
        public void Initialize()
        {
        }
        public void Terminate()
        {
        }
        [CommandMethod("V1", CommandFlags.UsePickSet)]
        public void V1()
        {
            //draw1.fDr1();
            draw2.fDr2();

        }
        [CommandMethod("V11", CommandFlags.UsePickSet)]
        public void V11()
        {
            //draw1.fDr1();
            draw21.fDr21();

        }
        [CommandMethod("getXdata", CommandFlags.UsePickSet)]
        public void getXdata()
        {
            
            Voronoi3DPile.getXdata();

        }
        [CommandMethod("CopyB2B", CommandFlags.UsePickSet)]
        public void CopyB2B()
        {
            draw3.b2b();
        }
        [CommandMethod("Abos", CommandFlags.UsePickSet)]
        public void Abos()
        {
            draw4.Abos1();
        }
        [CommandMethod("Abos2", CommandFlags.UsePickSet)]
        public void Abos2()
        {
            draw5.Abos2();
        }
        [CommandMethod("Abos3", CommandFlags.UsePickSet)]
        public void Abos3()
        {
            CDrawAbosGeology.DrawAbosGeology();
        }
    }
}
