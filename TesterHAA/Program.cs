using System;
using AlternativeAlgorithm;
using Nito.AsyncEx;

namespace TesterAA
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncContext.Run(() => MainAsync(args));
        }
        static async void MainAsync(string[] args)
        {
            Console.WriteLine("Hello World!");
            double[,] locations = new double[,] {
                                           {15.969988, 45.803403 }, // longitude latitude ZAgreb 
                                           {16.834784, 45.901936}, // Bjelovar
                                           {16.355739, 48.200272 }, // Beč
                                           {18.214190, 46.070907}, // Pecs
                                           {15.2314, 44.1194}}; // Zadar
            Node[] nodes = new Node[locations.GetLength(0)];
            for (int i = 0; i < locations.GetLength(0); ++i)
            {
                nodes[i] = new Node();
                nodes[i].Latitude = locations[i, 1];
                nodes[i].Longitude = locations[i, 0];
                nodes[i].Index = i;
            }
            Solvers solvers = new Solvers(nodes);
            long[,] matrixHYB = await solvers.CalculateDistanceMatrix(Solvers.MODE_HYBRID);
            long[,] matrixHAV = await solvers.CalculateDistanceMatrix(Solvers.MODE_HAVERSINE);
            long[,] matrixORS = await solvers.CalculateDistanceMatrix(Solvers.MODE_ORS);
            MatrixEquality(matrixHYB, matrixHAV);
            Console.WriteLine("Hybrid:");
            PrintMatrix(matrixHYB);
            Console.WriteLine("Haversine:");
            PrintMatrix(matrixHAV);
            Console.WriteLine("Open Route Service:");
            PrintMatrix(matrixORS);
            Console.WriteLine("Yo");
        }

        public static void PrintMatrix(long[,] arr)
        {
            int rowLength = arr.GetLength(0);
            int colLength = arr.GetLength(1);

            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < colLength; j++)
                {
                    Console.Write(string.Format("{0} ", arr[i, j]));
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
        }
        public static void MatrixEquality(long[,] arr, long[,] arr2)
        {
            int rowLength = arr.GetLength(0);
            int colLength = arr.GetLength(1);

            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < colLength; j++)
                {
                    Console.Write(string.Format("{0} ", arr[i, j] == arr2[i, j]));
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
        }
    }
}
