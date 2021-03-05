using System;
using AlternativeAlgorithm;
using Nito.AsyncEx;
using System.Linq;

namespace TesterAAConstants
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("------- Alternative algorithm hybrid mode tester --------");
            AsyncContext.Run(() => MainAsync(args));
        }
        static async void MainAsync(string[] args)
        {
            double[,] locations = new double[,] {
                                           {15.969988, 45.803403 }, // longitude latitude ZAgreb 
                                           {16.834784, 45.901936}, // Bjelovar
                                           {16.355739, 48.200272 }, // Beč
                                           {18.214190, 46.070907}, // Pecs
                                           {15.2314, 44.1194}}; // Zadar
            Node[] nodes = new Node[locations.GetLength(0)];
            for (int i = 0; i < locations.GetLength(0); ++i)
            {
                nodes[i] = new Node
                {
                    Latitude = locations[i, 1],
                    Longitude = locations[i, 0],
                    Index = i
                };
            }

            Solvers solvers = new Solvers(nodes);
            long[,] haversine = await solvers.CalculateDistanceMatrix(Solvers.MODE_HAVERSINE);
            int dM = 319277; // max distance
            int mode = Solvers.MODE_HYBRID;
            //int[] M = { 3, 4 };
            //double[] K = { 0.1 * dM, dM };
            //double[] k = { 0.1, 0.4, 0.7 };
            //double[] p = { 0.2 };
            //foreach ((int M_, double K_, double k_, double p_) in M.SelectMany(M_ => K.SelectMany(K_ => k.SelectMany(k_ => p.Select(p_ => (M_, K_, k_, p_))))))
            //{
            //    long[,] hybrid = await solvers.CalculateDistanceMatrix(mode, M_, K_, k_, p_);
            //    Console.WriteLine("-- M,K,k,p = {0},{1}dM,{2}dM,{3} ------------------", M_, K_ / dM, k_, p_);
            //    MatrixEquality(hybrid, haversine);
            //}

            int M_1 = 3;
            double K_1 = 0.5 * dM;
            double k_1 =  0.4;
            double p_1 =  0.2 ;

            long[,] hybrid = await solvers.CalculateDistanceMatrix(mode);
            Console.WriteLine("-- M,K,k,p = {0},{1}dM,{2}dM,{3} ------------------", M_1, K_1 / dM, k_1, p_1);
            MatrixEquality(hybrid, haversine);
        }

        public static void MatrixEquality(long[,] arr, long[,] arr2)
        {
            int rowLength = arr.GetLength(0);
            int colLength = arr.GetLength(1);
            int numberOfSameElements = 0;
            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < colLength; j++)
                {
                    //if (i > j) continue;
                    if((i < j ) && (arr[i, j] != arr2[i, j])) ++numberOfSameElements;
                    //Console.Write(string.Format("{0} ", arr[i, j] == arr2[i, j]));
                }
                //Console.Write(Environment.NewLine + Environment.NewLine);
            }
            Console.WriteLine("Broj promijenjenih udaljenosti: {0}", numberOfSameElements);
            //Console.WriteLine();
        }
    }
}
