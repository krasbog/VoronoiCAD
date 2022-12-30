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
using ExpLayer;
using ABOS2;

namespace VoronoiCAD
{
    public class draw21
    {
        public static void fDr21()
        {
            
            List < CIngGeolElem > lIGE=new List<CIngGeolElem>();
            List<CGeolSkv> lSkvs = new List<CGeolSkv>();                      
            ABOSparams abosParams = new ABOSparams();
            FundamParams fundamParams = new FundamParams();
            List<CRzFromXLS> lRz = new List<CRzFromXLS>();
            AbosGeology.getXlsGeoData(ref lIGE, ref lSkvs,ref abosParams, ref fundamParams, ref lRz);
            List<KE56> lKE56 = new List<KE56>();
            if(fundamParams.fileNameOrigLiraTxt!="")
            lKE56 = LiraTxtReadWrite.getTxtLiraFileKE56(fundamParams.fileNameOrigLiraTxt);

            double x1 = 0;
            double x2 = 0;
            double y1 = 0;
            double y2 = 0;

            double dx = abosParams.dx;
            double dy = abosParams.dy;
            double delta = abosParams.delta;

            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor acDocEd = acDoc.Editor;



            PromptSelectionResult acSSPrompt = acDocEd.GetSelection();
            if (acSSPrompt.Status == PromptStatus.OK)
            {
                SelectionSet acSSet = acSSPrompt.Value;
                List<CPileCell> pileCells = VoronoiPile.GetPileCells(acSSet);
                AbosGeology.getDwgGeoData(acSSet, ref lSkvs,ref x1, ref x2, ref y1, ref y2);
                //VoronoiPile.DrawPileCells(acSSet, pileCells);
           
            List<List<Point3d>> AbosPtss = new List<List<Point3d>>();
            List<string> layerNames = new List<string>();
            List<CdMatrix> AbosZs = new List<CdMatrix>();
            AbosGeology.getAbosGeology(ref AbosZs, ref AbosPtss, ref layerNames,
           ref abosParams, ref lSkvs,
           ref x1, ref x2, ref y1, ref y2);
            AbosGeology.DrawAbosGeology(ref AbosPtss, ref layerNames);

                //Voronoi3DPile.Draw3Dpiles(acSSet, fundamParams);
               
                //Формирование списка свай ( добавка поля pileCell, отм. воды, геометрии
                //слоев с названием грунта слоя
                List<CPile> piles = new List<CPile>();
            
            for (int i = 0; i < pileCells.Count; i++)
            {
                CPile pile = new CPile();
                pile.pileCell = pileCells[i];
                pile._FundamParams = fundamParams;
                pile.Otm_vod = ABOS2.CAbos.getPointFromAbosMatrixRoundZ(
                  pile.pileCell.ptPile.X, pile.pileCell.ptPile.Y, x1, x2, y1, y2, dx, dy, AbosZs[0]);
                for (int j = 1; j < AbosZs.Count-1; j++)
                {
                    double top = ABOS2.CAbos.getPointFromAbosMatrixRoundZ(
                  pile.pileCell.ptPile.X, pile.pileCell.ptPile.Y, x1, x2, y1, y2, dx, dy, AbosZs[j]);
                    double bottom = ABOS2.CAbos.getPointFromAbosMatrixRoundZ(
                  pile.pileCell.ptPile.X, pile.pileCell.ptPile.Y, x1, x2, y1, y2, dx, dy, AbosZs[j+1]);
                        Layer lay = new Layer(top, bottom);
                        lay.obozn = layerNames[j];
                        pile._layers.Add(lay);

                    }
                    piles.Add(pile);
            }
              
                foreach (CPile pile in piles)
                {
                    //Разбиение слоев в сваях по УГВ
                    pile._layers.divideLayersInOneLevel(pile.Otm_vod);
                    //Разбиение слоев в сваях по низу ростверка
                    pile._layers.divideLayersInOneLevel(pile._FundamParams.nizRostv);
                    //Разбиение слоев в сваях по низу свай
                    pile._layers.divideLayersInOneLevel(pile._FundamParams.nizSvai);
                    //2018-04-01
                    //Разбиение слоев в сваях на отм. ниже концов свай на 0,5 длины свай
                    double otm05L = pile._FundamParams.nizSvai - 0.5 * pile._FundamParams.dlinaSvai;
                    pile._layers.divideLayersInOneLevel(otm05L);
                    //2018-04-01
                    //Назначение слоям характеристик ИГЭ
                    foreach (var lay in pile._layers._layers)
                    {
                        string IGEname = lay.obozn;

                        CIngGeolElem IGEelem =
                            lIGE.Find(IGE => (IGE.m_obozn == lay.obozn));

                        lay.KoefPuassona = IGEelem.m_dKoefPuassona;
                        lay._PileSoils = IGEelem.m_PileSoils;
                        lay._type_of_soil = IGEelem.m_type_of_soil;
                        lay._e = IGEelem.m_e;
                        lay._IL = IGEelem.m_IL;
                        lay.m_type_of_soil_bearing = IGEelem.m_type_of_soil_bearing;
                        lay.m_gamma_CR = IGEelem.m_gamma_CR;
                        lay.m_gamma_CF = IGEelem.m_gamma_CF;
                        if (lay.Bottom>= pile.Otm_vod)
                        {
                            lay.E = IGEelem.m_E_prirodn;
                            lay.EII = IGEelem.m_EII_prirodn;
                            lay.Gamma = IGEelem.m_gamma_prirodn;
                        }
                        else
                        {
                            lay.E = IGEelem.m_E_vodonas;
                            lay.EII = IGEelem.m_EII_vodonas;
                            lay.Gamma = IGEelem.m_gamma_vzveshen;

                        }

                    }
                    
                    

                }
                if (fundamParams.fileNameOrigLiraTxt != "")
                {
                    //Запишем в сваи инфу об усилиях и данных КЭ56 из исходного (оригинального) файла Лиры
                    if (lKE56.Count > 0)
                    {
                        foreach (CPile pile in piles)
                        {
                            foreach (var KE56 in lKE56)
                            {
                                if (Math.Abs(pile.pileCell.ptPile.X - Convert.ToDouble(KE56._strX)) < 0.05
                                    && Math.Abs(pile.pileCell.ptPile.Y - Convert.ToDouble(KE56._strY)) < 0.05)
                                {
                                    pile._K56data = KE56;
                                }
                            }

                        }
                    }
                    if (lRz.Count > 0)
                    {
                        foreach (CPile pile in piles)
                        {
                            foreach (var eachRz in lRz)
                            {
                                if (pile._K56data._ElementNumber == eachRz._elemNumber)
                                {
                                    pile.P = eachRz._Rz;
                                }
                            }

                        }
                    }
                }
                foreach (CPile pile in piles)
                {

                    if (pile.P == 0) pile.P = 100;
                    //Получение слоев ниже концов свай
                    //pile._layersUnderFL = pile._layers;
                    pile._layersUnderFL._layers = new List<Layer>(pile._layers._layers);
                    pile._layersUnderFL.removeHigher(pile._FundamParams.nizSvai);
                    //Получение слоев выше низа ростверка
                    //pile._layersUpperFL= pile._layers;
                    pile._layersUpperFL._layers = new List<Layer>(pile._layers._layers);
                    pile._layersUpperFL.removeBelow(pile._FundamParams.nizRostv);
                    //Вычисление sigmaZg0
                    for (int i = 0; i < pile._layersUpperFL._layers.Count; i++)
                        pile.sigmaZg0
                            +=
                            pile._layersUpperFL._layers[i].Gamma * 0.1019716 * pile._layersUpperFL._layers[i].H;


                    //Получение слоев в уровне свай
                    //pile._layersFL = pile._layers;
                    pile._layersFL._layers = new List<Layer>(pile._layers._layers);
                    pile._layersFL.removeHigher(pile._FundamParams.nizRostv);
                    pile._layersFL.removeBelow(pile._FundamParams.nizSvai);

                    //2018-04-01
                    //Получение слоев ниже концов свай на 0,5 длины свай
                    pile.kustData._layersUnderFL05L._layers = new List<Layer>(pile._layers._layers);
                    double otm05L = pile._FundamParams.nizSvai - 0.5*pile._FundamParams.dlinaSvai;
                    pile.kustData._layersUnderFL05L.removeHigher(pile._FundamParams.nizSvai);
                    pile.kustData._layersUnderFL05L.removeBelow(otm05L);
                    //2018-04-01

                    //Вычисление sigmaZg1 в уровне свай (масса грунта в уровне свай)
                    for (int i = 0; i < pile._layersFL._layers.Count; i++)
                        pile.sigmaZg01 +=
                            pile._layersFL._layers[i].Gamma * 0.1019716 * pile._layersFL._layers[i].H;

                    //Разбиение на элементарные слои
                    double step = pile._FundamParams.b * 0.4;
                    if (pile._FundamParams.deltaHmin != 0 && pile._FundamParams.deltaHmin < step)
                        step = pile._FundamParams.deltaHmin;
                    pile._layersUnderFL.divideLayers(step);

                    //Разбиение на элементарные слои (в уровне свай)
                   double stepSv = pile._FundamParams.deltaHmin;
                    pile._layersFL.divideLayers(step);


                    //Вычисление sigmaZg (в уровне низа слоя)
                    double sigma_zgi = pile.sigmaZg0 + pile.sigmaZg01;
                    for (int i = 0; i < pile._layersUnderFL._layers.Count; i++)
                    {
                        double sigma_zg_delta =
                            pile._layersUnderFL._layers[i].Gamma * 0.1019716 * pile._layersUnderFL._layers[i].H;
                        sigma_zgi += sigma_zg_delta;
                        pile._layersUnderFL._layers[i].sigmaZg = sigma_zgi;
                    }
                }
                foreach (CPile pile in piles)
                {

                    //Вычисление sigmaZp и sigmaZgamma (в уровне низа слоя)                 
                    for (int i = 0; i < pile._layersUnderFL._layers.Count; i++)
                    {
                        double Z;//глубина точки от низа свай
                        Z = pile._FundamParams.nizSvai - pile._layersUnderFL._layers[i].Bottom;
                        double Zgamma = pile._FundamParams.nizRostv - pile._layersUnderFL._layers[i].Bottom;
                        double Zgamma0=0;
                        if(i==0)Zgamma0 = pile._FundamParams.nizRostv - pile._FundamParams.nizSvai;
                        double m = 0.5*Math.Sqrt(pile.pileCell.Area);
                        double p = pile.P / pile.pileCell.Area;
                        pile._layersUnderFL._layers[i].sigmaZp = sigma_zp_DBN(p, 0, 0, Z, m, m);
                        pile._layersUnderFL._layers[i].sigmaZgamma= sigma_zp_DBN(pile.sigmaZg0, 0, 0, Zgamma, m, m);
                        if (i == 0)pile.sigmaZgamma0= sigma_zp_DBN(pile.sigmaZg0, 0, 0, Zgamma0, m, m);
                        for (int j=0;j< piles.Count;j++)
                        {
                            if(pile.pileCell.ptPile!= piles[j].pileCell.ptPile)
                            {
                                double Xj = pile.pileCell.ptPile.X - piles[j].pileCell.ptPile.X;
                                double Yj = pile.pileCell.ptPile.Y - piles[j].pileCell.ptPile.Y;
                                double Zj = piles[j]._FundamParams.nizSvai - pile._layersUnderFL._layers[i].Bottom;
                                double Zgammaj = piles[j]._FundamParams.nizRostv - pile._layersUnderFL._layers[i].Bottom;
                                double Zgammaj0 = 0;
                                if (i == 0) Zgammaj0 = piles[j]._FundamParams.nizRostv - pile._FundamParams.nizSvai;
                                double mj = 0.5 * Math.Sqrt(piles[j].pileCell.Area);
                                double pj = piles[j].P / piles[j].pileCell.Area;
                                pile._layersUnderFL._layers[i].sigmaZp += sigma_zp_DBN(pj, Xj, Yj, Zj, mj, mj);
                                pile._layersUnderFL._layers[i].sigmaZgamma += sigma_zp_DBN(piles[j].sigmaZg0, Xj, Yj, Zgammaj, mj, mj);
                                if (i == 0) pile.sigmaZgamma0 += sigma_zp_DBN(piles[j].sigmaZg0, Xj, Yj, Zgammaj0, mj, mj);
                            }
                        }
                    }

                    }
                //Отсечение слоев, залегающих ниже BC, сжимаемой толщи
                double kHc = 0.5;
                foreach (CPile pile in piles)
                {
                    for (int i = 0; i < pile._layersUnderFL._layers.Count; i++)
                    {
                        double g1;
                        if (i == 0) g1 = pile.sigmaZg0 + pile.sigmaZg01;
                        else g1 = pile._layersUnderFL._layers[i - 1].sigmaZg;
                        double g2 = pile._layersUnderFL._layers[i].sigmaZg;

                        double p1;
                        if (i == 0) p1 = pile.P / pile.pileCell.Area;
                        else p1 = pile._layersUnderFL._layers[i - 1].sigmaZp;
                        double p2 = pile._layersUnderFL._layers[i].sigmaZp;

                        if (p1 >= g1 * kHc && p2 <= g2 * kHc)
                        {
                            double hx = (p1 - g1 * kHc) /
               ((g2 - g1) * kHc / pile._layersUnderFL._layers[i].H - (p2 - p1) / pile._layersUnderFL._layers[i].H);

                            double sigmaZpBC = p1 + (p2 - p1) / pile._layersUnderFL._layers[i].H * hx;

                            double sigmaZgBC = g1 + (g2 - g1) / pile._layersUnderFL._layers[i].H * hx;

                            double sigmaZgammaBC = 0;
                            double p1g = 0;
                            if (i == 0) p1g = pile.sigmaZgamma0;
                            else p1g = pile._layersUnderFL._layers[i - 1].sigmaZgamma;
                            double p2g = pile._layersUnderFL._layers[i].sigmaZgamma;
                            sigmaZgammaBC = p1g + (p2g - p1g) / pile._layersUnderFL._layers[i].H * hx;

                            pile._layersUnderFL.divideLayersInOneLevel(pile._layersUnderFL._layers[i].Top - hx);
                            pile._layersUnderFL.removeBelow(pile._layersUnderFL._layers[i].Top - hx);

                            pile._layersUnderFL._layers[pile._layersUnderFL._layers.Count - 1].sigmaZg = sigmaZgBC;
                            pile._layersUnderFL._layers[pile._layersUnderFL._layers.Count - 1].sigmaZp = sigmaZpBC;
                            pile._layersUnderFL._layers[pile._layersUnderFL._layers.Count - 1].sigmaZgamma = sigmaZgammaBC;
                            break;

                        }
                    }
                }

                //2018-04-02
                if (piles[0].pileCell.KustName != null)
                {
                    foreach (CPile pile in piles)
                    {

                        //Вычисление sigmaZp и sigmaZgamma (в уровне низа слоя)                 
                        for (int i = 0; i < pile._layersUnderFL._layers.Count; i++)
                        {
                            double Z;//глубина точки от низа свай
                            Z = pile._FundamParams.nizSvai - pile._layersUnderFL._layers[i].Bottom;
                            double Zgamma = pile._FundamParams.nizRostv - pile._layersUnderFL._layers[i].Bottom;
                            double Zgamma0 = 0;
                            if (i == 0) Zgamma0 = pile._FundamParams.nizRostv - pile._FundamParams.nizSvai;
                            double m = 0.5 * Math.Sqrt(pile.pileCell.Area);
                            double p = pile.P / pile.pileCell.Area;
                            // pile._layersUnderFL._layers[i].sigmaZp = sigma_zp_DBN(p, 0, 0, Z, m, m);
                            // pile._layersUnderFL._layers[i].sigmaZgamma = sigma_zp_DBN(pile.sigmaZg0, 0, 0, Zgamma, m, m);
                            // if (i == 0) pile.sigmaZgamma0 = sigma_zp_DBN(pile.sigmaZg0, 0, 0, Zgamma0, m, m);
                            pile._layersUnderFL._layers[i].sigmaZp = 0;
                             pile._layersUnderFL._layers[i].sigmaZgamma = 0;
                             if (i == 0) pile.sigmaZgamma0 = 0;
                            for (int j = 0; j < piles.Count; j++)
                            {
                                if (pile.pileCell.KustName != piles[j].pileCell.KustName)
                                {
                                    if (pile.pileCell.ptPile != piles[j].pileCell.ptPile)
                                    {
                                        double Xj = pile.pileCell.ptPile.X - piles[j].pileCell.ptPile.X;
                                        double Yj = pile.pileCell.ptPile.Y - piles[j].pileCell.ptPile.Y;
                                        double Zj = piles[j]._FundamParams.nizSvai - pile._layersUnderFL._layers[i].Bottom;
                                        double Zgammaj = piles[j]._FundamParams.nizRostv - pile._layersUnderFL._layers[i].Bottom;
                                        double Zgammaj0 = 0;
                                        if (i == 0) Zgammaj0 = piles[j]._FundamParams.nizRostv - pile._FundamParams.nizSvai;
                                        double mj = 0.5 * Math.Sqrt(piles[j].pileCell.Area);
                                        double pj = piles[j].P / piles[j].pileCell.Area;
                                        pile._layersUnderFL._layers[i].sigmaZp += sigma_zp_DBN(pj, Xj, Yj, Zj, mj, mj);
                                        pile._layersUnderFL._layers[i].sigmaZgamma += sigma_zp_DBN(piles[j].sigmaZg0, Xj, Yj, Zgammaj, mj, mj);
                                        if (i == 0) pile.sigmaZgamma0 += sigma_zp_DBN(piles[j].sigmaZg0, Xj, Yj, Zgammaj0, mj, mj);
                                    }
                                }
                            }
                        }

                    }
                }
                ///2018-04-02
                //Вычисление давлений в середине слоев
                foreach (CPile pile in piles)
                {
                    if (piles[0].pileCell.KustName == null)
                    {
                        pile._layersUnderFL._layers[0].sigmaZpC =
                                0.5 * (pile.P / pile.pileCell.Area + pile._layersUnderFL._layers[0].sigmaZp);
                    }
                    else
                        pile._layersUnderFL._layers[0].sigmaZpC =
                                0.5 * (pile._layersUnderFL._layers[0].sigmaZp);

                    pile._layersUnderFL._layers[0].sigmaZgC =
                        0.5 * (pile.sigmaZg0 + pile.sigmaZg01 + pile._layersUnderFL._layers[0].sigmaZg);
                    pile._layersUnderFL._layers[0].sigmaZgammaC =
                        0.5 * (pile.sigmaZgamma0 + pile._layersUnderFL._layers[0].sigmaZgamma);

                    for (int i=1;i< pile._layersUnderFL._layers.Count;i++)
                    {
                       
                            pile._layersUnderFL._layers[i].sigmaZpC = 
                                0.5*(pile._layersUnderFL._layers[i-1].sigmaZp + pile._layersUnderFL._layers[i].sigmaZp);
                            pile._layersUnderFL._layers[i].sigmaZgC = 
                                0.5 * (pile._layersUnderFL._layers[i-1].sigmaZg + pile._layersUnderFL._layers[i].sigmaZg);
                            pile._layersUnderFL._layers[i].sigmaZgammaC = 
                                0.5 * (pile._layersUnderFL._layers[i-1].sigmaZgamma + pile._layersUnderFL._layers[i].sigmaZgamma);
                       

                    }
                }
                //Вычисление осадки условного фундамента в элементарном слое, м
                foreach (CPile pile in piles)
                {
                    for (int i = 0; i < pile._layersUnderFL._layers.Count; i++)
                    {
                        if(pile.P/pile.pileCell.Area>pile.sigmaZg0+pile.sigmaZg01)
                        {
                            pile._layersUnderFL._layers[i].Sef_i = 0.8 * (
                               (pile._layersUnderFL._layers[i].sigmaZpC - pile._layersUnderFL._layers[i].sigmaZgammaC) *
                               pile._layersUnderFL._layers[i].H * 0.009806652048217 /
                               pile._layersUnderFL._layers[i].E) +
                               0.8 *
                               pile._layersUnderFL._layers[i].sigmaZgammaC *
                               pile._layersUnderFL._layers[i].H * 0.009806652048217 /
                               pile._layersUnderFL._layers[i].EII;

                        }
                        else
                        {
                            pile._layersUnderFL._layers[i].Sef_i = 0.8 *
                               pile._layersUnderFL._layers[i].sigmaZpC *
                               pile._layersUnderFL._layers[i].H * 0.009806652048217 /
                               pile._layersUnderFL._layers[i].EII;

                        }
                        
                    }
                }
                //2018-04-02
                if (piles[0].pileCell.KustName != null)
                {
                    foreach (CPile pile in piles)
                    {
                        pile.kustData.A = pile._FundamParams.dAreaSvai;
                        pile.kustData.d = pile._FundamParams.diamSvai;
                        pile.kustData.l = pile._FundamParams.dlinaSvai;
                        pile.kustData.E = pile._FundamParams.dESvai;
                        double E1 =  pile._layersFL._layers.Sum(x => x.E * x.H)
                            / pile._layersFL._layers.Sum(x => x.H);
                        double E2 =  pile.kustData._layersUnderFL05L._layers.Sum(x => x.E * x.H)
                            / pile.kustData._layersUnderFL05L._layers.Sum(x => x.H);
                        pile.kustData.v1 = pile._layersFL._layers.Sum(x => x.KoefPuassona * x.H)
                            / pile._layersFL._layers.Sum(x => x.H);
                        pile.kustData.v2 = pile.kustData._layersUnderFL05L._layers.Sum(x => x.KoefPuassona * x.H)
                           / pile.kustData._layersUnderFL05L._layers.Sum(x => x.H);
                        pile.kustData.G1 = E1 / (2 * (1+pile.kustData.v1));
                        pile.kustData.G2 = E2 / (2 * (1+pile.kustData.v2));

                    }
                    foreach (CPile pile in piles)
                    {
                        pile.kustData._s = getSodinochn(pile.P/100, pile.kustData.G1, pile.kustData.G2,
                            pile.kustData.l, pile.kustData.d, pile.kustData.E, pile.kustData.A,
                            pile.kustData.v1, pile.kustData.v2
                            );
                        foreach (CPile pile_i in piles)
                        {
                            double aa = pile.pileCell.ptPile.GetDistanceTo(pile_i.pileCell.ptPile);
                            if (pile.pileCell.KustName == pile_i.pileCell.KustName && aa>0.001)
                            {
                                
                                pile.kustData._s += getSad(pile_i.P/100, pile.kustData.G1, pile.kustData.G2,
                            pile.kustData.l, aa,
                            pile.kustData.v1, pile.kustData.v2
                                    );
                            }
                        }
                    }
                }

                    ///2018-04-02
                    //Вычисление осадки условного фундамента, м
                    foreach (CPile pile in piles)
                {
                    pile._s_ef = pile._layersUnderFL._layers.Sum(x => x.Sef_i);
                }
                //Вычисление осадки продавливания и сжатия ствола сваи и суммарной осадки
                foreach (CPile pile in piles)
                {
                    double a = Math.Sqrt(pile.pileCell.Area);//шаг свай в окрестности сваи
                    double r = pile.pileCell.Radius;//радиус ячейки
                    double d = fundamParams.diamSvai;
                    double l = fundamParams.dlinaSvai;

                    double nu2 = pile._layersUnderFL._layers.Sum(x => x.KoefPuassona*x.H)
                        / pile._layersUnderFL._layers.Sum(x => x.H);

                    double P = pile.P;
                    double OMEGA = pile.pileCell.Area;
                    double p = P / OMEGA;

                    double E1 = 100 * pile._layersFL._layers.Sum(x => x.E * x.H)
                        / pile._layersFL._layers.Sum(x => x.H);

                    double E2 = 100 * pile._layersUnderFL._layers.Sum(x => x.E * x.H)
                        / pile._layersUnderFL._layers.Sum(x => x.H);

                    
                    double E = fundamParams.dESvai * 100;//модуль упругости бетона свай
                    double A = fundamParams.dAreaSvai;//площадь поп. сечения свай
                    double k = Math.Sqrt(A / OMEGA);

                    double deltaSp1 = 3.14159 * (1 - nu2 * nu2) * p / (4 * E2) * (a - 1.5 * d);
                    double deltaSp0 = (1 - nu2 * nu2) * (1 - k) * P / (d * E2);
                    pile._delta_s_p = deltaSp1 / (deltaSp1 / deltaSp0 * (1 - E1 / E2) + E1 / E2);
                    pile._delta_s_с = P * (l - a) / (E * A);
                    //Суммарная осадка
                    pile._s = pile._s_ef + pile._delta_s_p + pile._delta_s_с;
                    //2018-04-02
                    if (piles[0].pileCell.KustName != null)
                    {
                        pile._s = pile.kustData._s + pile._s_ef;
                        pile._delta_s_p = pile.kustData._s;
                        pile._delta_s_с = 0;
                    }
                    ///2018-04-02

                    pile.m_EI_Rz = pile.P / pile._s;


                }

                //Вычисление параметров для расчета на горизонтальную нагрузку
                double d1 = fundamParams.diamSvai;
                //Условная ширина сваи
                double bp = 0;
                if (d1 >= 0.8) bp = d1 + 1;
                else bp = 1.5 * d1 + 0.5;
                double gammaC = 3;//Коэф. условий работы для отдельно стоящей сваи

                foreach (CPile pile in piles)
                {
                    pile._bp = bp;
                   foreach (var lay in pile._layersFL._layers)
                    {
                        double K = 0;
                        if(lay._type_of_soil== "Пески" || lay._type_of_soil == "Пески мелкие")
                        {
                           K= Layers.GetK(lay._PileSoils, lay._type_of_soil, lay._e);

                        }
                        else
                        {
                            K = Layers.GetK(lay._PileSoils, lay._type_of_soil, lay._IL);
                        }
                        lay.C1 = K * (fundamParams.nizRostv - lay.Middle) / gammaC;
                        lay.C1x1 = lay.C1 * pile.pileCell.AlphaiX;
                        lay.C1x2 = lay.C1 * pile.pileCell.AlphaiY;
                    }



                }
                //2018-03-30->
                // Отбросим слои нулевой толщины
                foreach (CPile pile in piles)
                {
                    List<Layer> _laysNoNull = new List<Layer>();
                    foreach (var lay in pile._layersFL._layers)
                    {
                        if (lay.H > 0.001) _laysNoNull.Add(lay);

                    }
                    pile._layersFL._layers = _laysNoNull;

                }
                //2018-03-30<-

                if (fundamParams.fileNameNewLiraPileTxt != "")
                {
                    string strFile = LiraTxtReadWrite.genTxtLiraPiles(ref piles, ref fundamParams, "1");
                    File.WriteAllText(fundamParams.fileNameNewLiraPileTxt, strFile, Encoding.Default);
                }
                AbosGeology.DrawBS(ref piles);
                Voronoi3DPile.Draw3DpilesX(acSSet, piles, fundamParams);

                if (fundamParams.fileNameOrigLiraTxt != "")
                {
                    //Дописываем в документ №3 новые жесткости->
                    //Новая строка жесткостей

                    string strNewEIs = "";
                    int m_iNumEI = 1000;
                    string strFile1 = File.ReadAllText(fundamParams.fileNameOrigLiraTxt, Encoding.Default);
                    for (int i = 0; i < piles.Count; i++, m_iNumEI++)
                        strNewEIs +=
                    LiraTxtReadWrite.getStrEI_LIRA(m_iNumEI, piles[i].m_EI_Rz, piles[i]._K56data._EINumber, ref strFile1);

                    //Читаем из строки начало и конец документа №3
                    int poz = 0;
                    int pozEndDoc = 0;
                    poz = LiraTxtReadWrite.MoovePozToDocument("3", ref strFile1);
                    pozEndDoc = strFile1.IndexOf(")", poz);

                    //И вписываем в строку новые жесткости
                    string strNewFile_3 = "";
                    strNewFile_3 += strFile1.Substring(0, pozEndDoc);
                    strNewFile_3 += strNewEIs;
                    strNewFile_3 += strFile1.Substring(pozEndDoc, strFile1.Count() - pozEndDoc);

                    strFile1 = strNewFile_3;

                    List<List<string>> vvstrElements = LiraTxtReadWrite.getDocument("1", ref strFile1, 0);

                    //Заменяем жесткости КЭ56

                    for (int i = 0; i < piles.Count; i++)
                    {
                        string strNumEI = (1000 + i).ToString();
                        int iElementNumber = Convert.ToInt32(piles[i]._K56data._ElementNumber);
                        List<string> vstrElement = vvstrElements[iElementNumber - 1];
                        vvstrElements[iElementNumber - 1][1] = strNumEI;

                    }

                    StringBuilder strNewDoc1 = new StringBuilder();
                    for (int i = 0; i < vvstrElements.Count; i++)
                    {
                        for (int j = 0; j < vvstrElements[i].Count; j++)
                        {
                            if (vvstrElements[i].Count <= 6)
                            {
                                strNewDoc1.Append(vvstrElements[i][j]);
                                strNewDoc1.Append(" ");
                                if (j == vvstrElements[i].Count - 1)
                                {
                                    strNewDoc1.Append("/");
                                    strNewDoc1.Append((char)10);
                                    strNewDoc1.Append((char)13);
                                }
                            }
                            else
                            {
                                strNewDoc1.Append(vvstrElements[i][j]);
                                strNewDoc1.Append(" ");
                                if (j == 5 || j == vvstrElements[i].Count - 1)
                                {
                                    strNewDoc1.Append("/");
                                    strNewDoc1.Append((char)10);
                                    strNewDoc1.Append((char)13);
                                }
                            }
                        }

                    }

                    //Читаем из строки начало и конец документа №1
                    poz = 0;
                    pozEndDoc = 0;
                    poz = LiraTxtReadWrite.MoovePozToDocument("1", ref strFile1);
                    pozEndDoc = strFile1.IndexOf(")", poz);


                    string strNewFile = strFile1.Substring(0, poz + 1);
                    strNewFile += (char)10;
                    strNewFile += (char)13;
                    strNewFile += strNewDoc1;
                    strNewFile += strFile1.Substring(pozEndDoc, strFile1.Count() - pozEndDoc);
                    strFile1 = strNewFile;

                    if (fundamParams.fileNameResultLiraTxt != "")
                    {

                        File.WriteAllText(fundamParams.fileNameResultLiraTxt, strFile1, Encoding.Default);
                    }
                }
                //Несущая способность свай
                foreach (CPile pile in piles)
                {
                 double NL =   pile._layers._layers[0].Top;//ур. земли природный
                    double z = NL - pile._layersUnderFL._layers[0].Top;
                    double IL = pile._layersUnderFL._layers[0]._IL;
                    double gammaCR = pile._layersUnderFL._layers[0].m_gamma_CR;
                    double R = 0;
                    R = Convert.ToDouble(PileBearing.getR72_clay(z, IL));
                    double gammaCF_fi_hi = 0;
                    foreach(var lay in pile._layersFL._layers)
                    {
                        double zi = NL - lay.Middle;
 gammaCF_fi_hi += lay.m_gamma_CF * lay.H * Convert.ToDouble(PileBearing.getFi73_clay(zi, lay._IL));                          
                    }
                    pile.Fd = fundamParams.gammaC * (gammaCR*R*fundamParams.dAreaSvai+
                        fundamParams.perimSvai* gammaCF_fi_hi);
                    pile.Fd_Dopustim = fundamParams.gamma0 * pile.Fd /
                        (fundamParams.gammaN*fundamParams.gammaK);
                }
                VoronoiPile.DrawPileCells(acSSet, pileCells, piles);



                int rr = 1;



            }

        }
        
       public static double sigma_zp_DBN(double p, double x, double y, double z, double m, double n)
        {
            if (z < 0) return 0;
            double sigma_zp;
            double pi2 = 2 * 3.14159;
            sigma_zp = p / pi2 * (
                Math.Atan((x + m) * (y + n) / (z * Math.Sqrt((x + m) * (x + m) + (y + n) * (y + n) + z * z))) -
                Math.Atan((x + m) * (y - n) / (z * Math.Sqrt((x + m) * (x + m) + (y - n) * (y - n) + z * z))) +
                Math.Atan((x - m) * (y - n) / (z * Math.Sqrt((x - m) * (x - m) + (y - n) * (y - n) + z * z))) -
                Math.Atan((x - m) * (y + n) / (z * Math.Sqrt((x - m) * (x - m) + (y + n) * (y + n) + z * z))) +
                z * (x + m) * (y + n) * ((x + m) * (x + m) + (y + n) * (y + n) + 2 * z * z) / (((x + m) * (x + m) + z * z) * ((y + n) * (y + n) + z * z) * Math.Sqrt((x + m) * (x + m) + (y + n) * (y + n) + z * z)) -
                z * (x + m) * (y - n) * ((x + m) * (x + m) + (y - n) * (y - n) + 2 * z * z) / (((x + m) * (x + m) + z * z) * ((y - n) * (y - n) + z * z) * Math.Sqrt((x + m) * (x + m) + (y - n) * (y - n) + z * z)) +
                z * (x - m) * (y - n) * ((x - m) * (x - m) + (y - n) * (y - n) + 2 * z * z) / (((x - m) * (x - m) + z * z) * ((y - n) * (y - n) + z * z) * Math.Sqrt((x - m) * (x - m) + (y - n) * (y - n) + z * z)) -
                z * (x - m) * (y + n) * ((x - m) * (x - m) + (y + n) * (y + n) + 2 * z * z) / (((x - m) * (x - m) + z * z) * ((y + n) * (y + n) + z * z) * Math.Sqrt((x - m) * (x - m) + (y + n) * (y + n) + z * z))
                );
            return sigma_zp;
        }
        public static double getSodinochn
            (
            double N,
            double G1,
        double G2,
        double l,
        double d,
        double E,
        double A,
        double v1,
        double v2
            )
        {
            //double N = 1.3;//МН
            //double G1 = 5.72;//МПа
            //double G2 = 5.44;//МПа
            //double l = 21.7;//м
            //double d = 0.35;//м
            //double E = 30000;//МПа
            //double A = 0.1225;//м2
            //double v1 = 0.35;
            //double v2 = 0.35;
            double v = (v1 + v2) / 2;
            double kv = 2.82 - 3.78 * v + 2.18 * v * v;
            double kv1 = 2.82 - 3.78 * v1 + 2.18 * v1 * v1;
            double betta_shtrih = 0.17 * Math.Log(kv * G1 * l / (G2 * d));
            double alpha_shtrih = 0.17 * Math.Log(kv1 * l / d);
            double hi = E * A / (G1 * l * l);
            double lambda1 = 2.12 * Math.Pow(hi, 0.75) / (1 + 2.12 * Math.Pow(hi, 0.75));
            double betta = betta_shtrih / lambda1 + (1 - betta_shtrih / alpha_shtrih) / hi;
            return betta * N / (G1 * l);

        }
        public static double getSad
            (
        double N,
        double G1,
        double G2,
        double l,
        double a,
        double v1,
        double v2
            )
        {
            double v = (v1 + v2) / 2;
            double kv = 2.82 - 3.78 * v + 2.18 * v * v;
            double delta = 0;
            if (kv * G1 * l / (2 * G2 * a) > 1) delta = 0.17 * Math.Log(kv * G1 * l / (2 * G2 * a));
            return delta * N / (G1 * l);
        }
    }
}