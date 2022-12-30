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
using ConvertToExcel;
using System.Data;
using ExpLayer;
using System.Windows.Forms;

namespace VoronoiCAD
{
    public class CDrawAbosGeology
    {

        public static void DrawAbosGeology()
        {
            MainDialog dlg = new MainDialog();
            System.Windows.Forms.DialogResult result =
         _AcAp.Application.ShowModalDialog(_AcAp.Application.MainWindow, dlg);
            //Файл .xls с исходными данными
            string XLSfileName = dlg.GeoFileName;
            if (XLSfileName != "" && result == DialogResult.OK)
            {
                //Файл .xls с исходными данными
                //string XLSfileName = @"C:\Users\user\Documents\Visual Studio 2015\Projects\ExpLayer\ExpLayer\555.xls";

                //Список инженерно-геологических элементов
                List<CIngGeolElem> lIGE = InputData.getIGEtable(XLSfileName);

                //Список скважин.
                //Отметки верха и низа слоев из таблицы исх. данных
                //Отметки уровня подземных вод
                //Обозначения скважин
                List<CGeolSkv> lSkvs = InputData.getSkvs(XLSfileName);

                //Параметры ABOS интерполяции
                ABOSparams abosParams = ABOSparams.getABOSparams(XLSfileName);

                //Параметры свайного фундамента
                FundamParams fundamParams = FundamParams.getFundamParams(XLSfileName);


                Document acDoc = _AcAp.Application.DocumentManager.MdiActiveDocument;
                Editor acDocEd = acDoc.Editor;
                Database acDocDb = acDoc.Database;



                double x1 = 0;
                double x2 = 0;
                double y1 = 0;
                double y2 = 0;

                double dx = abosParams.dx;
                double dy = abosParams.dy;
                double delta = abosParams.delta;





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
                                    if (currentEntity.Layer == "ABOS_Contour")
                                    {
                                        int vn = ((Polyline)currentEntity).NumberOfVertices;
                                        List<TriangleNet.Geometry.Vertex> vertexList = new List<TriangleNet.Geometry.Vertex>();
                                        for (int i = 0; i < vn; i++)

                                        {



                                            Point2d pt = ((Polyline)currentEntity).GetPoint2dAt(i);
                                            vertexList.Add(new TriangleNet.Geometry.Vertex(pt.X, pt.Y, marker));



                                        }



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

                                    if (currentEntity.Layer == "GeoSkvazhina")
                                    {
                                        double x = ((BlockReference)currentEntity).Position.X;
                                        double y = ((BlockReference)currentEntity).Position.Y;
                                        string oboznSkv = "";

                                        foreach (ObjectId attId in acBlock.AttributeCollection)
                                        {
                                            var acAtt = tr.GetObject(attId, OpenMode.ForRead) as AttributeReference;
                                            if (acAtt == null) continue;

                                            if (acAtt.Tag.Equals("OBOZN", StringComparison.CurrentCultureIgnoreCase))
                                            {
                                                acAtt.UpgradeOpen();
                                                oboznSkv = acAtt.TextString;
                                            }
                                            //pts.Add(new XYZ(x, y, z));



                                        }
                                        //здесь добавить к сважинам их координаты центра

                                        foreach (var skv in lSkvs)
                                        {
                                            if (skv.obozn == oboznSkv)
                                            {
                                                skv.Centr = new CFemPoint(x, y, 0);
                                                //skv.Centr.x = x;
                                                //skv.Centr.y = y;
                                            }

                                        }
                                    }
                                }
                            }
                            tr.Commit();
                        }
                    }



                }
                //Списки базовых точек поверхностей и названий их слоев
                //первая поверхность - УПВ, остальные - кровли ИГЭ
                //вторая поверхность - ур. земли (кровля ИГЭ 1)
                List<List<XYZ>> ptss = new List<List<XYZ>>();
                List<string> layerNames = new List<string>();
                layerNames.Add("УПВ");
                List<XYZ> ptsUPV = new List<XYZ>();
                for (int j = 0; j < lSkvs.Count; j++)
                {

                    double x = lSkvs[j].Centr.x;
                    double y = lSkvs[j].Centr.y;
                    double z = lSkvs[j].Otm_vod;
                    ptsUPV.Add(new XYZ(x, y, z));

                }
                ptss.Add(ptsUPV);

                for (int i = 0; i < lSkvs[0].Lays._layers.Count; i++)
                {
                    List<XYZ> pts = new List<XYZ>();
                    layerNames.Add(lSkvs[0].Lays._layers[i].obozn);


                    for (int j = 0; j < lSkvs.Count; j++)
                    {
                        double x = lSkvs[j].Centr.x;
                        double y = lSkvs[j].Centr.y;
                        double z = lSkvs[j].Lays._layers[i].Top;
                        pts.Add(new XYZ(x, y, z));



                    }
                    ptss.Add(pts);

                }
                //Вычисление ABOS поверхностей из базовых поверхностей
                List<List<Point3d>> AbosPtss = new List<List<Point3d>>();
                for (int k = 0; k < ptss.Count; k++)
                {
                    List<XYZ> pts = ptss[k];
                    CdMatrix m1 = CAbos.getAbosMatrix(pts, x1, x2, y1, y2, dx, dy, delta);
                    List<Point3d> AbosPts = new List<Point3d>();
                    for (int i = 0; i < m1.size1(); i++)
                    {
                        for (int j = 0; j < m1.size2(); j++)
                        {
                            AbosPts.Add(new Point3d(x1 + i * dx, y1 + j * dy, m1[i, j]));

                        }
                    }
                    AbosPtss.Add(AbosPts);

                }

                //Если нижняя поверхность выше верхней, то в этих точках опустить её до верхней
                for (int i = 1; i < AbosPtss.Count - 1; i++)
                {
                    for (int j = 0; j < AbosPtss[i].Count; j++)
                    {
                        if (AbosPtss[i][j].Z < AbosPtss[i + 1][j].Z) AbosPtss[i + 1][j] = AbosPtss[i][j];
                    }
                }


                //Отрисовка ABOS поверхностей
                for (int k = 0; k < AbosPtss.Count; k++)
                {
                    //List<XYZ> pts = ptss[k];
                    List<Point3d> AbosPts = AbosPtss[k];
                    string layerName = layerNames[k];
                    short colorIndx;
                    if (layerName == "УПВ") colorIndx = 5;
                    else
                    {
                        colorIndx = (short)(k + 1);
                        if (colorIndx == 5) colorIndx += 1;
                    }
                    DatabaseCAD.do_addLayer(layerName, colorIndx);
                    //DatabaseCAD.SetLayerCurrent(layerName);
                    //Application.SetSystemVariable("CLAYER", layerName);

                    var p1 = new Polygon();//точки полигона для триангуляции
                    int marker1 = 0;
                    foreach (var pt in AbosPts)
                    {
                        p1.Add(new TriangleNet.Geometry.Vertex(pt.X, pt.Y, marker1));
                        marker1++;
                    }
                    Mesh mesh = p1.Triangulate() as Mesh;
                    foreach (var triang in mesh.Triangles)
                    {
                        Point3d pt0 = new Point3d();
                        Point3d pt1 = new Point3d();
                        Point3d pt2 = new Point3d();
                        foreach (var pt in AbosPts)
                        {
                            if (triang.GetVertex(0).X == pt.X && triang.GetVertex(0).Y == pt.Y) pt0 = pt;
                            if (triang.GetVertex(1).X == pt.X && triang.GetVertex(1).Y == pt.Y) pt1 = pt;
                            if (triang.GetVertex(2).X == pt.X && triang.GetVertex(2).Y == pt.Y) pt2 = pt;
                        }
                        DatabaseCAD.do_addFace(pt0, pt1, pt2, layerName);


                    }

                }
                //Продолжение...

            }
        }
    }
}