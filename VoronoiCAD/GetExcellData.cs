using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConvertToExcel;
using System.Data;

namespace ExpLayer
{

   public struct ABOSparams
    {
        //исх данные для расчета ABOS интерполяции 
        public double dx;//шаг интерполяционной ABOS сетки по Х, м
        public double dy;//то же по Y
        public double delta;//точность интерполяции в долях единицы
        public static ABOSparams getABOSparams(string fileName)
        {
            ABOSparams _ABOSparams = new ABOSparams();
            DataSet ds = ExcelImport.ReadExcelXLS(fileName, false);
            List<List<string>> ABOSTable = ExcelImport.getTable(ds, "ABOS");
            _ABOSparams.dx = Convert.ToDouble(ABOSTable[2][0]);
            _ABOSparams.dy = Convert.ToDouble(ABOSTable[2][1]);
            _ABOSparams.delta = Convert.ToDouble(ABOSTable[2][2]);
            return _ABOSparams;
        }
    }
  public  struct FundamParams
    {
        //исх данные для расчета свайного фундамента
        public string fileNameNewLiraPileTxt;//имя нового файла .txt Лиры со сваями

        public string fileNameOrigLiraTxt;//имя исходного файла .txt Лиры

        public string fileNameResultLiraTxt;//имя результирующего файла .txt Лиры


        public string _obozn;//обозначение фундамента
        public double b;//ширина условного фундамента, м
        public double deltaHmin;//толщина элементарного слоя, м
        public double nizRostv;//абс. отм. низа ростверка, м
        public double nizSvai;//абс. отм. низа сваи, м
        public double diamSvai;//сторона или диаметр сваи, м
        public double dlinaSvai;//длина сваи, м
        public double dAreaSvai;//площадь поперечного сечения сваи, м2
        public double dESvai;//Модуль упругости сваи, МПа
        public double gammaC;//Коэф. усл работы сваи в грунте, γС п.7.2.2 СП24
        public double gamma0;//п.7.1.11 СП24
        public double gammaN;//п.7.1.11 СП24
        public double gammaK;//п.7.1.11 СП24
        public double perimSvai;//периметр сваи, м

        public static FundamParams getFundamParams(string fileName)
        {
            FundamParams _FundamParams = new FundamParams();
            DataSet ds = ExcelImport.ReadExcelXLS(fileName, false);
            List<List<string>> FundamTable = ExcelImport.getTable(ds, "Фундамент");
            _FundamParams._obozn = FundamTable[2][0];
            _FundamParams.b = Convert.ToDouble(FundamTable[2][1]);
            _FundamParams.deltaHmin = Convert.ToDouble(FundamTable[2][2]);
            _FundamParams.nizRostv = Convert.ToDouble(FundamTable[2][3]);
            _FundamParams.nizSvai = Convert.ToDouble(FundamTable[2][4]);
            _FundamParams.diamSvai = Convert.ToDouble(FundamTable[2][5]);
            _FundamParams.dlinaSvai = Convert.ToDouble(FundamTable[2][6]);
            _FundamParams.dAreaSvai = Convert.ToDouble(FundamTable[2][7]);
            _FundamParams.dESvai = Convert.ToDouble(FundamTable[2][8]);
            _FundamParams.gammaC = Convert.ToDouble(FundamTable[2][9]);
            _FundamParams.gamma0 = Convert.ToDouble(FundamTable[2][10]);
            _FundamParams.gammaN = Convert.ToDouble(FundamTable[2][11]);
            _FundamParams.gammaK = Convert.ToDouble(FundamTable[2][12]);
            _FundamParams.perimSvai= Convert.ToDouble(FundamTable[2][13]);
            return _FundamParams;
        }
    }


    class InputData
    {
       
public static string vLookUp(List<List<string>> SkvTable, string rowName, string columnName)
        {
            int rowNumber = -1;
            for (int i = 0; i < SkvTable.Count; i++)
            {
                if (SkvTable[i][0] == rowName) { rowNumber = i; break; }
            }
            int columnNumber = -1;
            for (int i = 0; i < SkvTable[1].Count; i++)
            {
                if (SkvTable[1][i] == columnName) { columnNumber = i; break; }
            }
            string str = "";
            if (rowNumber >= 0 && columnNumber >= 0) str = SkvTable[rowNumber][columnNumber];
            return str;
        }
        public static List<CGeolSkv> getSkvs(string fileName)
        {
            DataSet ds = ExcelImport.ReadExcelXLS(fileName, false);
            List<List<string>> SkvTable = ExcelImport.getTable(ds, "Скважины");
            List<CGeolSkv> lGeolSkv = new List<CGeolSkv>();

            List<string> sloiObozns = new List<string>();
            for (int i = 2; i < SkvTable.Count; i++)
            {
                if (SkvTable[i][0] != "#УПВ#") sloiObozns.Add(SkvTable[i][0]);
            }

            List<string> skvObozns = new List<string>();
            for (int i = 1; i < SkvTable[1].Count; i++)
            {
                skvObozns.Add(SkvTable[1][i]);
            }

            foreach (string skvObozn in skvObozns)
            {
                CGeolSkv geolSkv = new CGeolSkv();


                for (int i = 0; i < sloiObozns.Count; i++)
                {
                    double top = Convert.ToDouble(vLookUp(SkvTable, sloiObozns[i], skvObozn));
                    double bottom;
                    if (i < sloiObozns.Count - 1)
                        bottom = Convert.ToDouble(vLookUp(SkvTable, sloiObozns[i + 1], skvObozn));
                    else
                        bottom = top - 20;
                    Layer layer = new Layer(top, bottom);
                    layer.obozn = sloiObozns[i];

                    geolSkv.Lays.Add(layer);

                }
                geolSkv.Otm_vod = Convert.ToDouble(vLookUp(SkvTable, "#УПВ#", skvObozn));
                geolSkv.obozn = skvObozn;

                lGeolSkv.Add(geolSkv);
            }





            return lGeolSkv;
        }

        public static List<CRzFromXLS> getRzFromXLS(string fileName)
        {
            DataSet ds = ExcelImport.ReadExcelXLS(fileName, false);
            List<List<string>> Rztable = ExcelImport.getTable(ds, 0);
            List<CRzFromXLS> lRz = new List<CRzFromXLS>();
            for (int i = 3; i < Rztable.Count; i++)
            {
                CRzFromXLS elem = new CRzFromXLS();
                elem._elemNumber = Rztable[i][0];
                elem._Rz= Convert.ToDouble(Rztable[i][4]);
                lRz.Add(elem);
            }
            return lRz;
            }
        public static List<CIngGeolElem> getIGEtable(string fileName)
        {
            DataSet ds = ExcelImport.ReadExcelXLS(fileName, false);
            List<List<string>> IGEtable = ExcelImport.getTable(ds, "ИГЭ");
            List<CIngGeolElem> lIGE = new List<CIngGeolElem>();
            for (int i = 2; i < IGEtable.Count; i++)
            {
                CIngGeolElem IGE = new CIngGeolElem();
                IGE.m_obozn = IGEtable[i][0];
                IGE.m_gamma_prirodn = Convert.ToDouble(IGEtable[i][1]);
                IGE.m_gamma_vodonas = Convert.ToDouble(IGEtable[i][2]);
                IGE.m_gamma_vzveshen = Convert.ToDouble(IGEtable[i][3]);
                IGE.m_E_prirodn = Convert.ToDouble(IGEtable[i][4]);
                IGE.m_E_vodonas = Convert.ToDouble(IGEtable[i][5]);
                IGE.m_EII_prirodn = Convert.ToDouble(IGEtable[i][6]);
                IGE.m_EII_vodonas = Convert.ToDouble(IGEtable[i][7]);
                IGE.m_dKoefPuassona = Convert.ToDouble(IGEtable[i][8]);
                IGE.m_PileSoils = IGEtable[i][9];
                IGE.m_type_of_soil = IGEtable[i][10];
                IGE.m_e = Convert.ToDouble(IGEtable[i][11]);
                IGE.m_IL = Convert.ToDouble(IGEtable[i][12]);
                IGE.m_type_of_soil_bearing= IGEtable[i][13];
                IGE.m_gamma_CR= Convert.ToDouble(IGEtable[i][14]);
                IGE.m_gamma_CF = Convert.ToDouble(IGEtable[i][15]);
                lIGE.Add(IGE);
            }
            return lIGE;

        }
    }
    public class CKustData
    {
        //Слои ниже концов свай на 0,5 длины свай
        public Layers _layersUnderFL05L = new Layers();

        public double G1;
        public double G2;
        public double l;
        public double d;
        public double E;
        public double A;
        public double v1;
        public double v2;

        public double _s;//Осадка сваи в составе куста свай
        public double _s_ef;//Доп осадка сваи от учета влияния других кустов свай

    }

    public class CPile
    {
        public CKustData kustData = new CKustData();
       public VoronoiCAD.CPileCell pileCell = new VoronoiCAD.CPileCell();//данные Вороного, координаты центра, данные для расчета на гор. нагрузку
       public Layers _layers = new Layers();//Слои с геометрией,физическими и механическими характеристиками
       public double Otm_vod { get; set; }//отм. воды
       public FundamParams _FundamParams = new FundamParams();
        // CFemPoint m_centroid;//координаты сваи

        //Слои ниже концов свай
        public Layers _layersUnderFL = new Layers();
        //Слои выше верха ростверка
        public Layers _layersUpperFL = new Layers();
        //Слои в уровне сваи
        public Layers _layersFL = new Layers();

        public double sigmaZgamma0 { get; set; }//в уровне низа ростверка
        public double sigmaZg0 { get; set; }//в уровне низа ростверка
        public double sigmaZg01 { get; set; }//в уровне низа свай
       





        public double m_E1;//модуль общей деформации в пределах длины сваи
        public double m_E2;//модуль общей деформации в пределах Hc
        public double m_nu1;//общий коэф. Пуассона в пределах длины сваи
        public double m_nu2;//общий коэф. Пуассона в пределах Hc
        public double m_Rz;//сжимающее усилие новое
        public double m_Rz_old;//сжимающее усилие старое
        public double m_Z0;//импортируемое перемещение - наверно не нужно

        public double _s;//Вычисленная осадка
        public double _s_ef;//Осадка условного фундамента
        public double _delta_s_p;//Осадка продавливания
        public double _delta_s_с;//Осадка за счет сжатия ствола свай

       
        public double _bp; //Условная ширина сваи, м

        public double m_EI_Rz;//вычисленная жесткость сваи

        public VoronoiCAD.KE56 _K56data;//Данные КЭ56 из оригинального файла Лиры txt
        public double P { get; set; }//нагрузка на ячейку, т
        public double Fd { get; set; }//несущая способность сваи, т
        public double Fd_Dopustim { get; set; }//допускаемая нагрузка на сваю, т


        string m_strKE59_ELEMENT_NUMBER;//номер элемента в Лировском файле
        string m_strKE59_EI_NUMBER;//номер типа жесткости в Лировском файле
        string m_strKE59_NODE_NUMBER;//номер узла в Лировском файле

    }
  public  class CFemPoint
    {
        public double x;
        public double y;
        public double z;
        public CFemPoint() { }
        public CFemPoint(double _x, double _y, double _z) { x = _x; y = _y; z = _z; }
    }
  public  class CGeolSkv
    {
        string _obozn;
        double _otm_vod;
        CFemPoint _centr;
        Layers _lays = new Layers();
        public string obozn { get { return _obozn; } set { _obozn = value; } }
        public double Otm_vod { get { return _otm_vod; } set { _otm_vod = value; } }
        public CFemPoint Centr { get { return _centr; } set { _centr = value; } }
        public Layers Lays { get { return _lays; } set { _lays = value; } }
    }

    public class CRzFromXLS
    {

        public string _elemNumber;
        public double _Rz;
    }

        public  class CIngGeolElem
    {

        public string m_obozn;
        public double m_gamma_prirodn;
        public double m_gamma_vodonas;//пока не используется
        public double m_gamma_vzveshen;
        public double m_E_prirodn;
        public double m_E_vodonas;
        public double m_EII_prirodn;
        public double m_EII_vodonas;
        public double m_dKoefPuassona;
        public string m_PileSoils;//Гр-ты, окруж. сваи и их х-ки колонка 1 табл. В.1 СП 24.13330.2011
        public string m_type_of_soil;//Вид грунта Крупнообломочные, Пески, Глины и суглинки, Супеси
        public double m_e;//коэф. пористости
        public double m_IL;//показатель текучести
        public string m_type_of_soil_bearing;//Вид грунта для расчета несущей способности свай
                                             //Пески гравелистые
                                             //Пески крупные
                                             //Пески средней крупности
                                             //Пески мелкие
                                             //Пески пылеватые
        public double m_gamma_CR;//Коэф. условий работы под нижним концом, γCR
        public double m_gamma_CF;//Коэф. условий работы на боковой поверхности, γCF



    }





    public class Layer
    {
        string _obozn;//обозначение слоя, например ИГЭ 1

        
        public double C1 { get; set; }//коэф пропорциональности тс/м4
        public double C1x1 { get; set; }//в направлении действия гор. нагрузки слева с учетом взаимовлияния
        public double C1x2 { get; set; }//в направлении действия гор. нагрузки снизу с учетом взаимовлияния

        public string _PileSoils;//Гр-ты, окруж. сваи и их х-ки колонка 1 табл. В.1 СП 24.13330.2011
        public string _type_of_soil;//Вид грунта Крупнообломочные, Пески, Глины и суглинки, Супеси
        public double _e;//коэф. пористости
        public double _IL;//показатель текучести

        double _gamma;//объемный вес слоя, кН/м3, выше либо ниже УПВ
        double _E;//модуль по первичной ветви деформации, МПа
        double _EII;//то же по вторичной ветви
        double _KoefPuassona;//коэф. Пуассона

        public string m_type_of_soil_bearing;//Вид грунта для расчета несущей способности свай
                                             //Пески гравелистые
                                             //Пески крупные
                                             //Пески средней крупности
                                             //Пески мелкие
                                             //Пески пылеватые
        public double m_gamma_CR;//Коэф. условий работы под нижним концом, γCR
        public double m_gamma_CF;//Коэф. условий работы на боковой поверхности, γCF

        double _top;   //абс. отм. верха
        double _middle;//абс. отм. середины
        double _bottom;//абс. отм. низа
        double _h;//толщина слоя

        public double sigmaZp { get; set; }//на нижней границе слоя
        public double sigmaZg { get; set; }//на нижней границе слоя
        public double sigmaZgamma { get; set; }//на нижней границе слоя

        public double sigmaZpC { get; set; }//в середине слоя
        public double sigmaZgC { get; set; }//в середине слоя
        public double sigmaZgammaC { get; set; }//в середине слоя
        public double Sef_i { get; set; }//осадка условного фундамента в элементарном слое

        public string obozn { get { return _obozn; } set { _obozn = value; } }
        public double Gamma { get { return _gamma; } set { _gamma = value; } }
        public double E { get { return _E; } set { _E = value; } }
        public double EII { get { return _EII; } set { _EII = value; } }
        public double KoefPuassona { get { return _KoefPuassona; } set { _KoefPuassona = value; } }

        public double Top { get { return _top; } set { _top = value; } }
        public double Middle { get { return _middle; } set { _middle = value; } }
        public double Bottom { get { return _bottom; } set { _bottom = value; } }
        public double H { get { return _h; } set { _h = value; } }
        public Layer(double top, double bottom, Layer oldLayer)
        {
            _obozn = oldLayer.obozn;
            _gamma = oldLayer.Gamma;
            _E = oldLayer.E;
            _EII = oldLayer.EII;
            _KoefPuassona = oldLayer.KoefPuassona;
            _PileSoils = oldLayer._PileSoils;
            _type_of_soil = oldLayer._type_of_soil;
            _e = oldLayer._e;
            _IL = oldLayer._IL;
            m_type_of_soil_bearing = oldLayer.m_type_of_soil_bearing;
            m_gamma_CR = oldLayer.m_gamma_CR;
            m_gamma_CF = oldLayer.m_gamma_CF;

            if (top < bottom)
            {
                bottom = top;
            }
            _top = top;
            _bottom = bottom;
            _middle = (_top + _bottom) / 2;
            _h = _top - _bottom;
        }
        public Layer(double top, double bottom)
        {
            if (top < bottom)
            {
                bottom = top;
            }
            _top = top;
            _bottom = bottom;
            _middle = (_top + _bottom) / 2;
            _h = _top - _bottom;
        }

    }
   public class Layers
    {
      public  List<Layer> _layers = new List<Layer>();//слои

        public void Add(Layer layer)
        { _layers.Add(layer); }

        public void removeHigher(double level)
        {
            List<Layer> newLayers = new List<Layer>();
            foreach (var layer in _layers)
            {
                if (layer.Top <= level) newLayers.Add(layer);
            }
            _layers = newLayers;
        }

        public void removeBelow(double level)
        {
            List<Layer> newLayers = new List<Layer>();
            foreach (var layer in _layers)
            {
                if (layer.Bottom >= level) newLayers.Add(layer);
            }
            _layers = newLayers;
        }

        public void divideLayers(double step)
        {
            List<Layer> newLayers = new List<Layer>();
            foreach (var layer in _layers)
            {
                newLayers.AddRange(divideLayer(layer, step));
            }
            _layers = newLayers;

        }

        List<Layer> divideLayer(Layer layer, double step)
        {
            List<Layer> newLayers = new List<Layer>();
            double top = layer.Top;
            double bottom = layer.Bottom;
            if (layer.H > step)
            {
                for (double level = top - step; ; level -= step, top -= step)
                {

                    if (top > level && bottom < level)
                    {
                        newLayers.Add(new Layer(top, level, layer));
                    }
                    else { newLayers.Add(new Layer(top, bottom, layer)); break; }

                }
            }
            else newLayers.Add(layer);
            return newLayers;

        }
        public void divideLayersInOneLevel(double level)
        {

            for (int i = 0; i < _layers.Count; i++)
            {
                if (_layers[i].Top > level && _layers[i].Bottom < level)
                {

                    Layer topLayer = new Layer(_layers[i].Top, level, _layers[i]);
                    Layer bottomLayer = new Layer(level, _layers[i].Bottom, _layers[i]);

                    _layers.RemoveAt(i);
                    _layers.Insert(i, topLayer);
                    i++;
                    _layers.Insert(i, bottomLayer);
                    break;

                }
            }


        }


        public static double GetK(string m_PileSoils, string m_type_of_soil, double m_eIL)
        {
            double eILmin = 0;
            double eILmax = 0;
            double K = 0;
            if (m_PileSoils == "1")
            {
                double Kmin = 1800;
                double Kmax = 3000;


                if (m_type_of_soil == "Пески")
                {
                    eILmin = 0.55;
                    eILmax = 0.7;
                }

                if (m_type_of_soil != "Глины и суглинки")
                    K = Kmax - (Kmax - Kmin) / (eILmax - eILmin) * (m_eIL - eILmin);
                else K = 1800;
            }

            if (m_PileSoils == "2")
            {
                double Kmin = 1200;
                double Kmax = 1800;


                if (m_type_of_soil == "Пески мелкие")
                {
                    eILmin = 0.6;
                    eILmax = 0.75;
                }
                if (m_type_of_soil == "Пески")
                {
                    eILmin = 0.55;
                    eILmax = 0.7;
                }
                if (m_type_of_soil == "Глины и суглинки")
                {
                    eILmin = 0;
                    eILmax = 0.75;
                }
                if (m_type_of_soil != "Супеси")
                    K = Kmax - (Kmax - Kmin) / (eILmax - eILmin) * (m_eIL - eILmin);
                else K = 1200;
            }

            if (m_PileSoils == "3")
            {
                double Kmin = 700;
                double Kmax = 1200;


                if (m_type_of_soil == "Пески")
                {
                    eILmin = 0.6;
                    eILmax = 0.8;
                }
                if (m_type_of_soil == "Супеси")
                {
                    eILmin = 0;
                    eILmax = 0.75;
                }
                if (m_type_of_soil == "Глины и суглинки")
                {
                    eILmin = 0.5;
                    eILmax = 0.75;
                }

                K = Kmax - (Kmax - Kmin) / (eILmax - eILmin) * (m_eIL - eILmin);

            }

            if (m_PileSoils == "4")
            {
                double Kmin = 400;
                double Kmax = 700;

                if (m_type_of_soil == "Глины и суглинки")
                {
                    eILmin = 0.75;
                    eILmax = 1;
                }

                K = Kmax - (Kmax - Kmin) / (eILmax - eILmin) * (m_eIL - eILmin);

            }

            if (m_PileSoils == "5")
            {
                double Kmin = 5000;
                double Kmax = 10000;


                if (m_type_of_soil == "Пески")
                {
                    eILmin = 0.55;
                    eILmax = 0.7;
                }

                if (m_type_of_soil != "Крупнообломочные")
                    K = Kmax - (Kmax - Kmin) / (eILmax - eILmin) * (m_eIL - eILmin);
                else K = 10000;
            }
            //if (K == 0) MessageBox.Show("K=0! Проверь грунты вокруг свай!");
            return K;
        }

    }
    class Program
    {
        static void Main1(string[] args)
        {
            //Layers layers = new Layers();
            //layers.Add(new Layer(10, 0));
            //layers.divideLayers(1);
            //layers.divideLayersInOneLevel(2.26);
            //layers.divideLayersInOneLevel(8.26);
            //layers.removeBelow(2.26);
            //layers.removeHigher(8.26);

            //Файл .xls с исходными данными
            string XLSfileName = @"C:\Users\user\Documents\Visual Studio 2015\Projects\ExpLayer\ExpLayer\555.xls";

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

            int i = 0;



        }
    }
}
