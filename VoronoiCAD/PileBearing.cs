using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoronoiCAD
{
    class PileBearing
    {
        public static double? getFi73_clay(double z, double IL)
        {
            //Табл. 7.3 СП 24 Глинистые грунты
            double[,] table73_Clay = new double[14, 10]
              {
                    { -1,   0.2,  0.3,  0.4,  0.5,  0.6,  0.7,  0.8, 0.9, 1.0} ,
                    {  1,    35,   23,   15,   12,    8,    4,    4,   3,   2} ,
                    {  2,    42,   30,   21,   17,   12,    7,    5,   4,   4} ,
                    {  3,    48,   35,   25,   20,   14,    8,    7,   6,   5} ,
                    {  4,    53,   38,   27,   22,   16,    9,    8,   7,   5} ,
                    {  5,    56,   40,   29,   24,   17,   10,    8,   7,   6} ,
                    {  6,    58,   42,   31,   25,   18,   10,    8,   7,   6} ,
                    {  8,    62,   44,   33,   26,   19,   10,    8,   7,   6} ,
                    { 10,    65,   46,   34,   27,   19,   10,    8,   7,   6} ,
                    { 15,    72,   51,   38,   28,   20,   11,    8,   7,   6} ,
                    { 20,    79,   56,   41,   30,   20,   12,    8,   7,   6} ,
                    { 25,    86,   61,   44,   32,   20,   12,    8,   7,   6} ,
                    { 30,    93,   66,   47,   34,   21,   12,    9,   8,   7} ,
                    { 35,   100,   70,   50,   36,   22,   13,    9,   8,   7}

              };
            return DoubleInterp(table73_Clay, z, IL);

        }
        public static double? getR72_clay(double z, double IL)
        {
            //Табл. 7.2 СП 24 Глинистые грунты
            double[,] table72_Clay = new double[9, 8]
               {
                    { -1,   0,    0.1,  0.2,  0.3,  0.4,  0.5,  0.6} ,
                    { 5, 8800,   6200, 4000, 2800, 2000, 1300,  800} ,
                    { 7, 9700,   6900, 4300, 3300, 2200, 1400,  850} ,
                    {10, 10500,  7300, 5000, 3500, 2400, 1500,  900} ,
                    {15, 11700,  7500, 5600, 4000, 2900, 1650, 1000} ,
                    {20, 12600,  8500, 6200, 4500, 3200, 1800, 1100} ,
                    {25, 13400,  9000, 6800, 5200, 3500, 1950, 1200} ,
                    {30, 14200,  9500, 7400, 5600, 3800, 2100, 1300} ,
                    {35, 15000, 10000, 8000, 6000, 4100, 2250, 1400}
               };
            return DoubleInterp(table72_Clay, z, IL);

        }

        public static double? DoubleInterp(double[,] InArray, double arg_vertic, double arg_goris)
        {



            int num_of_rows = InArray.GetUpperBound(0);
            int num_of_cols = InArray.GetUpperBound(1);

            if (arg_vertic >= InArray[1, 0] &&
                arg_goris >= InArray[0, 1] &&
                arg_vertic <= InArray[num_of_rows, 0] &&
                arg_goris <= InArray[0, num_of_cols]
                )
            {
                long i = 1;
                double x1, x2;
                do
                {
                    x1 = InArray[i, 0];
                    x2 = InArray[i + 1, 0];
                    i++;

                } while (!(arg_vertic >= x1 && arg_vertic <= x2));



                long j = 1;
                double y1, y2;
                do
                {
                    y1 = InArray[0, j];
                    y2 = InArray[0, j + 1];
                    j++;
                } while (!(arg_goris >= y1 && arg_goris <= y2));

                i--;
                j--;
                double a1 = InArray[i, j];
                double a2 = InArray[i, j + 1];
                double a3 = InArray[i + 1, j];
                double a4 = InArray[i + 1, j + 1];

                double res1 = 0, res2 = 0;
                if (x2 - x1 > 0)
                {
                    res1 = a1 + (a3 - a1) * (arg_vertic - x1) / (x2 - x1);
                    res2 = a2 + (a4 - a2) * (arg_vertic - x1) / (x2 - x1);
                }

                if (y2 - y1 > 0)
                    return res1 + (res2 - res1) * (arg_goris - y1) / (y2 - y1);
            }


            return null;

        }

        public static double? DoubleInterp1(double[,] InArray, double arg_goris)
        {



            int num_of_rows = InArray.GetUpperBound(0);
            int num_of_cols = InArray.GetUpperBound(1);

            if (
                arg_goris >= InArray[0, 0] &&

                arg_goris <= InArray[0, num_of_cols]
                )
            {




                long j = 1;
                double y1, y2;
                do
                {
                    y1 = InArray[0, j];
                    y2 = InArray[0, j + 1];
                    j++;
                } while (!(arg_goris >= y1 && arg_goris <= y2));


                j--;
                double a1 = InArray[1, j];
                double a2 = InArray[1, j + 1];

                return a1 + (a2 - a1) * (arg_goris - y1) / (y2 - y1);
            }


            return null;

        }
    }
}
