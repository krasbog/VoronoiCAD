using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABOS2
{
    public class XYZ
    {

        public double x, y, z;
        //public bool isRay;//Является продолжением луча
        //public int iZPointNumber;//Номер точки для матрицы NB
        //public double x1, x2, y1, y2;//Координаты линии разрыва

        public XYZ() { }
        public XYZ(double x1, double y1, double z1) { x = x1; y = y1; z = z1; }

    };
   public class CiMatrix
    {
        int[,] _matrix;
        int _size1;
        int _size2;
        public int size1() { return _size1; }
        public int size2() { return _size2; }
        public CiMatrix(int size1, int size2)
        {
            _matrix = new int[size1, size2];
            _size1 = size1;
            _size2 = size2;
        }
        public int this[int i, int j]
        {
            get { return _matrix[i, j]; }
            set { _matrix[i, j] = value; }
        }
        //public static CiMatrix operator +(CiMatrix m1, CiMatrix m2)

        //{
        //    CiMatrix m = new CiMatrix(m1.size1(), m1.size2());
        //    for (int i = 0; i < m.size1(); i++)
        //        for (int j = 0; j < m.size2(); j++)
        //        {
        //            m[i, j] = m1[i, j] + m2[i, j];
        //        }
        //    return m;
        //}
        public void print()
        {
            for (int i = 0; i < _size1; i++)
            {
                for (int j = 0; j < _size2; j++)
                {
                    Console.Write(_matrix[i, j]);
                    Console.Write(" ");
                }
                Console.WriteLine("\n");

            }
            Console.WriteLine("\n");

        }


    }

   public class CdMatrix
    {
        double[,] _matrix;
        int _size1;
        int _size2;
        public int size1() { return _size1; }
        public int size2() { return _size2; }
        public CdMatrix(int size1, int size2)
        {
            _matrix = new double[size1, size2];
            _size1 = size1;
            _size2 = size2;
        }
        public double this[int i, int j]
        {
            get { return _matrix[i, j]; }
            set { _matrix[i, j] = value; }
        }
        //public static CdMatrix operator +(CdMatrix m1, CdMatrix m2)

        //{
        //    CdMatrix m = new CdMatrix(m1.size1(), m1.size2());
        //    for (int i = 0; i < m.size1(); i++)
        //        for (int j = 0; j < m.size2(); j++)
        //        {
        //            m[i, j] = m1[i, j] + m2[i, j];
        //        }
        //    return m;
        //}
        public void print()
        {
            for (int i = 0; i < _size1; i++)
            {
                for (int j = 0; j < _size2; j++)
                {
                    Console.Write(_matrix[i, j]);
                    Console.Write(" ");
                }
                Console.WriteLine("\n");

            }
            Console.WriteLine("\n");

        }


    }
 public   class CAbos
    {
     public static   bool isNearest(int i, int j, List<XYZ> vRoundPts, int ordinalPtNumber)
        {
            double minDist = default(double);
            int ptNumber = default(int);
            for (int k = 0; k < vRoundPts.Count; k++)
            {
                if (k == 0)
                {
                    minDist = Math.Sqrt((vRoundPts[k].x - i) * (vRoundPts[k].x - i) +
                    (vRoundPts[k].y - j) * (vRoundPts[k].y - j));
                    ptNumber = k;
                }
                else
                {
                    if (minDist > Math.Sqrt((vRoundPts[k].x - i) * (vRoundPts[k].x - i) +
                    (vRoundPts[k].y - j) * (vRoundPts[k].y - j)))
                    {
                        minDist = Math.Sqrt((vRoundPts[k].x - i) * (vRoundPts[k].x - i) +
                    (vRoundPts[k].y - j) * (vRoundPts[k].y - j));
                        ptNumber = k;
                    }

                }


            }
            if (ordinalPtNumber == ptNumber) return true;
            else return false;
        }
        public static bool isInitMatrix(CiMatrix p)
        {
            for (int i = 0; i < p.size1(); i++)
            {
                for (int j = 0; j < p.size2(); j++)
                {
                    if (p[i, j] == -1) return false;
                }
            }
            return true;
        }
        static public double Round(double value, double precision)
        {
            if (value > 0)
            {
                double ost = value % precision;
                double multi_precision = Math.Floor(value / precision);
                if (ost >= precision / 2) multi_precision++;
                return precision * multi_precision;
            }
            else
            {
                double ost = value % precision;
                double multi_precision = Math.Ceiling(value / precision);
                if (Math.Abs(ost) >= precision / 2) multi_precision++;
                return precision * multi_precision;

            }
        }

        public static XYZ roundPointForMatrix(XYZ point, double dx, double dy, int Kmax)
        {
            XYZ roundPt=new XYZ();
            //roundPt.iZPointNumber = point.iZPointNumber;
            roundPt.x = Round(point.x / dx, 1) + Kmax;
            roundPt.y = Round(point.y / dy, 1) + Kmax;
            roundPt.z = point.z;
            return roundPt;


        }


     public static   CdMatrix getAbosMatrix
(List<XYZ> pts,
 double x1, double x2,
 double y1, double y2,
 double dx, double dy,
 double delta)
        {
            //Вектор z координат точек
            List<double> Z = new List<double>();
            for (int i = 0; i < pts.Count; i++)
                Z.Add(pts[i].z);

            List<double> DZ = new List<double>();
            for (int i = 0; i < pts.Count; i++)
                DZ.Add(pts[i].z);
            //DZ = Z;



            //Минимальная и максимальная координаты x
            //double x1 = 0.0;
            //double x2 = 4.0;
            //Минимальная и максимальная координаты y
            //double y1 = 0.0;
            //double y2 = 4.0;
            //Шаг сетки по x и y направлениям
            //double dx = .5;
            //double dy = .5;
            //Количество узлов сетки вдоль x и y
            int i1 = (int)((x2 - x1) / dx + 1);
            int j1 = (int)((y2 - y1) / dy + 1);
            //Матрица расстояний узла до ближайшей точки в единицах сетки для определения Kmax
            CiMatrix K = new CiMatrix(i1, j1);

            //Минимальная и максимальная координата z
            double z1 = 0;
            for (int i = 0; i < Z.Count; i++)
            {
                if (i == 0) z1 = Z[i];
                else
                if (Z[i] < z1) z1 = Z[i];
            }

            double z2 = 0;
            for (int i = 0; i < Z.Count; i++)
                if (Z[i] > z2) z2 = Z[i];

            double dzMax = z2;



            //////////////////////////////////////////////////////////////////////////
            //Заполнение матрицы расстояний начальными значениями -1
            for (int i = 0; i < i1; i++)
            {
                for (int j = 0; j < j1; j++)
                {
                    K[i, j] = -1;
                }
            }
            ////Вектор центров волн
            //vector<XYZ> vRoundPts;
            //vRoundPts.push_back(roundPointForMatrix(pt1, dx, dy, 0));
            //vRoundPts.push_back(roundPointForMatrix(pt2, dx, dy, 0));
            //vRoundPts.push_back(roundPointForMatrix(pt3, dx, dy, 0));

            //Вектор центров волн
            List<XYZ> vRoundPts=new List<XYZ>();
            for (int i = 0; i < pts.Count; i++)
            {
                pts[i].x = pts[i].x - x1;
                pts[i].y = pts[i].y - y1;
                vRoundPts.Add(roundPointForMatrix(pts[i], dx, dy, 0));
            }



            int CI = 0;
            for (CI = 0; !isInitMatrix(K); CI++)
            {
                for (int i = 0; i < vRoundPts.Count; i++)
                {
                    int x = (int)vRoundPts[i].x;
                    int y = (int)vRoundPts[i].y;
                    for (int r = 0; r <= CI; r++)
                    {

                        if (x + CI < K.size1() && y + r < K.size2())
                            if (K[x + CI, y + r] == -1 && isNearest(x + CI, y + r, vRoundPts, i)) { K[x + CI, y + r] = CI; }
                        if (x + CI < K.size1() && y - r >= 0)
                            if (K[x + CI, y - r] == -1 && isNearest(x + CI, y - r, vRoundPts, i)) { K[x + CI, y - r] = CI; }
                        if (x - CI >= 0 && y + r < K.size2())
                            if (K[x - CI, y + r] == -1 && isNearest(x - CI, y + r, vRoundPts, i)) { K[x - CI, y + r] = CI; }
                        if (x - CI >= 0 && y - r >= 0)
                            if (K[x - CI, y - r] == -1 && isNearest(x - CI, y - r, vRoundPts, i)) { K[x - CI, y - r] = CI; }
                        if (x + r < K.size1() && y + CI < K.size2())
                            if (K[x + r, y + CI] == -1 && isNearest(x + r, y + CI, vRoundPts, i)) { K[x + r, y + CI] = CI; }
                        if (x - r >= 0 && y + CI < K.size2())
                            if (K[x - r, y + CI] == -1 && isNearest(x - r, y + CI, vRoundPts, i)) { K[x - r, y + CI] = CI; }
                        if (x + r < K.size1() && y - CI >= 0)
                            if (K[x + r, y - CI] == -1 && isNearest(x + r, y - CI, vRoundPts, i)) { K[x + r, y - CI] = CI; }
                        if (x - r >= 0 && y - CI >= 0)
                            if (K[x - r, y - CI] == -1 && isNearest(x - r, y - CI, vRoundPts, i)) { K[x - r, y - CI] = CI; }


                    }
                }
            }


            int Kmax = 0;
            for (int i = 0; i < i1; i++)
            {

                for (int j = 0; j < j1; j++)
                {
                    if (K[i, j] > Kmax) Kmax = K[i, j];
                }

            }
            ///////////////////////////////////////////////////////////////////////////////////////////
            i1 = i1 + 2 * Kmax;
            j1 = j1 + 2 * Kmax;
            //Матрица расстояний узла до ближайшей точки в единицах сетки
            CiMatrix _K =new CiMatrix(i1, j1);
            //Матрица порядковых номеров ближайших точек
            CiMatrix NB = new CiMatrix(i1, j1);
            //Интерполяционная матрица
            CdMatrix P = new CdMatrix(i1, j1);
            //Вспомогательная матрица
            CdMatrix DP=new CdMatrix(i1, j1);

            //Заполнение матриц начальными значениями -1 и 0
            for (int i = 0; i < i1; i++)
            {
                for (int j = 0; j < j1; j++)
                {
                    _K[i, j] = -1;
                    DP[i, j] = 0;
                }
            }
            //Вектор центров волн
            vRoundPts.Clear();
            //vRoundPts.push_back(roundPointForMatrix(pt1, dx, dy, Kmax));
            //vRoundPts.push_back(roundPointForMatrix(pt2, dx, dy, Kmax));
            //vRoundPts.push_back(roundPointForMatrix(pt3, dx, dy, Kmax));

            for (int i = 0; i < pts.Count; i++)
                vRoundPts.Add(roundPointForMatrix(pts[i], dx, dy, Kmax));


            CI = 0;
            for (CI = 0; !isInitMatrix(_K); CI++)
            {
                for (int i = 0; i < vRoundPts.Count; i++)
                {
                    int x = (int)vRoundPts[i].x;
                    int y = (int)vRoundPts[i].y;
                    for (int r = 0; r <= CI; r++)
                    {



                        if (x + CI < _K.size1() && y + r < _K.size2())
                            if (_K[x + CI, y + r] == -1 && isNearest(x + CI, y + r, vRoundPts, i)) { _K[x + CI, y + r] = CI; NB[x + CI, y + r] = i; }
                        if (x + CI < _K.size1() && y - r >= 0)
                            if (_K[x + CI, y - r] == -1 && isNearest(x + CI, y - r, vRoundPts, i)) { _K[x + CI, y - r] = CI; NB[x + CI, y - r] = i; }
                        if (x - CI >= 0 && y + r < _K.size2())
                            if (_K[x - CI, y + r] == -1 && isNearest(x - CI, y + r, vRoundPts, i)) { _K[x - CI, y + r] = CI; NB[x - CI, y + r] = i; }
                        if (x - CI >= 0 && y - r >= 0)
                            if (_K[x - CI, y - r] == -1 && isNearest(x - CI, y - r, vRoundPts, i)) { _K[x - CI, y - r] = CI; NB[x - CI, y - r] = i; }
                        if (x + r < _K.size1() && y + CI < _K.size2())
                            if (_K[x + r, y + CI] == -1 && isNearest(x + r, y + CI, vRoundPts, i)) { _K[x + r, y + CI] = CI; NB[x + r, y + CI] = i; }
                        if (x - r >= 0 && y + CI < _K.size2())
                            if (_K[x - r, y + CI] == -1 && isNearest(x - r, y + CI, vRoundPts, i)) { _K[x - r, y + CI] = CI; NB[x - r, y + CI] = i; }
                        if (x + r < _K.size1() && y - CI >= 0)
                            if (_K[x + r, y - CI] == -1 && isNearest(x + r, y - CI, vRoundPts, i)) { _K[x + r, y - CI] = CI; NB[x + r, y - CI] = i; }
                        if (x - r >= 0 && y - CI >= 0)
                            if (_K[x - r, y - CI] == -1 && isNearest(x - r, y - CI, vRoundPts, i)) { _K[x - r, y - CI] = CI; NB[x - r, y - CI] = i; }


                    }
                }
            }


            int iIter = 0;
            //while (dzMax>(z2-z1)*delta)
            do
            {
                iIter++;
                //Первичное заполнение матрицы P
                for (int i = 0; i < i1; i++)
                {
                    for (int j = 0; j < j1; j++)
                    {
                        P[i, j] = DZ[NB[i, j]];
                    }
                }

               

                //Натяжение
                /* int iTensioning = 4;
                 if ((Kmax/2+2)>iTensioning)iTensioning=Kmax/2+2;

                 for(int N = iTensioning; N>=1; N--){
                 for (int i=Kmax; i<i1-Kmax; i++){
                     for (int j=Kmax; j<j1-Kmax; j++){
                         int k = _K(i, j);
                         if (k > N) k=N;
                         P(i,j)=(P(i+k,j)+P(i,j+k)+P(i-k,j)+P(i,j-k))/4;
                     }
                 }
                 }*/

                int iTensioning = 4;
                if ((Kmax / 2 + 2) > iTensioning) iTensioning = Kmax / 2 + 2;

                for (int N = iTensioning; N >= 1; N--)
                {
                    for (int i = 0; i < i1; i++)
                    {
                        for (int j = 0; j < j1; j++)
                        {
                            int k = _K[i, j];
                            if (k > N) k = N;
                            /*vector<double> vSumP;
                            if((i+k)<P.size1())vSumP.push_back(P(i+k,j));
                            if((j+k)<P.size2())vSumP.push_back(P(i,j+k));
                            if((i-k)>=0)vSumP.push_back(P(i-k,j));
                            if((j-k)>=0)vSumP.push_back(P(i,j-k));
                            P(i,j)=0;
                            for(int iSum = 0; iSum<vSumP.size(); iSum++)
                                P(i,j)+=vSumP[iSum]/vSumP.size();*/
                            if ((i + k) < P.size1() && (j + k) < P.size2() && (i - k) >= 0 && (j - k) >= 0)
                                P[i, j] = (P[i + k, j] + P[i, j + k] + P[i - k, j] + P[i, j - k]) / 4;

                        }
                    }
                }

                //Линейное натяжение (степень 0)


                /* for(int N = iTensioning; N>=1; N--){
                 for (int i=Kmax; i<i1-Kmax; i++){
                     for (int j=Kmax; j<j1-Kmax; j++){
                         int u = vRoundPts[NB(i, j)].x - i;
                         int v = vRoundPts[NB(i, j)].y - j;
                         double vec_len = sqrt((double)(u*u+v*v));
                         if(vec_len>N){double c = N/vec_len;
                         u = round(u*c);
                         v = round(v*c);
                         }
                         double L = 0.7/((0.107*Kmax-0.714)*Kmax);
                         double R = 1;
                         double Q = L*(Kmax-_K(i, j))*(Kmax-_K(i, j));

                         P(i,j)=(Q*(P(i+u,j+v)+P(i-u,j-v))+R*(P(i-v,j+u)+P(i+v,j-u)))/(2*Q+2*R);
                     }
                 }
                 }*/



                //Линейное натяжение (степень 1)


                /* for(int N = iTensioning; N>=1; N--){
                 for (int i=Kmax; i<i1-Kmax; i++){
                     for (int j=Kmax; j<j1-Kmax; j++){
                         int u = vRoundPts[NB(i, j)].x - i;
                         int v = vRoundPts[NB(i, j)].y - j;
                         double vec_len = sqrt((double)(u*u+v*v));
                         if(vec_len>N){double c = N/vec_len;
                         u = round(u*c);
                         v = round(v*c);
                                     }
                         double L = 1/((0.107*Kmax-0.714)*Kmax);
                         double R = 1;
                         double Q = L*(Kmax-_K(i, j))*(Kmax-_K(i, j));
                         P(i,j)=(Q*(P(i+u,j+v)+P(i-u,j-v))+R*(P(i-v,j+u)+P(i+v,j-u)))/(2*Q+2*R);
                     }
                 }
                 }*/

                /*for(int N = iTensioning; N>=1; N--){
                   for (int i=Kmax; i<i1-Kmax; i++){
                       for (int j=Kmax; j<j1-Kmax; j++){
                           if(_K(i, j)>0)
                           {
                           int u = vRoundPts[NB(i, j)].x - i;
                           int v = vRoundPts[NB(i, j)].y - j;
                           double vec_len = sqrt((double)(u*u+v*v));
                           if(vec_len>N){double c = N/vec_len;
                           u = round(u*c);
                           v = round(v*c);
                                       }
                           double L = 1/((0.107*Kmax-0.714)*Kmax);
                           double Q = L*(Kmax-_K(i, j))*(Kmax-_K(i, j));
                           double p1 = P(i+u,j+v);
                            double p2 = P(i-u,j-v);
                             double p3 = P(i-v,j+u);
                              double p4 = P(i+v,j-u);
                              double z_new = (Q*(P(i+u,j+v)+P(i-u,j-v))+P(i-v,j+u)+P(i+v,j-u))/(2*Q+2);
                           P(i,j)=(Q*(P(i+u,j+v)+P(i-u,j-v))+P(i-v,j+u)+P(i+v,j-u))/(2*Q+2);
                           }

                       }
                   }
                   }*/

                //Линейное натяжение (степень 2)


                /* for(int N = iTensioning; N>=1; N--){
                 for (int i=Kmax; i<i1-Kmax; i++){
                     for (int j=Kmax; j<j1-Kmax; j++){
                         int u = vRoundPts[NB(i, j)].x - i;
                         int v = vRoundPts[NB(i, j)].y - j;
                         double vec_len = sqrt((double)(u*u+v*v));
                         if(vec_len>N){double c = N/vec_len;
                         u = round(u*c);
                         v = round(v*c);
                         }
                         double L = 1/(0.0360625*Kmax+0.192);
                         double R = 1;
                         double Q = L*(Kmax-_K(i, j));

                         P(i,j)=(Q*(P(i+u,j+v)+P(i-u,j-v))+R*(P(i-v,j+u)+P(i+v,j-u)))/(2*Q+2*R);
                     }
                 }
                 }*/

                //Линейное натяжение (степень 3)


                /* for(int N = iTensioning; N>=1; N--){
                 for (int i=Kmax; i<i1-Kmax; i++){
                     for (int j=Kmax; j<j1-Kmax; j++){
                         int u = vRoundPts[NB(i, j)].x - i;
                         int v = vRoundPts[NB(i, j)].y - j;
                         double vec_len = sqrt((double)(u*u+v*v));
                         if(vec_len>N){double c = N/vec_len;
                         u = round(u*c);
                         v = round(v*c);
                         }
                         double R = 0;
                         double Q = 1;

                         P(i,j)=(Q*(P(i+u,j+v)+P(i-u,j-v))+R*(P(i-v,j+u)+P(i+v,j-u)))/(2*Q+2*R);
                     }
                 }
                 }*/

                //for(int N = iTensioning; N>=1; N--){
                //	 for (int i=0; i<i1; i++){
                //		 for (int j=0; j<j1; j++){
                //			 int u = vRoundPts[NB(i, j)].x - i;
                //			 int v = vRoundPts[NB(i, j)].y - j;
                //			 double vec_len = sqrt((double)(u*u+v*v));
                //			 if(vec_len>N){double c = N/vec_len;
                //			 u = round(u*c);
                //			 v = round(v*c);
                //			 }
                //			 double R = 0;
                //			 double Q = 1;
                //			if((i+u)<P.size1() && (j+u)<P.size2() && (i-u)>=0 && (j-u)>=0 &&
                //			   (i+v)<P.size1() && (j+v)<P.size2() && (i-v)>=0 && (j-v)>=0 &&
                //			   (i+u)>=0 && (j+u)>=0 && (i-u)<P.size1() && (j-u)<P.size2() &&
                //			   (i+v)>=0 && (j+v)>=0 && (i-v)<P.size1() && (j-v)<P.size2())
                //			 P(i,j)=(Q*(P(i+u,j+v)+P(i-u,j-v))+R*(P(i-v,j+u)+P(i+v,j-u)))/(2*Q+2*R);
                //		 }
                //	 }
                //	 }

                //Сглаживание
                /* iTensioning =4;
                 if(Kmax*Kmax/16>iTensioning)iTensioning=Kmax*Kmax/16;

                 for(int N = iTensioning; N>=1; N--){
                for (int i=Kmax; i<i1-Kmax; i++){
                    for (int j=Kmax; j<j1-Kmax; j++){
                        double q = 0.5;
                        double t=0;
                        if(N==iTensioning)t=0;
                        else
                            for (int k=1;k<=2;k++){
                        t=t+(P(i,j)-P(i+k,j))*(P(i,j)-P(i+k,j))+
                            (P(i,j)-P(i+k,j+k))*(P(i,j)-P(i+k,j+k))+
                            (P(i,j)-P(i+k,j-k))*(P(i,j)-P(i+k,j-k))+
                            (P(i,j)-P(i,j+k))*(P(i,j)-P(i,j+k))+
                            (P(i,j)-P(i,j-k))*(P(i,j)-P(i,j-k))+
                            (P(i,j)-P(i-k,j))*(P(i,j)-P(i-k,j))+
                            (P(i,j)-P(i-k,j+k))*(P(i,j)-P(i-k,j+k))+
                            (P(i,j)-P(i-k,j-k))*(P(i,j)-P(i-k,j-k));
                            }


                    P(i,j)=(P(i-1,j-1)+P(i-1,j)+P(i-1,j+1)+
                            P(i,  j-1)+P(i,  j)+P(i,  j+1)+
                            P(i+1,j-1)+P(i+1,j)+P(i+1,j+1)+
                            P(i,j)*(q*t-1))/(q*t+8);

                    }
                }
                }*/
               
                iTensioning = 4;
                if (Kmax * Kmax / 16 > iTensioning) iTensioning = Kmax * Kmax / 16;

                for (int N = iTensioning; N >= 1; N--)
                {
                    for (int i = 0; i < i1; i++)
                    {
                        for (int j = 0; j < j1; j++)
                        {
                            double q = 0.5;
                            double t = 0;
                            if (N == iTensioning) t = 0;
                            else
                            {

                                for (int k = 1; k <= 2; k++)
                                {
                                    if ((i + k) < P.size1() && (j + k) < P.size2() && (i - k) >= 0 && (j - k) >= 0)
                                        t = t + (P[i, j] - P[i + k, j]) * (P[i, j] - P[i + k, j]) +
                                            (P[i, j] - P[i + k, j + k]) * (P[i, j] - P[i + k, j + k]) +
                                            (P[i, j] - P[i + k, j - k]) * (P[i, j] - P[i + k, j - k]) +
                                            (P[i, j] - P[i, j + k]) * (P[i, j] - P[i, j + k]) +
                                            (P[i, j] - P[i, j - k]) * (P[i, j] - P[i, j - k]) +
                                            (P[i, j] - P[i - k, j]) * (P[i, j] - P[i - k, j]) +
                                            (P[i, j] - P[i - k, j + k]) * (P[i, j] - P[i - k, j + k]) +
                                            (P[i, j] - P[i - k, j - k]) * (P[i, j] - P[i - k, j - k]);
                                }
                            }

                            if ((i + 1) < P.size1() && (j + 1) < P.size2() && (i - 1) >= 0 && (j - 1) >= 0)
                                P[i, j] = (P[i - 1, j - 1] + P[i - 1, j] + P[i - 1, j + 1] +
                                        P[i, j - 1] + P[i, j] + P[i, j + 1] +
                                        P[i + 1, j - 1] + P[i + 1, j] + P[i + 1, j + 1] +
                                        P[i, j] * (q * t - 1)) / (q * t + 8);

                        }
                    }
                }

                //P.print();
                //DP.print();

                //P = P + DP;
                for(int i=0;i<P.size1();i++)
                {
                    for(int j=0;j<P.size2();j++)
                    {
                        P[i, j] += DP[i, j];
                    }
                }
                //P.print();

                for (int i = 0; i < vRoundPts.Count; i++)
                {
                    DZ[i] = Z[i] - P[(int)vRoundPts[i].x, (int)vRoundPts[i].y];
                }
                
                double dzMax_i = 0;
                for (int i = 0; i < DZ.Count; i++)
                    if (DZ[i] > dzMax_i) dzMax_i = DZ[i];

                dzMax = dzMax_i;
                for (int i = 0; i < DP.size1(); i++)
                {
                    for (int j = 0; j < DP.size2(); j++)
                    {
                         DP[i, j]= P[i, j];
                    }
                }

                //DP = P;
            }//while
            while (dzMax > (z2 - z1) * delta);

            /* for (int i=Kmax; i<i1-Kmax; i++){
        cout << i-Kmax << "_____";
             for (int j=Kmax; j<j1-Kmax; j++){
                 cout << P(i,j) << " ";
             }
             cout << "\n";
         }*/



            CdMatrix P_return=new CdMatrix (i1 - 2 * Kmax, j1 - 2 * Kmax);

            for (int i = Kmax; i < i1 - Kmax; i++)
            {

                for (int j = Kmax; j < j1 - Kmax; j++)
                {
                    P_return[i - Kmax, j - Kmax] = P[i, j];

                }
            }

            /*for (int i=0; i<P_return.size1(); i++){
                cout << i << "_____";

                    for (int j=0; j<P_return.size2(); j++){

                        cout << P_return(i,j) << " ";

                    }
                     cout << "\n";
                }
            cout << "Iter " << iIter << "\n";*/

            return P_return;
            //return P;

        }
        //Первоначальный вариант getPointFromAbosMatrix
        XYZ getPointFromAbosMatrixRound(double x, double y,
                                    double x1, double x2,
                                    double y1, double y2,
                                    double dx, double dy,
                                    CdMatrix AbosMatrix)
        {
            x = x - x1;
            y = y - y1;
            XYZ pt_ret= new XYZ(x, y, 0);
            XYZ pt = roundPointForMatrix(pt_ret, dx, dy, 0);
            pt_ret.z = AbosMatrix[(int)pt.x, (int)pt.y];
            return pt_ret;
        }

        //получение только Z
        public static double getPointFromAbosMatrixRoundZ(double x, double y,
                                    double x1, double x2,
                                    double y1, double y2,
                                    double dx, double dy,
                                    CdMatrix AbosMatrix)
        {
            x = x - x1;
            y = y - y1;
            XYZ pt_ret = new XYZ(x, y, 0);
            XYZ pt = roundPointForMatrix(pt_ret, dx, dy, 0);

            return AbosMatrix[(int)pt.x, (int)pt.y];
        }

        //bilinear interpolation
        XYZ getPointFromAbosMatrix(double x, double y,
                                    double x1, double x2,
                                    double y1, double y2,
                                    double dx, double dy,
                                    CdMatrix AbosMatrix)
        {
            XYZ pt_ret = new XYZ(x, y, 0);
            x = x - x1;
            y = y - y1;

            XYZ point= new XYZ(x, y, 0);
            double x0 = point.x / dx;
            double y0 = point.y / dy;

            double x10 = Math.Floor(x0);
            double x20 = Math.Ceiling(x0);
            double y10 = Math.Floor(y0);
            double y20 = Math.Ceiling(y0);

            double Q11 = AbosMatrix[(int)x10, (int)y10];
            double Q12 = AbosMatrix[(int)x10, (int)y20];
            double Q22 = AbosMatrix[(int)x20, (int)y20];
            double Q21 = AbosMatrix[(int)x20, (int)y10];

            double R1, R2;
            if (x10 != x20)
            {
                R1 = ((x20 - x0) / (x20 - x10)) * Q11 + ((x0 - x10) / (x20 - x10)) * Q21;
                R2 = ((x20 - x0) / (x20 - x10)) * Q12 + ((x0 - x10) / (x20 - x10)) * Q22;
            }
            else { R1 = Q11; R2 = Q12; }
            double P;
            if (y10 != y20) P = ((y20 - y0) / (y20 - y10)) * R1 + ((y0 - y10) / (y20 - y10)) * R2;
            else P = R1;

            pt_ret.z = P;


            return pt_ret;
        }

    }
}