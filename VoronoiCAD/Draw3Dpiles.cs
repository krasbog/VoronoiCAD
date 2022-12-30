// system 
using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Specialized;



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
using ExpLayer;
using CsStepToDim;

namespace VoronoiCAD
{
    public class Voronoi3DPile
    {
        public static string getStringFromListString(List<string> list)
        {
            string str = null;

            foreach (var d in list)
            {
                str += d;
                str += "#";

            }
            return str;

        }

        public static string getStringFromListDouble(List<double> list)
        {
            string str = null;

            foreach (var d in list)
            {
                str += d;
                str += "#";

            }
            return str;

        }
        public static void Draw3DpilesX(SelectionSet acSSet, List<CPile> piles, FundamParams fundamParams)
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
                        if (currentEntity.Layer == "3DPile")
                        {
                            BlockReference sourceBlock = (BlockReference)currentEntity;
                            Point3d acPt3d = sourceBlock.Position;
                            foreach (var pile in piles)
                            {
                                BlockReference block = sourceBlock.Clone() as BlockReference;
                                //Здесь пишем Х записи
                                List<double> otm = new List<double>();
                                List<double> listSigmaP = new List<double>();
                                List<double> listSigmaZg = new List<double>();
                                List<double> listSigmaZgamma = new List<double>();

                                List<double> listSigmaPC = new List<double>();
                                List<double> listSigmaZgC = new List<double>();
                                List<double> listSigmaZgammaC = new List<double>();

                                List<double> listSef = new List<double>();

                                List<string> NeighborPts = new List<string>(); 

                                otm.Add(fundamParams.nizSvai);
                                listSigmaP.Add(pile.P / pile.pileCell.Area);
                                listSigmaZg.Add(pile.sigmaZg0 + pile.sigmaZg01);
                                listSigmaZgamma.Add(pile.sigmaZgamma0);

                                foreach(var lay in pile._layersUnderFL._layers)
                                {
                                    otm.Add(lay.Bottom);
                                    listSigmaP.Add(lay.sigmaZp);
                                    listSigmaZg.Add(lay.sigmaZg);
                                    listSigmaZgamma.Add(lay.sigmaZgamma);

                                    listSigmaPC.Add(lay.sigmaZpC);
                                    listSigmaZgC.Add(lay.sigmaZgC);
                                    listSigmaZgammaC.Add(lay.sigmaZgammaC);

                                    listSef.Add(lay.Sef_i);

                                }
                                foreach (var pt in pile.pileCell.ptNeighborPiles)
                                {
                                    string np = pt.ToString() + " " + 
                                   pile.pileCell.ptPile.GetDistanceTo(pt).ToString();
                                    NeighborPts.Add(np);
                                }

                                

                               
                                //
                                Point3d acPti = new Point3d(pile.pileCell.ptPile.X, pile.pileCell.ptPile.Y, fundamParams.nizRostv);
                                Vector3d acVec3d = acPt3d.GetVectorTo(acPti);
                                block.TransformBy(Matrix3d.Displacement(acVec3d));

                                btr.UpgradeOpen();
                                btr.AppendEntity(block);
                                tr.AddNewlyCreatedDBObject(block, true);
                                Entity block1 = tr.GetObject(block.Id, OpenMode.ForWrite, false) as Entity;
                                CXdata.SetXrecordString(block.Id, "Отметка", getStringFromListDouble(otm));

                                CXdata.SetXrecordString(block.Id, "SigmaZp", getStringFromListDouble(listSigmaP));
                                CXdata.SetXrecordString(block.Id, "SigmaZg", getStringFromListDouble(listSigmaZg));
                                CXdata.SetXrecordString(block.Id, "SigmaZgamma", getStringFromListDouble(listSigmaZgamma));

                                CXdata.SetXrecordString(block.Id, "SigmaZpС", getStringFromListDouble(listSigmaPC));
                                CXdata.SetXrecordString(block.Id, "SigmaZgС", getStringFromListDouble(listSigmaZgC));
                                CXdata.SetXrecordString(block.Id, "SigmaZgammaС", getStringFromListDouble(listSigmaZgammaC));

                                CXdata.SetXrecordString(block.Id, "s_ef_i", getStringFromListDouble(listSef));

                                CXdata.SetXrecordString(block.Id, "s_ef", pile._s_ef.ToString());
                                CXdata.SetXrecordString(block.Id, "delta_s_p", pile._delta_s_p.ToString());
                                CXdata.SetXrecordString(block.Id, "delta_s_c", pile._delta_s_с.ToString());
                                CXdata.SetXrecordString(block.Id, "s", pile._s.ToString());

                                CXdata.SetXrecordString(block.Id, "NeighborPiles", getStringFromListString(NeighborPts));



                            }



                        }
                    }
                    tr.Commit();
                }
                }
            }
        public static void getXdata()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            PromptEntityOptions peo = new PromptEntityOptions("\nSelect an entity with Xdata: ");
            peo.AllowNone = false;
            PromptEntityResult per = ed.GetEntity(peo);
            if (per.Status == PromptStatus.OK)
            {
                ObjectId id = per.ObjectId;
                StringCollection strings = CXdata.GetXrecordKeys(id);
                List<string[]> colsData = new List<string[]>();
                List<string> colsName = new List<string>();
                if (strings == null || strings.Count == 0)
                _AcAp.Application.ShowAlertDialog("\nNone xrecord keys");
                else
                {
                    foreach (string str in strings)
                    {
                        colsData.Add(CXdata.GetXrecordString(id, str).Split('#'));
                        colsName.Add(str);
                        //ed.WriteMessage("\n{0}", str);
                    }
                    XDataForm dlg = new XDataForm(colsName, colsData);
                    System.Windows.Forms.DialogResult result =
                 _AcAp.Application.ShowModalDialog(_AcAp.Application.MainWindow, dlg);

                }
            }
        }
        public static void Draw3Dpiles(SelectionSet acSSet, FundamParams fundamParams)
        {
            if (acSSet.Count > 0)
            {

                List<CForPileCell> vSvai = new List<CForPileCell>();

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




                        if (currentEntity is BlockReference)
                        {



                            double x = ((BlockReference)currentEntity).Position.X;
                            double y = ((BlockReference)currentEntity).Position.Y;



                            if (currentEntity.Layer == "Vertex")
                            {

                                CForPileCell fpc = new CForPileCell();
                                fpc.pt = new Point2d(x, y);
                                vSvai.Add(fpc);
                            }




                        }

                    }
                    //
                    foreach (ObjectId id in acSSet.GetObjectIds())
                    {
                        Entity currentEntity = tr.GetObject(id, OpenMode.ForWrite, false) as Entity;
                        if (currentEntity.Layer == "3DPile")
                        {
                            BlockReference sourceBlock = (BlockReference)currentEntity;
                            Point3d acPt3d = sourceBlock.Position;
                            foreach (var pt in vSvai)
                            {
                                BlockReference block = sourceBlock.Clone() as BlockReference;
                                Point3d acPti = new Point3d(pt.pt.X, pt.pt.Y, fundamParams.nizRostv);
                                Vector3d acVec3d = acPt3d.GetVectorTo(acPti);
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