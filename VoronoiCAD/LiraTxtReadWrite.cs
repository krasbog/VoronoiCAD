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
    public class KE56
    {
        public string _ElementNumber;
        public string _EINumber;
        public string _NodeNumber;
        public string _strX;
        public string _strY;
        public string _strZ;
        public double _Rz;//Жесткость старая

    }
    class LiraTxtReadWrite
    {
        public static string GetStrNumNode(Point3d fptNew, ref List<Point3d> CoordsNodes, ref int MaxNumNode)
        {
           
            int numfpt = CoordsNodes.FindIndex(x => x == fptNew);
            if (numfpt != -1) return (++numfpt).ToString();
            else
            {
                CoordsNodes.Add(fptNew);
                return (++MaxNumNode).ToString();
            }

        }
        public static string makeStrNewElement(Point3d pt1, Point3d pt2, ref List<Point3d> CoordsNodes,ref int MaxNumNode)
        {
            string str = "10 1 ";
            // 10 1 1 2 /
            str += GetStrNumNode(pt1, ref CoordsNodes, ref MaxNumNode);
            str += " ";
            str += GetStrNumNode(pt2, ref CoordsNodes, ref MaxNumNode);
            str += "/";
            str += (char)10;
            str += (char)13;
           
            return str;
        }

        public static string makeDok_1_KE10 (ref List<CPile> piles, ref List<Point3d> CoordsNodes,
            ref List<string> ForcesNodes, ref List<string> nizNodes, ref int MaxNumNode)
        {
            string Dok_1_KE10 = "( 1/";
            Dok_1_KE10 += (char)10;
            Dok_1_KE10 += (char)13;

            foreach (var pile in piles)
            {
                int ifor = 0;
                foreach (var lay in pile._layersFL._layers)
                {
                   
                    Point3d pt1 = new Point3d(pile.pileCell.ptPile.X, pile.pileCell.ptPile.Y, lay.Bottom);
                    Point3d pt2 = new Point3d(pile.pileCell.ptPile.X, pile.pileCell.ptPile.Y, lay.Top);
                    //if(pt1.DistanceTo(pt2)>0.001)
                    Dok_1_KE10 += makeStrNewElement(pt1, pt2, ref CoordsNodes, ref MaxNumNode);
                    if (ifor == 0) ForcesNodes.Add(MaxNumNode.ToString());
                    ifor++;
                    if (ifor == pile._layersFL._layers.Count) nizNodes.Add(MaxNumNode.ToString());

                }

            }
            Dok_1_KE10 += ")";
            Dok_1_KE10 += (char)10;
            Dok_1_KE10 += (char)13;
            return Dok_1_KE10;
        }
        public static string makeDok_4_coords(ref List<Point3d> CoordsNodes)
        {
            string Dok_4_coords = "( 4/";
            Dok_4_coords += (char)10;
            Dok_4_coords += (char)13;

            foreach (var pt in CoordsNodes)
            {

                // 0.00000 0.00000 0.00000 /
                Dok_4_coords += pt.X;
                Dok_4_coords += " ";
                Dok_4_coords += pt.Y;
                Dok_4_coords += " ";
                Dok_4_coords += pt.Z;
                Dok_4_coords += "/";
                Dok_4_coords += (char)10;
                Dok_4_coords += (char)13;


            }
            Dok_4_coords += ")";
            Dok_4_coords += (char)10;
            Dok_4_coords += (char)13;
            return Dok_4_coords;

        }
        public static string makeDok_5_connection(ref List<string> nizNodes)
        {
            string Dok_5_connection = "( 5/";
            Dok_5_connection += (char)10;
            Dok_5_connection += (char)13;

            foreach (var numderNode in nizNodes)
            {
                Dok_5_connection += numderNode;
                Dok_5_connection += " ";
                Dok_5_connection += "6";
                Dok_5_connection += "/";
                Dok_5_connection += (char)10;
                Dok_5_connection += (char)13;
            }
            Dok_5_connection += ")";
            Dok_5_connection += (char)10;
            Dok_5_connection += (char)13;
            return Dok_5_connection;

        }

        public static string makeDok_6_ForcesNodes(ref List<string> ForcesNodes, string direction)
        {
            string Dok_6_ForcesNodes = "( 6/";
            Dok_6_ForcesNodes += (char)10;
            Dok_6_ForcesNodes += (char)13;

            foreach (var namderNode in ForcesNodes)
            {

                //                2.Документ 6 «Типы нагрузок»
                //2 0 1 1 /
                //2 — Номер узла
                //0 — Вид нагрузки — узловая в общей системе координат
                //1 — Направление нагрузки по ОХ
                //1 — Номер строки в документе 7
                Dok_6_ForcesNodes += namderNode;
                Dok_6_ForcesNodes += " ";
                Dok_6_ForcesNodes += "0";
                Dok_6_ForcesNodes += " ";
                Dok_6_ForcesNodes += direction;
                Dok_6_ForcesNodes += " ";
                Dok_6_ForcesNodes += "1";
                Dok_6_ForcesNodes += "/";
                Dok_6_ForcesNodes += (char)10;
                Dok_6_ForcesNodes += (char)13;


            }
            Dok_6_ForcesNodes += ")";
            Dok_6_ForcesNodes += (char)10;
            Dok_6_ForcesNodes += (char)13;
            return Dok_6_ForcesNodes;

        }
        public static string makeDok_7_ForcesMagnitude()
        {
            string Dok_7_ForcesMagnitude = "( 7/ 1 1 / )";
            Dok_7_ForcesMagnitude += (char)10;
            Dok_7_ForcesMagnitude += (char)13;
            return Dok_7_ForcesMagnitude;
        }

        public static string makeDok_3_EI(ref FundamParams fp)
        {
            return @"(3 /
1 S0 3.000E+006     35     35 /
 0 RO 0.33687 /

 )
";
        }

        public static string makeDok_0()
        {
            return @"(0 / 1; МОДЕЛЬ СВАИ ТХТ / 2; 5 /

 28; 0 1 0  1 0 0  0 0 1; /
 33; M 1 CM 100 T 1 C 1 /
  39;
            1: ЗАГРУЖЕНИЕ 1;
 /
)
";
        }

        public static string makeDok_19_KE10(ref List<CPile> piles)
        {
            string makeDok_19_KE10 = "( 19/";
            makeDok_19_KE10 += (char)10;
            makeDok_19_KE10 += (char)13;
            int i = 1;
            foreach (var pile in piles)
            {

                foreach (var lay in pile._layersFL._layers)
                {
                    // 1 1.01 300 0 1.02 300 0
                    makeDok_19_KE10 += i.ToString();
                    makeDok_19_KE10 += " ";
                    makeDok_19_KE10 += pile._bp;
                    makeDok_19_KE10 += " ";
                    makeDok_19_KE10 += lay.C1x1;
                    makeDok_19_KE10 += " 0";
                    makeDok_19_KE10 += " ";
                    makeDok_19_KE10 += pile._bp;
                    makeDok_19_KE10 += " ";
                    makeDok_19_KE10 += lay.C1x2;
                    makeDok_19_KE10 += " 0";
                    makeDok_19_KE10 += "/";
                    makeDok_19_KE10 += (char)10;
                    makeDok_19_KE10 += (char)13;
                    i++;

                }

            }
            makeDok_19_KE10 += ")";
            makeDok_19_KE10 += (char)10;
            makeDok_19_KE10 += (char)13;
            return makeDok_19_KE10;
        }
        public static string genTxtLiraPiles(ref List<CPile> piles, ref FundamParams fp, string direction)
        {
            List<Point3d> CoordsNodes = new List<Point3d>();
            int MaxNumNode = 0;
            List<string> ForcesNodes = new List<string>();
            List<string> nizNodes = new List<string>();
            string str = makeDok_0();
            str+= makeDok_1_KE10 (ref piles, ref CoordsNodes, ref ForcesNodes, ref nizNodes, ref MaxNumNode);
            str += makeDok_3_EI(ref fp);
            str += makeDok_4_coords(ref CoordsNodes);
            //str += makeDok_5_connection(ref nizNodes);
            str += makeDok_6_ForcesNodes(ref ForcesNodes, direction);
            str += makeDok_7_ForcesMagnitude();
            str += makeDok_19_KE10(ref piles);
            return str;
        }
//        public static string genTxtLiraPiles(string Dok_1_KE10, string makeDok_4_coords, string makeDok_19_KE10)
//        {
//            string str =
//@"(0 / 1; МОДЕЛЬ СВАИ ТХТ / 2; 5 /

// 28; 0 1 0  1 0 0  0 0 1; /
// 33; M 1 CM 100 T 1 C 1 /
//  39;
//            1: ЗАГРУЖЕНИЕ 1;
// /
//)
//";
//            str += Dok_1_KE10;

//            str +=
//            @"(3 /
//1 S0 3.000E+006     35     35 /
// 0 RO 0.33687 /

// )
//";
//            str += makeDok_4_coords;

//            str +=
//           @"(6 /
//4 0 3 1 1 /

// )
//(7 /
//1 1 0 /

// )
//";

//            str += makeDok_19_KE10;

//            return str;

//        }

        public static int MoovePozToDocument(string docNumberTamplate, ref string strFile)
        {
            //установка позиции поиска на первую косую черту документа с заданным номером
            string docNumber = null;
            int poz = 0;

            while (poz <= strFile.Count() && docNumber != docNumberTamplate)
            {
                docNumber = null;
                poz = strFile.IndexOf('(', poz);
                if (poz == -1) return -1;
                while (strFile[poz + 1] == 32) poz++;
                while (strFile[poz + 1] != '/' && strFile[poz + 1] != 32
                        && strFile[poz + 1] != 10 && strFile[poz + 1] != 13)
                    docNumber += strFile[++poz];
            }
            return strFile.IndexOf('/', poz);
        }

        public static void MooveFromColumn(ref int poz, ref string strFile, int j)
        {
            //перемещение позиции поиска из текущей графы на количество граф
            for (int i = 0; i < j; i++)
            {
                while (strFile[poz + 1] == 32 || strFile[poz + 1] == 10 || strFile[poz + 1] == 13) poz++;
                while (strFile[poz + 1] != 32 && strFile[poz + 1] != 10 && strFile[poz + 1] != 13
                    && strFile[poz + 1] != '/' && strFile[poz + 1] != ')') poz++;
            }
        }

        public static string GetObjFromColumn(int poz, ref string strFile)
        {
            //получение объекта текущей позиции из графы
            string strObjFromColumn = null;
            while (strFile[poz + 1] == 32 || strFile[poz + 1] == 10 || strFile[poz + 1] == 13) poz++;
            while (strFile[poz + 1] != 32 && strFile[poz + 1] != 10 && strFile[poz + 1] != 13
                && strFile[poz + 1] != '/' && strFile[poz + 1] != ')')
            {
                poz++;
                strObjFromColumn += strFile[poz];
            }

            return strObjFromColumn;
        }

        public static List<string> GetElementNodes(ref int poz, ref string strFile)
        {
            //получение узлов элемента
            List<string> vstrNodes = new List<string>();

            for (int i = 0; i < 10; i++)
            {
                string str = GetObjFromColumn(poz, ref strFile);
                if (str != null)
                {
                    vstrNodes.Add(str);
                    MooveFromColumn(ref poz, ref strFile, 1);
                }
                else
                {
                    MooveFromColumn(ref poz, ref strFile, 1);
                    break;
                }
            }

            return vstrNodes;
        }

        public static List<List<string>> getDocument(string docNumberTamplate, ref string strFile, int iMooveFromColumn)
        {
            int poz = 0;
            int pozEndDoc = 0;

            //Читаем из строки содержание документа №1 в вектор - элементы
            poz = MoovePozToDocument(docNumberTamplate, ref strFile);
            pozEndDoc = strFile.IndexOf(')', poz);

            List<List<string>> vvstrElementsNodes = new List<List<string>>();
            while (poz < pozEndDoc && poz > 0)
            {
                MooveFromColumn(ref poz, ref strFile, iMooveFromColumn);
                List<string> vstrElementNodes = GetElementNodes(ref poz, ref strFile);
                if (vstrElementNodes.Count > 0)
                    vvstrElementsNodes.Add(vstrElementNodes);
                poz = strFile.IndexOf('/', poz);
            }
            return vvstrElementsNodes;
        }


        public static string getStrEI_LIRA(int iNewNumEI, double dNewEIz, string strNumEI56, ref string strFile)
        {

            List<List<string>> vvstrEIs = getDocument("3", ref strFile, 0);
            string strNewEIs = null;
            List<string> vstrEI = new List<string>();
            for (int i = 0; i < vvstrEIs.Count; i++)
                if (strNumEI56 == vvstrEIs[i][0])
                {
                    vstrEI = vvstrEIs[i];
                    break;
                }
            vstrEI[0] = iNewNumEI.ToString();
            vstrEI[3] = dNewEIz.ToString();
            for (int i = 0; i < vstrEI.Count; i++)
            {
                strNewEIs += vstrEI[i];
                strNewEIs += " ";
            }
            strNewEIs += "/";
            strNewEIs += (char)10;
            strNewEIs += (char)13;
            return strNewEIs;
        }


        //
        public static List<KE56> getTxtLiraFileKE56(string fileName)
        {
            string strFile = File.ReadAllText(fileName, Encoding.Default);
            List<List<string>> vvstrElements = getDocument("1", ref strFile, 0);
            List<List<string>> vvstrCoordsNodes = getDocument("4", ref strFile, 0);
            List<List<string>> vvstrEIs = getDocument("3", ref strFile, 0);
            List<KE56> vKE56 = new List<KE56>();
            for (int i = 0; i < vvstrElements.Count; i++)
            {
                List<string> elem = vvstrElements[i];
                KE56 _ke56 = new KE56();
                if (elem.Count > 0 && elem[0] == "56")
                {
                    _ke56._EINumber = elem[1];
                    _ke56._NodeNumber = elem[2];
                    int iNodeNumber = Convert.ToInt32(elem[2]);
                    _ke56._ElementNumber = (i+1).ToString();
                    _ke56._strX = vvstrCoordsNodes[iNodeNumber - 1][0];
                    _ke56._strY = vvstrCoordsNodes[iNodeNumber - 1][1];
                    _ke56._strZ = vvstrCoordsNodes[iNodeNumber - 1][2];
                    foreach (var vstrEI in vvstrEIs)
                    {
                        if (vstrEI[0] == _ke56._EINumber)
                        {
                            _ke56._Rz = Convert.ToDouble(vstrEI[3]);
                        }
                    }
                    vKE56.Add(_ke56);
                }
            }
            return vKE56;
        }
        //



        static void Main1(string[] args)
        {
            string strFile = File.ReadAllText
 (@"C:\Users\Public\Documents\LIRA SAPR\LIRA SAPR 2015\Data\модель сваи тхт.txt", Encoding.Default);
            List<List<string>> vvstrElementsNodes = getDocument("1", ref strFile, 2);
            List<List<string>> vvstrCoordsNodes = getDocument("4", ref strFile, 0);
            List<List<string>> vvstrC1s = getDocument("19", ref strFile, 0);

            List<int> list1 = new List<int> { 3, 4, 6, 5, 7, 8 };
            Console.WriteLine(list1.FindIndex(x => x == 1));




            int i = 0;
        }


    }
}
